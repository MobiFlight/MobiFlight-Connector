"""
Adds support for the Rotate MD-11 in X-Plane.

See xplane_cdu_common.py for the shared connection/orchestration logic this script relies on.
"""

import asyncio
import base64
import json
import os
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

BALLOT_BOX = "☐"
DEGREES = "°"

CHAR_MAP = {"$": BALLOT_BOX, "`": DEGREES}
COLOR_MAP = {1: "g", 2: "g", 4: "e", 5: "g"}


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "cdu_0"
    CoPilot = "cdu_1"
    Observer = "cdu_2"

    def get_dataref_base(self) -> str:
        return f"Rotate/aircraft/controls/{self}/mcdu_line"

    def get_content_dataref(self, i: int) -> str:
        return f"Rotate/aircraft/controls/{self}/mcdu_line_{i}_content"

    def get_style_dataref(self, i: int) -> str:
        return f"Rotate/aircraft/controls/{self}/mcdu_line_{i}_style"


def get_char(char: str | int) -> str:
    if isinstance(char, str) and len(char) == 1:
        return CHAR_MAP.get(char, char)
    try:
        return chr(int(char))
    except Exception:
        return " "


def get_color(color: int) -> str:
    return COLOR_MAP.get(color, "w")


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: name.startswith(device.get_dataref_base())
    )


def decode_value(dataref_name: str, value):
    return (
        base64.b64decode(value).decode(errors="ignore").replace("\x00", " ")
        if isinstance(value, str)
        else value
    )


def generate_display_json(device: CduDevice, values: dict[str, str]) -> str:
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
