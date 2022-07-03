using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class Precondition : IXmlSerializable, ICloneable
    {
        public const string LOGIC_AND = "and";
        public const string LOGIC_OR = "or";
        
        private string preconditionLabel = null;
        public string PreconditionLabel { 
            get {
                if (preconditionLabel != null) return preconditionLabel;
                if (PreconditionType=="config")
                    return "<Ref:" + PreconditionRef + "> " + PreconditionOperand + " " + PreconditionValue + " <Logic:" + PreconditionLogic + ">";
                else if (PreconditionType == "pin")
                {
                    return "<Serial:" + PreconditionSerial + "><Pin:" + PreconditionPin + "> " + PreconditionOperand + " " + PreconditionValue + " <Logic:" + PreconditionLogic + ">";
                }
                return "<none>";
            }
            set
            {
                preconditionLabel = value;
            }
        }
        public string PreconditionType { get; set; }
        public string PreconditionRef { get; set; }
        public string PreconditionSerial { get; set; }
        public string PreconditionPin { get; set; }
        public string PreconditionOperand { get; set; }
        public string PreconditionValue { get; set; }
        public string PreconditionLogic { get; set; }
        public bool PreconditionActive { get; set; }

        public Precondition()
        {
            PreconditionType = "none";
            PreconditionActive = false;            
            PreconditionLogic = "and";
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            PreconditionType = reader["type"];
            if (null != reader.GetAttribute("active"))
                PreconditionActive = bool.Parse(reader["active"]);
            else if (PreconditionType != "none")
                PreconditionActive = true;

            if (null != reader.GetAttribute("label"))
                PreconditionLabel = reader["label"];
            else
                PreconditionLabel = null;


            if (PreconditionType == "config")
            {
                PreconditionRef = reader["ref"];
                PreconditionOperand = reader["operand"];
                PreconditionValue = reader["value"];
            }
            else if (PreconditionType == "pin")
            {
                PreconditionSerial = reader["serial"];
                PreconditionPin = reader["pin"];
                PreconditionOperand = reader["operand"];
                PreconditionValue = reader["value"];
            }

            if (null != reader.GetAttribute("logic"))
                PreconditionLogic = reader["logic"];

            reader.Read();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            if (PreconditionType == "none") return;

            writer.WriteStartElement("precondition");
            writer.WriteAttributeString("type", PreconditionType);
            
            if (preconditionLabel!=null)
                writer.WriteAttributeString("label", preconditionLabel);

            writer.WriteAttributeString("active", PreconditionActive ? "true" : "false");
            switch (PreconditionType)
            {
                case "config":
                    writer.WriteAttributeString("ref", PreconditionRef);
                    writer.WriteAttributeString("operand", PreconditionOperand);
                    writer.WriteAttributeString("value", PreconditionValue);
                    break;
                case "pin":
                    writer.WriteAttributeString("serial", PreconditionSerial);
                    writer.WriteAttributeString("pin", PreconditionPin);
                    writer.WriteAttributeString("operand", PreconditionOperand);
                    writer.WriteAttributeString("value", PreconditionValue);
                    break;
            }

            writer.WriteAttributeString("logic", PreconditionLogic);

            writer.WriteEndElement();
        }

        public object Clone()
        {
            Precondition clone = new Precondition();
            // the precondition stuff
            clone.PreconditionType = this.PreconditionType;
            clone.PreconditionActive = this.PreconditionActive;
            clone.PreconditionRef = this.PreconditionRef;
            clone.PreconditionSerial = this.PreconditionSerial;
            clone.PreconditionPin = this.PreconditionPin;
            clone.PreconditionOperand = this.PreconditionOperand;
            clone.PreconditionValue = this.PreconditionValue;
            clone.PreconditionLogic = this.PreconditionLogic;
            clone.preconditionLabel = this.preconditionLabel;

            return clone;
        }

        public override bool Equals(object obj)
        {
            return 
                obj != null && obj is Precondition &&
                PreconditionType ==  (obj as Precondition).PreconditionType &&
                PreconditionActive == (obj as Precondition).PreconditionActive &&
                PreconditionRef == (obj as Precondition).PreconditionRef &&
                PreconditionSerial == (obj as Precondition).PreconditionSerial &&
                PreconditionPin == (obj as Precondition).PreconditionPin &&
                PreconditionOperand == (obj as Precondition).PreconditionOperand &&
                PreconditionValue == (obj as Precondition).PreconditionValue &&
                PreconditionLogic == (obj as Precondition).PreconditionLogic &&
                preconditionLabel == (obj as Precondition).preconditionLabel
            ;
        }

        override public string ToString()
        {
            return this.PreconditionLabel;
        }
    }
}
