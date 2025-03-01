using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HidSharp.Utility;
using Microsoft.FlightSimulator.SimConnect;
using System.Text.RegularExpressions;
using MobiFlight.Base;
using Newtonsoft.Json;

namespace MobiFlight.SimConnectMSFS
{
    // String properties must be packed inside of a struct
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct StringData
    {
        // this is how you declare a fixed size string
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public String sValue;

        // other definitions can be added to this struct
        // ...
    };

    public class SimConnectCache : SimConnectCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler LVarListUpdated;
        public event EventHandler<String> AircraftChanged;
        public event EventHandler<String> AircraftPathChanged;

        private uint MaxClientDataDefinition = 0;

        private const string STANDARD_EVENT_GROUP = "STANDARD";

        private WasmModuleClientData WasmInitClientData;
        private WasmModuleClientData WasmRuntimeClientData;

        // offset 4, because first two definitions are the client response channels, aircraft name and path
        private const int SIMVAR_DATA_DEFINITION_OFFSET = 4;

        /// The message size for commands and responses
        /// This has to be changed also in SimConnectDefintions
        private const int MOBIFLIGHT_MESSAGE_SIZE = 1024;

        /// Constants regarding the handling of string SimVars
        private const int MOBIFLIGHT_STRINGVAR_ID_OFFSET = 10000;
        private const int MOBIFLIGHT_STRINGVAR_SIZE = 128;
        private const int MOBIFLIGHT_STRINGVAR_MAX_AMOUNT = 64;
        // This must not exceed SIMCONNECT_CLIENTDATA_MAX_SIZE!
        private const int MOBIFLIGHT_STRINGVAR_DATAAREA_SIZE = MOBIFLIGHT_STRINGVAR_SIZE * MOBIFLIGHT_STRINGVAR_MAX_AMOUNT;

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
        private List<StringSimVar> StringSimVars = new List<StringSimVar>();
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
                AREA_STRINGSIMVAR_ID = SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_STRINGVAR,
                DATA_DEFINITION_ID = SIMCONNECT_DEFINE_ID.INIT_CLIENT,
                RESPONSE_OFFSET = 0    
            };

            string hashMachineName = Environment.MachineName.GenerateSimpleHash().ToString();          
            WasmRuntimeClientData = new WasmModuleClientData()
            {
                NAME = $"Client_{hashMachineName}",
                AREA_SIMVAR_ID = SIMCONNECT_CLIENT_DATA_ID.RUNTIME_LVARS,
                AREA_COMMAND_ID = SIMCONNECT_CLIENT_DATA_ID.RUNTIME_CMD,
                AREA_RESPONSE_ID = SIMCONNECT_CLIENT_DATA_ID.RUNTIME_RESPONSE,
                AREA_STRINGSIMVAR_ID = SIMCONNECT_CLIENT_DATA_ID.RUNTIME_STRINGVAR,
                DATA_DEFINITION_ID = SIMCONNECT_DEFINE_ID.RUNTIME_CLIENT,
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
                if (m_oSimConnect == null)
                {
                    // The constructor is similar to SimConnect_Open in the native API
                    m_oSimConnect = new SimConnect("Simconnect - MobiFlight", _handle, WM_USER_SIMCONNECT, null, 0);

                    // Listen to connect and quit msgs
                    m_oSimConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
                    m_oSimConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

                    // Now the sim is running, request information on the user aircraft
                    m_oSimConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(SimConnect_RecvSimobjectData);
                    // Register aircraft name
                    m_oSimConnect.AddToDataDefinition(SIMCONNECT_DEFINE_ID.AIRCRAFT_NAME, "Title", null, SIMCONNECT_DATATYPE.STRING128, 0, SimConnect.SIMCONNECT_UNUSED);
                    m_oSimConnect.RequestDataOnSimObject((SIMCONNECT_REQUEST_ID)SIMCONNECT_DEFINE_ID.AIRCRAFT_NAME, SIMCONNECT_DEFINE_ID.AIRCRAFT_NAME, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_PERIOD.SECOND, SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
                    m_oSimConnect.RegisterDataDefineStruct<StringData>(SIMCONNECT_DEFINE_ID.AIRCRAFT_NAME);

                    // Listen to exceptions
                    m_oSimConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnRecvException);

                    m_oSimConnect.OnRecvEventFilename += SimConnect_OnRecvEventFilename;
                    m_oSimConnect.SubscribeToSystemEvent(MOBIFLIGHT_EVENTS.AIRCRAFT_LOADED, "AircraftLoaded");

                    m_oSimConnect.OnRecvSystemState += SimConnect_OnRecvSystemState;
                    m_oSimConnect.RequestSystemState((SIMCONNECT_REQUEST_ID)SIMCONNECT_DEFINE_ID.AIRCRAFT_PATH, "AircraftLoaded");
                    
                    Log.Instance.log("SimConnect (MSFS) instantiated", LogSeverity.Debug);
                }
            }
            catch (COMException ex)
            {
                m_oSimConnect = null;
                return false;
            }

            return true;
        }

        private void SimConnect_OnRecvSystemState(SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data)
        {
            switch (data.dwRequestID)
            {
                case (uint)((SIMCONNECT_REQUEST_ID)SIMCONNECT_DEFINE_ID.AIRCRAFT_PATH):
                    AircraftPathChanged?.Invoke(this, data.szString);                    
                    break;
            }
        }

        private void SimConnect_OnRecvEventFilename(SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data)
        {
            switch (data.uEventID)
            {
                case (uint)MOBIFLIGHT_EVENTS.AIRCRAFT_LOADED:
                    AircraftPathChanged?.Invoke(this, data.szFileName);                                          
                    break;
            }
        }

        private void SimConnect_RecvSimobjectData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            var title = (StringData) data.dwData[0];
            AircraftChanged?.Invoke(this, title.sValue);
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

            // register Client Data (for String-SimVars)
            (sender).MapClientDataNameToID($"{clientData.NAME}.StringVars", clientData.AREA_STRINGSIMVAR_ID);
            (sender).CreateClientData(clientData.AREA_STRINGSIMVAR_ID, MOBIFLIGHT_STRINGVAR_DATAAREA_SIZE, SIMCONNECT_CREATE_CLIENT_DATA_FLAG.DEFAULT);

            (sender).AddToClientDataDefinition(clientData.DATA_DEFINITION_ID, 
                                                clientData.RESPONSE_OFFSET, MOBIFLIGHT_MESSAGE_SIZE, 0, 0);

            (sender).RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, ResponseString>(clientData.DATA_DEFINITION_ID);
            (sender).RequestClientData(
                clientData.AREA_RESPONSE_ID,
                (SIMCONNECT_REQUEST_ID)clientData.DATA_DEFINITION_ID,
                clientData.DATA_DEFINITION_ID,
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
            try
            {
                // Init Client Callback
                if (data.dwRequestID == (uint)WasmInitClientData.DATA_DEFINITION_ID)
                {
                    var simData = (ResponseString)(data.dwData[0]);

                    if (simData.Data == "MF.Pong")
                    {
                        if (!_wasmConnected)
                        {
                            // Next add runtime client                    
                            WasmModuleClient.AddAdditionalClient(m_oSimConnect, WasmRuntimeClientData.NAME, WasmInitClientData);
                        }
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
                else if (data.dwRequestID == (uint)WasmRuntimeClientData.DATA_DEFINITION_ID)
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
                else if (data.dwRequestID > MOBIFLIGHT_STRINGVAR_ID_OFFSET) // -> is string SimVar
                {
                    var simData = (ClientDataStringValue)(data.dwData[0]);
                    var simVarIndex = (int)(data.dwRequestID - MOBIFLIGHT_STRINGVAR_ID_OFFSET - 1);

                    if (StringSimVars.Count <= simVarIndex || simVarIndex < 0) return;
                    StringSimVars[simVarIndex].Data = simData.data;
                }
                else // -> Must be float SimVar
                {
                    var simData = (ClientDataValue)(data.dwData[0]);
                    var simVarIndex = (int)(data.dwRequestID) - SIMVAR_DATA_DEFINITION_OFFSET;

                    if (SimVars.Count <= simVarIndex || simVarIndex < 0) return;
                    SimVars[simVarIndex].Data = simData.data;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Exception in SimConnect Callback: {ex.Message}", LogSeverity.Error);
                throw; // Exception is caught in SimConnect
            }
        }

        internal void Stop()
        {
            try
            {
                WasmModuleClient.Stop(m_oSimConnect, WasmRuntimeClientData);
            }
            catch (Exception e)
            {
                Log.Instance.log($"Unable to stop WasmModule: {e.Message} {JsonConvert.SerializeObject(WasmRuntimeClientData)}", LogSeverity.Error);
            }

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

        public FSUIPCOffsetType GetSimVar(String simVarName, out String stringVal, out double floatVal)
        {
            FSUIPCOffsetType simVarType = FSUIPCOffsetType.Float;
            bool isFloat = false;
            bool isString = false;

            stringVal = "0";
            floatVal = 0.0F;

            if (!IsConnected()) 
                return simVarType;

            if (simVarName == null)
                return simVarType;  

            isFloat = SimVars.Exists(lvar => lvar.Name == simVarName);
            if(!isFloat)
            {
                isString = StringSimVars.Exists(lvar => lvar.Name == simVarName);

                if (!isString)
                {
                    simVarType = RegisterSimVar(simVarName);
                    if (simVarType == FSUIPCOffsetType.String)
                    {
                        WasmModuleClient.AddStringSimVar(m_oSimConnect, simVarName, WasmRuntimeClientData);
                    }
                    else
                    {
                        WasmModuleClient.AddSimVar(m_oSimConnect, simVarName, WasmRuntimeClientData);
                    }
                }
                else
                {
                    simVarType = FSUIPCOffsetType.String;
                }
            }

            if(simVarType == FSUIPCOffsetType.Float)
            {
                floatVal = SimVars.Find(lvar => lvar.Name == simVarName).Data;
            }
            else
            {
                stringVal = StringSimVars.Find(lvar => lvar.Name == simVarName).Data;
                stringVal = (stringVal) == null ? "0" : stringVal;
            }

            return simVarType;
        }

        public void SetSimVar(String SimVarCode)
        {
            WasmModuleClient.SetSimVar(m_oSimConnect, SimVarCode, WasmRuntimeClientData);   
        }

        private FSUIPCOffsetType RegisterSimVar(string SimVarName)
        {
            // Matches presets like "(A:TITLE,String)" in different variations. These will most likely be of type String an will therefore be treated as String.
            Match stringPreset = Regex.Match(SimVarName, "\\(.*A.*:.*,.*String.*\\)", RegexOptions.IgnoreCase);
            FSUIPCOffsetType simVarType = stringPreset.Success ? FSUIPCOffsetType.String : FSUIPCOffsetType.Float;

            if(simVarType == FSUIPCOffsetType.Float)
            {
                // Register SimVar as float
                SimVar newSimVar = new SimVar() { Name = SimVarName, ID = (uint)(SimVars.Count + SIMVAR_DATA_DEFINITION_OFFSET) };
                SimVars.Add(newSimVar);
                if(MaxClientDataDefinition < (SimVars.Count + SIMVAR_DATA_DEFINITION_OFFSET - 1 + StringSimVars.Count))
                {
                    // Register SimVar as float
                    m_oSimConnect?.AddToClientDataDefinition(
                        (SIMCONNECT_DEFINE_ID)newSimVar.ID,
                        (uint)((newSimVar.ID - SIMVAR_DATA_DEFINITION_OFFSET) * sizeof(float)),
                        sizeof(float),
                        0,
                        0);

                    m_oSimConnect?.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, ClientDataValue>((SIMCONNECT_DEFINE_ID)newSimVar.ID);

                    m_oSimConnect?.RequestClientData(
                        WasmRuntimeClientData.AREA_SIMVAR_ID,
                        (SIMCONNECT_REQUEST_ID)newSimVar.ID,
                        (SIMCONNECT_DEFINE_ID)newSimVar.ID,
                        SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                        SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                        0,
                        0,
                        0
                    );

                    MaxClientDataDefinition = (uint)(SimVars.Count + SIMVAR_DATA_DEFINITION_OFFSET - 1 + StringSimVars.Count);
                }
            }
            else
            {
                // Register SimVar as string (different ID-offset, size and a separate data-area)
                StringSimVar newStringSimVar = new StringSimVar() { Name = SimVarName, ID = (uint)StringSimVars.Count + 1 + MOBIFLIGHT_STRINGVAR_ID_OFFSET };
                StringSimVars.Add(newStringSimVar);
                if (MaxClientDataDefinition < (SimVars.Count + SIMVAR_DATA_DEFINITION_OFFSET - 1 + StringSimVars.Count))
                {
                    m_oSimConnect?.AddToClientDataDefinition(
                        (SIMCONNECT_DEFINE_ID)(newStringSimVar.ID),
                        (uint)((StringSimVars.Count - 1) * MOBIFLIGHT_STRINGVAR_SIZE),
                        MOBIFLIGHT_STRINGVAR_SIZE,
                        0,
                        0);

                    m_oSimConnect?.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, ClientDataStringValue>((SIMCONNECT_DEFINE_ID)newStringSimVar.ID);

                    m_oSimConnect?.RequestClientData(
                        WasmRuntimeClientData.AREA_STRINGSIMVAR_ID,
                        (SIMCONNECT_REQUEST_ID)(newStringSimVar.ID),
                        (SIMCONNECT_DEFINE_ID)(newStringSimVar.ID),
                        SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                        SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                        0,
                        0,
                        0
                    );
    
                    MaxClientDataDefinition = (uint)(SimVars.Count + SIMVAR_DATA_DEFINITION_OFFSET - 1 + StringSimVars.Count);
                }
            }

            return simVarType;
        }

        private void ClearSimVars()
        {            
            SimVars.Clear();
            StringSimVars.Clear();
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
