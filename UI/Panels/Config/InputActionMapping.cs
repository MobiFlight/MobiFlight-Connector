using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Action;
using System;
using System.Collections.Generic;

namespace MobiFlight.UI.Panels.Config
{
    internal class InputActionMapping
    {
        public static Dictionary<Type, string> InputActionTypesToInputLabels = new Dictionary<Type, string>
        {
            { typeof(FsuipcOffsetInputAction), FsuipcOffsetInputAction.Label },
            { typeof(KeyInputAction), KeyInputAction.Label },
            { typeof(EventIdInputAction), EventIdInputAction.Label },
            { typeof(PmdgEventIdInputAction), PmdgEventIdInputAction.Label },
            { typeof(JeehellInputAction), JeehellInputAction.Label },
            { typeof(VJoyInputAction), VJoyInputAction.Label },
            { typeof(LuaMacroInputAction), LuaMacroInputAction.Label },
            { typeof(RetriggerInputAction), RetriggerInputAction.Label },
            { typeof(MSFS2020EventIdInputAction), MSFS2020EventIdInputAction.Label },
            { typeof(MSFS2020CustomInputAction), MSFS2020CustomInputAction.Label },
            { typeof(VariableInputAction), VariableInputAction.Label },
            { typeof(XplaneInputAction), XplaneInputAction.Label }
        };

        public static Dictionary<string, Type> InputLabelsToConfigPanelTypes = new Dictionary<string, Type>
        {
            { FsuipcOffsetInputAction.Label, typeof(FsuipcConfigPanel) },
            { KeyInputAction.Label, typeof(KeyboardInputPanel) },
            { EventIdInputAction.Label, typeof(EventIdInputPanel) },
            { PmdgEventIdInputAction.Label, typeof(PmdgEventIdInputPanel) },
            { JeehellInputAction.Label, typeof(JeehellInputPanel) },
            { VJoyInputAction.Label, typeof(VJoyInputPanel) },
            { LuaMacroInputAction.Label, typeof(LuaMacroInputPanel) },
            { RetriggerInputAction.Label, typeof(RetriggerInputPanel) },
            { MSFS2020EventIdInputAction.Label, typeof(MSFS2020CustomInputPanel) },
            { MSFS2020CustomInputAction.Label, typeof(MSFS2020CustomInputPanel) },
            { VariableInputAction.Label, typeof(VariableInputPanel) },
            { XplaneInputAction.Label, typeof(XplaneInputPanel) }
        };
    }
}
