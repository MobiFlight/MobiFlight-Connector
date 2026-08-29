using System;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Parses the three-byte switch-panel payload. The HID report ID is removed by
    /// <see cref="HidReport.Payload"/> before the payload reaches this class.
    /// </summary>
    internal sealed class SwitchPanelReport
    {
        public const int PayloadLength = 3;
        public const int ButtonCount = 20;

        private readonly byte[] StateBytes;

        private SwitchPanelReport(ReadOnlySpan<byte> payload)
        {
            StateBytes = payload.Slice(0, PayloadLength).ToArray();
        }

        /// <summary>
        /// Validates and copies the switch payload independently of USB transport.
        /// </summary>
        public static SwitchPanelReport Parse(ReadOnlySpan<byte> payload)
        {
            if (payload.Length < PayloadLength)
            {
                throw new ArgumentException($"Invalid switch-panel payload length. Expected at least {PayloadLength}, got {payload.Length}.", nameof(payload));
            }

            return new SwitchPanelReport(payload);
        }

        /// <summary>
        /// Produces the absolute state consumed by the standard joystick transition logic.
        /// </summary>
        public JoystickState ToJoystickState()
        {
            var state = new JoystickState();

            // The 20 useful protocol bits intentionally map directly to stable
            // MobiFlight button IDs 0 through 19 in report order.
            for (var buttonIndex = 0; buttonIndex < ButtonCount; buttonIndex++)
            {
                var byteIndex = buttonIndex / 8;
                var bitIndex = buttonIndex % 8;
                state.Buttons[buttonIndex] = (StateBytes[byteIndex] & (1 << bitIndex)) != 0;
            }

            return state;
        }
    }
}
