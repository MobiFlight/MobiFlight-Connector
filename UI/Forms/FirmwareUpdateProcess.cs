using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Forms
{
    public partial class FirmwareUpdateProcess : Form
    {
        public FirmwareUpdateProcess()
        {
            InitializeComponent();
            timer1.Interval = 200;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value + 10 > progressBar1.Maximum) progressBar1.Value = 0;
            progressBar1.Value += 10;
        }

        private void FirmwareUpdateProcess_Shown(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            timer1.Start();
        }

        private void FirmwareUpdateProcess_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Stop();
        }
    }
}
