using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class MSFS2020EventIdInputAction : InputAction, ICloneable
    {
        const Int16 BaseOffset = 0x3110;
        const Int16 ParamOffset = 0x3114;

        public new const String Label = "MSFS2020 - Events";
        public new const String CacheType = "SimConnect";
        public const String TYPE = "MSFS2020EventIdInputAction";
        public String EventId;
        
        override public object Clone()
        {
            MSFS2020EventIdInputAction clone = new MSFS2020EventIdInputAction();
            clone.EventId = EventId;

            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            String eventId = reader["eventId"];

            EventId = eventId;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", getType());
            writer.WriteAttributeString("eventId", EventId.ToString());
        }

        protected virtual String getType()
        {
            return TYPE;
        }

        public override void execute(
            FSUIPC.FSUIPCCacheInterface fsuipcCache,
            SimConnectMSFS.SimConnectCacheInterface simConnectCache,
            MobiFlightCacheInterface moduleCache,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            simConnectCache.setEventID(EventId);
        }
    }
}
