import asyncio
from collections import deque
from enum import IntEnum, StrEnum
from itertools import chain
import json
import logging
from math import ceil, floor
import re
from typing import Literal, Never, Optional, List, Dict, Union
import websockets.asyncio.client as ws_client


class MfCharSize(IntEnum):
    Large = 0
    Small = 1


class MfColour(StrEnum):
    Amber = "a"
    Brown = "o"
    Cyan = "c"
    Green = "g"
    Grey = "e"
    Khaki = "k"
    Magenta = "m"
    Red = "r"
    White = "w"
    Yellow = "y"


MfMcduChar = Union[tuple[Never], tuple[str, MfColour, MfCharSize]]

# URLs for WinWing CDU WebSockets
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
CO_PILOT_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"

# FlyByWire SimBridge MCDU WebSocket URL
FBW_MCDU_URL: str = "ws://localhost:8380/interfaces/v1/mcdu"

# Display dimensions
CDU_COLUMNS: int = 24
CDU_ROWS: int = 14
CDU_CELLS: int = CDU_COLUMNS * CDU_ROWS

# Special character mapping
REPLACED_CHARS = {
    "←": "\u2190",  # left arrow
    "→": "\u2192",  # right arrow
    "↑": "\u2191",  # up arrow
    "↓": "\u2193",  # down arrow
    "_": "\u2610",  # box
    "\u00b0": "°",  # degree symbol
    "&": "\u0394",  # delta symbol (overfly)
    "\xa0": " ",    # non-breaking space
    "{": "\u2190",  # left arrow
    "}": "\u2192",  # right arrow
    "|": "/",       # forward slash
}


class MobiFlightClient:
    def __init__(self, websocket_uri: str, max_retries: int = 3) -> None:
        self.websocket: Optional[ws_client.WebSocketClientProtocol] = None
        self.connected: asyncio.Event = asyncio.Event()
        self.websocket_uri: str = websocket_uri
        self.retries: int = 0
        self.max_retries: int = max_retries

    async def run(self) -> None:
        while self.retries < self.max_retries:
            try:
                if self.websocket is None:
                    logging.info("Connecting to MobiFlight at %s", self.websocket_uri)
                    self.websocket = await ws_client.connect(
                        self.websocket_uri, ping_interval=None
                    )
                    logging.info("MobiFlight connected at %s", self.websocket_uri)
                    self.connected.set()
                await self.websocket.recv()
            except Exception as e:
                self.retries += 1
                logging.debug(f"Retrying MobiFlight websocket, attempt {self.retries}: {e}")
                self.websocket = None
                self.connected.clear()
            await asyncio.sleep(5)
        logging.info(
            "Max retries reached. Giving up connecting to MobiFlight at %s. "
            "If you only have one CDU attached, you can ignore this message.",
            self.websocket_uri,
        )
        self.connected.set()

    async def send(self, data: str) -> None:
        if self.websocket and self.connected.is_set():
            await self.websocket.send(data)

    async def close(self) -> None:
        if self.websocket:
            await self.websocket.close()
            self.websocket = None
            self.connected.clear()

    def is_connected(self) -> bool:
        return self.websocket and self.connected.is_set()


FormatStack = deque[Union[MfCharSize, MfColour, Literal["left", "right"]]]


def get_format_colour(format_stack: FormatStack) -> MfColour:
    return next((x for x in format_stack if isinstance(x, MfColour)), MfColour.White)


def get_format_size(format_stack: FormatStack, is_label_line: bool) -> MfCharSize:
    return next(
        (x for x in format_stack if isinstance(x, MfCharSize)),
        MfCharSize.Small if is_label_line else MfCharSize.Large,
    )


def get_format_alignment(
    format_stack: FormatStack,
) -> Union[Literal["left", "right"], None]:
    return next((x for x in format_stack if x == "left" or x == "right"), None)


FBW_TAGS = [
    "end",
    "sp",
    "left",
    "right",
    "small",
    "big",
    "amber",
    "cyan",
    "green",
    "inop",
    "magenta",
    "red",
    "white",
    "yellow",
    "",
]
FBW_TAG_REGEX = re.compile("{(" + "|".join(FBW_TAGS) + ")}")


def parse_fbw_segment(
    segment: str, is_label_line: bool
) -> tuple[List[MfMcduChar], List[MfMcduChar], List[MfMcduChar]]:
    """
    Returns a list of characters that are not specifically aligned,
    a list that are left aligned, and a list that are right aligned.
    """

    normal_chars: List[MfMcduChar] = []
    left_chars: List[MfMcduChar] = []
    right_chars: List[MfMcduChar] = []

    format_stack: FormatStack = deque()
    tag = None

    last_match_index = 0
    for match in FBW_TAG_REGEX.finditer(segment):
        match get_format_alignment(format_stack):
            case "left":
                current_chars = left_chars
            case "right":
                current_chars = right_chars
            case _:
                current_chars = normal_chars

        for c in segment[last_match_index:match.start()]:
            current_chars.append(
                (
                    REPLACED_CHARS.get(c, c),
                    get_format_colour(format_stack),
                    get_format_size(format_stack, is_label_line),
                )
            )
        last_match_index = match.end()

        tag = match.group(1)
        match tag:
            case "end":
                format_stack.popleft()
            case "small":
                format_stack.appendleft(MfCharSize.Small)
            case "big":
                format_stack.appendleft(MfCharSize.Large)
            case "amber":
                format_stack.appendleft(MfColour.Amber)
            case "cyan":
                format_stack.appendleft(MfColour.Cyan)
            case "green":
                format_stack.appendleft(MfColour.Green)
            case "inop":
                format_stack.appendleft(MfColour.Grey)
            case "magenta":
                format_stack.appendleft(MfColour.Magenta)
            case "red":
                format_stack.appendleft(MfColour.Red)
            case "white":
                format_stack.appendleft(MfColour.White)
            case "yellow":
                format_stack.appendleft(MfColour.Yellow)
            case "sp":
                current_chars.append(tuple())
            case "left" | "right":  # these are used only in the F-PLN title line...
                format_stack.appendleft(tag)
            case _:
                logging.warning(f'Unknown format tag "{tag}"!')
                format_stack.appendleft(None)

    for c in segment[last_match_index:]:
        current_chars.append(
            (
                REPLACED_CHARS.get(c, c),
                get_format_colour(format_stack),
                get_format_size(format_stack, is_label_line),
            )
        )

    # centre the content in the same way the FBW HTML layout does if it's too long
    if len(normal_chars) > CDU_COLUMNS:
        logging.debug(f"Line is too long ({len(normal_chars)})!")
        diff = len(normal_chars) - CDU_COLUMNS
        start = floor(diff / 2)
        end = ceil(diff / 2)
        normal_chars = normal_chars[start:-end]

    return normal_chars, left_chars[:CDU_COLUMNS], right_chars[:CDU_COLUMNS]


def is_blank_char(char: MfMcduChar) -> bool:
    return len(char) == 0 or char[0] == " "


def place_chars_in_row(
    row: List[MfMcduChar],
    chars: tuple[List[MfMcduChar], List[MfMcduChar], List[MfMcduChar]],
    column: int,
) -> None:
    for i, c in enumerate(chars[1]):  # left-aligned
        if not is_blank_char(c):
            row[i] = c

    for i, c in enumerate(chars[2]):  # right-aligned
        if not is_blank_char(c):
            row[CDU_COLUMNS - len(chars[2]) + i] = c

    for i, c in enumerate(chars[0]):  # normal alignment
        if not is_blank_char(c):
            row[column + i] = c

    assert len(row) == CDU_COLUMNS


def create_mobi_json(content: Dict) -> str:
    """Convert FlyByWire MCDU data to MobiFlight JSON format"""

    output_lines = [[[] for _ in range(CDU_COLUMNS)] for _ in range(CDU_ROWS)]

    # Extract MCDU content from the payload, process it in the same order as the HTML layers in the FBW CDU
    try:
        # Process title left (if any, left-aligned)
        title_left = content.get("titleLeft")
        if title_left is not None:
            chars = parse_fbw_segment(title_left, False)
            place_chars_in_row(output_lines[0], chars, 0)

        # Process title (centred)
        title = content.get("title")
        if title is not None:
            chars = parse_fbw_segment(title, False)
            column = (CDU_COLUMNS - len(chars[0])) // 2
            place_chars_in_row(output_lines[0], chars, column)

        # Left/right arrows on title row, right side
        arrows = content.get("arrows", [False, False, False, False])
        if arrows[2]:  # Left arrow
            output_lines[0][CDU_COLUMNS - 2] = (
                REPLACED_CHARS["←"],
                MfColour.White,
                MfCharSize.Large,
            )
        if arrows[3]:  # Right arrow
            output_lines[0][CDU_COLUMNS - 1] = (
                REPLACED_CHARS["→"],
                MfColour.White,
                MfCharSize.Large,
            )

        # Process page on title row, right side
        page = content.get("page")
        if page is not None:
            chars = parse_fbw_segment(page, True)
            place_chars_in_row(output_lines[0], chars, CDU_COLUMNS - len(chars[0]))

        # Process main content lines
        lines = content.get("lines", [])
        for line_idx, line in enumerate(lines):
            if line_idx >= CDU_ROWS - 1:  # Reserve last row for scratchpad
                break

            row = output_lines[line_idx + 1]
            is_label_line = line_idx % 2 == 0

            # Process line data - each line has left, right, and center columns
            for segment_idx, segment in enumerate(line[:3]):
                if not segment:
                    continue

                chars = parse_fbw_segment(segment, is_label_line)

                if segment_idx == 0:  # Left column
                    place_chars_in_row(row, chars, 0)
                elif segment_idx == 1:  # Right column
                    place_chars_in_row(row, chars, CDU_COLUMNS - len(chars[0]))
                else:  # Center column
                    column = (CDU_COLUMNS - len(chars[0])) // 2
                    place_chars_in_row(row, chars, column)

        # Process scratchpad on last row
        scratchpad = content.get("scratchpad")
        if scratchpad is not None:
            chars = parse_fbw_segment(scratchpad, is_label_line)
            place_chars_in_row(output_lines[CDU_ROWS - 1], chars, 0)

        # Up/down arrows in the scratchpad line, right side
        if arrows[0]:  # Up arrow
            output_lines[CDU_ROWS - 1][CDU_COLUMNS - 2] = (
                REPLACED_CHARS["↑"],
                MfColour.White,
                MfCharSize.Large,
            )
        if arrows[1]:  # Down arrow
            output_lines[CDU_ROWS - 1][CDU_COLUMNS - 1] = (
                REPLACED_CHARS["↓"],
                MfColour.White,
                MfCharSize.Large,
            )

        # Create final message
        message = {"Target": "Display", "Data": list(chain(*output_lines))}

        return json.dumps(message)

    except Exception as e:
        logging.error(f"Error creating MobiFlight JSON: {e}")
        # Return empty display in case of error
        return json.dumps({"Target": "Display", "Data": [[] for _ in range(CDU_CELLS)]})


class FbwMcduClient:
    """Client for the FlyByWire MCDU WebSocket"""

    def __init__(
        self, mobiflight_left: MobiFlightClient, mobiflight_right: MobiFlightClient
    ) -> None:
        self.mobiflight = dict(left=mobiflight_left, right=mobiflight_right)
        self.fbw_websocket = None
        self.last_mcdu_data: dict[Literal["left", "right"], dict] = dict()
        self.retries = 0
        self.max_retries = 10

    async def connect_to_mcdu(self):
        """Connect to the FBW MCDU WebSocket"""
        while self.retries < self.max_retries:
            try:
                if self.fbw_websocket is None:
                    logging.info("Connecting to FlyByWire SimBridge...")
                    self.fbw_websocket = await ws_client.connect(FBW_MCDU_URL)
                    logging.info("Connected to FlyByWire SimBridge")
                    self.retries = 0  # Reset retries on successful connection

                    # Request an update as soon as connected
                    await self.fbw_websocket.send("requestUpdate")
                    return True
            except Exception as e:
                self.retries += 1
                logging.debug(f"Retrying SimBridge, attempt {self.retries}: {e}")
                self.fbw_websocket = None
                await asyncio.sleep(5)  # Wait before retry
        return False

    async def run(self):
        """Main processing loop"""
        logging.info("Starting FlyByWire SimBridge client")

        # Initial connection
        if not await self.connect_to_mcdu():
            return

        while True:
            try:
                # Wait for messages from the MCDU
                if self.fbw_websocket is None:
                    if not await self.connect_to_mcdu():
                        logging.info("Max SimBridge attempts reached. Waiting for new attempts.")
                        await asyncio.sleep(
                            10
                        )  # Wait longer between retries if we can't connect
                        continue

                msg = await self.fbw_websocket.recv()

                # Process any update messages
                if msg.startswith("update:"):
                    data_json = json.loads(msg[msg.index(":") + 1:])

                    for side in ("left", "right"):
                        mobiflight = self.mobiflight.get(side)
                        mcdu_data = data_json.get(side)
                        if mobiflight is not None and mobiflight.is_connected():
                            # only update if there is new data to display
                            if (
                                mcdu_data is not None
                                and self.last_mcdu_data.get(side) != mcdu_data
                            ):
                                self.last_mcdu_data[side] = mcdu_data
                                await mobiflight.send(create_mobi_json(mcdu_data))
                            elif mcdu_data is None:
                                self.last_mcdu_data[side] = None
                                # clear the display
                                await mobiflight.send(create_mobi_json(dict()))
                        else:
                            # make sure we get a refresh if we later connect
                            self.last_mcdu_data[side] = None

            except Exception as e:
                logging.error(f"Error processing MCDU data: {e}")
                self.fbw_websocket = None
                await asyncio.sleep(5)


async def main():
    logging.basicConfig(
        level=logging.INFO, format="%(asctime)s - %(levelname)s - %(message)s"
    )

    logging.info("----STARTED FBW A32NX MCDU to WinWing CDU Integration----")

    mobiflight_left = MobiFlightClient(CAPTAIN_CDU_URL)
    mobiflight_right = MobiFlightClient(CO_PILOT_CDU_URL)
    mobiflight_left_task = asyncio.create_task(mobiflight_left.run())
    mobiflight_right_task = asyncio.create_task(mobiflight_right.run())

    fbw_client = FbwMcduClient(mobiflight_left, mobiflight_right)
    fbw_task = asyncio.create_task(fbw_client.run())

    # Wait for all to complete (they shouldn't unless there's an error)
    await asyncio.gather(mobiflight_left_task, mobiflight_right_task, fbw_task)


if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        logging.info("Process terminated by user")
    except Exception as e:
        logging.error(f"Error: {e}")
