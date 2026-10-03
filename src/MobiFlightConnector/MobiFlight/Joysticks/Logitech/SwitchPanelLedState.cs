using System;

namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Composes the six independent red/green gear LED channels into report ID zero.
    /// </summary>
    internal sealed class SwitchPanelLedState
    {
        public const byte ReportId = 0;
        public const int FeatureReportLength = 2;
        public const int ChannelCount = 6;

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

            var mask = (byte)(1 << channel);
            Value = enabled ? (byte)(Value | mask) : (byte)(Value & ~mask);
        }

        /// <summary>
        /// Serializes the HidSharp feature report as report ID followed by LED payload.
        /// </summary>
        public byte[] ToFeatureReport()
        {
            return new[] { ReportId, Value };
        }
    }
}
