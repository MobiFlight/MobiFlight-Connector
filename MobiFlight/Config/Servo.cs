using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Servo : BaseDevice
    {
        const ushort _paramCount = 1;
        
        [XmlAttribute]
        public String DataPin = "1";

        public Servo() { Name = "Servo"; _type = MobiFlightModule.DeviceType.Servo; }

        override public String ToInternal()
        {
            return base.ToInternal() + separator
                 + DataPin + separator
                 + Name;
        }

        override public bool FromInternal(String value)
        {
            String[] paramList = value.Split(separator);
            if (paramList.Count() != _paramCount)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            DataPin = paramList[1];
            
            return true;
        }
    }
}
