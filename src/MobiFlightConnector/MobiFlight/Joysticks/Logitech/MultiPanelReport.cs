using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Parses the three-byte multi-panel payload. The HID report ID is removed by
    /// <see cref="HidReport.Payload"/> before the payload reaches this class.
    /// </summary>
    internal sealed class MultiPanelReport
    {
        public const int PayloadLength = 3;
        public const int ButtonCount = 21;

        private readonly byte[] StateBytes;

        public MultiPanelReport() : this(new byte[PayloadLength])
        {
        }

        private MultiPanelReport(ReadOnlySpan<byte> payload)
        {
            StateBytes = payload.Slice(0, PayloadLength).ToArray();
        }

        /// <summary>
        /// Validates and copies the multi-panel payload independently of USB transport.
        /// </summary>
        public static MultiPanelReport Parse(ReadOnlySpan<byte> payload)
        {
            if (payload.Length < PayloadLength)
            {
                throw new ArgumentException($"Invalid multi-panel payload length. Expected at least {PayloadLength}, got {payload.Length}.", nameof(payload));
            }

            return new MultiPanelReport(payload);
        }

        /// <summary>
        /// Produces the absolute state consumed by the standard joystick transition logic.
        /// </summary>
        public JoystickState ToJoystickState()
        {
            var state = new JoystickState();

            // The 21 useful protocol bits intentionally map directly to stable
            // MobiFlight button IDs 0 through 20 in report order.
            for (var buttonIndex = 0; buttonIndex < ButtonCount; buttonIndex++)
            {
                var byteIndex = buttonIndex / 8;
                var bitIndex = buttonIndex % 8;
                state.Buttons[buttonIndex] = (StateBytes[byteIndex] & (1 << bitIndex)) != 0;
            }

            return state;
        }

        public static byte[] FromOutputDeviceState(List<JoystickOutputDevice> state)
        {
            var LastOutputBufferState = new byte[13];
            LastOutputBufferState[0] = 1; // Feature report ID for MultiPanel

            // OUTPUT DATA STRUCTURE - MultiPanel Output Report
            // Byte 1-5: First row
            // Byte 6-10: Second row
            // Byte 11: LEDs
            // 
            // Character encoding for the display is as follows:
            // 0000xxxx Binary encoded decimal (0x00 shows 0, 0x01 shows 1, etc.)
            // 00001111 Turns the number off
            // 11011110 Shows a dash on the bottom row(0xde)
            state.ForEach(item =>
            {
                if (item.Type == DeviceType.LcdDisplay)
                {
                    var lcdDisplay = item as JoystickOutputDisplay;
                    if (lcdDisplay == null) return;

                    var byteIndex = lcdDisplay.Byte;
                    LastOutputBufferState = UpdateLcdDisplayOutputState(lcdDisplay, byteIndex, LastOutputBufferState);
                    return;
                }

                UpdateDisplayOutputState(item, LastOutputBufferState);
            });

            return LastOutputBufferState.Clone() as byte[];
        }

        private static byte[] UpdateDisplayOutputState(JoystickOutputDevice item, byte[] LastOutputBufferState)
        {
            var itemByte = item.Byte;

            if (itemByte != 11) return LastOutputBufferState;

            if (item.State == 1)
            {
                LastOutputBufferState[itemByte] |= (byte)(1 << item.Bit);
            }
            else
            {
                LastOutputBufferState[itemByte] &= (byte)~(1 << item.Bit);
            }

            return LastOutputBufferState;
        }

        private static byte[] UpdateLcdDisplayOutputState(JoystickOutputDisplay lcdDisplay, int byteIndex, byte[] LastOutputBufferState)
        {
            if (byteIndex < 0 || byteIndex >= LastOutputBufferState.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(byteIndex), $"Invalid byte index for LCD display output state: {byteIndex}");
            }

            for (int i = 0; i < lcdDisplay.Cols; i++)
            {
                int charByteIndex = byteIndex + i;
                if (charByteIndex >= LastOutputBufferState.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(charByteIndex), $"Invalid character byte index for LCD display output state: {charByteIndex}");
                }

                if (lcdDisplay.Text.Length <= i) return LastOutputBufferState;

                // Convert the character to the appropriate byte representation
                byte charByte = ConvertCharToByte(lcdDisplay.Text[i]);
                LastOutputBufferState[charByteIndex] = charByte;
            }

            return LastOutputBufferState;
        }

        public static byte ConvertCharToByte(char c)
        {
            // Implement character to byte conversion logic here
            // Character encoding for the display is as follows:
            // 0000xxxx Binary encoded decimal (0x00 shows 0, 0x01 shows 1, etc.)
            // 00001111 Turns the number off
            // 11011110 Shows a dash on the bottom row(0xde)
            if (c == ' ')
            {
                return 0x0F;
            }
            else if (c == '-')
            {
                return 0xDE;
            }
            else if (c >= '0' && c <= '9')
            {
                // Convert digit character to its byte representation
                return (byte)(c - '0'); 
            }
            else
            {
                // Show blank for unsupported characters
                return 0x0F; 
            }
        }
    }
}