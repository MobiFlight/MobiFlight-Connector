import asyncio, json
import xml.etree.ElementTree as ET
import logging
import websockets.asyncio.client as ws_client
import websockets.exceptions
from gql import Client, gql
from gql.transport.websockets import WebsocketsTransport

subs = {'#': '☐',    # ballot box \u2610
        '¤': '↑',    # up arrow    \u2191
        '¥': '↓',    # down arrow  \u2193
        '¢': '→',    # right arrow \u2192
        '£': '←',    # left arrow  \u2190
        '&': 'Δ',}   # greek delta for overfly \u0394

replace_chars =  ['£', '¢', '¥', '¤', '#', '&' ]
format_chars = ['s', 'l', 'a', 'c', 'y', 'w', 'g', 'm']

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
        logging.debug(child.text)   
    logging.debug(message)
    return json.dumps(message, separators=(',', ':')) 


async def run_fenix_graphql_client(mobi_client1, mobi_client2):
    await asyncio.sleep(1)
    transport = WebsocketsTransport(url="ws://localhost:8083/graphql/")
    client = Client(transport=transport)
    op_name = "OnDataRefChanged"
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
    params = {"names": ["aircraft.mcdu1.display", "aircraft.mcdu2.display"]}   
    session = await client.connect_async(reconnecting=True) 
    while (True):
        try:
            async for result in session.subscribe(subscription, variable_values=params, operation_name=op_name):
                if "dataRefs" in result:
                    if (result["dataRefs"]["name"] == "aircraft.mcdu1.display"):
                        mobi_json = create_mobi_json(result["dataRefs"]["value"])
                        await mobi_client1.send_json_data(mobi_json)
                    elif (result["dataRefs"]["name"] == "aircraft.mcdu2.display"):
                        mobi_json = create_mobi_json(result["dataRefs"]["value"])
                        await mobi_client2.send_json_data(mobi_json)              
        except Exception as ex: 
            logging.error(f"run_fenix_graphql_client: {ex}")  
        await asyncio.sleep(5)


class Mobiflight_Client:

    def __init__(self, uri, id):
        self.uri = uri
        self.id = id
        self.websocket_connection = None

    async def run_mobiflight_websocket_client(self):  
        while (True):
            try:
                if self.websocket_connection == None:                                
                    self.websocket_connection = await ws_client.connect(self.uri)  
                    logging.info(f"Established connection to MobiFlight websocket interface for {self.id}.")                       
                    # Load font                                        
                    fontName = "AirbusThales"
                    await self.websocket_connection.send(f'{{ "Target": "Font", "Data": "{fontName}" }}')
                    logging.info(f"Setting font: {fontName}")
                    await asyncio.sleep(1) # wait a second for font to be set
                # Wait for disconnection or data
                await self.websocket_connection.recv()    
            except websockets.exceptions.InvalidStatus as invalid:      
                self.websocket_connection = None
                if invalid.response.status_code == 501:
                    logging.info(f"MobiFlight websocket interface for {self.id} not active. Stop trying.")
                    # Break and stop for that CDU
                    break
                else:
                    logging.error(f"Error on trying to connect to MobiFlight websocket interface for {self.id}. Will retry: {invalid}")    
            except Exception as ex:            
                logging.error(f"Error on trying to connect to MobiFlight websocket interface for {self.id}. Will retry: {ex}")                  
                self.websocket_connection = None                                        
            await asyncio.sleep(5)

    async def send_json_data(self, mobi_json):
        if self.websocket_connection is not None:
            await self.websocket_connection.send(mobi_json)  

    

async def main():   
    logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')  
    log = logging.getLogger("gql.transport.websockets")
    log.setLevel(logging.WARNING)   
    logging.info("----STARTED fenix_winwing_cdu.py----")   
    client1 = Mobiflight_Client("ws://localhost:8320/winwing/cdu-captain", "CDU-CAPTAIN")
    client2 = Mobiflight_Client("ws://localhost:8320/winwing/cdu-co-pilot", "CDU-CO-PILOT")  
    mobi_task = asyncio.create_task(client1.run_mobiflight_websocket_client())
    mobi_task2 = asyncio.create_task(client2.run_mobiflight_websocket_client())
    fenix_task = asyncio.create_task(run_fenix_graphql_client(client1, client2))
    await asyncio.gather(fenix_task, mobi_task, mobi_task2)
    

# --------- MAIN -----------
asyncio.run(main())
