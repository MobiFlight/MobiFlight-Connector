"""
Adds support for the IXEG 737 in X-Plane.

See xplane_cdu_common.py for the shared connection/orchestration logic this script relies on.
"""

from __future__ import annotations

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
    "'": "°",
    "#": "☐",
}


class StyledText:
    SMALL_FONT = (1 << 0)
    REVERSE_VIDEO = (1 << 1)

    def __init__(self, text: str, styles: list[int]):
        assert len(text) == len(styles)
        self.text = text
        self.styles = styles

    def __add__(self, other):
        return StyledText(self.text + other.text, self.styles + other.styles)

    def __len__(self):
        return len(self.text)

    @classmethod
    def from_bytes(cls, values: bytes):
        cur_style = 0
        text = ""
        styles = []
        for v in list(values):
            if v == 0:
                break
            if v == 0x26:
                cur_style |= cls.REVERSE_VIDEO
            elif v == 0xa3:
                cur_style |= cls.SMALL_FONT
            elif v == 0xa4:
                cur_style = 0
            elif v >= 0x80:
                # Anything non-ASCII is likely a style directive that we don't
                # support, so just skip.
                pass
            else:
                # The datarefs sometimes contain lower-case characters, but
                # the CDU in the virtual cockpit always displays them as
                # upper case, so we do the same.
                char = chr(v).upper()
                char = CHAR_MAP.get(char, char)
                text = text + char
                styles.append(cur_style)

        return StyledText(text, styles)

    @classmethod
    def padding(cls, length: int) -> "StyledText":
        return StyledText(" " * length, [0] * length)

    def pad_to(self, length: int) -> "StyledText":
        return self + StyledText.padding(length - len(self))

    def char_at(self, i: int) -> str:
        return self.text[i]

    def size_at(self, i: int) -> int:
        return 1 if self.styles[i] & self.SMALL_FONT else 0

    def reverse_video_at(self, i: int) -> int:
        return 1 if self.styles[i] & self.REVERSE_VIDEO else 0

    def slice(self, start: int, stop: int) -> "StyledText":
        return StyledText(self.text[start:stop], self.styles[start:stop])

    def lstrip(self) -> "StyledText":
        new_text = self.text.lstrip()
        new_styles = self.styles[len(self.text) - len(new_text):]

        return StyledText(new_text, new_styles)

    def rstrip(self) -> "StyledText":
        new_text = self.text.rstrip()
        new_styles = self.styles[:len(new_text)]

        return StyledText(new_text, new_styles)


class CduDevice(cdu_common.CduDeviceMixin, StrEnum):
    Captain = "cdu1D"
    CoPilot = "cdu2D"

    def get_dataref_prefix(self) -> str:
        return f"ixeg/733/FMC/{self}"

    def get_line_text(self, line, values) -> StyledText:
        if line == 0:
            title = StyledText.from_bytes(values[f"ixeg/733/FMC/{self}_title"])
            page = StyledText.from_bytes(values[f"ixeg/733/FMC/{self}_pg_number"])
            title = title.pad_to(19).slice(0, 19)
            return (title + page).pad_to(CDU_COLUMNS)
        if line == 13:
            return StyledText.from_bytes(values[f"ixeg/733/FMC/{self}_scrpad"]).pad_to(CDU_COLUMNS)
        logical_line = (line + 1) // 2
        dataref_base = f"ixeg/733/FMC/{self}_line{logical_line}"
        suffix = "_d" if line % 2 == 0 else "_t"

        left = StyledText.from_bytes(values[f"{dataref_base}L{suffix}"]).rstrip()
        right = StyledText.from_bytes(values[f"{dataref_base}R{suffix}"]).lstrip()

        return left.pad_to(CDU_COLUMNS - len(right)) + right


def fetch_dataref_mapping(device: CduDevice) -> dict[int, str]:
    return cdu_common.fetch_dataref_mapping(
        lambda name: device.get_dataref_prefix() in name
    )


def decode_value(dataref_name: str, value):
    # StyledText.from_bytes() does its own decoding, so keep this as bytes.
    return base64.b64decode(value)


def generate_display_json(device: CduDevice, values: dict[str, bytes]):
    display_data = [[] for _ in range(CDU_CELLS)]

    color = "g"

    for row in range(CDU_ROWS):
        text = device.get_line_text(row, values)

        for col in range(CDU_COLUMNS):
            char = text.char_at(col)
            if char == " ":
                continue

            size = text.size_at(col)
            reverse_video = text.reverse_video_at(col)

            index = row * CDU_COLUMNS + col
            display_data[index] = [char, color, size, reverse_video]

    return json.dumps({"Target": "Display", "Data": display_data})


if __name__ == "__main__":
    asyncio.run(
        cdu_common.run(
            CduDevice, fetch_dataref_mapping, generate_display_json, decode_value=decode_value, font_name="Boeing"
        )
    )
