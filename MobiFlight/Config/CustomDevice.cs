using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class CustomDevice : BaseDevice
    {
        const ushort _paramCount = 4;
        [XmlAttribute]
        public String CustomType = "";
        [XmlAttribute]
        public String Pins = "";
        [XmlAttribute]
        public String Config = "";

        /* 
         * Configured pins 
         * 
         * This property contains all the pins that have been selected
         * by the user. The number can be arbitrary and depends on the
         * defined number of required pins by the custom device
         * 
         * @see: MobiFlight.Config.CustomDevice.Pins
         * 
         * => pins with empty strings are ignored
         */
        public List<string> ConfiguredPins = new List<string>();

        public CustomDevice() { 
            Name = "Custom Device"; 
            _type = DeviceType.CustomDevice; 
        }

        override public String ToInternal()
        {
            var joinedPins = String.Join("|", ConfiguredPins);
            
            return base.ToInternal() + Separator
                 + CustomType + Separator
                 + joinedPins + Separator
                 + Config + Separator
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

            CustomType = paramList[1];
            Pins = paramList[2];
            Config = paramList[3];
            Name = paramList[4];

            ConfiguredPins = Pins.Split('|').ToList();

            return true;
        }

        public override bool Equals(object obj)
        {
            CustomDevice other = obj as CustomDevice;
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name
                && this.CustomType == other.CustomType
                && this.Pins == other.Pins
                && this.Config == other.Config;
        }

        public override string ToString()
        {
            return $"{Type}:{Name} CustomType:{CustomType} Pins:{Pins} Config:{Config}";
        }
    }
}
