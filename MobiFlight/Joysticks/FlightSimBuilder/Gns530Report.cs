﻿using SharpDX.DirectInput;

namespace MobiFlight.Joysticks.FlightSimBuilder
{
    internal class Gns530Report : IReportParser
    {
        public uint reportID;
        public int rxAxis;
        public uint buttonState;
        public IReportParser Parse(byte[] inputBuffer)
        {
            // get Report ID
            reportID = inputBuffer[0];
            // Extract the 10-bit value (bitmask 0x03FF isolates the first 10 bits)
            // rxAxis = (inputBuffer[2] << 8 | inputBuffer[1]) & 0x03FF;
            // get 32 bit Button report field:
            buttonState = (uint)inputBuffer[1] + ((uint)inputBuffer[2] << 8) + ((uint)inputBuffer[3] << 16) + ((uint)inputBuffer[4] << 24);
            return this;
        }
        public JoystickState ToJoystickState()
        {
            var result = new JoystickState();

            for (int i = 0; i != 30; i++)
            {
                result.Buttons[i] = (buttonState & (1 << i)) != 0;
            }

            return result;
        }
    }
}
