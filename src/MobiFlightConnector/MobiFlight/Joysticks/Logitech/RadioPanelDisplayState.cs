using System;
using System.Globalization;

namespace MobiFlight.Joysticks.Logitech
{
    internal enum RadioPanelBand
    {
        None,
        Com1,
        Com2
    }

    /// <summary>
    /// Radio Panel LCD feature report: [report ID, 4 x 5 digits, 2 trailer bytes].
    /// The panel has five numeric positions per frequency, so 8.33 kHz channel
    /// notation cannot be represented exactly.
    /// </summary>
    internal sealed class RadioPanelDisplayState
    {
        public const int FeatureReportLength = 23;
        public const byte Blank = 0x0F;
        private const byte DecimalPoint = 0xD0;
        private const int DigitsPerDisplay = 5;
        private const int DisplayCount = 4;

        private readonly byte[][] Displays = new byte[DisplayCount][];

        public RadioPanelDisplayState()
        {
            for (var i = 0; i < DisplayCount; i++)
            {
                Displays[i] = NewBlank();
            }
        }

        private static byte[] NewBlank() => [Blank, Blank, Blank, Blank, Blank];

        /// <summary>
        /// Accepts kHz (123450) or MHz (123.45 / 123.450). The fraction is truncated, not rounded,
        /// to fit five digits. Values of 100-200 MHz are shown as 123.45; values below 100 MHz are
        /// shown as 18.025 so that a profile can drop the leading 1 and keep the 1 kHz digit.
        /// Empty, unparsable, zero or out-of-band input blanks the display.
        /// </summary>
        public static byte[] EncodeFrequency(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return NewBlank();
            if (!decimal.TryParse(input.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var value)) return NewBlank();

            if (value >= 1000m) value /= 1000m;
            if (value <= 0m || value >= 200m) return NewBlank();

            var withoutLeadingOne = value < 100m;
            var scaled = withoutLeadingOne ? value * 1000m : value * 100m;
            var digits = ((int)decimal.Truncate(scaled)).ToString("D5", CultureInfo.InvariantCulture);

            var result = new byte[DigitsPerDisplay];
            for (var i = 0; i < DigitsPerDisplay; i++)
            {
                result[i] = (byte)(digits[i] - '0');
            }

            result[withoutLeadingOne ? 1 : 2] |= DecimalPoint;
            return result;
        }

        /// <summary>
        /// Displays are ordered upper active, upper standby, lower active, lower standby.
        /// Each row shows the COM1 or COM2 pair selected on its rotary, any other position blanks it.
        /// </summary>
        public void SetValues(string com1Active, string com1Standby, string com2Active, string com2Standby,
            RadioPanelBand upper, RadioPanelBand lower)
        {
            var com1 = (Active: EncodeFrequency(com1Active), Standby: EncodeFrequency(com1Standby));
            var com2 = (Active: EncodeFrequency(com2Active), Standby: EncodeFrequency(com2Standby));

            Displays[0] = Select(upper, com1.Active, com2.Active);
            Displays[1] = Select(upper, com1.Standby, com2.Standby);
            Displays[2] = Select(lower, com1.Active, com2.Active);
            Displays[3] = Select(lower, com1.Standby, com2.Standby);
        }

        private static byte[] Select(RadioPanelBand band, byte[] com1, byte[] com2) => band switch
        {
            RadioPanelBand.Com1 => com1,
            RadioPanelBand.Com2 => com2,
            _ => NewBlank()
        };

        public byte[] ToFeatureReport()
        {
            var report = new byte[FeatureReportLength];
            for (var i = 0; i < DisplayCount; i++)
            {
                Array.Copy(Displays[i], 0, report, 1 + i * DigitsPerDisplay, DigitsPerDisplay);
            }

            return report;
        }
    }
}
