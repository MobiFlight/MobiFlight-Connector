using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Servo : BaseDevice
    {
        const ushort _paramCount = 2;
        
        [XmlAttribute]
        public String DataPin = "1";

        public Servo() { Name = "Servo"; _type = DeviceType.Servo; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + DataPin + Separator
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

            DataPin = paramList[1];
            Name = paramList[2];
            
            return true;
        }
        public override bool Equals(object obj)
        {
            Servo other = obj as Servo;
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name
                && this.DataPin == other.DataPin
                && this.Type == other.Type;
        }

        public override string ToString()
        {
            return $"{Type}:{Name} DataPin:{DataPin}";
        }
    }
}
