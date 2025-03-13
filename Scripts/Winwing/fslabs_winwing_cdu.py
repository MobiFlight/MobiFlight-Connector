import asyncio
import os
import json
import logging
import logging.handlers
import websockets.asyncio.client as ws_client
import urllib.request
import time
import http.client

# FSL Color Mapping
FSL_COLOR_MAP = {
    0: "w",  # black (ignore)
    1: "c",  # cyan
    2: "e",  # gray
    3: "y",  # yellow
    4: "g",  # light green
    5: "m",  # magenta
    6: "a",  # amber
    7: "w",  # white
}

# Special Character Substitutions
subs = {
    28: "°",  # Degrees symbol
    29: "\u2610",  # Ballot box
    30: "\u2193",  # Down arrow
    31: "\u2192",  # Right arrow
    94: "\u2191",  # Up arrow
    95: "\u2190",  # Left arrow
    110:"\u0394",  # Overfly Arrow
    112: "*",
    "#": "\u2610",  # Ballot box
    "¤": "\u2191",  # Up arrow
    "¥": "\u2193",  # Down arrow
    "¢": "\u2192",  # Right arrow
    "£": "\u2190",  # Left arrow
}
FSL_LOG_FILE = "fsl_input_log.txt"  # Path to log file

FSL_API_URL = "http://localhost:8080/MCDU/Display/3CA1"
MOBIFLIGHT_WS_URI = "ws://localhost:8320/winwing/cdu-captain"
mobi_websocket_connection = None
data_queue = asyncio.Queue()  # Thread-safe async queue for MCDU updates

async def fetch_fsl_mcdu():
    """Fetch MCDU data using a persistent HTTP connection, avoiding redundant updates."""
    global last_mcdu_data
    last_fetched_data = None

    conn = http.client.HTTPConnection("localhost", 8080, timeout=1)  # Persistent connection

    while True:
        try:
            conn.request("GET", "/MCDU/Display/3CA1")
            response = conn.getresponse()

            if response.status == 200:
                new_data = json.load(response)        
                    
                if "Value" in new_data:
                    parsed_data = parse_fsl_mcdu(new_data["Value"])

                    if parsed_data != last_fetched_data:
                        last_fetched_data = parsed_data
                        await data_queue.put(parsed_data)  # Send to WebSocket
                    #else:
                        #logging.info("No MCDU data change, skipping update.")

        except (http.client.HTTPException, TimeoutError) as ex:
            logging.error(f"fetch_fsl_mcdu: Timeout or HTTP error: {ex}")
            await asyncio.sleep(2)  # Increase delay after failure
            conn = http.client.HTTPConnection("localhost", 8080, timeout=1)  # Reset connection

        except Exception as ex:
            logging.error(f"fetch_fsl_mcdu: {ex}")
            conn = http.client.HTTPConnection("localhost", 8080, timeout=1)  # Reset connection

        await asyncio.sleep(0.3)  # Fetch every 50ms

async def run_fsl_http_client():
    """Measure the time it takes to send updates to MobiFlight."""
    global mobi_websocket_connection

    while True:
        mobi_json = await data_queue.get()

        if mobi_json and mobi_websocket_connection:
            await mobi_websocket_connection.send(mobi_json)



async def run_mobiflight_websocket_client():
    """Measure WebSocket connection times."""
    global mobi_websocket_connection

    while True:

        try:
            if mobi_websocket_connection is None:
                logging.warning("Connecting to MobiFlight WebSocket...")
                mobi_websocket_connection = await ws_client.connect(MOBIFLIGHT_WS_URI)
                logging.warning("Connected to MobiFlight WebSocket.")

            await asyncio.sleep(0.05)

        except Exception as ex:
            logging.error(f"WebSocket Error: {ex}")
            mobi_websocket_connection = None
            await asyncio.sleep(2)

    
def parse_fsl_mcdu(value_list):
    """Convert FSL JSON to MobiFlight format while ensuring correct data structure."""
    message = {"Target": "Display", "Data": []}

    for row in value_list:
        if row == []:  # Preserve empty cells
            message["Data"].append([])
            continue

        # Ensure the row contains exactly 3 elements (ASCII, color, font size)
        if len(row) != 3:
            logging.warning(f"Invalid MCDU row format: {row}, expected 3 elements or []")
            continue  # Skip malformed data

        ascii_value, color_value, font_size = row
        
        if ascii_value == 0:  # Null character should be replaced
            char = "-"  # Replace with correct character
        else:
            char = subs.get(ascii_value, chr(ascii_value))  # Convert ASCII to mapped symbol or normal char        c
            
        color = FSL_COLOR_MAP.get(color_value, "w")  # Default to white if unknown

        if char == "\u0000":  # Double-check for null character
            char = "-"
        entry = [char, color, font_size]  # Ensure structure is correct
        message["Data"].append(entry)

    return json.dumps(message, separators=(',', ':'))  # Use compact JSON format for efficiency

async def main():
    """Main function to start both tasks."""
    setup_logging(logging.WARNING, os.path.join(os.getcwd(), "logs/fslMcduLogging.log"))
    logging.warning("---- STARTED FSLWinwingCduCaptain.py ----")

    # Start both tasks
    fetch_task = asyncio.create_task(fetch_fsl_mcdu())
    process_task = asyncio.create_task(run_fsl_http_client())
    ws_task = asyncio.create_task(run_mobiflight_websocket_client())

    await asyncio.gather(fetch_task, process_task, ws_task)

def setup_logging(log_level, log_file_full_path):
    """Setup logging to both file and console."""
    os.makedirs(os.path.dirname(log_file_full_path), exist_ok=True)

    log_formatter = logging.Formatter("%(asctime)s [%(levelname)-5.5s]  %(message)s")
    root_logger = logging.getLogger()
    root_logger.setLevel(logging.INFO)  # <--- Change from WARNING to INFO

    file_handler = logging.handlers.RotatingFileHandler(log_file_full_path, maxBytes=500000, backupCount=7)
    file_handler.setFormatter(log_formatter)
    root_logger.addHandler(file_handler)

    console_handler = logging.StreamHandler()
    console_handler.setFormatter(log_formatter)
    root_logger.addHandler(console_handler)


# Run the async event loop
asyncio.run(main())
