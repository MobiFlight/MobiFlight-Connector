using System;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{

    public partial class SimConnectPanel : UserControl
    {
        public event EventHandler ModifyTabLink;
        public SimConnectPanel()
        {
            InitializeComponent();

            transformOptionsGroup1.setMode(true);
            transformOptionsGroup1.ModifyTabLink += (s, e) => {
                ModifyTabLink?.Invoke(this, e);
            };

            HubHopPresetPanel.Mode = Config.HubHopPanelMode.Output;
            HubHopPresetPanel.FlightSimType = FlightSimType.MSFS2020;
            HubHopPresetPanel.LoadPresets();
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            HubHopPresetPanel.syncToConfig(config);
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // Sync the transform panel
            transformOptionsGroup1.syncFromConfig(config);
            HubHopPresetPanel.syncFromConfig(config);
        }
    }    
}
