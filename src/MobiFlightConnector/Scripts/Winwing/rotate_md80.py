"""
MD80 MCDU Support for MobiFlight in X-Plane 12.
Adapted for the Rotate MD80 aircraft.

The MD80 has a single MCDU with green-only display.
This script fetches the CDU display data from X-Plane and sends it to MobiFlight
for display on physical CDU hardware.

MD80 CDU Characteristics:
- Only one MCDU (not separate Captain/CoPilot units)
- Only green color display
- Virtual CDU in X-Plane: 30 characters wide
- Physical display limit: 24 characters wide (intelligently trimmed)
- Alternating text sizes: even lines (1,3,5,7,9,11,13) are large text for data,
  odd lines (2,4,6,8,10,12) are small text for headers/labels. One exception is the last line (14) which is large text (input zone).
- Uses $ character for ballot boxes

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

CDU_COLUMNS = cdu_common.CDU_COLUMNS  # Physical display width limit
CDU_ROWS = cdu_common.CDU_ROWS
CDU_CELLS = cdu_common.CDU_CELLS
VIRTUAL_CDU_WIDTH = 30  # MD80 virtual CDU width in X-Plane

BALLOT_BOX = "☐"
DEGREES = "°"
CHAR_MAP = {
    "$": BALLOT_BOX,  # MD80 uses $ for ballot boxes
    "`": DEGREES,
}


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    # The MD80 only has a single MCDU; it's named Captain purely so it
    # resolves to the shared "Captain" websocket endpoint.
    Captain = "mcdu"

    def get_line_datarefs(self) -> list[str]:
        """Get list of dataref paths for all CDU lines"""
        datarefs = []
        for i in range(1, CDU_ROWS + 1):
            line_num = str(i).zfill(2)
            datarefs.append(f"Rotate/md80/instruments/cdu_line_{line_num}")
        return datarefs


def get_char(char: str) -> str:
    return CHAR_MAP.get(char, char)


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    line_datarefs = set(device.get_line_datarefs())
    return cdu_common.fetch_dataref_mapping(lambda name: name in line_datarefs)


def decode_value(dataref_name: str, value):
    if isinstance(value, str):
        try:
            text = base64.b64decode(value).decode("utf-8", errors="ignore")
        except Exception:
            text = value
    else:
        text = str(value)

    return text.replace("\x00", " ")


def trim_line_intelligently(line_text: str, target_width: int = CDU_COLUMNS) -> str:
    """
    Intelligently trim a line from virtual CDU width (30) to physical display width (24).
    Tries to preserve content by removing excess dashes, extra spaces, etc.
    """
    if len(line_text) <= target_width:
        return line_text.ljust(target_width)

    if line_text.startswith("-") and line_text.endswith("-"):
        while line_text.startswith("-") and line_text.endswith("-") and len(line_text) > target_width:
            line_text = line_text[1:-1]

    line_text = line_text.rstrip()
    if len(line_text) <= target_width:
        return line_text.ljust(target_width)

    while "  " in line_text and len(line_text) > target_width:
        line_text = line_text.replace("  ", " ", 1)

    return line_text[:target_width]


def generate_display_json(device: CduDevice, values: dict[str, str]) -> str:
    """
    Generate the display JSON for MobiFlight from CDU line data.
    MD80 specific: all text is green, alternating small/large text.
    Even lines (1,3,5,7,9,11,13) are large text (data).
    Odd lines (2,4,6,8,10,12) are small text (headers/labels).
    Line 14 (row_idx 13) is large text (input zone).
    Lines are trimmed from virtual width (30) to physical width (24).
    """
    display_data = [[] for _ in range(CDU_CELLS)]

    cdu_lines = [values.get(dr, "") for dr in device.get_line_datarefs()]

    for row_idx, line_text in enumerate(cdu_lines[:CDU_ROWS]):
        line_text = trim_line_intelligently(line_text, CDU_COLUMNS)

        # row_idx 0 = line 1 (odd) -> large; row_idx 1 = line 2 (even) -> small
        text_size = 0 if (row_idx % 2 == 0) else 1
        if row_idx == 13:  # Last line (14) is large text
            text_size = 0

        for col_idx, char in enumerate(line_text):
            if char != " ":
                cell_idx = row_idx * CDU_COLUMNS + col_idx
                display_data[cell_idx] = [get_char(char), "g", text_size]

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(cdu_common.run(CduDevice, fetch_dataref_mapping, generate_display_json, decode_value=decode_value))
