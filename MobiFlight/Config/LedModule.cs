using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class LedModule : BaseDevice
    {
        const ushort _paramCount = 6;
        [XmlAttribute]
        public String DinPin = "1";
        [XmlAttribute]
        public String ClsPin = "2";
        [XmlAttribute]
        public String ClkPin = "3";
        [XmlAttribute]
        public Byte Brightness = 15;
        [XmlAttribute]
        public String NumModules = "1";

        public LedModule() { Name = "LedModule"; _type = DeviceType.LedModule; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + DinPin + Separator                 
                 + ClsPin + Separator
                 + ClkPin + Separator
                 + Brightness + Separator
                 + NumModules + Separator
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

            DinPin = paramList[1];            
            ClsPin = paramList[2];
            ClkPin = paramList[3];
            Brightness = Byte.Parse(paramList[4]);
            NumModules = paramList[5];
            Name = paramList[6];

            return true;
        }
    }
}
