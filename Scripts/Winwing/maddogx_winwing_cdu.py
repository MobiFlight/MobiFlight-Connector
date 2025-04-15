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

# CDU data struct as defined in MaddogX
# typedef struct _CDU_SC_DATA
# {
# 	unsigned char cdutype;              // 0
# 	unsigned char exec;                 // 1
# 	unsigned char menu;                 // 2
# 	unsigned char msg;                  // 3
# 	unsigned char offset;               // 4
# 	unsigned char display;              // 5
# 	unsigned char spare1;               // 6
# 	unsigned char spare2;               // 7
# 	unsigned char screen[CDU_CELLS];    // 8
# 	unsigned char atrb[CDU_CELLS];      // 344
# } CDU_SC_DATA, *PCDU_SC_DATA, **PPCDU_SC_DATA;
CDU_ROWS: int = 14
CDU_COLUMNS: int = 24
CDU_CELLS: int = CDU_COLUMNS * CDU_ROWS
CDU_TYPE_OFFSET: int = 0
CDU_DATA_OFFSET: int = 8
CDU_ATRB_OFFSET: int = CDU_DATA_OFFSET + CDU_CELLS
CDU_SC_DATA_SIZE: int = CDU_CELLS * 2 + CDU_DATA_OFFSET

# 0 = Honeywell, 1 = Canadian
CDU_TYPE_HW: int = 0
CDU_TYPE_CM: int = 1

# CDU Color constants
CDU_COLOR_WHITE: int = 7
CDU_COLOR_AMBER: int = 6
CDU_COLOR_CYAN: int = 5
CDU_COLOR_GREEN: int = 4
CDU_COLOR_MAGENTA: int = 3
CDU_COLOR_RED: int = 2
CDU_COLOR_BLUE: int = 1
CDU_COLOR_BLACK: int = 0

# CDU Flag constants
CDU_COLOR_MASK: int = 0x0f
CDU_FLAG_MASK: int = 0xf0
CDU_FLAG_SMALL_FONT: int = 0x80
CDU_FLAG_REVERSE: int = 0x10

# MDX CDU Client Data Area Names and IDs
MDX_CDU_0_NAME: str = "MaddogX CDU1 Data Atrb"
MDX_CDU_0_ID: int = 85
MDX_CDU_0_DEFINITION: int = 802
MDX_CDU_1_NAME: str = "MaddogX CDU2 Data Atrb"
MDX_CDU_1_ID: int = 86
MDX_CDU_1_DEFINITION: int = 802

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
                    logging.info("MobiFlight connected at %s", self.websocket_uri)
                    self.connected.set()
                await self.websocket.recv()
            except Exception as e: 
                self.retries += 1
                logging.info(f"WebSocket error: {e} with retries {self.retries}")
                self.websocket = None
                self.connected.clear()
            await asyncio.sleep(5)
        logging.info("Max retries reached. Giving up connecting to MobiFlight at %s", self.websocket_uri)
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
    
    # 0 = Honeywell, 1 = Canadian
    cdutype = data[CDU_TYPE_OFFSET]
    
    # Process data in row-major order as received from MaddogX
    for y in range(CDU_ROWS):
        for x in range(CDU_COLUMNS):
            src_idx: int = y * CDU_COLUMNS + x + CDU_DATA_OFFSET
            src_iax: int = y * CDU_COLUMNS + x + CDU_ATRB_OFFSET
            dst_idx: int = y * CDU_COLUMNS + x
            
            if src_idx >= CDU_ATRB_OFFSET or src_iax >= len(data):
                message["Data"][dst_idx] = []
                continue
                
            try:
                symbol: str = chr(data[src_idx])
                color: int = data[src_iax] & CDU_COLOR_MASK
                flags: int = data[src_iax] & CDU_FLAG_MASK

                if symbol == ' ' or symbol == '\0':
                    message["Data"][dst_idx] = []
                else:
                    # Handle special characters
                    if symbol == '{': symbol = "["
                    elif symbol == '}': symbol = "]"
                    elif symbol == '[': symbol = "\u2610"                   # box
                    elif cdutype == CDU_TYPE_HW and symbol == '$': symbol = "\u00B0"  # degrees
                    elif cdutype == CDU_TYPE_HW and symbol == '!': symbol = "\u2193"  # down arrow
                    
                    # Handle color
                    if cdutype == CDU_TYPE_HW:
                        color_str: str = "g" # always green
                    #elif flags & CDU_FLAG_REVERSE:
                    #    color_str = "e"  # Gray for reverse video
                    else:
                        color_str = {
                            CDU_COLOR_WHITE: "w",
                            CDU_COLOR_AMBER: "a",
                            CDU_COLOR_CYAN: "c",
                            CDU_COLOR_GREEN: "g",
                            CDU_COLOR_MAGENTA: "m",
                            CDU_COLOR_RED: "r",
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

class MDXCDUClient:
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
                CDU_SC_DATA_SIZE,
                0,
                0
            )

            # Request data updates
            self.sc_mobiflight.dll.RequestClientData(
                self.sc_mobiflight.hSimConnect,
                self.cdu_id,
                self.cdu_id,
                self.cdu_definition,
                Enum.SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_ON_SET,
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
                data: bytes = bytes(client_data.dwData)
                if len(data) >= CDU_SC_DATA_SIZE:
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
    captain_client: MDXCDUClient = MDXCDUClient(sc_mobiflight, CAPTAIN_CDU_URL, MDX_CDU_0_NAME, MDX_CDU_0_ID, MDX_CDU_0_DEFINITION)
    co_pilot_client: MDXCDUClient = MDXCDUClient(sc_mobiflight, CO_PILOT_CDU_URL, MDX_CDU_1_NAME, MDX_CDU_1_ID, MDX_CDU_1_DEFINITION)
    
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
