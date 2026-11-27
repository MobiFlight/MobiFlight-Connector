using System;
using System.Collections.Generic;
using System.Globalization;

namespace MobiFlight.Joysticks.Logitech
{
    internal enum RadioPanelDisplaySlot
    {
        Active = 0,
        Standby = 1
    }

    /// <summary>
    /// Radio Panel LCD feature report: [report ID, 4 x 5 digits, 2 trailer bytes].
    /// Values are cached per selector mode, each row shows the active/standby pair
    /// of the mode selected on its rotary.
    /// </summary>
    internal sealed class RadioPanelDisplayState
    {
        public const int FeatureReportLength = 23;
        public const byte Blank = 0x0F;
        private const byte DecimalPoint = 0xD0;
        private const int DigitsPerDisplay = 5;

        private static readonly Dictionary<string, (RadioPanelMode Mode, RadioPanelDisplaySlot Slot)> Addresses = CreateAddresses();

        private readonly string[,] Values = new string[RadioPanelModes.Count, 2];

        public RadioPanelMode UpperMode { get; private set; } = RadioPanelMode.None;
        public RadioPanelMode LowerMode { get; private set; } = RadioPanelMode.None;

        /// <summary>Output id of a display, e.g. "LCD COM1 ACTIVE" or "LCD XPDR STBY".</summary>
        public static string GetAddress(RadioPanelMode mode, RadioPanelDisplaySlot slot) =>
            $"LCD {RadioPanelModes.GetName(mode)} {(slot == RadioPanelDisplaySlot.Active ? "ACTIVE" : "STBY")}";

        public static bool TryParseAddress(string address, out RadioPanelMode mode, out RadioPanelDisplaySlot slot)
        {
            if (address != null && Addresses.TryGetValue(address, out var target))
            {
                (mode, slot) = target;
                return true;
            }

            mode = RadioPanelMode.None;
            slot = RadioPanelDisplaySlot.Active;
            return false;
        }

        private static Dictionary<string, (RadioPanelMode, RadioPanelDisplaySlot)> CreateAddresses()
        {
            var result = new Dictionary<string, (RadioPanelMode, RadioPanelDisplaySlot)>();
            foreach (var mode in RadioPanelModes.All)
            {
                result[GetAddress(mode, RadioPanelDisplaySlot.Active)] = (mode, RadioPanelDisplaySlot.Active);
                result[GetAddress(mode, RadioPanelDisplaySlot.Standby)] = (mode, RadioPanelDisplaySlot.Standby);
            }

            return result;
        }

        /// <summary>Caches the value of one display. Returns true if it changed.</summary>
        public bool SetValue(RadioPanelMode mode, RadioPanelDisplaySlot slot, string value)
        {
            if (mode == RadioPanelMode.None) return false;
            if (Values[(int)mode, (int)slot] == value) return false;

            Values[(int)mode, (int)slot] = value;
            return true;
        }

        /// <summary>Sets the mode each row displays. Returns true if either changed.</summary>
        public bool SetModes(RadioPanelMode upper, RadioPanelMode lower)
        {
            if (upper == UpperMode && lower == LowerMode) return false;

            UpperMode = upper;
            LowerMode = lower;
            return true;
        }

        /// <summary>
        /// Displays are ordered upper active, upper standby, lower active, lower standby.
        /// A row whose selector position is not known yet stays blank.
        /// </summary>
        public byte[] ToFeatureReport()
        {
            var report = new byte[FeatureReportLength];
            WriteDisplay(report, 0, UpperMode, RadioPanelDisplaySlot.Active);
            WriteDisplay(report, 1, UpperMode, RadioPanelDisplaySlot.Standby);
            WriteDisplay(report, 2, LowerMode, RadioPanelDisplaySlot.Active);
            WriteDisplay(report, 3, LowerMode, RadioPanelDisplaySlot.Standby);
            return report;
        }

        private void WriteDisplay(byte[] report, int display, RadioPanelMode mode, RadioPanelDisplaySlot slot)
        {
            var digits = mode == RadioPanelMode.None
                ? NewBlank()
                : Encode(mode, Values[(int)mode, (int)slot]);
            Array.Copy(digits, 0, report, 1 + display * DigitsPerDisplay, DigitsPerDisplay);
        }

        public static byte[] Encode(RadioPanelMode mode, string value) =>
            RadioPanelModes.ShowsFrequency(mode) ? EncodeFrequency(value) : EncodeNumber(value);

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
        /// Shows a plain number right-aligned, used for ADF, DME and XPDR. Digits are taken as
        /// given, so leading zeros are kept (squawk 0123). Accepts at most one decimal point;
        /// decimals that do not fit into five digits are truncated. Empty input, anything that
        /// is not digits and an optional decimal point, or more than five integer digits blanks
        /// the display.
        /// </summary>
        public static byte[] EncodeNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return NewBlank();

            var text = input.Trim();
            var separator = text.IndexOf('.');
            var integerPart = separator < 0 ? text : text.Substring(0, separator);
            var fractionPart = separator < 0 ? string.Empty : text.Substring(separator + 1);

            if (!IsDigits(integerPart) || !IsDigits(fractionPart)) return NewBlank();
            if (integerPart.Length == 0 && fractionPart.Length == 0) return NewBlank();
            if (integerPart.Length == 0) integerPart = "0";
            if (integerPart.Length > DigitsPerDisplay) return NewBlank();

            if (integerPart.Length + fractionPart.Length > DigitsPerDisplay)
            {
                fractionPart = fractionPart.Substring(0, DigitsPerDisplay - integerPart.Length);
            }

            var digits = integerPart + fractionPart;
            var result = NewBlank();
            var offset = DigitsPerDisplay - digits.Length;
            for (var i = 0; i < digits.Length; i++)
            {
                result[offset + i] = (byte)(digits[i] - '0');
            }

            if (fractionPart.Length > 0)
            {
                result[offset + integerPart.Length - 1] |= DecimalPoint;
            }

            return result;
        }

        private static bool IsDigits(string text)
        {
            foreach (var c in text)
            {
                if (c < '0' || c > '9') return false;
            }

            return true;
        }
    }
}
