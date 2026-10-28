using System;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Three-byte Radio Panel input report (the report ID is already stripped by HidReport).
    /// Button indexes 0..23 correspond to ascending bits in the report:
    /// 0-6 upper selector, 7-13 lower selector, 14-15 ACT/STBY, 16-19 upper and 20-23 lower encoders.
    /// </summary>
    internal sealed class RadioPanelReport
    {
        public const int PayloadLength = 3;
        public const int ButtonCount = 24;

        private readonly byte[] StateBytes;

        private RadioPanelReport(ReadOnlySpan<byte> payload)
        {
            StateBytes = payload.Slice(0, PayloadLength).ToArray();
        }

        public static RadioPanelReport Parse(ReadOnlySpan<byte> payload)
        {
            if (payload.Length < PayloadLength)
            {
                throw new ArgumentException($"Radio Panel report expects {PayloadLength} bytes, got {payload.Length}.", nameof(payload));
            }

            return new RadioPanelReport(payload);
        }

        public JoystickState ToJoystickState()
        {
            var state = new JoystickState();
            for (var index = 0; index < ButtonCount; index++)
            {
                state.Buttons[index] = (StateBytes[index / 8] & (1 << (index % 8))) != 0;
            }

            return state;
        }
    }
}
