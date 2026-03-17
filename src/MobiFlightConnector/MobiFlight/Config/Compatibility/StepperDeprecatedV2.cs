using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.Config.Compatibility
{
    public class StepperDeprecatedV2 : BaseDevice
    {
        const ushort _paramCount = 6;

        [XmlAttribute]
        public String Pin1 = "1";
        [XmlAttribute]
        public String Pin2 = "2";
        [XmlAttribute]
        public String Pin3 = "3";
        [XmlAttribute]
        public String Pin4 = "4";
        [XmlAttribute]
        public String BtnPin = "0";

        public StepperDeprecatedV2() { Name = "Stepper"; _type = DeviceType.StepperDeprecatedV2; }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + Pin1 + Separator
                 + Pin2 + Separator
                 + Pin3 + Separator
                 + Pin4 + Separator
                 + BtnPin + Separator
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
            BtnPin = paramList[5];
            Name = paramList[6];

            return true;
        }

        public override bool Equals(object obj)
        {
            Stepper other = obj as Stepper;
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name
                && this.Pin1 == other.Pin1
                && this.Pin2 == other.Pin2
                && this.Pin3 == other.Pin3
                && this.Pin4 == other.Pin4
                && this.BtnPin == other.BtnPin
                && this.Type == other.Type;
        }

        public override string ToString()
        {
            return $"{Type}: {Name} Pin1: {Pin1} Pin2: {Pin2} Pin3: {Pin3} Pin4: {Pin4} BtnPin: {BtnPin}";
        }
    }
}
