import asyncio, ctypes, json, logging, os, struct
from ctypes import wintypes
from typing import Any
import websockets.asyncio.client as ws_client
from SimConnect import SimConnect, Enum
from SimConnect.Enum import SIMCONNECT_CLIENT_DATA_ID, SIMCONNECT_RECV_ID, SIMCONNECT_RECV_CLIENT_DATA

# --- Config ---
CAPTAIN_MCDU_URL = "ws://localhost:8320/winwing/cdu-captain"
FO_MCDU_URL = "ws://localhost:8320/winwing/cdu-co-pilot"

MCDU_COLOR_MAP = {0:"w",1:"r",2:"o",3:"g",4:"y"}

MCDU_COLUMNS, MCDU_ROWS = 24, 14
FA50_CDU_COLUMNS, FA50_CDU_ROWS = 22, 10
MCDU_CHARS = MCDU_COLUMNS * MCDU_ROWS

MCDU_DATA_SIZE = (22 * 3 * 10) + 10
FA50_MCDU_CPT_NAME, FA50_CPT_MCDU_CLIENT_DATA_ID = "FA50_CDU_LEFT", 1
FA50_MCDU_FO_NAME, FA50_FO_MCDU_CLIENT_DATA_ID = "FA50_CDU_RIGHT", 2
FA50_MCDU_CPT_DEFINITION, FA50_MCDU_FO_DEFINITION = 3, 4

# --- SimConnect Wrapper ---
class SimConnectMobiFlight(SimConnect):
    def __init__(self, auto_connect=True, library_path=None):
        self.client_data_handlers = []
        super().__init__(auto_connect, library_path) if library_path else super().__init__(auto_connect)
        self.dll.MapClientDataNameToID.argtypes = [wintypes.HANDLE, ctypes.c_char_p, SIMCONNECT_CLIENT_DATA_ID]

    def register_client_data_handler(self, h): 
        if h not in self.client_data_handlers: self.client_data_handlers.append(h)

    def unregister_client_data_handler(self, h): 
        if h in self.client_data_handlers: self.client_data_handlers.remove(h)

    def my_dispatch_proc(self, pData, cbData, pContext):
        if not pData: return
        if pData.contents.dwID == SIMCONNECT_RECV_ID.SIMCONNECT_RECV_ID_CLIENT_DATA:
            client_data = ctypes.cast(pData, ctypes.POINTER(SIMCONNECT_RECV_CLIENT_DATA)).contents
            for handler in self.client_data_handlers:
                handler(client_data)
        else:
            super().my_dispatch_proc(pData, cbData, pContext)

# --- MobiFlight WebSocket Client ---
class MobiFlightClient:
    def __init__(self, uri:str, max_retries=3):
        self.uri, self.max_retries, self.retries = uri, max_retries, 0
        self.connected, self.websocket = asyncio.Event(), None
        self._was_connected, self.last_data = False, None

    async def run(self):
        while self.retries < self.max_retries:
            try:
                logging.info(f"Connecting to {self.uri}")
                self.websocket = await ws_client.connect(self.uri, ping_interval=None)                
                await self.websocket.send(json.dumps({"Target":"Font","Data":"Collins"}))
                logging.info(f"Setting font: Collins")
                await asyncio.sleep(1) # wait a second for font to be set
                self.connected.set()
                if self._was_connected and self.last_data: await self.send(self.last_data)
                self._was_connected, self.retries = True, 0
                async for _ in self.websocket: pass
            except Exception as e:
                self.retries += 1               
                logging.info(f"WebSocket failure: {e} ({self.retries}/{self.max_retries})")
                self.websocket = None
                self.connected.clear()
            await asyncio.sleep(5)     
        logging.info("Max retries reached. Giving up connecting to MobiFlight at %s. If you only have one CDU attached, you can ignore this message.", self.uri)
        self.connected.set()
        
    async def send(self, data:str):
        if self.websocket and self.connected.is_set():
            await self.websocket.send(data)
            self.last_data = data
            
    async def close(self):
        if self.websocket: 
            await self.websocket.close()
            self.websocket = None
            self.connected.clear()
# --- Data Conversion ---
def create_mobi_json(data:bytes)->str:
    out = {"Target":"Display","Data":[[] for _ in range(MCDU_CHARS)]}
    for r in range(FA50_CDU_ROWS):
        if r > 0 and r < 6:
            left,right = data[(FA50_CDU_COLUMNS * FA50_CDU_ROWS * 3) + ((r-1) * 2)], data[(FA50_CDU_COLUMNS * FA50_CDU_ROWS * 3) + ((r-1) * 2)+1]
            row_offset = ((r-1) * 2) + 1
            if left == 1:
                 idxo = (MCDU_COLUMNS * (row_offset))
                 out["Data"][idxo] = ['<', 'w', 1]
            if right == 1:
                idxo = (MCDU_COLUMNS * (1+row_offset)) -1
                out["Data"][idxo] = ['>', 'w', 1]
        for c in range(FA50_CDU_COLUMNS):
            idx = (r * (FA50_CDU_COLUMNS * 3)) + (c * 3)
            try:
                sym, color, s = chr(data[idx]), data[idx+1], data[idx+2]
                if sym in (" ","\0", "\n"): continue
                idxo = (MCDU_COLUMNS * r) + (c +1)
                out["Data"][idxo] = [sym,  "a" if s == 1 else MCDU_COLOR_MAP.get(color, "w"), 0]
            except Exception as e:  
                logging.error(f"Error processing character at index {idx}: {e}")
    return json.dumps(out)

# --- MCDU Client ---
class Fa50MCDUClient:
    def __init__(self, sc:SimConnectMobiFlight, uri:str, def_id:int, client_area_name:str, client_area_id:int):
        self.sc = sc
        self.uri = uri
        self.def_id = def_id
        self.CA_NAME = client_area_name
        self.CA_ID = client_area_id
        self.mobiflight, self.last_data, self.loop = MobiFlightClient(uri), None, None
        logging.info(f"Connecting to {self.uri}")

    def setup(self):
        try:
            sc = self.sc
            h = sc.hSimConnect
            sc.dll.MapClientDataNameToID(h, self.CA_NAME.encode(), self.CA_ID)
            sc.dll.AddToClientDataDefinition(h, self.def_id, 0, MCDU_DATA_SIZE, 0, 0)
            sc.dll.RequestClientData(h, self.CA_ID, self.def_id, self.def_id,
                Enum.SIMCONNECT_CLIENT_DATA_PERIOD.SIMCONNECT_CLIENT_DATA_PERIOD_VISUAL_FRAME,
                Enum.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.SIMCONNECT_CLIENT_DATA_REQUEST_FLAG_DEFAULT,0,0,0)
            sc.register_client_data_handler(self.on_data)
            return True
        except Exception as e: 
            logging.error(f"SimConnect setup failed: {e}")
            return False

    def on_data(self, d:Any):
        if d.dwDefineID!=self.def_id or not hasattr(d,"dwData"): return
        data = ctypes.string_at(d.dwData, MCDU_DATA_SIZE)
        json_data=create_mobi_json(data)
        if json_data==self.last_data: return
        self.last_data=json_data
        asyncio.run_coroutine_threadsafe(self.mobiflight.send(json_data), self.loop)

    async def run(self):
        try:
            self.loop=asyncio.get_running_loop()
            task_ws=asyncio.create_task(self.mobiflight.run())
            await self.mobiflight.connected.wait()
            if self.mobiflight.retries>=self.mobiflight.max_retries: return
            if not self.setup(): return
            await asyncio.gather(task_ws)
        finally:
            await self.mobiflight.close()
            
# --- Main ---
if __name__ == "__main__":
    logging.basicConfig(
        level=os.environ.get("LOGLEVEL", "DEBUG").upper(),
        format='%(levelname)s:%(message)s'
    )
    sc=SimConnectMobiFlight()

    mcdu_cpt=Fa50MCDUClient(sc, CAPTAIN_MCDU_URL, FA50_MCDU_CPT_DEFINITION, FA50_MCDU_CPT_NAME, FA50_CPT_MCDU_CLIENT_DATA_ID)
    mcdu_fo=Fa50MCDUClient(sc, FO_MCDU_URL, FA50_MCDU_FO_DEFINITION, FA50_MCDU_FO_NAME, FA50_FO_MCDU_CLIENT_DATA_ID)

    async def main(): 
        await asyncio.gather(
            mcdu_cpt.run(),
            mcdu_fo.run()
        )
    try: asyncio.run(main())
    except KeyboardInterrupt: pass
    finally: sc.exit()