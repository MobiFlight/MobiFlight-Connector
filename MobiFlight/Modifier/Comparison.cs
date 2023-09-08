using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.Modifier
{
    public class Comparison : ModifierBase
    {
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
            return (
                obj != null && (obj is Comparison) &&
                this.Active     == (obj as Comparison).Active &&
                this.Operand    == (obj as Comparison).Operand &&
                this.Value      == (obj as Comparison).Value &&
                this.IfValue    == (obj as Comparison).IfValue &&
                this.ElseValue  == (obj as Comparison).ElseValue
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
            var result = connectorValue.Clone() as ConnectorValue;

            if (connectorValue.type == FSUIPCOffsetType.String)
            {
                result.String = ExecuteStringComparison(connectorValue);
                return result;
            }

            Double value = connectorValue.Float64;
            
            if (Value == "")
            {
                return result;
            }

            Double comparisonValue = Double.Parse(Value);
            string comparisonIfValue = IfValue != "" ? IfValue : value.ToString();
            string comparisonElseValue = ElseValue != "" ? ElseValue : value.ToString();
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

            foreach (ConfigRefValue configRef in configRefs)
            {
                comparisonResult = comparisonResult.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            try
            {
                var ce = new NCalc.Expression(comparisonResult);
                comparisonResult = (ce.Evaluate()).ToString();
            }
            catch
            {
                if (Log.LooksLikeExpression(comparisonResult))
                    Log.Instance.log($"Exception on NCalc evaluate => {result}.", LogSeverity.Warn);
            }

            try
            {
                result.Float64 = Double.Parse(comparisonResult);
                if (result.Float64.ToString()!=comparisonResult.ToString())
                {
                    result.String = comparisonResult;
                    result.type = FSUIPCOffsetType.String;
                }
            } catch(FormatException e)
            {
                result.type = FSUIPCOffsetType.String;
                result.String = comparisonResult;
            }

            return result;
        }
        private string ExecuteStringComparison(ConnectorValue connectorValue)
        {
            string result = connectorValue.String;
            string value = connectorValue.String;

            string comparisonValue = Value;
            string comparisonIfValue = IfValue;
            string comparisonElseValue = ElseValue;

            switch (Operand)
            {
                case OPERAND_NOT_EQUAL:
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case OPERAND_EQUAL:
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
            }

            return result;
        }

        public override string ToSummaryLabel()
        {
            return $"Compare: If current value {Operand} {Value} then {IfValue} else {ElseValue}";
        }
    }
}
