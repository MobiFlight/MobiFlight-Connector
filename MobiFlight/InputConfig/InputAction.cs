using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    abstract public class InputAction : IXmlSerializable, ICloneable
    {
        public const String Label = "InputAction";
        public const String CacheType = "FSUIPC";
        public char[] ExpressionIndicator = { '=', '+', '-', '/', '%', '(', ')' };

        protected System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");
        abstract public object Clone();
        public System.Xml.Schema.XmlSchema GetSchema() {
            return (null);
        }
        abstract public void ReadXml(System.Xml.XmlReader reader);
        abstract public void WriteXml(System.Xml.XmlWriter writer);

        abstract public void execute(
            FSUIPC.FSUIPCCacheInterface fsuipcCache, 
            SimConnectMSFS.SimConnectCacheInterface simConnectCache, 
            MobiFlightCacheInterface moduleCache, 
            InputEventArgs e,
            List<ConfigRefValue> configRefs);

        public virtual string Replace(string expression, List<Tuple<string, string>> replacements) {
            if (replacements.Count == 0) return expression;

            foreach (Tuple<string, string> replacement in replacements)
            {
                expression = expression.Replace(replacement.Item1, replacement.Item2);
            }

            var ce = new NCalc.Expression(expression);
            try
            {
                expression = (ce.Evaluate()).ToString();
            }
            catch
            {
                if(LooksLikeExpression(expression))
                    Log.Instance.log("InputAction.Replace : Exception on NCalc evaluate => " + expression , LogSeverity.Warn);
            }
            return expression; 
        }

        private bool LooksLikeExpression(String expression)
        {
            return expression.IndexOfAny(ExpressionIndicator) != -1;
        }
    }
}
