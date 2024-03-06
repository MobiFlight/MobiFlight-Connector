using MobiFlight.Modifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class Precondition : IXmlSerializable, ICloneable
    {
        public const string OPERAND_DEFAULT = Comparison.OPERAND_EQUAL;
        public const string LOGIC_AND = "and";
        public const string LOGIC_OR = "or";

        private string label = null;

        [JsonIgnore]
        public string Label
        {
            get
            {
                if (label != null) return label;
                if (Type == "config")
                    return $"Config: <Ref:{Ref}> {Operand} {Value} <Logic:{Logic}>";
                else if (Type == "variable")
                    return $"Variable: <Variable:{Ref}> {Operand} {Value} <Logic:{Logic}>";
                else if (Type == "pin")
                {
                    return $"Pin: <Serial:{Serial}><Pin:{Pin}> {Operand} {Value} <Logic:{Logic}>";
                }
                return $"<none><Logic:{Logic}>";
            }
            set
            {
                label = value;
            }
        }
        public string Type { get; set; }
        public string Ref { get; set; }
        public string Serial { get; set; }
        public string Pin { get; set; }
        public string Operand { get; set; }
        public string Value { get; set; }
        public string Logic { get; set; }
        public bool Active { get; set; }

        public Precondition()
        {
            Type = "none";
            Active = true;
            Logic = "and";
            Operand = "=";
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return (null);
        }

        public virtual void ReadXml(XmlReader reader)
        {
            Type = reader["type"];
            if (null != reader.GetAttribute("active"))
                Active = bool.Parse(reader["active"]);
            else if (Type != "none")
                Active = true;

            if (null != reader.GetAttribute("label"))
                Label = reader["label"];
            else
                Label = null;


            if (Type == "config" || Type == "variable")
            {
                Ref = reader["ref"];
                Operand = reader["operand"];
                if (Operand == "")
                    Operand = OPERAND_DEFAULT;

                Value = reader["value"];
            }
            else if (Type == "pin")
            {
                Serial = reader["serial"];
                Pin = reader["pin"];
                Operand = reader["operand"];
                Value = reader["value"];
            }

            if (null != reader.GetAttribute("logic"))
                Logic = reader["logic"];

            reader.Read();
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            if (Type == "none") return;

            writer.WriteStartElement("precondition");
            writer.WriteAttributeString("type", Type);

            if (label != null)
                writer.WriteAttributeString("label", label);

            writer.WriteAttributeString("active", Active ? "true" : "false");
            switch (Type)
            {
                case "variable":
                case "config":
                    writer.WriteAttributeString("ref", Ref);
                    writer.WriteAttributeString("operand", Operand);
                    writer.WriteAttributeString("value", Value);
                    break;
                case "pin":
                    writer.WriteAttributeString("serial", Serial);
                    writer.WriteAttributeString("pin", Pin);
                    writer.WriteAttributeString("operand", Operand);
                    writer.WriteAttributeString("value", Value);
                    break;
            }

            writer.WriteAttributeString("logic", Logic);

            writer.WriteEndElement();
        }

        public object Clone()
        {
            Precondition clone = new Precondition();
            // the precondition stuff
            clone.Type = this.Type;
            clone.Active = this.Active;
            clone.Ref = this.Ref;
            clone.Serial = this.Serial;
            clone.Pin = this.Pin;
            clone.Operand = this.Operand;
            clone.Value = this.Value;
            clone.Logic = this.Logic;
            clone.label = this.label;

            return clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is Precondition &&
                Type == (obj as Precondition).Type &&
                Active == (obj as Precondition).Active &&
                Ref == (obj as Precondition).Ref &&
                Serial == (obj as Precondition).Serial &&
                Pin == (obj as Precondition).Pin &&
                Operand == (obj as Precondition).Operand &&
                Value == (obj as Precondition).Value &&
                Logic == (obj as Precondition).Logic &&
                label == (obj as Precondition).label
            ;
        }

        override public string ToString()
        {
            return this.Label;
        }



        public bool Evaluate(MobiFlightVariable value)
        {
            var result = false;

            var comparison = new Comparison();
            comparison.Active = true;
            comparison.Value = Value;
            comparison.Operand = Operand;
            comparison.IfValue = "True";
            comparison.ElseValue = "False";

            var connectorValue = new ConnectorValue();
            if (value.TYPE == MobiFlightVariable.TYPE_NUMBER)
            {
                connectorValue.type = FSUIPCOffsetType.Float;
                connectorValue.Float64 = value.Number;

            }
            else
            {
                connectorValue.type = FSUIPCOffsetType.String;
                connectorValue.String = value.Text;
            }

            var compResult = comparison.Apply(connectorValue, new List<ConfigRefValue>());

            return compResult.ToString() == "True";
        }

        internal bool Evaluate(string value, ConnectorValue currentValue)
        {
            var result = true;
            var comparison = new Comparison();
            comparison.Active = true;
            comparison.Value = Value.Replace("$", currentValue.ToString());

            if (comparison.Value != Value)
            {
                var ce = new NCalc.Expression(comparison.Value);
                try
                {
                    comparison.Value = (ce.Evaluate()).ToString();
                }
                catch (Exception ex)
                {
                    //argh!
                    Log.Instance.log($"Exception on eval of comparison value: {ex.Message}", LogSeverity.Error);
                }
            }

            comparison.Operand = Operand;
            comparison.IfValue = "1";
            comparison.ElseValue = "0";

            var connectorValue = new ConnectorValue();
            connectorValue.type = FSUIPCOffsetType.Float;
            if (!Double.TryParse(value, out connectorValue.Float64))
            {
                // likely to be a string
                connectorValue.type = FSUIPCOffsetType.String;
                connectorValue.String = value;
            }

            try
            {
                result = (comparison.Apply(connectorValue, new List<ConfigRefValue>()).ToString() == "1");
            }
            catch (FormatException ex)
            {
                // maybe it is a text string
                // @todo do something in the future here
                Log.Instance.log($"Exception on comparison execution, wrong format: {ex.Message}", LogSeverity.Error);
            }

            return result;
        }
    }
}
