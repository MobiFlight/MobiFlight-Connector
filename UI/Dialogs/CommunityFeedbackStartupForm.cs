using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class CommunityFeedbackStartupForm : Form
    {
        public bool CommunityFeedback = false;
        public CommunityFeedbackStartupForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CommunityFeedback = true;
            this.DialogResult = DialogResult.OK;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.mobiflight.com/en/documentation/community-feedback-program.html");
        }
    }
}
