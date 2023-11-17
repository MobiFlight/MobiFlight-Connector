using System;
using System.Collections.Generic;
using System.Xml;

namespace MobiFlight.Modifier
{
    public class Transformation : ModifierBase
    {
        public String Expression = "$";

        public override void ReadXml(XmlReader reader)
        {
            if (reader["active"] != null)
                Active = bool.Parse(reader["active"]);
            // read precondition settings if present
            if (reader["expression"] != null)
                Expression = reader["expression"] as String;
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("transformation");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("expression", Expression);
            writer.WriteEndElement();
        }

        public override object Clone()
        {
            Transformation Clone = new Transformation();
            Clone.Active = Active;
            Clone.Expression = Expression;
            return Clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is Transformation &&
                this.Active == (obj as Transformation).Active &&
                this.Expression == (obj as Transformation).Expression;
        }

        public override ConnectorValue Apply(ConnectorValue value, List<ConfigRefValue> configRefs)
        {
            ConnectorValue result = value;

            switch (value.type)
            {
                case FSUIPCOffsetType.Float:
                case FSUIPCOffsetType.Integer:
                    string tmpValue = Apply(value.Float64.ToString(), configRefs);
                    if (Double.TryParse(tmpValue, out value.Float64))
                    {
                        value.Float64 = value.Float64;
                    } 
                    else 
                    {
                        // Expression has now made this a string
                        value.type = FSUIPCOffsetType.String;
                        value.String = tmpValue;
                    }
                    break;

                case FSUIPCOffsetType.String:
                    value.String = Apply(value.String, configRefs);
                    break;
            }

            return result;
        }


        protected string Apply(string value, List<ConfigRefValue> configRefs)
        {
            string result = value;

            // we have to use the US culture because "." must be used as decimal separator
            string exp = Expression.Replace("$", value.ToString());

            foreach (ConfigRefValue configRef in configRefs)
            {
                exp = exp.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            var ce = new NCalc.Expression(exp);
            string ncalcResult = null;
            try
            {
                ncalcResult = ce.Evaluate().ToString();
            }
            catch (Exception e)
            {
                Log.Instance.log($"Exception on NCalc evaluate: {e.Message}", LogSeverity.Warn);
                throw new Exception(i18n._tr("uiMessageErrorOnParsingExpression"));
            }

            if (ncalcResult!=null)
            {
                double dValue;
                if (double.TryParse(ncalcResult, out dValue)) {
                    result = dValue.ToString();
                }
                else
                    result = ncalcResult;
            }

            return result;
        }
        public override string ToSummaryLabel()
        {
            return $"Expression: {Expression}";
        }
    }
}
