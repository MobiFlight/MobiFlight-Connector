using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace MobiFlight.OutputConfig
{
    public class LcdDisplay : DeviceConfig
    {
        public const string DeprecatedType = "LcdDisplay";
        public override string Name { get { return Address; } }
        public String Address { get; set; }
        public List<String> Lines { get; set; }

        public LcdDisplay ()
        {
            Lines = new List<string>();
            _type = DeprecatedType; // set the type to LcdDisplay for backward compatibility
        }

        public override bool Equals(object obj)
        {
            bool linesAreEqual = true && Lines.Count == (obj as LcdDisplay)?.Lines.Count;
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

        public override object Clone()
        {
            LcdDisplay clone = new LcdDisplay();
            clone.Address = Address;
            foreach(string line in Lines)
            {
                clone.Lines.Add(line);
            }

            return clone;
        }

        
        public override void ReadXml(XmlReader reader)
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

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("address", Address);

            foreach (string line in Lines) {
                writer.WriteElementString("line", line);
            }
        }
    }
}
