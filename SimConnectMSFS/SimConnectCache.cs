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
        public enum SIMCONNECT_NOTIFICATION_GROUP_ID
        {
            SIMCONNECT_GROUP_PRIORITY_DEFAULT,
            SIMCONNECT_GROUP_PRIORITY_HIGHEST
        }
        public enum MOBIFLIGHT_EVENTS
        {
            DUMMY
        };

        private bool _connected = false;

        /// User-defined win32 event
        public const int WM_USER_SIMCONNECT = 0x0402;

        /// Window handle
        private IntPtr _handle = new IntPtr(0);

        /// SimConnect object
        private SimConnect m_oSimConnect = null;

        public List<String> Events { get; private set; }

        public String PresetFile = null;

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
            m_oSimConnect?.ReceiveMessage();
        }

        private void _loadEventPresets()
        {
            if (Events == null) Events = new List<string>();
            Events.Clear();
            if (PresetFile == null) PresetFile = @"Presets\msfs2020_eventids.cip";
            string[] lines = System.IO.File.ReadAllLines(PresetFile);

            foreach (string line in lines)
            {
                var cols = line.Split(':');
                if (cols.Length > 1) continue; // we found a group

                Events.Add(cols[0]);
            }
        } 

        public bool Connect()
        {
            _loadEventPresets();

            try
            {
                /// The constructor is similar to SimConnect_Open in the native API
                m_oSimConnect = new SimConnect("Simconnect - Simvar test", _handle, WM_USER_SIMCONNECT, null, 0);

                /// Listen to connect and quit msgs
                m_oSimConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnRecvOpen);
                m_oSimConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnRecvQuit);

                /// Listen to exceptions
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

            int eventIdx = 0;

            // register Events
            foreach (string eventName in Events)
            {
                (sender).MapClientEventToSimEvent((MOBIFLIGHT_EVENTS) eventIdx, "MobiFlight." + eventName);
                eventIdx++;
            }

            Connected?.Invoke(this, null);
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
            if (m_oSimConnect != null)
            {
                /// Dispose serves the same purpose as SimConnect_Close()
                m_oSimConnect.Dispose();
                m_oSimConnect = null;
            }
            _connected = false;

            Closed?.Invoke(this, null);
            
            return true;
        }

        public bool isConnected()
        {
            return _connected;
        }

        public void setEventID(string eventID)
        {
            if (m_oSimConnect == null || !isConnected()) return;

            int eventIdx = Events.FindIndex(x => x==eventID);
            if (eventIdx < 0)
            {
                Log.Instance.log("SimConnectCache::setEventID: Unknown event ID: " + eventID, LogSeverity.Error);
                return;
            }

            //var result = Enum.TryParse(eventID, out MOBIFLIGHT_EVENTS selectedEventId);
            //if (!result) return;

            m_oSimConnect.TransmitClientEvent(
                    0,
                    (MOBIFLIGHT_EVENTS) eventIdx,
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
    }
}
