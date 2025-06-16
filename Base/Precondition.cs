using MobiFlight.Base;
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
        
        private string preconditionLabel = null;

        private static readonly LRUCache<string, bool> evaluationCache = new LRUCache<string, bool>(1000); // Set cache size limit to 1000

        [JsonIgnore]
        public string PreconditionLabel { 
            get {
                if (preconditionLabel != null) return preconditionLabel;
                if (PreconditionType=="config")
                    return $"Config: <Ref:{PreconditionRef}> {PreconditionOperand} {PreconditionValue} <Logic:{PreconditionLogic}>";
                else if (PreconditionType == "variable")
                    return $"Variable: <Variable:{PreconditionRef}> {PreconditionOperand} {PreconditionValue} <Logic:{PreconditionLogic}>";
                else if (PreconditionType == "pin")
                {
                    return $"Pin: <Serial:{PreconditionSerial}><Pin:{PreconditionPin}> {PreconditionOperand} {PreconditionValue} <Logic:{PreconditionLogic}>";
                }
                return $"<none><Logic:{PreconditionLogic}>";
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
            PreconditionActive = true;
            PreconditionLogic = "and";
            PreconditionOperand = "=";
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


            if (PreconditionType == "config" || PreconditionType == "variable")
            {
                PreconditionRef = reader["ref"];
                PreconditionOperand = reader["operand"];
                if (PreconditionOperand == "")
                    PreconditionOperand = OPERAND_DEFAULT;

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
                case "variable":
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
                PreconditionLabel == (obj as Precondition).PreconditionLabel
            ;
        }

        override public string ToString()
        {
            return this.PreconditionLabel;
        }



        public bool Evaluate(MobiFlightVariable value)
        {
            var cacheKey = $"{value.TYPE}:{value.Number}:{value.Text}:{PreconditionValue}:{PreconditionOperand}";
            if (evaluationCache.TryGetValue(cacheKey, out bool cachedResult))
            {
                return cachedResult;
            }

            var comparison = new Comparison
            {
                Active = true,
                Value = PreconditionValue,
                Operand = PreconditionOperand,
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
            var cacheKey = $"{value}:{currentValue}:{PreconditionValue}:{PreconditionOperand}";
            if (evaluationCache.TryGetValue(cacheKey, out bool cachedResult))
            {
                return cachedResult;
            }

            var comparison = new Comparison
            {
                Active = true,
                Value = PreconditionValue.Replace("$", currentValue.ToString()),
                Operand = PreconditionOperand,
                IfValue = "1",
                ElseValue = "0"
            };

            if (comparison.Value != PreconditionValue)
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
