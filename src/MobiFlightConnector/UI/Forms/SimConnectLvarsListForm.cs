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
    public partial class SimConnectLvarsListForm : Form
    {
        public String SelectedVariable = null;
        public SimConnectLvarsListForm()
        {
            InitializeComponent();
        }

        public void SetLVarsList(List<String> LVars)
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(LVars.ToArray());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedVariable = (sender as ListBox).SelectedItem.ToString();
        }
    }
}
