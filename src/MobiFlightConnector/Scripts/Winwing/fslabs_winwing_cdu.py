import asyncio
import os
import json
import logging
import logging.handlers
import websockets.asyncio.client as ws_client
import http.client

FSL_COLOR_MAP = {
    0: "w",  # black (ignore)
    1: "o",  # cyan
    2: "e",  # gray
    3: "y",  # yellow
    4: "g",  # light green
    5: "m",  # magenta
    6: "a",  # amber
    7: "w",  # white
}

# Special Character Substitutions
subs = {
    28: "°",  
    29: "\u2610",  
    30: "\u2193",  
    31: "\u2192",  
    94: "\u2191",  
    95: "\u2190",  
    110: "\u0394",  
    112: "*",
}

mobi_websocket_connections = {"captain": None, "co-pilot": None}
data_queues = {"3CA1": asyncio.Queue(), "3CA2": asyncio.Queue()}

# Most recent frame per MCDU, kept so it can be re-sent whenever a CDU
# (re)connects -- otherwise a static display stays blank until it next changes.
last_frames = {"3CA1": None, "3CA2": None}

MAX_WS_RETRIES = 3   # <--- Added retry limit


async def fetch_fsl_mcdu(mcdu):
    """Fetch MCDU data using a persistent HTTP connection, avoiding redundant updates."""
    global data_queues
    global last_frames

    last_fetched_data = None
    conn = http.client.HTTPConnection("localhost", 8080, timeout=1)

    while True:
        try:
            conn.request("GET", f"/MCDU/Display/{mcdu}")
            response = conn.getresponse()

            if response.status == 200:
                new_data = json.load(response)

                if "Value" in new_data:
                    parsed_data = parse_fsl_mcdu(new_data["Value"])

                    if parsed_data != last_fetched_data:
                        last_fetched_data = parsed_data
                        last_frames[mcdu] = parsed_data
                        await data_queues[mcdu].put(parsed_data)
            else:
                # Drain the body even on errors, otherwise the persistent
                # HTTPConnection is left in a bad state and the next request
                # raises ResponseNotReady.
                response.read()

        except (http.client.HTTPException, TimeoutError) as ex:
            logging.warning(f"fetch_fsl_mcdu: Connection to FSLabs aircraft not possible. Timeout or HTTP error: {ex}")
            await asyncio.sleep(2)
            conn = http.client.HTTPConnection("localhost", 8080, timeout=1)

        except Exception as ex:
            logging.error(f"fetch_fsl_mcdu: {ex}")
            conn = http.client.HTTPConnection("localhost", 8080, timeout=1)

        await asyncio.sleep(0.3)


async def run_fsl_http_client(mcdu, cdu):
    global mobi_websocket_connections
    global data_queues
    global last_frames

    was_connected = False

    while True:
        conn = mobi_websocket_connections[cdu]

        # Not connected yet (or dropped): keep the latest frame queued and wait.
        if conn is None:
            was_connected = False
            await asyncio.sleep(0.1)
            continue

        # Just (re)connected: the queue may only hold stale intermediate frames
        # (or nothing), so drop the backlog and push the current frame straight
        # away. This is what stops the screen loading blank on startup/reconnect.
        if not was_connected:
            was_connected = True
            while not data_queues[mcdu].empty():
                data_queues[mcdu].get_nowait()
            if last_frames[mcdu] is not None:
                try:
                    await conn.send(last_frames[mcdu])
                except Exception as ex:
                    logging.warning(f"[{cdu}] resend on connect failed, will resync: {ex}")
                    continue

        # Wait for new frames, but time out so a disconnect is noticed promptly.
        try:
            mobi_json = await asyncio.wait_for(data_queues[mcdu].get(), timeout=0.5)
        except asyncio.TimeoutError:
            continue

        conn = mobi_websocket_connections[cdu]
        if mobi_json and conn:
            try:
                await conn.send(mobi_json)
            except Exception as ex:
                # Socket dropped between the check and the send; the ws task
                # will reset the connection and we'll resync on reconnect.
                logging.warning(f"[{cdu}] send failed, will resync on reconnect: {ex}")


async def run_mobiflight_websocket_client(cdu_type):
    """WebSocket client.

    The retry limit applies only until the first successful connection (so a
    missing CDU stops trying); once connected, it reconnects indefinitely so a
    transient drop doesn't permanently kill the CDU.
    """
    global mobi_websocket_connections

    retries = 0
    has_connected_once = False

    while True:
        try:
            # Give up only if we've never connected -- likely no CDU attached.
            if not has_connected_once and retries >= MAX_WS_RETRIES:
                logging.info(
                    f"[{cdu_type}] No CDU detected after {retries} attempts -> stopping task. "
                    f"If you only have one CDU attached, you can ignore this message."
                )
                return

            ws_url = f"ws://localhost:8320/winwing/cdu-{cdu_type}"
            logging.info(
                f"[{cdu_type}] Connecting to MobiFlight WebSocket "
                f"(attempt {retries + 1}) -> {ws_url}"
            )

            connection = await ws_client.connect(ws_url)
            logging.info(f"[{cdu_type}] Connected.")

            # Set the font and let it settle BEFORE exposing the connection, so
            # the first frame isn't sent with the wrong font.
            fontName = "AirbusThales"
            await connection.send(f'{{ "Target": "Font", "Data": "{fontName}" }}')
            logging.info(f"[{cdu_type}] Setting font: {fontName}")
            await asyncio.sleep(1)  # wait a second for font to be set

            mobi_websocket_connections[cdu_type] = connection
            has_connected_once = True
            retries = 0  # reset retry counter

            # Block until the connection drops. We don't expect inbound data;
            # iterating the socket simply unblocks (or raises) when it closes.
            try:
                async for _ in connection:
                    pass
            finally:
                mobi_websocket_connections[cdu_type] = None
                logging.info(f"[{cdu_type}] Connection closed -> will reconnect.")

        except Exception as ex:
            logging.info(f"[{cdu_type}] WebSocket connection failed: {ex}")
            mobi_websocket_connections[cdu_type] = None
            retries += 1
            await asyncio.sleep(2)


def parse_fsl_mcdu(value_list):
    message = {"Target": "Display", "Data": []}

    for row in value_list:
        if row == []:
            message["Data"].append([])
            continue

        if len(row) != 3:
            logging.warning(f"Invalid MCDU row format: {row}")
            continue

        ascii_value, color_value, font_size = row

        if ascii_value == 0:
            char = "-"
        else:
            char = subs.get(ascii_value, chr(ascii_value))

        color = FSL_COLOR_MAP.get(color_value, "w")

        message["Data"].append([char, color, font_size])

    return json.dumps(message, separators=(',', ':'))



# Wrapper to run all tasks for a CDU and cancel fetch/process if websocket exits
async def run_cdu_tasks(mcdu, cdu):
    fetch_task = asyncio.create_task(fetch_fsl_mcdu(mcdu))
    process_task = asyncio.create_task(run_fsl_http_client(mcdu, cdu))
    ws_task = asyncio.create_task(run_mobiflight_websocket_client(cdu))

    done, pending = await asyncio.wait([ws_task], return_when=asyncio.FIRST_COMPLETED)

    # If ws_task is done (returns early), cancel the others
    if ws_task in done:
        fetch_task.cancel()
        process_task.cancel()
        try:
            await fetch_task
        except asyncio.CancelledError:
            pass
        try:
            await process_task
        except asyncio.CancelledError:
            pass


async def main():
    setup_logging(logging.INFO, os.path.join(os.getcwd(), "logs/fslMcduLogging.log"))
    logging.info("----- STARTED FSLWinwingCdu.py (FSLabs MCDU Bridge) ----")

    capt_cdu_task = asyncio.create_task(run_cdu_tasks("3CA1", "captain"))
    fo_cdu_task = asyncio.create_task(run_cdu_tasks("3CA2", "co-pilot"))

    await asyncio.gather(
        capt_cdu_task,
        fo_cdu_task,
    )


def setup_logging(log_level, log_file_full_path):
    os.makedirs(os.path.dirname(log_file_full_path), exist_ok=True)

    log_formatter = logging.Formatter("%(asctime)s [%(levelname)-5.5s]  %(message)s")
    root_logger = logging.getLogger()
    root_logger.setLevel(log_level)

    file_handler = logging.handlers.RotatingFileHandler(
        log_file_full_path, maxBytes=500000, backupCount=7
    )
    file_handler.setFormatter(log_formatter)
    root_logger.addHandler(file_handler)

    console_handler = logging.StreamHandler()
    console_handler.setFormatter(log_formatter)
    root_logger.addHandler(console_handler)


asyncio.run(main())
