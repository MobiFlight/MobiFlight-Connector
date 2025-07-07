import asyncio
import base64
import json
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
WS_CO_PILOT = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-co-pilot"
WS_OBSERVER = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-observer"

BALLOT_BOX = "\u2610"
DEGREES = "\u00b0"

CHAR_MAP = {"#": BALLOT_BOX, "*": DEGREES}
COLOR_MAP = {1: "w", 2: "m", 3: "g", 4: "c", 5: "e", 6: "c"}


class CduDevice(StrEnum):
    Captain = "cduL"
    CoPilot = "cduR"
    Observer = "cduC"

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

    def get_symbol_dataref(self) -> str:
        return f"1-sim/{self}/display/symbols"

    def get_symbol_color_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsColor"

    def get_symbol_size_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsSize"

    def get_symbol_effects_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsEffects"


def get_char(char: str) -> str:
    return CHAR_MAP.get(char, char)


def get_color(color: int, effect: int) -> str:
    if effect == 1:
        return "e"
    return COLOR_MAP.get(color, "w")


def fetch_dataref_mapping(device: CduDevice):
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)

        return dict(
            map(
                lambda dataref: (int(dataref["id"]), str(dataref["name"]).strip()),
                filter(
                    lambda x: device.get_symbol_dataref() in str(x["name"]),
                    response_json["data"],
                ),
            )
        )


def generate_display_json(device: CduDevice, values: dict[str, str]):
    display_data = [[] for _ in range(CDU_ROWS * CDU_COLUMNS)]

    cdu_lines = [
        (char, size, color, effect)
        for char, size, color, effect in zip(
            values[device.get_symbol_dataref()],
            values[device.get_symbol_size_dataref()],
            values[device.get_symbol_color_dataref()],
            values[device.get_symbol_effects_dataref()],
        )
    ]

    for row in range(CDU_ROWS):
        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col

            char, size, color, effect = cdu_lines[index]
            if char == 0:
                continue

            display_data[index] = [
                get_char(char),
                get_color(color, effect),
                size,
            ]

    return json.dumps({"Target": "Display", "Data": display_data})


async def handle_device_update(queue: asyncio.Queue, device: CduDevice):
    last_run_time = 0
    rate_limit_time = 0.1

    endpoint = device.get_endpoint()
    async for websocket in websockets.connect(endpoint):
        while True:
            values = await queue.get()

            try:
                elapsed = asyncio.get_event_loop().time() - last_run_time

                if elapsed < rate_limit_time:
                    await asyncio.sleep(rate_limit_time - elapsed)

                display_json = generate_display_json(device, values)
                await websocket.send(display_json)
                last_run_time = asyncio.get_event_loop().time()

            except websockets.exceptions.ConnectionClosed:
                await queue.put(values)
                break


async def handle_dataref_updates(queue: asyncio.Queue, device: CduDevice):
    last_known_values = {}

    dataref_map = fetch_dataref_mapping(device)
    async for websocket in websockets.connect(BASE_WEBSOCKET_URI):
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

                    new_values[dataref_name] = (
                        base64.b64decode(value).decode().replace("\x00", " ")
                        if isinstance(value, str)
                        else value
                    )

                if new_values == last_known_values:
                    continue

                last_known_values = new_values
                await queue.put(new_values)
        except websockets.exceptions.ConnectionClosed:
            continue


async def get_available_devices() -> list[CduDevice]:
    device_candidates = [device for device in CduDevice]

    available_devices = []

    for device in device_candidates:
        try:
            async with websockets.connect(device.get_endpoint()) as _:
                available_devices.append(device)
        except websockets.WebSocketException:
            continue

    return available_devices


async def main():
    available_devices = await get_available_devices()

    tasks = []

    for device in available_devices:
        queue = asyncio.Queue()

        tasks.append(asyncio.create_task(handle_dataref_updates(queue, device)))
        tasks.append(asyncio.create_task(handle_device_update(queue, device)))

    await asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())
