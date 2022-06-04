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
        }

        internal void syncToConfig(OutputConfigItem config)
        {

            config.XplaneDataRef.Path = DataRefTextBox.Text;
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            // Sync the transform panel
            transformOptionsGroup1.syncFromConfig(config);
            DataRefTextBox.Text = config.XplaneDataRef.Path;
        }
    }
}
