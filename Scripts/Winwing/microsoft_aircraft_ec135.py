# H135_all_in_one
# Single-file bridge: SimConnect(MobiFlight LVARs) -> WinWing MCDU (captain)
# Requires: pip install websocket-client SimConnect==0.4.24 (or your working SimConnect lib)
# CREDITS: Koseng on GitHub and his MSFSPythonSimConnectMobiFlightExtension (https://github.com/Koseng/MSFSPythonSimConnectMobiFlightExtension)

import json
import logging
import logging.handlers
import ctypes
from ctypes import wintypes
from time import sleep, time
from typing import List, Union, Optional
from itertools import chain
from websocket import create_connection, WebSocket, WebSocketException

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
        self.dll.MapClientDataNameToID.argtypes = [wintypes.HANDLE, ctypes.c_char_p, SIMCONNECT_CLIENT_DATA_ID]

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
import struct
from ctypes import sizeof
from ctypes.wintypes import FLOAT

class SimVariable:
    """
    Represents a simulation variable used in MobiFlight variable requests.

    Attributes:
        id (int): Unique identifier for the simulation variable.
        name (str): Name of the simulation variable.
        float_value (float, optional): The current value of the variable as a float.
        initialized (bool): Indicates whether the variable has been initialized.
    """
    def __init__(self, id, name, float_value=None):
        self.id = id
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
            offset = (var_id - 1) * sizeof(FLOAT)
            self.add_to_client_data_definition(var_id, offset, sizeof(FLOAT))
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

    def set(self, variableString):
        logging.debug("set: %s", variableString)
        self.send_command("MF.SimVars.Set." + variableString)

    def clear_sim_variables(self):
        logging.info("clear_sim_variables")
        self.sim_vars.clear()
        self.sim_var_name_to_id.clear()
        self.send_command("MF.SimVars.Clear")

# ========================= Logging =========================
def setup_Logging(logFileName):
    logFormatter = logging.Formatter("%(asctime)s [%(levelname)-5.5s]  %(message)s")
    rootLogger = logging.getLogger()
    rootLogger.setLevel(logging.DEBUG)
    fileHandler = logging.handlers.RotatingFileHandler(logFileName, maxBytes=500000, backupCount=7)
    fileHandler.setFormatter(logFormatter)
    rootLogger.addHandler(fileHandler)
    consoleHandler = logging.StreamHandler()
    consoleHandler.setFormatter(logFormatter)
    rootLogger.addHandler(consoleHandler)

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
    if not (0 <= row < CDU_ROWS): return
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
        if row > CONTENT_LAST_ROW: break
    # RIGHT 11 chars
    row = CONTENT_FIRST_ROW
    for lbl in right_labels[:MAX_ROWS]:
        put_text(grid, lbl[:11].ljust(11), row, RIGHT_COL_START, colour="a", size=LARGE)
        row += 1
        if row > CONTENT_LAST_ROW: break

def get_state(v) -> int:
    try:
        if v is None: return 0
        if isinstance(v, bool): return 1 if v else 0
        if isinstance(v, (int, float)):
            f = float(v)
            if f < 0.5: return 0
            elif f < 1.5: return 1
            else: return 2
        s = str(v).strip().strip('"').strip("'").lower()
        if s in ("2", "two"): return 2
        if s in ("1", "true", "on", "yes", "y"): return 1
        if s in ("0", "false", "off", "no", "n", ""): return 0
        f = float(s)
        if f < 0.5: return 0
        elif f < 1.5: return 1
        else: return 2
    except: return 0


# ========================= Simple persistent WebSocket =========================
class McduSocket:
    """
    Manages a persistent WebSocket connection to the WinWing MCDU hardware.
    Handles connection establishment, reconnection, pinging to keep the connection alive,
    and sending grid data to the MCDU device.
    """
    def __init__(self, url: str, connect_timeout: float = 2.0):
        self.url = url
        self.ws: Optional[WebSocket] = None
        self.connect_timeout = connect_timeout
        self._last_ping = 0.0
        self._ping_interval = 20.0

    def _connect(self):
        logging.info(f"Connecting to MCDU at {self.url}")
        self.ws = create_connection(self.url, timeout=self.connect_timeout)
        self.ws.settimeout(1.0)
        logging.info("MCDU connected.")

    def _ensure(self):
        if self.ws is None:
            self._connect()

    def _maybe_ping(self):
        now = time()
        if now - self._last_ping >= self._ping_interval:
            try:
                self.ws.ping("")
                self._last_ping = now
            except Exception:
                try:
                    self.ws.close()
                except Exception as e:
                    logging.debug(f"Exception during ws.close(): {e}")
                self.ws = None

    def send_grid(self, grid: List[List[Cell]]):
        payload = grid_to_payload(grid)
        for attempt in (1, 2):
            try:
                self._ensure()
                self.ws.send(payload)
                logging.debug(f"→ MCDU SEND {len(payload)} bytes")
                self._maybe_ping()
                return
            except (WebSocketException, OSError) as e:
                logging.debug(f"MCDU send attempt {attempt} failed: {e}")
                try:
                    if self.ws: self.ws.close()
                except Exception:
                    # Ignore errors during socket close; best effort cleanup
                    pass
                self.ws = None
        logging.debug("MCDU send failed after retry.")

# ========================= MAIN =========================
if __name__ == "__main__":
    # Uncomment to log to file + console:
    # setup_Logging("SimConnectMobiFlight.log")

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

    """
    NOTE:
    vr.get() is only potentially blocking during first-time registration of an LVAR,
    while waiting for the initial SimConnect client-data callback.
    After initialization, all LVAR values are updated asynchronously via
    client_data_callback_handler(), and subsequent vr.get() calls are simple
    non-blocking dictionary reads.
    
    Therefore, no external batching loop is required here.
    """
    while True:
        try:
            # HELPERS
            cdsPage     = get_state(vr.get("(L:cdsPage)"))
            cdsBreaker  = get_state(vr.get("(L:brkCDS1)"))
            
            # LEFT
            engine1Fail = get_state(vr.get("(L:engine1Fail)"))     # ENG FAIL
            eng1OilPr   = get_state(vr.get("(L:engine1OilPress)")) # ENG OIL P
            fadec1Fail  = get_state(vr.get("(L:fadecFail1)"))      # FADEC FAIL
            eng1FuelPr  = get_state(vr.get("(L:fuelPress1)"))      # FUEL PRESS
            eng1Idle    = get_state(vr.get("(L:eng1Idle)"))        # ENG IDLE
            train1      = get_state(vr.get("(L:train1)"))          # TRAIN
            train1idle  = get_state(vr.get("(L:trainIdle1)"))      # TRAIN IDLE
            eng1Manual  = get_state(vr.get("(L:eng1Manual)"))      # ENG MANUAL
            twinsgrip1  = get_state(vr.get("(L:twinsgrip1)"))      # TWIST GRIP
            fuelValve1  = get_state(vr.get("(L:fuelValve1)"))      # FUEL VALVE
            primePump1  = get_state(vr.get("(L:primePump1)"))      # PRIME PUMP
            degraded1   = get_state(vr.get("(L:degraded1)"))       # DEGRADED
            redund1     = get_state(vr.get("(L:redund1)"))         # REDUND
            eng1HydPr   = get_state(vr.get("(L:hydraulic1)"))      # HYD PRESS
            gen1disc    = get_state(vr.get("(L:genDiscon1)"))      # GEN DISCON
            inverter1   = get_state(vr.get("(L:inv1)"))            # INVERTER
            fireTest1Ext= get_state(vr.get("(L:fireTest1Ext)"))    # FIRE EXT
            fireTest1   = get_state(vr.get("(L:fireTest1)"))       # FIRE TEST
            bustie1     = get_state(vr.get("(L:bustie1)"))         # BUS TIE
            starter1    = get_state(vr.get("(L:starter1)"))        # STARTER

            # RIGHT
            engine2Fail = get_state(vr.get("(L:engine2Fail)"))     # ENG FAIL
            eng2OilPr   = get_state(vr.get("(L:engine2OilPress)")) # ENG OIL P
            fadec2Fail  = get_state(vr.get("(L:fadecFail2)"))      # FADEC FAIL
            eng2FuelPr  = get_state(vr.get("(L:fuelPress2)"))      # FUEL PRESS
            eng2Idle    = get_state(vr.get("(L:eng2Idle)"))        # ENG IDLE
            train2      = get_state(vr.get("(L:train2)"))          # TRAIN
            train2idle  = get_state(vr.get("(L:trainIdle2)"))      # TRAIN IDLE
            eng2Manual  = get_state(vr.get("(L:eng2Manual)"))      # ENG MANUAL
            twinsgrip2  = get_state(vr.get("(L:twinsgrip2)"))      # TWIST GRIP
            fuelValve2  = get_state(vr.get("(L:fuelValve2)"))      # FUEL VALVE
            primePump2  = get_state(vr.get("(L:primePump2)"))      # PRIME PUMP
            degraded2   = get_state(vr.get("(L:degraded2)"))       # DEGRADED
            redund2     = get_state(vr.get("(L:redund2)"))         # REDUND
            eng2HydPr   = get_state(vr.get("(L:hydraulic2)"))      # HYD PRESS
            gen2disc    = get_state(vr.get("(L:genDiscon2)"))      # GEN DISCON
            inverter2   = get_state(vr.get("(L:inv2)"))            # INVERTER
            fireTest2Ext= get_state(vr.get("(L:fireTest2Ext)"))    # FIRE EXT
            fireTest2   = get_state(vr.get("(L:fireTest2)"))       # FIRE TEST
            bustie2     = get_state(vr.get("(L:bustie2)"))         # BUS TIE
            starter2    = get_state(vr.get("(L:starter2)"))        # STARTER

            # MISC
            xmsnOilTemp= get_state(vr.get("(L:xmsnOilTemp)"))      # XMSN OIL T
            rotorBrake = get_state(vr.get("(L:rotorBrake)"))       # ROTOR BRAKE
            autopilot  = get_state(vr.get("(L:autopilot)"))        # AUTOPILOT
            fuelPumpAf = get_state(vr.get("(L:fuelPumpAft)"))      # F PUMP AFT
            fuelPumpFw = get_state(vr.get("(L:fuelPumpFwd)"))      # F PUMP FWD
            batDisc    = get_state(vr.get("(L:batDisc)"))          # BAT DISCON
            extPower   = get_state(vr.get("(L:extPower)"))         # EXT POWER
            shedEmer   = get_state(vr.get("(L:shedEmer)"))         # SHED EMER

            # GREEN
            pitotPilot = get_state(vr.get("(L:pitotPilot)"))       # P/S-HTR-P
            pitotCoPi  = get_state(vr.get("(L:pitotCoPilot)"))     # P/S-HTR-C
            cdsAck     = get_state(vr.get("(L:cdsSelfTestAcknoledge)"))       # CDS PASSED & INP PASSED
            landLight  = get_state(vr.get("(L:landLight)"))        # LDG LIGHT
            landLiExt  = get_state(vr.get("(L:landLightExtr)"))    # LDG LIGHT RET/EXT
            airCond    = get_state(vr.get("(L:airCond)"))          # AIR COND
            
            # -------- Build rolling (compacted) lists --------
            left_pairs = [
                (engine1Fail,  "ENG FAIL"),
                (eng1OilPr,    "ENG OIL P"),
                (fadec1Fail,   "FADEC FAIL"),
                (eng1FuelPr,   "FUEL PRESS"),
                (eng1Idle,     "ENG IDLE"),
                (train1,       "TRAIN"),
                (train1idle,   "TRAIN IDLE"),
                (eng1Manual,   "ENG MANUAL"),
                (twinsgrip1,   "TWIST GRIP"),
                (fuelValve1,   "FUEL VALVE"),
                (primePump1,   "PRIME PUMP"),
                (degraded1,    "DEGRADED"),
                (redund1,      "REDUND"),
                (eng1HydPr,    "HYD PRESS"),
                (gen1disc,     "GEN DISCON"),
                (inverter1,    "INVERTER"),
                (fireTest1Ext, "FIRE EXT"),
                (fireTest1,    "FIRE TEST"),
                (bustie1,      "BUS TIE"),
                (starter1,     "STARTER"),
            ]

            right_pairs = [
                (engine2Fail,  "ENG FAIL"),
                (eng2OilPr,    "ENG OIL P"),
                (fadec2Fail,   "FADEC FAIL"),
                (eng2FuelPr,   "FUEL PRESS"),
                (eng2Idle,     "ENG IDLE"),
                (train2,       "TRAIN"),
                (train2idle,   "TRAIN IDLE"),
                (eng2Manual,   "ENG MANUAL"),
                (twinsgrip2,   "TWIST GRIP"),
                (fuelValve2,   "FUEL VALVE"),
                (primePump2,   "PRIME PUMP"),
                (degraded2,    "DEGRADED"),
                (redund2,      "REDUND"),
                (eng2HydPr,    "HYD PRESS"),
                (gen2disc,     "GEN DISCON"),
                (inverter2,    "INVERTER"),
                (fireTest2Ext, "FIRE EXT"),
                (fireTest2,    "FIRE TEST"),
                (bustie2,      "BUS TIE"),
                (starter2,     "STARTER"),
            ]

            misc_pairs = [
                (xmsnOilTemp, "XMSN OIL T"),
                (rotorBrake,  "ROTOR BRAKE"),
                (autopilot,   "AUTOPILOT"),
                (fuelPumpAf,  "F PUMP AFT"),
                (fuelPumpFw,  "F PUMP FWD"),
                (batDisc,     "BAT DISCON"),
                (extPower,    "EXT POWER"),
                (shedEmer,    "SHED EMER"),
            ]

            # build labels
            left_labels  = compact_labels(left_pairs)
            right_labels = compact_labels(right_pairs)
            misc_labels  = [label for val, label in misc_pairs if val == 1]

            # paging (cdsPage: 0,1,2) — 6 rows per page for ALL THREE lists
            page_size = 6
            start = cdsPage * page_size
            end   = start + page_size

            visible_left  = left_labels[start:end]
            visible_right = right_labels[start:end]
            visible_misc  = misc_labels[start:end]

            # Draw & send
            grid = empty_grid()
            put_text_center(grid, "MISC", 6, colour="k", size=LARGE)
            if cdsBreaker == 1:  # Check if CDS has power
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
                if pitotPilot == 1: put_text(grid, "P/S-HTR-P", 10, 0,  colour="g", size=LARGE)
                if pitotCoPi  == 1: put_text(grid, "P/S-HTR-C", 10, 13, colour="g", size=LARGE)

                if cdsAck == 0:
                    row11, row12, row13 = "CDS PASSED", "INP PASSED", None
                else:
                    row11 = "LDG L EXT" if landLiExt == 1 else "LDG L RET"
                    if landLight == 1:
                        row12, row13 = "LDG LIGHT", ("AIR COND " if airCond == 1 else None)
                    else:
                        row12, row13 = ("AIR COND " if airCond == 1 else None), None

                for r, txt in ((11, row11), (12, row12), (13, row13)):
                    if txt: put_text_center(grid, txt, r, colour="g", size=LARGE)
            # MCDU send
            mcdu.send_grid(grid)

        except Exception as e:
            logging.exception(f"Loop error: {e}")

        sleep(0.1)  # tick rate
