using MobiFlight.InputConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class XplaneInputPanel : UserControl
    {
        public XplaneInputPanel()
        {
            InitializeComponent();
            hubHopPresetPanel1.FlightSimType = FlightSimType.XPLANE;
            hubHopPresetPanel1.Mode = Config.HubHopPanelMode.Input;
            hubHopPresetPanel1.PresetFile = @"Presets\xplane_hubhop_presets.json";
            hubHopPresetPanel1.LoadPresets();
            Disposed += (sender, args) => { hubHopPresetPanel1.Dispose(); };
        }
        internal void syncFromConfig(InputConfig.XplaneInputAction inputAction)
        {
            hubHopPresetPanel1.syncFromConfig(inputAction);
        }

        internal InputConfig.InputAction ToConfig()
        {
            return hubHopPresetPanel1.ToConfig();
        }

    }
}
