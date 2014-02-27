using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Button : BaseDevice
    {
        [XmlAttribute]
        public String Pin = "1";                

        const ushort _paramCount = 3;

        public Button() { Name = "Button"; _type  = MobiFlightModule.DeviceType.Button; }

        override public String ToInternal()
        {
            return base.ToInternal() + separator
                    + Pin + separator
                    + Name;
        }

        override public bool FromInternal(String value)
        {
            String[] paramList = value.Split(separator);
            if (paramList.Count() != _paramCount)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }
            
            Pin = paramList[1];
            Name = paramList[2];

            return true;
        }
    }
}
