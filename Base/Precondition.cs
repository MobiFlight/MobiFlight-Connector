using MobiFlight.Base;
using MobiFlight.Modifier;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace MobiFlight
{
    [JsonConverter(typeof(Base.Serialization.Json.PreconditionConverter))]
    public class Precondition : IXmlSerializable, ICloneable
    {
        public const string OPERAND_DEFAULT = Comparison.OPERAND_EQUAL;
        public const string LOGIC_AND = "and";
        public const string LOGIC_OR = "or";

        private string _label = null;

        private static readonly LRUCache<string, bool> evaluationCache = new LRUCache<string, bool>(1000); // Set cache size limit to 1000

        [JsonIgnore]
        public string Label
        {
            get
            {
                if (_label != null) return _label;
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
                _label = value;
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

            if (_label != null)
                writer.WriteAttributeString("label", _label);

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
            clone._label = this._label;

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
                Label == (obj as Precondition).Label
            ;
        }

        /// <summary>
        /// Determines whether this precondition is empty (unconfigured).
        /// An empty precondition has Type="none" or all key fields (Ref, Serial, Pin, Value) are null.
        /// </summary>
        /// <returns><see langword="true"/> if this precondition is empty; otherwise, <see langword="false"/>.</returns>
        public bool IsEmpty()
        {
            return Type == "none" || (
                        Ref == null &&
                        Serial == null &&
                        Pin == null &&
                        Value == null
                        );
        }

        override public string ToString()
        {
            return this.Label;
        }



        public bool Evaluate(MobiFlightVariable value)
        {
            var cacheKey = $"{value.TYPE}:{value.Number}:{value.Text}:{Value}:{Operand}";
            if (evaluationCache.TryGetValue(cacheKey, out bool cachedResult))
            {
                return cachedResult;
            }

            var comparison = new Comparison
            {
                Active = true,
                Value = Value,
                Operand = Operand,
                IfValue = "1",
                ElseValue = "0"
            };

            var connectorValue = new ConnectorValue
            {
                type = value.TYPE == MobiFlightVariable.TYPE_NUMBER ? FSUIPCOffsetType.Float : FSUIPCOffsetType.String,
                Float64 = value.TYPE == MobiFlightVariable.TYPE_NUMBER ? value.Number : 0,
                String = value.TYPE == MobiFlightVariable.TYPE_NUMBER ? null : value.Text
            };

            var compResult = comparison.Apply(connectorValue, new List<ConfigRefValue>());
            var result = compResult.ToString() == "1";

            evaluationCache[cacheKey] = result;
            return result;
        }

        internal bool Evaluate(string value, ConnectorValue currentValue)
        {
            var cacheKey = $"{value}:{currentValue}:{Value}:{Operand}";
            if (evaluationCache.TryGetValue(cacheKey, out bool cachedResult))
            {
                return cachedResult;
            }

            var comparison = new Comparison
            {
                Active = true,
                Value = Value.Replace("$", currentValue.ToString()),
                Operand = Operand,
                IfValue = "1",
                ElseValue = "0"
            };

            if (comparison.Value != Value)
            {
                try
                {
                    var ce = new NCalc.Expression(comparison.Value);
                    comparison.Value = ce.Evaluate().ToString();
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Exception on eval of comparison value: {ex.Message}", LogSeverity.Error);
                }
            }

            var connectorValue = new ConnectorValue
            {
                type = double.TryParse(value, out double floatValue) ? FSUIPCOffsetType.Float : FSUIPCOffsetType.String,
                Float64 = floatValue,
                String = floatValue == 0 ? value : null
            };

            try
            {
                var result = comparison.Apply(connectorValue, new List<ConfigRefValue>()).ToString() == "1";
                evaluationCache[cacheKey] = result;
                return result;
            }
            catch (FormatException ex)
            {
                Log.Instance.log($"Exception on comparison execution, wrong format: {ex.Message}", LogSeverity.Error);
                return false;
            }
        }
    }
}
