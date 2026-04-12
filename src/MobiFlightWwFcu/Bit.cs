using System;

namespace MobiFlightWwFcu
{
    /// <summary>
    /// Represents a single controllable bit in a hardware display control message.
    /// Combines the bit's location (byte and bit position) with its state.
    /// </summary>
    internal class Bit
    {
        /// <summary>
        /// The byte number in the control message (0-indexed).
        /// </summary>
        public int ByteNumber { get; }

        /// <summary>
        /// The bit position within the byte (0-7, where 0 is LSB).
        /// </summary>
        public int BitPosition { get; }

        /// <summary>
        /// The state of the bit (on/off).
        /// </summary>
        public bool Value { get; set; }

        public Bit(int byteNumber, int bitPosition, bool value = false)
        {
            if (bitPosition < 0 || bitPosition > 7)
                throw new ArgumentOutOfRangeException(nameof(bitPosition),
                    "Bit position must be between 0 and 7");

            ByteNumber = byteNumber;
            BitPosition = bitPosition;
            Value = value;
        }
    }
}
