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

        private static readonly Lazy<vJoy> joystick = new(() => new vJoy());

        private static vJoy.JoystickState joyReport;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static List<uint> getAvailableVJoys()
        {
            var joy = joystick.Value;
            List<uint> ret = new List<uint>();

            if (!joy.vJoyEnabled())
            {
                throw new VJoyNotEnabledException();
            }
            for (uint i = 1; i <= MAX_JOYSTICKS; i++)
            {
                VjdStat status = joy.GetVJDStatus(i);
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
            var joy = joystick.Value;

            if (!joy.vJoyEnabled())
            {
                throw new VJoyNotEnabledException();

            }

            return joy.GetVJDButtonNumber(vJoyID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static AxisState getAvailableAxis(uint vJoyID)
        {
            var joy = joystick.Value;
            joyReport = new vJoy.JoystickState();

            if (!joy.vJoyEnabled())
            {
                throw new VJoyNotEnabledException();
            }
            AxisState state = new AxisState();
            state.X = joy.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_X);
            state.Y = joy.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_Y);
            state.Z = joy.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_Z);
            state.RX = joy.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RX);
            state.RY = joy.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RY);
            state.RZ = joy.GetVJDAxisExist(vJoyID, HID_USAGES.HID_USAGE_RZ);
            return state;
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
            var joy = joystick.Value;
            var status = joy.GetVJDStatus(vJoyID);
            bool isReady = (status == VjdStat.VJD_STAT_OWN) || (status == VjdStat.VJD_STAT_FREE && joy.AcquireVJD(vJoyID));
            if (!joy.vJoyEnabled() && !isReady)
            {
                throw new VJoyNotEnabledException();
            }

            try
            {
                return joy.SetBtn(state, vJoyID, buttonNr);
            }
            finally
            {
                joy.RelinquishVJD(vJoyID);
            }

        }

        [MethodImpl(MethodImplOptions.Synchronized)]

        public static bool setAxisVal(uint vJoyID, string axisString, int value)
        {
            var joy = joystick.Value;
            var status = joy.GetVJDStatus(vJoyID);
            bool isReady = (status == VjdStat.VJD_STAT_OWN) || (status == VjdStat.VJD_STAT_FREE && joy.AcquireVJD(vJoyID));
            if (!joy.vJoyEnabled() && !isReady)
            {
                throw new VJoyNotEnabledException();
            }
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
    }
}