"""
Adds support for the FlightFactor Boeing 777v2 in X-Plane.

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

CHAR_MAP = {"#": BALLOT_BOX, "*": DEGREES}
COLOR_MAP = {1: "w", 2: "m", 3: "g", 4: "c", 5: "e", 6: "c"}


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "cduL"
    CoPilot = "cduR"
    Observer = "cduC"

    def get_symbol_dataref(self) -> str:
        return f"1-sim/{self}/display/symbols"

    def get_symbol_color_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsColor"

    def get_symbol_size_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsSize"

    def get_symbol_effects_dataref(self) -> str:
        return f"1-sim/{self}/display/symbolsEffects"


def get_char(char: str) -> str:
    return CHAR_MAP.get(char, char)


def get_color(color: int) -> str:
    return COLOR_MAP.get(color, "w")


def get_size(size: int) -> int:
    # FlightFactor's 777v2 size starts at 1
    # 1 = large
    # 2 = small
    return 1 if size == 2 else 0


def get_style(effect: int) -> int:
    # FlightFactor's 777v2 style uses the following values
    # 0 = normal
    # 1 = highlighted
    # 2 = reverse
    return 1 if effect == 1 else 0


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: device.get_symbol_dataref() in name, strip_names=True
    )


def generate_display_json(device: CduDevice, values: dict[str, str]):
    display_data = [[] for _ in range(CDU_ROWS * CDU_COLUMNS)]

    cdu_lines = [
        (char, size, color, effect)
        for char, size, color, effect in zip(
            values[device.get_symbol_dataref()],
            values[device.get_symbol_size_dataref()],
            values[device.get_symbol_color_dataref()],
            values[device.get_symbol_effects_dataref()],
        )
    ]

    for row in range(CDU_ROWS):
        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col

            char, size, color, effect = cdu_lines[index]
            if char == " ":
                continue

            display_data[index] = [
                get_char(char),
                get_color(color),
                get_size(size),
                get_style(effect),
            ]

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(CduDevice, fetch_dataref_mapping, generate_display_json, font_name="Boeing")
    )
