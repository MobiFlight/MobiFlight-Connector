"""
Adds support for the Hotstart Challenger 650 in X-Plane.

Datarefs of interest follow the pattern:
CL650/CDU/<CDU Number>/screen/text_lineX - text lines where X is from 0 to 14
CL650/CDU/<CDU Number>/screen/style_lineX - character styles lines where X is from 0 to 14, bytes type with 24 elements, each element representing type for each character

See xplane_cdu_common.py for the shared connection/orchestration logic this script relies on.
"""

import asyncio
import base64
import json
import logging
import os
import re
import sys
from enum import StrEnum, IntEnum
from typing import TypedDict, TypeAlias

# MobiFlight's embedded Python does not add the script directory to sys.path.
_here = os.path.dirname(os.path.abspath(__file__))
if _here not in sys.path:
    sys.path.insert(0, _here)

import xplane_cdu_common as cdu_common

CDU_COLUMNS = cdu_common.CDU_COLUMNS
CDU_ROWS = cdu_common.CDU_ROWS


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
    CYAN = "c"
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


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "1"
    CoPilot = "2"
    Observer = "3"


# this allows filtering of ALL datarefs returned by X-Plane to only the ones we care about
DATAREF_FILTER_PATTERNS = {
    CduDevice.Captain: re.compile("^CL650/CDU/1/screen/(style|text)_line[0-9]{1,2}$"),
    CduDevice.CoPilot: re.compile("^CL650/CDU/2/screen/(style|text)_line[0-9]{1,2}$"),
    CduDevice.Observer: re.compile("^CL650/CDU/3/screen/(style|text)_line[0-9]{1,2}$"),
}

# this allows processing datarefs and extract line number and type of data (text or style)
DATAREF_PROCESS_PATTERN = re.compile("^(text|style)_line([0-9]{1,2})$")  # regex group 1 will be "text" or "style", group 2 will be line number
DATAREF_LINE_COUNT = 15  # total of 15 lines in dataref, 0 through 14, last line is a Message line which is not displayed on Winwing


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: DATAREF_FILTER_PATTERNS[device].match(name) is not None
    )


def decode_value(dataref_name: str, value):
    # process_datarefs() does the actual base64 decoding once a line is complete.
    return value


def render_cdu_data(cdu_data: CduData) -> str:
    display_data: list[tuple[str, CduCharacterColor, CduCharacterSize]] = []

    for row in range(CDU_ROWS):

        if row in cdu_data:
            character_styles = list(cdu_data[row]["style"])
            row_text = cdu_data[row]["text"]

            if len(row_text) == 0:
                for _ in range(CDU_COLUMNS):
                    display_data.append((" ", CduCharacterColor.WHITE, CduCharacterSize.SMALL))  # fill up empty line if text was empty
            else:
                for character_index in range(CDU_COLUMNS):
                    if character_index < len(row_text):  # or populate text with styles
                        display_data.append((row_text[character_index], CduCharacterColor.from_style(character_styles[character_index]), CduCharacterSize.from_style(character_styles[character_index])))
                    else:
                        display_data.append((" ", CduCharacterColor.WHITE, CduCharacterSize.SMALL))  # fill up rest of characters, but this is very unlikely to hit as datarefs have full characters

    return json.dumps({"Target": "Display", "Data": display_data})


def process_datarefs(values: dict[str, str]) -> CduData:
    results: CduData = {}

    for dataref_name, dataref_value in values.items():
        short_name = dataref_name[dataref_name.rfind("/") + 1:]  # strip everything before last '/', CL650/CDU/1/screen/text_line0 becomes text_line0
        re_match = DATAREF_PROCESS_PATTERN.fullmatch(short_name)  # applies compiled regular expression to extract part of name
        if re_match is None:
            # no match
            logging.error("error trying to extract type and line number from dataref: %s", short_name)
            continue

        line_number = int(re_match.group(2))
        line_type = re_match.group(1)

        if line_number not in results:
            results[line_number] = {"text": "", "style": bytes()}  # initialize line data to be filled later if not already initialized

        if line_type == "text":
            # if this is text dataref , then base64 encoded value is a string, so we decode it
            try:
                value = base64.b64decode(dataref_value).decode().replace("\x00", "")
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


def generate_display_json(device: CduDevice, values: dict[str, str]) -> str:
    return render_cdu_data(process_datarefs(values))


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(CduDevice, fetch_dataref_mapping, generate_display_json, decode_value=decode_value, font_name="Boeing")
    )
