using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class AnalogDevice : BaseDevice
    {
        [XmlAttribute]
        public String Pin = "1";

        [XmlAttribute]
        public String Sensitivity = "1";

        const ushort _paramCount = 3;

        public AnalogDevice() { Name = "AnalogDevice"; _type  = DeviceType.Analog; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                    + Pin + Separator
                    + Sensitivity + Separator
                    + Name                    
                    + End;
        }

        override public bool FromInternal(String value)
        {
            if (value.Length==value.IndexOf(End)+1) value = value.Substring(0,value.Length-1);

            String[] paramList = value.Split(Separator);
            if (paramList.Count() != _paramCount+1)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }
            
            Pin = paramList[1];            
            Sensitivity = paramList[2];
            Name = paramList[3];

            return true;
        }
    }
}
