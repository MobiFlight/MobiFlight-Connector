using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MobiFlight
{
    public class Transformation : IXmlSerializable, ICloneable
    {
        private System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("en");
        public String Expression = "$";
        public int SubStrStart;
        public int SubStrEnd;

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // read precondition settings if present
            if (reader["expression"] != null)
                Expression = reader["expression"] as String;

            if (reader["substrStart"] != null)
                SubStrStart = int.Parse(reader["substrStart"]);

            if (reader["substrEnd"] != null)
                SubStrEnd = int.Parse(reader["substrEnd"]);

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("transformation");
            writer.WriteAttributeString("expression", Expression);
            writer.WriteAttributeString("substrStart", Expression);
            writer.WriteAttributeString("substrEnd", Expression);
            writer.WriteEndElement();
        }

        public object Clone()
        {
            Transformation Clone = new Transformation();
            Clone.Expression = Expression;
            Clone.SubStrStart = SubStrStart;
            Clone.SubStrEnd = SubStrEnd;
            return Clone;
        }

        public double Apply(double value)
        {
            double result = 0;
            string exp = Expression.Replace("$", value.ToString());

            var ce = new NCalc.Expression(exp);
            try
            {
                result = Double.Parse(ce.Evaluate().ToString());
            }
            catch (Exception e)
            {
                Log.Instance.log("checkPrecondition : Exception on NCalc evaluate", LogSeverity.Warn);
                throw new Exception(MainForm._tr("uiMessageErrorOnParsingExpression"));
            }

            return result;
        }

        public string Apply(string value)
        {
            return value.Substring(SubStrStart, (SubStrEnd - SubStrStart));
        }
    }
}
