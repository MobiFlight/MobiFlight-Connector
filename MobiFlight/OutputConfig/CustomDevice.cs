using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class CustomDevice : IXmlSerializable, ICloneable
    {
        public const string Type = "CustomDevice";
        public String CustomType { get; set; }
        public String CustomName { get; set; }
        public String MessageType { get; set; }
        public String Value { get; set; }

        public CustomDevice()
        {
        }

        public override bool Equals(object obj)
        {
            return (
                obj != null && obj is CustomDevice &&
                this.CustomType == (obj as CustomDevice).CustomType &&
                this.CustomName == (obj as CustomDevice).CustomName &&
                this.MessageType == (obj as CustomDevice).MessageType &&
                this.Value == (obj as CustomDevice).Value
            );
        }

        public object Clone()
        {
            CustomDevice clone = new CustomDevice();
            clone.CustomType = this.CustomType;
            clone.CustomName = CustomName;
            clone.MessageType = MessageType;
            clone.Value = this.Value;

            return clone;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["customType"] != null && reader["customType"] != "")
            {
                CustomType = reader["customType"].ToString();
            }

            if (reader["customName"] != null && reader["customName"] != "")
            {
                CustomName = reader["customName"].ToString();
            }

            if (reader["messageType"] != null && reader["messageType"] != "")
            {
                MessageType = reader["messageType"].ToString();
            }

            if (reader["value"] != null && reader["value"] != "")
            {
                Value = reader["value"].ToString();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("customType", CustomType);
            writer.WriteAttributeString("customName", CustomName);
            writer.WriteAttributeString("messageType", MessageType);
            writer.WriteAttributeString("value", Value);
        }
    }
}
