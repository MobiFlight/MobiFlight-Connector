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

        public LcdDisplay ()
        {
            Lines = new List<string>();
        }

        public override bool Equals(object obj)
        {
            bool linesAreEqual = true && Lines.Count == (obj as LcdDisplay).Lines.Count;
            if (linesAreEqual)
            for(int i=0; i!=Lines.Count; i++)
            {
                 linesAreEqual = linesAreEqual && (Lines[i] == (obj as LcdDisplay).Lines[i]);
            }
            
            return (
                obj != null && obj is LcdDisplay &&
                this.Address == (obj as LcdDisplay).Address &&
                linesAreEqual
            );
        }

        public object Clone()
        {
            LcdDisplay clone = new LcdDisplay();
            clone.Address = Address;
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

            if (reader.LocalName == "line")
            {
                while (reader.LocalName == "line")
                {
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        Lines.Add(reader.Value);
                        if (reader.NodeType == XmlNodeType.Text)
                            reader. Read();
                        reader.ReadEndElement(); //line
                    }
                    else
                    {
                        Lines.Add("");
                        reader.Read();
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("address", Address);

            foreach (string line in Lines) {
                writer.WriteElementString("line", line);
            }
        }
    }
}
