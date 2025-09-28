import asyncio
import base64
import json
import logging
import urllib.request
import websockets
from enum import StrEnum

CDU_COLUMNS = 24
CDU_ROWS = 14
CDU_CELLS = CDU_COLUMNS * CDU_ROWS

WEBSOCKET_HOST = "localhost"
WEBSOCKET_PORT = 8320

BASE_REST_URL = "http://localhost:8086/api/v2/datarefs"
BASE_WEBSOCKET_URI = f"ws://{WEBSOCKET_HOST}:8086/api/v2"

WS_CAPTAIN = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-captain"
WS_COPILOT = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-co-pilot"

BALLOT_BOX = "☐"
DEGREES = "°"

CHAR_MAP = {"$": BALLOT_BOX, "`": DEGREES}
COLOR_MAP = {1: "g", 2: "g", 4: "e", 5: "g"}

FONT_REQUEST = json.dumps({"Target": "Font", "Data": "Boeing"})


class CduDevice(StrEnum):
    Captain = "captain"
    Copilot = "copilot"

    def get_endpoint(self) -> str:
        match self:
            case CduDevice.Captain:
                return WS_CAPTAIN
            case CduDevice.Copilot:
                return WS_COPILOT
            case _:
                raise KeyError(f"Invalid device specified {self}")

    def get_content_dataref(self, i: int) -> str:
        if self == CduDevice.Captain:
            return f"Rotate/aircraft/controls/cdu_0/mcdu_line_{i}_content"
        elif self == CduDevice.Copilot:
            return f"Rotate/aircraft/controls/cdu_1/mcdu_line_{i}_content"

    def get_style_dataref(self, i: int) -> str:
        if self == CduDevice.Captain:
            return f"Rotate/aircraft/controls/cdu_0/mcdu_line_{i}_style"
        elif self == CduDevice.Copilot:
            return f"Rotate/aircraft/controls/cdu_1/mcdu_line_{i}_style"

    def get_symbol_datarefs(self) -> list[str]:
        return [self.get_content_dataref(i) for i in range(16)] + [
            self.get_style_dataref(i) for i in range(16)
        ]


def get_char(char: str | int) -> str:
    if isinstance(char, str) and len(char) == 1:
        return CHAR_MAP.get(char, char)
    try:
        return chr(int(char))
    except Exception:
        return " "


def get_color(color: int) -> str:
    return COLOR_MAP.get(color, "w")


def fetch_dataref_mapping(device: CduDevice):
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)
        target_names = device.get_symbol_datarefs()
        return {
            int(dr["id"]): dr["name"]
            for dr in response_json["data"]
            if dr["name"] in target_names
        }


def generate_display_json(values: dict[str, str], device: CduDevice):
    display_data = [[] for _ in range(CDU_CELLS)]

    content_lines = [
        values.get(device.get_content_dataref(i), "").ljust(CDU_COLUMNS)
        for i in range(CDU_ROWS)
    ]
    style_lines = [
        values.get(device.get_style_dataref(i), [1] * CDU_COLUMNS)
        for i in range(CDU_ROWS)
    ]
    style_lines = [s if isinstance(s, list) else [1] * CDU_COLUMNS for s in style_lines]

    for row in range(CDU_ROWS):
        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col
            char = get_char(content_lines[row][col])
            color = get_color(style_lines[row][col]) if col < len(style_lines[row]) else "w"
            display_data[index] = [char, color, 1]

    return json.dumps({"Target": "Display", "Data": display_data})


async def handle_device_update(queue: asyncio.Queue, device: CduDevice):
    last_run_time = 0
    rate_limit_time = 0.05

    endpoint = device.get_endpoint()
    logging.info("Connecting to CDU device %s", device)
    async for websocket in websockets.connect(endpoint):
        logging.info("Connected successfully to CDU device %s", device)

        try:
            await websocket.send(FONT_REQUEST)
        except Exception:
            logging.warning("Could not set font for %s", device)

        while True:
            values = await queue.get()
            try:
                elapsed = asyncio.get_event_loop().time() - last_run_time
                if elapsed < rate_limit_time:
                    await asyncio.sleep(rate_limit_time - elapsed)

                display_json = generate_display_json(values, device)
                await websocket.send(display_json)
                last_run_time = asyncio.get_event_loop().time()

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
                            "datarefs": [{"id": id_value} for id_value in dataref_map.keys()]
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
                    decoded_value = (
                        base64.b64decode(value).decode(errors="ignore").replace("\x00", " ")
                        if isinstance(value, str)
                        else value
                    )
                    new_values[dataref_name] = decoded_value

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
    devices = []
    for device in CduDevice:
        endpoint = device.get_endpoint()
        try:
            async with websockets.connect(endpoint) as socket:
                logging.info("Discovered CDU device %s at endpoint %s", device, endpoint)
                try:
                    await socket.send(FONT_REQUEST)
                    await asyncio.sleep(1)
                except Exception:
                    logging.warning("Could not set font for %s", device)
                    continue
                devices.append(device)
        except websockets.WebSocketException:
            logging.warning("CDU device %s not available at %s", device, endpoint)
            continue
    return devices


async def main():
    logging.basicConfig(level=logging.INFO)
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
