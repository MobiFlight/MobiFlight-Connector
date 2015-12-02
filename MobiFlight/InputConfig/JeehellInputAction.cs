using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class JeehellInputAction : InputAction, ICloneable
    {
        const Int16 BaseOffset = 0x73CC;
        const Int16 ParamOffset = 0x73CD;

        public Int32 EventId;
        public Int32 Param;
        
        override public object Clone()
        {
            JeehellInputAction clone = new JeehellInputAction();
            clone.EventId = EventId;
            clone.Param = Param;

            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            String eventId = reader["pipeId"];
            String param = reader["value"];

            EventId = Int32.Parse(eventId);
            Param = Int32.Parse(param);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", "JeehellInputAction");
            writer.WriteAttributeString("pipeId", EventId.ToString());
            writer.WriteAttributeString("value", Param.ToString());
        }

        public override void execute(MobiFlight.Fsuipc2Cache fsuipcCache)
        {
            fsuipcCache.setOffset(ParamOffset, Param);
            fsuipcCache.setOffset(BaseOffset, EventId);            
            fsuipcCache.ForceUpdate();
        }
    }
}
