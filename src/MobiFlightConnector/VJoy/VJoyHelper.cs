using System;
using System.Collections.Generic;
using vJoyInterfaceWrap;

namespace MobiFlight.VJoy
{
    public struct AxisState
    {
        public bool X;
        public bool Y;
        public bool Z;
        public bool RX;
        public bool RY;
        public bool RZ;
    }

    public class VJoyDefinition
    {
        public int Id { get; set; }
        public int Buttons { get; set; }
        public AxisState Axis { get; set; }
    }

    static public class VJoyHelper
    {
        const uint MAX_JOYSTICKS = 16;

        private static vJoy joystick;
        private static vJoy.JoystickState joyReport;

        public static List<uint> getAvailableVJoys()
        {
            if (joystick == null)
                joystick = new vJoy();

            List<uint> ret = new List<uint>();
            if (!joystick.vJoyEnabled())
            {
                throw new VJoyNotEnabledException();
            }
            for (uint i = 1; i <= MAX_JOYSTICKS; i++)
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

        public static List<VJoyDefinition> GetAvailableVJoyDefinitions()
        {
            List<VJoyDefinition> definitions = new List<VJoyDefinition>();
            foreach (uint vJoyID in getAvailableVJoys())
            {
                VJoyDefinition def = new VJoyDefinition();
                def.Id = (int)vJoyID;
                def.Buttons = getAvailableButtons(vJoyID);
                def.Axis = getAvailableAxis(vJoyID);
                definitions.Add(def);
            }
            return definitions;
        }

        public static int getAvailableButtons(uint vJoyID)
        {
            joystick = new vJoy();
            if (joystick.vJoyEnabled())
            {
                return joystick.GetVJDButtonNumber(vJoyID);
            }
            else
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
                state.X = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_X);
                state.Y = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_Y);
                state.Z = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_Z);
                state.RX = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RX);
                state.RY = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RY);
                state.RZ = joystick.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RZ);
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
            if (joystick.vJoyEnabled() && (joystick.GetVJDStatus(vJoyID) == VjdStat.VJD_STAT_FREE && joystick.AcquireVJD(vJoyID)))
            {
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
            if (joystick.vJoyEnabled() && (joystick.GetVJDStatus(vJoyID) == VjdStat.VJD_STAT_FREE && joystick.AcquireVJD(vJoyID)))
            {
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