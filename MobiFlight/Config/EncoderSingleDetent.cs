using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class EncoderSingleDetent : BaseDevice
    {
        [XmlAttribute]
        public String PinLeft = "1";
        [XmlAttribute]
        public String PinRight = "2";

        const ushort _paramCount = 3;

        public EncoderSingleDetent() { Name = "Encoder"; _type = DeviceType.EncoderSingleDetent; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + PinLeft + Separator
                 + PinRight + Separator
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

            PinLeft = paramList[1];
            PinRight = paramList[2];
            Name = paramList[3];

            return true;
        }

        public override string ToString()
        {
            return Type + ":" + Name + " PinLeft:" + PinLeft + " PinRight:" + PinRight;
        }
    }
}