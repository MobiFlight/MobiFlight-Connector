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
        // REMINDER: MobiFlightPanel.openToolStripButton_Click()

        public DigInputMux(MobiFlight.Config.MuxDriverS muxSelector) { 
            Name = "DigInputMux"; 
            _type = DeviceType.DigInputMux;
            _muxClient = true;
            Selector = muxSelector; //XTODO ???
        }

        //XTODO
        //~DigInputMux()
        //{
        //    if (Selector != null) Selector.unregisterClient();
        //}

        override public String ToInternal()
        {
            string dummySel = "-1" + Separator + "-1" + Separator + "-1" + Separator + "-1" + Separator;
            return base.ToInternal() + Separator
                 + DataPin + Separator
                 // Selector pins, always sent
                 + (Selector?.ToInternalStripped() ?? dummySel)
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
            NumModules  = paramList[6];
            Name        = paramList[7];

            // pass the MuxDriver pins, but only if the muxDriver wasn't already set
            if (Selector == null || Selector.isInitialized()) return false;
            value = ((int)DeviceType.MuxDriver).ToString() + Separator + paramList[2] + Separator + paramList[3] + Separator + paramList[4] + Separator + paramList[5] + End;
            Selector.FromInternal(value);
            // The FromInternal() call takes care internally of the activation counter and the "initialized" flag
            return true;
        }

        public override string ToString()
        {
            return $"{Type}:{Name}";
        }
    }
}