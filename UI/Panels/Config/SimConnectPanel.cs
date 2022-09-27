using MobiFlight.OutputConfig;
using MobiFlight.HubHop;
using MobiFlight.UI.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{

    public partial class SimConnectPanel : UserControl
    {
 
        public SimConnectPanel()
        {
            InitializeComponent();

            transformOptionsGroup1.setMode(true);
            transformOptionsGroup1.ShowSubStringPanel(false);

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
