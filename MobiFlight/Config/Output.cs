using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Output : BaseDevice
    {
        const ushort _paramCount = 1;

        [XmlAttribute]
        public String Pin = "1";

        public Output() { Name = "Output"; _type = MobiFlightModule.DeviceType.Output; }

        override public String ToInternal()
        {
            return (int)Type + separator
                 + Pin;
        }

        override public bool FromInternal(String value)
        {
            String[] paramList = value.Split(separator);
            if (paramList.Count() != _paramCount)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            Pin = paramList[1];

            return true;
        }
    }
}
