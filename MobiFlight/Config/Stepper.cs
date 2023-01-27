using MobiFlight.Config.Compatibility;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class Stepper : BaseDevice
    {
        const ushort _paramCount = 10;
        public const string AUTOHOME_NONE = "0";

        [XmlAttribute]
        public String Pin1 = "1";
        [XmlAttribute]
        public String Pin2 = "2";
        [XmlAttribute]
        public String Pin3 = "3";
        [XmlAttribute]
        public String Pin4 = "4";
        [XmlAttribute]
        public String BtnPin = AUTOHOME_NONE;
        [XmlAttribute]
        public int Mode = 0;
        [XmlAttribute]
        public int Backlash = 0;
        [XmlAttribute]
        public bool Deactivate = false;
        [XmlAttribute]
        public int Profile = 0;

        public Stepper() { Name = "Stepper"; _type = DeviceType.Stepper; }

        public Stepper(StepperDeprecatedV2 stepper)
        {
            _type   = DeviceType.Stepper;
            Name    = stepper.Name;
            Pin1    = stepper.Pin1;
            Pin2    = stepper.Pin2;
            Pin3    = stepper.Pin3;
            Pin4    = stepper.Pin4;
            BtnPin  = stepper.BtnPin;
            Mode    = 0;
            Backlash = 0;
            Deactivate = false;
            Profile = 0; // 0 is the fallback profile (Classic 28BYJ)
        }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + Pin1 + Separator
                 + Pin2 + Separator
                 + Pin3 + Separator
                 + Pin4 + Separator
                 + BtnPin + Separator
                 + Mode + Separator
                 + Backlash + Separator
                 + (Deactivate ? "1" : "0") + Separator
                 + Profile + Separator
                 + Name + End;
        }

        override public bool FromInternal(String value)
        {
            if (value.Length == value.IndexOf(End) + 1) value = value.Substring(0, value.Length - 1);
            String[] paramList = value.Split(Separator);
            if (paramList.Count() != _paramCount+1)
            {
                throw new ArgumentException("Param count does not match. " + paramList.Count() + " given, " + _paramCount + " expected");
            }

            Pin1        = paramList[1];
            Pin2        = paramList[2];
            Pin3        = paramList[3];
            Pin4        = paramList[4];
            BtnPin      = paramList[5];
            Mode        = int.Parse(paramList[6]);
            Backlash    = int.Parse(paramList[7]);
            Deactivate  = paramList[8] == "1";
            Profile     = int.Parse(paramList[9]);
            Name        = paramList[10];

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
                && this.Mode == other.Mode
                && this.Backlash == other.Backlash
                && this.Deactivate == other.Deactivate
                && this.Profile == other.Profile
                && this.Type == other.Type;
        }

        public override string ToString()
        {
            return $"{Type}: {Name} Pin1: {Pin1} Pin2: {Pin2} Pin3: {Pin3} Pin4: {Pin4} BtnPin: {BtnPin} Mode: {Mode} Backlash: {Backlash} Deactivate: {Deactivate} Profile: {Profile}";
        }
    }

    public class StepperProfilePreset
    {
        public int id { get; set; }
        public StepperMode Mode { get; set; }
        public int StepsPerRevolution { get; set; }
        public int Speed { get; set; }
        public int Acceleration { get; set; }
        public int BacklashCompensation { get; set; }
        public bool Deactivate { get; set; }
    }

    public enum StepperMode
    {
        FULLSTEP,
        HALFSTEP,
        DRIVER
    }
}
