using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ArcazeUSB;
using MobiFlight.InputConfig;

namespace MobiFlight
{
    public class InputConfigItem : IBaseConfigItem, IXmlSerializable, ICloneable
    {
        // we initialize a cultureInfo object 
        // which is used for serialization
        // independently from current cultureInfo
        // @see: https://forge.simple-solutions.de/issues/275
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");

        public string ModuleSerial { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public ButtonInputConfig button  { get; set; }
        public EncoderInputConfig encoder { get; set; }
        public List<Precondition> Preconditions             { get; set; }
        
        public InputConfigItem()
        {
            Preconditions = new List<Precondition>();
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            ModuleSerial = reader["serial"];
            Name = reader["name"];

            reader.Read(); // this should be the button or encoder
            switch (reader.LocalName)
            {
                case "button":
                    button = new ButtonInputConfig();
                    button.ReadXml(reader);
                    if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
                    break;

                case "encoder":
                    encoder = new EncoderInputConfig();
                    encoder.ReadXml(reader);
                    if (reader.NodeType != XmlNodeType.EndElement) reader.Read(); // this should be the corresponding "end" node
                    break;
            }
                        
            reader.Read(); // this should be the preconditions tag
            if (reader.LocalName == "preconditions")
            {
                bool atPosition = false;
                // read precondition settings if present
                if (reader.ReadToDescendant("precondition"))
                {
                    // load a list
                    do
                    {
                        Precondition tmp = new Precondition();
                        tmp.ReadXml(reader);
                        Preconditions.Add(tmp);
                        reader.ReadStartElement();
                    } while (reader.LocalName == "precondition");
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("serial", this.ModuleSerial);
            writer.WriteAttributeString("name", this.Name);

            if (button != null)
            {
                writer.WriteStartElement("button");
                button.WriteXml(writer);
                writer.WriteEndElement();
            }

            if (encoder != null)
            {
                writer.WriteStartElement("encoder");
                encoder.WriteXml(writer);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("preconditions");
            foreach (Precondition p in Preconditions)
            {
                p.WriteXml(writer);
            }
            writer.WriteEndElement();
        }
        
        public object Clone()
        {
            InputConfigItem clone = new InputConfigItem();
            if (button != null)
            clone.button = (ButtonInputConfig) this.button.Clone();

            if (encoder != null)
            clone.encoder = (EncoderInputConfig) this.encoder.Clone();
            
            foreach (Precondition p in Preconditions)
            {
                clone.Preconditions.Add(p.Clone() as Precondition);
            }

            return clone;
        }

        internal void execute(Fsuipc2Cache fsuipcCache, ButtonArgs e)
        {
            if (button != null) button.execute(fsuipcCache, e);
            else if (encoder != null) encoder.execute(fsuipcCache, e);
        }
    }
}
