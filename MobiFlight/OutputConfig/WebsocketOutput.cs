using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static MobiFlight.KeyboardInput;

namespace MobiFlight.OutputConfig
{
    public class WebsocketOutput
    {
        public const string Type = "WebsocketOutput";
        public String Payload { get; set; } = "{}";

        public WebsocketOutput()
        {
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader["payload"] != null && reader["payload"] != "")
            {
                Payload = reader["payload"];
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("payload", Payload);
        }

        public object Clone()
        {
            return new WebsocketOutput() { 
                Payload = Payload.Clone() as string
            };
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is WebsocketOutput && 
                this.Payload.Equals((obj as WebsocketOutput).Payload);
        }

        internal string Apply(string value, List<ConfigRefValue> configRefs)
        {
            string result = Payload;
            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();

            if (result.Contains("$"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("$", value);
                replacements.Add(replacement);
            }

            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(
                                                        item.ConfigRef.Placeholder, 
                                                        item.Value);
                replacements.Add(replacement);
            }

            result = Replace(result, replacements);

            return result;
        }

        public virtual string Replace(string expression, List<Tuple<string, string>> replacements)
        {
            if (replacements.Count == 0) return expression;

            foreach (Tuple<string, string> replacement in replacements)
            {
                expression = expression.Replace(replacement.Item1, replacement.Item2);
            }

            // we don't do any ncalc stuff here... not needed.
            // if calculations have to be done, they can be performed before.

            return expression;
        }
    }
}
