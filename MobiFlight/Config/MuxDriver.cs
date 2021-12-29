using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class MuxDriver : BaseDevice
    {
        const ushort _paramCount = 5;
        [XmlAttribute]
        public String[] PinSx = { "-1", "-2", "-3", "-4" };

        public MuxDriver() { Name = "MuxDriver"; _type = DeviceType.MuxDriver; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + PinSx[0] + Separator
                 + PinSx[1] + Separator
                 + PinSx[2] + Separator 
                 + PinSx[3] + Separator
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

            PinSx[0] = paramList[1];
            PinSx[1] = paramList[2];
            PinSx[2] = paramList[3];
            PinSx[3] = paramList[4];
            // Name = paramList[5];     // Single instance, internal use; no need to change name

            return true;
        }

        public override string ToString()
        {
            return $"{Type}:{Name}";
        }
    }
}