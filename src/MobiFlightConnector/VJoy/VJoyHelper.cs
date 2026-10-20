using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        private static vJoy GetJoystick()
        {
            if (joystick == null)
            {
                joystick = new vJoy();
            }
            return joystick;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static List<uint> getAvailableVJoys()
        {
            var joystick = GetJoystick();
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

        [MethodImpl(MethodImplOptions.Synchronized)]
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static int getAvailableButtons(uint vJoyID)
        {

            var joystick = GetJoystick();

            if (joystick.vJoyEnabled())
            {
                return joystick.GetVJDButtonNumber(vJoyID);
            }
            else
            {
                throw new VJoyNotEnabledException();
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static AxisState getAvailableAxis(uint vJoyID)
        {
            var joystick = GetJoystick();
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool sendButton(uint vJoyID, uint buttonNr, bool state)
        {

            var joy = GetJoystick();
            var status = joy.GetVJDStatus(vJoyID);
            bool isReady = (status == VjdStat.VJD_STAT_OWN) || (status == VjdStat.VJD_STAT_FREE && joy.AcquireVJD(vJoyID));
            if (joy.vJoyEnabled() && isReady)
            {
                try
                {
                    return joy.SetBtn(state, vJoyID, buttonNr);
                }
                finally
                {
                    joy.RelinquishVJD(vJoyID);
                }
            }
            else
            {
                throw new VJoyNotEnabledException();
            }
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static bool setAxisVal(uint vJoyID, string axisString, int value)
        {
            var joy = GetJoystick();
            var status = joy.GetVJDStatus(vJoyID);
            bool isReady = (status == VjdStat.VJD_STAT_OWN) || (status == VjdStat.VJD_STAT_FREE && joy.AcquireVJD(vJoyID));
            if (joy.vJoyEnabled() && isReady)
            {
                try
                {
                    long maxVal = 0;
                    HID_USAGES axis = getAxis(axisString);
                    joy.GetVJDAxisMax(vJoyID, axis, ref maxVal);
                    int valToSet = (value * (int)maxVal) / 100;
                    return joy.SetAxis(valToSet, vJoyID, axis);
                }
                finally
                {
                    joy.RelinquishVJD(vJoyID);
                }
            }
            else
            {
                throw new VJoyNotEnabledException();
            }
        }
    }
}