using SharpDX.DirectInput;

namespace MobiFlight.Joysticks.FlightSimBuilder
{
    internal class Gns1000Report : IReportParser
    {
        public uint reportID;
        public int rxAxis;
        public uint buttonState;
        public uint buttonState2;
        public uint buttonState3;

        public IReportParser Parse(byte[] inputBuffer)
        {
            var result = new Gns1000Report();
            // get Report ID
            result.reportID = inputBuffer[0];
            // Extract the 10-bit value (bitmask 0x03FF isolates the first 10 bits)
            result.rxAxis = (inputBuffer[2] << 8 | inputBuffer[1]) & 0x03FF;
            // get 32 bit Button report field:
            // result.buttonState = (uint)inputBuffer[1] + ((uint)inputBuffer[2] << 8) + ((uint)inputBuffer[3] << 16) + ((uint)inputBuffer[4] << 24);
            
            result.buttonState = (uint)inputBuffer[3] + ((uint)inputBuffer[4] << 8) + ((uint)inputBuffer[5] << 16) + ((uint)inputBuffer[6] << 24);
            result.buttonState2 = (uint)inputBuffer[7] + ((uint)inputBuffer[8] << 8) + ((uint)inputBuffer[9] << 16) + ((uint)inputBuffer[10] << 24);
            result.buttonState3 = (uint)inputBuffer[11] & 0b111;
            return result;
        }
        public JoystickState ToJoystickState()
        {
            var result = new JoystickState();

            for (int i = 0; i != 32; i++)
            {
                result.Buttons[i] = (buttonState & (1 << i)) != 0;
            }

            for (int j = 0; j != 32; j++)
            {
                result.Buttons[j+32] = (buttonState2 & (1 << j)) != 0;
            }

            for (int i = 0; i != 3; i++)
            {
                result.Buttons[i+64] = (buttonState3 & (1 << i)) != 0;
            }

            result.RotationX = rxAxis;

            return result;
        }
    }
}
