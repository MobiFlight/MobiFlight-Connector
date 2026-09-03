"""
Adds support for the X-Plane UNS-1.

Many X-Plane aircraft have similar formats for datarefs and the means of retrieving, translating and sending updates is mostly the same.

In order to support multiple CDU devices seamlessly, a dynamic approach is taken whereby an enum class is defined that contains the supported devices.
A device is considered "supported" if it exists in the aircraft. Some aircraft have 3 CDUs while others have 2.
Each enum member is assigned a value that is used to construct the X-Plane dataref identifier. Example: "cdu1" in "uns1/cdu1/text_line_0".

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
import urllib.request
from dataclasses import dataclass, KW_ONLY
from enum import StrEnum

import websockets

CDU_COLUMNS = 24
CDU_ROWS = 11
CDU_CELLS = CDU_COLUMNS * CDU_ROWS

WEBSOCKET_HOST = "localhost"
WEBSOCKET_PORT = 8320

BASE_REST_URL = "http://localhost:8086/api/v2/datarefs"
BASE_WEBSOCKET_URI = f"ws://{WEBSOCKET_HOST}:8086/api/v2"

WS_CAPTAIN = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-captain"
WS_CO_PILOT = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-co-pilot"

# Q4XP
COLOR_MAP_0 = {
    0: "w",
    1: "w",
    2: "g",
    3: "y",
    4: "c",
    5: "m",
    6: "g",
    7: "o",
    8: "r",
    9: "e",
    10: "w",
    11: "g",
    12: "c",
}

# ProLine
COLOR_MAP_1 = {
    0: "w",
    1: "w",
    2: "c",
    3: "y",
    4: "m",
    5: "m",
    6: "c",
    7: "y",
    8: "r",
    9: "c",
    10: "g",
    11: "g",
    12: "m",
}

# Primus
COLOR_MAP_2 = {
    0: "w",
    1: "g",
    2: "w",
    3: "y",
    4: "c",
    5: "m",
    6: "g",
    7: "y",
    8: "r",
    9: "e",
    10: "g",
    11: "g",
    12: "c",
}

# Standard
COLOR_MAP_3 = {
    0: "w",
    1: "g",
    2: "a",
    3: "y",
    4: "c",
    5: "m",
    6: "g",
    7: "r",
    8: "r",
    9: "e",
    10: "g",
    11: "g",
    12: "e",
}

COLOR_SETS = {
  0: COLOR_MAP_0,
  1: COLOR_MAP_1,
  2: COLOR_MAP_2,
  3: COLOR_MAP_3,
}

CHAR_MAP = {
    "\u24c2": "m", # Circled Latin Capital Letter M
    "─": "-",      # Box Drawings Light Horizontal
    "│": "|",      # Box Drawings Light Vertical
    "┌": "-",      # Box Drawings Light Down and Right
    "┐": "-",      # Box Drawings Light Down and Left
    "└": "-",      # Box Drawings Light Up and Right
    "┘": "-",      # Box Drawings Light Up and Left
    "├": "|",      # Box Drawings Light Vertical and Right
    "┤": "|",      # Box Drawings Light Vertical and Left
    "┬": "-",      # Box Drawings Light Down and Horizontal
    "┴": "-",      # Box Drawings Light Up and Horizontal
}

FONT_REQUEST = json.dumps({"Target": "Font", "Data": "Boeing"})

HOLD_DIR_RIGHT = 1
HOLD_DIR_LEFT = -1

HOLD_GRAPHICS_INBOUND = 0
HOLD_GRAPHICS_DIRECTION = 1
HOLD_GRAPHICS_TURN = 2
HOLD_GRAPHICS_TIME = 3
HOLD_GRAPHICS_ALL = 4

@dataclass
class Overlay:
    """An ASCII-art overlay that can be superimposed on the display.

    This is used to display the various canned graphics that the UNS-1
    displays on the holding page.

    text:
        List of one or more strings containing rows of ASCII art text.
        Any non-space character in `text` will overwrite the existing
        character on the display.

    style:
        Numeric style value to apply to the text. Anywhere that `text` contains
        a non-space character, the existing style of that display cell will be
        overwritten with `style`.

    row_offset:
        Number of rows to offset the overlay from the top of the display.
    """

    text: [str]
    _: KW_ONLY
    style: int
    row_offset: int

    def draw_onto(self, text: [str], style: [bytes]):
        for i in range(len(self.text)):
            text[i + self.row_offset] = [
                bg if fg == " " else fg
                for (bg, fg) in zip(text[i + self.row_offset], self.text[i])
            ]
            style[i + self.row_offset] = [
                bg if fg == " " else self.style
                for (bg, fg) in zip(style[i + self.row_offset], self.text[i])
            ]

HOLD_R_OVERLAY = Overlay([
    r"       ----------       ",
    r"      /          \      ",
    r"      |          |      ",
    r"      \          /      ",
    r"       -----------      ",
], style = 3, row_offset = 3)

HOLD_R_INBOUND_ARROW = Overlay([
    r"            ←-          ",
], style = 1, row_offset = 8)

HOLD_R_DIR_ARROW = Overlay([
    r"          -   →         ",
], style = 1, row_offset = 4)

HOLD_R_TURN_ARROW = Overlay([
    r"      ↑                 ",
    r"       \                ",
], style = 1, row_offset = 5)

HOLD_R_TIME_ARROWS = Overlay([
    r"        |←    →|        ",
    r"        .               ",
], style = 1, row_offset = 6)

HOLD_L_OVERLAY = Overlay([
    r"       ----------       ",
    r"      /          \      ",
    r"      |          |      ",
    r"      \          /      ",
    r"      -----------       ",
], style = 3, row_offset = 3)

HOLD_L_INBOUND_ARROW = Overlay([
    r"          -→            ",
], style = 1, row_offset = 8)

HOLD_L_DIR_ARROW = Overlay([
    r"         ←   -          ",
], style = 1, row_offset = 4)

HOLD_L_TURN_ARROW = Overlay([
    r"                 ↑      ",
    r"                /       ",
], style = 1, row_offset = 5)

HOLD_L_TIME_ARROWS = Overlay([
    r"        |←    →|        ",
    r"               .        ",
], style = 1, row_offset = 6)


class CduDevice(StrEnum):
    Captain = "cdu1"
    CoPilot = "cdu2"

    def get_endpoint(self) -> str:
        match self:
            case CduDevice.Captain:
                return WS_CAPTAIN
            case CduDevice.CoPilot:
                return WS_CO_PILOT
            case _:
                raise KeyError(f"Invalid device specified {self}")


class AircraftCduDevice:
    def __init__(self, cdu: CduDevice, q4xp: bool):
        self.cdu = cdu
        self.prefix = "FJS/Q4XP" if q4xp else "uns1"

    def get_color_set_dataref(self) -> str:
        return f"{self.prefix}/{self.cdu}/color_set"

    def get_hold_dir_dataref(self) -> str:
        return f"{self.prefix}/{self.cdu}/hold_dir"

    def get_hold_graphics_dataref(self) -> str:
        return f"{self.prefix}/{self.cdu}/hold_graphics"

    def get_text_dataref(self, line) -> str:
        return f"{self.prefix}/{self.cdu}/text_line_{line}"

    def get_style_dataref(self, line) -> str:
        return f"{self.prefix}/{self.cdu}/style_line_{line}"

    def get_dataref_prefixes(self) -> str:
        return [
            f"{self.prefix}/{self.cdu}/text_line_",
            f"{self.prefix}/{self.cdu}/style_line_",
            self.get_color_set_dataref(),
            self.get_hold_dir_dataref(),
            self.get_hold_graphics_dataref(),
        ]


def fetch_dataref_mapping(device: AircraftCduDevice):
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)

        prefixes = device.get_dataref_prefixes()

        return dict(
            map(
                lambda dataref: (int(dataref["id"]), str(dataref["name"])),
                filter(
                    lambda x: any(p in str(x["name"]) for p in prefixes),
                    response_json["data"],
                ),
            )
        )

def color_from_style(style, color_map):
    return color_map.get(style & 0xf, "w")

def size_from_style(style):
    return 0 if style & (1 << 7) else 1

def reverse_video_from_style(style):
    return 1 if style & (1 << 6) else 0

def generate_display_json(device: AircraftCduDevice, values: dict[str, str | bytes]):
    display_data = [[] for _ in range(CDU_CELLS)]

    text = [values[device.get_text_dataref(row)] for row in range(CDU_ROWS)]
    style = [values[device.get_style_dataref(row)] for row in range(CDU_ROWS)]

    color_set = values[device.get_color_set_dataref()]
    color_map = COLOR_SETS.get(color_set, COLOR_MAP_3)

    hold_dir = values[device.get_hold_dir_dataref()]
    hold_graphics = values[device.get_hold_graphics_dataref()]

    if hold_dir == HOLD_DIR_RIGHT:
        HOLD_R_OVERLAY.draw_onto(text, style)

        if hold_graphics in [HOLD_GRAPHICS_INBOUND, HOLD_GRAPHICS_ALL]:
            HOLD_R_INBOUND_ARROW.draw_onto(text, style)
        if hold_graphics in [HOLD_GRAPHICS_DIRECTION, HOLD_GRAPHICS_ALL]:
            HOLD_R_DIR_ARROW.draw_onto(text, style)
        if hold_graphics in [HOLD_GRAPHICS_TURN, HOLD_GRAPHICS_ALL]:
            HOLD_R_TURN_ARROW.draw_onto(text, style)
        if hold_graphics in [HOLD_GRAPHICS_TIME, HOLD_GRAPHICS_ALL]:
            HOLD_R_TIME_ARROWS.draw_onto(text, style)
    elif hold_dir == HOLD_DIR_LEFT:
        HOLD_L_OVERLAY.draw_onto(text, style)

        if hold_graphics in [HOLD_GRAPHICS_INBOUND, HOLD_GRAPHICS_ALL]:
            HOLD_L_INBOUND_ARROW.draw_onto(text, style)
        if hold_graphics in [HOLD_GRAPHICS_DIRECTION, HOLD_GRAPHICS_ALL]:
            HOLD_L_DIR_ARROW.draw_onto(text, style)
        if hold_graphics in [HOLD_GRAPHICS_TURN, HOLD_GRAPHICS_ALL]:
            HOLD_L_TURN_ARROW.draw_onto(text, style)
        if hold_graphics in [HOLD_GRAPHICS_TIME, HOLD_GRAPHICS_ALL]:
            HOLD_L_TIME_ARROWS.draw_onto(text, style)

    for row in range(CDU_ROWS):
        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col

            char = text[row][col]
            char = CHAR_MAP.get(char, char)

            color = color_from_style(style[row][col], color_map)
            size = size_from_style(style[row][col])
            reverse_video = reverse_video_from_style(style[row][col])

            if char == " " and reverse_video == 0:
                continue

            display_data[index] = [char, color, size, reverse_video]

    return json.dumps({"Target": "Display", "Data": display_data})


def is_q4xp() -> bool:
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)
        names = (x["name"] for x in response_json["data"])
        return "FJS/Q4XP/cdu1/cdu_model" in names

async def handle_device_update(queue: asyncio.Queue, device: AircraftCduDevice):
    """
    Translates and sends dataref updates to MobiFlight.
    """
    last_run_time = 0
    rate_limit_time = 0.1

    endpoint = device.cdu.get_endpoint()
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


async def handle_dataref_updates(queue: asyncio.Queue, device: AircraftCduDevice):
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

                    if "text_line" in dataref_name:
                        new_values[dataref_name] = base64.b64decode(value).decode(errors='replace').replace("\x00", " ")
                    elif "style_line" in dataref_name:
                        new_values[dataref_name] = base64.b64decode(value)
                    else:
                        new_values[dataref_name] = value


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
                available_devices.append(AircraftCduDevice(device, is_q4xp()))
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
        queue = asyncio.Queue()

        tasks.append(asyncio.create_task(handle_dataref_updates(queue, device)))
        tasks.append(asyncio.create_task(handle_device_update(queue, device)))

    logging.info("Started background tasks for %s", available_devices)

    await asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())
