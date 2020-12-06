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
    class SimConnectCache
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
            AS1000_PFD_VOL_1_INC,
            AS1000_PFD_VOL_1_DEC,
            AS1000_PFD_VOL_2_INC,
            AS1000_PFD_VOL_2_DEC,
            AS1000_PFD_NAV_Switch,
            AS1000_PFD_NAV_Large_INC,
            AS1000_PFD_NAV_Large_DEC,
            AS1000_PFD_NAV_Small_INC,
            AS1000_PFD_NAV_Small_DEC,
            AS1000_PFD_NAV_Push,
            AS1000_PFD_COM_Switch_Long,
            AS1000_PFD_COM_Switch,
            AS1000_PFD_COM_Large_INC,
            AS1000_PFD_COM_Large_DEC,
            AS1000_PFD_COM_Small_INC,
            AS1000_PFD_COM_Small_DEC,
            AS1000_PFD_COM_Push,
            AS1000_PFD_BARO_INC,
            AS1000_PFD_BARO_DEC,
            AS1000_PFD_CRS_INC,
            AS1000_PFD_CRS_DEC,
            AS1000_PFD_CRS_PUSH,
            AS1000_PFD_SOFTKEYS_1,
            AS1000_PFD_SOFTKEYS_2,
            AS1000_PFD_SOFTKEYS_3,
            AS1000_PFD_SOFTKEYS_4,
            AS1000_PFD_SOFTKEYS_5,
            AS1000_PFD_SOFTKEYS_6,
            AS1000_PFD_SOFTKEYS_7,
            AS1000_PFD_SOFTKEYS_8,
            AS1000_PFD_SOFTKEYS_9,
            AS1000_PFD_SOFTKEYS_10,
            AS1000_PFD_SOFTKEYS_11,
            AS1000_PFD_SOFTKEYS_12,
            AS1000_PFD_DIRECTTO,
            AS1000_PFD_ENT_Push,
            AS1000_PFD_CLR_Long,
            AS1000_PFD_CLR,
            AS1000_PFD_MENU_Push,
            AS1000_PFD_FPL_Push,
            AS1000_PFD_PROC_Push,
            AS1000_PFD_FMS_Upper_INC,
            AS1000_PFD_FMS_Upper_DEC,
            AS1000_PFD_FMS_Upper_PUSH,
            AS1000_PFD_FMS_Lower_INC,
            AS1000_PFD_FMS_Lower_DEC,
            AS1000_PFD_RANGE_INC,
            AS1000_PFD_RANGE_DEC,
            AS1000_PFD_JOYSTICK_PUSH,
            AS1000_PFD_ActivateMapCursor,
            AS1000_PFD_DeactivateMapCursor,
            AS1000_PFD_JOYSTICK_RIGHT,
            AS1000_PFD_JOYSTICK_LEFT,
            AS1000_PFD_JOYSTICK_UP,
            AS1000_PFD_JOYSTICK_DOWN,
            PARKING_BRAKES,
            AS430_ENT_Push,
            AS430_RightLargeKnob_Left,
            AS430_RightLargeKnob_Right
        };

        private bool _connected = false;

        /// User-defined win32 event
        public const int WM_USER_SIMCONNECT = 0x0402;

        /// Window handle
        private IntPtr m_hWnd = new IntPtr(0);

        /// SimConnect object
        private SimConnect m_oSimConnect = null;

        /* public void Clear()
         {
             throw new NotImplementedException();
         }*/

        public bool Connect()
        {
            try
            {
                /// The constructor is similar to SimConnect_Open in the native API
                m_oSimConnect = new SimConnect("Simconnect - Simvar test", m_hWnd, WM_USER_SIMCONNECT, null, 0);

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

            // register Events
            foreach (var enumValue in Enum.GetNames(typeof(MOBIFLIGHT_EVENTS)))
            {
                var result = Enum.TryParse(enumValue, out MOBIFLIGHT_EVENTS currentEventId);
                if (!result) continue;
                (sender).MapClientEventToSimEvent(currentEventId, "MobiFlight." + enumValue);
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

            ConnectionLost?.Invoke(this, null);
            Closed?.Invoke(this, null);
            
            return true;
        }

        public bool isConnected()
        {
            return _connected;
        }

        public void sendEventID(string eventID, int param)
        {
            if (m_oSimConnect == null || !isConnected()) return;
            var result = Enum.TryParse(eventID, out MOBIFLIGHT_EVENTS selectedEventId);
            if (!result) return;

            m_oSimConnect.TransmitClientEvent(
                    0,
                    selectedEventId,
                    1,
                    SIMCONNECT_NOTIFICATION_GROUP_ID.SIMCONNECT_GROUP_PRIORITY_DEFAULT,
                    SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY
            );
        }
    }
}
