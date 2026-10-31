using MobiFlight.BrowserMessages.Incoming.Converter;
using MobiFlight.ProSim;
using MobiFlight.xplane;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MobiFlight.InputConfig
{
    public class CacheCollection
    {
        public FSUIPC.FSUIPCCacheInterface fsuipcCache;
        public SimConnectMSFS.SimConnectCacheInterface simConnectCache;
        public MobiFlightCacheInterface moduleCache;
        public XplaneCacheInterface xplaneCache;
        public ProSimCacheInterface proSimCache;
        public JoystickManager joystickManager;
    }

    [JsonConverter(typeof(InputActionConverter))]
    abstract public class InputAction : IXmlSerializable, ICloneable
    {
        [JsonProperty("Type")]
        public string Type => GetType().Name;

        public const String Label = "InputAction";
        public const String CacheType = "FSUIPC";
        
        protected System.Globalization.CultureInfo serializationCulture = new System.Globalization.CultureInfo("de");
        abstract public object Clone();
        public System.Xml.Schema.XmlSchema GetSchema() {
            return (null);
        }
        abstract public void ReadXml(System.Xml.XmlReader reader);
        abstract public void WriteXml(System.Xml.XmlWriter writer);

        abstract public void execute(
            CacheCollection cacheCollection,
            InputEventArgs e,
            List<ConfigRefValue> configRefs);

        /// <summary>
        /// Whether this action and <paramref name="other"/> both target the same action,
        /// such that a PRESS-side "begin" on this action is safe to pair with a
        /// RELEASE-side "end" on <paramref name="other"/>.
        /// </summary>
        public virtual bool TargetsSameCommandAs(InputAction other) => false;

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
            catch (Exception ex)
            {
                if(Log.LooksLikeExpression(expression))
                    Log.Instance.log($"Exception on NCalc evaluate => {expression}: {ex.Message}", LogSeverity.Error);
            }
            return expression; 
        }

    }
}