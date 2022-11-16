using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class PmdgEventIdInputAction : EventIdInputAction
    {
        public new const String TYPE = "PmdgEventIdInputAction";
        public new const String Label = "FSUIPC - PMDG - Event ID";
        public new String Param;
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
            Param = param;
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

        public override void execute(
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            String value = Param;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();
            
            if (value.Contains("@"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("@", args.Value.ToString());
                replacements.Add(replacement);
            }

            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            value = Replace(value, replacements);

            try
            {
                cacheCollection.fsuipcCache.setEventID(EventId, (int)UInt32.Parse(value));
            }
            catch
            {
                Log.Instance.log($"Unable to convert eventId {EventId} value {value} to an integer.", LogSeverity.Error);
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is PmdgEventIdInputAction &&
                EventId == (obj as PmdgEventIdInputAction).EventId &&
                 Param == (obj as PmdgEventIdInputAction).Param &&
                 AircraftType == (obj as PmdgEventIdInputAction).AircraftType;
        }
    }
}
