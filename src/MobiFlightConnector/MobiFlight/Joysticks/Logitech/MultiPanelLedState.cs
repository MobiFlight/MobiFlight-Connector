using System;

namespace MobiFlight.Joysticks.Logitech
{
    internal sealed class MultiPanelLedState
    {
        public const byte ReportId = 0;
        public const int FeatureReportLength = 13;
        public const int ChannelCount = 8;

        public byte Value { get; private set; }

        /// <summary>
        /// Sets one LED channel while preserving all other channel bits.
        /// </summary>
        public void SetChannel(int channel, bool enabled)
        {
            if (channel < 0 || channel >= ChannelCount)
            {
                throw new ArgumentOutOfRangeException(nameof(channel));
            }

            byte mask = (byte)(1 << channel);
            Value = enabled ? (byte)(Value | mask) : (byte)(Value & ~mask);
        }

        public byte[] ToFeatureReport()
        {
            return new[]
            {
                // Report ID
                (byte)0x00,

                // Display top row: 5 digits OFF
                (byte)0x0F,
                (byte)0x0F,
                (byte)0x0F,
                (byte)0x0F,
                (byte)0x0F,

                // Display bottom row: 5 digits OFF
                (byte)0x0F,
                (byte)0x0F,
                (byte)0x0F,
                (byte)0x0F,
                (byte)0x0F,

                // Button LEDs
                Value,

                // Required trailing byte
                (byte)0xFF
            };
        }
        
        /// <summary>
        /// Writes the button-LED byte and trailing byte into a full 13-byte
        /// feature report. Leaves report[0] (report ID) and report[1..10]
        /// (display rows) untouched — those belong to MultiPanelDisplayState.
        /// </summary>
        public void WriteInto(byte[] report)
        {
            report[11] = Value;
            report[12] = 0xFF;
        }
    }
}