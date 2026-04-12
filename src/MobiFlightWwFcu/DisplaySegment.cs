using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    /// <summary>
    /// Represents a display component on Winwing hardware, 
    /// supporting both 7-segment displays and single-bit indicators.
    /// </summary>
    internal class DisplaySegment
    {
        public Bit[] Bits { get; }
        public bool IsSevenSegment { get; }

        /// <summary>
        /// For 7-segment displays, bits represent segments in order:
        /// top, top-right, bottom-right, bottom, bottom-left, top-left, middle
        /// </summary>
        public DisplaySegment(Bit[] bits, bool isSevenSegment)
        {
            Bits = bits;
            IsSevenSegment = isSevenSegment;
        }

        /// <summary>
        /// Creates a single-bit indicator element.
        /// </summary>
        public DisplaySegment(Bit bit)
        {
            Bits = new Bit[] { bit };
            IsSevenSegment = false;
        }

        /// <summary>
        /// Creates a 7-segment display element for PAP3 or PAC devices.
        /// </summary>
        /// <param name="topByte">Byte number of the top segment (0-indexed)</param>
        /// <param name="bitNumber">Bit position within bytes (0-7)</param>
        /// <param name="initChar">Initial character to display</param>
        public DisplaySegment(int topByte, int bitNumber, char initChar = '*', bool isReverse = true)
        {
            List<Bit> bits = new List<Bit>();
            if (isReverse)
            {
                for (int i = 0; i < 7; i++)
                {
                    bits.Add(new Bit(topByte - i * 4, bitNumber));
                }
            }
            else
            {
                for (int i = 6; i >= 0; i--)
                {
                    bits.Add(new Bit(topByte - i * 4, bitNumber));
                }
            }
            Bits = bits.ToArray();
            IsSevenSegment = true;
            SetCharacter(initChar);
        }

        /// <summary>
        /// Sets the character to display on a 7-segment element.
        /// </summary>
        public void SetCharacter(char c)
        {
            if (IsSevenSegment)
            {
                if (WinwingConstants.CharacterDict.TryGetValue(c, out bool[] values))
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        Bits[i].Value = values[i];
                    }
                }
            }
        }

        /// <summary>
        /// Sets all bits to the specified value (for indicators).
        /// </summary>
        public void SetValue(bool value)
        {
            foreach (var b in Bits)
            {
                b.Value = value;
            }
        }
    }
}