"""
Adds support for the default X-Plane FMS dataref family
(sim/cockpit2/radios/indicators/fms_cduN_text/style_lineX), used by the stock FMS
and by aircraft that reuse it, such as the X-Crafts ERJ family.

DYNAMIC LINE COUNT / SCRATCHPAD HANDLING:
Not every aircraft on this dataref family publishes the same number of lines. The stock/default
FMS publishes a full 14 lines (0-13), one per WinWing grid row, mapped 1:1. The X-Crafts ERJ only
publishes 9 lines (0-8): lines 0-7 are page content, and line 8 is the scratchpad / text entry
line, which on the real ERJ MCDU is shown on the bottom-most row of the screen rather than
directly beneath line 7.

Rather than hardcoding a line count and hardcoding this exception for the ERJ specifically, the
number of lines an aircraft actually publishes is discovered per-device from the datarefs
fetch_dataref_mapping() returns (build_line_row_map()). If an aircraft publishes a full 14 lines,
each one maps directly to the matching grid row, same as before. If it publishes fewer, every line
except the last maps directly to its matching grid row, and the last (the scratchpad) is mirrored
to the bottom-most grid row (CDU_ROWS - 1). This lets one script serve both the default FMS and
reduced implementations like the ERJ's without any aircraft-specific branching.

See xplane_cdu_common.py for the shared connection/orchestration logic this script relies on.
"""

import asyncio
import base64
import json
import logging
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
    Captain = "fms_cdu1"
    CoPilot = "fms_cdu2"
    Observer = "fms_cdu3"

    def get_dataref_prefix(self) -> str:
        return f"sim/cockpit2/radios/indicators/{self}"

    def get_text_dataref(self, line) -> str:
        return f"sim/cockpit2/radios/indicators/{self}_text_line{line}"

    def get_style_dataref(self, line) -> str:
        return f"sim/cockpit2/radios/indicators/{self}_style_line{line}"


# Keyed by device so generate_display_json() can reuse the map fetch_dataref_mapping() built.
_line_row_maps: dict[CduDevice, dict[int, int]] = {}


def build_line_row_map(dataref_map: dict[int, str]) -> dict[int, int]:
    """
    Determines which WinWing grid row each FMS dataref line should be rendered on.

    A line is considered available only when both its text and style datarefs
    exist. Lines must be contiguous starting from line 0.

    Full 14-line FMS implementations map directly to the 14-row display.
    Reduced implementations, such as the X-Crafts ERJ, map the final published
    line to the bottom row as the scratchpad.
    """
    text_lines = set()
    style_lines = set()

    for name in dataref_map.values():
        match = LINE_PATTERN.search(name)
        if not match:
            continue

        line = int(match.group(1))

        if "_text_line" in name:
            text_lines.add(line)
        elif "_style_line" in name:
            style_lines.add(line)

    complete_lines = text_lines & style_lines

    line_count = 0
    while line_count in complete_lines:
        line_count += 1

    if line_count == 0:
        return {}

    logging.info("Discovered %d FMS lines", line_count)

    if line_count >= CDU_ROWS:
        return {line: line for line in range(CDU_ROWS)}

    content_line_count = line_count - 1
    row_map = {line: line for line in range(content_line_count)}
    row_map[content_line_count] = CDU_ROWS - 1

    return row_map


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    dataref_map = cdu_common.fetch_dataref_mapping(
        lambda name: device.get_dataref_prefix() in name
    )

    line_row_map = build_line_row_map(dataref_map)
    if not line_row_map:
        logging.warning("No FMS lines discovered for device %s", device)
    _line_row_maps[device] = line_row_map

    return dataref_map


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
    return COLOR_MAP.get(style & 0xf, "w")


def size_from_style(style):
    return 0 if style & (1 << 7) else 1


def reverse_video_from_style(style):
    return 1 if style & (1 << 6) else 0


def render_fms_line(
    device: CduDevice,
    values: dict[str, str | bytes],
    fms_line: int,
    grid_row: int,
    display_data: list,
) -> None:
    """
    Renders a single FMS dataref line (fms_line) into a specific row of the
    WinWing's 14-row display grid (grid_row, 0-13). The FMS line and the grid row
    don't have to match - this is what lets a scratchpad line be placed on the
    bottom grid row instead of directly under the aircraft's last content line.

    If we haven't yet received both the text and style values for this FMS line
    (e.g. during the first update after connecting, before X-Plane has pushed
    every subscribed dataref), the row is simply left blank instead of raising a
    KeyError.
    """
    text_key = device.get_text_dataref(fms_line)
    style_key = device.get_style_dataref(fms_line)

    if text_key not in values or style_key not in values:
        return

    # Strings are sometimes shorter than a full line, so pad with spaces to the expected width.
    text = values[text_key].ljust(CDU_COLUMNS)
    style = values[style_key]

    # Style bytes can also arrive shorter than the text they describe; guard
    # against that so a short style buffer doesn't raise an IndexError either.
    for col in range(min(CDU_COLUMNS, len(style))):
        char = text[col]

        if char == " ":
            continue

        index = grid_row * CDU_COLUMNS + col
        color = color_from_style(style[col])
        size = size_from_style(style[col])
        reverse_video = reverse_video_from_style(style[col])

        display_data[index] = [char, color, size, reverse_video]


def generate_display_json(device: CduDevice, values: dict[str, str | bytes]) -> str:
    display_data = [[] for _ in range(CDU_CELLS)]

    line_row_map = _line_row_maps.get(device, {})
    for fms_line, grid_row in line_row_map.items():
        render_fms_line(device, values, fms_line, grid_row, display_data)

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(
            CduDevice, fetch_dataref_mapping, generate_display_json, decode_value=decode_value, font_name="Boeing"
        )
    )
