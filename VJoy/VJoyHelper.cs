using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using vJoyInterfaceWrap;

namespace MobiFlight.VJoy
{
    static class VJoyHelper
    {
        public struct AxisState
        {
            public bool xAxis;
            public bool yAxis;
            public bool zAxis;
            public bool rXAxis;
            public bool rYAxis;
            public bool rZAxis;
        }

        private static vJoy joystick;
        private static vJoy.JoystickState joyReport;

        public static List<uint> getAvailableVJoys()
        {
            joystick = new vJoy();
            List<uint> ret= new List<uint>();
            if (!joystick.vJoyEnabled())
            {
                throw new VJoyNotEnabledException();
            }
            for (uint i = 1; i <= 16; i++)
            {
                VjdStat status = joystick.GetVJDStatus(i);
                switch (status)
                {
                    case VjdStat.VJD_STAT_OWN:
                        Log.Instance.log($"vJoy device {i} is already owned by this feeder.", LogSeverity.Debug);
                        ret.Add(i);
                        break;
                    case VjdStat.VJD_STAT_FREE:
                        Log.Instance.log($"vJoy device {i} is free.", LogSeverity.Debug);
                        ret.Add(i);
                        break;
                    case VjdStat.VJD_STAT_BUSY:
                        Log.Instance.log($"vJoy device {i} is already owned by another feeder, cannot continue.", LogSeverity.Error);
                        break;
                    case VjdStat.VJD_STAT_MISS:
                        Log.Instance.log($"vJoy device {i} is not installed or disabled, cannot continue.", LogSeverity.Error);
                        break;
                }
            }
            return ret;
        }

        public static int getAvailableButtons(uint vJoyID)
        {
            joystick = new vJoy();
            if (joystick.vJoyEnabled())
            {
                return joystick.GetVJDButtonNumber(vJoyID);
            }else
            {
                throw new VJoyNotEnabledException();
            }
        }

        public static AxisState getAvailableAxis(uint vJoyID)
        {
            joystick = new vJoy();
            joyReport = new vJoy.JoystickState();
            if (joystick.vJoyEnabled())
            {
                AxisState state = new AxisState();
                state.xAxis = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_X);
                state.yAxis = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_Y);
                state.zAxis = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_Z);
                state.rXAxis = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RX);
                state.rYAxis = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RY);
                state.rZAxis = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RZ);
                return state;
            }
            else
            {
                throw new VJoyNotEnabledException();
            }
        }

        private static HID_USAGES getAxis(String axisName)
        {
            HID_USAGES ret = HID_USAGES.HID_USAGE_X;
            switch (axisName)
            {
                case ("X"):
                    ret = HID_USAGES.HID_USAGE_X;
                    break;
                case ("Y"):
                    ret = HID_USAGES.HID_USAGE_Y;
                    break;
                case ("Z"):
                    ret = HID_USAGES.HID_USAGE_Z;
                    break;
                case ("RX"):
                    ret = HID_USAGES.HID_USAGE_RX;
                    break;
                case ("RY"):
                    ret = HID_USAGES.HID_USAGE_RY;
                    break;
                case ("RZ"):
                    ret = HID_USAGES.HID_USAGE_RZ;
                    break;
            }
            return ret;
        }

        public static bool sendButton(uint vJoyID, uint buttonNr, bool state)
        {
            joystick = new vJoy();
            if (joystick.vJoyEnabled() && (joystick.GetVJDStatus(vJoyID) == VjdStat.VJD_STAT_FREE && joystick.AcquireVJD(vJoyID))) {
                bool ret = joystick.SetBtn(state, vJoyID, buttonNr);
                joystick.RelinquishVJD(vJoyID);
                return ret;
            }
            else
            {
                throw new VJoyNotEnabledException();
            }
        }

        public static bool setAxisVal(uint vJoyID, string axisString, int value)
        {
            joystick = new vJoy();
            if (joystick.vJoyEnabled() && (joystick.GetVJDStatus(vJoyID) == VjdStat.VJD_STAT_FREE && joystick.AcquireVJD(vJoyID))) {
                long maxVal = 0;
                HID_USAGES axis = getAxis(axisString);
                joystick.GetVJDAxisMax(vJoyID, axis, ref maxVal);
                int valToSet = (value * (int)maxVal) / 100;
                bool ret = joystick.SetAxis(valToSet, vJoyID, axis);
                joystick.RelinquishVJD(vJoyID);
                return ret;
            }
            else
            {
                throw new VJoyNotEnabledException();
            }
        }

    }
}
