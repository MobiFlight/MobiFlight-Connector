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
        public enum PmdgAircraftType { B737, B777, B747 };
        public PmdgAircraftType AircraftType = PmdgAircraftType.B737;

        override public object Clone()
        {
            PmdgEventIdInputAction clone = new PmdgEventIdInputAction();
            clone.EventId = EventId;
            clone.Param = Param;
            clone.AircraftType = AircraftType;

            return clone;
        }

        protected override String getType()
        {
            return TYPE;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {

            String eventId = reader["eventId"];
            String param = reader["param"];
            String aircraftType = reader["aircraft"];
            
            EventId = Int32.Parse(eventId);
            Param = UInt32.Parse(param);
            if (aircraftType!=null)
                AircraftType = (PmdgAircraftType)Enum.Parse(typeof(PmdgAircraftType), aircraftType);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", getType());
            writer.WriteAttributeString("eventId", EventId.ToString());
            writer.WriteAttributeString("param", Param.ToString());
            writer.WriteAttributeString("aircraft", AircraftType.ToString());
        }

        public override void execute(FSUIPC.FSUIPCCacheInterface cache, MobiFlightCacheInterface moduleCache)
        {
            (cache as MobiFlight.FSUIPC.FSUIPCCacheInterface).setEventID(EventId, (int) Param);
        }
    }
}
