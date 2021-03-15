using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class ShiftRegister : BaseDevice
    {
        const ushort _paramCount = 6;
        [XmlAttribute]
        public String LatchPin = "-1";
        [XmlAttribute]
        public String ClockPin = "-2";
        [XmlAttribute]
        public String DataPin = "-3";
        [XmlAttribute]
        public String PWMPin = "-4";

        [XmlAttribute]
        public String NumModules = "1";

        public ShiftRegister() { Name = "ShiftRegister"; _type = DeviceType.ShiftRegister; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + LatchPin + Separator   // latch
                 + ClockPin + Separator  // clock                          
                 + DataPin + Separator  // data
                 + PWMPin + Separator // PWM
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

            LatchPin = paramList[1];
            ClockPin = paramList[2];
            DataPin = paramList[3];
            PWMPin = paramList[4];
            NumModules = paramList[5];
            Name = paramList[6];

            return true;
        }
    }
}
