import copy
from ctypes import wintypes
import ctypes
import json
import logging
import asyncio
import os
import struct
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
OBSERVER_CDU_URL: str = "ws://localhost:8320/winwing/cdu-observer"

# Constants from PMDG_NG3_SDK.h
CDU_COLUMNS: int = 24
CDU_ROWS: int = 14
CDU_CELLS: int = CDU_COLUMNS * CDU_ROWS
CDU_CELL_BYTE_COUNT: int = 3

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

PMDG_CDU_2_NAME: str = "PMDG_777X_CDU_2"
PMDG_CDU_2_ID: int = 0x4E477837
PMDG_CDU_2_DEFINITION: int = 0x4E47783A


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
                    fontName: str = "Boeing"
                    await self.websocket.send(f'{{ "Target": "Font", "Data": "{fontName}" }}')
                    logging.info(f"Setting font: {fontName}")
                    self.connected.set()
                await self.websocket.recv()
            except Exception as e: 
                self.retries += 1
                logging.info(f"WebSocket error: {e} with retries {self.retries}")
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
    
    # Process data in column-major order as received from PMDG
    for x in range(CDU_COLUMNS):
        for y in range(CDU_ROWS):
            src_idx: int = (x * CDU_ROWS + y) * CDU_CELL_BYTE_COUNT
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

class PMDGConfiguration:
    config_name = "777_Options.ini"
    directories = [
        "pmdg-aircraft-77w",
        "pmdg-aircraft-77f"
    ]

    def verify_sdk_config(self):
        """Verify and potentially update the SDK configuration in the options file."""
        # Determine the correct path based on MS Store or Steam installation
        base_path = None
        ms_store_path = os.path.join(
            os.environ.get("LOCALAPPDATA", ""),
            "Packages",
            "Microsoft.FlightSimulator_8wekyb3d8bbwe",
            "LocalState",
            "packages",
        )

        steam_path = os.path.join(
            os.environ.get("APPDATA", ""), "Microsoft Flight Simulator", "Packages"
        )

        paths = [ms_store_path, steam_path]

        for directory in self.directories:
            for path in paths:
                if os.path.exists(path):
                    base_path = os.path.join(path, directory, "work")
                    if os.path.exists(base_path):
                        self.process_config(base_path)


    def process_config(self, base_path: str):
        logging.info(f"Processing config for {base_path}")

        options_path = os.path.join(base_path,  self.config_name)

        # Check if options file exists
        if not os.path.exists(options_path):
            logging.warning(f"Options file not found: {options_path}")
            return

        # Check if SDK configuration is present
        config = self.parse_ini_file(options_path)

        original_config = copy.deepcopy(config)


        if 'SDK' not in config:
            config['SDK'] = {}
        sdk = config["SDK"]
        sdk['EnableDataBroadcast'] = 1
        sdk['EnableCDUBroadcast.0'] = 1
        sdk['EnableCDUBroadcast.1'] = 1
        sdk['EnableCDUBroadcast.2'] = 1

        if original_config != config:
            logging.info("Updating SDK configuration")
            self.write_ini_file(config, options_path)
        else:
            logging.info("No changes to SDK configuration needed")

    def parse_ini_file(self, file_path):
        config = {}
        current_section = None
        
        with open(file_path, 'r') as file:
            for line in file:
                # Remove leading/trailing whitespace
                line = line.strip()
                
                # Skip empty lines
                if not line:
                    continue
                    
                # Check if line is a section header
                if line.startswith('[') and line.endswith(']'):
                    current_section = line[1:-1]  # Remove brackets
                    config[current_section] = {}
                    continue
                    
                # Skip if no section has been defined yet
                if current_section is None:
                    continue
                    
                # Parse key-value pairs
                if '=' in line:
                    key, value = line.split('=', 1)  # Split on first '=' only
                    key = key.strip()
                    value = value.strip()
                    
                    # Convert value to appropriate type
                    try:
                        if value.isdigit():
                            value = int(value)
                        elif value.replace('.', '').isdigit() and value.count('.') == 1:
                            value = float(value)
                    except ValueError:
                        pass  # Keep as string if conversion fails
                        
                    config[current_section][key] = value

        return config

    def write_ini_file(self, config, file_path):
        with open(file_path, 'w') as file:
            for section, settings in config.items():
                file.write(f"[{section}]\n")
                for key, value in settings.items():
                    file.write(f"{key}={value}\n")
                file.write("\n")  # Add blank line between sections

if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s - %(levelname)s - %(message)s'
    )

    ini_configurator = PMDGConfiguration()
    ini_configurator.verify_sdk_config()
    
    sc_mobiflight: SimConnectMobiFlight = SimConnectMobiFlight()
    captain_client: PMDGCDUClient = PMDGCDUClient(sc_mobiflight, CAPTAIN_CDU_URL, PMDG_CDU_0_NAME, PMDG_CDU_0_ID, PMDG_CDU_0_DEFINITION)
    co_pilot_client: PMDGCDUClient = PMDGCDUClient(sc_mobiflight, CO_PILOT_CDU_URL, PMDG_CDU_1_NAME, PMDG_CDU_1_ID, PMDG_CDU_1_DEFINITION)
    observer_client: PMDGCDUClient = PMDGCDUClient(sc_mobiflight, OBSERVER_CDU_URL, PMDG_CDU_2_NAME, PMDG_CDU_2_ID, PMDG_CDU_2_DEFINITION)

    async def run_clients():
        await asyncio.gather(
            captain_client.run(), 
            co_pilot_client.run(),
            observer_client.run(),
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
