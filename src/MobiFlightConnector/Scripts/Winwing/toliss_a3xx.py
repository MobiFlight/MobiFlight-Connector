"""
Adds support for the ToLiss Airbus family in X-Plane.

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

BALLOT_BOX = "☐"
UP_ARROW = "↑"
DOWN_ARROW = "↓"
LEFT_ARROW = "←"
RIGHT_ARROW = "→"
GREEK_DELTA = "Δ"
DEGREES = "°"

CONTENT_MAP = {"`": DEGREES, "|": GREEK_DELTA}

SYMBOL_MAP = CONTENT_MAP | {
    "A": "[",
    "B": "]",
    "E": BALLOT_BOX,
    "0": LEFT_ARROW,
    "1": RIGHT_ARROW,
    "2": LEFT_ARROW,
    "3": RIGHT_ARROW,
    "4": LEFT_ARROW,
    "5": RIGHT_ARROW,
    "C": UP_ARROW,
    "D": DOWN_ARROW,
}

COLOR_MAP = {"b": "c"}

SYMBOL_COLOR_MAP = {"E": "a", "4": "a", "5": "a"}


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "MCDU1"
    CoPilot = "MCDU2"


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: name.startswith(f"AirbusFBW/{device}")
    )


def process_slew_keys(value: int) -> str:
    match value:
        case 1:
            result = f"{UP_ARROW}{DOWN_ARROW}"
        case 2:
            result = f"{UP_ARROW} "
        case 3:
            result = f"{DOWN_ARROW}"
        case _:
            result = ""

    return result.rjust(24)


def decode_value(dataref_name: str, value):
    if dataref_name.endswith("VertSlewKeys"):
        return process_slew_keys(value)

    return cdu_common.default_decode_value(dataref_name, value)


def get_color(dataref_name: str, char: str):
    suffix = dataref_name[-1]

    if (
        ("label" in dataref_name or "title" in dataref_name) and suffix == "s"
    ) or dataref_name.endswith("VertSlewKeys"):
        return "w"
    elif suffix == "s":
        return SYMBOL_COLOR_MAP.get(char, "c")

    return COLOR_MAP.get(suffix, suffix)


def get_char(dataref_name: str, char: str) -> str:
    suffix = dataref_name[-1]

    if suffix == "s":
        return SYMBOL_MAP.get(char, char)

    return CONTENT_MAP.get(char, char)


def get_size(dataref_name):
    return (
        1
        if "scont" in dataref_name
        or ("label" in dataref_name and "labelL" not in dataref_name)
        else 0
    )


def process_cdu_line(line_datarefs: dict[str, str], row: int) -> list[list]:
    line_chars = [[] for _ in range(CDU_COLUMNS)]

    target_suffix = "label" if row % 2 == 0 else "cont"

    for dataref, text in line_datarefs.items():
        if not text or text.isspace():
            continue

        # The first and last rows only cover a single row. All other rows cover 2 rows between the label and the main content (scont, cont)
        if (row != 0 and row != 14) and not target_suffix in dataref:
            continue

        for i, char in enumerate(text[:CDU_COLUMNS]):
            if char == " ":
                continue

            line_chars[i] = (
                get_char(dataref, char),
                get_color(dataref, char),
                get_size(dataref),
            )

    return line_chars


def group_datarefs_by_line(values: dict[str, str]) -> dict[int, dict[str, str]]:
    grouped_datarefs = {}

    for dataref, value in values.items():
        try:
            line_num = (
                0
                if "title" in dataref
                else (
                    7
                    if dataref.endswith("spa")
                    or dataref.endswith("spw")
                    or dataref.endswith("VertSlewKeys")
                    else int(next(i for i in list(dataref[::-1]) if i.isdigit()))
                )
            )
            if line_num not in grouped_datarefs:
                grouped_datarefs[line_num] = {}

            grouped_datarefs[line_num][dataref] = value
        except Exception:
            pass

    return grouped_datarefs


def generate_display_json(device: CduDevice, values: dict[str, str]):
    display_data = [[] for _ in range(CDU_ROWS * CDU_COLUMNS)]

    grouped_datarefs = group_datarefs_by_line(values)

    for row in range(CDU_ROWS + 1):
        if (
            row == 1
        ):  # Line 0 covers a single row. Skip row 1 as this will be populated by Line01
            continue

        dataref_line = row // 2 if row > 0 else 0

        line_datarefs = grouped_datarefs.get(dataref_line, {})
        if not line_datarefs:
            continue

        line_chars = process_cdu_line(line_datarefs, row)

        start_index = (row - 1 if row > 0 else row) * CDU_COLUMNS
        display_data[start_index : start_index + CDU_COLUMNS] = (
            line_chars  # Bulk assign the entire row
        )

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(
            CduDevice, fetch_dataref_mapping, generate_display_json, decode_value=decode_value
        )
    )
