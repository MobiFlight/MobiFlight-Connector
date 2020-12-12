using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MobiFlight.FSUIPC;

namespace MobiFlight.InputConfig
{
    class LuaMacroInputAction : InputAction
    {
        public String MacroName = "";
        public Int32 MacroValue = 0;
        public new const String Label = "Lua Macro";
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
            MacroValue = Int32.Parse(reader["value"]);
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("macroName", MacroName);
            writer.WriteAttributeString("value", MacroValue.ToString());
        }

        public override void execute(FSUIPC.FSUIPCCacheInterface cache, SimConnectMSFS.SimConnectCacheInterface simConnectCache, MobiFlightCacheInterface moduleCache)
        {
            if (MacroName == "") return;
            
            Log.Instance.log("LuaMacoInputAction:Execute : Calling macro " + MacroName, LogSeverity.Debug);
            cache.executeMacro(MacroName, MacroValue);
        }
    }
}
