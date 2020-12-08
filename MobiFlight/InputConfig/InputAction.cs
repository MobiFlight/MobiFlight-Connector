using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    abstract public class InputAction : IXmlSerializable, ICloneable
    {
        public const String Label = "InputAction";
        public const String CacheType = "FSUIPC";

        protected System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");
        abstract public object Clone();
        public System.Xml.Schema.XmlSchema GetSchema() {
            return (null);
        }
        abstract public void ReadXml(System.Xml.XmlReader reader);
        abstract public void WriteXml(System.Xml.XmlWriter writer);

        abstract public void execute(FSUIPC.Fsuipc2Cache fsuipcCache, SimConnectMSFS.SimConnectCache simConnectCache, MobiFlightCacheInterface moduleCache);
    }
}
