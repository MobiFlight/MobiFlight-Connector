using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MobiFlight.InputConfig;
using MobiFlight.UI.Panels.Config;

namespace MobiFlight.UI.Panels.Action
{
    public partial class MSFS2020CustomInputPanel : UserControl, IPanelConfigSync
    {
        ErrorProvider errorProvider = new ErrorProvider();

        public MSFS2020CustomInputPanel()
        {
            InitializeComponent();
            hubHopPresetPanel1.Mode = Config.HubHopPanelMode.Input;
            hubHopPresetPanel1.FlightSimType = FlightSimType.MSFS2020;
            hubHopPresetPanel1.LoadPresets();
            Disposed += (sender, args) => { hubHopPresetPanel1.Dispose(); };
        }       

        public InputConfig.InputAction ToConfig()
        {
            return hubHopPresetPanel1.ToConfig();            
        }

        public void syncFromConfig(object config)
        {
            if (config is MSFS2020CustomInputAction)
            {
                hubHopPresetPanel1.syncFromConfig(config as MSFS2020CustomInputAction);
            }
            else if (config is MSFS2020EventIdInputAction)
            {
                hubHopPresetPanel1.syncFromConfig(config as MSFS2020EventIdInputAction);
            }            
        }
    }
}
