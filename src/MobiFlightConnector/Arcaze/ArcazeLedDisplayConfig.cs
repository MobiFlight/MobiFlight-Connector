using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public enum DecodeMode { Numeric = 0x0f, Binary = 0x00 };

    [Serializable]
    public class ArcazeLedDisplayConfig
    {
        public DecodeMode mode { get; set; }
        public String address { get; set; }
        public ushort intensity { get; set; }
        public ushort scanLimit { get; set; }

        public ushort addressInternal()
        {
            return ushort.Parse(address.Replace("0x",""), System.Globalization.NumberStyles.HexNumber);
        }        
    }
}
