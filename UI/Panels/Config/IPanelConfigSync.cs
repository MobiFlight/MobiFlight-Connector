using MobiFlight.InputConfig;

namespace MobiFlight.UI.Panels.Config
{
    internal interface IPanelConfigSync
    {
        InputAction ToConfig();
        void syncFromConfig(object config);
    }
}
