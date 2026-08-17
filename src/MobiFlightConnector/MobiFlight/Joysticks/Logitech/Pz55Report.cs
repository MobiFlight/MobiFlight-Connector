using System;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Parses the three-byte PZ55 switch payload. The HID report ID is removed by
    /// <see cref="HidReport.Payload"/> before the payload reaches this class.
    /// </summary>
    internal sealed class Pz55Report
    {
        public const int PayloadLength = 3;
        public const int ButtonCount = 20;

        private readonly byte[] StateBytes;

        private Pz55Report(ReadOnlySpan<byte> payload)
        {
            StateBytes = payload.Slice(0, PayloadLength).ToArray();
        }

        /// <summary>
        /// Validates and copies the switch payload independently of USB transport.
        /// </summary>
        public static Pz55Report Parse(ReadOnlySpan<byte> payload)
        {
            if (payload.Length < PayloadLength)
            {
                throw new ArgumentException($"Invalid PZ55 payload length. Expected at least {PayloadLength}, got {payload.Length}.", nameof(payload));
            }

            return new Pz55Report(payload);
        }

        /// <summary>
        /// Produces the absolute state consumed by the standard joystick transition logic.
        /// </summary>
        public JoystickState ToJoystickState()
        {
            var state = new JoystickState(ButtonCount);

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
