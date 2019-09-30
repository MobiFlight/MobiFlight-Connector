using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class PmdgEventIdInputAction : EventIdInputAction
    {
        public new const String TYPE = "PmdgEventIdInputAction";
        public new const String Label = "PMDG - Event ID";
        public new UInt32 Param;

        protected override String getType()
        {
            return TYPE;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {

            String eventId = reader["eventId"];
            String param = reader["param"];
            
            EventId = Int32.Parse(eventId);
            Param = UInt32.Parse(param);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", getType());
            writer.WriteAttributeString("eventId", EventId.ToString());
            writer.WriteAttributeString("param", Param.ToString());
        }

        public override void execute(FSUIPC.FSUIPCCacheInterface cache, MobiFlightCacheInterface moduleCache)
        {
            (cache as MobiFlight.FSUIPC.FSUIPCCacheInterface).setEventID(EventId, (int) Param);
        }
    }
}
