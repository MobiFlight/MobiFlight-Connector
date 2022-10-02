using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class XplaneDataRefPanel : UserControl
    {
        public XplaneDataRefPanel()
        {
            InitializeComponent();
            transformOptionsGroup1.setMode(true);
            transformOptionsGroup1.ShowSubStringPanel(false);
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
