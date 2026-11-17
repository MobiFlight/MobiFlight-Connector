"""
Adds support for the X-Plane UNS-1.

See xplane_cdu_common.py for the shared connection/orchestration logic this script relies on.
"""

import asyncio
import base64
import json
import os
import sys
import urllib.request
from dataclasses import dataclass, KW_ONLY
from enum import StrEnum

# MobiFlight's embedded Python does not add the script directory to sys.path.
_here = os.path.dirname(os.path.abspath(__file__))
if _here not in sys.path:
    sys.path.insert(0, _here)

import xplane_cdu_common as cdu_common

CDU_COLUMNS = cdu_common.CDU_COLUMNS
CDU_ROWS = 11
CDU_CELLS = CDU_COLUMNS * CDU_ROWS

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
    "Ⓜ": "m",  # Circled Latin Capital Letter M
    "─": "-",       # Box Drawings Light Horizontal
    "│": "|",       # Box Drawings Light Vertical
    "┌": "-",       # Box Drawings Light Down and Right
    "┐": "-",       # Box Drawings Light Down and Left
    "└": "-",       # Box Drawings Light Up and Right
    "┘": "-",       # Box Drawings Light Up and Left
    "├": "|",       # Box Drawings Light Vertical and Right
    "┤": "|",       # Box Drawings Light Vertical and Left
    "┬": "-",       # Box Drawings Light Down and Horizontal
    "┴": "-",       # Box Drawings Light Up and Horizontal
}

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

    text: list[str]
    _: KW_ONLY
    style: int
    row_offset: int

    def draw_onto(self, text: list[str], style: list[bytes]):
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
], style=3, row_offset=3)

HOLD_R_INBOUND_ARROW = Overlay([
    r"            ←-          ",
], style=1, row_offset=8)

HOLD_R_DIR_ARROW = Overlay([
    r"          -   →         ",
], style=1, row_offset=4)

HOLD_R_TURN_ARROW = Overlay([
    r"      ↑                 ",
    r"       \                ",
], style=1, row_offset=5)

HOLD_R_TIME_ARROWS = Overlay([
    r"        |←    →|        ",
    r"        .               ",
], style=1, row_offset=6)

HOLD_L_OVERLAY = Overlay([
    r"       ----------       ",
    r"      /          \      ",
    r"      |          |      ",
    r"      \          /      ",
    r"      -----------       ",
], style=3, row_offset=3)

HOLD_L_INBOUND_ARROW = Overlay([
    r"          -→            ",
], style=1, row_offset=8)

HOLD_L_DIR_ARROW = Overlay([
    r"         ←   -          ",
], style=1, row_offset=4)

HOLD_L_TURN_ARROW = Overlay([
    r"                 ↑      ",
    r"                /       ",
], style=1, row_offset=5)

HOLD_L_TIME_ARROWS = Overlay([
    r"        |←    →|        ",
    r"               .        ",
], style=1, row_offset=6)


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "cdu1"
    CoPilot = "cdu2"


def get_prefix(q4xp: bool) -> str:
    return "FJS/Q4XP" if q4xp else "uns1"


def get_color_set_dataref(device: CduDevice, q4xp: bool) -> str:
    return f"{get_prefix(q4xp)}/{device}/color_set"


def get_hold_dir_dataref(device: CduDevice, q4xp: bool) -> str:
    return f"{get_prefix(q4xp)}/{device}/hold_dir"


def get_hold_graphics_dataref(device: CduDevice, q4xp: bool) -> str:
    return f"{get_prefix(q4xp)}/{device}/hold_graphics"


def get_text_dataref(device: CduDevice, q4xp: bool, line) -> str:
    return f"{get_prefix(q4xp)}/{device}/text_line_{line}"


def get_style_dataref(device: CduDevice, q4xp: bool, line) -> str:
    return f"{get_prefix(q4xp)}/{device}/style_line_{line}"


def get_dataref_prefixes(device: CduDevice, q4xp: bool) -> list[str]:
    return [
        f"{get_prefix(q4xp)}/{device}/text_line_",
        f"{get_prefix(q4xp)}/{device}/style_line_",
        get_color_set_dataref(device, q4xp),
        get_hold_dir_dataref(device, q4xp),
        get_hold_graphics_dataref(device, q4xp),
    ]


def fetch_dataref_mapping(device: CduDevice, q4xp: bool) -> dict[int, str]:
    prefixes = get_dataref_prefixes(device, q4xp)
    return cdu_common.fetch_dataref_mapping(
        lambda name: any(p in name for p in prefixes)
    )


def decode_value(dataref_name: str, value):
    if "text_line" in dataref_name:
        return base64.b64decode(value).decode(errors="replace").replace("\x00", " ")
    if "style_line" in dataref_name:
        return base64.b64decode(value)
    return value


def color_from_style(style, color_map):
    return color_map.get(style & 0xf, "w")


def size_from_style(style):
    return 0 if style & (1 << 7) else 1


def reverse_video_from_style(style):
    return 1 if style & (1 << 6) else 0


def generate_display_json(device: CduDevice, q4xp: bool, values: dict[str, str | bytes]):
    display_data = [[] for _ in range(CDU_CELLS)]

    text = [
        values.get(get_text_dataref(device, q4xp, row), "").ljust(CDU_COLUMNS)
        for row in range(CDU_ROWS)
    ]
    style = [
        values.get(get_style_dataref(device, q4xp, row), "").ljust(CDU_COLUMNS)
        for row in range(CDU_ROWS)
    ]

    color_set = values.get(get_color_set_dataref(device, q4xp), 0)
    color_map = COLOR_SETS.get(color_set, COLOR_MAP_3)

    hold_dir = values.get(get_hold_dir_dataref(device, q4xp), 0)
    hold_graphics = values.get(get_hold_graphics_dataref(device, q4xp), 0)

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
    with urllib.request.urlopen(cdu_common.BASE_REST_URL, timeout=5) as response:
        response_json = json.load(response)
        names = (x["name"] for x in response_json["data"])
        return "FJS/Q4XP/cdu1/cdu_model" in names


if __name__ == "__main__":
    # Determined once since every device on this aircraft shares the same answer.
    _q4xp = is_q4xp()

    def _fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
        return fetch_dataref_mapping(device, _q4xp)

    def _generate_display_json(device: CduDevice, values: dict[str, str | bytes]) -> str:
        return generate_display_json(device, _q4xp, values)

    asyncio.run(
        cdu_common.run(
            CduDevice,
            _fetch_dataref_mapping,
            _generate_display_json,
            decode_value=decode_value,
            font_name="Boeing",
        )
    )
