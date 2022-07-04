using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Base
{
    public class ConfigRef : IXmlSerializable, ICloneable
    {
        public bool Active { get; set; }
        public String Ref { get; set; } 
        public String Placeholder { get; set; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["active"] != null && reader["active"] != "") Active = bool.Parse( reader["active"]);
            if (reader["ref"] != null && reader["ref"] != "") Ref = reader["ref"];
            if (reader["placeholder"] != null && reader["placeholder"] != "") Placeholder = reader["placeholder"];
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Ref == null) return;

            writer.WriteStartElement("configref");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("ref", Ref);
                writer.WriteAttributeString("placeholder", Placeholder);
            writer.WriteEndElement();
        }

        public object Clone()
        {
            ConfigRef clone = new ConfigRef();
            clone.Active = Active;
            clone.Ref = Ref;
            clone.Placeholder = Placeholder;
            return clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is ConfigRef &&
                Active == (obj as ConfigRef).Active &&
                Ref == (obj as ConfigRef).Ref &&
                Placeholder == (obj as ConfigRef).Placeholder;
        }
    }
}
