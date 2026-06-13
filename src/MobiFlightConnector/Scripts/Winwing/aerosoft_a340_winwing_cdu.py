import asyncio
import ctypes
import functools
import json
import logging
import struct
from ctypes import wintypes
from typing import Any, Optional

import websockets.asyncio.client as ws_client
from SimConnect import SimConnect, Enum
from SimConnect.Enum import SIMCONNECT_CLIENT_DATA_ID, SIMCONNECT_RECV_ID, SIMCONNECT_RECV_CLIENT_DATA

# --- MobiFlight endpoints ---
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
CO_PILOT_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"
OBSERVER_CDU_URL: str = "ws://localhost:8320/winwing/cdu-observer"

# --- Display dimensions ---
CDU_COLUMNS: int = 24
CDU_ROWS: int = 14
CDU_CELLS: int = CDU_COLUMNS * CDU_ROWS

# --- SegmentParams layout (10 bytes per cell, row-major) ---
# byte 0:   c       (character)
# byte 1:   f       (FontSize: 0=Large, 1=Small)
# bytes 2-5: col    (r, g, b, a  foreground)
# bytes 6-9: bgCol  (r, g, b, a  background)
CELL_BYTE_COUNT: int = 10
CDU_DATA_SIZE: int = CDU_CELLS * CELL_BYTE_COUNT

# --- Aerosoft A340 SimConnect client data areas ---                   
MCDU1_NAME: str = "as_hw_intf_api_sc_client_area_mcdu1_screen"
MCDU2_NAME: str = "as_hw_intf_api_sc_client_area_mcdu2_screen"
MCDU3_NAME: str = "as_hw_intf_api_sc_client_area_mcdu3_screen"

MCDU1_CLIENT_DATA_ID: int = 100
MCDU2_CLIENT_DATA_ID: int = 101
MCDU3_CLIENT_DATA_ID: int = 102

MCDU1_DEFINITION_ID: int = 0
MCDU2_DEFINITION_ID: int = 1
MCDU3_DEFINITION_ID: int = 2

# --- Special character substitutions ---
SPECIAL_CHARS: dict[str, str] = {
    "^": "Δ",   # overfly / delta  \u0394
    "`": "°",   # degree           \u00b0
    "?": "☐",   # ballot box       \u2610
    "|": "↑",   # up arrow         \u2191
    "\\": "↓",  # down arrow       \u2193
    "{": "←",   # left arrow       \u2190
    "}": "→",   # right arrow      \u2192
}

# --- RGB reference values per MobiFlight colour code (from WinWing SDK) ---
COLOR_REFS: dict[str, tuple[int, int, int]] = {
    "w": (255, 255, 255),  # white
    "g": ( 32, 255,  32),  # green
    "c": (127, 255, 255),  # cyan
    "o": (  0, 150, 255),  # blue   (no SDK reference; approximate)
    "a": (255, 187,   0),  # amber
    "m": (255, 127, 255),  # magenta
    "r": (255,   0,   0),  # red
    "y": (253, 223,   0),  # yellow
    "e": (128, 128, 128),  # grey
}

# Sum of R+G+B below this value is treated as black (no reverse)
BG_BLACK_THRESHOLD: int = 30

# cache returns already computed for identical RGB inputs to speed up processing of large displays with many cells of the same colour
@functools.lru_cache(maxsize=None)
def rgb_to_mobi_colour(r: int, g: int, b: int) -> str:
    best_code = "w"
    best_dist = float("inf")
    for code, (cr, cg, cb) in COLOR_REFS.items():
        dist = (r - cr) ** 2 + (g - cg) ** 2 + (b - cb) ** 2
        if dist < best_dist:
            best_dist = dist
            best_code = code
    return best_code


def create_mobi_json(data: bytes) -> str:
    message: dict = {"Target": "Display", "Data": [[] for _ in range(CDU_CELLS)]}

    for row in range(CDU_ROWS):
        for col in range(CDU_COLUMNS):
            cell_idx = row * CDU_COLUMNS + col
            src = cell_idx * CELL_BYTE_COUNT

            if src + CELL_BYTE_COUNT > len(data):
                continue

            try:
                char = chr(data[src])
                if char in (" ", "\0"):
                    continue

                font_size = 1 if data[src + 1] else 0
                fg_r, fg_g, fg_b = data[src + 2], data[src + 3], data[src + 4]
                bg_r, bg_g, bg_b = data[src + 6], data[src + 7], data[src + 8]

                char = SPECIAL_CHARS.get(char, char)
                colour = rgb_to_mobi_colour(fg_r, fg_g, fg_b)
                reverse = 1 if (bg_r + bg_g + bg_b) > BG_BLACK_THRESHOLD else 0

                message["Data"][cell_idx] = [char, colour, font_size, reverse]

            except Exception as e:
                logging.debug("Error processing cell (%d, %d): %s", row, col, e)

    return json.dumps(message)


class SimConnectMobiFlight(SimConnect):
    def __init__(self, auto_connect: bool = True, library_path: Optional[str] = None) -> None:
        self.client_data_handlers: list = []
        if library_path:
            super().__init__(auto_connect, library_path)
        else:
            super().__init__(auto_connect)
        self.dll.MapClientDataNameToID.argtypes = [
            wintypes.HANDLE, ctypes.c_char_p, SIMCONNECT_CLIENT_DATA_ID
        ]

    def register_client_data_handler(self, handler: Any) -> None:
        if handler not in self.client_data_handlers:
            self.client_data_handlers.append(handler)

    def unregister_client_data_handler(self, handler: Any) -> None:
        if handler in self.client_data_handlers:
            self.client_data_handlers.remove(handler)

    def my_dispatch_proc(self, pData: Any, cbData: Any, pContext: Any) -> None:
        dwID = pData.contents.dwID
        if dwID == SIMCONNECT_RECV_ID.SIMCONNECT_RECV_ID_CLIENT_DATA:
            client_data = ctypes.cast(
                pData, ctypes.POINTER(SIMCONNECT_RECV_CLIENT_DATA)
            ).contents
            for handler in self.client_data_handlers:
                handler(client_data)
        else:
            super().my_dispatch_proc(pData, cbData, pContext)


class MobiFlightClient:
    def __init__(self, websocket_uri: str, max_retries: int = 3) -> None:
        self.websocket: Optional[ws_client.ClientConnection] = None
        self.connected: asyncio.Event = asyncio.Event()
        self.websocket_uri: str = websocket_uri
        self.retries: int = 0
        self.max_retries: int = max_retries

    async def run(self) -> None:
        while self.retries < self.max_retries:
            try:
                if self.websocket is None:
                    logging.info("Connecting to MobiFlight at %s", self.websocket_uri)
                    self.websocket = await ws_client.connect(
                        self.websocket_uri, ping_interval=None
                    )
                    logging.info("MobiFlight connected at %s", self.websocket_uri)
                    await self.websocket.send('{"Target": "Font", "Data": "AirbusThales"}')
                    await asyncio.sleep(1)
                    self.connected.set()
                await self.websocket.recv()
            except Exception as e:
                self.retries += 1
                logging.info(
                    "Connection not possible at %s: (attempt %d/%d)",
                    self.websocket_uri, self.retries, self.max_retries,
                )
                self.websocket = None
                self.connected.clear()
            await asyncio.sleep(5)
        logging.info(
            "Max retries reached for %s. "
            "If you only have one CDU attached, you can ignore this message.",
            self.websocket_uri,
        )
        self.connected.set()

    async def send(self, data: str) -> None:
        if self.websocket and self.connected.is_set():
            await self.websocket.send(data)

    async def close(self) -> None:
        if self.websocket:
            await self.websocket.close()
            self.websocket = None
            self.connected.clear()


class AerosoftA340MCDUClient:
    def __init__(
        self,
        sc: SimConnectMobiFlight,
        websocket_uri: str,
        mcdu_name: str,
        client_data_id: int,
        definition_id: int,
    ) -> None:
        self.sc = sc
        self.mobiflight = MobiFlightClient(websocket_uri)
        self.mcdu_name = mcdu_name
        self.client_data_id = client_data_id
        self.definition_id = definition_id
        self.event_loop: Optional[asyncio.AbstractEventLoop] = None

    def setup_simconnect(self) -> bool:
        try:
            h = self.sc.hSimConnect
            self.sc.dll.MapClientDataNameToID(h, self.mcdu_name.encode(), self.client_data_id)
            self.sc.dll.AddToClientDataDefinition(h, self.definition_id, 0, CDU_DATA_SIZE, 0, 0)
            self.sc.dll.RequestClientData(
                h,
                self.client_data_id,
                self.definition_id,
                self.definition_id,
                Enum.SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_VISUAL_FRAME,
                Enum.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG_CHANGED,
                0, 0, 0,
            )
            self.sc.register_client_data_handler(self.handle_cdu_data)
            logging.info("SimConnect initialised for %s", self.mcdu_name)
            return True
        except Exception as e:
            logging.error("SimConnect setup failed for %s: %s", self.mcdu_name, e)
            return False

    def handle_cdu_data(self, client_data: Any) -> None:
        if self.event_loop is None:
            return
        if client_data.dwDefineID != self.definition_id or not hasattr(client_data, "dwData"):
            return
        try:
            int_count = CDU_DATA_SIZE // 4
            if len(client_data.dwData) < int_count:
                logging.warning(
                    "Incomplete CDU data for %s: got %d ints, expected %d",
                    self.mcdu_name, len(client_data.dwData), int_count,
                )
                return
            data = struct.pack(f"{int_count}I", *client_data.dwData[:int_count])
            asyncio.run_coroutine_threadsafe(
                self.mobiflight.send(create_mobi_json(data)), self.event_loop
            )
        except Exception as e:
            logging.error("Error handling CDU data for %s: %s", self.mcdu_name, e)

    async def run(self) -> None:
        self.event_loop = asyncio.get_running_loop()
        try:
            mobiflight_task = asyncio.create_task(self.mobiflight.run())
            await self.mobiflight.connected.wait()
            if self.mobiflight.retries >= self.mobiflight.max_retries:
                logging.info("Failed to connect to MobiFlight for %s", self.mcdu_name)
                return
            if self.setup_simconnect():
                await asyncio.gather(mobiflight_task)
            else:
                logging.error("SimConnect initialisation failed for %s", self.mcdu_name)
        except Exception as e:
            logging.error("Error in MCDU client for %s: %s", self.mcdu_name, e)
        finally:
            await self.mobiflight.close()


if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format="%(asctime)s - %(levelname)s - %(message)s",
    )
    # Startup log must remain after basicConfig to ensure the handler is registered
    logging.info("---- Aerosoft Toliss A340 MCDU to WinWing CDU Integration ----")

    sc = SimConnectMobiFlight()

    mcdu1 = AerosoftA340MCDUClient(
        sc, CAPTAIN_CDU_URL, MCDU1_NAME, MCDU1_CLIENT_DATA_ID, MCDU1_DEFINITION_ID
    )
    mcdu2 = AerosoftA340MCDUClient(
        sc, CO_PILOT_CDU_URL, MCDU2_NAME, MCDU2_CLIENT_DATA_ID, MCDU2_DEFINITION_ID
    )
    mcdu3 = AerosoftA340MCDUClient(
        sc, OBSERVER_CDU_URL, MCDU3_NAME, MCDU3_CLIENT_DATA_ID, MCDU3_DEFINITION_ID
    )

    async def main() -> None:
        results = await asyncio.gather(
            mcdu1.run(),
            mcdu2.run(),
            mcdu3.run(),
            return_exceptions=True,
        )
        for result in results:
            if isinstance(result, Exception):
                logging.error("MCDU client failed: %s", result)

    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        logging.info("Shutting down")
    finally:
        sc.exit()
