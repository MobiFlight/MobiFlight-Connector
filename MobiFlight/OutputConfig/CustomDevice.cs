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

        public String DeviceType { get; set; }
        public String Address { get; set; }
        public String MessageType { get; set; }
        public String Value { get; set; } = "";

        public CustomDevice()
        {
        }

        public override bool Equals(object obj)
        {
            return (
                obj != null && obj is CustomDevice &&
                this.DeviceType == (obj as CustomDevice).DeviceType &&
                this.Address == (obj as CustomDevice).Address &&
                this.Value == (obj as CustomDevice).Value
            );
        }

        public object Clone()
        {
            CustomDevice clone = new CustomDevice();
            clone.DeviceType = this.DeviceType;
            clone.Address = Address;
            clone.Value = this.Value;

            return clone;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["deviceType"] != null && reader["deviceType"] != "")
            {
                DeviceType = reader["deviceType"].ToString();
            }

            if (reader["address"] != null && reader["address"] != "")
            {
                Address = reader["address"].ToString();
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
            writer.WriteAttributeString("deviceType", DeviceType);
            writer.WriteAttributeString("address", Address);
            writer.WriteAttributeString("messageType", MessageType);
            writer.WriteAttributeString("value", Value);
        }
    }
}
