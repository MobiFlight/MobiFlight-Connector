"""
MD80 MCDU Support for MobiFlight in X-Plane 12
Adapted for the Rotate MD80 aircraft

The MD80 has a single MCDU with green-only display.
This script fetches the CDU display data from X-Plane and sends it to MobiFlight
for display on physical CDU hardware.

MD80 CDU Characteristics:
- Only one MCDU (not separate Captain/CoPilot units)
- Only green color display
- Virtual CDU in X-Plane: 30 characters wide
- Physical display limit: 24 characters wide (intelligently trimmed)
- Alternating text sizes: even lines (1,3,5,7,9,11,13) are large text for data,
  odd lines (2,4,6,8,10,12) are small text for headers/labels. One exception is the last line (14) which is large text (input zone).
- Uses $ character for ballot boxes
"""

import asyncio
import json
import logging
import urllib.request
import websockets
from enum import StrEnum
from typing import List, Dict
import base64

# Configure logging
logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

# CDU Display Configuration
CDU_COLUMNS = 24  # Physical display width limit
CDU_ROWS = 14
CDU_CELLS = CDU_COLUMNS * CDU_ROWS
VIRTUAL_CDU_WIDTH = 30  # MD80 virtual CDU width in X-Plane

# WebSocket Configuration
WEBSOCKET_HOST = "localhost"
WEBSOCKET_PORT = 8320
BASE_REST_URL = "http://localhost:8086/api/v2/datarefs"
BASE_WEBSOCKET_URI = f"ws://{WEBSOCKET_HOST}:8086/api/v2"

# MobiFlight WebSocket endpoint for the single MD80 MCDU
WS_MD80_MCDU = f"ws://{WEBSOCKET_HOST}:{WEBSOCKET_PORT}/winwing/cdu-captain"

# Special character mappings
BALLOT_BOX = "☐"
DEGREES = "°"
CHAR_MAP = {
    "$": BALLOT_BOX,      # MD80 uses $ for ballot boxes
    "`": DEGREES,
}


class CduDevice(StrEnum):
    """Single MCDU device for MD80"""
    MD80_MCDU = "mcdu"
    
    def get_endpoint(self) -> str:
        """Get the MobiFlight WebSocket endpoint"""
        return WS_MD80_MCDU
    
    def get_line_datarefs(self) -> List[str]:
        """Get list of dataref paths for all CDU lines"""
        # Based on MD80 dataref structure - adjust if the actual paths are different
        datarefs = []
        for i in range(1, CDU_ROWS + 1):
            # Format: Rotate/md80/instruments/cdu_line_XX
            line_num = str(i).zfill(2)
            datarefs.append(f"Rotate/md80/instruments/cdu_line_{line_num}")
        return datarefs


def get_char(char: str) -> str:
    """Convert special characters to displayable format"""
    return CHAR_MAP.get(char, char)


def fetch_dataref_ids(device: CduDevice) -> Dict[int, str]:
    """
    Fetch dataref IDs from X-Plane for the CDU lines
    Returns a mapping of dataref ID to dataref name
    """
    try:
        with urllib.request.urlopen(BASE_REST_URL, timeout=5) as response:
            response_json = json.load(response)
            
            dataref_map = {}
            line_datarefs = device.get_line_datarefs()
            
            for dataref_entry in response_json.get("data", []):
                dataref_name = str(dataref_entry.get("name", ""))
                dataref_id = int(dataref_entry.get("id", 0))
                
                # Check if this dataref is one of our CDU lines
                if dataref_name in line_datarefs:
                    dataref_map[dataref_id] = dataref_name
                    logging.info(f"Found dataref: {dataref_name} with ID {dataref_id}")
            
            if not dataref_map:
                logging.warning("No CDU datarefs found! Check the dataref paths.")
            
            return dataref_map
    except Exception as e:
        logging.error(f"Error fetching dataref mapping: {e}")
        return {}


def trim_line_intelligently(line_text: str, target_width: int = CDU_COLUMNS) -> str:
    """
    Intelligently trim a line from virtual CDU width (30) to physical display width (24)
    Tries to preserve content by removing excess dashes, extra spaces, etc.
    """
    # If line is already at or below target width, just pad/return as is
    if len(line_text) <= target_width:
        return line_text.ljust(target_width)
    
    # If line starts and end with dashes, trim those first until it fits
    if line_text.startswith('-') and line_text.endswith('-'):
        while line_text.startswith('-') and line_text.endswith('-') and len(line_text) > target_width:
            line_text = line_text[1:-1]
    
    # Strip trailing spaces first
    line_text = line_text.rstrip()
    if len(line_text) <= target_width:
        return line_text.ljust(target_width)
    
    # Try to remove multiple spaces (reduce to single spaces)
    while '  ' in line_text and len(line_text) > target_width:
        line_text = line_text.replace('  ', ' ', 1)
    
    # If still too long, just truncate
    return line_text[:target_width]


def generate_display_json(cdu_lines: List[str]) -> str:
    """
    Generate the display JSON for MobiFlight from CDU line data
    MD80 specific: all text is green, alternating small/large text
    Even lines (1,3,5,7,9,11,13) are large text (data)
    Odd lines (2,4,6,8,10,12) are small text (headers/labels)
    Line 14 (row_idx 13) is large text (input zone)
    Lines are trimmed from virtual width (30) to physical width (24)
    """
    display_data = [[] for _ in range(CDU_CELLS)]
    
    # Process each line of the CDU
    for row_idx, line_text in enumerate(cdu_lines[:CDU_ROWS]):
        # Intelligently trim line from 30 to 24 characters
        line_text = trim_line_intelligently(line_text, CDU_COLUMNS)
        
        # Determine text size based on line number
        # Line 1,3,5,7,9,11,13 (odd numbers) should be large (data)
        # Line 2,4,6,8,10,12 (even numbers) should be small (headers/labels)
        # Line 14 (row_idx 13) is large text (input zone)
        # row_idx 0 = line 1 (odd) → large
        # row_idx 1 = line 2 (even) → small
        text_size = 0 if (row_idx % 2 == 0) else 1
        if row_idx == 13:  # Last line (14) is large text
            text_size = 0
        
        # Process each character in the line
        for col_idx, char in enumerate(line_text):
            if char != ' ':  # Skip empty spaces
                cell_idx = row_idx * CDU_COLUMNS + col_idx
                # Format: [character, color, size]
                # MD80: always green ('g'), alternating size
                display_data[cell_idx] = [get_char(char), 'g', text_size]
    
    return json.dumps({"Target": "Display", "Data": display_data})


async def handle_device_update(queue: asyncio.Queue, device: CduDevice):
    """
    Handles sending display updates to MobiFlight
    Reads from the queue and sends formatted data to the CDU hardware
    """
    last_run_time = 0
    rate_limit_time = 0.1  # Rate limiting to prevent overwhelming the connection
    
    endpoint = device.get_endpoint()
    logging.info(f"Connecting to MobiFlight CDU at {endpoint}")
    
    async for websocket in websockets.connect(endpoint):
        logging.info("Successfully connected to MobiFlight CDU")
        while True:
            try:
                cdu_lines = await queue.get()
                
                # Rate limiting
                elapsed = asyncio.get_event_loop().time() - last_run_time
                if elapsed < rate_limit_time:
                    await asyncio.sleep(rate_limit_time - elapsed)
                
                # Generate and send display data
                display_json = generate_display_json(cdu_lines)
                await websocket.send(display_json)
                last_run_time = asyncio.get_event_loop().time()
                
            except websockets.exceptions.ConnectionClosed:
                logging.error("MobiFlight connection lost. Attempting to reconnect...")
                await queue.put(cdu_lines)  # Put data back in queue
                break
            except Exception as e:
                logging.error(f"Error sending to MobiFlight: {e}")


async def handle_dataref_updates(queue: asyncio.Queue, device: CduDevice):
    """
    Handles receiving dataref updates from X-Plane
    Subscribes to CDU line datarefs and pushes updates to the queue
    """
    # Initialize lines with empty strings
    current_cdu_lines = [''] * CDU_ROWS
    last_sent_lines = None
    
    # Get dataref mapping
    dataref_map = fetch_dataref_ids(device)
    if not dataref_map:
        logging.error("No datarefs found. Exiting dataref handler.")
        return
    
    logging.info(f"Monitoring {len(dataref_map)} CDU line datarefs")
    logging.info("Connecting to X-Plane WebSocket server")
    
    async for websocket in websockets.connect(BASE_WEBSOCKET_URI):
        logging.info("Successfully connected to X-Plane WebSocket server")
        try:
            # Subscribe to all CDU line datarefs
            subscribe_msg = json.dumps({
                "type": "dataref_subscribe_values",
                "req_id": 1,
                "params": {
                    "datarefs": [{"id": id_value} for id_value in dataref_map.keys()]
                }
            })
            await websocket.send(subscribe_msg)
            logging.info(f"Subscribed to {len(dataref_map)} datarefs")
            
            while True:
                message = await websocket.recv()
                data = json.loads(message)
                
                if "data" not in data:
                    continue
                
                # Update only the lines that have changed
                for dataref_id, value in data["data"].items():
                    dataref_id = int(dataref_id)
                    if dataref_id not in dataref_map:
                        continue
                    
                    dataref_name = dataref_map[dataref_id]
                    
                    # Extract line number from dataref name
                    # Expected format: Rotate/md80/instruments/cdu_line_XX
                    try:
                        line_num = int(dataref_name.split('_')[-1]) - 1
                        if 0 <= line_num < CDU_ROWS:
                            # Decode the line text
                            if isinstance(value, str):
                                # Handle base64 encoded strings if necessary
                                try:
                                    line_text = base64.b64decode(value).decode('utf-8', errors='ignore')
                                except Exception as e:
                                    logging.warning(f"Base64 decode failed for line {line_num + 1}: {e}")
                                    line_text = value
                            else:
                                line_text = str(value)
                            
                            # Replace null characters with spaces
                            line_text = line_text.replace('\x00', ' ')
                            current_cdu_lines[line_num] = line_text
                    except (ValueError, IndexError) as e:
                        logging.warning(f"Could not parse line number from {dataref_name}: {e}")
                
                # Only send update if display has changed
                if current_cdu_lines != last_sent_lines:
                    last_sent_lines = current_cdu_lines.copy()
                    await queue.put(current_cdu_lines.copy())
                    
        except websockets.exceptions.ConnectionClosed:
            logging.error("X-Plane WebSocket connection lost. Attempting to reconnect...")
            continue
        except Exception as e:
            logging.error(f"Error in dataref handler: {e}")
            await asyncio.sleep(5)  # Wait before retry


async def check_device_availability(device: CduDevice) -> bool:
    """Check if the MobiFlight CDU device is available"""
    try:
        endpoint = device.get_endpoint()
        async with websockets.connect(endpoint) as ws:
            logging.info(f"CDU device available at {endpoint}")
            return True
    except Exception as e:
        logging.warning(f"CDU device not available at {device.get_endpoint()}: {e}")
        return False


async def main():
    """Main entry point for the MD80 MCDU integration"""
    logging.info("Starting MD80 MCDU MobiFlight Integration")
    
    # Check if device is available
    device = CduDevice.MD80_MCDU
    
    if not await check_device_availability(device):
        logging.error("MobiFlight CDU device not available. Please check MobiFlight is running.")
        return
    
    # Create communication queue
    queue = asyncio.Queue()
    
    # Start handler tasks
    tasks = [
        asyncio.create_task(handle_dataref_updates(queue, device)),
        asyncio.create_task(handle_device_update(queue, device))
    ]
    
    logging.info("MD80 MCDU integration started successfully")
    
    try:
        await asyncio.gather(*tasks)
    except KeyboardInterrupt:
        logging.info("Shutting down MD80 MCDU integration")
    except Exception as e:
        logging.error(f"Unexpected error: {e}")
    finally:
        for task in tasks:
            task.cancel()


if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        logging.info("MD80 MCDU integration stopped by user")