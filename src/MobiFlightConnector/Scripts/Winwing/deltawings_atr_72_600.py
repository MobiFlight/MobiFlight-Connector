"""
Adds support for the DeltaWings Simulations ATR 72-600 in X-Plane.

See xplane_cdu_common.py for the shared connection/orchestration logic this script relies on.
"""

import asyncio
import base64
import json
import os
import re
import sys
from enum import StrEnum

# MobiFlight's embedded Python does not add the script directory to sys.path.
_here = os.path.dirname(os.path.abspath(__file__))
if _here not in sys.path:
    sys.path.insert(0, _here)

import xplane_cdu_common as cdu_common

CDU_COLUMNS = cdu_common.CDU_COLUMNS
CDU_ROWS = cdu_common.CDU_ROWS
CDU_CELLS = cdu_common.CDU_CELLS

# The ATR publishes text/style lines 0-15, but lines 13 and 15 are never written to.
# The remaining lines map in order onto the grid rows, so line 14 lands on the bottom row.
SKIPPED_LINES = {13, 15}
DATAREF_LINES = [line for line in range(16) if line not in SKIPPED_LINES]

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


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "mcdu1"
    CoPilot = "mcdu2"

    def get_dataref_prefix(self) -> str:
        return f"DWSim/ATR_Series/{self}"

    def get_text_dataref(self, line) -> str:
        return f"DWSim/ATR_Series/{self}/text_line{line}"

    def get_style_dataref(self, line) -> str:
        return f"DWSim/ATR_Series/{self}/style_line{line}"


def is_skipped_line(dataref_name: str) -> bool:
    match = LINE_PATTERN.search(dataref_name)
    return match is not None and int(match.group(1)) in SKIPPED_LINES


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: device.get_dataref_prefix() in name and not is_skipped_line(name)
    )


def decode_value(dataref_name: str, value):
    if "text_line" in dataref_name:
        return base64.b64decode(value).decode().replace("\x00", " ")

    # style_line bytes are bit-unpacked per-column later, so keep them as bytes.
    return base64.b64decode(value)


def color_from_style(style):
    # According to the documentation
    # (https://developer.x-plane.com/article/datarefs-for-the-cdu-screen/)
    # the four lowest bits encode color, but only color indexes 0 through 7
    # are defined at this point. We default to white for any color indexes that
    # currently aren't defined.
    return COLOR_MAP.get(style & 0xF, "w")


def size_from_style(style):
    return 0 if style & (1 << 7) else 1


def reverse_video_from_style(style):
    return 1 if style & (1 << 6) else 0


def generate_display_json(device: CduDevice, values: dict[str, str | bytes]):
    display_data = [[] for _ in range(CDU_CELLS)]

    for row, line in enumerate(DATAREF_LINES):
        text_key = device.get_text_dataref(line)
        style_key = device.get_style_dataref(line)

        if text_key not in values or style_key not in values:
            continue

        text = values[text_key].ljust(CDU_COLUMNS)
        style = values[style_key]

        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col

            char = text[col]
            if char == " ":
                continue

            display_data[index] = [
                char,
                color_from_style(style[col]),
                size_from_style(style[col]),
                reverse_video_from_style(style[col]),
            ]
    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(
            CduDevice,
            fetch_dataref_mapping,
            generate_display_json,
            decode_value=decode_value,
            font_name="Boeing",
        )
    )
