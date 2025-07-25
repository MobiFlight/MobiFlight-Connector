using MobiFlight.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace MobiFlight.Modifier
{
    public class Comparison : ModifierBase
    {
        private static readonly LRUCache<string, ConnectorValue> evaluationCache = new LRUCache<string, ConnectorValue>(1000); // Set cache size limit to 1000

        public const string OPERAND_EQUAL = "=";
        public const string OPERAND_NOT_EQUAL = "!=";
        public const string OPERAND_GREATER_THAN = ">";
        public const string OPERAND_GREATER_THAN_EQUAL = ">=";
        public const string OPERAND_LESS_THAN = "<";
        public const string OPERAND_LESS_THAN_EQUAL = "<=";

        public string Operand { get; set; }
        public string Value { get; set; }
        public string IfValue { get; set; }
        public string ElseValue { get; set; }

        public Comparison() 
        {
            Active = false;
            Operand = OPERAND_EQUAL;
            Value = "";
            IfValue = "";
            ElseValue = "";
        }

        override public bool Equals(Object obj)
        {
            if (obj == null || !(obj is Comparison)) return false;
            var other = obj as Comparison;

            return (
                Active     == other.Active &&
                Operand    == other.Operand &&
                Value      == other.Value &&
                IfValue    == other.IfValue &&
                ElseValue  == other.ElseValue
            );
        }

        override public XmlSchema GetSchema()
        {
            return null;
        }

        override public void ReadXml(XmlReader reader)
        {
            Active = Boolean.Parse(reader["active"]);
            Value = reader["value"];
            Operand = reader["operand"];
            IfValue = reader["ifValue"];
            ElseValue = reader["elseValue"];
        }


        override public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("comparison");

                writer.WriteAttributeString("active",   Active.ToString());
                writer.WriteAttributeString("value",    Value);
                writer.WriteAttributeString("operand",  Operand);
                writer.WriteAttributeString("ifValue",  IfValue);
                writer.WriteAttributeString("elseValue",ElseValue);

            writer.WriteEndElement();
        }

        override public object Clone()
        {
            Comparison clone = new Comparison();
            clone.Active = this.Active;
            clone.Operand = this.Operand;
            clone.Value = this.Value;
            clone.IfValue = this.IfValue;
            clone.ElseValue = this.ElseValue;

            return clone;
        }
        public override ConnectorValue Apply(ConnectorValue connectorValue, List<ConfigRefValue> configRefs)
        {
            var cacheKey = $"{connectorValue}:{Value}:{Operand}:{IfValue}:{ElseValue}";
            if (evaluationCache.TryGetValue(cacheKey, out ConnectorValue cachedResult))
            {
                return cachedResult;
            }

            var result = connectorValue.Clone() as ConnectorValue;

            if (connectorValue.type == FSUIPCOffsetType.String)
            {
                result.String = ExecuteStringComparison(connectorValue, configRefs);
                evaluationCache[cacheKey] = result;
                return result;
            }

            if (string.IsNullOrEmpty(Value))
            {
                return result;
            }

            if (!double.TryParse(Value, out double comparisonValue))
            {
                return result;
            }

            double value = connectorValue.Float64;
            string comparisonIfValue = !string.IsNullOrEmpty(IfValue) ? IfValue : value.ToString();
            string comparisonElseValue = !string.IsNullOrEmpty(ElseValue) ? ElseValue : value.ToString();
            string comparisonResult = "";

            switch (Operand)
            {
                case OPERAND_NOT_EQUAL:
                    comparisonResult = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case OPERAND_GREATER_THAN:
                    comparisonResult = (value > comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case OPERAND_GREATER_THAN_EQUAL:
                    comparisonResult = (value >= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case OPERAND_LESS_THAN_EQUAL:
                    comparisonResult = (value <= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case OPERAND_LESS_THAN:
                    comparisonResult = (value < comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case OPERAND_EQUAL:
                    comparisonResult = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                default:
                    comparisonResult = (value > 0) ? "1" : "0";
                    break;
            }

            comparisonResult = comparisonResult.Replace("$", value.ToString());

            foreach (var configRef in configRefs)
            {
                comparisonResult = comparisonResult.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            try
            {
                var ce = new NCalc.Expression(comparisonResult);
                comparisonResult = ce.Evaluate().ToString();
            }
            catch
            {
                if (Log.LooksLikeExpression(comparisonResult))
                    Log.Instance.log($"Exception on NCalc evaluate => {comparisonResult}.", LogSeverity.Warn);
            }

            if (double.TryParse(comparisonResult, out double parsedResult))
            {
                result.Float64 = parsedResult;
                if (result.Float64.ToString() != comparisonResult)
                {
                    result.String = comparisonResult;
                    result.type = FSUIPCOffsetType.String;
                }
            }
            else
            {
                result.type = FSUIPCOffsetType.String;
                result.String = comparisonResult;
            }

            evaluationCache[cacheKey] = result;
            return result;
        }
        private string ExecuteStringComparison(ConnectorValue connectorValue, List<ConfigRefValue> configRefs)
        {
            string result = connectorValue.String;
            string value = connectorValue.String;

            string comparisonValue = Value;
            string comparisonIfValue = !String.IsNullOrEmpty(IfValue) ? IfValue : value;
            string comparisonElseValue = !String.IsNullOrEmpty(ElseValue) ? ElseValue : value;

            foreach (ConfigRefValue configRef in configRefs)
            {
                comparisonValue = comparisonValue.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
                comparisonIfValue = comparisonIfValue.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
                comparisonElseValue = comparisonElseValue.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            switch (Operand)
            {
                case OPERAND_NOT_EQUAL:
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case OPERAND_EQUAL:
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
            }

            result = result.Replace("$", value.ToString());

            return result;
        }

        public override string ToSummaryLabel()
        {
            return $"Compare: If current value {Operand} {Value} then {IfValue} else {ElseValue}";
        }
    }
}
