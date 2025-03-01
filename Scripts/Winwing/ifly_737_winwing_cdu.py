from ctypes import Structure, c_int, c_long, c_ubyte, c_double, c_bool, c_char
import ctypes
import json
import logging
import asyncio
from typing import Dict, List, Optional, Union
from websockets.client import connect
import mmap

# WebSocket URLs
CAPTAIN_CDU_URL: str = "ws://localhost:8320/winwing/cdu-captain"
FO_CDU_URL: str = "ws://localhost:8320/winwing/cdu-co-pilot"
# Constants
COLUMNS = 24
ROWS = 14
CELLS = COLUMNS * ROWS

# Memory map name
MEMORY_MAP_NAME = "iFly737MAX_SDK_FileMappingObject"

class ShareMemory737MAXSDK(ctypes.Structure):
    """Structure matching the iFly 737 MAX SDK memory layout"""
    _fields_ = [
        # Manually gathered from SDK_Defines.h
        ("OFFSET", c_ubyte * 0x42C),

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
            self._was_connected = True
            logging.info(f"Connected to WebSocket at {self.url}")
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

def create_mobi_json(memory_map: ShareMemory737MAXSDK, cdu_index: int) -> Dict:
    """Create JSON message for MobiFlight WebSocket from memory map data"""
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
                    char = memory_map.LSKChar[cdu_index][row][col].decode('ascii', errors='replace')
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

    def setup_memory_map(self) -> bool:
        try:
            self.memory_map = mmap.mmap(-1, ctypes.sizeof(ShareMemory737MAXSDK),
                                      MEMORY_MAP_NAME,
                                      access=mmap.ACCESS_READ)
            logging.info(f"Successfully opened memory map for CDU {self.cdu_index}")
            return True
        except Exception as e:
            logging.error(f"Failed to open memory map for CDU {self.cdu_index}: {e}")
            return False

    async def process_memory_map(self) -> None:
        if not self.memory_map:
            return
        
        try:
            # Read the entire structure
            data = self.memory_map.read(ctypes.sizeof(ShareMemory737MAXSDK))
            self.memory_map.seek(0)
            
            # Create structure from memory map data
            memory_struct = ShareMemory737MAXSDK.from_buffer_copy(data)
            
            # Create and send JSON message
            json_data = create_mobi_json(memory_struct, self.cdu_index)
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