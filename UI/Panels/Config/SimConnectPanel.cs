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
    public partial class SimConnectPanel : UserControl
    {
        public SimConnectPanel()
        {
            InitializeComponent();
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            config.SimConnectValue.LVar = textBox1.Text;
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            textBox1.Text = config.SimConnectValue.LVar;
        }
    }
}
