using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class ArcazeLedDigit
    {
        public const string TYPE = "Display Module";
        public const string OLDTYPE = "LedDigits";
        static protected readonly Dictionary<char,byte> map = new Dictionary<char,byte> {
            { '0', 0x7E },
            { '1', 0x30 },
            { '2', 0x6D },
            { '3', 0x79 },
            { '4', 0x33 },
            { '5', 0x5B },
            { '6', 0x5F },
            { '7', 0x70 },
            { '8', 0x7F },
            { '9', 0x7B },
            { 'a', 0x1 },   //todo
            { 'b', 0x1F },
            { 'c', 0xD },
            { 'd', 0x3D },
            { 'e', 0x1 },   //todo
            { 'f', 0x1 },   //todo
            { 'g', 0x1 },   //todo
            { 'h', 0x1 },   //todo
            { 'i', 0x1 },   //todo
            { 'l', 0xE },   //todo
            { 's', 0x5B },  //todo
            { 't', 0xF },   //todo
            { '-', 0x1 },
            { ' ', 0x0 },
        };         

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
            decimalPoints |= (byte) (1 << (digit));
        }

        public List<byte> convert(string value)
        {
            List<byte> result = new List<byte>{0,0,0,0,0,0,0,0};
            byte pos = 0;
            foreach (char c in value.ToArray().Reverse())
            {
                while ((pos < 8) && (((1 << pos) & activeDigits) == 0))
                {
                    pos++;
                }
                if (pos >= result.Count) break;
                result[pos] = (byte)(_convertToDigit(c) | ( ((1<<pos) & decimalPoints) == (1<<pos) ? 0x80 : 0 ));
                pos++;
            } //foreach

            if (connector == 2)
            {
                result.Reverse();
            }
     
            return result;
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

        static protected byte _convertToDigit(char c)
        {            
            if (!map.ContainsKey(c)) return 0;
            return map[c];            
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
