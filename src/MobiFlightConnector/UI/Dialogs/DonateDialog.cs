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
    public partial class DonateDialog : Form
    {
        public bool CommunityFeedback = false;
        private Timer timer = new Timer();
        private byte Countdown = 10;

        public DonateDialog()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=7GV3DCC7BXWLY");
            DialogResult = DialogResult.OK;
        }

        private void DonateDialog_Shown(object sender, EventArgs e)
        {
            timer.Interval = 1000;
            timer.Tick += (object t, EventArgs args) => {
                if (--Countdown == 0)
                {
                    DialogResult = DialogResult.No;
                    timer.Stop();
                    timer.Dispose();
                    Close();
                };
            };
            timer.Start();
        }
    }
}
