using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class CustomDevice : DeviceConfig, IXmlSerializable, ICloneable
    {
        public override string Name { get { return CustomName; } }

        public const string DeprecatedType = "CustomDevice";
        public String CustomType { get; set; }
        public String CustomName { get; set; }
        public int MessageType { get; set; }
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

        public override object Clone()
        {
            return new CustomDevice
            {
                CustomType = this.CustomType,
                CustomName = CustomName,
                MessageType = MessageType,
                Value = this.Value
            };
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
                if (Int32.TryParse(reader["messageType"], out int convertedMessageType))
                {
                    MessageType = convertedMessageType;
                }
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
            writer.WriteAttributeString("messageType", MessageType.ToString());
            writer.WriteAttributeString("value", Value);
        }
    }
}
