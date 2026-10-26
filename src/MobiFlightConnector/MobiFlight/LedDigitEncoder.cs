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
            
            return result;
        }

        public byte getDecimalPoints()
        {
            return decimalPoints;
        }
    }
}
