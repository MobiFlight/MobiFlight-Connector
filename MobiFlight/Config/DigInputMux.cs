using System;
using System.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Config
{
    public class DigInputMux : BaseDevice
    {
        public MobiFlight.Config.MuxDriverS Selector;

        const ushort _paramCount = 7;
        [XmlAttribute]
        public String DataPin = "-1";
        [XmlAttribute]
        public String NumModules = "2"; // defaults to CD4067
        //TODO how to include Selector's pins as XMLattributes here? Are they necessary?

        public DigInputMux(MobiFlight.Config.MuxDriverS muxSelector) { 
            Name = "DigInputMux"; 
            _type = DeviceType.DigInputMux;
            _muxClient = true;
            Selector = muxSelector; //XTODO ???
        }

        override public String ToInternal()
        {
            return base.ToInternal() + Separator
                 + DataPin + Separator
                 // Selector pins, always sent
                 + Selector?.ToInternal()       
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
            Selector.FromInternal(paramList[3] + Separator + paramList[4] + Separator + paramList[5] + Separator + paramList[6]);
            Name        = paramList[7];
            return true;
        }

        public override string ToString()
        {
            return $"{Type}:{Name}";
        }
    }
}