using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Encoder : BaseDevice
    {
        [XmlAttribute]
        public String PinLeft = "1";
        [XmlAttribute]
        public String PinRight = "2";

        const ushort _paramCount = 4;
        
        public Encoder() { Name = "Encoder"; _type = MobiFlightModule.DeviceType.Encoder;}

        override public String ToInternal()
        {
            return (int)Type + separator
                 + PinLeft + separator
                 + PinRight + separator
                 + Name;
        }

        override public bool FromInternal(String value)
        {
            String[] paramList = value.Split(separator);
            if (paramList.Count() != _paramCount)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            PinLeft = paramList[1];
            PinRight = paramList[2];
            Name = paramList[3];

            return true;
        }
    }
}
