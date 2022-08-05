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

namespace MobiFlight.UI.Panels.Action
{
    public partial class MSFS2020CustomInputPanel : UserControl
    {
        ErrorProvider errorProvider = new ErrorProvider();

        public MSFS2020CustomInputPanel()
        {
            InitializeComponent();
            hubHopPresetPanel1.Mode = Config.HubHopPanelMode.Input;
            hubHopPresetPanel1.LoadPresets();
            Disposed += (sender, args) => { hubHopPresetPanel1.Dispose(); };
        }       

        internal void syncFromConfig(InputConfig.MSFS2020CustomInputAction inputAction)
        {
            hubHopPresetPanel1.syncFromConfig(inputAction);
        }

        internal InputConfig.InputAction ToConfig()
        {
            return hubHopPresetPanel1.ToConfig();            
        }

        internal void syncFromConfig(MSFS2020EventIdInputAction inputAction)
        {
            hubHopPresetPanel1.syncFromConfig(inputAction);
        }
    }
}
