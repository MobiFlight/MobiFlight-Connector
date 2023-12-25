using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class AnalogInput : BaseDevice
    {
        [XmlAttribute]
        public String Pin = "54";

        [XmlAttribute]
        public String Sensitivity = "5";

        const ushort _paramCount = 3;

        public AnalogInput() { Name = "Analog Input"; _type  = DeviceType.AnalogInput; }

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
        public override bool Equals(object obj)
        {
            AnalogInput other = obj as AnalogInput;
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name
                && this.Pin == other.Pin
                && this.Sensitivity == other.Sensitivity;
        }

        public override string ToString()
        {
            return $"{Type}:{Name} Pin:{Pin} Sensitivity:{Sensitivity}";
        }
    }
}
