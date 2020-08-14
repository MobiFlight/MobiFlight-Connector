using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.Forms
{
    public partial class StartupPanel : UserControl
    {
        delegate void progressBarCallback(int percent);

        public StartupPanel()
        {
            InitializeComponent();
        }

        public void UpdateStatusText(String StatusText)
        {
            this.StatusText.Text = StatusText;
        }

        public void UpdateProgressBar(int percent)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new progressBarCallback(UpdateProgressBar), new object[] { percent });
            }
            else
            {
                this.progressBar.Value = percent;
            }
        }

        public int GetProgressBar()
        {
            return this.progressBar.Value;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
