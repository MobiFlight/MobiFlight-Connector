using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class CustomDevice : BaseDevice
    {
        const ushort _paramCount = 8;
        [XmlAttribute]
        public String Pin1 = "";
        [XmlAttribute]
        public String Pin2 = "";
        [XmlAttribute]
        public String Pin3 = "";
        [XmlAttribute]
        public String Pin4 = "";
        [XmlAttribute]
        public String Pin5 = "";
        [XmlAttribute]
        public String Pin6 = "";
        [XmlAttribute]
        public String Config = "";

        public CustomDevice() { Name = "CustomDevice"; _type = DeviceType.CustomDevice; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + Pin1 + Separator
                 + Pin2 + Separator
                 + Pin3 + Separator
                 + Pin4 + Separator
                 + Pin5 + Separator
                 + Pin6 + Separator
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

            Pin1 = paramList[1];
            Pin2 = paramList[2];
            Pin3 = paramList[3];
            Pin4 = paramList[4];
            Pin5 = paramList[5];
            Pin6 = paramList[6];
            Config = paramList[7];
            Name = paramList[8];

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
                && this.Pin1 == other.Pin1
                && this.Pin2 == other.Pin2
                && this.Pin3 == other.Pin3
                && this.Pin4 == other.Pin4
                && this.Pin5 == other.Pin5
                && this.Pin6 == other.Pin6
                && this.Config == other.Config;
        }

        public override string ToString()
        {
            return $"{Type}:{Name} Pin1:{Pin1} Pin2:{Pin2} Pin3:{Pin3} Pin4:{Pin4} Pin5:{Pin5} Pin6:{Pin6} Config:{Config}";
        }
    }
}
