using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Stepper : BaseDevice
    {
        const ushort _paramCount = 6;

        [XmlAttribute]
        public String Pin1 = "1";
        [XmlAttribute]
        public String Pin2 = "2";
        [XmlAttribute]
        public String Pin3 = "3";
        [XmlAttribute]
        public String Pin4 = "4";
        [XmlAttribute]
        public String BtnPin = "5";

        public Stepper() { Name = "Stepper"; _type = DeviceType.Stepper; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + Pin1 + Separator
                 + Pin2 + Separator
                 + Pin3 + Separator
                 + Pin4 + Separator
                 + BtnPin + Separator
                 + Name + End;
        }

        override public bool FromInternal(String value)
        {
            if (value.Length == value.IndexOf(End) + 1) value = value.Substring(0, value.Length - 1);
            String[] paramList = value.Split(Separator);
            if (paramList.Count() != _paramCount+1)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            Pin1    = paramList[1];
            Pin2    = paramList[2];
            Pin3    = paramList[3];
            Pin4    = paramList[4];
            BtnPin  = paramList[5];
            Name    = paramList[6];

            return true;
        }
    }
}
