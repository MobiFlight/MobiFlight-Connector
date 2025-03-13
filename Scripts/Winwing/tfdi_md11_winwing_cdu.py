from ctypes import wintypes, Structure, c_uint16, c_bool
import ctypes
import json
import logging
import asyncio
import websockets.asyncio.client as ws_client
from typing import Optional, List, Dict, Union, Any
from SimConnect import SimConnect, Enum
from SimConnect.Enum import SIMCONNECT_CLIENT_DATA_ID, SIMCONNECT_RECV_ID, SIMCONNECT_RECV_CLIENT_DATA

# URLs
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
CENTER_CDU_URL: str = "ws://localhost:8320/winwing/cdu-observer"
CO_PILOT_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"

class MCDUChar(Structure):
    _pack_ = 1
    _fields_ = [
        ("value", c_uint16),
        ("large", c_bool)
    ]

class MCDUStatus(Structure):
    _pack_ = 1
    _fields_ = [
        ("dspy", c_bool),
        ("fail", c_bool),
        ("msg", c_bool),
        ("ofst", c_bool)
    ]

# Constants for MD11 MCDU
MCDU_COLUMNS: int = 24
MCDU_ROWS: int = 14
MCDU_CHARS: int = MCDU_COLUMNS * MCDU_ROWS

# Calculate MCDU_DATA_SIZE just like in the C++ code
MCDU_DATA_SIZE: int = (ctypes.sizeof(MCDUChar) * MCDU_CHARS) + (ctypes.sizeof(c_bool) * 4)

# MD11 Client Data Area Names and IDs
MD11_MCDU_NAME: str = "MD11MCDU"
MD11_MCDU_CLIENT_DATA_ID: int = 0  # CLIENT_DATA_ID_MCDU

# MD11 MCDU Definition IDs
MD11_MCDU_LEFT_DEFINITION: int = 0    # CLIENT_DATA_DEFINE_ID_LMCDU
MD11_MCDU_CENTER_DEFINITION: int = 1  # CLIENT_DATA_DEFINE_ID_CMCDU
MD11_MCDU_RIGHT_DEFINITION: int = 2   # CLIENT_DATA_DEFINE_ID_RMCDU


class SimConnectMobiFlight(SimConnect):
    def __init__(self, auto_connect=True, library_path=None):
        self.client_data_handlers = []
        if library_path:
            super().__init__(auto_connect, library_path)
        else:
            super().__init__(auto_connect)
        # Fix missing types
        self.dll.MapClientDataNameToID.argtypes = [wintypes.HANDLE, ctypes.c_char_p, SIMCONNECT_CLIENT_DATA_ID]

    def register_client_data_handler(self, handler):
        if not handler in self.client_data_handlers:
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

class MobiFlightClient:
    def __init__(self, websocket_uri: str, max_retries: int = 3) -> None:
        self.websocket: Optional[ws_client.WebSocketClientProtocol] = None
        self.connected: asyncio.Event = asyncio.Event()
        self.websocket_uri: str = websocket_uri
        self.retries: int = 0
        self.max_retries: int = max_retries
        self.last_display_data: Optional[str] = None
        self._was_connected: bool = False

    async def run(self) -> None:
        while self.retries < self.max_retries:
            try:
                if self.websocket is None:
                    logging.info("Connecting to MobiFlight at %s", self.websocket_uri)
                    self.websocket = await ws_client.connect(self.websocket_uri, ping_interval=None)
                    logging.info("MobiFlight connected")
                    self.connected.set()
                    
                    # If we were previously connected and have last display data, resend it
                    if self._was_connected and self.last_display_data:
                        logging.info("Resending last display data after reconnection")
                        await self.send(self.last_display_data)
                    
                    self._was_connected = True
                    self.retries = 0  # Reset retries on successful connection
                
                await self.websocket.recv()
            except Exception as e: 
                self.retries += 1
                logging.error(f"WebSocket error: {e} with retries {self.retries}")
                self.websocket = None
                self.connected.clear()
            await asyncio.sleep(5)
        logging.error("Max retries reached. Giving up connecting to MobiFlight at %s", self.websocket_uri)
        self.connected.set()

    async def send(self, data: str) -> None:
        if self.websocket and self.connected.is_set():
            await self.websocket.send(data)
            self.last_display_data = data

    async def close(self) -> None:
        if self.websocket:
            await self.websocket.close()
            self.websocket = None
            self.connected.clear()
            self._was_connected = False

def create_mobi_json(data: bytes) -> str:
    message: Dict[str, Union[str, List[List[Union[str, int]]], Dict[str, bool]]] = {
        "Target": "Display",
        "Data": [[] for _ in range(MCDU_CHARS)]
    }
    
    # We know exactly how many characters we should have - it's MCDU_CHARS
    # The data includes MCDU_CHARS number of MCDUChar structures plus 4 bools at the end
    char_size = ctypes.sizeof(MCDUChar)
    
    if len(data) < MCDU_DATA_SIZE:
        logging.error(f"Received data size {len(data)} is smaller than expected {MCDU_DATA_SIZE}")
        return json.dumps(message)
    

    # Now get the character array that follows the status lights
    char_data_start = ctypes.sizeof(MCDUStatus)
    char_size = ctypes.sizeof(MCDUChar)
    mcdu_chars = (MCDUChar * MCDU_CHARS).from_buffer_copy(data[char_data_start:char_data_start + (MCDU_CHARS * char_size)])
    
    # Process each character - note we're using row-major order here since that's how the display expects it
    for y in range(MCDU_ROWS):
        for x in range(MCDU_COLUMNS):
            # Convert from row-major to array index
            src_idx = y * MCDU_COLUMNS + x
            dst_idx = src_idx  # In this case, they're the same since we want row-major output
            
            try:
                char = mcdu_chars[src_idx]
                # Convert char16_t to Python string
                symbol = chr(char.value)
                
                if symbol == ' ' or symbol == '\0':
                    message["Data"][dst_idx] = []
                else:
                    message["Data"][dst_idx] = [
                        symbol,
                        "g",  # green color
                        0 if char.large else 1  # small font if not large
                    ]
            except (ValueError, TypeError, IndexError) as e:
                message["Data"][dst_idx] = []
                logging.debug(f"Error processing cell at ({x}, {y}): {e}")
    
    return json.dumps(message)

class MD11CDUClient:
    def __init__(self, sc_mobiflight: SimConnectMobiFlight, websocket_uri: str, cdu_definition: int) -> None:
        self.sc_mobiflight: SimConnectMobiFlight = sc_mobiflight
        self.mobiflight: MobiFlightClient = MobiFlightClient(websocket_uri)
        self.event_loop: Optional[asyncio.AbstractEventLoop] = None
        self.cdu_definition: int = cdu_definition
        self.last_data: Optional[bytes] = None

    def failed_to_connect(self) -> bool:
        return self.mobiflight.retries >= self.mobiflight.max_retries

    def setup_simconnect(self) -> bool:
        try:
            # Map the MD11 MCDU data area
            self.sc_mobiflight.dll.MapClientDataNameToID(
                self.sc_mobiflight.hSimConnect, 
                MD11_MCDU_NAME.encode(), 
                MD11_MCDU_CLIENT_DATA_ID
            )

            # Use MCDU_DATA_SIZE for data definition and offset calculation
            offset = MCDU_DATA_SIZE * self.cdu_definition
            
            self.sc_mobiflight.dll.AddToClientDataDefinition(
                self.sc_mobiflight.hSimConnect,
                self.cdu_definition,
                offset,
                MCDU_DATA_SIZE,
                0,
                0
            )

            # Request data updates
            self.sc_mobiflight.dll.RequestClientData(
                self.sc_mobiflight.hSimConnect,
                MD11_MCDU_CLIENT_DATA_ID,
                0,
                self.cdu_definition,
                Enum.SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_VISUAL_FRAME,
                Enum.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG_CHANGED,
                0, 0, 0
            )

            # Set up the handler
            self.sc_mobiflight.register_client_data_handler(self.handle_cdu_data)
            logging.info("SimConnect initialized for MD11 MCDU")
            return True
        except Exception as e:
            logging.error(f"SimConnect setup failed: {e}")
            return False

    def handle_cdu_data(self, client_data: Any) -> None:
        try:
            if client_data.dwDefineID == self.cdu_definition and hasattr(client_data, 'dwData'):
                data: bytes = bytes(client_data.dwData)
                # Only send if data has changed
                if data != self.last_data:
                    self.last_data = data
                    json_data = create_mobi_json(data)
                    asyncio.run_coroutine_threadsafe(self.mobiflight.send(json_data), self.event_loop)
        except Exception as e:
            logging.error(f"Error handling MCDU data: {e}")

    async def process_simconnect(self) -> None:
        while True:
            try:
                self.sc_mobiflight.my_dispatch_proc()
            except:
                pass
            await asyncio.sleep(0.1)

    async def run(self) -> None:
        self.event_loop = asyncio.get_running_loop()
        logging.info("Starting MCDU client")
        
        try:
            # Start MobiFlight connection
            mobiflight_task: asyncio.Task = asyncio.create_task(self.mobiflight.run())
            await self.mobiflight.connected.wait()
            if self.failed_to_connect():
                logging.error("Failed to connect to MobiFlight")
                return
            
            # Initialize SimConnect
            if self.setup_simconnect():
                simconnect_task: asyncio.Task = asyncio.create_task(self.process_simconnect())
                await asyncio.gather(mobiflight_task, simconnect_task)
            else:
                logging.error("Failed to start - SimConnect initialization failed")
        except KeyboardInterrupt:
            logging.info("Shutting down")
        except Exception as e:
            logging.error(f"Error: {e}")
        finally:
            await self.mobiflight.close()

if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s - %(levelname)s - %(message)s'
    )
    
    sc_mobiflight: SimConnectMobiFlight = SimConnectMobiFlight()
    
    # Create clients for all three MCDUs
    left_mcdu: MD11CDUClient = MD11CDUClient(sc_mobiflight, CAPTAIN_CDU_URL, MD11_MCDU_LEFT_DEFINITION)
    center_mcdu: MD11CDUClient = MD11CDUClient(sc_mobiflight, CENTER_CDU_URL, MD11_MCDU_CENTER_DEFINITION)
    right_mcdu: MD11CDUClient = MD11CDUClient(sc_mobiflight, CO_PILOT_CDU_URL, MD11_MCDU_RIGHT_DEFINITION)
    
    async def run_clients():
        await asyncio.gather(
            left_mcdu.run(),
            center_mcdu.run(),
            right_mcdu.run(),
            return_exceptions=True
        )

    try:
        asyncio.run(run_clients())
    except KeyboardInterrupt:
        logging.info("Shutting down")
    except Exception as e:
        logging.error(f"Error: {e}")
    finally:
        sc_mobiflight.exit() 