"""
Adds support for the Laminar 747-400 FMS in X-Plane

Many X-Plane aircraft have similar formats for datarefs and the means of retrieving, translating and sending updates is mostly the same.

In order to support multiple CDU devices seamlessly, a dynamic approach is taken whereby an enum class is defined that contains the supported devices.
A device is considered "supported" if it exists in the aircraft. Some aircraft have 3 CDUs while others have 2.
Each enum member is assigned a value that is used to construct the X-Plane dataref identifier. Example: "fms1" in "laminar/B747/fms1/Line01_L".

Upon script start, MobiFlight is probed (get_available_devices()) to detect the devices connected to the PC. Any device that returns a successful response is then tracked.

Two tasks are started independently for each available CDU device.
1. handle_dataref_updates -> Listens to X-Plane's WebSocket server for dataref updates for that specific CDU and pushes an event to a queue
2. handle_device_update   -> Listens to the queue and dispatches updates to MobiFlight to update that CDU

Tasks are started independently for each CDU device to ensure each device can update quickly, particularly when players might be performing shared cockpit flights.

Upon a failed connection while dispatching updates to MobiFlight, the handle_device_update function uses `async for` with the websockets client. The failed message is put back in the queue, the loop continues to the next iteration which then reconnects again.
The failed message is picked back up and dispatched to MobiFlight. This ensures a user's device eventually receives the updated display contents and doesn't hang which would require the user to cycle the page again.
"""

import asyncio
import base64
import json
import logging
import urllib.request
from enum import StrEnum

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

CHAR_MAP = {
    "*": "☐",
    "`": "°",
}

FONT_REQUEST = json.dumps({"Target": "Font", "Data": "Boeing"})


class CduDevice(StrEnum):
    # Not a typo -- for some strange reason, the Captain's FMS is number 3.
    Captain = "fms3"
    CoPilot = "fms2"
    Observer = "fms1"

    def get_endpoint(self) -> str:
        match self:
            case CduDevice.Captain:
                return WS_CAPTAIN
            case CduDevice.CoPilot:
                return WS_CO_PILOT
            case CduDevice.Observer:
                return WS_OBSERVER
            case _:
                raise KeyError(f"Invalid device specified {self}")

    def get_dataref_prefix(self) -> str:
        return f"laminar/B747/{self}/Line"

    def get_large_text_dataref(self, line) -> str:
        return f"laminar/B747/{self}/Line{line+1:02d}_L"

    def get_small_text_dataref(self, line) -> str:
        return f"laminar/B747/{self}/Line{line+1:02d}_S"


def fetch_dataref_mapping(device: CduDevice):
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)

        return dict(
            map(
                lambda dataref: (int(dataref["id"]), str(dataref["name"])),
                filter(
                    lambda x: device.get_dataref_prefix() in str(x["name"]),
                    response_json["data"],
                ),
            )
        )


def generate_display_json(device: CduDevice, values: dict[str, str | bytes]):
    display_data = [[] for _ in range(CDU_CELLS)]

    for row in range(CDU_ROWS):
        # Strings are sometimes empty, so pad with spaces to the full expected width.
        large_text = values[device.get_large_text_dataref(row)].ljust(CDU_COLUMNS)
        small_text = values[device.get_small_text_dataref(row)].ljust(CDU_COLUMNS)

        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col

            # The dataref and WinWing both use Unicode, so no conversion
            # of special characters is necessary.
            char = large_text[col]
            size = 0
            if char == " ":
                char = small_text[col]
                size = 1
            if char == " ":
                continue

            # The datarefs sometimes contain lower-case characters, but the CDU
            # in the virtual cockpit always displays them as upper case, so we
            # do the same.
            char = char.upper()

            char = CHAR_MAP.get(char, char)

            color = "g"
            reverse_video = 0

            display_data[index] = [char, color, size, reverse_video]

    return json.dumps({"Target": "Display", "Data": display_data})


async def handle_device_update(queue: asyncio.Queue, device: CduDevice):
    """
    Translates and sends dataref updates to MobiFlight.
    """
    last_run_time = 0
    rate_limit_time = 0.1

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


async def handle_dataref_updates(queue: asyncio.Queue, device: CduDevice):
    last_known_values = {}

    dataref_map = fetch_dataref_mapping(device)
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

                    new_values[dataref_name] = base64.b64decode(value).decode(errors='replace').replace("\x00", " ")
 

                if new_values == last_known_values:
                    continue

                last_known_values = new_values
                await queue.put(new_values)
        except websockets.exceptions.ConnectionClosed:
            logging.error(
                "X-Plane websocket connection was closed... Attempting to reconnect"
            )
            continue


async def get_available_devices() -> list[CduDevice]:
    device_candidates = [device for device in CduDevice]

    available_devices = []

    logging.info("Checking MobiFlight for available CDU devices")
    for device in device_candidates:
        device_endpoint = device.get_endpoint()
        try:
            async with websockets.connect(device_endpoint) as socket:
                logging.info(
                    "Discovered CDU device %s at endpoint %s", device, device_endpoint
                )
                available_devices.append(device)
                await socket.send(FONT_REQUEST)
                await asyncio.sleep(1) # wait a second for font to be set
        except websockets.WebSocketException:
            logging.warning(
                "Attempted to probe CDU device %s at endpoint %s but device wasn't available",
                device,
                device_endpoint,
            )
            continue

    return available_devices


async def main():
    available_devices = await get_available_devices()

    tasks = []

    for device in available_devices:
        queue = asyncio.Queue()

        tasks.append(asyncio.create_task(handle_dataref_updates(queue, device)))
        tasks.append(asyncio.create_task(handle_device_update(queue, device)))

    logging.info("Started background tasks for %s", available_devices)

    await asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())
