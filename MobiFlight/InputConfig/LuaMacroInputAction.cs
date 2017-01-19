using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.InputConfig
{
    class LuaMacroInputAction : InputAction
    {
        const Int16 OFFSET_MACRO_PARAM = 0x0D6C;
        const Int16 OFFSET_MACRO_NAME = 0x0D70;
        public String MacroName = "";
        public const String Label = "Lua Macro";
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
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("type", TYPE);
            writer.WriteAttributeString("macroName", MacroName);
        }

        public override void execute(FSUIPC.FSUIPCCacheInterface cache)
        {
            if (MacroName == "") return;

            FSUIPC.FSUIPCConfigItem cfg = new FSUIPC.FSUIPCConfigItem();
            cfg.FSUIPCOffset = OFFSET_MACRO_NAME;
            cfg.FSUIPCOffsetType = FSUIPCOffsetType.String;
            cfg.FSUIPCSize = (byte)(MacroName.Length);
            cfg.Value = MacroName;

            // later provide param for value too
            Log.Instance.log("LuaMacoInputAction:Execute : Calling macro " + MacroName, LogSeverity.Debug);
            FSUIPC.FsuipcHelper.executeWrite(MacroName, cfg, cache);
            cache.ForceUpdate();
        }
    }
}
