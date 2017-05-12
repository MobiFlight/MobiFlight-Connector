using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class LcdDisplay : IXmlSerializable, ICloneable
    {
        public const string Type = "LcdDisplay";
        public String Address { get; set; }
        public List<String> Lines { get; set; }

        public object Clone()
        {
            LcdDisplay clone = new LcdDisplay();
            foreach(string line in Lines)
            {
                clone.Lines.Add(line);
            }

            return clone;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["address"] != null && reader["address"] != "")
            {
                Address = reader["address"].ToString();
            }
            reader.Read();
            while ( reader.LocalName == "line")
            {
                Lines.Add(reader.Value);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Address", Address);

            foreach (string line in Lines) {
                writer.WriteElementString("line", line);
            }
        }
    }
}
