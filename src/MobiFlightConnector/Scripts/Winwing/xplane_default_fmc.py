"""
Adds support for the default X-Plane FMS dataref family
(sim/cockpit2/radios/indicators/fms_cduN_text/style_lineX), used by the stock FMS
and by aircraft that reuse it, such as the X-Crafts ERJ family.

Many X-Plane aircraft have similar formats for datarefs and the means of retrieving, translating and sending updates is mostly the same.

In order to support multiple CDU devices seamlessly, a dynamic approach is taken whereby an enum class is defined that contains the supported devices.
A device is considered "supported" if it exists in the aircraft. Some aircraft have 3 CDUs while others have 2.
Each enum member is assigned a value that is used to construct the X-Plane dataref identifier. Example: "fms_cdu1" in "sim/cockpit2/radios/indicators/fms_cdu1_text_line0".

DYNAMIC LINE COUNT / SCRATCHPAD HANDLING:
Not every aircraft on this dataref family publishes the same number of lines. The stock/default
FMS publishes a full 14 lines (0-13), one per WinWing grid row, mapped 1:1. The X-Crafts ERJ only
publishes 9 lines (0-8): lines 0-7 are page content, and line 8 is the scratchpad / text entry
line, which on the real ERJ MCDU is shown on the bottom-most row of the screen rather than
directly beneath line 7.

Rather than hardcoding a line count and hardcoding this exception for the ERJ specifically, the
number of lines an aircraft actually publishes is discovered per-device from the datarefs
fetch_dataref_mapping() returns (build_line_row_map()). If an aircraft publishes a full 14 lines,
each one maps directly to the matching grid row, same as before. If it publishes fewer, every line
except the last maps directly to its matching grid row, and the last (the scratchpad) is mirrored
to the bottom-most grid row (CDU_ROWS - 1). This lets one script serve both the default FMS and
reduced implementations like the ERJ's without any aircraft-specific branching.

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
import os
import re
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

# Matches the trailing line number on either a text or style dataref name, e.g.
# ..._text_line3 or ..._style_line12, so the published line count can be derived
# from whatever datarefs actually exist for a given aircraft/device.
LINE_PATTERN = re.compile(r"_line(\d+)$")

COLOR_MAP = {
  0: "e",  # Grey instead of black, which doesn't exist
  1: "c",
  2: "r",
  3: "y",
  4: "g",
  5: "m",
  6: "a",
  7: "w",
}


FONT_REQUEST = json.dumps({"Target": "Font", "Data": "Boeing"})


class CduDevice(StrEnum):
    Captain = "fms_cdu1"
    CoPilot = "fms_cdu2"
    Observer = "fms_cdu3"

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
        return f"sim/cockpit2/radios/indicators/{self}"

    def get_text_dataref(self, line) -> str:
        return f"sim/cockpit2/radios/indicators/{self}_text_line{line}"

    def get_style_dataref(self, line) -> str:
        return f"sim/cockpit2/radios/indicators/{self}_style_line{line}"


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


def build_line_row_map(dataref_map: dict[int, str]) -> dict[int, int]:
    """
    Determines which WinWing grid row each FMS dataref line should be rendered on,
    derived dynamically from whichever lines the aircraft actually publishes (per
    fetch_dataref_mapping()). This lets the same script support both full 14-line
    FMS implementations, where every dataref line maps 1:1 to a grid row, and
    reduced implementations like the ERJ's 9-line FMS, where the last published
    line is a scratchpad that belongs on the bottom-most row of the display
    instead of directly after the aircraft's other content lines.
    """
    line_numbers = set()
    for name in dataref_map.values():
        match = LINE_PATTERN.search(name)
        if match:
            line_numbers.add(int(match.group(1)))

    if not line_numbers:
        return {}

    line_count = max(line_numbers) + 1

    if line_count >= CDU_ROWS:
        # Full-size FMS: every published line maps directly to the matching grid row.
        return {line: line for line in range(CDU_ROWS)}

    # Reduced FMS (e.g. the ERJ's 9-line implementation): every line except the
    # last maps directly to its matching grid row, and the last published line
    # (the scratchpad) is mirrored to the bottom row of the grid.
    content_line_count = line_count - 1
    row_map = {line: line for line in range(content_line_count)}
    row_map[content_line_count] = CDU_ROWS - 1
    return row_map


def color_from_style(style):
    # According to the documentation
    # (https://developer.x-plane.com/article/datarefs-for-the-cdu-screen/)
    # the four lowest bits encode color, but only color indexes 0 through 7
    # are defined at this point. We default to white for any color indexes that
    # currently aren't defined.
    return COLOR_MAP.get(style & 0xf, "w")


def size_from_style(style):
    return 0 if style & (1 << 7) else 1


def reverse_video_from_style(style):
    return 1 if style & (1 << 6) else 0


def render_fms_line(
    device: CduDevice,
    values: dict[str, str | bytes],
    fms_line: int,
    grid_row: int,
    display_data: list,
) -> None:
    """
    Renders a single FMS dataref line (fms_line) into a specific row of the
    WinWing's 14-row display grid (grid_row, 0-13). The FMS line and the grid row
    don't have to match - this is what lets a scratchpad line be placed on the
    bottom grid row instead of directly under the aircraft's last content line.

    If we haven't yet received both the text and style values for this FMS line
    (e.g. during the first update after connecting, before X-Plane has pushed
    every subscribed dataref), the row is simply left blank instead of raising a
    KeyError.
    """
    text_key = device.get_text_dataref(fms_line)
    style_key = device.get_style_dataref(fms_line)

    if text_key not in values or style_key not in values:
        return

    # Strings are sometimes shorter than a full line, so pad with spaces to the expected width.
    text = values[text_key].ljust(CDU_COLUMNS)
    style = values[style_key]

    # Style bytes can also arrive shorter than the text they describe; guard
    # against that so a short style buffer doesn't raise an IndexError either.
    if len(style) < CDU_COLUMNS:
        return

    for col in range(CDU_COLUMNS):
        # The dataref and WinWing both use Unicode, so no conversion
        # of special characters is necessary.
        char = text[col]
        if char == " ":
            continue

        index = grid_row * CDU_COLUMNS + col
        color = color_from_style(style[col])
        size = size_from_style(style[col])
        reverse_video = reverse_video_from_style(style[col])

        display_data[index] = [char, color, size, reverse_video]


def generate_display_json(
    device: CduDevice,
    values: dict[str, str | bytes],
    line_row_map: dict[int, int],
):
    display_data = [[] for _ in range(CDU_CELLS)]

    for fms_line, grid_row in line_row_map.items():
        render_fms_line(device, values, fms_line, grid_row, display_data)

    return json.dumps({"Target": "Display", "Data": display_data})


async def handle_device_update(
    queue: asyncio.Queue, device: CduDevice, line_row_map: dict[int, int]
):
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

                display_json = generate_display_json(device, values, line_row_map)
                await websocket.send(display_json)
                last_run_time = asyncio.get_running_loop().time()

            except websockets.exceptions.ConnectionClosed:
                logging.error(
                    "MobiFlight websocket connection was closed... Attempting to reconnect"
                )
                await queue.put(values)
                break


async def handle_dataref_updates(
    queue: asyncio.Queue, device: CduDevice, dataref_map: dict[int, str]
):
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

                    if "text_line" in dataref_name:
                        new_values[dataref_name] = base64.b64decode(value).decode().replace("\x00", " ")
                    elif "style_line" in dataref_name:
                        new_values[dataref_name] = base64.b64decode(value)

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
    logging.basicConfig(
        level=os.environ.get("LOGLEVEL", "WARNING").upper(),
        format='%(levelname)s:%(message)s'
    )

    available_devices = await get_available_devices()

    tasks = []

    for device in available_devices:
        dataref_map = fetch_dataref_mapping(device)
        line_row_map = build_line_row_map(dataref_map)

        queue = asyncio.Queue()

        tasks.append(
            asyncio.create_task(handle_dataref_updates(queue, device, dataref_map))
        )
        tasks.append(
            asyncio.create_task(handle_device_update(queue, device, line_row_map))
        )

    logging.info("Started background tasks for %s", available_devices)

    await asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())