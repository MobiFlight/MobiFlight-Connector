from pathlib import Path
from typing import Callable, Optional
import json
import logging
import asyncio
import websockets
import xml.etree.ElementTree as ET
from gql import Client, gql
from gql.transport.websockets import WebsocketsTransport
from gql.transport.websockets import log as websockets_logger

# Connection settings for ProSim GraphQL
GRAPHQL_URL = "ws://localhost:5000/graphql"

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

class MobiFlightClient:
    def __init__(self, websocket_uri: str, max_retries: int = 3) -> None:
        self.websocket: Optional[websockets.WebSocketClientProtocol] = None
        self.connected: asyncio.Event = asyncio.Event()
        self.websocket_uri: str = websocket_uri
        self.retries: int = 0
        self.max_retries: int = max_retries

    async def run(self) -> None:
        while self.retries < self.max_retries:
            try:
                if self.websocket is None:
                    logging.info("Connecting to MobiFlight at %s", self.websocket_uri)
                    self.websocket = await websockets.connect(self.websocket_uri, ping_interval=None)
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
    return json.dumps(message, separators=(',', ':'))

class ProSimGraphQLClient:
    """
    Client for handling GraphQL communication with ProSim
    """
    def __init__(self) -> None:
        self.transport = WebsocketsTransport(url=GRAPHQL_URL)
        self.client = Client(transport=self.transport)
        self.session = None
        self.connected = False
        self._callback_tasks = set()  # Keep track of callback tasks

    async def connect(self) -> bool:
        """
        Connect to ProSim GraphQL server
        
        Returns:
            bool: True if connection was successful, False otherwise
        """
        try:
            logging.info(f"Connecting to ProSim GraphQL at {GRAPHQL_URL}")
            self.session = await self.client.connect_async(reconnecting=True)
            self.connected = True
            logging.info("Successfully connected to ProSim GraphQL")
            return True
        except Exception as e:
            logging.error(f"Failed to connect to ProSim GraphQL: {e}")
            self.connected = False
            return False

    async def disconnect(self) -> bool:
        """
        Disconnect from ProSim GraphQL server
        
        Returns:
            bool: True if disconnect was successful, False otherwise
        """
        try:
            if self.session:
                await self.session.close()
            self.connected = False
            return True
        except Exception as e:
            logging.error(f"Error disconnecting from ProSim GraphQL: {e}")
            return False

    async def subscribe_to_datarefs(self, dataref_names: list[str], callback: Callable[[str, str], None]) -> None:
        """
        Subscribe to ProSim datarefs using GraphQL subscription
        
        Args:
            dataref_names: List of dataref names to subscribe to
            callback: Async callback function to handle dataref updates
        """
        if not self.connected:
            logging.error("Not connected to ProSim GraphQL")
            return

        subscription = gql(
            """
            subscription OnDataRefChanged($names: [String!]!) {
                dataRefs(names: $names) {
                    name
                    value
                }
            }
            """
        )
        params = {"names": dataref_names}

        try:
            async for result in self.session.subscribe(subscription, params, "OnDataRefChanged"):
                if "dataRefs" in result:
                    # Create a task for the callback to handle it asynchronously
                    task = asyncio.create_task(callback(result["dataRefs"]["name"], result["dataRefs"]["value"]))
                    # Add task to the set of tracked tasks
                    self._callback_tasks.add(task)
                    # Remove task from set when done
                    task.add_done_callback(self._callback_tasks.discard)
        except Exception as e:
            logging.error(f"Error in GraphQL subscription: {e}")
            self.connected = False

class ProSimCDUClient:
    def __init__(self, prosim_client: ProSimGraphQLClient, websocket_uri: str, cdu_name: str, cdu_dataref_name: str) -> None:
        self.prosim_client: ProSimGraphQLClient = prosim_client
        self.mobiflight: MobiFlightClient = MobiFlightClient(websocket_uri)
        self.event_loop: Optional[asyncio.AbstractEventLoop] = None
        self.cdu_name: str = cdu_name
        self.cdu_dataref_name: str = cdu_dataref_name
        self.last_cdu_data = None
        self._callback_tasks = set()  # Keep track of callback tasks

    def failed_to_connect(self) -> bool:
        return self.mobiflight.retries >= self.mobiflight.max_retries

    async def setup_prosim(self) -> bool:
        try:
            if not self.prosim_client.connected and not await self.prosim_client.connect():
                logging.error(f"Failed to connect to ProSim GraphQL for {self.cdu_name}")
                return False

            logging.info(f"ProSim GraphQL connection initialized for {self.cdu_name} CDU")
            return True
        except Exception as e:
            logging.error(f"ProSim GraphQL setup failed for {self.cdu_name}: {e}")
            return False

    async def handle_dataref_update(self, dataref_name: str, value: str) -> None:
        """
        Handle dataref updates from ProSim GraphQL
        
        Args:
            dataref_name: Name of the dataref that was updated
            value: New value of the dataref
        """
        if dataref_name == self.cdu_dataref_name and value != self.last_cdu_data:
            try:
                json_data = create_mobi_json(value)
                await self.mobiflight.send(json_data)
                self.last_cdu_data = value
            except Exception as e:
                logging.error(f"Error processing CDU data for {self.cdu_name}: {e}")

    async def run(self) -> None:
        self.event_loop = asyncio.get_running_loop()
        logging.info(f"Starting {self.cdu_name} CDU client")
        
        try:
            # Start MobiFlight connection
            mobiflight_task: asyncio.Task = asyncio.create_task(self.mobiflight.run())
            await self.mobiflight.connected.wait()
            
            if self.failed_to_connect():
                logging.error(f"Failed to connect to MobiFlight for {self.cdu_name}")
                return
                
            # Initialize ProSim GraphQL connection
            if await self.setup_prosim():
                # Start processing ProSim data
                await self.prosim_client.subscribe_to_datarefs(
                    [self.cdu_dataref_name],
                    self.handle_dataref_update
                )
            else:
                logging.error(f"Failed to start {self.cdu_name} - ProSim GraphQL initialization failed")
                
        except asyncio.CancelledError:
            logging.info(f"Shutting down {self.cdu_name} CDU client")
        except Exception as e:
            logging.error(f"Error in {self.cdu_name} CDU client: {e}")
        finally:
            # Cancel any pending callback tasks
            for task in self._callback_tasks:
                task.cancel()
            self._callback_tasks.clear()
            await self.mobiflight.close()

if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s - %(levelname)s - %(message)s'
    )
    
    async def cleanup(prosim_client):
        """Clean up resources"""
        logging.info("Cleaning up resources")
        try:
            if prosim_client:
                await prosim_client.disconnect()
        except Exception as e:
            logging.error(f"Error during cleanup: {e}")
    
    try:
        # Initialize the ProSim GraphQL client
        logging.info("Initializing ProSim GraphQL client")
        prosim_client = ProSimGraphQLClient()
        
        # Create the CDU clients
        logging.info("Creating CDU clients")
        captain_client = ProSimCDUClient(prosim_client, CAPTAIN_CDU_URL, "PILOT", "aircraft.mcdu1.display")
        co_pilot_client = ProSimCDUClient(prosim_client, CO_PILOT_CDU_URL, "COPILOT", "aircraft.mcdu2.display")
        
        # Run the clients
        async def run_clients():
            # Connect to ProSim first
            if not await prosim_client.connect():
                logging.error("Failed to connect to ProSim GraphQL. Please check if ProSim is running.")
                return
                
            logging.info("Starting CDU clients")
            await asyncio.gather(
                captain_client.run(), 
                co_pilot_client.run(),
                return_exceptions=True
            )
            
        # Run the async loop
        asyncio.run(run_clients())
        
    except KeyboardInterrupt:
        logging.info("Shutting down due to keyboard interrupt")
    except Exception as e:
        logging.error(f"Error in main: {e}")
        import traceback
        logging.error(traceback.format_exc())
    finally:
        # Clean up resources
        asyncio.run(cleanup(prosim_client if 'prosim_client' in locals() else None))
        
        logging.info("Application terminated")
