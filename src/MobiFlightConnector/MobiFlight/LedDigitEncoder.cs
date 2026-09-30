namespace MobiFlight
{
    /// <summary>
    /// Computes the active-digit/decimal-point bit masks for a "Display Module" (7-segment LED digit)
    /// output, used by MobiFlightCache when writing display values to a connected LED digit module.
    /// </summary>
    class LedDigitEncoder
    {
        byte activeDigits = 0;
        byte decimalPoints = 0;
        byte connector = 1;

        public void setConnector(byte value)
        {
            connector = value;
        }

        public void setActive(ushort digit)
        {
            activeDigits |= (byte)(1 << (digit));
        }

        public void setDecimalPoint(ushort digit)
        {
            decimalPoints |= (byte)(1 << (digit));
        }

        public int getMask()
        {
            byte result = activeDigits;
            if (connector == 2)
            {
                result = _reverse(result);
            }
            return result;
        }

        public byte getDecimalPoints()
        {
            return decimalPoints;
        }

        // Reverses bits in a byte
        static protected byte _reverse(byte b)
        {
            int rev = (b >> 4) | ((b & 0xf) << 4);
            rev = ((rev & 0xcc) >> 2) | ((rev & 0x33) << 2);
            rev = ((rev & 0xaa) >> 1) | ((rev & 0x55) << 1);

            return (byte)rev;
        }
    }
}
