"""
Shared helpers for Winwing CDU scripts that mirror an X-Plane aircraft's CDU/FMC
display onto Winwing CDU hardware via MobiFlight's websocket bridge.

In order to support multiple CDU devices seamlessly, a dynamic approach is taken whereby an enum class is defined that contains the supported devices.
A device is considered "supported" if it exists in the aircraft. Some aircraft have 3 CDUs while others have 2.

Upon script start, MobiFlight is probed (get_available_devices()) to detect the devices connected to the PC. Any device that returns a successful response is then tracked.

Two tasks are started independently for each available CDU device.
1. handle_dataref_updates -> Listens to X-Plane's WebSocket server for dataref updates for that specific CDU and pushes an event to a queue
2. handle_device_update   -> Listens to the queue and dispatches updates to MobiFlight to update that CDU

Tasks are started independently for each CDU device to ensure each device can update quickly, particularly when players might be performing shared cockpit flights.

Upon a failed connection while dispatching updates to MobiFlight, the handle_device_update function uses `async for` with the websockets client. The failed message is put back in the queue, the loop continues to the next iteration which then reconnects again.
The failed message is picked back up and dispatched to MobiFlight. This ensures a user's device eventually receives the updated display contents and doesn't hang which would require the user to cycle the page again.

Scope: X-Plane-targeting Winwing CDU scripts only. MSFS/SimConnect-based scripts
are explicitly out of scope and should not import this module.
"""

import asyncio
import base64
import json
import logging
import os
import urllib.request
from typing import Callable, Optional

import websockets

CDU_COLUMNS = 24
CDU_ROWS = 14
CDU_CELLS = CDU_COLUMNS * CDU_ROWS

WEBSOCKET_HOST = "localhost"
WEBSOCKET_PORT = 8320

BASE_REST_URL = "http://localhost:8086/api/v2/datarefs"
BASE_WEBSOCKET_URI = f"ws://{WEBSOCKET_HOST}:8086/api/v2"

WS_CAPTAIN = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-captain"
WS_CO_PILOT = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-co-pilot"
WS_OBSERVER = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-observer"


class CduDeviceMixin:
    """
    Mix into a per-aircraft `class CduDevice(CduDeviceMixin, StrEnum): ...`.

    Dispatches get_endpoint() purely on the enum member's NAME ("Captain" /
    "CoPilot" / "Observer"), never on its VALUE, which each aircraft script
    repurposes as its own dataref-namespace prefix.
    """

    _ENDPOINT_BY_NAME = {
        "Captain": WS_CAPTAIN,
        "CoPilot": WS_CO_PILOT,
        "Observer": WS_OBSERVER,
    }

    def get_endpoint(self) -> str:
        try:
            return self._ENDPOINT_BY_NAME[self.name]
        except KeyError:
            raise KeyError(f"Invalid device specified {self}") from None


def fetch_dataref_mapping(
    name_filter: Callable[[str], bool], strip_names: bool = False
) -> dict[int, str]:
    """
    Fetches the id/name dataref mapping from X-Plane's REST API, keeping only
    the datarefs for which `name_filter` returns True.
    """
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)

        filtered = filter(
            lambda x: name_filter(str(x["name"])), response_json["data"]
        )

        if strip_names:
            return {int(d["id"]): str(d["name"]).strip() for d in filtered}

        return {int(d["id"]): str(d["name"]) for d in filtered}


def default_decode_value(dataref_name: str, value):
    """
    Base64-decodes and denulls a string payload; passes any other payload
    (e.g. a raw int from a numeric/array dataref) through unchanged.
    """
    return (
        base64.b64decode(value).decode().replace("\x00", " ")
        if isinstance(value, str)
        else value
    )


async def handle_dataref_updates(
    queue: asyncio.Queue,
    device,
    dataref_map: dict[int, str],
    decode_value: Callable[[str, object], object] = default_decode_value,
) -> None:
    last_known_values = {}

    logging.info("Connecting to X-Plane websocket server")
    async for websocket in websockets.connect(BASE_WEBSOCKET_URI):
        logging.info("Connected successfully to X-Plane websocket server")
        try:
            await websocket.send(
                json.dumps(
                    {
                        "type": "dataref_subscribe_values",
                        "req_id": 1,
                        "params": {
                            "datarefs": [
                                {"id": id_value} for id_value in dataref_map.keys()
                            ]
                        },
                    }
                )
            )
            while True:
                message = await websocket.recv()
                data = json.loads(message)

                if "data" not in data:
                    continue

                new_values = dict(last_known_values)

                for dataref_id, value in data["data"].items():
                    dataref_id = int(dataref_id)
                    if dataref_id not in dataref_map:
                        continue

                    dataref_name = dataref_map[dataref_id]
                    new_values[dataref_name] = decode_value(dataref_name, value)

                if new_values == last_known_values:
                    continue

                last_known_values = new_values
                await queue.put(new_values)
        except websockets.exceptions.ConnectionClosed:
            logging.error(
                "X-Plane websocket connection was closed... Attempting to reconnect"
            )
            continue


async def handle_device_update(
    queue: asyncio.Queue,
    device,
    generate_display_json: Callable[[object, dict], str],
    rate_limit_time: float = 0.1,
) -> None:
    """
    Translates and sends dataref updates to MobiFlight.
    """
    last_run_time = 0

    endpoint = device.get_endpoint()
    logging.info("Connecting to CDU device %s", device)
    async for websocket in websockets.connect(endpoint):
        logging.info("Connected successfully to CDU device %s", device)
        while True:
            values = await queue.get()

            try:
                elapsed = asyncio.get_running_loop().time() - last_run_time

                # Weaker CPUs may experience performance issues when a websocket connection is saturated with requests, such as when pages are frequently changed.
                # This rate limits the number of active websocket requests to MobiFlight.
                # The delay should not be noticeable unless a user heavily spams page changes, but it should be enough that too many messages won't be pushed at once.
                if elapsed < rate_limit_time:
                    await asyncio.sleep(rate_limit_time - elapsed)

                display_json = generate_display_json(device, values)
                await websocket.send(display_json)
                last_run_time = asyncio.get_running_loop().time()

            except websockets.exceptions.ConnectionClosed:
                logging.error(
                    "MobiFlight websocket connection was closed... Attempting to reconnect"
                )
                await queue.put(values)
                break


async def get_available_devices(device_enum_cls, font_name: Optional[str] = None) -> list:
    """
    Probes MobiFlight for each candidate device. If `font_name` is given, a
    device is only kept in the available list once its font-switch request
    succeeds; a device that fails the font request is excluded entirely. If
    `font_name` is None, no font traffic is sent and a device is kept as soon
    as the connection succeeds.
    """
    available_devices = []
    font_request = (
        json.dumps({"Target": "Font", "Data": font_name}) if font_name else None
    )

    logging.info("Checking MobiFlight for available CDU devices")
    for device in device_enum_cls:
        device_endpoint = device.get_endpoint()
        try:
            async with websockets.connect(device_endpoint) as socket:
                logging.info(
                    "Discovered CDU device %s at endpoint %s", device, device_endpoint
                )

                if font_request:
                    try:
                        await socket.send(font_request)
                        await asyncio.sleep(1)  # wait a second for font to be set
                    except websockets.ConnectionClosed as e:
                        logging.warning(
                            "Attempt to change font on CDU device %s but request failed: %s",
                            device,
                            e,
                        )
                        continue

                available_devices.append(device)
        except websockets.WebSocketException:
            logging.warning(
                "Attempted to probe CDU device %s at endpoint %s but device wasn't available",
                device,
                device_endpoint,
            )
            continue

    return available_devices


async def run(
    device_enum_cls,
    fetch_dataref_mapping: Callable[[object], dict[int, str]],
    generate_display_json: Callable[[object, dict], str],
    decode_value: Optional[Callable[[str, object], object]] = None,
    font_name: Optional[str] = None,
    rate_limit_time: float = 0.1,
) -> None:
    """
    Full orchestration for a CDU script: logging setup, device discovery,
    and spawning the dataref-update/device-update task pair for each
    available device.
    """
    logging.basicConfig(
        level=os.environ.get("LOGLEVEL", "WARNING").upper(),
        format="%(levelname)s:%(message)s",
    )

    available_devices = await get_available_devices(device_enum_cls, font_name=font_name)
    decode_fn = decode_value or default_decode_value

    tasks = []
    for device in available_devices:
        dataref_map = fetch_dataref_mapping(device)

        queue = asyncio.Queue()

        tasks.append(
            asyncio.create_task(
                handle_dataref_updates(queue, device, dataref_map, decode_fn)
            )
        )
        tasks.append(
            asyncio.create_task(
                handle_device_update(queue, device, generate_display_json, rate_limit_time)
            )
        )

    logging.info("Started background tasks for %s", available_devices)

    await asyncio.gather(*tasks)
