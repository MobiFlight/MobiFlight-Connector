using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Config;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class XplaneInputPanel : UserControl, IPanelConfigSync
    {
        public XplaneInputPanel()
        {
            InitializeComponent();
            hubHopPresetPanel1.FlightSimType = FlightSimType.XPLANE;
            hubHopPresetPanel1.Mode = Config.HubHopPanelMode.Input;
            hubHopPresetPanel1.PresetFile = @"Presets\xplane_hubhop_presets.json";
            Disposed += (sender, args) => { hubHopPresetPanel1.Dispose(); };
        }
        public void LoadPresets(ProjectInfo projectInfo)
        {
            hubHopPresetPanel1.LoadPresets(projectInfo);
        }
        public void syncFromConfig(object config)
        {
            XplaneInputAction inputAction = config as XplaneInputAction;
            hubHopPresetPanel1.syncFromConfig(inputAction);
        }

        public InputAction ToConfig()
        {
            return hubHopPresetPanel1.ToConfig();
        }

    }
}