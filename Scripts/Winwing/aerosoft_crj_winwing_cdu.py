from ctypes import wintypes
import ctypes
import json
import struct
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


subs = {'@': '☐',    # ballot box \u2610
        'a': '↑',    # up arrow    \u2191
        'b': '↓',    # down arrow  \u2193
        'c': '→',    # left arrow  \u2190
        'd': '←',    # right arrow \u2192       
        'e': '°',}   

# URLs
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
CO_PILOT_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"

CDU_COLUMNS: int = 24
CDU_ROWS: int = 14
CDU_CELLS: int = CDU_COLUMNS * CDU_ROWS
CDU_CELL_BYTE_COUNT = 2

# CDU Color constants
CDU_COLOR_BLACK: int = 0
CDU_COLOR_WHITE: int = 1
CDU_COLOR_RED: int = 2
CDU_COLOR_GREEN: int = 3
CDU_COLOR_BLUE: int = 4
CDU_COLOR_CYAN: int = 5
CDU_COLOR_MAGENTA: int = 6
CDU_COLOR_YELLOW: int = 7

# CRJ CDU Client Data Area Names and IDs
CRJ_CDU_0_NAME: str = "ASCRJ CDU1 Data"
CRJ_CDU_0_CLIENT_DATA_ID: int = 0 # CLIENT_DATA_ID_MCDU
CRJ_CDU_1_NAME: str = "ASCRJ CDU2 Data"
CRJ_CDU_1_CLIENT_DATA_ID: int = 1

# CRJ CDU Definition IDs
CRJ_CDU_0_DEFINITION: int = 0 # CLIENT_DATA_DEFINE_ID_LCDU
CRJ_CDU_1_DEFINITION: int = 1 # CLIENT_DATA_DEFINE_ID_RCDU


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
                    self.websocket = await ws_client.connect(self.websocket_uri, ping_interval=None)
                    logging.info("MobiFlight connected at %s", self.websocket_uri)
                    # Load font                                        
                    fontName = "Collins"
                    await self.websocket.send(f'{{ "Target": "Font", "Data": "{fontName}" }}')
                    logging.info(f"Setting font: {fontName}")
                    self.connected.set()
                await self.websocket.recv()
            except Exception as e: 
                self.retries += 1
                logging.info(f"Failed to connect to {self.websocket_uri}: {e} with retries {self.retries}")
                self.websocket = None
                self.connected.clear()
            await asyncio.sleep(5)
        logging.info("Max retries reached. Giving up connecting to MobiFlight at %s. If you only have one CDU attached, you can ignore this message.", self.websocket_uri)
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

    # Process each character - note we're using row-major order here since that's how the display expects it
    for y in range(CDU_ROWS):
        for x in range(CDU_COLUMNS):
            # Convert from row-major to array index
            src_idx = (y * CDU_COLUMNS + x) * CDU_CELL_BYTE_COUNT
            dst_idx = y * CDU_COLUMNS + x 
            
            try:
                symbol: str = chr(data[src_idx])
                symbol = subs.get(symbol, symbol)
                format = data[src_idx + 1]                      
                color: int = format & 0b01111111
                     
                is_small: bool = (format & 0b10000000) == 128               
                # Heading lines should be small as well, except input line
                is_small = is_small or ((y % 2 == 1) and not (y == 13))    

                color_str = {
                    CDU_COLOR_BLACK: "e",  # use grey instead
                    CDU_COLOR_WHITE: "w",
                    CDU_COLOR_RED: "r",
                    CDU_COLOR_GREEN: "g",
                    CDU_COLOR_BLUE: "o", 
                    CDU_COLOR_CYAN: "o", # is shown as blue on CDU
                    CDU_COLOR_MAGENTA: "m",
                    CDU_COLOR_YELLOW: "y"
                }.get(color, "w")
                
                if symbol == ' ' or symbol == '\0':
                    message["Data"][dst_idx] = []
                else:
                    message["Data"][dst_idx] = [
                        symbol,
                        color_str,
                        1 if is_small else 0
                    ]
            except (ValueError, TypeError, IndexError) as e:
                message["Data"][dst_idx] = []
                logging.debug(f"Error processing cell at ({x}, {y}): {e}")

    return json.dumps(message)                


class CRJCDUClient:
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
                0, # offset to start
                CDU_COLUMNS * CDU_ROWS * CDU_CELL_BYTE_COUNT, # size client data in bytes
                0,
                0
            )

            # Request data updates
            self.sc_mobiflight.dll.RequestClientData(
                self.sc_mobiflight.hSimConnect,
                self.cdu_id,
                self.cdu_id,
                self.cdu_definition,
                Enum.SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_VISUAL_FRAME,
                Enum.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG_CHANGED,
                0, 0, 0
            )

            # Set up the handler
            self.sc_mobiflight.register_client_data_handler(self.handle_cdu_data)
            logging.info("SimConnect initialized for %s", self.cdu_name)
            return True
        except Exception as e:
            logging.error(f"SimConnect setup failed for {self.cdu_name}: {e}")
            return False
    

    def handle_cdu_data(self, client_data: Any) -> None:
        try:
            if client_data.dwDefineID == self.cdu_definition and hasattr(client_data, 'dwData'):                
                int_count : int = int(CDU_COLUMNS * CDU_ROWS * CDU_CELL_BYTE_COUNT / 4)              
                if len(client_data.dwData) >= int_count:
                    data_list : bytearray = bytearray()                  
                    for i in range(int_count): 
                        my_bytes : bytes = struct.pack("I", client_data.dwData[i])
                        data_list.extend(my_bytes)                
                    data: bytes = bytes(data_list)                                       
                    asyncio.run_coroutine_threadsafe(self.mobiflight.send(create_mobi_json(data)), self.event_loop)
        except Exception as e:
            logging.error(f"Error handling CDU data: {e}")


    async def run(self) -> None:
        self.event_loop = asyncio.get_running_loop()
        logging.info("Starting CDU client")
        
        try:
            # Start MobiFlight connection
            mobiflight_task: asyncio.Task = asyncio.create_task(self.mobiflight.run())
            await self.mobiflight.connected.wait()
            if self.failed_to_connect():
                logging.info("Failed to connect to MobiFlight for %s", self.cdu_name)
                return

            # Initialize SimConnect
            if self.setup_simconnect():               
                await asyncio.gather(mobiflight_task)
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
    captain_client: CRJCDUClient = CRJCDUClient(sc_mobiflight, CAPTAIN_CDU_URL, CRJ_CDU_0_NAME, CRJ_CDU_0_CLIENT_DATA_ID, CRJ_CDU_0_DEFINITION)
    co_pilot_client: CRJCDUClient = CRJCDUClient(sc_mobiflight, CO_PILOT_CDU_URL, CRJ_CDU_1_NAME, CRJ_CDU_1_CLIENT_DATA_ID, CRJ_CDU_1_DEFINITION)
    
    async def run_clients():
        await asyncio.gather(
            captain_client.run(), 
            co_pilot_client.run(),
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
