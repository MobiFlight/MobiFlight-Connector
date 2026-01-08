# H135_all_in_one
# Single-file bridge: SimConnect(MobiFlight LVARs) -> WinWing MCDU (captain)
# Requires: pip install websockets SimConnect==0.4.24 (or your working SimConnect lib)
# CREDITS: Koseng on GitHub and his MSFSPythonSimConnectMobiFlightExtension (https://github.com/Koseng/MSFSPythonSimConnectMobiFlightExtension)
# pylint: disable=redefined-outer-name,broad-exception-caught


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
        wait_counter = 0
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
        logging.debug("set: %s", variable_string)
        self.send_command("MF.SimVars.Set." + variable_string)

    def clear_sim_variables(self):
        logging.info("clear_sim_variables")
        self.sim_vars.clear()
        self.sim_var_name_to_id.clear()
        self.send_command("MF.SimVars.Clear")

# ========================= Logging =========================
def setup_logging(log_file_name):
    log_formatter = logging.Formatter("%(asctime)s [%(levelname)-5.5s]  %(message)s")
    root_logger = logging.getLogger()
    root_logger.setLevel(logging.DEBUG)
    file_handler = logging.handlers.RotatingFileHandler(log_file_name, maxBytes=500000, backupCount=7)
    file_handler.setFormatter(log_formatter)
    root_logger.addHandler(file_handler)
    console_handler = logging.StreamHandler()
    console_handler.setFormatter(log_formatter)
    root_logger.addHandler(console_handler)

# ========================= MCDU display primitives =========================
CDU_COLUMNS = 24
CDU_ROWS = 14
LARGE = 0
SMALL = 1
Cell = List[Union[str, int]]  # [] or [char, colour, size]

def empty_grid() -> List[List[Cell]]:
    return [[[] for _ in range(CDU_COLUMNS)] for _ in range(CDU_ROWS)]

REPLACED = {
    "←":"\u2190","→":"\u2192","↑":"\u2191","↓":"\u2193",
    "_":"\u2610","\u00b0":"°","&":"\u0394","\xa0":" ",
    "{":"\u2190","}":"\u2192","|":"/",
}

def put_text(grid: List[List[Cell]], text: str, row: int, col: int, colour="a", size=LARGE):
    if not 0 <= row < CDU_ROWS:
        return
    for i, ch in enumerate(text):
        cc = col + i
        if 0 <= cc < CDU_COLUMNS:
            grid[row][cc] = [REPLACED.get(ch, ch), colour, size]

def put_text_center(grid: List[List[Cell]], text: str, row: int, colour="a", size=LARGE):
    text = text[:CDU_COLUMNS]  # safety
    col = (CDU_COLUMNS - len(text)) // 2
    put_text(grid, text, row, col, colour=colour, size=size)


def grid_to_payload(grid: List[List[Cell]]) -> str:
    return json.dumps({"Target": "Display", "Data": list(chain(*grid))})

# ========================= Rolling list layout =========================
LEFT_COL_START  = 0
RIGHT_COL_START = 13
CONTENT_FIRST_ROW = 0
CONTENT_LAST_ROW  = 5
MAX_ROWS = CONTENT_LAST_ROW - CONTENT_FIRST_ROW + 1

def clear_area_with_spaces(grid, r0, r1, c0=0, c1=CDU_COLUMNS, colour="w", size=0):
    for r in range(r0, r1 + 1):
        for c in range(c0, c1):
            grid[r][c] = [" ", colour, size]

def compact_labels(pairs):
    return [label for val, label in pairs if val == 1]

def draw_columns(grid: List[List[Cell]], left_labels: List[str], right_labels: List[str]):
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

    This function converts various LVAR input types (None, bool, int, float, or str)
    into one of three integer states:
        0 = OFF / inactive
        1 = ON / active
        2 = PAGE / extended state

    Numeric values are interpreted using thresholds:
        < 0.5  -> 0
        < 1.5  -> 1
        >= 1.5 -> 2

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
        return 2

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

                        # Drain any newer payloads (keep only the latest)
                        try:
                            while True:
                                payload = self._queue.get_nowait()
                        except asyncio.QueueEmpty:
                            pass

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
            if self._loop:
                self._loop.call_soon_threadsafe(lambda: None)
        except Exception:
            pass

# ========================= MAIN =========================
if __name__ == "__main__":
    # Uncomment to log to file + console:
    # setup_logging("SimConnectMobiFlight.log")

    # SimConnect / MobiFlight var reader
    sm = SimConnectMobiFlight()
    vr = MobiFlightVariableRequests(sm)
    vr.clear_sim_variables()

    # MCDU socket (captain)
    MCDU_URL = "ws://127.0.0.1:8320/winwing/cdu-captain"
    mcdu = McduSocket(MCDU_URL)

    # initial screen
    grid = empty_grid()
    clear_area_with_spaces(grid, 0, CDU_ROWS-1)  # full screen spaces
    put_text_center(grid, "MISC", 6, colour="k", size=LARGE)
    mcdu.send_grid(grid)
    row_11 = row_12 = row_13 = None  # pylint: disable=invalid-name

##    NOTE:
##    vr.get() is only potentially blocking during first-time registration of an LVAR,
##    while waiting for the initial SimConnect client-data callback.
##    After initialization, all LVAR values are updated asynchronously via
##    client_data_callback_handler(), and subsequent vr.get() calls are simple
##    non-blocking dictionary reads.
##    Therefore, no external batching loop is required here.

    while True:
        try:
            # HELPERS
            cds_page     = get_state(vr.get("(L:cdsPage)"))
            cds_breaker  = get_state(vr.get("(L:brkCDS1)"))

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

            # MISC
            xmsn_oil_temp     = get_state(vr.get("(L:xmsnOilTemp)"))       # XMSN OIL T
            rotor_brake       = get_state(vr.get("(L:rotorBrake)"))        # ROTOR BRAKE
            autopilot         = get_state(vr.get("(L:autopilot)"))         # AUTOPILOT
            fuel_pump_aft     = get_state(vr.get("(L:fuelPumpAft)"))       # F PUMP AFT
            fuel_pump_fwd     = get_state(vr.get("(L:fuelPumpFwd)"))       # F PUMP FWD
            bat_disc          = get_state(vr.get("(L:batDisc)"))           # BAT DISCON
            ext_power         = get_state(vr.get("(L:extPower)"))          # EXT POWER
            shed_emer         = get_state(vr.get("(L:shedEmer)"))          # SHED EMER

            # GREEN
            pitot_pilot       = get_state(vr.get("(L:pitotPilot)"))        # P/S-HTR-P
            pitot_copilot     = get_state(vr.get("(L:pitotCoPilot)"))      # P/S-HTR-C
            cds_ack           = get_state(vr.get("(L:cdsSelfTestAcknoledge)"))  # CDS & INP PASSED
            land_light        = get_state(vr.get("(L:landLight)"))         # LDG LIGHT
            land_light_ext    = get_state(vr.get("(L:landLightExtr)"))     # LDG LIGHT RET/EXT
            air_cond          = get_state(vr.get("(L:airCond)"))           # AIR CON

            # -------- Build rolling (compacted) lists --------
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

            misc_pairs = [
                (xmsn_oil_temp, "XMSN OIL T"),
                (rotor_brake,   "ROTOR BRAKE"),
                (autopilot,     "AUTOPILOT"),
                (fuel_pump_aft, "F PUMP AFT"),
                (fuel_pump_fwd, "F PUMP FWD"),
                (bat_disc,      "BAT DISCON"),
                (ext_power,     "EXT POWER"),
                (shed_emer,     "SHED EMER"),
            ]

            # build labels
            left_labels  = compact_labels(left_pairs)
            right_labels = compact_labels(right_pairs)
            misc_labels  = [label for val, label in misc_pairs if val == 1]

            # paging (cds_page: 0,1,2) — 6 rows per page for ALL THREE lists
            page_size = 6
            start = cds_page * page_size
            end   = start + page_size

            visible_left  = left_labels[start:end]
            visible_right = right_labels[start:end]
            visible_misc  = misc_labels[start:end]

            # Draw & send
            grid = empty_grid()
            put_text_center(grid, "MISC", 6, colour="k", size=LARGE)
            if cds_breaker == 1:  # Check if CDS has power
                # left/right columns (clears rows 1..6 internally)
                draw_columns(grid, visible_left, visible_right)

                # --- MISC block (2 columns × 3 rows, fill left column first) ---
                # Split page into two 3-item columns
                misc_left  = visible_misc[:3]
                misc_right = visible_misc[3:6]

                # Paint left column (cols 0..11), rows 8..10
                for i, label in enumerate(misc_left):
                    put_text(grid, label[:11].ljust(11), 7 + i, 0, colour="a", size=LARGE)

                # Paint right column (cols 13..23), rows 8..10
                for i, label in enumerate(misc_right):
                    put_text(grid, label[:11].ljust(11), 7 + i, 13, colour="a", size=LARGE)

                # --- Green block ---
                if pitot_pilot == 1:
                    put_text(grid, "P/S-HTR-P", 10, 0,  colour="g", size=LARGE)
                if pitot_copilot  == 1:
                    put_text(grid, "P/S-HTR-C", 10, 13, colour="g", size=LARGE)

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
                        put_text_center(grid, txt, r, colour="g", size=LARGE)
            # MCDU send
            mcdu.send_grid(grid)

        except Exception as e:
            logging.exception("Loop error: %s", e)

        sleep(0.1)  # tick rate
