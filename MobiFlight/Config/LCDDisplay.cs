using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class LcdDisplay : BaseDevice
    {
        const ushort _paramCount = 4;
        [XmlAttribute]
        public Byte Address = 0x27;
        [XmlAttribute]
        public Byte Cols = 16;
        [XmlAttribute]
        public Byte Lines = 2;
        
        public LcdDisplay() { Name = "LcdDisplay"; _type = DeviceType.LcdDisplay; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + Address + Separator                 
                 + Cols + Separator
                 + Lines + Separator
                 + Name + End;
        }

        override public bool FromInternal(String value)
        {
            if (value.Length == value.IndexOf(End) + 1) value = value.Substring(0, value.Length - 1);
            String[] paramList = value.Split(Separator);
            if (paramList.Count() != _paramCount + 1)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            Address = byte.Parse(paramList[1]);
            Cols = byte.Parse(paramList[2]);
            Lines = byte.Parse(paramList[3]);
            Name = paramList[4];

            return true;
        }
    }
}
