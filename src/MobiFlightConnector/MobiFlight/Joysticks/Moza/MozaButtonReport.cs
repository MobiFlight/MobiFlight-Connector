using MobiFlight.Joysticks.WingFlex;
using System;

namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// Parses the MOZA base unit's HID button report. The byte/bit offsets are unknown
    /// until MOZA documents the report layout - TODO once that exists, replace
    /// <see cref="PayloadLength"/> and populate <see cref="ToJoystickState"/> with a real
    /// button table, the same shape as <see cref="Logitech.SwitchPanelReport"/>. Until
    /// then buttons will not register presses (plan risk R-BUTTONREPORT).
    /// </summary>
    internal sealed class MozaButtonReport
    {
        // TODO: replace with the real HID input report length once MOZA documents it.
        public const int PayloadLength = 18;
        private byte[] LastInputBufferState = new byte[18];

        private MozaButtonReport() { }

        public void CopyFromInputBuffer(byte[] inputBuffer)
        {
            if (inputBuffer == null || inputBuffer.Length < LastInputBufferState.Length)
            {
                throw new ArgumentException($"Invalid input buffer length. Expected {LastInputBufferState.Length}, got {inputBuffer?.Length ?? 0}");
            }
            LastInputBufferState = (byte[])inputBuffer?.Clone();
        }

        public static MozaButtonReport Parse(byte[] inputBuffer)
        {
            var result = new MozaButtonReport();
            result.CopyFromInputBuffer(inputBuffer);

            return result;
        }

        // TODO: populate button bits once MOZA's HID report layout is documented.
        public JoystickState ToJoystickState()
        {
            JoystickState state = new();

            // Buttons
            // copy the button states from the buffer to the Buttons bit by bit starting from byte 6 to byte 8
            var startingByte = 6;
            for (int i = 0; i < 74; i++)
            {
                int byteIndex = startingByte + (i / 8);
                int bitIndex = i % 8;
                bool isPressed = (LastInputBufferState[byteIndex] & (1 << bitIndex)) != 0;
                state.Buttons[i] = isPressed;
            }

            // Axes
            // Brightness value
            // state.X = (int)(LastInputBufferState[4] << 8 | LastInputBufferState[5]);

            return state;
        }
    }
}