# Microsoft EC135 all_in_one
# Single-file bridge: SimConnect(MobiFlight LVARs) -> WinWing MCDUs 
# Requires: pip install websockets SimConnect==0.4.24 (or your working SimConnect lib)
# CREDITS: Koseng on GitHub and his MSFSPythonSimConnectMobiFlightExtension (https://github.com/Koseng/MSFSPythonSimConnectMobiFlightExtension)
# pylint: disable=redefined-outer-name,broad-exception-caught

"""
EC135 WinWing MCDU bridge for Microsoft Flight Simulator (MSFS).

Purpose:
    Provide a single-file integration that bridges MSFS SimConnect and MobiFlight
    LVARs to WinWing MCDU websocket displays. This script targets the Microsoft
    EC135 helicopter and renders cockpit pages on external MCDU hardware.

Aircraft/Simulators:
    - Microsoft EC135 (MSFS)
    - Uses SimConnect and MobiFlight WASM LVAR access

Architecture:
    - SimConnectMobiFlight extends SimConnect to register client data handlers.
    - MobiFlightVariableRequests manages LVAR subscriptions and value caching.
    - McduSocket runs an asyncio websocket sender loop in a background thread.
    - Two display threads render grids and send payloads to WinWing displays.
    - The cds_Swap LVAR can swap CDS/CPDS output between captain/copilot MCDUs.
      cds_Swap is a custom LVAR that needs to be explicitly set by User Custom code
      Recommended usage of MobiFlight Input Config with a Custom Preset Code "(L:cds_Swap) ! (>L:cds_Swap, Bool)"
      The LVAR defaults to "0" if not used, keeping CDS1 on Captain and CD2 on Copilot MCDU's

Displays:
    - CDS1 (captain side) and CPDS (copilot side) are rendered separately.
    - Each display thread builds a grid and sends it at a fixed tick interval.
    - The CDS swap LVAR (cds_Swap) can route CDS/CPDS output to the opposite
      MCDU, allowing quick display handover between captain and copilot units.
"""


import json
import logging
import logging.handlers
import struct
import ctypes
from time import sleep
from typing import List, Union
from itertools import chain
import asyncio
import threading
from websockets import connect
from websockets.exceptions import WebSocketException as WsWebSocketException

# ========================= SimConnectMobiFlight =========================
from SimConnect import SimConnect
from SimConnect.Enum import (
    SIMCONNECT_CLIENT_DATA_ID,
    SIMCONNECT_RECV_ID,
    SIMCONNECT_RECV_CLIENT_DATA,
    SIMCONNECT_CLIENT_DATA_PERIOD,
    SIMCONNECT_UNUSED,
)

class SimConnectMobiFlight(SimConnect):
    """
    Extends SimConnect to support MobiFlight client data handlers.
    This class allows registration and management of client data handlers,
    enabling custom processing of MobiFlight client data received from the simulator.
    """
    def __init__(self, auto_connect=True, library_path=None):
        self.client_data_handlers = []
        if library_path:
            super().__init__(auto_connect, library_path)
        else:
            super().__init__(auto_connect)
        # Fix missing types
        self.dll.MapClientDataNameToID.argtypes = [ctypes.wintypes.HANDLE, ctypes.c_char_p, SIMCONNECT_CLIENT_DATA_ID]

    def register_client_data_handler(self, handler):
        if handler not in self.client_data_handlers:
            logging.info("Register new client data handler")
            self.client_data_handlers.append(handler)

    def unregister_client_data_handler(self, handler):
        if handler in self.client_data_handlers:
            logging.info("Unregister client data handler")
            self.client_data_handlers.remove(handler)

    def my_dispatch_proc(self, pData, cbData, pContext):
        dwID = pData.contents.dwID
        if dwID == SIMCONNECT_RECV_ID.SIMCONNECT_RECV_ID_CLIENT_DATA:
            client_data = ctypes.cast(pData, ctypes.POINTER(SIMCONNECT_RECV_CLIENT_DATA)).contents
            for handler in self.client_data_handlers:
                handler(client_data)
        else:
            super().my_dispatch_proc(pData, cbData, pContext)

# ========================= MobiFlightVariableRequests =========================
class SimVariable:
    """
    Represents a simulation variable used in MobiFlight variable requests.

    Attributes:
        id (int): Unique identifier for the simulation variable.
        name (str): Name of the simulation variable.
        float_value (float, optional): The current value of the variable as a float.
        initialized (bool): Indicates whether the variable has been initialized.
    """
    def __init__(self, init_id, name, float_value=None):
        self.id = init_id
        self.name = name
        self.float_value = float_value
        self.initialized = False
    def __str__(self):
        return f"Id={self.id}, value={self.float_value}, name={self.name}"

class MobiFlightVariableRequests:
    """
    Manages SimConnect variable requests for MobiFlight integration.

    This class handles the setup and management of SimConnect client data areas,
    variable subscriptions, and callbacks for MobiFlight LVARs. It provides methods
    to add variable definitions, subscribe to data changes, and process incoming
    client data from the simulator.
    """
    def __init__(self, simConnect: SimConnectMobiFlight):
        logging.info("MobiFlightVariableRequests __init__")
        self.sm = simConnect
        self.sim_vars = {}
        self.sim_var_name_to_id = {}
        self._vr_lock = threading.Lock()
        self.CLIENT_DATA_AREA_LVARS    = 0
        self.CLIENT_DATA_AREA_CMD      = 1
        self.CLIENT_DATA_AREA_RESPONSE = 2
        self.FLAG_DEFAULT = 0
        self.FLAG_CHANGED = 1
        self.DATA_STRING_SIZE = 256
        self.DATA_STRING_OFFSET = 0
        self.DATA_STRING_DEFINITION_ID = 0
        self.sm.register_client_data_handler(self.client_data_callback_handler)
        self.initialize_client_data_areas()

    def add_to_client_data_definition(self, definition_id, offset, size):
        logging.info("add_to_client_data_definition definition_id=%s, offset=%s, size=%s", definition_id, offset, size)
        self.sm.dll.AddToClientDataDefinition(
            self.sm.hSimConnect,
            definition_id,
            offset,
            size,
            0,  # fEpsilon
            SIMCONNECT_UNUSED,  # DatumId
        )

    def subscribe_to_data_change(self, data_area_id, request_id, definition_id):
        logging.info("subscribe_to_data_change data_area_id=%s, request_id=%s, definition_id=%s", data_area_id, request_id, definition_id)
        self.sm.dll.RequestClientData(
            self.sm.hSimConnect,
            data_area_id,
            request_id,
            definition_id,
            SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_ON_SET,
            self.FLAG_CHANGED,
            0, # origin
            0, # interval
            0, # limit
        )

    def send_data(self, data_area_id, definition_id, size, dataBytes):
        logging.info("send_data data_area_id=%s, definition_id=%s, size=%s, dataBytes=%s", data_area_id, definition_id, size, dataBytes)
        self.sm.dll.SetClientData(
            self.sm.hSimConnect,
            data_area_id,
            definition_id,
            self.FLAG_DEFAULT,
            0, # dwReserved
            size,
            dataBytes,
        )

    def send_command(self, command: str):
        logging.info("send_command command=%s", command)
        data_byte_array = bytearray(command, "ascii")
        data_byte_array.extend(bytearray(self.DATA_STRING_SIZE - len(data_byte_array)))  # pad to fixed size
        self.send_data(self.CLIENT_DATA_AREA_CMD, self.DATA_STRING_DEFINITION_ID, self.DATA_STRING_SIZE, bytes(data_byte_array))

    def initialize_client_data_areas(self):
        logging.info("initialize_client_data_areas")
        # LVars area
        self.sm.dll.MapClientDataNameToID(self.sm.hSimConnect, "MobiFlight.LVars".encode("ascii"), self.CLIENT_DATA_AREA_LVARS)
        self.sm.dll.CreateClientData(self.sm.hSimConnect, self.CLIENT_DATA_AREA_LVARS, 4096, self.FLAG_DEFAULT)
        # Command area
        self.sm.dll.MapClientDataNameToID(self.sm.hSimConnect, "MobiFlight.Command".encode("ascii"), self.CLIENT_DATA_AREA_CMD)
        self.sm.dll.CreateClientData(self.sm.hSimConnect, self.CLIENT_DATA_AREA_CMD, self.DATA_STRING_SIZE, self.FLAG_DEFAULT)
        # Response area
        self.sm.dll.MapClientDataNameToID(self.sm.hSimConnect, "MobiFlight.Response".encode("ascii"), self.CLIENT_DATA_AREA_RESPONSE)
        self.sm.dll.CreateClientData(self.sm.hSimConnect, self.CLIENT_DATA_AREA_RESPONSE, self.DATA_STRING_SIZE, self.FLAG_DEFAULT)
        # Subscribe to WASM responses
        self.add_to_client_data_definition(self.DATA_STRING_DEFINITION_ID, self.DATA_STRING_OFFSET, self.DATA_STRING_SIZE)
        self.subscribe_to_data_change(self.CLIENT_DATA_AREA_RESPONSE, self.DATA_STRING_DEFINITION_ID, self.DATA_STRING_DEFINITION_ID)

    # ---- BUGFIXED handler: always set float_value on first frame, no dropping first 0.0 ----
    def client_data_callback_handler(self, client_data):
        if client_data.dwDefineID in self.sim_vars:
            data_bytes = struct.pack("I", client_data.dwData[0])
            float_data = struct.unpack('<f', data_bytes)[0]
            float_value = round(float_data, 5)
            sim_var = self.sim_vars[client_data.dwDefineID]
            if not sim_var.initialized:
                sim_var.initialized = True
            self.sim_vars[client_data.dwDefineID].float_value = float_value
            logging.debug("client_data_callback_handler %s, raw=%s", sim_var, float_value)
        else:
            logging.warning("client_data_callback_handler DefinitionID %s not found!", client_data.dwDefineID)

    def get(self, variableString: str):
        with self._vr_lock:
            if variableString not in self.sim_var_name_to_id:
                # add new variable
                var_id = len(self.sim_vars) + 1
                self.sim_vars[var_id] = SimVariable(var_id, variableString)
                self.sim_var_name_to_id[variableString] = var_id
                # subscribe to variable data change
                offset = (var_id - 1) * ctypes.sizeof(ctypes.wintypes.FLOAT)
                self.add_to_client_data_definition(var_id, offset, ctypes.sizeof(ctypes.wintypes.FLOAT))
                self.subscribe_to_data_change(self.CLIENT_DATA_AREA_LVARS, var_id, var_id)
                self.send_command("MF.SimVars.Add." + variableString)
            # determine id and return value
            variable_id = self.sim_var_name_to_id[variableString]
            sim_var = self.sim_vars[variable_id]
            wait_counter = 0  # 10ms ticks, max ~500ms
            # NOTE: SimConnect Python wrapper runs CallDispatch() in a background thread.
            # The wait loop below relies on async callbacks and is safe.
            while wait_counter < 50:  # wait max 500ms
                if sim_var.float_value is None:
                    sleep(0.01)  # wait 10ms
                    wait_counter += 1
                else:
                    break
            if sim_var.float_value is None and sim_var.initialized:
                sim_var.float_value = 0.0
            logging.debug("get %s. wait_counter=%s, Return=%s", variableString, wait_counter, sim_var.float_value)
            return sim_var.float_value

    def set(self, variable_string):
        with self._vr_lock:
            logging.debug("set: %s", variable_string)
            self.send_command("MF.SimVars.Set." + variable_string)

    def clear_sim_variables(self):
        with self._vr_lock:
            logging.info("clear_sim_variables")
            self.sim_vars.clear()
            self.sim_var_name_to_id.clear()
            self.send_command("MF.SimVars.Clear")

# ========================= Logging =========================
def setup_logging(log_file_name):
    """Configure root logging to a rotating file and console."""
    log_formatter = logging.Formatter("%(asctime)s [%(levelname)-5.5s]  %(message)s")
    root_logger = logging.getLogger()
    root_logger.setLevel(logging.DEBUG)
    file_handler = logging.handlers.RotatingFileHandler(log_file_name, maxBytes=500000, backupCount=7)
    file_handler.setFormatter(log_formatter)
    root_logger.addHandler(file_handler)
    console_handler = logging.StreamHandler()
    console_handler.setFormatter(log_formatter)
    root_logger.addHandler(console_handler)

# ========================= MCDU URLs =========================
CAPT_MCDU_URL = "ws://127.0.0.1:8320/winwing/cdu-captain"
COPI_MCDU_URL = "ws://127.0.0.1:8320/winwing/cdu-co-pilot"

# ========================= MCDU display primitives =========================
CDU_COLUMNS = 24
CDU_ROWS = 14
LARGE = 0  # WinWing size code for large font
SMALL = 1  # WinWing size code for small font
Cell = List[Union[str, int]]  # [] or [char, colour, size]

def empty_grid() -> List[List[Cell]]:
    """Return a blank CDU grid with empty cells."""
    return [[[] for _ in range(CDU_COLUMNS)] for _ in range(CDU_ROWS)]

# Map placeholders to glyphs expected by the WinWing display.
REPLACED = {
    "←":"\u2190","→":"\u2192","↑":"\u2191","↓":"\u2193",
    "_":"\u2610","\u00b0":"°","&":"\u0394","\xa0":" ",
    "{":"\u2190","}":"\u2192","|":"/",
}

def put_text(grid: List[List[Cell]], text: str, row: int, col: int, colour="w", size=LARGE):
    """Write text into the grid with bounds checks and glyph replacements."""
    if not 0 <= row < CDU_ROWS:
        return
    for i, ch in enumerate(text):
        cc = col + i
        if 0 <= cc < CDU_COLUMNS:
            grid[row][cc] = [REPLACED.get(ch, ch), colour, size]

def put_text_center(grid: List[List[Cell]], text: str, row: int, colour="w", size=LARGE):
    """Center text on a row and write it into the grid."""
    text = text[:CDU_COLUMNS]  # safety
    col = (CDU_COLUMNS - len(text)) // 2
    put_text(grid, text, row, col, colour=colour, size=size)


def grid_to_payload(grid: List[List[Cell]]) -> str:
    """Serialize the grid into a WinWing websocket payload."""
    return json.dumps({"Target": "Display", "Data": list(chain(*grid))})

def select_mcdu(mcdu_primary: "McduSocket", mcdu_alt: "McduSocket", cds_swap: int) -> "McduSocket":
    """Pick the active MCDU based on the CDS swap LVAR value."""
    return mcdu_alt if cds_swap == 1 else mcdu_primary

# ========================= Rolling list layout =========================
LEFT_COL_START  = 0
RIGHT_COL_START = 13  # 12-char left block + 1 column gap
CONTENT_FIRST_ROW = 0
CONTENT_LAST_ROW  = 5
MAX_ROWS = CONTENT_LAST_ROW - CONTENT_FIRST_ROW + 1

def clear_area_with_spaces(grid, r0, r1, c0=0, c1=CDU_COLUMNS, colour="w", size=0):
    """Fill a rectangular area with space cells."""
    for r in range(r0, r1 + 1):
        for c in range(c0, c1):
            grid[r][c] = [" ", colour, size]

def compact_labels(pairs):
    """Return labels whose state is active (state == 1)."""
    return [label for val, label in pairs if val == 1]

def draw_columns(grid: List[List[Cell]], left_labels: List[str], right_labels: List[str]):
    """Render left/right label columns into the grid."""
    clear_area_with_spaces(grid, CONTENT_FIRST_ROW, CONTENT_LAST_ROW)
    # LEFT 12 chars
    row = CONTENT_FIRST_ROW
    for lbl in left_labels[:MAX_ROWS]:
        put_text(grid, lbl[:12].ljust(12), row, LEFT_COL_START, colour="a", size=LARGE)
        row += 1
        if row > CONTENT_LAST_ROW:
            break
    # RIGHT 11 chars
    row = CONTENT_FIRST_ROW
    for lbl in right_labels[:MAX_ROWS]:
        put_text(grid, lbl[:11].ljust(11), row, RIGHT_COL_START, colour="a", size=LARGE)
        row += 1
        if row > CONTENT_LAST_ROW:
            break

def get_state(v) -> int:
    """
    Normalize a raw LVAR value into a discrete display state.
    This returns desired INT - not to be used for other numerical operations

    This function converts various LVAR input types (None, bool, int, float, or str)
    into one of six discrete states:
        0 = OFF / inactive
        1 = ON / active
        2 = PAGE / extended state
        3 = additional state (e.g. tertiary mode / higher-level active state)
        4 = additional state (e.g. quaternary mode / higher-level active state)
        5 = additional state (e.g. highest-level mode / special condition)

    Numeric values are interpreted using thresholds:
        < 0.5  -> 0
        < 1.5  -> 1
        < 2.5  -> 2
        < 3.5  -> 3
        < 4.5  -> 4
        >=4.5  -> 5

    String values are matched against common textual representations
    (e.g. "true", "on", "1", "2"), with a fallback to float parsing.

    Any invalid or unexpected input safely defaults to state 0.
    """
    try:
        if v is None:
            return 0

        if isinstance(v, bool):
            return 1 if v else 0

        # Normalize to float if possible
        if isinstance(v, (int, float)):
            f = float(v)
        else:
            s = str(v).strip().strip('"').strip("'").lower()
            if s in ("2", "two"):
                return 2
            if s in ("1", "true", "on", "yes", "y"):
                return 1
            if s in ("0", "false", "off", "no", "n", ""):
                return 0
            f = float(s)

        if f < 0.5:
            return 0
        if f < 1.5:
            return 1
        if f < 2.5:
            return 2
        if f < 3.5:
            return 3
        if f < 4.5:
            return 4
        return 5

    except Exception:
        return 0

# ========================= Simple persistent WebSocket =========================
class McduSocket:
    """
    Persistent WebSocket sender using the `websockets` library (async) while keeping
    a synchronous API for the rest of the script.

    - Runs an asyncio event loop in a background thread
    - `send_grid(grid)` is synchronous and just queues the latest payload
    - Automatically reconnects
    - Uses built-in ping/keepalive from `websockets`
    """

    def __init__(self, url: str, connect_timeout: float = 2.0):
        self.url = url
        self.connect_timeout = connect_timeout

        self._loop = None
        self._queue = None
        self._ready = threading.Event()
        self._stop = threading.Event()

        self._thread = threading.Thread(target=self._thread_main, name="McduSocketThread", daemon=True)
        self._thread.start()

        # Wait briefly for loop/queue to exist (avoid first-send race)
        self._ready.wait(timeout=self.connect_timeout)

    def _thread_main(self):
        try:
            loop = asyncio.new_event_loop()
            asyncio.set_event_loop(loop)
            self._loop = loop
            self._queue = asyncio.Queue()
            self._ready.set()
            loop.run_until_complete(self._run())
        except Exception as e:
            logging.exception("MCDU thread crashed: %s", e)
        finally:
            try:
                if self._loop:
                    self._loop.stop()
                    self._loop.close()
            except Exception:
                pass

    async def _run(self):
        # Reconnect loop
        while not self._stop.is_set():
            try:
                logging.info("Connecting to MCDU at %s", self.url)

                # websockets keepalive is handled via ping_interval/ping_timeout
                async with connect(
                    self.url,
                    open_timeout=self.connect_timeout,
                    close_timeout=1.0,
                    ping_interval=20.0,
                    ping_timeout=10.0,
                    max_queue=1,  # keep internal queue small
                ) as ws:
                    logging.info("MCDU connected.")

                    while not self._stop.is_set():
                        # Wait for next payload; we coalesce to "latest only"
                        payload = await self._queue.get()
                        if payload is None:
                            break

                        # Drain any newer payloads (keep only the latest)
                        try:
                            while True:
                                next_payload = self._queue.get_nowait()
                                if next_payload is None:
                                    payload = None
                                    break
                                payload = next_payload
                        except asyncio.QueueEmpty:
                            pass

                        if payload is None:
                            break

                        # Send
                        await ws.send(payload)
                        logging.debug("→ MCDU SEND %s bytes", len(payload))

            except (OSError, WsWebSocketException, asyncio.TimeoutError) as e:
                logging.debug("MCDU connection/send error: %s", e)
                # Backoff a bit before reconnect
                await asyncio.sleep(0.5)
            except Exception as e:
                # Unexpected error — log and retry
                logging.exception("MCDU unexpected error: %s", e)
                await asyncio.sleep(0.5)

    def send_grid(self, grid: List[List[Cell]]):
        payload = grid_to_payload(grid)

        # If the thread/loop isn't ready yet, just drop the frame (next tick will resend)
        if not self._ready.is_set() or self._loop is None or self._queue is None:
            return

        # Thread-safe enqueue into asyncio.Queue
        try:
            self._loop.call_soon_threadsafe(self._queue.put_nowait, payload)
        except Exception as e:
            logging.debug("MCDU enqueue failed: %s", e)

    def close(self):
        # Optional explicit shutdown if you ever want it
        self._stop.set()
        try:
            if self._loop and self._queue:
                # Unblock _queue.get() so the async loop can exit cleanly.
                self._loop.call_soon_threadsafe(self._queue.put_nowait, None)
        except Exception as e:
            logging.debug("MCDU close encountered error during shutdown: %s", e)
            pass
            # Best-effort shutdown: log at debug level but do not raise during close().

# ========================= CPDS (COPILOT) RENDERER =========================
def _safe_float(v, default: float = 0.0) -> float:
    """Convert input to float, returning default on failure."""
    try:
        if v is None:
            return default
        return float(v)
    except Exception:
        return default

def _fmt_h_mm(seconds: float) -> str:
    """Format seconds as H:MM."""
    s = max(0.0, _safe_float(seconds, 0.0))
    h = int(s // 3600)
    m = int((s % 3600) // 60)
    return f"{h}:{m:02d}"

def _fmt_num(v: float, decimals: int) -> str:
    """Format a float with the requested decimal precision."""
    v = _safe_float(v, 0.0)
    if decimals <= 0:
        return str(int(round(v)))
    return f"{v:.{decimals}f}"

def _ceil_int(v: float) -> int:
    """Ceil a numeric value to the next integer."""
    v = _safe_float(v, 0.0)
    i = int(v)
    return i if v == i else i + 1

def _oat_text_kelvin_to_c(k: float) -> str:
    """Format OAT text from a Kelvin input."""
    # "OAT  13 C" — number is width 3, leading zeros become spaces automatically
    c = int(round(_safe_float(k, 0.0) - 273.15))
    num = f"{c:3d}"
    return f"OAT {num} C"

# CPDS uses 3 blocks of 8 chars to fit 24 cols: [0..7][8..15][16..23]
CPDS_L_START = 0
CPDS_C_START = 8
CPDS_R_START = 16
CPDS_W = 8

def _put_blk(grid, row: int, start: int, text: str, align: str = "left", colour="w", size=LARGE):
    """
    align: "left" | "right" | "center"
    """
    t = (text or "")
    if len(t) > CPDS_W:
        t = t[:CPDS_W]
    if align == "right":
        t = t.rjust(CPDS_W)
    elif align == "center":
        t = t.center(CPDS_W)
    else:
        t = t.ljust(CPDS_W)
    put_text(grid, t, row, start, colour=colour, size=size)

def _put_blk_lc(grid, row: int, text: str, align: str = "left", colour="w", size=LARGE):
    """
    Put text spanning LEFT+CENTER blocks (16 columns total).
    """
    width = CPDS_W * 2
    t = (text or "")
    if len(t) > width:
        t = t[:width]
    if align == "right":
        t = t.rjust(width)
    elif align == "center":
        t = t.center(width)
    else:
        t = t.ljust(width)
    put_text(grid, t, row, CPDS_L_START, colour=colour, size=size)

def _cpds_separator(grid, row: int):
    """Draw a CPDS separator line."""
    put_text(grid, "-" * CDU_COLUMNS, row, 0, colour="k", size=SMALL)

def build_cpds_grid(
    vr: MobiFlightVariableRequests,
    knob_cds: int,
    cpds_scroll: int,
    volt_amp: int,
    rad_alt_scrl: int,
    cds_test: int,
    msg1=None,
    msg2=None
) -> List[List[Cell]]:
    """ 
    This function is intentionally side‑effect free: it only formats the current
    CPDS state into a character grid. It does **not** perform any simulator or
    device I/O and therefore does **not** call ``get_state()`` itself.
    The caller is responsible for:
    - Reading the current CPDS‑related states (typically via ``get_state()`` in
      the main while‑loop "HELPERS" section).
    - Passing the resulting raw integer values into this function:
      * ``knob_cds``: current CPDS / CDS knob position selector.
      * ``cpds_scroll``: vertical scroll index for the CPDS page.
      * ``volt_amp``: selector for voltage/ampere display mode.
      * ``rad_alt_scrl``: scroll/selector value for radar‑altimeter related
        information.
      * ``cds_test``: LVAR indicating that the "CDS test" switch is switched.
    By keeping all calls to ``get_state()`` in the outer loop and passing the
    values in as arguments, we avoid redundant SimConnect/MobiFlight lookups
    inside this formatting routine and make it easier to test in isolation.
    """

    grid = empty_grid()
    clear_area_with_spaces(grid, 0, CDU_ROWS - 1)

    # Static separator rows
    for r in (2, 4, 6, 10):
        _cpds_separator(grid, r)

    # Fuel static labels (row 9)
    _put_blk(grid, 9, CPDS_L_START, "SPLY 1", align="left", size=SMALL)
    _put_blk(grid, 9, CPDS_C_START, "MAIN", align="center", size=SMALL)
    _put_blk(grid, 9, CPDS_R_START, "SPLY 2", align="right", size=SMALL)

    # Bottom static labels (rows 11-13)
    if knob_cds < 5:
        _put_blk(grid, 11, CPDS_L_START, "Vne", align="center", size=SMALL)
        _put_blk_lc(grid, 12, "GROSS MASS", align="left", size=SMALL)
        _put_blk(grid, 11, CPDS_R_START, "RAD ALT", align="left", size=SMALL)
    elif knob_cds == 5:
        _put_blk_lc(grid, 13, "HOOK LOAD", align="left", size=SMALL)
        _put_blk(grid, 12, CPDS_R_START, "CABLE", align="left", size=SMALL)
        _put_blk(grid, 13, CPDS_R_START, "LENGHT", align="left", size=SMALL)  # intentional typo matches in-game display

    # ---------------- AVAR reads (only what we need) ----------------
    eng1_oth = _safe_float(vr.get("(A:GENERAL ENG ELAPSED TIME:1,number)"))  # engine 1 operating time (seconds)
    eng2_oth = _safe_float(vr.get("(A:GENERAL ENG ELAPSED TIME:2,number)"))  # engine 2 operating time (seconds)

    eng1_n1  = _safe_float(vr.get("(A:ENG N1 RPM:1,number)"))
    eng2_n1  = _safe_float(vr.get("(A:ENG N1 RPM:2,number)"))
    eng1_n2  = _safe_float(vr.get("(A:ENG ROTOR RPM:1,number)"))
    eng2_n2  = _safe_float(vr.get("(A:ENG ROTOR RPM:2,number)"))

    eng1_egt = _safe_float(vr.get("(A:GENERAL ENG EXHAUST GAS TEMPERATURE:1,number)"))
    eng2_egt = _safe_float(vr.get("(A:GENERAL ENG EXHAUST GAS TEMPERATURE:2,number)"))
    eng1_t1  = _safe_float(vr.get("(A:TURB ENG ITT:1,number)"))
    eng2_t1  = _safe_float(vr.get("(A:TURB ENG ITT:2,number)"))

    collective = _safe_float(vr.get("(A:COLLECTIVE POSITION,number)"))
    pr         = _safe_float(vr.get("(A:BAROMETER PRESSURE,number)"))  # raw sim pressure value (unit per sim settings)

    eng1_tq = _safe_float(vr.get("(A:ENG TORQUE PERCENT:1,number)"))
    eng2_tq = _safe_float(vr.get("(A:ENG TORQUE PERCENT:2,number)"))

    dc1_volt  = _safe_float(vr.get("(A:ELECTRICAL MAIN BUS VOLTAGE:1,number)"))
    dc2_volt  = _safe_float(vr.get("(A:ELECTRICAL MAIN BUS VOLTAGE:2,number)"))
    gen1_amps = _safe_float(vr.get("(A:ELECTRICAL GENALT BUS AMPS:1,number)"))
    gen2_amps = _safe_float(vr.get("(A:ELECTRICAL GENALT BUS AMPS:2,number)"))
    bat_amps  = _safe_float(vr.get("(A:ELECTRICAL BATTERY LOAD,number)"))

    oat_k = _safe_float(vr.get("(A:AMBIENT TEMPERATURE,number)"))

    fuel_sply1 = _safe_float(vr.get("(A:FUELSYSTEM TANK WEIGHT:2,number)"))
    fuel_main  = _safe_float(vr.get("(A:FUELSYSTEM TANK WEIGHT:1,number)"))
    fuel_sply2 = _safe_float(vr.get("(A:FUELSYSTEM TANK WEIGHT:3,number)"))
    fuel_lvl_l = _safe_float(vr.get("(A:FUELSYSTEM TANK LEVEL:2,number)"))
    fuel_lvl_c = _safe_float(vr.get("(A:FUELSYSTEM TANK LEVEL:1,number)"))
    fuel_lvl_r = _safe_float(vr.get("(A:FUELSYSTEM TANK LEVEL:3,number)"))

    rad_alt_raw = _safe_float(vr.get("(A:RADIO HEIGHT,number)"))

    # ---------------- Row 0 (mode-dependent, 3 columns) ----------------
    if knob_cds == 0:
        _put_blk(grid, 0, CPDS_L_START, _fmt_h_mm(eng1_oth), align="center")
        _put_blk(grid, 0, CPDS_C_START, " OTH ", align="center", size=SMALL)
        _put_blk(grid, 0, CPDS_R_START, _fmt_h_mm(eng2_oth), align="center")

    elif knob_cds == 1:
        _put_blk(grid, 0, CPDS_L_START, "---.-", align="left")
        _put_blk(grid, 0, CPDS_C_START, " MEM ", align="center", size=SMALL)
        _put_blk(grid, 0, CPDS_R_START, "---.-", align="right")

    elif knob_cds == 2:
        # messages TBD — leave blank for now
        if msg1:
            _put_blk(grid, 0, CPDS_L_START, msg1, align="center")
        if msg2:
            _put_blk(grid, 0, CPDS_R_START, msg2, align="center")
        _put_blk(grid, 0, CPDS_C_START, " MSG ", align="center", size=SMALL)

    else:
        # knob_cds >= 3 uses cpds_scroll pages
        if cpds_scroll == 0:
            _put_blk(grid, 0, CPDS_L_START, _fmt_num(eng1_n1 * 100.0, 1), align="center")
            _put_blk(grid, 0, CPDS_C_START, "N1 %", align="center", size=SMALL)
            _put_blk(grid, 0, CPDS_R_START, _fmt_num(eng2_n1 * 100.0, 1), align="center")

        elif cpds_scroll == 1:
            eng1_n2_pct = eng1_n2 * 100.0
            eng2_n2_pct = eng2_n2 * 100.0
            eng1_n2_colour = "r" if eng1_n2_pct <= 80.0 or eng1_n2_pct >= 106.0 else "w"
            eng2_n2_colour = "r" if eng2_n2_pct <= 80.0 or eng2_n2_pct >= 106.0 else "w"
            _put_blk(grid, 0, CPDS_L_START, _fmt_num(eng1_n2_pct, 1), align="center", colour=eng1_n2_colour)
            _put_blk(grid, 0, CPDS_C_START, "N2 %", align="center", size=SMALL)
            _put_blk(grid, 0, CPDS_R_START, _fmt_num(eng2_n2_pct, 1), align="center", colour=eng2_n2_colour)

        elif cpds_scroll == 2:
            eng1_egt_c = eng1_egt - 273.15
            eng2_egt_c = eng2_egt - 273.15
            eng1_egt_colour = "r" if eng1_egt_c >= 895 else ("a" if eng1_egt_c >= 855 else "w")
            eng2_egt_colour = "r" if eng2_egt_c >= 895 else ("a" if eng2_egt_c >= 855 else "w")
            _put_blk(grid, 0, CPDS_L_START, _fmt_num(eng1_egt_c, 0), align="center", colour=eng1_egt_colour)
            _put_blk(grid, 0, CPDS_C_START, "EGT C", align="center", size=SMALL)
            _put_blk(grid, 0, CPDS_R_START, _fmt_num(eng2_egt_c, 0), align="center", colour=eng2_egt_colour)

        elif cpds_scroll == 3:
            _put_blk(grid, 0, CPDS_L_START, _fmt_num(eng1_t1 - 273.15, 0), align="center")
            _put_blk(grid, 0, CPDS_C_START, " T1 C", align="center", size=SMALL)
            _put_blk(grid, 0, CPDS_R_START, _fmt_num(eng2_t1 - 273.15, 0), align="center")

        elif cpds_scroll == 4:
            _put_blk(grid, 0, CPDS_L_START, _fmt_num(collective * 100.0, 0), align="center")
            _put_blk(grid, 0, CPDS_C_START, "CLP %", align="center", size=SMALL)
            _put_blk(grid, 0, CPDS_R_START, _fmt_num(collective * 100.0, 0), align="center")

        else:  # cpds_scroll == 5
            _put_blk(grid, 0, CPDS_L_START, _fmt_num(pr / 100.0, 0), align="center")
            _put_blk(grid, 0, CPDS_C_START, "PR MB", align="center", size=SMALL)
            _put_blk(grid, 0, CPDS_R_START, _fmt_num(pr / 100.0, 0), align="center")

    # ---------------- Row 1 (TQ%) ----------------
    eng1_tq_pct = eng1_tq * 100.0
    eng2_tq_pct = eng2_tq * 100.0
    eng1_tq_colour = "r" if eng1_tq_pct >= 75.0 else ("a" if eng1_tq_pct >= 70.0 else "w")
    eng2_tq_colour = "r" if eng2_tq_pct >= 75.0 else ("a" if eng2_tq_pct >= 70.0 else "w")
    _put_blk(grid, 1, CPDS_L_START, _fmt_num(eng1_tq_pct, 1), align="center", colour=eng1_tq_colour)
    _put_blk(grid, 1, CPDS_C_START, "TQ%", align="center")
    _put_blk(grid, 1, CPDS_R_START, _fmt_num(eng2_tq_pct, 1), align="center", colour=eng2_tq_colour)

    # ---------------- Row 3 (DC/GEN/BAT) ----------------
    if volt_amp == 0:
        dc1_col = "r" if dc1_volt <= 20.5 else ("a" if dc1_volt <= 21.0 else "w")
        dc2_col = "r" if dc2_volt <= 20.5 else ("a" if dc2_volt <= 21.0 else "w")
        _put_blk(grid, 3, CPDS_L_START, _fmt_num(dc1_volt, 1), align="center", colour=dc1_col)
        _put_blk(grid, 3, CPDS_C_START, "DC VOLTS", align="center", size=SMALL)
        _put_blk(grid, 3, CPDS_R_START, _fmt_num(dc2_volt, 1), align="center", colour=dc2_col)
    elif volt_amp == 1:
        _put_blk(grid, 3, CPDS_L_START, _fmt_num(gen1_amps, 1), align="center")
        _put_blk(grid, 3, CPDS_C_START, "GEN AMPS", align="center", size=SMALL)
        _put_blk(grid, 3, CPDS_R_START, _fmt_num(gen2_amps, 1), align="center")
    else:
        _put_blk(grid, 3, CPDS_L_START, _fmt_num(bat_amps, 1), align="center")
        _put_blk(grid, 3, CPDS_C_START, "BAT AMPS", align="center", size=SMALL)
        _put_blk(grid, 3, CPDS_R_START, _fmt_num(bat_amps, 1), align="center")

    # ---------------- Row 5 (OAT left+center span) ----------------
    oat_text = _oat_text_kelvin_to_c(oat_k)
    if oat_text.startswith("OAT"):
        rest = oat_text[3:]
        max_rest = (CPDS_W * 2) - 3
        if max_rest < 0:
            max_rest = 0
        rest = rest[:max_rest]
        put_text(grid, "OAT", 5, CPDS_L_START, size=SMALL)
        put_text(grid, rest, 5, CPDS_L_START + 3, size=LARGE)
    else:
        _put_blk_lc(grid, 5, oat_text, align="left")

    # ---------------- Row 7 "NNN %" / LOW ----------------
    fuel_lvl_l_pct = _ceil_int(fuel_lvl_l * 100.0)
    fuel_lvl_c_pct = _ceil_int(fuel_lvl_c * 100.0)
    fuel_lvl_r_pct = _ceil_int(fuel_lvl_r * 100.0)
    fuel_lvl_l_colour = "r" if fuel_lvl_l_pct <= 5 else ("a" if fuel_lvl_l_pct <= 10 else "w")
    fuel_lvl_c_colour = "r" if fuel_lvl_c_pct <= 5 else ("a" if fuel_lvl_c_pct <= 10 else "w")
    fuel_lvl_r_colour = "r" if fuel_lvl_r_pct <= 5 else ("a" if fuel_lvl_r_pct <= 10 else "w")
    low_fuel_kg = 13.5 # = 5.0Gal simulator internal logic used for "LOW FUEL" warnings
    low_l = fuel_sply1 <= low_fuel_kg
    low_c = fuel_main <= low_fuel_kg
    low_r = fuel_sply2 <= low_fuel_kg
    if low_l:
        _put_blk(grid, 7, CPDS_L_START, "LOW", align="left", colour="r")
    if low_c:
        _put_blk(grid, 7, CPDS_C_START, "LOW", align="center", colour="r")
    if low_r:
        _put_blk(grid, 7, CPDS_R_START, "LOW", align="right", colour="r")

    def _put_kg_value(row, start, value_text, align, colour):
        total_len = len(value_text) + 2
        if align == "right":
            col = start + (CPDS_W - total_len)
        elif align == "center":
            col = start + (CPDS_W - total_len) // 2
        else:
            col = start
        put_text(grid, value_text, row, col, colour=colour, size=LARGE)
        put_text(grid, "KG", row, col + len(value_text), colour=colour, size=SMALL)

    # ---------------- Row 8 "NNNKG" ----------------
    _put_kg_value(8, CPDS_L_START, f"{int(round(fuel_sply1)):>3}", "left", fuel_lvl_l_colour)
    _put_kg_value(8, CPDS_C_START, f"{int(round(fuel_main )):>3}", "center", fuel_lvl_c_colour)
    _put_kg_value(8, CPDS_R_START, f"{int(round(fuel_sply2)):>3}", "right", fuel_lvl_r_colour)

    # ---------------- Row 11 center value (RAD ALT / KT / ----) ----------------
    if knob_cds < 5:
        if rad_alt_scrl == 0:
            ft = int(round((rad_alt_raw - 1.5) * 3.28084))  
                # Subtract 1.5 m from the LVAr in meters to account for the radio altimeter sensor offset (approx. antenna height / calibration)
            rad_alt_mkr = _safe_float(vr.get("(L:radioHeightMkr)")) * 10
            rad_alt_colour = "a" if rad_alt_raw <= rad_alt_mkr else "w"
            _put_blk(grid, 11, CPDS_C_START, f" {ft} FT  ", align="right", colour=rad_alt_colour)
        else:
            _put_blk(grid, 11, CPDS_C_START, "--- KT  ", align="right")
    elif knob_cds == 5:
        _put_blk(grid, 11, CPDS_C_START, " ---- ", align="center")

    if cds_test == 1:
        # Override all test display fields regardless of other switch states
        _put_blk(grid, 0, CPDS_L_START, "888.8", align="center")
        _put_blk(grid, 0, CPDS_C_START, "8888", align="center")
        _put_blk(grid, 0, CPDS_R_START, "888.8", align="center")

        _put_blk(grid, 1, CPDS_L_START, "888.8", align="center")
        _put_blk(grid, 1, CPDS_R_START, "888.8", align="center")

        _put_blk(grid, 3, CPDS_L_START, "888.8", align="center")
        _put_blk(grid, 3, CPDS_R_START, "888.8", align="center")

        put_text(grid, "OAT", 5, CPDS_L_START, size=SMALL)
        put_text(grid, " 888 C", 5, CPDS_L_START + 3, size=LARGE)

        _put_blk(grid, 7, CPDS_L_START, "LOW", align="left", colour="r")
        _put_blk(grid, 7, CPDS_C_START, "LOW", align="center", colour="r")
        _put_blk(grid, 7, CPDS_R_START, "LOW", align="right", colour="r")

        _put_kg_value(8, CPDS_L_START, "888", "left", "r")
        _put_kg_value(8, CPDS_C_START, "888", "center", "r")
        _put_kg_value(8, CPDS_R_START, "888", "right", "r")

        _put_blk(grid, 11, CPDS_L_START, "Vne", align="center", size=SMALL)
        _put_blk(grid, 11, CPDS_C_START, "888 88", align="center")
        _put_blk(grid, 11, CPDS_R_START, "RAD ALT", align="left", size=SMALL)

        _put_blk(grid, 12, CPDS_R_START, "CABLE", align="left", size=SMALL)

        _put_blk_lc(grid, 12, "GROSS MASS", align="left", size=SMALL)

        _put_blk_lc(grid, 13, "HOOK LOAD", align="left", size=SMALL)
        _put_blk(grid, 13, CPDS_R_START, "LENGHT", align="left", size=SMALL) # intentional typo matches in-game display

    return grid


# ========================= CPDS MSG abbreviations =========================
CPDS_MSG_ABBR = {
    "ENG FAIL":   "EG FL",
    "ENG OIL P":  "OIL P",
    "FADEC FAIL": "FA FL",
    "FUEL PRESS": "FU PR",
    "ENG IDLE":   "EG ID",
    "TRAIN":      "TRN",
    "TRAIN IDLE": "TR ID",
    "ENG MANUAL": "EG MN",
    "TWIST GRIP": "TW GP",
    "FUEL VALVE": "FU VL",
    "PRIME PUMP": "PR PP",
    "DEGRADED":   "DEGR",
    "REDUND":     "RED",
    "HYD PRESS":  "HY PR",
    "GEN DISCON": "GEN D",
    "INVERTER":   "INV",
    "FIRE EXT":   "FR EX",
    "FIRE TEST":  "FR TS",
    "BUS TIE":    "BUS T",
    "STARTER":    "STRT",
}

def cpds_pick_msg(pairs):
    """
    pairs: list of (state, label) like left_pairs / right_pairs
    return: 5-char abbreviation of the highest priority active label (based on list order), or None
    """
    for state, label in pairs:
        if state == 1:
            return CPDS_MSG_ABBR.get(label, None)
    return None

def _get_cds1_pairs(vr: MobiFlightVariableRequests):
    """Collect CDS1 left/right annunciator pairs from LVARs."""
    # LEFT
    engine1_fail      = get_state(vr.get("(L:engine1Fail)"))       # ENG FAIL
    eng1_oil_pr       = get_state(vr.get("(L:engine1OilPress)"))   # ENG OIL P
    fadec1_fail       = get_state(vr.get("(L:fadecFail1)"))        # FADEC FAIL
    eng1_fuel_pr      = get_state(vr.get("(L:fuelPress1)"))        # FUEL PRESS
    eng1_idle         = get_state(vr.get("(L:eng1Idle)"))          # ENG IDLE
    train1            = get_state(vr.get("(L:train1)"))            # TRAIN
    train1_idle       = get_state(vr.get("(L:trainIdle1)"))        # TRAIN IDLE
    eng1_manual       = get_state(vr.get("(L:eng1Manual)"))        # ENG MANUAL
    twist_grip1       = get_state(vr.get("(L:twinsgrip1)"))        # TWIST GRIP
    fuel_valve1       = get_state(vr.get("(L:fuelValve1)"))        # FUEL VALVE
    prime_pump1       = get_state(vr.get("(L:primePump1)"))        # PRIME PUMP
    degraded1         = get_state(vr.get("(L:degraded1)"))         # DEGRADED
    redund1           = get_state(vr.get("(L:redund1)"))           # REDUND
    eng1_hyd_pr       = get_state(vr.get("(L:hydraulic1)"))        # HYD PRESS
    gen1_disc         = get_state(vr.get("(L:genDiscon1)"))        # GEN DISCON
    inverter1         = get_state(vr.get("(L:inv1)"))              # INVERTER
    fire_test1_ext    = get_state(vr.get("(L:fireTest1Ext)"))      # FIRE EXT
    fire_test1        = get_state(vr.get("(L:fireTest1)"))         # FIRE TEST
    bus_tie1          = get_state(vr.get("(L:bustie1)"))           # BUS TIE
    starter1          = get_state(vr.get("(L:starter1)"))          # STARTER

    # RIGHT
    engine2_fail      = get_state(vr.get("(L:engine2Fail)"))       # ENG FAIL
    eng2_oil_pr       = get_state(vr.get("(L:engine2OilPress)"))   # ENG OIL P
    fadec2_fail       = get_state(vr.get("(L:fadecFail2)"))        # FADEC FAIL
    eng2_fuel_pr      = get_state(vr.get("(L:fuelPress2)"))        # FUEL PRESS
    eng2_idle         = get_state(vr.get("(L:eng2Idle)"))          # ENG IDLE
    train2            = get_state(vr.get("(L:train2)"))            # TRAIN
    train2_idle       = get_state(vr.get("(L:trainIdle2)"))        # TRAIN IDLE
    eng2_manual       = get_state(vr.get("(L:eng2Manual)"))        # ENG MANUAL
    twist_grip2       = get_state(vr.get("(L:twinsgrip2)"))        # TWIST GRIP
    fuel_valve2       = get_state(vr.get("(L:fuelValve2)"))        # FUEL VALVE
    prime_pump2       = get_state(vr.get("(L:primePump2)"))        # PRIME PUMP
    degraded2         = get_state(vr.get("(L:degraded2)"))         # DEGRADED
    redund2           = get_state(vr.get("(L:redund2)"))           # REDUND
    eng2_hyd_pr       = get_state(vr.get("(L:hydraulic2)"))        # HYD PRESS
    gen2_disc         = get_state(vr.get("(L:genDiscon2)"))        # GEN DISCON
    inverter2         = get_state(vr.get("(L:inv2)"))              # INVERTER
    fire_test2_ext    = get_state(vr.get("(L:fireTest2Ext)"))      # FIRE EXT
    fire_test2        = get_state(vr.get("(L:fireTest2)"))         # FIRE TEST
    bus_tie2          = get_state(vr.get("(L:bustie2)"))           # BUS TIE
    starter2          = get_state(vr.get("(L:starter2)"))          # STARTER

    left_pairs = [
        (engine1_fail,   "ENG FAIL"),
        (eng1_oil_pr,    "ENG OIL P"),
        (fadec1_fail,    "FADEC FAIL"),
        (eng1_fuel_pr,   "FUEL PRESS"),
        (eng1_idle,      "ENG IDLE"),
        (train1,         "TRAIN"),
        (train1_idle,    "TRAIN IDLE"),
        (eng1_manual,    "ENG MANUAL"),
        (twist_grip1,    "TWIST GRIP"),
        (fuel_valve1,    "FUEL VALVE"),
        (prime_pump1,    "PRIME PUMP"),
        (degraded1,      "DEGRADED"),
        (redund1,        "REDUND"),
        (eng1_hyd_pr,    "HYD PRESS"),
        (gen1_disc,      "GEN DISCON"),
        (inverter1,      "INVERTER"),
        (fire_test1_ext, "FIRE EXT"),
        (fire_test1,     "FIRE TEST"),
        (bus_tie1,       "BUS TIE"),
        (starter1,       "STARTER"),
    ]

    right_pairs = [
        (engine2_fail,   "ENG FAIL"),
        (eng2_oil_pr,    "ENG OIL P"),
        (fadec2_fail,    "FADEC FAIL"),
        (eng2_fuel_pr,   "FUEL PRESS"),
        (eng2_idle,      "ENG IDLE"),
        (train2,         "TRAIN"),
        (train2_idle,    "TRAIN IDLE"),
        (eng2_manual,    "ENG MANUAL"),
        (twist_grip2,    "TWIST GRIP"),
        (fuel_valve2,    "FUEL VALVE"),
        (prime_pump2,    "PRIME PUMP"),
        (degraded2,      "DEGRADED"),
        (redund2,        "REDUND"),
        (eng2_hyd_pr,    "HYD PRESS"),
        (gen2_disc,      "GEN DISCON"),
        (inverter2,      "INVERTER"),
        (fire_test2_ext, "FIRE EXT"),
        (fire_test2,     "FIRE TEST"),
        (bus_tie2,       "BUS TIE"),
        (starter2,       "STARTER"),
    ]

    return left_pairs, right_pairs

def _get_cds1_misc_pairs(vr: MobiFlightVariableRequests):
    """Collect CDS1 miscellaneous annunciator pairs from LVARs."""
    xmsn_oil_temp     = get_state(vr.get("(L:xmsnOilTemp)"))       # XMSN OIL T
    rotor_brake       = get_state(vr.get("(L:rotorBrake)"))        # ROTOR BRAKE
    autopilot         = get_state(vr.get("(L:autopilot)"))         # AUTOPILOT
    fuel_pump_aft     = get_state(vr.get("(L:fuelPumpAft)"))       # F PUMP AFT
    fuel_pump_fwd     = get_state(vr.get("(L:fuelPumpFwd)"))       # F PUMP FWD
    bat_disc          = get_state(vr.get("(L:batDisc)"))           # BAT DISCON
    ext_power         = get_state(vr.get("(L:extPower)"))          # EXT POWER
    shed_emer         = get_state(vr.get("(L:shedEmer)"))          # SHED EMER

    return [
        (xmsn_oil_temp, "XMSN OIL T"),
        (rotor_brake,   "ROTOR BRAKE"),
        (autopilot,     "AUTOPILOT"),
        (fuel_pump_aft, "F PUMP AFT"),
        (fuel_pump_fwd, "F PUMP FWD"),
        (bat_disc,      "BAT DISCON"),
        (ext_power,     "EXT POWER"),
        (shed_emer,     "SHED EMER"),
    ]

def _get_cds1_green_states(vr: MobiFlightVariableRequests):
    """Collect CDS1 green status annunciator states."""
    pitot_pilot       = get_state(vr.get("(L:pitotPilot)"))        # P/S-HTR-P
    pitot_copilot     = get_state(vr.get("(L:pitotCoPilot)"))      # P/S-HTR-C
    cds_ack           = get_state(vr.get("(L:cdsSelfTestAcknoledge)"))  # CDS & INP PASSED
    land_light        = get_state(vr.get("(L:landLight)"))         # LDG LIGHT
    land_light_ext    = get_state(vr.get("(L:landLightExtr)"))     # LDG LIGHT RET/EXT
    air_cond          = get_state(vr.get("(L:airCond)"))           # AIR CON

    return pitot_pilot, pitot_copilot, cds_ack, land_light, land_light_ext, air_cond

class Cds1DisplayThread:
    """Background renderer for CDS1/MISC pages on the MCDU."""
    def __init__(self, vr: MobiFlightVariableRequests, mcdu_primary: McduSocket, mcdu_alt: McduSocket, tick: float = 0.1):
        self.vr = vr
        self.mcdu = mcdu_primary
        self.mcdu_alt = mcdu_alt
        self.tick = tick
        self._stop = threading.Event()
        self._thread = threading.Thread(target=self._run, name="CDS1Thread", daemon=True)

    def start(self):
        self._send_initial()
        self._thread.start()

    def stop(self):
        self._stop.set()
        self._thread.join(timeout=1.0)

    def _send_initial(self):
        cds1_grid = empty_grid()
        clear_area_with_spaces(cds1_grid, 0, CDU_ROWS-1)  # full screen spaces
        put_text_center(cds1_grid, "MISC", 6, colour="k", size=LARGE)
        cds_swap = get_state(self.vr.get("(L:cds_Swap)"))  # Swap CDS display between captain/copilot MCDUs
        select_mcdu(self.mcdu, self.mcdu_alt, cds_swap).send_grid(cds1_grid)

    def _run(self):
        row_11 = row_12 = row_13 = None  # pylint: disable=invalid-name

        while not self._stop.is_set():
            try:
                # NOTE: "CIRCUIT GENERAL PANEL ON" is a general panel power circuit SimVar.
                # In MSFS it indicates whether the main/panel bus is supplying power to the
                # cockpit panels, not a dedicated "avionics master" line. For the EC135
                # profile we treat this as "display power available" for the CPDS and
                # blank the display whenever this SimVar is 0.
                avionics_on  = get_state(self.vr.get("(A:CIRCUIT GENERAL PANEL ON,Bool)"))
                cds1_page    = get_state(self.vr.get("(L:cdsPage)")) # 0 - 2
                cds1_breaker = get_state(self.vr.get("(L:brkCDS1)"))
                cds_swap     = get_state(self.vr.get("(L:cds_Swap)"))  # Swap CDS display between captain/copilot MCDUs                

                left_pairs, right_pairs = _get_cds1_pairs(self.vr)
                misc_pairs = _get_cds1_misc_pairs(self.vr)
                pitot_pilot, pitot_copilot, cds_ack, land_light, land_light_ext, air_cond = _get_cds1_green_states(self.vr)

                left_labels  = compact_labels(left_pairs)
                right_labels = compact_labels(right_pairs)
                misc_labels  = [label for val, label in misc_pairs if val == 1]

                page_size = 6
                start = cds1_page * page_size
                end   = start + page_size

                visible_left  = left_labels[start:end]
                visible_right = right_labels[start:end]
                visible_misc  = misc_labels[start:end]

                cds1_grid = empty_grid()
                put_text_center(cds1_grid, "MISC", 6, colour="k", size=LARGE)
                if avionics_on == 0:
                    clear_area_with_spaces(cds1_grid, 0, CDU_ROWS-1)
                    put_text_center(cds1_grid, "MISC", 6, colour="k", size=LARGE)
                elif cds1_breaker == 1:  # Check if CDS has power
                    # left/right columns (clears rows 1..6 internally)
                    draw_columns(cds1_grid, visible_left, visible_right)

                    # --- MISC block (2 columns × 3 rows, fill left column first) ---
                    # Split page into two 3-item columns
                    misc_left  = visible_misc[:3]
                    misc_right = visible_misc[3:6]

                    # Paint left column (cols 0..11), rows 8..10
                    for i, label in enumerate(misc_left):
                        put_text(cds1_grid, label[:11].ljust(11), 7 + i, 0, colour="a", size=LARGE)

                    # Paint right column (cols 13..23), rows 8..10
                    for i, label in enumerate(misc_right):
                        put_text(cds1_grid, label[:11].ljust(11), 7 + i, 13, colour="a", size=LARGE)

                    # --- Green block ---
                    if pitot_pilot == 1:
                        put_text(cds1_grid, "P/S-HTR-P", 10, 0,  colour="g", size=LARGE)
                    if pitot_copilot  == 1:
                        put_text(cds1_grid, "P/S-HTR-C", 10, 13, colour="g", size=LARGE)

                    if cds_ack == 0: # pylint: disable=use-implicit-booleaness-not-comparison-to-zero
                        row_11, row_12, row_13 = "CDS PASSED", "INP PASSED", None
                    else:
                        row_11 = "LDG L EXT" if land_light_ext == 1 else "LDG L RET"
                        if land_light == 1:
                            row_12, row_13 = "LDG LIGHT", ("AIR COND " if air_cond == 1 else None)
                        else:
                            row_12, row_13 = ("AIR COND " if air_cond == 1 else None), None

                    for r, txt in ((11, row_11), (12, row_12), (13, row_13)):
                        if txt:
                            put_text_center(cds1_grid, txt, r, colour="g", size=LARGE)
                else:
                    clear_area_with_spaces(cds1_grid, 0, CDU_ROWS-1)
                    put_text_center(cds1_grid, "MISC", 6, colour="k", size=LARGE)
                select_mcdu(self.mcdu, self.mcdu_alt, cds_swap).send_grid(cds1_grid)

            except Exception as e:
                logging.exception("CDS1 loop error: %s", e)

            sleep(self.tick)

class CpdsDisplayThread:
    """Background renderer for the CPDS (copilot) display."""
    def __init__(self, vr: MobiFlightVariableRequests, mcdu_primary: McduSocket, mcdu_alt: McduSocket, tick: float = 0.1):
        self.vr = vr
        self.mcdu = mcdu_primary
        self.mcdu_alt = mcdu_alt
        self.tick = tick
        self._stop = threading.Event()
        self._thread = threading.Thread(target=self._run, name="CPDSThread", daemon=True)

    def start(self):
        self._send_initial()
        self._thread.start()

    def stop(self):
        self._stop.set()
        self._thread.join(timeout=1.0)

    def _send_initial(self):
        cpds_grid = empty_grid()
        clear_area_with_spaces(cpds_grid, 0, CDU_ROWS-1)
        put_text_center(cpds_grid, "CPDS", 6, colour="k", size=LARGE)
        cds_swap = get_state(self.vr.get("(L:cds_Swap)"))  # Swap CDS display between captain/copilot MCDUs
        select_mcdu(self.mcdu, self.mcdu_alt, cds_swap).send_grid(cpds_grid)

    def _run(self):
        while not self._stop.is_set():
            try:
                avionics_on  = get_state(self.vr.get("(A:CIRCUIT GENERAL PANEL ON,Bool)"))
                cpds_breaker = get_state(self.vr.get("(L:brkCDS2)"))
                knob_cds     = get_state(self.vr.get("(L:knobCdsMode)"))  # Range: 0-5
                cpds_scroll  = get_state(self.vr.get("(L:cdsDisplayScroll)"))  # Range: 0-5
                volt_amp     = get_state(self.vr.get("(L:voltampScroll)")) # 0 - 2
                rad_alt_scrl = get_state(self.vr.get("(L:cdsVneRadAltScroll)"))  # toggle RAD ALT vs VNE/KT display
                cds_test     = get_state(self.vr.get("(L:switchCDStest)"))
                cds_swap     = get_state(self.vr.get("(L:cds_Swap)"))  # Swap CDS display between captain/copilot MCDUs

                left_pairs, right_pairs = _get_cds1_pairs(self.vr)
                msg1 = cpds_pick_msg(left_pairs)
                msg2 = cpds_pick_msg(right_pairs)

                cpds_grid = empty_grid()
                if avionics_on == 0:
                    clear_area_with_spaces(cpds_grid, 0, CDU_ROWS-1)
                elif cpds_breaker == 1:
                    cpds_grid = build_cpds_grid(self.vr, knob_cds, cpds_scroll, volt_amp, rad_alt_scrl, cds_test, msg1, msg2)
                else:
                    clear_area_with_spaces(cpds_grid, 0, CDU_ROWS-1)
                    # Intentionally disabled: older behavior showed an explicit "CPDS OFF" label when the CPDS breaker was out.
                    # Keep this call commented so users can re-enable the overlay for debugging or custom setups without changing defaults.
                    # put_text_center(cpds_grid, "CPDS OFF", 6, colour="k", size=LARGE)

                select_mcdu(self.mcdu, self.mcdu_alt, cds_swap).send_grid(cpds_grid)

            except Exception as e:
                logging.exception("CPDS loop error: %s", e)

            sleep(self.tick)


# ========================= MAIN =========================
if __name__ == "__main__":
    # Uncomment to log to file + console:
    # setup_logging("SimConnectMobiFlight.log")

    # SimConnect / MobiFlight var reader
    sm = SimConnectMobiFlight()
    vr = MobiFlightVariableRequests(sm)
    vr.clear_sim_variables()

    # MCDU sockets (captain and copilot)
    mcdu_capt = McduSocket(CAPT_MCDU_URL)
    mcdu_copi = McduSocket(COPI_MCDU_URL)

    cds1_display = Cds1DisplayThread(vr, mcdu_capt, mcdu_copi)
    cpds_display = CpdsDisplayThread(vr, mcdu_copi, mcdu_capt)
    cds1_display.start()
    cpds_display.start()

    try:
        while True:
            sleep(1.0)
    except KeyboardInterrupt:
        pass
    finally:
        cds1_display.stop()
        cpds_display.stop()
        mcdu_capt.close()
        mcdu_copi.close()

