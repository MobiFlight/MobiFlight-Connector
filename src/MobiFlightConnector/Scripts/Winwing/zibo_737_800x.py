"""
Adds support for the Zibo 737 in X-Plane.

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
CDU_CELLS = cdu_common.CDU_CELLS

CHARACTER_MAPPING = {"`": "°", "*": "☐", "=": "*"}
COLOR_MAPPING = {"G": "g", "C": "c", "M": "m"}


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "fmc1"
    CoPilot = "fmc2"


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: name.startswith(f"laminar/B738/{device}")
    )


def get_color(dataref: str) -> str:
    dataref_ending = dataref[dataref.rindex("_") + 1]

    return COLOR_MAPPING.get(dataref_ending, "w")


def get_size(dataref: str) -> int:
    return 1 if dataref.endswith("_X") or dataref.endswith("_S") else 0


def get_style(dataref: str) -> int:
    dataref_ending = dataref[dataref.rindex("_") + 1]

    return 1 if dataref_ending == "I" else 0


def process_cdu_line(line_datarefs: dict[str, str], row: int) -> list[list]:
    line_chars = [[] for _ in range(CDU_COLUMNS)]

    target_suffixes = (
        ["_X", "_LX", "_GX"] if row % 2 == 0 else ["_G", "_L", "_M", "_S", "_I", "_SI"]
    )

    for dataref, text in line_datarefs.items():
        if not text or text.isspace():
            continue

        # The first and last rows only cover a single row. All other rows cover 2 rows between the label (X, LX or GX) and the main content (G, L, M, S, I or SI)
        if (row != 0 and row != 14) and not any(
            dataref.endswith(suffix) for suffix in target_suffixes
        ):
            continue

        for i, char in enumerate(text[:CDU_COLUMNS]):
            if char == " ":
                continue

            line_chars[i] = (
                CHARACTER_MAPPING.get(char, char),
                get_color(dataref),
                get_size(dataref),
                get_style(dataref),
            )

    return line_chars


def group_datarefs_by_line(values: dict[str, str]) -> dict[int, dict[str, str]]:
    grouped_datarefs = {}

    for dataref, value in values.items():
        dataref_name = dataref[dataref.rindex("/") + 1 :]
        line_num = (
            7 if dataref_name.startswith("Line_entry") else int(dataref_name[4:6])
        )

        if line_num not in grouped_datarefs:
            grouped_datarefs[line_num] = {}
        grouped_datarefs[line_num][dataref] = value

    return grouped_datarefs


def generate_display_json(device: CduDevice, values: dict[str, str]) -> str:
    display_data = [[] for _ in range(CDU_CELLS)]

    grouped_datarefs = group_datarefs_by_line(values)

    for row in range(CDU_ROWS + 1):
        if row == 1:
            continue

        dataref_line = row // 2 if row > 0 else 0

        line_datarefs = grouped_datarefs.get(dataref_line, {})
        if not line_datarefs:
            continue

        line_chars = process_cdu_line(line_datarefs, row)

        start_index = (row - 1 if row > 0 else row) * CDU_COLUMNS
        display_data[start_index : start_index + CDU_COLUMNS] = line_chars

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(CduDevice, fetch_dataref_mapping, generate_display_json, font_name="Boeing")
    )
