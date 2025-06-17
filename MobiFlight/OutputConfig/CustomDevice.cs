using MobiFlight.Base;
using System;
using System.Xml;
using System.Xml.Schema;

namespace MobiFlight.OutputConfig
{
    public class CustomDevice : DeviceConfig, ICloneable
    {
        public override string Name { get { return CustomName; } }

        public const string DeprecatedType = "CustomDevice";
        public String CustomType { get; set; }
        public String CustomName { get; set; }
        public int MessageType { get; set; }
        public String Value { get; set; }

        public CustomDevice()
        {
            _type = DeprecatedType; // set the type to CustomDevice for backward compatibility
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

        public override XmlSchema GetSchema()
        {
            return null;
        }

        public override void ReadXml(XmlReader reader)
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

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("customType", CustomType);
            writer.WriteAttributeString("customName", CustomName);
            writer.WriteAttributeString("messageType", MessageType.ToString());
            writer.WriteAttributeString("value", Value);
        }
    }
}
