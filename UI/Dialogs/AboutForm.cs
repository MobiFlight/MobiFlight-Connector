using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            label1.Text = label1.Text.Replace("{VERSION}", MainForm.DisplayVersion())
                                     .Replace("{BUILD}", MainForm.Build);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
