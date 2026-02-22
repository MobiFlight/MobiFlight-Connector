from ctypes import Structure, c_int, c_int32, c_long, c_ubyte, c_double, c_bool, c_char, c_wchar
from enum import Enum
import ctypes
import json
import logging
import asyncio
from typing import Dict, List, Optional, Union
from websockets.asyncio.client import connect
import mmap

# WebSocket URLs
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
FO_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"
# Constants
COLUMNS = 24
ROWS = 14
CELLS = COLUMNS * ROWS

# Memory map name
NG_SDK2_MEMORY_MAP_NAME = "iFly737NG_SDK2_FileMappingObject"
MAX_SDK_MEMORY_MAP_NAME = "iFly737MAX_SDK_FileMappingObject"

class iFlySDK_Identifier(Enum):
    SDK_UNKNOWN = 1
    SDK_NG = 2
    SDK_MAX = 3

class ShareMemory737NGSDK2(ctypes.Structure):
    """Structure matching the iFly 737 NG SDK2 SDK memory layout"""
    _fields_ = [
        # Manually gathered from SDK_CDU.h

        ("LSKChar", ((c_wchar * 24) * 14) * 2),         # <WCHAR> = 16bit unicode character (2 bytes)
        ("LSK_SmallFont", ((c_int32 * 24) * 14) * 2),   # <BOOL> = 32bit int (4 bytes)
        ("LSK_Color", ((c_int * 24) * 14) * 2),
        ("CDU_Can_Display", c_int32 * 2),               # <BOOL> # FALSE: the screen is blank due to power loss or other situation: TRUE: the screen can display normally
        # Unused, but might be useful at some point
        ("CDU_MSG_Status", c_int32 * 2),                # <BOOL> = 32bit int (4 bytes)
        ("CDU_EXEC_Status", c_int32 * 2),               # <BOOL> = 32bit int (4 bytes)
        ("CDU_CALL_Status", c_int32 * 2),               # <BOOL> = 32bit int (4 bytes)
        ("CDU_OFST_Status", c_int32 * 2),               # <BOOL> = 32bit int (4 bytes)
        ("CDU_TEST_Status", c_int * 2),   # 0: no test, test colours = 1:RED, 2:GREEN, 3:BLUE, 4:AMBER, 5:MAGENTA, 6:CYAN, 7:WHITE, 8:GRAYSCALE, 8:CHECKERBOARD
        ("iFly737NG_State", c_int),                     # iFly737NG is running
    ]


class ShareMemory737MAXSDK(ctypes.Structure):
    """Structure matching the iFly 737 MAX SDK memory layout"""
    _fields_ = [
        # Manually gathered from SDK_Defines.h
        ("iFly737MAX_State", c_int32),                  # iFly737MAX is running
        ("OFFSET", c_ubyte * 0x428),

        ("LSKChar", ((c_char * 24) * 14) * 2),
        ("LSK_SmallFont", ((c_bool * 24) * 14) * 2),
        ("LSK_Color", ((c_ubyte * 24) * 14) * 2),
        # Unused, but might be useful at some point
        ("CDU_FAIL_Status", c_ubyte * 2),
        ("CDU_MSG_Status", c_ubyte * 2),
        ("CDU_EXEC_Status", c_ubyte * 2),
        ("CDU_CALL_Status", c_ubyte * 2),
        ("CDU_OFST_Status", c_ubyte * 2),
        ("CDU_BRT_Switch_Status", c_ubyte * 2),
    ]


class MobiFlightClient:
    def __init__(self, url: str) -> None:
        self.url: str = url
        self.websocket = None
        self._was_connected: bool = False

    async def connect(self) -> None:
        try:
            self.websocket = await connect(self.url)            
            logging.info(f"Connected to WebSocket at {self.url}")
            # Load font           
            fontName: str = "Boeing"
            await self.websocket.send(f'{{ "Target": "Font", "Data": "{fontName}" }}')
            logging.info(f"Setting font: {fontName}")
            await asyncio.sleep(1) # wait a second for font to be set
            self._was_connected = True
        except Exception as e:
            logging.error(f"Failed to connect to WebSocket: {e}")
            self._was_connected = False

    async def send(self, data: Dict) -> None:
        if not self.websocket:
            if self._was_connected:
                # Try to reconnect if we were previously connected
                await self.connect()
            if not self.websocket:
                return
        
        try:
            await self.websocket.send(json.dumps(data))
        except Exception as e:
            logging.error(f"Failed to send data: {e}")
            self._was_connected = False
            self.websocket = None

    async def close(self) -> None:
        if self.websocket:
            await self.websocket.close()
            self.websocket = None

#
#       CDU: Display "WAITING FOR IFLY 737" while waiting to connect to iFly
#
def create_wait_ifly_json() -> Dict:
    """Create JSON message for MobiFlight WebSocket"""
    message: Dict[str, Union[str, List[List[Union[str, int]]]]] = {
        "Target": "Display",
        "Data": [[] for _ in range(CELLS)]
    }
    
    data = []        

    # Empty cells for first 5 rows
    data.extend([[]] * (5 * COLUMNS))

    # Display message lines
    line6 = [[],[],[],[],[],["W","c",1],[],["A","c",1],[],["I","c",1],[],["T","c",1],[],["I","c",1],[],["N","c",1],[],["G","c",1],[],[],[],[],[],[]]
    line7 = [[],[],[],[],[],[],[],[],[],["F","c",1],[],["O","c",1],[],["R","c",1],[],[],[],[],[],[],[],[],[],[]]
    line8 = [[],[],[],[],[],["I","w",1],[],["F","w",1],[],["L","w",1],[],["Y","w",1],[],["7","m",1],[],["3","m",1],[],["7","m",1],[],[],[],[],[],[]]

    data.extend(line6 + line7 + line8)

    # Empty cells for remaining rows
    data.extend([[]] * (6 * COLUMNS))

    message["Data"] = data
                
    return message
#
#       CDU/NG: Display we're switched off / empty
#
def create_ng_nopower_cdu_json() -> Dict:
    """Create JSON message for MobiFlight WebSocket"""
    message: Dict[str, Union[str, List[List[Union[str, int]]]]] = {
        "Target": "Display",
        "Data": [[] for _ in range(CELLS)]
    }
    
    data = [[]] * (ROWS * COLUMNS)

    message["Data"] = data
                
    return message
#
#       CDU: Display contents of the iFly CDU (works for both NG and MAX)
#
def create_cdu_mobi_json(memory_map: Union[ShareMemory737NGSDK2, ShareMemory737MAXSDK], cdu_index: int) -> Dict:
    """Create JSON message for MobiFlight WebSocket from memory map data
    
    Args:
        memory_map: Either ShareMemory737NGSDK2 or ShareMemory737MAXSDK structure
        cdu_index: 0 for captain, 1 for first officer
        
    Returns:
        Dictionary with Target and Data fields for MobiFlight WebSocket
    """
    message: Dict[str, Union[str, List[List[Union[str, int]]]]] = {
        "Target": "Display",
        "Data": [[] for _ in range(CELLS)]
    }
    
    # Color mapping from iFly to MobiFlight format
    color_map = {
        0: "w",  # White
        1: "g",  # Green
        2: "c",  # Cyan
        3: "m",  # Magenta
        4: "e",  # Grey (for reverse video/background)
        5: "w",  # Box (using grey)
        6: "w",  # Degree Symbol (White)
        7: "e",  # Degree Symbol (Grey)
        8: "m",  # Degree Symbol (Magenta)
        9: "w",  # Left Arrow (White)
        10: "w"  # Right Arrow (White)
    }

    try:
        data = []        
        for row in range(ROWS):
            for col in range(COLUMNS):
                    char_raw = memory_map.LSKChar[cdu_index][row][col]
                    # Handle both c_char (bytes) from MAX and c_wchar (str) from NG
                    if isinstance(char_raw, bytes):
                        char = char_raw.decode('ascii', errors='replace')
                    else:
                        char = char_raw
                    
                    small_font = memory_map.LSK_SmallFont[cdu_index][row][col]
                    color = memory_map.LSK_Color[cdu_index][row][col]
                    
                    if color == 0 and char in [' ', '\0']:
                        data.append([])
                    else:
                        # Handle special characters
                        if color == 5:  # Box character
                            char = "\u2610"  # Unicode box
                        elif color == 9:  # Left arrow
                            char = "\u2190"  # Unicode left arrow
                        elif color == 10:  # Right arrow
                            char = "\u2192"  # Unicode right arrow
                        elif color in (6, 7, 8):  # Degree symbol
                            char = "\u00B0"  # Unicode degree symbol

                        data.append([
                            char,
                            color_map.get(color, "w"),
                            1 if small_font else 0
                        ])

        message["Data"] = data
                
    except Exception as e:
        logging.error(f"Error processing CDU data: {e}")
        return {"Target": "Display", "Data": [[] for _ in range(CELLS)]}
    
    return message

class IFlyCDUClient:
    def __init__(self, cdu_index: int) -> None:
        self.cdu_index: int = cdu_index  # 0 for captain, 1 for F/O
        self.client = MobiFlightClient(CAPTAIN_CDU_URL if cdu_index == 0 else FO_CDU_URL)
        self.memory_map: Optional[mmap.mmap] = None
        self._running: bool = False
        self.iflySDK: iFlySDK_Identifier = iFlySDK_Identifier.SDK_UNKNOWN

    def _memory_map_exists(self, name: str) -> bool:
        """Check if a named memory map exists without creating it"""
        try:
            kernel32 = ctypes.windll.kernel32
            # Try to open existing mapping (FILE_MAP_READ = 0x0004)
            handle = kernel32.OpenFileMappingW(
                0x0004,  # FILE_MAP_READ
                False,   # Don't inherit handle
                name
            )
            
            if handle:
                kernel32.CloseHandle(handle)
                return True
            return False
        except Exception as e:
            logging.debug(f"Error checking memory map '{name}': {e}")
            return False

    def setup_memory_map(self) -> bool:
        """Setup memory map by detecting which iFly variant is running"""
        try:
            # Check MAX first
            if self._memory_map_exists(MAX_SDK_MEMORY_MAP_NAME):
                try:
                    self.memory_map = mmap.mmap(-1, ctypes.sizeof(ShareMemory737MAXSDK),
                                              MAX_SDK_MEMORY_MAP_NAME,
                                              access=mmap.ACCESS_READ)
                    data = self.memory_map.read(ctypes.sizeof(ShareMemory737MAXSDK))
                    self.memory_map.seek(0)
                    memory_struct = ShareMemory737MAXSDK.from_buffer_copy(data)
                    
                    if 1 == memory_struct.iFly737MAX_State:
                        self.iflySDK = iFlySDK_Identifier.SDK_MAX
                        logging.info(f"Successfully opened memory map for iFly737MAX CDU {self.cdu_index}")
                        return True
                    else:
                        # MAX memory map exists but simulator not running
                        logging.warning(f"iFly737MAX memory map exists but State={memory_struct.iFly737MAX_State}")
                        self.memory_map.close()
                        self.memory_map = None
                except Exception as e:
                    logging.error(f"Failed to read MAX memory map: {e}")
                    if self.memory_map:
                        self.memory_map.close()
                        self.memory_map = None
            
            # Check NG
            if self._memory_map_exists(NG_SDK2_MEMORY_MAP_NAME):
                try:
                    self.memory_map = mmap.mmap(-1, ctypes.sizeof(ShareMemory737NGSDK2),
                                              NG_SDK2_MEMORY_MAP_NAME,
                                              access=mmap.ACCESS_READ)
                    data = self.memory_map.read(ctypes.sizeof(ShareMemory737NGSDK2))
                    self.memory_map.seek(0)
                    memory_struct = ShareMemory737NGSDK2.from_buffer_copy(data)
                    
                    if 1 == memory_struct.iFly737NG_State:
                        self.iflySDK = iFlySDK_Identifier.SDK_NG
                        logging.info(f"Successfully opened memory map for iFly737NG CDU {self.cdu_index}")
                        return True
                    else:
                        # NG memory map exists but simulator not running
                        logging.warning(f"iFly737NG memory map exists but State={memory_struct.iFly737NG_State}")
                        self.memory_map.close()
                        self.memory_map = None
                except Exception as e:
                    logging.error(f"Failed to read NG memory map: {e}")
                    if self.memory_map:
                        self.memory_map.close()
                        self.memory_map = None
            
            # Neither memory map exists or both failed state checks
            logging.error(f"No active iFly 737 simulator found for CDU {self.cdu_index}")
            return False

        except Exception as e:
            logging.error(f"Failed to setup memory map for CDU {self.cdu_index}: {e}")
            if self.memory_map:
                self.memory_map.close()
                self.memory_map = None
            return False

    async def process_memory_map(self) -> None:
        if not self.memory_map:
            return
        
        try:
            # Get the appropriate structure type and size based on SDK
            if self.iflySDK == iFlySDK_Identifier.SDK_MAX:
                structure_class = ShareMemory737MAXSDK
                state_field = 'iFly737MAX_State'
                has_power_check = False
            elif self.iflySDK == iFlySDK_Identifier.SDK_NG:
                structure_class = ShareMemory737NGSDK2
                state_field = 'iFly737NG_State'
                has_power_check = True
            else:
                return
            
            # Read and parse memory structure (common code)
            data = self.memory_map.read(ctypes.sizeof(structure_class))
            self.memory_map.seek(0)
            memory_struct = structure_class.from_buffer_copy(data)
            
            # Determine which JSON to generate based on state
            state_value = getattr(memory_struct, state_field)
            
            if state_value == 0:
                # iFly unavailable
                json_data = create_wait_ifly_json()
            elif has_power_check and memory_struct.CDU_Can_Display[self.cdu_index] == 0:
                # NG only: CDU powered off
                json_data = create_ng_nopower_cdu_json()
            else:
                # Normal operation
                json_data = create_cdu_mobi_json(memory_struct, self.cdu_index)

            await self.client.send(json_data)
            
        except Exception as e:
            logging.error(f"Error processing memory map for CDU {self.cdu_index}: {e}")

    async def run(self) -> None:
        if not self.setup_memory_map():
            return
        
        self._running = True
        await self.client.connect()
        
        try:
            while self._running:
                await self.process_memory_map()
                await asyncio.sleep(0.1)  # Update 10 times per second
        except asyncio.CancelledError:
            logging.info(f"CDU {self.cdu_index} client was cancelled")
        except Exception as e:
            logging.error(f"Error in run loop for CDU {self.cdu_index}: {e}")
        finally:
            await self.client.close()
            if self.memory_map:
                self.memory_map.close()
                self.memory_map = None

    def stop(self) -> None:
        self._running = False

async def main() -> None:
    # Create clients for both CDUs
    captain_cdu = IFlyCDUClient(0)
    fo_cdu = IFlyCDUClient(1)
    
    try:
        # Run both clients concurrently
        await asyncio.gather(
            captain_cdu.run(),
            fo_cdu.run()
        )
    except KeyboardInterrupt:
        logging.info("Shutting down CDU clients...")
        captain_cdu.stop()
        fo_cdu.stop()

if __name__ == "__main__":
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s - %(levelname)s - %(message)s'
    )
    asyncio.run(main())