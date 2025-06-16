import asyncio
import base64
import copy
import json
import urllib.request
import websockets
from enum import Enum

CDU_COLUMNS = 24
CDU_ROWS = 14
CDU_CELLS = CDU_COLUMNS * CDU_ROWS

WEBSOCKET_HOST = "localhost"
WEBSOCKET_PORT = 8320

BASE_REST_URL = "http://localhost:8086/api/v2/datarefs"
BASE_WEBSOCKET_URI = f"ws://{WEBSOCKET_HOST}:8086/api/v2"

WS_CAPTAIN = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-captain"
WS_CO_PILOT = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-co-pilot"

CHARACTER_MAPPING = {"`": "\u00b0", "*": "\u2610", "=": "\u002a"}
COLOR_MAPPING = {"G": "g", "C": "c", "I": "e", "M": "m"}


class FanoutExchange:
    def __init__(self):
        self.__consumer_queues: list[asyncio.Queue] = []

    def add_consumer(self):
        queue = asyncio.Queue()
        self.__consumer_queues.append(queue)

        return queue

    async def put(self, message):
        for queue in self.__consumer_queues:
            await queue.put(message)


class CduDevice(Enum):
    Captain = "fmc1"
    CoPilot = "fmc2"

    def get_endpoint(self) -> str:
        match self:
            case CduDevice.Captain:
                return WS_CAPTAIN
            case CduDevice.CoPilot:
                return WS_CO_PILOT
            case _:
                raise KeyError(f"Invalid device specified {self}")


def fetch_dataref_mapping(available_devices: list[CduDevice]):
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)

        data = list(response_json["data"])
        dataref_map = filter(
            lambda x: any(
                str(x["name"]).startswith(f"laminar/B738/{device.value}")
                for device in available_devices
            ),
            data,
        )

        return dict(
            map(
                lambda dataref: (int(dataref["id"]), str(dataref["name"])),
                dataref_map,
            )
        )


def get_color(dataref: str) -> bool:
    dataref_ending = dataref[dataref.rindex("_") + 1]

    return COLOR_MAPPING.get(dataref_ending, "w")


def get_size(dataref: str) -> int:
    return 1 if dataref.endswith("_X") or dataref.endswith("_S") else 0


def process_cdu_line(line_datarefs: dict[str, str], row: int) -> list[list]:
    line_chars = [[] for _ in range(CDU_COLUMNS)]

    target_suffixes = (
        ["_X", "_LX", "_GX"] if row % 2 == 0 else ["_G", "_L", "_M", "_S", "_I", "_SI"]
    )

    for dataref, text in line_datarefs.items():
        if not text or text.isspace():
            continue

        # The first and last rows only cover a single row. All other rows cover 2 rows between the label (X, LX or GX) and the main content (G, L, M, S, I or SI)
        if (row != 0 and row != 14) and not any(
            dataref.endswith(suffix) for suffix in target_suffixes
        ):
            continue

        for i, char in enumerate(text[:CDU_COLUMNS]):
            if char == " ":
                continue

            line_chars[i] = (
                CHARACTER_MAPPING.get(char, char),
                get_color(dataref),
                get_size(dataref),
            )

    return line_chars


def group_datarefs_by_line(values: dict[str, str]) -> dict[int, dict[str, str]]:
    grouped_datarefs = {}

    for dataref, value in values.items():
        dataref_name = dataref[dataref.rindex("/") + 1 :]
        line_num = (
            7 if dataref_name.startswith("Line_entry") else int(dataref_name[4:6])
        )

        if line_num not in grouped_datarefs:
            grouped_datarefs[line_num] = {}
        grouped_datarefs[line_num][dataref] = value

    return grouped_datarefs


def get_display_json(values: dict[str, str]) -> str:
    display_data = [[] for _ in range(CDU_CELLS)]

    grouped_datarefs = group_datarefs_by_line(values)

    for row in range(CDU_ROWS + 1):
        if row == 1:
            continue

        dataref_line = row // 2 if row > 0 else 0

        line_datarefs = grouped_datarefs.get(dataref_line, {})
        if not line_datarefs:
            continue

        line_chars = process_cdu_line(line_datarefs, row)

        start_index = (row - 1 if row > 0 else row) * CDU_COLUMNS
        display_data[start_index : start_index + CDU_COLUMNS] = line_chars

    return json.dumps({"Target": "Display", "Data": display_data})


async def handle_device_update(device: CduDevice, queue: asyncio.Queue):
    async for device_connection in websockets.connect(device.get_endpoint()):
        while True:
            target_device, values = await queue.get()
            if target_device != device:
                continue

            display_json = get_display_json(values)
            try:
                await device_connection.send(display_json)
            except websockets.ConnectionClosedError:
                await queue.put((target_device, values))
                break


async def handle_dataref_updates(
    exchange: FanoutExchange, available_devices: list[CduDevice]
):
    last_known_values = {device: {} for device in available_devices}

    dataref_map = fetch_dataref_mapping(available_devices)
    async for websocket in websockets.connect(
        BASE_WEBSOCKET_URI,
    ):
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

                new_values = copy.deepcopy(last_known_values)

                for dataref_id, value in data["data"].items():
                    dataref_id = int(dataref_id)
                    if dataref_id not in dataref_map:
                        continue

                    dataref_name = dataref_map[dataref_id]
                    device = (
                        CduDevice.Captain
                        if CduDevice.Captain.value in dataref_name
                        else CduDevice.CoPilot
                    )

                    new_values[device][dataref_name] = (
                        base64.b64decode(value).decode().replace("\x00", " ")
                    )

                for device, values in new_values.items():
                    if values == last_known_values[device]:
                        continue

                    last_known_values[device] = values
                    await exchange.put((device, values))
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
    exchange = FanoutExchange()
    available_devices = await get_available_devices()

    tasks = [asyncio.create_task(handle_dataref_updates(exchange, available_devices))]

    for device in available_devices:
        tasks.append(
            asyncio.create_task(handle_device_update(device, exchange.add_consumer()))
        )

    await asyncio.gather(*tasks)


if __name__ == "__main__":
    asyncio.run(main())
