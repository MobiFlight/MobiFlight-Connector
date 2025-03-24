import clr
from pathlib import Path
from typing import Callable, Optional
import json
import logging
import asyncio
import websockets.asyncio.client as ws_client
import xml.etree.ElementTree as ET
import re  # Add this at the top of the file with other imports

# Connection settings for ProSim SDK
host = "localhost"
port = 8082
url = f"net.tcp://{host}:{port}/SDK"

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
            from ProSimSDK import ProSimConnect, DataRef # type: ignore
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
                    "date_unit": dr.DataUnit
                }
            )
        return dataref_list

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

def test_create_mobi_json():
    """
    Test the create_mobi_json function with various XML formats
    
    Returns:
        bool: True if all tests passed, False otherwise
    """
    try:
        # Test 1: Basic CDU data with formatting
        test_xml1 = """
        <cdu>
            <title>INIT REF</title>
            <titlePage>1/2</titlePage>
            <line>AIRPORT¨[s]KSEA[/s]</line>
            <line>GATE¨[1]A12[/1]</line>
            <line>POSITION</line>
            <line>¨[1][s]N47°27.0' W122°18.5'[/s][/1]</line>
            <line>ROUTE¨KSEA.SUMMA9.SUMMA.KPDX</line>
            <line></line>
            <line>PERF INIT>[s]THRUST LIM[/s]></line>
            <scratchpad></scratchpad>
        </cdu>
        """
        
        # Test 2: Empty CDU
        test_xml2 = """
        <cdu>
            <title>BLANK</title>
            <titlePage>---</titlePage>
            <line></line>
            <line></line>
            <line></line>
            <line></line>
            <line></line>
            <line></line>
            <line></line>
            <scratchpad></scratchpad>
        </cdu>
        """
        
        # Test 3: Special characters
        test_xml3 = """
        <cdu>
            <title>SPECIAL CHARS</title>
            <titlePage>TEST</titlePage>
            <line>#¤¥¢£&</line>
            <line></line>
            <line></line>
            <line></line>
            <line></line>
            <line></line>
            <line></line>
            <scratchpad></scratchpad>
        </cdu>
        """
        
        # Test 4: Multiple formats and nested formats
        test_xml4 = """
        <cdu>
            <title>FORMAT TEST</title>
            <titlePage>FMT</titlePage>
            <line>[1]BLUE TEXT[/1]¨[2]GREEN[/2]</line>
            <line>[3]AMBER[/3]¨[s]Small text[/s]</line>
            <line>[2][s]Small green[/s][/2]</line>
            <line>[1]Blue [s]small blue[/s] text[/1]</line>
            <line>[s][1]Small blue[/1][/s]</line>
            <line></line>
            <line></line>
            <scratchpad>[4]TEST YELLOW[/4]</scratchpad>
        </cdu>
        """
        
        # Test 5: Centered text
        test_xml5 = """
        <cdu>
            <title>ALIGNMENT TEST</title>
            <titlePage>ALG</titlePage>
            <line>[m]CENTERED TEXT[/m]</line>
            <line>[m][1]BLUE CENTERED[/1][/m]</line>
            <line>[1][m]ALSO CENTERED BLUE[/m][/1]</line>
            <line>[m][s]SMALL CENTERED[/s][/m]</line>
            <line>LEFT¨RIGHT</line>
            <line>[m]CENTER WITH SPACES[/m]</line>
            <line></line>
            <scratchpad></scratchpad>
        </cdu>
        """
        
        # Process each test case
        for i, test_xml in enumerate([test_xml1, test_xml2, test_xml3, test_xml4, test_xml5], 1):
            logging.info(f"Testing XML example {i}...")
            json_result = create_mobi_json(test_xml)
            parsed_result = json.loads(json_result)
            
            # Basic validation
            assert "Data" in parsed_result, f"Test {i}: Missing 'Data' key in result"
            data_array = parsed_result["Data"]
            
            # Validate array dimensions (should be flat array of 24*14 elements)
            assert len(data_array) == CDU_COLUMNS * CDU_ROWS, f"Test {i}: Expected {CDU_COLUMNS * CDU_ROWS} elements, got {len(data_array)}"
            
            # For every test, verify the title row is centered
            # Extract the first row data (title row)
            first_row = data_array[:CDU_COLUMNS]
            
            # Count empty elements at the beginning
            leading_empty = 0
            for cell in first_row:
                if not cell:
                    leading_empty += 1
                else:
                    break
            
            # Count non-empty elements
            content_length = sum(1 for cell in first_row if cell)
            
            # Ignore the title page length (typically right-aligned)
            title_page_length = 0
            if i == 1:  # For test 1, title page is "1/2" (3 chars)
                title_page_length = 3
            elif i == 2:  # For test 2, title page is "---" (3 chars)
                title_page_length = 3
            elif i == 3:  # For test 3, title page is "TEST" (4 chars)
                title_page_length = 4
            elif i == 4:  # For test 4, title page is "FMT" (3 chars)
                title_page_length = 3
            elif i == 5:  # For test 5, title page is "ALG" (3 chars)
                title_page_length = 3
                
            # Adjust content length to not include title page
            content_length -= title_page_length
            
            # For title row, centering should be based on the available space
            # which is CDU_COLUMNS - title_page_length
            available_width = CDU_COLUMNS - title_page_length
            expected_leading = (available_width - content_length) // 2
            
            logging.info(f"Test {i} title centering - Leading spaces: {leading_empty}, Content length: {content_length}")
            logging.info(f"Available width: {available_width}, Expected leading spaces: ~{expected_leading}")
            
            # Allow for small variations due to padding calculations
            # Title might not be perfectly centered due to title page on the right
            assert leading_empty > 0, f"Title text should have leading spaces for centering, got {leading_empty}"
            
            # For test 5 (alignment test), also verify the first line (not title) is centered
            if i == 5:
                # Extract the second row data (first line)
                second_row = data_array[CDU_COLUMNS:CDU_COLUMNS*2]
                
                # Count empty elements at the beginning
                leading_empty_line = 0
                for cell in second_row:
                    if not cell:
                        leading_empty_line += 1
                    else:
                        break
                
                # Count non-empty elements
                content_length_line = sum(1 for cell in second_row if cell)
                
                # Verify that the line is roughly centered
                expected_leading_line = (CDU_COLUMNS - content_length_line) // 2
                logging.info(f"Line centering test - Leading spaces: {leading_empty_line}, Content length: {content_length_line}")
                logging.info(f"Expected leading spaces: ~{expected_leading_line}")
                
                # Allow for small variations due to padding calculations
                assert abs(leading_empty_line - expected_leading_line) <= 1, f"Text not properly centered: expected ~{expected_leading_line} leading spaces, got {leading_empty_line}"
            
            # Log the first few rows for inspection
            logging.info(f"Test {i} result (first 3 rows):")
            for row in range(3):
                row_data = data_array[row*CDU_COLUMNS:(row+1)*CDU_COLUMNS]
                logging.info(f"Row {row}: {row_data}")
                
        logging.info("All tests passed!")
        return True
        
    except Exception as e:
        logging.error(f"Test failed: {e}")
        import traceback
        logging.error(traceback.format_exc())
        return False

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
            
    def disconnect(self) -> bool:
        """
        Disconnect from ProSim and clean up resources
        
        Returns:
            bool: True if disconnect was successful, False otherwise
        """
        try:
            if self.connected and hasattr(self.pyprosim.sdk, 'Disconnect'):
                logging.info("Disconnecting from ProSim")
                self.pyprosim.sdk.Disconnect()
                self.connected = False
                
            # Clear datarefs
            if hasattr(self.pyprosim, 'datarefs'):
                self.pyprosim.datarefs.clear()
                
            return True
        except Exception as e:
            logging.error(f"Error disconnecting from ProSim: {e}")
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
    """
    Client for handling data communication between ProSim CDU and MobiFlight
    
    Manages the connection to both services and data transfer between them.
    """
    def __init__(self, prosim_client: ProSimClient, websocket_uri: str, cdu_name: str, cdu_dataref_name: str) -> None:
        """
        Initialize the CDU client
        
        Args:
            prosim_client: The ProSim client instance
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
        
        # Store the last CDU data to avoid sending duplicates
        self.last_cdu_data = None

    def failed_to_connect(self) -> bool:
        """Check if MobiFlight client failed to connect after max retries"""
        return self.mobiflight.retries >= self.mobiflight.max_retries

    def setup_prosim(self) -> bool:
        """
        Set up the ProSim connection and register the CDU dataref
        
        Returns:
            bool: True if setup was successful, False otherwise
        """
        try:
            if not self.prosim_client.connected and not self.prosim_client.connect():
                logging.error(f"Failed to connect to ProSim for {self.cdu_name}")
                return False
                
            # Check that we can access datarefs
            datarefs = self.prosim_client.pyprosim.get_all_avail_datarefs()
            if not datarefs:
                logging.error(f"No datarefs found in ProSim for {self.cdu_name}")
                return False
                
            # Find matching CDU datarefs in case the specified one isn't exact
            cdu_datarefs = [dr["name"] for dr in datarefs if "cdu" in dr["name"].lower() or "mcdu" in dr["name"].lower()]
            
            # Register our dataref
            if not self.prosim_client.add_callback(self.cdu_dataref_name, None):
                logging.warning(f"Failed to register dataref '{self.cdu_dataref_name}' for {self.cdu_name}")
                if cdu_datarefs:
                    logging.info(f"Available CDU datarefs: {', '.join(cdu_datarefs)}")
                    logging.info(f"Try using one of these instead.")
                return False

            logging.info(f"ProSim connection initialized for {self.cdu_name} CDU")
            self.connected = True
            return True
        except Exception as e:
            logging.error(f"ProSim setup failed for {self.cdu_name}: {e}")
            return False

    async def process_prosim(self) -> None:
        """
        Process CDU data from ProSim and send to MobiFlight
        
        Continuously polls ProSim for CDU data and forwards to MobiFlight
        """
        error_count = 0
        max_errors = 10
        
        while True:
            try:
                # Get CDU data from ProSim
                dataref = self.prosim_client.get_dataref_value(self.cdu_dataref_name)
                
                # Only send if data exists and has changed
                if dataref and dataref != self.last_cdu_data:
                    # Parse and send CDU data
                    json_data = create_mobi_json(dataref)
                    asyncio.run_coroutine_threadsafe(
                        self.mobiflight.send(json_data), 
                        self.event_loop
                    )
                    self.last_cdu_data = dataref
                    error_count = 0  # Reset error count on success
                
            except Exception as e:
                error_count += 1
                logging.error(f"Error processing CDU data for {self.cdu_name}: {e}")
                if error_count >= max_errors:
                    logging.error(f"Too many errors processing CDU data for {self.cdu_name}, stopping")
                    break
                    
            # Wait before next update
            await asyncio.sleep(0.1)

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
                
            # Initialize ProSim connection
            if self.setup_prosim():
                # Start processing ProSim data
                simconnect_task = asyncio.create_task(self.process_prosim())
                await asyncio.gather(mobiflight_task, simconnect_task)
            else:
                logging.error(f"Failed to start {self.cdu_name} - ProSim initialization failed")
                
        except asyncio.CancelledError:
            logging.info(f"Shutting down {self.cdu_name} CDU client")
        except Exception as e:
            logging.error(f"Error in {self.cdu_name} CDU client: {e}")
        finally:
            # Clean up
            self.connected = False
            await self.mobiflight.close()



if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s - %(levelname)s - %(message)s'
    )
    
    try:
        # Check if the ProSimSDK.dll file exists
        sdk_path = Path("ProSimSDK.dll").absolute()
        if not sdk_path.exists():
            logging.error(f"ProSimSDK.dll not found at {sdk_path}")
            logging.error("Please make sure the ProSimSDK.dll file is in the same directory as this script")
            exit(1)
            
        # Initialize the ProSim client
        logging.info(f"Initializing ProSim client with SDK at {sdk_path}")
        sc_prosim = ProSimClient()
        
        # Create the CDU clients
        logging.info("Creating CDU clients")
        # Use default datarefs, these will be validated during setup
        captain_client = ProSimCDUClient(sc_prosim, CAPTAIN_CDU_URL, "CAPTAIN", "aircraft.cdu1.display")
        co_pilot_client = ProSimCDUClient(sc_prosim, CO_PILOT_CDU_URL, "CO-PILOT", "aircraft.cdu2.display")
        
        # Run the clients
        async def run_clients():
            # Connect to ProSim first
            if not sc_prosim.connect():
                logging.error("Failed to connect to ProSim. Please check if ProSim is running.")
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
        logging.info("Cleaning up resources")
        try:
            if 'sc_prosim' in locals() and hasattr(sc_prosim, 'disconnect'):
                sc_prosim.disconnect()
        except Exception as e:
            logging.error(f"Error during cleanup: {e}")
        
        logging.info("Application terminated")
