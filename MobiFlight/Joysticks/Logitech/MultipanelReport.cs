using SharpDX.DirectInput;

namespace MobiFlight.Joysticks.Logitech
{
    internal class MultipanelReport
    {
        public uint reportID;
        public int rxAxis;
        public uint buttonState;
        static public MultipanelReport ParseReport(byte[] inputBuffer)
        {
            var result = new MultipanelReport();
            // get Report ID
            result.reportID = inputBuffer[0];
            // Extract the 10-bit value (bitmask 0x03FF isolates the first 10 bits)
            // rxAxis = (inputBuffer[2] << 8 | inputBuffer[1]) & 0x03FF;
            // get 32 bit Button report field:
            result.buttonState = (uint)inputBuffer[1] + ((uint)inputBuffer[2] << 8) + ((uint)inputBuffer[3] << 16) + ((uint)inputBuffer[4] << 24);
            return result;
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
