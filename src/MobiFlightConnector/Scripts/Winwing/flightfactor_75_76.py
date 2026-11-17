"""
Adds support for the FlightFactor Boeing 757/767 in X-Plane.

See xplane_cdu_common.py for the shared connection/orchestration logic this script relies on.
"""

import asyncio
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

BALLOT_BOX = "☐"
DEGREES = "°"

CHAR_MAP = {"\x1d": BALLOT_BOX, "\x1c": DEGREES}


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "cduL"
    CoPilot = "cduR"

    def get_symbol_dataref(self) -> str:
        return f"1-sim/{self}/display/symbols"

    def get_symbol_color_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsColor"

    def get_symbol_size_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsSize"


def get_char(char: str) -> str:
    return CHAR_MAP.get(char, char)


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: device.get_symbol_dataref() in name
    )


def generate_display_json(device: CduDevice, values: dict[str, str]):
    display_data = [[] for _ in range(CDU_ROWS * CDU_COLUMNS)]

    cdu_lines = [
        (char, size, color)
        for char, size, color in zip(
            values[device.get_symbol_dataref()],
            values[device.get_symbol_size_dataref()],
            values[device.get_symbol_color_dataref()],
        )
    ]

    for row in range(CDU_ROWS):
        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col

            char, size, _ = cdu_lines[index]
            if char == " ":
                continue

            display_data[index] = [get_char(char), "g", size]

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(cdu_common.run(CduDevice, fetch_dataref_mapping, generate_display_json))
