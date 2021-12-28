using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class DigInputMux : BaseDevice
    {
        const ushort _paramCount = 2;
        [XmlAttribute]
        public String DataPin = "-1";

        public DigInputMux() { Name = "DigInputMux"; _type = DeviceType.DigInputMux; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
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
            Name = paramList[2];

            return true;
        }

        public override string ToString()
        {
            return $"{Type}:{Name}";
        }
    }
}