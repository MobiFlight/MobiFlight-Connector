from ctypes import wintypes
import ctypes
import json
import logging
import asyncio
import websockets.asyncio.client as ws_client
from typing import Optional, List, Dict, Union, Any
from SimConnect import SimConnect, Enum
from SimConnect.Enum import SIMCONNECT_CLIENT_DATA_ID, SIMCONNECT_RECV_ID, SIMCONNECT_RECV_CLIENT_DATA


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
# URLs
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
CO_PILOT_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"

# Constants from PMDG_NG3_SDK.h
CDU_COLUMNS: int = 24
CDU_ROWS: int = 14
CDU_CELLS: int = CDU_COLUMNS * CDU_ROWS

# CDU Color constants
CDU_COLOR_WHITE: int = 0
CDU_COLOR_CYAN: int = 1
CDU_COLOR_GREEN: int = 2
CDU_COLOR_MAGENTA: int = 3
CDU_COLOR_AMBER: int = 4
CDU_COLOR_RED: int = 5

# CDU Flag constants
CDU_FLAG_SMALL_FONT: int = 0x01
CDU_FLAG_REVERSE: int = 0x02
CDU_FLAG_UNUSED: int = 0x04

# PMDG CDU Client Data Area Names and IDs
PMDG_CDU_0_NAME: str = "PMDG_777X_CDU_0"
PMDG_CDU_0_ID: int = 0x4E477835
PMDG_CDU_0_DEFINITION: int = 0x4E477838

PMDG_CDU_1_NAME: str = "PMDG_777X_CDU_1"
PMDG_CDU_1_ID: int = 0x4E477836
PMDG_CDU_1_DEFINITION: int = 0x4E477839


class MobiFlightClient:
    def __init__(self, websocket_uri: str, max_retries: int = 3) -> None:
        self.websocket: Optional[ws_client.WebSocketClientProtocol] = None
        self.connected: asyncio.Event = asyncio.Event()
        self.websocket_uri: str = websocket_uri
        self.retries: int = 0
        self.max_retries: int = max_retries

    async def run(self) -> None:
        while self.retries < self.max_retries:
            try:
                if self.websocket is None:
                    logging.info("Connecting to MobiFlight at %s", self.websocket_uri)
                    self.websocket = await ws_client.connect(self.websocket_uri, ping_interval=None)
                    logging.info("MobiFlight connected")
                    self.connected.set()
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

    async def close(self) -> None:
        if self.websocket:
            await self.websocket.close()
            self.websocket = None
            self.connected.clear()

def create_mobi_json(data: bytes) -> str:
    message: Dict[str, Union[str, List[List[Union[str, int]]]]] = {
        "Target": "Display",
        "Data": [[] for _ in range(CDU_CELLS)]
    }
    
    # Process data in column-major order as received from PMDG
    for x in range(CDU_COLUMNS):
        for y in range(CDU_ROWS):
            src_idx: int = (x * CDU_ROWS + y) * 3
            dst_idx: int = y * CDU_COLUMNS + x
            
            if src_idx + 2 >= len(data):
                message["Data"][dst_idx] = []
                continue
                
            try:
                symbol: str = chr(data[src_idx])
                color: int = data[src_idx + 1]
                flags: int = data[src_idx + 2]

                if symbol == ' ' or symbol == '\0':
                    message["Data"][dst_idx] = []
                else:
                    # Handle special characters
                    if symbol == '\xA1': symbol = "\u2190"  # left arrow
                    elif symbol == '\xA2': symbol = "\u2192"  # right arrow
                    elif symbol == '\xA3': symbol = "\u2191"  # up arrow
                    elif symbol == '\xA4': symbol = "\u2193"  # down arrow
                    elif symbol == '\u00EA': symbol = "\u2610"  # box
                    
                    # Handle color based on flags
                    if flags & CDU_FLAG_UNUSED:
                        color_str: str = "e"  # Gray for unused
                    elif flags & CDU_FLAG_REVERSE:
                        color_str = "e"  # Gray for reverse video
                    else:
                        color_str = {
                            CDU_COLOR_WHITE: "w",
                            CDU_COLOR_CYAN: "c",
                            CDU_COLOR_GREEN: "g",
                            CDU_COLOR_MAGENTA: "m",
                            CDU_COLOR_AMBER: "a",
                            CDU_COLOR_RED: "r"
                        }.get(color, "w")

                    message["Data"][dst_idx] = [
                        symbol,
                        color_str,
                        1 if (flags & CDU_FLAG_SMALL_FONT) else 0
                    ]
            except (ValueError, TypeError, IndexError) as e:
                message["Data"][dst_idx] = []
                logging.debug(f"Error processing cell: {e}")
    
    return json.dumps(message)

class PMDGCDUClient:
    def __init__(self, sc_mobiflight: SimConnectMobiFlight, websocket_uri: str, cdu_name: str, cdu_id: int, cdu_definition: int) -> None:
        self.sc_mobiflight: SimConnectMobiFlight = sc_mobiflight
        self.mobiflight: MobiFlightClient = MobiFlightClient(websocket_uri)
        self.event_loop: Optional[asyncio.AbstractEventLoop] = None
        self.cdu_definition: int = cdu_definition
        self.cdu_name: str = cdu_name
        self.cdu_id: int = cdu_id

    def failed_to_connect(self) -> bool:
        return self.mobiflight.retries >= self.mobiflight.max_retries

    def setup_simconnect(self) -> bool:
        try:
            # Map and define the CDU data area
            self.sc_mobiflight.dll.MapClientDataNameToID(
                self.sc_mobiflight.hSimConnect, 
                self.cdu_name.encode(), 
                self.cdu_id
            )

            self.sc_mobiflight.dll.AddToClientDataDefinition(
                self.sc_mobiflight.hSimConnect,
                self.cdu_definition,
                0,
                CDU_COLUMNS * CDU_ROWS * 3,
                0,
                0
            )

            # Request data updates
            self.sc_mobiflight.dll.RequestClientData(
                self.sc_mobiflight.hSimConnect,
                self.cdu_id,
                0,
                self.cdu_definition,
                Enum.SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_VISUAL_FRAME,
                Enum.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG_CHANGED,
                0, 0, 0
            )

            # Set up the handler
            self.sc_mobiflight.register_client_data_handler(self.handle_cdu_data)
            logging.info("SimConnect initialized")
            return True
        except Exception as e:
            logging.error(f"SimConnect setup failed: {e}")
            return False

    def handle_cdu_data(self, client_data: Any) -> None:
        try:
            if client_data.dwDefineID == self.cdu_definition and hasattr(client_data, 'dwData'):
                data: bytes = bytes(client_data.dwData)
                if len(data) >= CDU_COLUMNS * CDU_ROWS * 3:
                    asyncio.run_coroutine_threadsafe(self.mobiflight.send(create_mobi_json(data)), self.event_loop)
        except Exception as e:
            logging.error(f"Error handling CDU data: {e}")

    async def process_simconnect(self) -> None:
        while True:
            # try:
            #     self.scMobiflight.my_dispatch_proc()
            # except:
            #     pass
            await asyncio.sleep(0.1)

    async def run(self) -> None:
        self.event_loop = asyncio.get_running_loop()
        logging.info("Starting CDU client")
        
        try:
            # Start MobiFlight connection
            mobiflight_task: asyncio.Task = asyncio.create_task(self.mobiflight.run())
            await self.mobiflight.connected.wait()
            if self.failed_to_connect():
                logging.error("Failed to connect to MobiFlight for %s", self.cdu_name)
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
    captain_client: PMDGCDUClient = PMDGCDUClient(sc_mobiflight, CAPTAIN_CDU_URL, PMDG_CDU_0_NAME, PMDG_CDU_0_ID, PMDG_CDU_0_DEFINITION)
    co_pilot_client: PMDGCDUClient = PMDGCDUClient(sc_mobiflight, CO_PILOT_CDU_URL, PMDG_CDU_1_NAME, PMDG_CDU_1_ID, PMDG_CDU_1_DEFINITION)
    
    async def run_clients():
        await asyncio.gather(
            captain_client.run(), 
            co_pilot_client.run(),
            return_exceptions=True
        )
    # this will not work
    try:
        asyncio.run(run_clients())
    except KeyboardInterrupt:
        logging.info("Shutting down")
    except Exception as e:
        logging.error(f"Error: {e}")
    finally:
        sc_mobiflight.exit()
