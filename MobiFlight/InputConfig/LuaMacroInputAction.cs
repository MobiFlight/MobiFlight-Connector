using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobiFlight.FSUIPC;

namespace MobiFlight.InputConfig
{
    public class LuaMacroInputAction : InputAction
    {
        public String MacroName = "";
        public String MacroValue = "0";
        public new const String Label = "FSUIPC - Lua Macro";
        public const String TYPE = "LuaMacroInputAction";

        public override object Clone()
        {
            LuaMacroInputAction clone = new LuaMacroInputAction();
            clone.MacroName = MacroName;

            return clone;
        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {

            MacroName = reader["macroName"];
            MacroValue = reader["value"];
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("macroName", MacroName);
            writer.WriteAttributeString("value", MacroValue.ToString());
        }

        public override void execute(
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefs)
        {
            if (MacroName == "") return;

            String value = MacroValue;

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

            Log.Instance.log("LuaMacoInputAction:Execute : Calling macro " + MacroName, LogSeverity.Debug);
            cacheCollection.fsuipcCache.executeMacro(MacroName, int.Parse(value));
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is LuaMacroInputAction &&
                MacroName == (obj as LuaMacroInputAction).MacroName &&
                MacroValue == (obj as LuaMacroInputAction).MacroValue;
        }
    }
}
