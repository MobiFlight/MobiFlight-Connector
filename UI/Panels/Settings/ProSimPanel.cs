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
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ProSimHost = proSimHostTextBox.Text;
        }
    }
}
