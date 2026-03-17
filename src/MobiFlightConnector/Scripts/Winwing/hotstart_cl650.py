"""
Adds support for the Hotstart Challenger 650 in X-Plane

Many X-Plane aircraft have similar formats for datarefs and the means of retrieving, translating and sending updates is mostly the same.

In order to support multiple CDU devices seamlessly, a dynamic approach is taken whereby an enum class is defined that contains the supported devices.
A device is considered "supported" if it exists in the aircraft. Some aircraft have 3 CDUs while others have 2.
Datarefs of interest follow the pattern:
CL650/CDU/<CDU Number>/screen/text_lineX - text lines where X is from 0 to 14
CL650/CDU/<CDU Number>/screen/style_lineX - character styles lines where X is from 0 to 14, bytes type with 24 elements, each element representing type for each character


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
import re
import json
import logging
import urllib.request
import websockets
from enum import StrEnum, IntEnum
from typing import TypedDict, TypeAlias

CDU_COLUMNS = 24
CDU_ROWS = 14

WEBSOCKET_HOST = "localhost"
WEBSOCKET_PORT = 8320

BASE_REST_URL = "http://localhost:8086/api/v2/datarefs"
BASE_WEBSOCKET_URI = f"ws://{WEBSOCKET_HOST}:8086/api/v2"

WS_CAPTAIN = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-captain"
WS_CO_PILOT = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-co-pilot"
WS_OBSERVER = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-observer"

FONT_REQUEST = json.dumps({"Target": "Font", "Data": "Boeing"})

# contains processed datarefs for each line
class LineData(TypedDict):
    text: str
    style: bytes

# defines shape of dict containing all information from CDU datarefs, essentially 15 elements with indexes 0 through 14 with values being LineData
CduData: TypeAlias = dict[int, LineData] 

class CduCharacterSize(IntEnum):
    LARGE = 0
    SMALL = 1

    @classmethod
    def from_style(cls, style: int):
        mask_large = 0x80
        if style & mask_large:
            return cls.LARGE
        else:
            return cls.SMALL 


class CduCharacterColor(StrEnum):
    AMBER = "a"
    WHITE = "w"
    CYAN =  "c"
    GREEN = "g"
    MAGENTA = "m"
    RED = "r"
    YELLOW = "y"
    BROWN = "o"
    GREY = "e"
    KHAKI = "k"

    @classmethod
    def from_style(cls, style: int):

        mask_cyan = 0x01
        mask_yellow = 0x03
        mask_green = 0x04
        mask_magenta = 0x05
        mask_white = 0x07

        if style & mask_white == mask_white:
            return cls.WHITE
        elif style & mask_magenta == mask_magenta:
            return cls.MAGENTA
        elif style & mask_green == mask_green:
            return cls.GREEN
        elif style & mask_yellow == mask_yellow:
            return cls.YELLOW
        elif style & mask_cyan == mask_cyan:
            return cls.CYAN

        return cls.WHITE


class CduDevice(StrEnum):
    Captain = "1"
    CoPilot = "2"
    Observer = "3"

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

# this allows filtering of ALL datarefs returned by X-Plane to only the ones we care about
DATAREF_FILTER_PATTERNS = {
    CduDevice.Captain: re.compile("^CL650/CDU/1/screen/(style|text)_line[0-9]{1,2}$"),
    CduDevice.CoPilot: re.compile("^CL650/CDU/2/screen/(style|text)_line[0-9]{1,2}$"),
    CduDevice.Observer: re.compile("^CL650/CDU/3/screen/(style|text)_line[0-9]{1,2}$")
    }

# this allows processing datarefs and extract line number and type of data (text or style)
DATAREF_PROCESS_PATTERN = re.compile("^(text|style)_line([0-9]{1,2})$") # regex group 1 will be "text" or "style", group 2 will be line number
DATAREF_LINE_COUNT = 15 # total of 15 lines in dataref, 0 through 14, last line is a Message line which is not displayed on Winwing

def fetch_dataref_mapping(device: CduDevice):
    with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)
        data = list(response_json["data"])

        dataref_map = filter(
            lambda x: DATAREF_FILTER_PATTERNS[device].match(x["name"]),
            data,
        )

        return dict(
            map(
                lambda dataref: (int(dataref["id"]), str(dataref["name"])),
                dataref_map,
            )
        )

def generate_display_json(cdu_data: CduData) -> str:
    display_data: list[tuple[str, CduCharacterColor, CduCharacterSize]] = []

    for row in range(CDU_ROWS):

        if row in cdu_data:
            character_styles = list(cdu_data[row]["style"])
            row_text = cdu_data[row]["text"]

            if len(row_text) == 0:
                for _ in range(CDU_COLUMNS):
                    display_data.append((" ", CduCharacterColor.WHITE, CduCharacterSize.SMALL)) # fill up empty line if text was empty
            else: 
                for character_index in range(CDU_COLUMNS):
                    if character_index < len(row_text): # or populate text with styles
                        display_data.append((row_text[character_index], CduCharacterColor.from_style(character_styles[character_index]), CduCharacterSize.from_style(character_styles[character_index])))
                    else:
                        display_data.append((" ", CduCharacterColor.WHITE, CduCharacterSize.SMALL)) # fill up rest of characters, but this is very unlikely to hit as datarefs have full characters

    return json.dumps({"Target": "Display", "Data": display_data})


def process_datarefs(values: dict[str, str]) -> CduData:
    results: CduData = {}

    for dataref_name, dataref_value in values.items():
        short_name = dataref_name[dataref_name.rfind('/')+1:] # strip everything before last '/', CL650/CDU/1/screen/text_line0 becomes text_line0
        re_match = DATAREF_PROCESS_PATTERN.fullmatch(short_name) # applies compiled regular expression to extract part of name
        if re_match is None:
            # no match
            logging.error("error trying to extract type and line number from dataref: %s", short_name)
            continue

        line_number = int(re_match.group(2))
        line_type = re_match.group(1)

        if line_number not in results:
            results[line_number] = {"text": "", "style": bytes()} # initialize line data to be filled later if not already initialized


        if line_type == "text":
            # if this is text dataref , then base64 encoded value is a string, so we decode it
            try:
                value = base64.b64decode(dataref_value).decode().replace("\x00","")
                results[line_number]["text"] = value
            except Exception:
                logging.exception("error decoding text line dataref value from base64: %s", dataref_value)
                continue

        elif line_type == "style":
            # if this is style dataref, then base64 encoded value is bytes
            try:
                value = base64.b64decode(dataref_value)
                results[line_number]["style"] = bytes(value)
            except Exception:
                logging.exception("error decoding style line dataref value from base64: %s", dataref_value)
                continue


    return results

def print_cdu_data(data: CduData):

    for row in range(DATAREF_LINE_COUNT):
        print(row, f"({len(data[row]['text'])}):", data[row]["text"], "****", f"({len(data[row]['style'])})", [hex(v) for v in list(data[row]["style"])])


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
                elapsed = asyncio.get_event_loop().time() - last_run_time

                # Weaker CPUs may experience performance issues when a websocket connection is saturated with requests, such as when pages are frequently changed.
                # This rate limits the number of active websocket requests to MobiFlight.
                # The delay should not be noticeable unless a user heavily spams page changes, but it should be enough that too many messages won't be pushed at once.
                if elapsed < rate_limit_time:
                    await asyncio.sleep(rate_limit_time - elapsed)

                cdu_data = process_datarefs(values)

                display_json = generate_display_json(cdu_data)
                await websocket.send(display_json)
                last_run_time = asyncio.get_event_loop().time()

            except websockets.exceptions.ConnectionClosed:
                logging.error("MobiFlight websocket connection was closed... Attempting to reconnect")
                await queue.put(values)
                break


async def handle_dataref_updates(queue: asyncio.Queue[dict[str,str]], device: CduDevice):
    last_known_values: dict[str, str] = {}

    dataref_map = fetch_dataref_mapping(device) # contains mapping between int id of dataref and name of dataref in X-Plane, values received only related to ids
    logging.info("Connecting to X-Plane websocket server")
    async for websocket in websockets.connect(
        BASE_WEBSOCKET_URI,
    ):
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

                new_values: dict[str, str] = dict(last_known_values)

                for dataref_id, value in data["data"].items():
                    dataref_id = int(dataref_id)
                    if dataref_id not in dataref_map:
                        continue

                    dataref_name = dataref_map[dataref_id]
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

    available_devices: list[CduDevice] = []

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
