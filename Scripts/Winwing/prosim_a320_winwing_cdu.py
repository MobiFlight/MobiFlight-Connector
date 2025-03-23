import clr
from pathlib import Path
from typing import Callable, Optional
import json
import logging
import asyncio
import websockets.asyncio.client as ws_client
import xml.etree.ElementTree as ET

# Connection settings for ProSim SDK
host = "localhost"
port = 8082
url = f"net.tcp://{host}:{port}/SDK"

subs = {'#': '\u2610',    # ballot box
        '¤': '\u2191',    # up arrow
        '¥': '\u2193',    # down arrow
        '¢': '\u2192',    # right arrow
        '£': '\u2190',    # left arrow
        '&': '\u0394',}   # greek delta for overfly
        
replace_chars =  ['£', '¢', '¥', '¤', '#', '&' ]
format_chars = ['s', 'l', 'a', 'c', 'y', 'w', 'g', 'm']
# URLs
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
CO_PILOT_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"

# Constants from PMDG_NG3_SDK.h
CDU_COLUMNS: int = 24
CDU_ROWS: int = 14

class PyProsimDLLException(Exception):
    pass


class PyProsimImportException(Exception):
    pass


class PyProsim:
    def __init__(
        self,
        prosimsdk_path: Path,
        on_connect_callback: Callable = None,
        on_disconnect_callback: Callable = None,
    ):
        """Py prosim class init
        Attributes:
        prosimsdk_path -- Path to prosim SDK DLL library
        on_connect_callback -- Callable object which will be called once we
                               are connected to prosim
        on_disconnect_callback -- Callable object which will be called once
                                  prosim disconnects
        """
        # Load CLR namespace
        try:
            clr.AddReference(str(prosimsdk_path))
        except Exception as e:
            raise PyProsimDLLException(e)

        # Finally import the required classes
        # Note the global definition is to make DataRef import available
        # to the rest of this implementation
        try:
            global DataRef
            from ProSimSDK import ProSimConnect, DataRef
        except Exception as e:
            raise PyProsimImportException(e)

        # Create Prosim SDK class
        self.sdk = ProSimConnect()

        # Set callbacks if required
        if on_connect_callback is not None:
            self.sdk.onConnect += on_connect_callback
        if on_disconnect_callback is not None:
            self.sdk.onDisconnect += on_disconnect_callback

        # Init main dictionary which will hold Prosim datarefs objects
        self.datarefs = {}

    def connect(self, ip_addr: str, synchronous: bool = True) -> None:
        """Simply connect to Prosim
        Attributes:
        ip_addr -- Host IP address when ProSim is running
        synchronous -- Blocking when True

        Return:
        None
        """
        self.sdk.Connect(ip_addr, synchronous)

    def add_dataref(
        self, dataref_name: str, interval: int, on_change_callback: Callable = None
    ) -> None:
        """Add a dataref request to ProSim. Then prosim can periodically send
        this value back.
        This dataref object is then accesible through this class datarfes dictionary,
        or getter method.
        Attributes:
        dataref_name -- Prosim dataref name
        interval -- How frequent prosim should send this dataref in miliseconds
        on_change_callback -- Callable object which will be called when dataref
                              changes.
        Return:
        None
        """
        # Create Prosim dataref object
        dr = DataRef(dataref_name, interval, self.sdk)

        # Set callback on change if needed
        if on_change_callback is not None:
            dr.onDataChange += on_change_callback

        # Store dataref object
        self.datarefs[dataref_name] = dr

    def get_dataref_value(self, dataref_name: str):
        """Get dataref value.
        Attributes:
        dataref_name -- Prosim dataref name
        Return:
        Prosim dataref value or None if dataref has not been
        added yet.
        """
        dataref = self.datarefs.get(dataref_name, None)
        if dataref is None:
            logging.warning(f"Dataref '{dataref_name}' not found in the dictionary, make sure you added it first")
            return None
        return dataref.value

    def get_all_avail_datarefs(self) -> list:
        """Request the full list of avaiable datarefs from ProSim
        Attributes:
        None
        Return:
        list -- List of dictionaries containing all info about the
                datarefs
        """
        dataref_list = []
        for dr in self.sdk.getDataRefDescriptions():
            dataref_list.append(
                {
                    "name": dr.Name,
                    "description": dr.Description,
                    "can_read": dr.CanRead,
                    "can_write": dr.CanWrite,
                    "data_type": dr.DataType,
                    "date_unit": dr.DataUnit,
                }
            )
        return dataref_list

    def get_info(self) -> dict:
        """Get simulator information
        Attributes:
        None
        Return:
        dict -- Dictionary with sim information
        """
        info = self.sdk.getLicensingInfo()
        features = []
        for f in info.Features:
            features.append(str(f))
        return {
            "mode": info.Mode,
            "features": features,
            "licensee": info.Licensee,
        }

    def set_dataref_value(self, dataref_name:str, dataref_value) -> None:
        """Set dataref value ** Only works if it's been added previously
        Attributes:
        dataref_name -- Prosim dataref name
        dataref_value -- New value for Prosim dataref
        Return:
        None
        """
        dataref = self.datarefs.get(dataref_name)
        if dataref is None:
            logging.error(f"Cannot set value for '{dataref_name}': dataref not found. Add it first with add_dataref().")
            raise Exception(f"Value '{dataref_name}' has not been added prior to setting it")
        
        dataref.value = dataref_value


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

def create_mobi_json(xml_string):
    message =  {}
    message["Target"] = "Display"
    message["Data"] = []    
    formatting = 'w'
    root = ET.fromstring(xml_string)
    for child in root:     
        size = 0 # default row start with size large  
        formatting = 'w' # default row start is white

        if not child.text:
            # empty row
            for _ in range(CDU_COLUMNS):
                message["Data"].append([])
            continue

        for char in child.text:
            entry = []
            if char in format_chars:
                if char == 's':
                    size = 1
                elif char == 'l':
                    size = 0
                else:
                    formatting = char
            elif char in replace_chars:
                    entry = [subs[char], formatting, size]
                    message["Data"].append(entry)
            else:
                if char != ' ':
                    entry = [char, formatting, size]
                message["Data"].append(entry)
        logging.debug(child.text)   
    logging.debug(message)
    return json.dumps(message, separators=(',', ':')) 

class ProSimClient:
    pyprosim: PyProsim = None

    def __init__(self) -> None:
        try:
            self.pyprosim = PyProsim(Path("ProSimSDK.dll").absolute())
            self.connected = False
        except Exception as e:
            logging.error(f"Failed to initialize ProSim SDK: {e}")
            raise

    def connect(self) -> bool:
        try:
            logging.info(f"Connecting to ProSim at {host}")
            self.pyprosim.connect(host, synchronous=True)
            self.connected = True
            logging.info("Successfully connected to ProSim")
            return True
        except Exception as e:
            logging.error(f"Failed to connect to ProSim: {e}")
            self.connected = False
            return False

    def add_callback(self, dataref_name: str, callback: Callable) -> bool:
        try:
            self.pyprosim.add_dataref(dataref_name, 50, callback)
            return True
        except Exception as e:
            logging.error(f"Failed to add callback for {dataref_name}: {e}")
            return False

    def get_dataref_value(self, dataref_name: str):
        try:
            return self.pyprosim.get_dataref_value(dataref_name)
        except Exception as e:
            logging.error(f"Error getting dataref value for {dataref_name}: {e}")
            return None


class ProSimCDUClient:
    def __init__(self, prosim_client: ProSimClient, websocket_uri: str, cdu_name: str, cdu_dataref_name: str) -> None:
        self.prosim_client: ProSimClient = prosim_client
        self.mobiflight: MobiFlightClient = MobiFlightClient(websocket_uri)
        self.event_loop: Optional[asyncio.AbstractEventLoop] = None
        self.cdu_name: str = cdu_name
        self.cdu_dataref_name: str = cdu_dataref_name

    def failed_to_connect(self) -> bool:
        return self.mobiflight.retries >= self.mobiflight.max_retries

    def setup_prosim(self) -> bool:
        try:
            self.prosim_client.connect()
                    
            # Check that we can access datarefs
            datarefs = self.prosim_client.pyprosim.get_all_avail_datarefs()
            if not datarefs:
                logging.error(f"No datarefs found in ProSim for {self.cdu_name}")
                return False
                
            self.prosim_client.add_callback(self.cdu_dataref_name, None)

            logging.info(f"ProSim connection initialized for {self.cdu_name}")
            return True
        except Exception as e:
            logging.error(f"ProSim setup failed for {self.cdu_name}: {e}")
            return False

    async def process_prosim(self) -> None:
        while True:
            dataref = self.prosim_client.get_dataref_value(self.cdu_dataref_name)
            if dataref:
                asyncio.run_coroutine_threadsafe(self.mobiflight.send(create_mobi_json(dataref)), self.event_loop)

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
            if self.setup_prosim():
                simconnect_task: asyncio.Task = asyncio.create_task(self.process_prosim())
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
    
    sc_prosim: ProSimClient = ProSimClient()
    captain_client: ProSimCDUClient = ProSimCDUClient(sc_prosim, CAPTAIN_CDU_URL, "PILOT", "aircraft.mcdu1.display")
    co_pilot_client: ProSimCDUClient = ProSimCDUClient(sc_prosim, CO_PILOT_CDU_URL, "COPILOT", "aircraft.mcdu2.display")
    
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
