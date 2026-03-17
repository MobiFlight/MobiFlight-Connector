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
        public string Ref { get; set; } 
        public string Placeholder { get; set; }
        public string TestValue { get; set; } = "1";
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!String.IsNullOrEmpty(reader["active"])) Active = bool.Parse( reader["active"]);
            if (!String.IsNullOrEmpty(reader["ref"])) Ref = reader["ref"];
            if (!String.IsNullOrEmpty(reader["placeholder"])) Placeholder = reader["placeholder"];
            if (!String.IsNullOrEmpty(reader["testvalue"])) TestValue = reader["testvalue"];
            reader.Read();
        }

        public void WriteXml(XmlWriter writer)
        {
            if (Ref == null) return;

            writer.WriteStartElement("configref");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("ref", Ref);
                writer.WriteAttributeString("placeholder", Placeholder);
                writer.WriteAttributeString("testvalue", TestValue);
            writer.WriteEndElement();
        }

        public object Clone()
        {
            ConfigRef clone = new ConfigRef();
            clone.Active = Active;
            clone.Ref = Ref;
            clone.Placeholder = Placeholder;
            clone.TestValue = TestValue;
            return clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is ConfigRef &&
                Active == (obj as ConfigRef).Active &&
                Ref == (obj as ConfigRef).Ref &&
                Placeholder == (obj as ConfigRef).Placeholder &&
                TestValue == (obj as ConfigRef).TestValue
                ;
        }
    }
}
