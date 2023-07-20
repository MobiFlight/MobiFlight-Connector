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

        /* Virtual pins */
        public String Pin1 = "";
        public String Pin2 = "";
        public String Pin3 = "";
        public String Pin4 = "";
        public String Pin5 = "";
        public String Pin6 = "";


        public CustomDevice() { Name = "Custom Device"; _type = DeviceType.CustomDevice; }

        override public String ToInternal()
        {
            var pinList = new List<string> { };
            if (Pin1 != "") pinList.Add(Pin1);
            if (Pin2 != "") pinList.Add(Pin2);
            if (Pin3 != "") pinList.Add(Pin3);
            if (Pin4 != "") pinList.Add(Pin4);
            if (Pin5 != "") pinList.Add(Pin5);
            if (Pin6 != "") pinList.Add(Pin6);

            var joinedPins = String.Join("|", pinList);
            
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

            var pinArr = Pins.Split('|');
            if (pinArr.Count() >= 1) Pin1 = pinArr[0];
            if (pinArr.Count() >= 2) Pin2 = pinArr[1];
            if (pinArr.Count() >= 3) Pin3 = pinArr[2];
            if (pinArr.Count() >= 4) Pin4 = pinArr[3];
            if (pinArr.Count() >= 5) Pin5 = pinArr[4];
            if (pinArr.Count() >= 6) Pin2 = pinArr[5];

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
