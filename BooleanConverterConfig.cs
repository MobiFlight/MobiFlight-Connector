using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MobiFlight
{    
    class BooleanConverterConfig : ConverterConfig
    {
        public const string LESS           = "<";
        public const string LESS_OR_EQUAL  = "<=";
        public const string EQUAL          = "=";
        public const string EQUAL_OR_MORE  = ">=";
        public const string MORE           = ">";

        private string value = null;
        private string type = null;
        private List<string> pins = new List<string>();
        
        public override void ReadXml(XmlReader reader)
        {
            value = reader["value"];
            type  = reader["type"];

            if (reader.ReadToDescendant("pins"))
            {
                while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "pin")
                {
                    pins.Add(reader.ReadString());
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("value", value);
            writer.WriteAttributeString("type", type);
            writer.WriteStartElement("pins");
            foreach (string pin in pins)
            {
                writer.WriteElementString("pin", pin.ToString());
            }
            writer.WriteEndElement();
        }

        public string getValue()
        {
            return value;
        }

        public string getType()
        {
            return type;
        }

        public string getPins()
        {
            return type;
        }
    }
}
