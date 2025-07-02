using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Settings
{
    public partial class ProSimPanel : UserControl
    {
        public ProSimPanel()
        {
            InitializeComponent();
        }

        public void LoadSettings()
        {
            proSimHostTextBox.Text = Properties.Settings.Default.ProSimHost;
            proSimPortTextBox.Text = Properties.Settings.Default.ProSimPort.ToString();
            proSimAutoConnectCheckBox.Checked = Properties.Settings.Default.ProSimAutoConnectEnabled;
            proSimMaxRetryNumericUpDown.Value = Properties.Settings.Default.ProSimMaxRetryAttempts;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ProSimHost = proSimHostTextBox.Text;

            var port = Properties.Settings.Default.ProSimPort;
            if (Int32.TryParse(proSimPortTextBox.Text, out var result))
            {
                port = result;
            }
            Properties.Settings.Default.ProSimPort = port;
            
            Properties.Settings.Default.ProSimAutoConnectEnabled = proSimAutoConnectCheckBox.Checked;
            Properties.Settings.Default.ProSimMaxRetryAttempts = (int)proSimMaxRetryNumericUpDown.Value;
        }
    }
}
