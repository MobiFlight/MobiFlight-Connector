using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Stepper : BaseDevice
    {
        const ushort _paramCount = 4;

        [XmlAttribute]
        public String Pin1 = "1";
        [XmlAttribute]
        public String Pin2 = "2";
        [XmlAttribute]
        public String Pin3 = "3";
        [XmlAttribute]
        public String Pin4 = "4";

        public Stepper() { Name = "Stepper"; _type = MobiFlightModule.DeviceType.Stepper; }

        override public String ToInternal()
        {
            return base.ToInternal() + separator
                 + Pin1 + separator
                 + Pin2 + separator
                 + Pin3 + separator
                 + Pin4 + separator
                 + Name;
        }

        override public bool FromInternal(String value)
        {
            String[] paramList = value.Split(separator);
            if (paramList.Count() != _paramCount)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            Pin1 = paramList[1];
            Pin2 = paramList[2];
            Pin3 = paramList[3];
            Pin4 = paramList[4];

            return true;
        }
    }
}
