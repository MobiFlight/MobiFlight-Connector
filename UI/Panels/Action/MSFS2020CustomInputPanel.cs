using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Action
{
    public partial class MSFS2020CustomInputPanel : UserControl
    {
        ErrorProvider errorProvider = new ErrorProvider();

        public MSFS2020CustomInputPanel()
        {
            InitializeComponent();
            hubHopPresetPanel1.Mode = Config.HubHopPanelMode.Input;
        }       

        internal void syncFromConfig(InputConfig.MSFS2020CustomInputAction inputAction)
        {
            hubHopPresetPanel1.syncFromConfig(inputAction);
        }

        internal InputConfig.InputAction ToConfig()
        {
            return hubHopPresetPanel1.ToConfig();            
        }
    }
}
