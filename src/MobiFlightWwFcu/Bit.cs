using System;
using System.Collections.Generic;
using System.Text;

namespace MobiFlightWwFcu
{
    internal class Bit
    {
        public int ByteNumber { get; }
        public int BitPosition { get; }
        public bool Value { get; set; }

        public Bit(int byteNumber, int bitPosition, bool value = false)
        {
            ByteNumber = byteNumber;
            BitPosition = bitPosition;
            Value = value;
        }
    }
}
