using System;
using System.Collections.Generic;
using System.Xml;

namespace MobiFlight.Modifier
{
    public class Transformation : ModifierBase
    {
        public string Expression { get; set; } = "$";

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
            var result = value.Clone() as ConnectorValue;

            string exp = Expression.Replace("$", value.Float64.ToString());

            if (value.type == FSUIPCOffsetType.String)
            {
                exp = Expression.Replace("$", value.String);
            }

            foreach (ConfigRefValue configRef in configRefs)
            {
                exp = exp.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            var ce = new NCalc.Expression(exp);
            string ncalcResult;
            try
            {
                ncalcResult = ce.Evaluate().ToString();
            }
            catch (Exception e)
            {
                Log.Instance.log($"Exception on NCalc evaluate: {e.Message}", LogSeverity.Warn);
                throw new Exception(i18n._tr("uiMessageErrorOnParsingExpression"));
            }

            if (ncalcResult != null)
            {
                if (double.TryParse(ncalcResult, out result.Float64) && result.Float64.ToString().Length == ncalcResult.Length)
                {
                    result.type = FSUIPCOffsetType.Float;
                    result.String = result.Float64.ToString();
                }
                else
                {
                    result.type = FSUIPCOffsetType.String;
                    result.String = ncalcResult;
                }
            }

            return result;
        }

        public override string ToSummaryLabel()
        {
            return $"Expression: {Expression}";
        }
    }
}
