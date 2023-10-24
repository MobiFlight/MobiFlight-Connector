using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class LedModule : BaseDevice
    {
        public const string MODEL_TYPE_MAX72xx   = "0";
        public const string MODEL_TYPE_TM1637_4DIGIT = "253";
        public const string MODEL_TYPE_TM1637_6DIGIT = "254";

        const ushort _paramCount = 7;
        [XmlAttribute]
        public String ModelType = MODEL_TYPE_MAX72xx;
        [XmlAttribute]
        public String DinPin = "-1";
        [XmlAttribute]
        public String ClsPin = "-2";
        [XmlAttribute]
        public String ClkPin = "-3";
        [XmlAttribute]
        public Byte Brightness = 15;
        [XmlAttribute]
        public String NumModules = "1";

        public LedModule() { Name = "LedModule"; _type = DeviceType.LedModule; }

        public LedModule(LedModuleDeprecated module)
        {
            Name = module.Name; 
            _type = DeviceType.LedModule;
            ModelType = MODEL_TYPE_MAX72xx;
            DinPin = module.DinPin;
            ClsPin = module.ClsPin;
            ClkPin = module.ClkPin;
            Brightness = module.Brightness;
            NumModules = module.NumModules;
        }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + ModelType + Separator
                 + DinPin + Separator                 
                 + ClsPin + Separator
                 + ClkPin + Separator
                 + Brightness + Separator
                 + NumModules + Separator
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
            ModelType = paramList[1];
            DinPin = paramList[2];            
            ClsPin = paramList[3];
            ClkPin = paramList[4];
            Byte.TryParse(paramList[5], out byte brightness);
            Brightness = brightness;
            NumModules = paramList[6];
            Name = paramList[7];

            return true;
        }

        public override bool Equals(object obj)
        {
            LedModule other = obj as LedModule;
            if (other == null)
            {
                return false;
            }

            return this.Name == other.Name
                && this.ModelType == other.ModelType
                && this.DinPin == other.DinPin
                && this.ClsPin == other.ClsPin
                && this.ClkPin == other.ClkPin
                && this.NumModules == other.NumModules
                && this.Brightness == other.Brightness;
        }

        public override string ToString()
        {
            return Type + ":" + Name
                        + " TypeId:" + ModelType
                        + " DinPin:" + DinPin
                        + " ClsPin:" + ClsPin
                        + " ClkPin:" + ClkPin
                        + " Brightness:" + Brightness
                        + " NumModules:" + NumModules;
                        
        }
    }
}
