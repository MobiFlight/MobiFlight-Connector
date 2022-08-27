using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class ArcazeBcd4056
    {
        public const string TYPE = "BCD4056";

        static protected readonly Dictionary<char, byte> map = new Dictionary<char, byte> {
            { '0', 0x0 },
            { '1', 0x1 },
            { '2', 0x2 },
            { '3', 0x3 },
            { '4', 0x4 },
            { '5', 0x5 },
            { '6', 0x6 },
            { '7', 0x7 },
            { '8', 0x8 },
            { '9', 0x9 },            
            { 'l', 0xA },
            { 'h', 0xB },
            { 'p', 0xC },
            { 'a', 0xD },
            { '-', 0xE },
            { ' ', 0xF }
        };

        protected byte _convertToByte(char c)
        {
            if (!map.ContainsKey(c)) return 0xF;
            return map[c];     
        }

        /*
         * fsuipcBCD v = new fsuipcBCD();
            v.Value = value;
*/
        public List<byte> convert(string value)
        {
            if (value == "") value = " ";

            byte val = _convertToByte(value.ToLower().Last());
            return new List<byte>()
            {
                (byte)(val & 0x1),
                (byte)(val & 0x2),
                (byte)(val & 0x4),
                (byte)(val & 0x8)                
            };
        }

    }
}
