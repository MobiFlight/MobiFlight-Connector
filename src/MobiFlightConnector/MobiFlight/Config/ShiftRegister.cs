using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class ShiftRegister : BaseDevice
    {
        const ushort _paramCount = 5;
        [XmlAttribute]
        public String LatchPin = "-1";
        [XmlAttribute]
        public String ClockPin = "-2";
        [XmlAttribute]
        public String DataPin = "-3";
        [XmlAttribute]
        public String NumModules = "1";

        public ShiftRegister() { Name = "ShiftRegister"; _type = DeviceType.ShiftRegister; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + LatchPin + Separator   // latch
                 + ClockPin + Separator  // clock                          
                 + DataPin + Separator  // data
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
            NumModules = paramList[4];
            Name = paramList[5];

            return true;
        }

        public override bool Equals(object obj)
        {
            ShiftRegister other = obj as ShiftRegister;
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name
                && this.LatchPin == other.LatchPin
                && this.ClockPin == other.ClockPin
                && this.DataPin == other.DataPin
                && this.NumModules == other.NumModules
                ;
        }
        public override string ToString()
        {
            return $"{Type}:{Name} LatchPin:{LatchPin} ClockPin:{ClockPin} DataPin:{DataPin} NumModules:{NumModules}";
        }
    }
}
