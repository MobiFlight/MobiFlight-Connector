using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Forms
{
    public partial class SimConnectPanelIndexSelectForm : Form
    {
        public int IndexValue { get; set; }
        public SimConnectPanelIndexSelectForm()
        {
            InitializeComponent();
            IndexValue = 1;
        }

        private void IndexNumberUpDown_ValueChanged(object sender, EventArgs e)
        {
            IndexValue = (int)(sender as NumericUpDown).Value;
        }
    }
}
