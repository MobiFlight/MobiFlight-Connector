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
            // G1000 PFD:GROUP
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
            // G1000 MFD:GROUP
            AS1000_MFD_VOL_1_INC,
            AS1000_MFD_VOL_1_DEC,
            AS1000_MFD_VOL_2_INC,
            AS1000_MFD_VOL_2_DEC,
            AS1000_MFD_NAV_Switch,
            AS1000_MFD_NAV_Large_INC,
            AS1000_MFD_NAV_Large_DEC,
            AS1000_MFD_NAV_Small_INC,
            AS1000_MFD_NAV_Small_DEC,
            AS1000_MFD_NAV_Push,
            AS1000_MFD_COM_Switch_Long,
            AS1000_MFD_COM_Switch,
            AS1000_MFD_COM_Large_INC,
            AS1000_MFD_COM_Large_DEC,
            AS1000_MFD_COM_Small_INC,
            AS1000_MFD_COM_Small_DEC,
            AS1000_MFD_COM_Push,
            AS1000_MFD_BARO_INC,
            AS1000_MFD_BARO_DEC,
            AS1000_MFD_CRS_INC,
            AS1000_MFD_CRS_DEC,
            AS1000_MFD_CRS_PUSH,
            AS1000_MFD_SOFTKEYS_1,
            AS1000_MFD_SOFTKEYS_2,
            AS1000_MFD_SOFTKEYS_3,
            AS1000_MFD_SOFTKEYS_4,
            AS1000_MFD_SOFTKEYS_5,
            AS1000_MFD_SOFTKEYS_6,
            AS1000_MFD_SOFTKEYS_7,
            AS1000_MFD_SOFTKEYS_8,
            AS1000_MFD_SOFTKEYS_9,
            AS1000_MFD_SOFTKEYS_10,
            AS1000_MFD_SOFTKEYS_11,
            AS1000_MFD_SOFTKEYS_12,
            AS1000_MFD_DIRECTTO,
            AS1000_MFD_ENT_Push,
            AS1000_MFD_CLR_Long,
            AS1000_MFD_CLR,
            AS1000_MFD_MENU_Push,
            AS1000_MFD_FPL_Push,
            AS1000_MFD_PROC_Push,
            AS1000_MFD_FMS_Upper_INC,
            AS1000_MFD_FMS_Upper_DEC,
            AS1000_MFD_FMS_Upper_PUSH,
            AS1000_MFD_FMS_Lower_INC,
            AS1000_MFD_FMS_Lower_DEC,
            AS1000_MFD_RANGE_INC,
            AS1000_MFD_RANGE_DEC,
            AS1000_MFD_JOYSTICK_PUSH,
            AS1000_MFD_ActivateMapCursor,
            AS1000_MFD_DeactivateMapCursor,
            AS1000_MFD_JOYSTICK_RIGHT,
            AS1000_MFD_JOYSTICK_LEFT,
            AS1000_MFD_JOYSTICK_UP,
            AS1000_MFD_JOYSTICK_DOWN,
            // G1000 MID:GROUP
            AS1000_MID_COM_1_Push,
            AS1000_MID_COM_2_Push,
            AS1000_MID_COM_3_Push,
            AS1000_MID_COM_Mic_1_Push,
            AS1000_MID_COM_Mic_2_Push,
            AS1000_MID_COM_Mic_3_Push,
            AS1000_MID_COM_Swap_1_2_Push,
            AS1000_MID_TEL_Push,
            AS1000_MID_PA_Push,
            AS1000_MID_SPKR_Push,
            AS1000_MID_MKR_Mute_Push,
            AS1000_MID_HI_SENS_Push,
            AS1000_MID_DME_Push,
            AS1000_MID_NAV_1_Push,
            AS1000_MID_NAV_2_Push,
            AS1000_MID_ADF_Push,
            AS1000_MID_AUX_Push,
            AS1000_MID_MAN_SQ_Push,
            AS1000_MID_Play_Push,
            AS1000_MID_Isolate_Pilot_Push,
            AS1000_MID_Isolate_Copilot_Push,
            AS1000_MID_Pass_Copilot_INC,
            AS1000_MID_Pass_Copilot_DEC,
            AS1000_MID_Display_Backup_Push,
            // G3000 PFD:GROUP
            AS3000_PFD_SOFTKEYS_1,
            AS3000_PFD_SOFTKEYS_2,
            AS3000_PFD_SOFTKEYS_3,
            AS3000_PFD_SOFTKEYS_4,
            AS3000_PFD_SOFTKEYS_5,
            AS3000_PFD_SOFTKEYS_6,
            AS3000_PFD_SOFTKEYS_7,
            AS3000_PFD_SOFTKEYS_8,
            AS3000_PFD_SOFTKEYS_9,
            AS3000_PFD_SOFTKEYS_10,
            AS3000_PFD_SOFTKEYS_11,
            AS3000_PFD_SOFTKEYS_12,
            // G3000 PFD Top Panel:GROUP
            AS3000_PFD_BottomKnob_Small_INC,
            AS3000_PFD_BottomKnob_Small_DEC,
            AS3000_PFD_BottomKnob_Push_Long,
            AS3000_PFD_BottomKnob_Push,
            AS3000_PFD_BottomKnob_Large_INC,
            AS3000_PFD_BottomKnob_Large_DEC,
            AS3000_PFD_TopKnob_Large_INC,
            AS3000_PFD_TopKnob_Large_DEC,
            AS3000_PFD_TopKnob_Small_INC,
            AS3000_PFD_TopKnob_Small_DEC,
            // G3000 MFD:GROUP
            AS3000_MFD_SOFTKEYS_1,
            AS3000_MFD_SOFTKEYS_2,
            AS3000_MFD_SOFTKEYS_3,
            AS3000_MFD_SOFTKEYS_4,
            AS3000_MFD_SOFTKEYS_5,
            AS3000_MFD_SOFTKEYS_6,
            AS3000_MFD_SOFTKEYS_7,
            AS3000_MFD_SOFTKEYS_8,
            AS3000_MFD_SOFTKEYS_9,
            AS3000_MFD_SOFTKEYS_10,
            AS3000_MFD_SOFTKEYS_11,
            AS3000_MFD_SOFTKEYS_12,
            // GTX 580 TBM:GROUP
            AS3000_TSC_Horizontal_SoftKey_1,
            AS3000_TSC_Horizontal_SoftKey_2,
            AS3000_TSC_Horizontal_SoftKey_3,
            AS3000_TSC_Horizontal_TopKnob_Large_INC,
            AS3000_TSC_Horizontal_TopKnob_Large_DEC,
            AS3000_TSC_Horizontal_TopKnob_Small_INC,
            AS3000_TSC_Horizontal_TopKnob_Small_DEC,
            AS3000_TSC_Horizontal_TopKnob_Push_Long,
            AS3000_TSC_Horizontal_TopKnob_Push,
            AS3000_TSC_Horizontal_BottomKnob_Small_INC,
            AS3000_TSC_Horizontal_BottomKnob_Small_DEC,
            AS3000_TSC_Horizontal_BottomKnob_Push,
            // GTX 580 Longitude:GROUP
            AS3000_TSC_Vertical_BottomKnob_Small_INC,
            AS3000_TSC_Vertical_BottomKnob_Small_DEC,
            AS3000_TSC_Vertical_BottomKnob_Push_Long,
            AS3000_TSC_Vertical_BottomKnob_Push,
            AS3000_TSC_Vertical_BottomKnob_Large_INC,
            AS3000_TSC_Vertical_BottomKnob_Large_DEC,
            AS3000_TSC_Vertical_TopKnob_Large_INC,
            AS3000_TSC_Vertical_TopKnob_Large_DEC,
            AS3000_TSC_Vertical_TopKnob_Small_INC,
            AS3000_TSC_Vertical_TopKnob_Small_DEC,
            // KAP140_:GROUP
            KAP140_Push_AP,
            KAP140_Push_HDG,
            KAP140_Push_NAV,
            KAP140_Push_APR,
            KAP140_Push_REV,
            KAP140_Push_ALT,
            KAP140_Push_UP,
            KAP140_Push_DN,
            KAP140_Long_Push_BARO,
            KAP140_Push_BARO,
            KAP140_Push_ARM,
            KAP140_Knob_Inner_INC,
            KAP140_Knob_Inner_DEC,
            KAP140_Knob_Outer_INC,
            KAP140_Knob_Outer_DEC,
            // M803:GROUP
            oclock_select,
            oclock_oat,
            oclock_control_long,
            oclock_control,
            // GNS 530:GROUP
            AS530_ENT_Push,
            AS530_MENU_Push,
            AS530_FPL_Push,
            AS530_DirectTo_Push,
            AS530_CLR_Push_Long,
            AS530_CLR_Push,
            AS530_MSG_Push,
            AS530_OBS_Push,
            AS530_VNAV_Push,
            AS530_PROC_Push,
            AS530_COMSWAP_Push,
            AS530_NAVSWAP_Push,
            AS530_RNG_Dezoom,
            AS530_RNG_Zoom,
            AS530_RightLargeKnob_Right,
            AS530_RightLargeKnob_Left,
            AS530_LeftLargeKnob_Right,
            AS530_LeftLargeKnob_Left,
            AS530_RightSmallKnob_Right,
            AS530_RightSmallKnob_Left,
            AS530_RightSmallKnob_Push,
            AS530_LeftSmallKnob_Right,
            AS530_LeftSmallKnob_Left,
            AS530_LeftSmallKnob_Push,
            // GNS 430:GROUP
            AS430_ENT_Push,
            AS430_MENU_Push,
            AS430_FPL_Push,
            AS430_DirectTo_Push,
            AS430_CLR_Push_Long,
            AS430_CLR_Push,
            AS430_MSG_Push,
            AS430_OBS_Push,
            AS430_PROC_Push,
            AS430_COMSWAP_Push,
            AS430_NAVSWAP_Push,
            AS430_RNG_Dezoom,
            AS430_RNG_Zoom,
            AS430_RightLargeKnob_Right,
            AS430_RightLargeKnob_Left,
            AS430_LeftLargeKnob_Right,
            AS430_LeftLargeKnob_Left,
            AS430_RightSmallKnob_Right,
            AS430_RightSmallKnob_Left,
            AS430_RightSmallKnob_Push,
            AS430_LeftSmallKnob_Right,
            AS430_LeftSmallKnob_Left,
            AS430_LeftSmallKnob_Push,
            // KR 87:GROUP
            adf_AntAdf,
            adf_bfo,
            adf_frqTransfert,
            adf_FltEt,
            adf_SetRst,
            // KT76C:GROUP
            TransponderIDT,
            TransponderVFR,
            TransponderCLR,
            Transponder0,
            Transponder1,
            Transponder2,
            Transponder3,
            Transponder4,
            Transponder5,
            Transponder6,
            Transponder7
        };

        private bool _connected = false;

        /// User-defined win32 event
        public const int WM_USER_SIMCONNECT = 0x0402;

        /// Window handle
        private IntPtr _handle = new IntPtr(0);

        /// SimConnect object
        private SimConnect m_oSimConnect = null;

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

        public bool Connect()
        {
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
