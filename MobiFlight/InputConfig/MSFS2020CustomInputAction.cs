using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    public class MSFS2020CustomInputAction : InputAction, ICloneable
    {
        const Int16 BaseOffset = 0x3110;
        const Int16 ParamOffset = 0x3114;

        public new const String Label = "MSFS2020 - Custom Input";
        public new const String CacheType = "SimConnect";
        public const String TYPE = "MSFS2020CustomInputAction";
        public String Command;
        
        override public object Clone()
        {
            MSFS2020CustomInputAction clone = new MSFS2020CustomInputAction();
            clone.Command = Command;

            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            String command = reader["command"];

            Command = command;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", getType());
            writer.WriteAttributeString("command", Command.ToString());
        }

        protected virtual String getType()
        {
            return TYPE;
        }

        public override void execute(
            FSUIPC.FSUIPCCacheInterface fsuipcCache,
            SimConnectMSFS.SimConnectCacheInterface simConnectCache,
            MobiFlightCacheInterface moduleCache,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            String value = Command;

            List<Tuple<string, string>> replacements = new List<Tuple<string, string>>();
            if (value.Contains("@"))
            {
                Tuple<string, string> replacement = new Tuple<string, string>("@", args.Value.ToString());
                replacements.Add(replacement);
            }

            foreach (ConfigRefValue item in configRefs)
            {
                Tuple<string, string> replacement = new Tuple<string, string>(item.ConfigRef.Placeholder, item.Value);
                replacements.Add(replacement);
            }

            value = Replace(value, replacements);

            simConnectCache.SetSimVar(value);
        }
    }
}
