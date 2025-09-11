import asyncio
import base64
import json
import urllib.request
import websockets
import time
from enum import StrEnum

CDU_COLUMNS = 24
CDU_ROWS = 14

BASE_REST_URL = "http://localhost:8086/api/v2/datarefs"
BASE_WEBSOCKET_URI = "ws://localhost:8086/api/v2"
WS_CAPTAIN = "ws://localhost:8320/winwing/cdu-captain"

CHAR_MAP = {35: "\u2610", 42: "\u00b0"}
COLOR_MAP = {1: "g", 2: "g", 4: "e", 5: "g"}

FONT = "Boeing"
FONT_REQUEST = json.dumps({"Target": "Font", "Data": FONT})

class CduDevice(StrEnum):
    RotateMD11 = "rotate"

    def get_symbol_datarefs(self) -> list[str]:
        base = "Rotate/aircraft/controls/cdu_0"
        return [
            f"{base}/mcdu_line_{i}_content" for i in range(16)
        ] + [
            f"{base}/mcdu_line_{i}_style" for i in range(16)
        ]


def get_char(char: str | int) -> str:
    if char == "$":
        return "□"
    if isinstance(char, str) and len(char) == 1:
        return char
    try:
        return chr(int(char))
    except:
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


def generate_display_json(values: dict[str, str]):
    display_data = [[] for _ in range(CDU_COLUMNS * CDU_ROWS)]

    content_lines = [
        values.get(f"Rotate/aircraft/controls/cdu_0/mcdu_line_{i}_content", "").ljust(24)
        for i in range(CDU_ROWS)
    ]
    style_lines = [
        values.get(f"Rotate/aircraft/controls/cdu_0/mcdu_line_{i}_style", [1] * 24)
        for i in range(CDU_ROWS)
    ]
    style_lines = [s if isinstance(s, list) else [1] * 24 for s in style_lines]

    for row in range(CDU_ROWS):
        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col
            char = get_char(content_lines[row][col])
            color = get_color(style_lines[row][col]) if col < len(style_lines[row]) else "w"
            display_data[index] = [char, color, 1]

    return json.dumps({"Target": "Display", "Data": display_data})


async def handle_dataref_updates(queue: asyncio.Queue, device: CduDevice):
    last_known_values = {}
    dataref_map = fetch_dataref_mapping(device)

    reconnect_delay = 0.5  

    while True:
        try:
            async with websockets.connect(BASE_WEBSOCKET_URI) as websocket:
                await websocket.send(json.dumps({
                    "type": "dataref_subscribe_values",
                    "req_id": 1,
                    "params": {
                        "datarefs": [{"id": id_value} for id_value in dataref_map.keys()]
                    },
                }))

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

                    if new_values != last_known_values:
                        last_known_values = new_values
                        await queue.put(new_values)

        except (websockets.exceptions.ConnectionClosed, ConnectionResetError):
            await asyncio.sleep(reconnect_delay)
            continue


async def handle_device_output(queue: asyncio.Queue):
    reconnect_delay = 0.5  
    debounce_time = 0.02  
    last_sent = 0

    while True:
        try:
            async with websockets.connect(WS_CAPTAIN) as websocket:
                await websocket.send(FONT_REQUEST)

                while True:
                    try:
                        values = await asyncio.wait_for(queue.get(), timeout=0.2)
                    except asyncio.TimeoutError:
                        continue

                    while not queue.empty():
                        values = queue.get_nowait()

                    display_json = generate_display_json(values)
                    now = time.time()

                    if now - last_sent >= debounce_time:
                        await websocket.send(display_json)
                        last_sent = now

        except (websockets.exceptions.ConnectionClosed, ConnectionResetError):
            await asyncio.sleep(reconnect_delay)
            if 'values' in locals():
                await queue.put(values)
            continue


async def main():
    queue = asyncio.Queue()
    device = CduDevice.RotateMD11

    await asyncio.gather(
        handle_dataref_updates(queue, device),
        handle_device_output(queue)
    )


if __name__ == "__main__":
    asyncio.run(main())
