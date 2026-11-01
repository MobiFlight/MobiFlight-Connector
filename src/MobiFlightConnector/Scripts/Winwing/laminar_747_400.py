"""
Adds support for the Laminar 747-400 FMS in X-Plane.

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

CHAR_MAP = {
    "*": "☐",
    "`": "°",
}


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    # Not a typo -- for some strange reason, the Captain's FMS is number 3.
    Captain = "fms3"
    CoPilot = "fms2"
    Observer = "fms1"

    def get_dataref_prefix(self) -> str:
        return f"laminar/B747/{self}/Line"

    def get_large_text_dataref(self, line) -> str:
        return f"laminar/B747/{self}/Line{line+1:02d}_L"

    def get_small_text_dataref(self, line) -> str:
        return f"laminar/B747/{self}/Line{line+1:02d}_S"


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: device.get_dataref_prefix() in name
    )


def decode_value(dataref_name: str, value):
    # Some lines contain characters that aren't valid UTF-8; replace rather
    # than raise so a single bad line doesn't take down the whole display.
    return base64.b64decode(value).decode(errors="replace").replace("\x00", " ")


def generate_display_json(device: CduDevice, values: dict[str, str]):
    display_data = [[] for _ in range(CDU_CELLS)]

    for row in range(CDU_ROWS):
        # Strings are sometimes empty, so pad with spaces to the full expected width.
        large_text = values[device.get_large_text_dataref(row)].ljust(CDU_COLUMNS)
        small_text = values[device.get_small_text_dataref(row)].ljust(CDU_COLUMNS)

        for col in range(CDU_COLUMNS):
            index = row * CDU_COLUMNS + col

            # The dataref and WinWing both use Unicode, so no conversion
            # of special characters is necessary.
            char = large_text[col]
            size = 0
            if char == " ":
                char = small_text[col]
                size = 1
            if char == " ":
                continue

            # The datarefs sometimes contain lower-case characters, but the CDU
            # in the virtual cockpit always displays them as upper case, so we
            # do the same.
            char = char.upper()

            char = CHAR_MAP.get(char, char)

            color = "g"
            reverse_video = 0

            display_data[index] = [char, color, size, reverse_video]

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(
            CduDevice, fetch_dataref_mapping, generate_display_json, decode_value=decode_value, font_name="Boeing"
        )
    )
