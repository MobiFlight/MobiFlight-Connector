using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class LedModule : BaseDevice
    {
        const ushort _paramCount = 5;
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

        public LedModule() { Name = "LedModule"; _type = MobiFlightModule.DeviceType.LedModule; }

        override public String ToInternal()
        {
            return base.ToInternal() + separator
                 + DinPin + separator
                 + ClsPin + separator
                 + ClkPin + separator
                 + Brightness + separator
                 + NumModules + separator
                 + Name;
        }

        override public bool FromInternal(String value)
        {
            String[] paramList = value.Split(separator);
            if (paramList.Count() != _paramCount)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            DinPin = paramList[1];
            ClsPin = paramList[2];
            ClkPin = paramList[3];
            Brightness = Byte.Parse(paramList[4]);
            NumModules = paramList[5];

            return true;
        }
    }
}
