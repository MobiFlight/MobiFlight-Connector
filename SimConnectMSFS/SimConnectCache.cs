using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.FlightSimulator.SimConnect;

namespace MobiFlight.SimConnectMSFS
{
    public class SimConnectCache : SimConnectCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler LVarListUpdated;

        private uint MaxClientDataDefinition = 0;

        private const string STANDARD_EVENT_GROUP = "STANDARD";

        private WasmModuleClientData WasmInitClientData;
        private WasmModuleClientData WasmRuntimeClientData;

        // offset 2, because first two definitions are the client response channels
        private const int SIMVAR_DATA_DEFINITION_OFFSET = 2;

        /// The message size for commands and responses
        /// This has to be changed also in SimConnectDefintions
        private const int MOBIFLIGHT_MESSAGE_SIZE = 1024;

        /// User-defined win32 event
        public const int WM_USER_SIMCONNECT = 0x0402;

        /// Window handle
        private IntPtr _handle = new IntPtr(0);

        /// SimConnect object
        private SimConnect m_oSimConnect = null;

        private bool _simConnectConnected = false;
        private bool _wasmConnected = false;

        public Dictionary<String, List<Tuple<String, uint>>> Events { get; private set; }

        public String PresetFile = null;
        public String PresetFileUser = null;

        private List<SimVar> SimVars = new List<SimVar>();
        private List<String> LVars = new List<String>();
        private String ResponseStatus = "NEW";

        public SimConnectCache()
        {
            WasmInitClientData = new WasmModuleClientData()
            {
                NAME = "MobiFlight",
                AREA_SIMVAR_ID = SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_LVARS,
                AREA_COMMAND_ID = SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD,
                AREA_RESPONSE_ID = SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE,
                DATA_DEFINITION_ID = 0,
                RESPONSE_OFFSET = 0    
            };

            WasmRuntimeClientData = new WasmModuleClientData()
            {
                NAME = $"Client_{Environment.MachineName}",
                AREA_SIMVAR_ID = SIMCONNECT_CLIENT_DATA_ID.RUNTIME_LVARS,
                AREA_COMMAND_ID = SIMCONNECT_CLIENT_DATA_ID.RUNTIME_CMD,
                AREA_RESPONSE_ID = SIMCONNECT_CLIENT_DATA_ID.RUNTIME_RESPONSE,
                DATA_DEFINITION_ID = 1,
                RESPONSE_OFFSET = 0
            };
        }

        public void Clear()
        {
             // do nothing
        }

        public void SetHandle(IntPtr handle)
        {
            _handle = handle;
        }

        public void ReceiveSimConnectMessage()
        {
            try
            {
                m_oSimConnect?.ReceiveMessage();
            } catch(Exception e)
            {
                Log.Instance.log(e.Message, LogSeverity.Debug);
                Disconnect();
            }
        }

        private void loadEventPresets()
        {
            if (Events == null) Events = new Dictionary<string, List<Tuple<String, uint>>> ();
            Events.Clear();

            if (PresetFile == null) PresetFile = @"Presets\msfs2020_eventids.cip";
            string[] lines = System.IO.File.ReadAllLines(PresetFile);
            var GroupKey = "Dummy";
            uint EventIdx = 0;

            Events[GroupKey] = new List<Tuple<String, uint>>();
            foreach (string line in lines)
            {
                if (line.StartsWith("//")) continue;

                var cols = line.Split(':');
                if (cols.Length > 1)
                {
                    GroupKey = cols[0];
                    if (Events.ContainsKey(GroupKey)) continue;

                    Events[GroupKey] = new List<Tuple<String, uint>>();
                    continue; // we found a group
                }

                Events[GroupKey].Add(new Tuple<string, uint>(cols[0], EventIdx++));
            }

            if (PresetFileUser == null) PresetFileUser = @"Presets\msfs2020_eventids_user.cip";
            if (System.IO.File.Exists(PresetFileUser)) { 
                lines = System.IO.File.ReadAllLines(PresetFileUser);
                GroupKey = "User";
                Events[GroupKey] = new List<Tuple<String, uint>>();
                foreach (string line in lines)
                {
                    if (line.StartsWith("//")) continue;
                    var cols = line.Split(':');
                    if (cols.Length > 1)
                    {
                        GroupKey = cols[0];
                        if (Events.ContainsKey(GroupKey)) continue;

                        Events[GroupKey] = new List<Tuple<String, uint>>();
                        continue; // we found a group
                    }

                    Events[GroupKey].Add(new Tuple<string, uint>(cols[0], EventIdx++));
                }
            }
        }

        internal void RefreshLVarsList()
        {
            if (m_oSimConnect == null) return;
            WasmModuleClient.GetLVarList(m_oSimConnect, WasmRuntimeClientData);
        }

        public bool Connect()
        {
            // If we have already established a connection with SimConnect
            // but we are still waiting for the WASM module to become available
            // Is called in case _wasmConnected is still false.
            if (_simConnectConnected)
            {
                WasmModuleClient.Ping(m_oSimConnect, WasmInitClientData);
                return true;
            }

            // Here we are only when we try
            // to connect to SimConnect first
            loadEventPresets();

            try
            {
                // The constructor is similar to SimConnect_Open in the native API
                m_oSimConnect = new SimConnect("Simconnect - Simvar test", _handle, WM_USER_SIMCONNECT, null, 0);

                // Listen to connect and quit msgs
                m_oSimConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
                m_oSimConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

                // Listen to exceptions
                m_oSimConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);
            }
            catch (COMException ex)
            {
                return false;
            }

            return true;
        }

        private void SimConnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            _simConnectConnected = true;
            // register Events
            foreach (string GroupKey in Events.Keys) { 
                foreach (Tuple<string, uint> eventItem in Events[GroupKey])
                {
                    var prefix = "";
                    if (GroupKey != STANDARD_EVENT_GROUP) prefix = "MobiFlight.";
                    (sender).MapClientEventToSimEvent((MOBIFLIGHT_EVENTS) eventItem.Item2, prefix + eventItem.Item1);
                }
            }
            // register receive data events
            (sender).OnRecvClientData += SimConnectCache_OnRecvClientData;

            // initialize init client
            InitializeClientDataAreas(sender, WasmInitClientData);

            Connected?.Invoke(this, null);

            WasmModuleClient.Ping(sender, WasmInitClientData);
        }

        private void InitializeClientDataAreas(SimConnect sender, WasmModuleClientData clientData)
        {
            // register Client Data (for SimVars)
            (sender).MapClientDataNameToID($"{clientData.NAME}.LVars", clientData.AREA_SIMVAR_ID);
            (sender).CreateClientData(clientData.AREA_SIMVAR_ID, 4096, SIMCONNECT_CREATE_CLIENT_DATA_FLAG.DEFAULT);

            // register Client Data (for WASM Module Commands)
            (sender).MapClientDataNameToID($"{clientData.NAME}.Command", clientData.AREA_COMMAND_ID);
            (sender).CreateClientData(clientData.AREA_COMMAND_ID, MOBIFLIGHT_MESSAGE_SIZE, SIMCONNECT_CREATE_CLIENT_DATA_FLAG.DEFAULT);

            // register Client Data (for WASM Module Responses)
            (sender).MapClientDataNameToID($"{clientData.NAME}.Response", clientData.AREA_RESPONSE_ID);
            (sender).CreateClientData(clientData.AREA_RESPONSE_ID, MOBIFLIGHT_MESSAGE_SIZE, SIMCONNECT_CREATE_CLIENT_DATA_FLAG.DEFAULT);

            (sender).AddToClientDataDefinition((SIMCONNECT_DEFINE_ID)clientData.DATA_DEFINITION_ID, 
                                                clientData.RESPONSE_OFFSET, MOBIFLIGHT_MESSAGE_SIZE, 0, 0);

            (sender).RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, ResponseString>((SIMCONNECT_DEFINE_ID)0);
            (sender).RequestClientData(
                clientData.AREA_RESPONSE_ID,
                (SIMCONNECT_REQUEST_ID)clientData.DATA_DEFINITION_ID,
                (SIMCONNECT_DEFINE_ID)clientData.DATA_DEFINITION_ID,
                SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                0,
                0,
                0
            );
        }

        internal void Start()
        {
            WasmModuleClient.SetConfig(m_oSimConnect, "MAX_VARS_PER_FRAME", "30", WasmInitClientData);
        }

        private void SimConnectCache_OnRecvClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
            // Init Client Callback
            if (data.dwRequestID == WasmInitClientData.DATA_DEFINITION_ID)
            {
                var simData = (ResponseString)(data.dwData[0]);

                if (simData.Data == "MF.Pong")
                {
                    // Next add runtime client                    
                    WasmModuleClient.AddAdditionalClient(m_oSimConnect, WasmRuntimeClientData.NAME, WasmInitClientData);
                }
                // Runtime client was added
                else if (simData.Data.Contains(WasmRuntimeClientData.NAME))
                {
                    InitializeClientDataAreas(m_oSimConnect, WasmRuntimeClientData);
                    _wasmConnected = true;
                    Connected?.Invoke(this, null);
                }
            }
            // Runtime Client Callback
            else if (data.dwRequestID == WasmRuntimeClientData.DATA_DEFINITION_ID)
            {
                var simData = (ResponseString)(data.dwData[0]);

                if (simData.Data == "MF.LVars.List.Start")
                {
                    ResponseStatus = "LVars.List.Receiving";
                    LVars.Clear();
                }
                else if (simData.Data == "MF.LVars.List.End")
                {
                    ResponseStatus = "LVars.List.Completed";
                    LVarListUpdated?.Invoke(LVars, new EventArgs());
                }
                else if (ResponseStatus == "LVars.List.Receiving")
                {
                    LVars.Add(simData.Data);
                }

#if DEBUG
                // this only for debug compilation
                // it slows down the client immensly.
                Log.Instance.log($"Received {simData.Data}.", LogSeverity.Debug);
#endif

            }
            // SimVar value callback
            else
            {
                var simData = (ClientDataValue)(data.dwData[0]);
                if (SimVars.Count < ((int)(data.dwRequestID) - SIMVAR_DATA_DEFINITION_OFFSET - 1)) 
                    return;
                SimVars[(int)(data.dwRequestID) - SIMVAR_DATA_DEFINITION_OFFSET].Data = simData.data;
            }
        }

        internal void Stop()
        {
            WasmModuleClient.Stop(m_oSimConnect, WasmRuntimeClientData);
            ClearSimVars();
        }

        // The case where the user closes game
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            ConnectionLost?.Invoke(this, null);
            Disconnect();
        }

        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            if (eException == SIMCONNECT_EXCEPTION.ALREADY_CREATED) {
                Log.Instance.log(eException.ToString(), LogSeverity.Debug);
            }
            else
                Log.Instance.log(eException.ToString(), LogSeverity.Error);
        }

        public bool Disconnect()
        {
            ClearSimVars();
            MaxClientDataDefinition = 0;

            if (m_oSimConnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close()
                m_oSimConnect.Dispose();
                m_oSimConnect = null;
            }

            if (_simConnectConnected || _wasmConnected)
            {
                _simConnectConnected = false;
                _wasmConnected = false;

                Closed?.Invoke(this, null);
            }

            return true;
        }

        public bool IsConnected()
        {
            return _wasmConnected;
        }

        public bool IsSimConnectConnected()
        {
            return _simConnectConnected;
        }

        public void setEventID(string eventID)
        {
            if (m_oSimConnect == null || !IsConnected()) return;

            Tuple<String, uint> eventItem = null;

            foreach (String GroupKey in Events.Keys)
            {
                eventItem = Events[GroupKey].Find(x => x.Item1 == eventID);
                if (eventItem != null) break;
            }

            if (eventItem == null)
            {
                Log.Instance.log($"Unknown event ID: {eventID}.", LogSeverity.Error);
                return;
            }
            m_oSimConnect?.TransmitClientEvent(
                    0,
                    (MOBIFLIGHT_EVENTS)eventItem.Item2,
                    1,
                    SIMCONNECT_NOTIFICATION_GROUP_ID.SIMCONNECT_GROUP_PRIORITY_DEFAULT,
                    SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY
            );
        }

        public float GetSimVar(String SimVarName)
        {
            float result = 0;
            if (!IsConnected()) 
                return result;

            if (!SimVars.Exists(lvar => lvar.Name == SimVarName))
            {
                RegisterSimVar(SimVarName);            
                WasmModuleClient.AddSimVar(m_oSimConnect, SimVarName, WasmRuntimeClientData);              
            }

            result = SimVars.Find(lvar => lvar.Name == SimVarName).Data;

            return result;
        }

        public void SetSimVar(String SimVarCode)
        {
            WasmModuleClient.SetSimVar(m_oSimConnect, SimVarCode, WasmRuntimeClientData);   
        }

        private void RegisterSimVar(string SimVarName)
        {
            SimVar NewSimVar = new SimVar() { Name = SimVarName, ID = (uint) (SimVars.Count + SIMVAR_DATA_DEFINITION_OFFSET) };
            SimVars.Add(NewSimVar);

            if (MaxClientDataDefinition >= NewSimVar.ID)
            {
                return;
            }

            MaxClientDataDefinition = NewSimVar.ID;

            uint offset = (uint)(NewSimVar.ID - SIMVAR_DATA_DEFINITION_OFFSET) * sizeof(float);

            m_oSimConnect?.AddToClientDataDefinition(
                (SIMCONNECT_DEFINE_ID)NewSimVar.ID, // data Definition ID
                offset,
                sizeof(float),
                0,
                0);

            m_oSimConnect?.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, ClientDataValue>((SIMCONNECT_DEFINE_ID)NewSimVar.ID);

            m_oSimConnect?.RequestClientData(
                WasmRuntimeClientData.AREA_SIMVAR_ID,          
                (SIMCONNECT_REQUEST_ID)NewSimVar.ID,
                (SIMCONNECT_DEFINE_ID)NewSimVar.ID,
                SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                0,
                0,
                0
            );
        }

        private void ClearSimVars()
        {            
            SimVars.Clear();
            Log.Instance.log("SimVars Cleared.", LogSeverity.Debug);
        }

        #region Not Implemented Yet
        public void setOffset(int offset, byte value)
        {
            throw new NotImplementedException();
        }

        public void setOffset(int offset, short value)
        {
            throw new NotImplementedException();
        }

        public void setOffset(int offset, int value, bool writeOnly = false)
        {
            throw new NotImplementedException();
        }

        public void setOffset(int offset, float value)
        {
            throw new NotImplementedException();
        }

        public void setOffset(int offset, double value)
        {
            throw new NotImplementedException();
        }

        public void setOffset(int offset, string value)
        {
            throw new NotImplementedException();
        }

        public void executeMacro(string macroName, int parameter)
        {
            throw new NotImplementedException();
        }

        public void Write()
        {
            throw new NotImplementedException();
        }

        public void setEventID(int eventID, int param)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
