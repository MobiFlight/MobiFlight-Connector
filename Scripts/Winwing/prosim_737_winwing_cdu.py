from pathlib import Path
from typing import Callable, Optional
import json
import logging
import asyncio
import websockets
import xml.etree.ElementTree as ET
import re
from gql import Client, gql
from gql.transport.websockets import WebsocketsTransport
from gql.transport.websockets import log as websockets_logger

# Connection settings for ProSim GraphQL
GRAPHQL_URL = "ws://localhost:5000/graphql"

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

def add_empty_row(data_array):
    """Add an empty row of 24 columns to the data array"""
    for _ in range(CDU_COLUMNS):
        data_array.append([])

def pad_to_position(row_data, position):
    """Pad a row with empty entries until it reaches the specified position"""
    while len(row_data) < position:
        row_data.append([])

def add_row_to_message(row_data, data_array):
    """Add a row to the message data array, ensuring it's exactly 24 columns"""
    # Ensure row is exactly 24 columns
    pad_to_position(row_data, CDU_COLUMNS)
    # Add the row data (limit to first 24 elements)
    data_array.extend(row_data[:CDU_COLUMNS])

# Color code mapping
COLOR_CODES = {
    '1': 'c',  # Blue color
    '2': 'g',  # Green
    '3': 'm',  # Amber
    '4': 'y',  # Yellow
    '5': 'a',  # Magenta
    # Add more color codes as needed
}

# Default formatting values
DEFAULT_COLOR = 'w'  # White
DEFAULT_SIZE = 0     # Large text
DEFAULT_ALIGN = 'l'  # Left alignment

def get_visible_length(text):
    """
    Calculate the visible length of text after removing formatting tags
    
    Args:
        text: Text that may contain formatting tags
        
    Returns:
        int: The number of visible characters
    """
    # Remove format tags by pattern
    # Match [anything] including [/anything]
    pattern = r'\[[^\]]*\]'
    clean_text = re.sub(pattern, '', text)
    return len(clean_text)

def process_text_with_format(text, row_data, default_format, default_size, default_alignment=DEFAULT_ALIGN):
    """
    Process text with formatting tags using a fast direct approach
    
    Args:
        text: The text to process
        row_data: Array to append characters with formatting to
        default_format: The default color format to use ('w' for white)
        default_size: The default size to use (0 for large, 1 for small)
        default_alignment: The default alignment to use ('l' for left, 'm' for middle/center)
        
    Returns:
        The row_data array with formatted text elements added
    """
    # Create a new row data array to work with
    local_row_data = []
    
    # Current state
    color = default_format
    size = default_size
    alignment = default_alignment  # Track text alignment (l=left, m=middle/center)
    
    # Simple format stack for nested formats (color, size, alignment)
    format_stack = []
    
    i = 0

    while i < len(text):
        if text[i] == '[':
            # Move to the character after the opening bracket
            i += 1
            if i < len(text):
                if text[i] == '/':
                    # End tag - pop format from stack if available
                    if format_stack:
                        color, size, alignment = format_stack.pop()
                    else:
                        # No matching start tag, revert to defaults
                        color = default_format
                        size = default_size
                        alignment = default_alignment
                elif text[i] == 's':  # Small text
                    # Push current format to stack
                    format_stack.append((color, size, alignment))
                    size = 1
                elif text[i] == 'm':  # Centered text
                    # Push current format to stack
                    format_stack.append((color, size, alignment))
                    alignment = 'm'
                elif text[i] in COLOR_CODES:  # Color code
                    # Push current format to stack
                    format_stack.append((color, size, alignment))
                    color = COLOR_CODES[text[i]]
                
                # Find and skip to after the closing bracket
                while i < len(text) and text[i] != ']':
                    i += 1
            
            # Move past the closing bracket
            i += 1
            continue
        
        # Process special characters
        if i < len(text):
            if text[i] == ' ':
                # Space
                local_row_data.append([])
            else:
                # Regular character
                local_row_data.append([text[i], color, size])
        
        i += 1
    
    # Handle centered text by adding appropriate padding
    if alignment == 'm' and local_row_data:
        # Calculate visible text length (excluding empty slots)
        visible_length = sum(1 for item in local_row_data if item)
        
        # Calculate padding needed at the beginning
        padding = max(0, (CDU_COLUMNS - visible_length) // 2)
        
        # Add padding at the beginning of the row
        if padding > 0:
            padding_elements = [[] for _ in range(padding)]
            # Add existing row data for proper appending
            final_data = padding_elements + local_row_data
            row_data.extend(final_data)
            return row_data
    
    # For non-centered text, just append to the row_data
    row_data.extend(local_row_data)
    return row_data

def create_mobi_json(xml_string):
    """
    Parse ProSim 737 CDU XML data and convert it to MobiFlight JSON format.
    
    The CDU display is 24 columns x 14 rows.
    Special formatting includes:
    - ¨ is used as a delimiter within lines
    - [1]...[/1] indicates blue text
    - [s]...[/s] indicates small text
    - [m]...[/m] indicates centered text
    - Empty lines or lines with just ¨ should be 24 empty entries
    - Title text is centered by default
    """
    message = {
        "Target": "Display",
        "Data": []
    }
    
    try:
        root = ET.fromstring(xml_string)
        row_count = 0
        
        # Process title row
        if root.find('title') is not None and root.find('titlePage') is not None:
            title_text = root.find('title').text or ""
            title_page = root.find('titlePage').text or ""
            row_data = []
            
            # Process title text (centered by default)
            # Skip if title is "false" (special case)
            if title_text and title_text.lower() != "false":
                # Only process the title text if it's not "false"
                title_parts = title_text.split("¨")
                title_to_center = title_parts[-1].strip()
                
                # For title, we need to center it in the space available
                # Calculate available width (total width minus title page width)
                available_width = CDU_COLUMNS - len(title_page)
                
                # Calculate the visible length of the title (without formatting tags)
                visible_title_length = get_visible_length(title_to_center)
                
                # Calculate centering padding
                padding = max(0, (available_width - visible_title_length) // 2)
                
                # Add padding
                for _ in range(padding):
                    row_data.append([])
                
                # Now add the actual title text
                process_text_with_format(title_to_center, row_data, 'w', 0, 'l')
            
            # Title page (right-aligned)
            pad_to_position(row_data, CDU_COLUMNS - len(title_page))
            for char in title_page:
                row_data.append([char, 'w', 0])
                
            add_row_to_message(row_data, message["Data"])
            row_count += 1
        
        # Process normal lines
        for line_elem in root.findall('line'):
            line_text = line_elem.text or "¨"
            line_text = line_text.replace('#', '\u2610')  # Replace empty box with Unicode character
            line_text = line_text.replace('[]', '\u2610')  # Replace empty box with Unicode character
            line_text = line_text.replace('`', '\u00B0')  # Replace unicode for degree symbol

            
            # If just a delimiter or empty, create an empty row
            if line_text == "¨" or not line_text:
                add_empty_row(message["Data"])
                row_count += 1
                continue
            
            # Process line with potential left and right parts
            parts = line_text.split("¨")
            row_data = []
            
            # Check if the whole line should be centered (has [m] tag at the beginning)
            if parts[0].strip().startswith('[m]'):
                # Process the entire line as centered text without splitting
                full_line = line_text.replace('¨', ' ')  # Replace delimiter with space
                process_text_with_format(full_line, row_data, 'w', 0, 'm')
                add_row_to_message(row_data, message["Data"])
                row_count += 1
                continue
            
            # Process left part (if not a centered line)
            if parts[0]:
                process_text_with_format(parts[0], row_data, 'w', 0, 'l')
            
            # Process right part if exists
            if len(parts) > 1:
                # Calculate where to start the right part using the visible length
                right_part_visible_length = get_visible_length(parts[1])
                pad_to_position(row_data, CDU_COLUMNS - right_part_visible_length)
                process_text_with_format(parts[1], row_data, 'w', 0, 'l')
            
            add_row_to_message(row_data, message["Data"])
            row_count += 1
        
        # Process scratchpad (last row)
        if root.find('scratchpad') is not None:
            scratchpad_text = root.find('scratchpad').text or ""
            row_data = []
            process_text_with_format(scratchpad_text, row_data, 'w', 0, 'l')
            add_row_to_message(row_data, message["Data"])
            row_count += 1
        
        # Fill any remaining rows to make exactly 14 rows
        while row_count < CDU_ROWS:
            add_empty_row(message["Data"])
            row_count += 1
            
    except Exception as e:
        logging.error(f"Error parsing CDU XML: {e}")
        # Return empty grid if parsing fails
        message["Data"] = [[] for _ in range(CDU_COLUMNS * CDU_ROWS)]
    
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
    """
    Client for handling data communication between ProSim CDU and MobiFlight
    
    Manages the connection to both services and data transfer between them.
    """
    def __init__(self, prosim_client: ProSimGraphQLClient, websocket_uri: str, cdu_name: str, cdu_dataref_name: str) -> None:
        """
        Initialize the CDU client
        
        Args:
            prosim_client: The ProSim GraphQL client instance
            websocket_uri: The WebSocket URI for MobiFlight communication
            cdu_name: Display name for this CDU (e.g., "CAPTAIN" or "CO-PILOT")
            cdu_dataref_name: The dataref name for this CDU's display data
        """
        self.prosim_client = prosim_client
        self.mobiflight = MobiFlightClient(websocket_uri)
        self.event_loop = None
        self.cdu_name = cdu_name
        self.cdu_dataref_name = cdu_dataref_name
        self.connected = False
        self.last_cdu_data = None
        self._callback_tasks = set()  # Keep track of callback tasks

    def failed_to_connect(self) -> bool:
        """Check if MobiFlight client failed to connect after max retries"""
        return self.mobiflight.retries >= self.mobiflight.max_retries

    async def setup_prosim(self) -> bool:
        """
        Set up the ProSim GraphQL connection
        
        Returns:
            bool: True if setup was successful, False otherwise
        """
        try:
            if not self.prosim_client.connected and not await self.prosim_client.connect():
                logging.error(f"Failed to connect to ProSim GraphQL for {self.cdu_name}")
                return False

            logging.info(f"ProSim GraphQL connection initialized for {self.cdu_name} CDU")
            self.connected = True
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
        """
        Main entry point for running the CDU client
        
        Sets up connections and starts processing data
        """
        self.event_loop = asyncio.get_running_loop()
        logging.info(f"Starting {self.cdu_name} CDU client")
        
        try:
            # Start MobiFlight connection
            mobiflight_task = asyncio.create_task(self.mobiflight.run())
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
        captain_client = ProSimCDUClient(prosim_client, CAPTAIN_CDU_URL, "CAPTAIN", "aircraft.cdu1.display")
        co_pilot_client = ProSimCDUClient(prosim_client, CO_PILOT_CDU_URL, "CO-PILOT", "aircraft.cdu2.display")
        
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
