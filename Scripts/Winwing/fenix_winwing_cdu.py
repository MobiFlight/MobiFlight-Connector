import asyncio, os, json
import xml.etree.ElementTree as ET
import logging, logging.handlers
import websockets.asyncio.client as ws_client

from gql import Client, gql
from gql.transport.websockets import WebsocketsTransport
from inspect import getsourcefile

subs = {'#': '\u2610',    # ballot box
        '¤': '\u2191',    # up arrow
        '¥': '\u2193',    # down arrow
        '¢': '\u2192',    # right arrow
        '£': '\u2190',    # left arrow
        '&': '\u0394',}   # greek delta for overfly
        
replace_chars =  ['£', '¢', '¥', '¤', '#', '&' ]
format_chars = ['s', 'l', 'a', 'c', 'y', 'w', 'g', 'm']

BASE_PATH = os.path.dirname(os.path.abspath(getsourcefile(lambda:0)))

# global mobi websocket connection
mobi_websocket_connection = None


def setup_logging(log_level, log_file_full_path):
    base_path = os.path.dirname(log_file_full_path)
    if base_path: os.makedirs(base_path, exist_ok=True)
    log_formatter = logging.Formatter("%(asctime)s [%(threadName)-12.12s] [%(levelname)-5.5s]  %(message)s")
    root_logger = logging.getLogger()
    root_logger.setLevel(log_level)
    file_handler = logging.handlers.RotatingFileHandler(log_file_full_path, maxBytes=500000, backupCount=7)
    file_handler.setFormatter(log_formatter)
    root_logger.addHandler(file_handler)
    console_handler = logging.StreamHandler()
    console_handler.setFormatter(log_formatter)
    root_logger.addHandler(console_handler)


def create_mobi_json(xml_string):   
    message =  {}
    message["Target"] = "Display"
    message["Data"] = []    
    formatting = 'w'
    root = ET.fromstring(xml_string)
    for child in root:     
        size = 0 # default row start with size large  
        formatting = 'w' # default row start is white
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
        logging.warning(child.text)   
    logging.debug(message)
    return json.dumps(message, separators=(',', ':')) 


async def run_fenix_graphql_client():
    global mobi_websocket_connection
    transport = WebsocketsTransport(url="ws://localhost:8083/graphql/")
    client = Client(transport=transport, fetch_schema_from_transport=True )
    op_name = "OnDataRefChanged"
    subscription = gql(
            """
            subscription OnDataRefChanged($names: [String!]!) {
                dataRefs(names: $names) {
                name
                value  
                __typename
                }
            }
        """
        )
    params = {"names": ["aircraft.mcdu1.display"]}   
    session = await client.connect_async(reconnecting=True) 
    while (True):
        try:
            async for result in session.subscribe(subscription, params, op_name):
                if "dataRefs" in result:
                    if (result["dataRefs"]["name"] == "aircraft.mcdu1.display"):
                        mobi_json = create_mobi_json(result["dataRefs"]["value"])
                        if mobi_websocket_connection is not None:
                            await mobi_websocket_connection.send(mobi_json)                   
        except Exception as ex: 
            logging.error(f"run_fenix_graphql_client: {ex}")  
        await asyncio.sleep(5)


async def run_mobiflight_websocket_client():
    global mobi_websocket_connection
    uri = "ws://localhost:8320/winwing/cdu-captain"
    while (True):
        try:
            if mobi_websocket_connection == None:
                logging.warning("run_mobiflight_websocket_client: Try to establish MobiFlight connection.")                  
                mobi_websocket_connection = await ws_client.connect(uri)  
                logging.warning("run_mobiflight_websocket_client: MobiFlight CONNECTION ESTABLISHED.")   
            # Wait for disconnection or data
            await mobi_websocket_connection.recv()   
        except Exception as ex:             
            logging.error(f"run_mobiflight_websocket_client: {ex}")  
            logging.error("run_mobiflight_websocket_client: MobiFlight DISCONNECTED.")  
            mobi_websocket_connection = None                                        
        await asyncio.sleep(5)
    

async def main():    
    # Use log level WARNING for standard info, to avoid verbose infos from gql library
    setup_logging(logging.WARNING, os.path.join(BASE_PATH, 'logs/fenixMcduLogging.log'))    
    logging.warning("----STARTED fenix_winwing_cdu.py----")
    fenix_task = asyncio.create_task(run_fenix_graphql_client())
    mobi_task = asyncio.create_task(run_mobiflight_websocket_client())
    await asyncio.gather(fenix_task, mobi_task)
    

# --------- MAIN -----------
asyncio.run(main())
