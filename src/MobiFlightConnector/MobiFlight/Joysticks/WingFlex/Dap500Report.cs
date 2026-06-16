using SharpDX.DirectInput;
using System;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class Dap500Report
    {
        private byte[] LastInputBufferState = new byte[4];
        
        public Dap500Report() {}

        public void CopyFromInputBuffer(byte[] inputBuffer)
        {
            if (inputBuffer == null || inputBuffer.Length < LastInputBufferState.Length)
            {
                throw new ArgumentException($"Invalid input buffer length. Expected {LastInputBufferState.Length}, got {inputBuffer?.Length ?? 0}");
            }
            LastInputBufferState = (byte[])inputBuffer?.Clone();
        }

        public Dap500Report Parse(byte[] inputBuffer)
        {
            var result = new Dap500Report();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }

        public JoystickState ToJoystickState()
        {
            // --- Byte 1
            // Device       Name                         Note                                    Mask    Byte[]  Bit[]   Example
            // Button       HDG_EC_L                     Press: 1, Release: 0                    0x01    0       0       1
            // Button       HDG_EC_R                     Press: 1, Release: 0                    0x02    0       1       1
            // Button       HDG_EC_SW                    Press: 1, Release: 0                    0x04    0       2       1
            // Button       NOSE_DOWN                    Press: 1, Release: 0                    0x08    0       3       1
            // Button       NOSE_UP                      Press: 1, Release: 0                    0x10    0       4       1
            // Button       ALT_EC_L                     Press: 1, Release: 0                    0x20    0       5       1
            // Button       ALT_EC_R                     Press: 1, Release: 0                    0x40    0       6       1
            // Button       ALT_EC_SW                    Press: 1, Release: 0                    0x80    0       7       1
            // --- Byte 2
            // Button       HDG                          Press: 1, Release: 0                    0x01    1       0       1
            // Button       APR                          Press: 1, Release: 0                    0x02    1       1       1
            // Button       NAV                          Press: 1, Release: 0                    0x04    1       2       1
            // Button       TRK                          Press: 1, Release: 0                    0x08    1       3       1
            // Button       AP                           Press: 1, Release: 0                    0x10    1       4       1
            // Button       FD                           Press: 1, Release: 0                    0x20    1       5       1
            // Button       LVL                          Press: 1, Release: 0                    0x40    1       6       1
            // Button       YD                           Press: 1, Release: 0                    0x80    1       7       1
            // --- Byte 3
            // Button       VNAV                         Press: 1, Release: 0                    0x01    2       0       1
            // Button       VS                           Press: 1, Release: 0                    0x02    2       1       1
            // Button       ALT                          Press: 1, Release: 0                    0x04    2       2       1
            // -            (Reserved)                    -                                       -      2       3       0
            // -            (Reserved)                    -                                       -      2       4       0
            // -            (Reserved)                    -                                       -      2       5       0
            // -            (Reserved)                    -                                       -      2       6       0
            // -            (Reserved)                    -                                       -      2       7       0
            // --- Byte 4
            // -            Light Sensor                 Single Byte Type (0-128)                -       3       -       0x02
            JoystickState state = new JoystickState();

            // Buttons
            // copy the button states from the buffer to the Buttons bit by bit starting from byte 6 to byte 8
            var startingByte = 1;
            for (int i = 0; i < 20; i++)
            {
                int byteIndex = startingByte + (i / 8);
                int bitIndex = i % 8;
                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            // Axes
            // Background Light Brightness
            state.X = LastInputBufferState[3];  // Light sensor value

            return state;
        }
    }
}