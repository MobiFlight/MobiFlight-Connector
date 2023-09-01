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

            HubHopPresetPanel.Mode = Config.HubHopPanelMode.Output;
            HubHopPresetPanel.FlightSimType = FlightSimType.MSFS2020;
            HubHopPresetPanel.LoadPresets();
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            HubHopPresetPanel.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            HubHopPresetPanel.syncFromConfig(config);
        }
    }    
}
