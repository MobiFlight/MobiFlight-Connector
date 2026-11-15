import asyncio
import ctypes
import json
import logging
import struct
from ctypes import wintypes, Structure, c_ubyte, sizeof
from pathlib import Path
from typing import Any

import websockets.asyncio.client as ws_client
from SimConnect import SimConnect, Enum
from SimConnect.Enum import SIMCONNECT_CLIENT_DATA_ID, SIMCONNECT_RECV_ID, SIMCONNECT_RECV_CLIENT_DATA


CAPTAIN_MCDU_URL = "ws://localhost:8320/winwing/cdu-captain"
FO_MCDU_URL = "ws://localhost:8320/winwing/cdu-co-pilot"
A300_MCDU_CPT_NAME, A300_CPT_MCDU_CLIENT_DATA_ID, A300_CPT_MCDU_DEFINITION_ID, A300_CPT_MCDU_REQUEST_ID= "iniAirbusMCDU_1", 100, 200, 300
A300_MCDU_FO_NAME, A300_FO_MCDU_CLIENT_DATA_ID, A300_FO_MCDU_DEFINITION_ID, A300_FO_MCDU_REQUEST_ID= "iniAirbusMCDU_2", 101, 201, 301

MCDU_COLUMNS = 24
MCDU_ROWS = 14
MCDU_CHARS = MCDU_COLUMNS * MCDU_ROWS

MCDU_FLAG_SMALL_FONT = 0x01
MCDU_COLOR_MAP = {
    0: "w",
    1: "c",
    2: "a",
    3: "g",
    4: "e",
    5: "r",
    6: "y",
    7: "m",
}

SPECIAL_CHARS = {
    "a": "←",
    "b": "→",
    "e": "↑",
    "f": "↓",
    "!": "☐",
    "d": "°",
    "c": "Δ",
    "p": "■",
}

LOGFILE = Path(__file__).with_name("ini_a300_winwing_cdu.log")


class MCDUChar(Structure):
    _pack_ = 1
    _fields_ = [
        ("Symbol", c_ubyte),
        ("Color", c_ubyte),
        ("Flags", c_ubyte),
    ]


class SIMCONNECT_RECV_EXCEPTION_RAW(ctypes.Structure):
    _fields_ = [
        ("dwSize", ctypes.c_uint32),
        ("dwVersion", ctypes.c_uint32),
        ("dwID", ctypes.c_uint32),
        ("dwException", ctypes.c_uint32),
        ("dwSendID", ctypes.c_uint32),
        ("dwIndex", ctypes.c_uint32),
    ]


MCDU_CHAR_SIZE = sizeof(MCDUChar)   # 3
MCDU_DATA_SIZE = MCDU_CHARS * MCDU_CHAR_SIZE  # 336 * 3 = 1008
MCDU_DWORDS = (MCDU_DATA_SIZE + 3) // 4       # 252


def setup_logging() -> None:
    formatter = logging.Formatter("%(asctime)s - %(levelname)s - %(message)s")

    root = logging.getLogger()
    root.setLevel(logging.INFO)
    root.handlers.clear()

    sh = logging.StreamHandler()
    sh.setFormatter(formatter)
    root.addHandler(sh)

    fh = logging.FileHandler(LOGFILE, encoding="utf-8")
    fh.setFormatter(formatter)
    root.addHandler(fh)


def ascii_preview(data: bytes, limit: int = 120) -> str:
    out = []
    for b in data[:limit]:
        out.append(chr(b) if 32 <= b <= 126 else ".")
    return "".join(out)


class SimConnectMobiFlight(SimConnect):
    def __init__(self, auto_connect=True, library_path=None):
        self.client_data_handlers = []

        if library_path:
            super().__init__(auto_connect, library_path)
        else:
            super().__init__(auto_connect)

        self.dll.MapClientDataNameToID.argtypes = [
            wintypes.HANDLE,
            ctypes.c_char_p,
            SIMCONNECT_CLIENT_DATA_ID,
        ]

    def register_client_data_handler(self, handler):
        if handler not in self.client_data_handlers:
            self.client_data_handlers.append(handler)

    def unregister_client_data_handler(self, handler):
        if handler in self.client_data_handlers:
            self.client_data_handlers.remove(handler)

    def my_dispatch_proc(self, pData, cbData, pContext):
        if not pData:
            return

        try:
            recv_id = pData.contents.dwID
        except Exception as exc:
            logging.error("Dispatch read failed: %s", exc)
            return

        if recv_id == SIMCONNECT_RECV_ID.SIMCONNECT_RECV_ID_CLIENT_DATA:
            try:
                client_data = ctypes.cast(
                    pData, ctypes.POINTER(SIMCONNECT_RECV_CLIENT_DATA)
                ).contents

                for handler in self.client_data_handlers:
                    try:
                        handler(client_data)
                    except Exception as exc:
                        logging.error("Client data handler failed: %s", exc)
            except Exception as exc:
                logging.error("CLIENT_DATA dispatch failed: %s", exc)
            return

        if int(recv_id) == 1:
            try:
                exc_raw = ctypes.cast(
                    pData, ctypes.POINTER(SIMCONNECT_RECV_EXCEPTION_RAW)
                ).contents
                logging.warning(
                    "SIMCONNECT EXCEPTION raw: code=%s send_id=%s index=%s",
                    exc_raw.dwException,
                    exc_raw.dwSendID,
                    exc_raw.dwIndex,
                )
            except Exception as exc:
                logging.warning("Could not decode raw SimConnect exception: %s", exc)
            return

        try:
            super().my_dispatch_proc(pData, cbData, pContext)
        except Exception:
            pass


class MobiFlightClient:
    def __init__(self, uri: str, tag: str, max_retries: int = 5):
        self.uri = uri
        self.tag = tag
        self.max_retries = max_retries
        self.retries = 0
        self.connected = asyncio.Event()
        self.websocket = None
        self.last_data = None

    async def run(self):
        while self.retries < self.max_retries:
            try:
                logging.info("[%s] Connecting to %s", self.tag, self.uri)
                self.websocket = await ws_client.connect(self.uri, ping_interval=None)

                await self.websocket.send(
                    json.dumps({"Target": "Font", "Data": "AirbusThales"})
                )
                logging.info("[%s] Setting font: AirbusThales", self.tag)

                self.connected.set()

                if self.last_data:
                    await self.websocket.send(self.last_data)

                async for _ in self.websocket:
                    pass

            except Exception as exc:
                self.retries += 1
                logging.warning(
                    "[%s] WebSocket failure: %s (%s/%s)",
                    self.tag,
                    exc,
                    self.retries,
                    self.max_retries,
                )
                self.websocket = None
                self.connected.clear()
                await asyncio.sleep(3)

        logging.error("[%s] Max retries reached. Could not connect to %s", self.tag, self.uri)
        self.connected.set()

    async def send(self, data: str):
        self.last_data = data
        if self.websocket and self.connected.is_set():
            await self.websocket.send(data)

    async def close(self):
        if self.websocket:
            await self.websocket.close()
            self.websocket = None
            self.connected.clear()


def create_mobi_json(data: bytes, tag: str) -> str:
    out = {"Target": "Display", "Data": [[] for _ in range(MCDU_CHARS)]}

    for row in range(MCDU_ROWS):
        for col in range(MCDU_COLUMNS):
            display_idx = row * MCDU_COLUMNS + col
            buf_idx = display_idx * MCDU_CHAR_SIZE  # stride=3, offset=0

            if buf_idx + 2 >= len(data):
                continue

            try:
                sym = chr(data[buf_idx])
                colr = data[buf_idx + 1]
                flg = data[buf_idx + 2]

                if sym in (" ", "\0"):
                    continue

                sym = SPECIAL_CHARS.get(sym, sym)

                out["Data"][display_idx] = [
                    sym,
                    MCDU_COLOR_MAP.get(colr, "w"),
                    int(bool(flg & MCDU_FLAG_SMALL_FONT)),
                ]
            except Exception as exc:
                logging.warning(
                    "[%s] Display conversion warning row=%s col=%s: %s",
                    tag,
                    row,
                    col,
                    exc,
                )

    return json.dumps(out)


class A300MCDUClient:
    def __init__(self, sc:SimConnectMobiFlight, tag:str, uri:str, def_id:int, req_id:int, client_area_name:str, client_area_id:int):
        self.sc = sc
        self.tag = tag
        self.uri = uri
        self.def_id = def_id
        self.req_id = req_id
        self.CA_NAME = client_area_name
        self.CA_ID = client_area_id
        self.mobiflight = MobiFlightClient(uri, tag)
        self.loop = None
        self.last_raw = None
        self.last_preview = None
        self.registered = False

    def setup_clientdata(self):
        h = self.sc.hSimConnect

        hr = self.sc.dll.MapClientDataNameToID(
            h,
            self.CA_NAME.encode(),
            self.CA_ID,
        )
        logging.info("[%s] MapClientDataNameToID(%s) -> %s", self.tag, self.CA_NAME, hr)

        hr = self.sc.dll.AddToClientDataDefinition(
            h,
            self.def_id,
            0,
            MCDU_DATA_SIZE,
            0,
            0,
        )
        logging.info(
            "[%s] AddToClientDataDefinition(def=%s, size=%s) -> %s",
            self.tag,
            self.def_id,
            MCDU_DATA_SIZE,
            hr,
        )

        hr = self.sc.dll.RequestClientData(
            h,
            self.CA_ID,
            self.req_id,
            self.def_id,
            Enum.SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_VISUAL_FRAME,
            Enum.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG_CHANGED,
            0,
            0,
            0,
        )
        logging.info(
            "[%s] RequestClientData(stream=%s, req=%s, def=%s) -> %s",
            self.tag,
            self.CA_NAME,
            self.req_id,
            self.def_id,
            hr,
        )

        if not self.registered:
            self.sc.register_client_data_handler(self.on_data)
            self.registered = True

    def on_data(self, d: Any):
        try:
            define_id = getattr(d, "dwDefineID", None)
            request_id = getattr(d, "dwRequestID", None)

            if define_id != self.def_id or request_id != self.req_id:
                return

            raw_words = list(d.dwData[:MCDU_DWORDS])
            packed = b"".join(struct.pack("I", x) for x in raw_words)

            if len(packed) < MCDU_DATA_SIZE:
                return

            payload = packed[:MCDU_DATA_SIZE]

            if payload == self.last_raw:
                return

            self.last_raw = payload

            preview = ascii_preview(payload)
            if preview != self.last_preview:
                self.last_preview = preview
                non_zero = sum(1 for b in payload if b != 0)
                logging.info(
                    "[%s] MCDU update | non_zero=%s/%s | preview=%s",
                    self.tag,
                    non_zero,
                    len(payload),
                    preview,
                )

            json_data = create_mobi_json(payload, self.tag)

            if self.loop:
                asyncio.run_coroutine_threadsafe(
                    self.mobiflight.send(json_data),
                    self.loop,
                )

        except Exception as exc:
            logging.error("[%s] on_data failed: %s", self.tag, exc)

    async def run(self):
        self.loop = asyncio.get_running_loop()

        ws_task = asyncio.create_task(self.mobiflight.run())
        await self.mobiflight.connected.wait()

        if self.mobiflight.retries >= self.mobiflight.max_retries:
            logging.error("[%s] WebSocket could not be established. If you only have one CDU device connected this is normal and can be ignored.", self.tag)
            return

        self.setup_clientdata()
        logging.info("[%s] A300 CDU bridge is running using stream: %s", self.tag, self.CA_NAME)

        try:
            await ws_task
        finally:
            if self.registered:
                self.sc.unregister_client_data_handler(self.on_data)
            await self.mobiflight.close()


if __name__ == "__main__":
    setup_logging()
    logging.info("A300 WinWing CDU bridge starting")

    sc = SimConnectMobiFlight()
    logging.info("SIM OPEN")

    cptClient = A300MCDUClient(sc, "CPT", CAPTAIN_MCDU_URL, A300_CPT_MCDU_DEFINITION_ID, A300_CPT_MCDU_REQUEST_ID, A300_MCDU_CPT_NAME, A300_CPT_MCDU_CLIENT_DATA_ID)
    foClient = A300MCDUClient(sc, "FO", FO_MCDU_URL, A300_FO_MCDU_DEFINITION_ID, A300_FO_MCDU_REQUEST_ID, A300_MCDU_FO_NAME, A300_FO_MCDU_CLIENT_DATA_ID)

    async def main():
        await asyncio.gather(
            cptClient.run(),
            foClient.run()
        )

    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        logging.info("Interrupted by user")
    finally:
        try:
            sc.exit()
        except Exception:
            pass
        logging.info("SIM CLOSED")
