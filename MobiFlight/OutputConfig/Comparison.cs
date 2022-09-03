using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight.OutputConfig
{
    public class Comparison
    {
        public bool   Active { get; set; }
        public string Operand { get; set; }
        public string Value { get; set; }
        public string IfValue { get; set; }
        public string ElseValue { get; set; }

        public Comparison() 
        {
            Active = false;
            Operand = "";
            Value = "";
            IfValue = "";
            ElseValue = "";
        }

        public override bool Equals(Object obj)
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

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Active = Boolean.Parse(reader["active"]);
            Value = reader["value"];
            Operand = reader["operand"];
            IfValue = reader["ifValue"];
            ElseValue = reader["elseValue"];
        }


        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("comparison");

                writer.WriteAttributeString("active",   Active.ToString());
                writer.WriteAttributeString("value",    Value);
                writer.WriteAttributeString("operand",  Operand);
                writer.WriteAttributeString("ifValue",  IfValue);
                writer.WriteAttributeString("elseValue",ElseValue);

            writer.WriteEndElement();
        }

        internal object Clone()
        {
            Comparison clone = new Comparison();
            clone.Active = this.Active;
            clone.Operand = this.Operand;
            clone.Value = this.Value;
            clone.IfValue = this.IfValue;
            clone.ElseValue = this.ElseValue;

            return clone;
        }
        public string Apply(ConnectorValue connectorValue, List<ConfigRefValue> configRefs)
        {
            string result = null;

            if (connectorValue.type == FSUIPCOffsetType.String)
            {
                return ExecuteStringComparison(connectorValue);
            }

            Double value = connectorValue.Int64;
            /*if (connectorValue.type == FSUIPCOffsetType.UnsignedInt) value = connectorValue.Uint64;*/
            if (connectorValue.type == FSUIPCOffsetType.Float) value = connectorValue.Float64;

            if (!Active)
            {
                return value.ToString();
            }

            if (Value == "")
            {
                return value.ToString();
            }

            Double comparisonValue = Double.Parse(Value);
            string comparisonIfValue = IfValue != "" ? IfValue : value.ToString();
            string comparisonElseValue = ElseValue != "" ? ElseValue : value.ToString();

            switch (Operand)
            {
                case "!=":
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case ">":
                    result = (value > comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case ">=":
                    result = (value >= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "<=":
                    result = (value <= comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "<":
                    result = (value < comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "=":
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                default:
                    result = (value > 0) ? "1" : "0";
                    break;
            }

            result = result.Replace("$", value.ToString());

            foreach (ConfigRefValue configRef in configRefs)
            {
                result = result.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            try
            {
                var ce = new NCalc.Expression(result);
                result = (ce.Evaluate()).ToString();
            }
            catch
            {
                if (Log.LooksLikeExpression(result))
                    Log.Instance.log("ExecuteComparison : Exception on NCalc evaluate => " + result, LogSeverity.Warn);
            }

            return result;
        }
        private string ExecuteStringComparison(ConnectorValue connectorValue)
        {
            string result = connectorValue.String;
            string value = connectorValue.String;

            if (!Active)
            {
                return connectorValue.String;
            }

            string comparisonValue = Value;
            string comparisonIfValue = IfValue;
            string comparisonElseValue = ElseValue;

            switch (Operand)
            {
                case "!=":
                    result = (value != comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
                case "=":
                    result = (value == comparisonValue) ? comparisonIfValue : comparisonElseValue;
                    break;
            }

            return result;
        }
    }
    }
