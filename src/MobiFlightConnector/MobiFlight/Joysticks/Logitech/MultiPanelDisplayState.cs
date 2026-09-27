using System;

namespace MobiFlight.Joysticks.Logitech
{
    internal enum MultiPanelSelector
    {
        Alt,
        Vs,
        Ias,
        Hdg,
        Crs
    }

    internal sealed class MultiPanelDisplayState
    {
        private const int DigitSlots = 5;
        private const byte DigitOff = 0x0F;
        private const byte DigitDash = 0xDE;

        private readonly byte[] TopRow = NewBlankRow();
        private readonly byte[] BottomRow = NewBlankRow();

        private static byte[] NewBlankRow() => new[] { DigitOff, DigitOff, DigitOff, DigitOff, DigitOff };

        /// <summary>
        /// Recomputes both display rows for the currently active selector position.
        /// ALT and VS always occupy both rows together; IAS/HDG/CRS occupy the top
        /// row only (3 of its 5 slots), with the bottom row blanked.
        /// </summary>
        public void SetDisplay(MultiPanelSelector selector, int altValue, int vsValue, int iasValue, int hdgValue, int crsValue)
        {
            switch (selector)
            {
                case MultiPanelSelector.Alt:
                case MultiPanelSelector.Vs:
                    EncodeInto(TopRow, altValue, digits: 5, allowSign: false);
                    EncodeInto(BottomRow, vsValue, digits: 4, allowSign: true);
                    break;

                case MultiPanelSelector.Ias:
                    EncodeInto(TopRow, iasValue, digits: 3, allowSign: false);
                    Array.Copy(NewBlankRow(), BottomRow, DigitSlots);
                    break;

                case MultiPanelSelector.Hdg:
                    EncodeInto(TopRow, hdgValue, digits: 3, allowSign: false);
                    Array.Copy(NewBlankRow(), BottomRow, DigitSlots);
                    break;

                case MultiPanelSelector.Crs:
                    EncodeInto(TopRow, crsValue, digits: 3, allowSign: false);
                    Array.Copy(NewBlankRow(), BottomRow, DigitSlots);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(selector));
            }
        }

        /// <summary>
        /// Writes a right-aligned, blank-padded value into a 5-slot row. Unused
        /// slots stay 0x0F (off) — never zero-padded — so 23 renders as
        /// blank/blank/blank/2/3, not 2/3/0/0/0.
        /// </summary>
        private static void EncodeInto(byte[] row, int value, int digits, bool allowSign)
        {
            Array.Copy(NewBlankRow(), row, DigitSlots);

            bool negative = allowSign && value < 0;
            string digitsText = Math.Abs(value).ToString();

            if (digitsText.Length > digits)
            {
                digitsText = digitsText.Substring(digitsText.Length - digits);
            }

            int start = DigitSlots - digitsText.Length;
            for (int i = 0; i < digitsText.Length; i++)
            {
                row[start + i] = (byte)(digitsText[i] - '0');
            }

            if (negative && start > 0)
            {
                row[start - 1] = DigitDash;
            }
        }

        /// <summary>
        /// Writes both display rows into a full 13-byte feature report.
        /// Leaves report[0] (report ID) and report[11..12] (LED byte, trailer)
        /// untouched — those belong to MultiPanelLedState.
        /// </summary>
        public void WriteInto(byte[] report)
        {
            Array.Copy(TopRow, 0, report, 1, DigitSlots);
            Array.Copy(BottomRow, 0, report, 6, DigitSlots);
        }
    }
}