using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class DigInputMux : BaseDevice
    {
        private MuxDriverS _selector;
        
        const ushort _paramCount = 7;
        [XmlAttribute]
        public String DataPin = "-1";
        [XmlAttribute]
        public String NumModules = "2"; // defaults to CD4067

        public DigInputMux() { 
            Name = "DigInputMux"; 
            _type = DeviceType.DigInputMux;
            _selector = MuxDriverS.Instance;
        }

        override public String ToInternal()
        {
            string selectorPins;
            return base.ToInternal() + Separator
                 + DataPin + Separator
                 // Selector pins, always sent
                 + _selector.ToInternal()
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

            DataPin     = paramList[1];
            NumModules  = paramList[2];
            // pass the MuxDriver pins
            // (could be more efficient...)
            _selector.FromInternal(paramList[3] + Separator + paramList[4] + Separator + paramList[5] + Separator + paramList[6]);
            Name        = paramList[7];
            return true;
        }

        public override string ToString()
        {
            return $"{Type}:{Name}";
        }
    }
}