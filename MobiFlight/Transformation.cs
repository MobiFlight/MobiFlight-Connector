using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class Transformation : IXmlSerializable, ICloneable
    {
        public bool Active = false;
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("en");
        public String Expression = "$";
        public int SubStrStart = 0;
        public int SubStrEnd = 7;

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["active"] != null)
                Active = bool.Parse(reader["active"]);
            // read precondition settings if present
            if (reader["expression"] != null)
                Expression = reader["expression"] as String;
            
            if (reader["substrStart"] != null)
                SubStrStart = int.Parse(reader["substrStart"]);

            if (reader["substrEnd"] != null)
                SubStrEnd = int.Parse(reader["substrEnd"]);
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("transformation");
                writer.WriteAttributeString("active", Active.ToString());
                writer.WriteAttributeString("expression", Expression);
                writer.WriteAttributeString("substrStart", SubStrStart.ToString());
                writer.WriteAttributeString("substrEnd", SubStrEnd.ToString());
            writer.WriteEndElement();
        }

        public object Clone()
        {
            Transformation Clone = new Transformation();
            Clone.Active = Active;
            Clone.Expression = Expression;
            Clone.SubStrStart = SubStrStart;
            Clone.SubStrEnd = SubStrEnd;
            return Clone;
        }

        public override bool Equals(object obj)
        {
            return
                obj != null && obj is Transformation &&
                this.Active == (obj as Transformation).Active &&
                this.Expression == (obj as Transformation).Expression &&
                this.SubStrStart == (obj as Transformation).SubStrStart &&
                this.SubStrEnd == (obj as Transformation).SubStrEnd;
        }

        public double Apply(double value, List<ConfigRefValue> configRefs)
        {
            double result = value;

            if (!Active) return result;

            // we have to use the US culture because "." must be used as decimal separator
            string exp = Expression.Replace("$", value.ToString(new CultureInfo("en-US")));

            foreach (ConfigRefValue configRef in configRefs)
            {
                exp = exp.Replace(configRef.ConfigRef.Placeholder, configRef.Value);
            }

            var ce = new NCalc.Expression(exp);
            try
            {
                result = Double.Parse(ce.Evaluate().ToString());
            }
            catch (Exception e)
            {
                Log.Instance.log("checkPrecondition : Exception on NCalc evaluate", LogSeverity.Warn);
                throw new Exception(i18n._tr("uiMessageErrorOnParsingExpression"));
            }

            return result;
        }

        public string Apply(string value)
        {
            if (!Active) return value;

            if (SubStrStart > value.Length) return "";

            int length = (SubStrEnd - SubStrStart);
            if (SubStrEnd > value.Length) length = value.Length - SubStrStart;

            return value.Substring(SubStrStart, length);
        }
    }
}
