using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.FlightSimulator.SimConnect;
//using LockheedMartin.Prepar3D.SimConnect;

namespace MobiFlight.SimConnectMSFS
{
    public class SimConnectCache : SimConnectCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;

        
        private const string STANDARD_EVENT_GROUP = "STANDARD";

        /// User-defined win32 event
        public const int WM_USER_SIMCONNECT = 0x0402;

        /// Window handle
        private IntPtr _handle = new IntPtr(0);

        /// SimConnect object
        private SimConnect m_oSimConnect = null;

        private bool _connected = false;

        public Dictionary<String, List<Tuple<String, uint>>> Events { get; private set; }

        public String PresetFile = null;
        public String PresetFileUser = null;

        private List<LVar> LVars = new List<LVar>();

        /* public void Clear()
         {
             throw new NotImplementedException();
         }*/

        public void SetHandle(IntPtr handle)
        {
            _handle = handle;
        }

        public void ReceiveSimConnectMessage()
        {
            try
            {
                m_oSimConnect?.ReceiveMessage();
            } catch(Exception)
            {
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

        public bool Connect()
        {
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
            _connected = true;

            // register Events
            foreach (string GroupKey in Events.Keys) { 
                foreach (Tuple<string, uint> eventItem in Events[GroupKey])
                {
                    var prefix = "";
                    if (GroupKey != STANDARD_EVENT_GROUP) prefix = "MobiFlight.";
                    (sender).MapClientEventToSimEvent((MOBIFLIGHT_EVENTS) eventItem.Item2, prefix + eventItem.Item1);
                }
            }

            

            Connected?.Invoke(this, null);
        }

        private void InitializeClientDataAreas(SimConnect sender)
        {
            // register Client Data (for LVars)
            (sender).MapClientDataNameToID("MobiFlight.LVars", SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_LVARS);

            // register Client Data (for Commands)
            var ClientDataStringSize = (uint)Marshal.SizeOf(typeof(ClientDataString));
            (sender).MapClientDataNameToID("MobiFlight.Command", SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD);
            (sender).CreateClientData(SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD, 256, SIMCONNECT_CREATE_CLIENT_DATA_FLAG.DEFAULT);

            (sender).MapClientDataNameToID("MobiFlight.Response", SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE);
            (sender).CreateClientData(SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE, 256, SIMCONNECT_CREATE_CLIENT_DATA_FLAG.DEFAULT);

            (sender).AddToClientDataDefinition((SIMCONNECT_DEFINE_ID)0, 0, ClientDataStringSize, 0, 0);
            (sender).RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, ClientDataString>((SIMCONNECT_DEFINE_ID)0);
            (sender).RequestClientData(
                SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE,
                (SIMCONNECT_REQUEST_ID)0,
                (SIMCONNECT_DEFINE_ID)0,
                SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                0,
                0,
                0
            );

            (sender).OnRecvClientData += SimConnectCache_OnRecvClientData; ;
        }

        private void DeInitializeClientDataAreas(SimConnect sender)
        {
            (sender)?.ClearClientDataDefinition((SIMCONNECT_DEFINE_ID) 0);
        }

        internal void Start()
        {
            InitializeClientDataAreas(m_oSimConnect);
        }

        private void SimConnectCache_OnRecvClientData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
            ClientDataValue simData = (ClientDataValue)(data.dwData[0]);
            LVars[(int)(data.dwRequestID - 1)].Data = simData.data;
        }

        internal void Stop()
        {
            ClearLVars();
            WasmModuleClient.Stop(m_oSimConnect);
            DeInitializeClientDataAreas(m_oSimConnect);
        }

        /// The case where the user closes game
        private void SimConnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            Disconnect();
        }

        private void SimConnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
        }

        public bool Disconnect()
        {
            Stop();

            if (m_oSimConnect != null)
            {
                // Dispose serves the same purpose as SimConnect_Close()
                m_oSimConnect.Dispose();
                m_oSimConnect = null;
            }
            _connected = false;

            Closed?.Invoke(this, null);
            
            return true;
        }

        public bool IsConnected()
        {
            return _connected;
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
                Log.Instance.log("SimConnectCache::setEventID: Unknown event ID: " + eventID, LogSeverity.Error);
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

        public float GetLVar(String LVarName)
        {
            float result = 0;
            if (!LVars.Exists(lvar => lvar.Name==LVarName))
            {
                RegisterLVar(LVarName);
                WasmModuleClient.SendWasmCmd(m_oSimConnect, "MF.LVars.Add." + LVarName);
            }

            result = LVars.Find(lvar => lvar.Name == LVarName).Data;

            return result;
        }

        private void RegisterLVar(string lVarName)
        {
            LVars.Add(new LVar() { Name = lVarName });

            m_oSimConnect?.AddToClientDataDefinition(
                (SIMCONNECT_DEFINE_ID)LVars.Count,
                (uint)((LVars.Count - 1) * sizeof(float)),
                sizeof(float),
                0,
                0);

            m_oSimConnect?.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, ClientDataValue>((SIMCONNECT_DEFINE_ID)LVars.Count);

            m_oSimConnect?.RequestClientData(
                SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_LVARS,
                (SIMCONNECT_REQUEST_ID)LVars.Count,
                (SIMCONNECT_DEFINE_ID)LVars.Count,
                SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                0,
                0,
                0
            );
        }

        private void ClearLVars()
        {
            int lVarIdx = 1;
            foreach (var lvar in LVars)
            {
                m_oSimConnect?.ClearClientDataDefinition((SIMCONNECT_DEFINE_ID)lVarIdx);
            }
            LVars.Clear();
        }
    }
}
