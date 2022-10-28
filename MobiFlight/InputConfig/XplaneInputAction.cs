using MobiFlight.Base;
using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class XplaneInputAction : InputAction, ICloneable
    {
        new public const String Label = "X-Plane (all versions)";
        public const String INPUT_TYPE_DATAREF = "DataRef";
        public const String INPUT_TYPE_COMMAND = "Command";
        public const String TYPE = "XplaneInputAction";
        public String InputType = "DataRef";
        public String Path = "";
        public String Expression = "$";
        
        override public object Clone()
        {
            XplaneInputAction clone = new XplaneInputAction();
            clone.InputType = InputType;
            clone.Path = Path.Clone() as String;
            clone.Expression = Expression.Clone() as String;
            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            Path = reader["path"];
            InputType = reader["inputType"];
            Expression = reader["expression"];
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("inputType", InputType);
            writer.WriteAttributeString("path", Path);
            writer.WriteAttributeString("expression", Expression);
        }

        protected virtual String getType()
        {
            return TYPE;
        }

        public override void execute(CacheCollection cacheCollection, 
                            InputEventArgs args,
                            List<ConfigRefValue> configRefs)
        {
            String value = Expression;
            XplaneCacheInterface xplaneCache = cacheCollection.xplaneCache;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();

            if (value.Contains("@"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("@", args.Value.ToString());
                replacements.Add(replacement);
            }

            if (value.Contains("$"))
            {
                float currentValue = xplaneCache.readDataRef(Path);
                Tuple<string, string> replacement = new Tuple<string, string>("$", currentValue.ToString());
                replacements.Add(replacement);
            }


            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            value = Replace(value, replacements);

            if (InputType == INPUT_TYPE_DATAREF)
            {
                try
                {
                    float finalValue = float.Parse(value);
                    xplaneCache.writeDataRef(Path, finalValue);
                }
                catch (Exception)
                {
                }
            } else if (InputType == INPUT_TYPE_COMMAND)
            {
                xplaneCache.sendCommand(Path);
            }
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is XplaneInputAction &&
                Path == (obj as XplaneInputAction).Path &&
                Expression == (obj as XplaneInputAction).Expression;
        }
    }
}
