using System;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class XplaneDataRefPanel : UserControl
    {
        public event EventHandler ModifyTabLink;
        public XplaneDataRefPanel()
        {
            InitializeComponent();
            transformOptionsGroup1.setMode(true);
            hubHopPresetPanel1.PresetFile = @"Presets\xplane_hubhop_presets.json";
            hubHopPresetPanel1.Mode = HubHopPanelMode.Output;
            hubHopPresetPanel1.FlightSimType = FlightSimType.XPLANE;
            hubHopPresetPanel1.LoadPresets();
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            hubHopPresetPanel1.syncToConfig(config);
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // Sync the transform panel
            transformOptionsGroup1.syncFromConfig(config);
            hubHopPresetPanel1.syncFromConfig(config);
        }
    }
}
