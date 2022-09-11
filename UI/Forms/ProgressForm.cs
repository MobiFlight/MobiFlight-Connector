using MobiFlight.Base;
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
    public partial class ProgressForm : Form
    {
        delegate void VoidDelegate(); 
        Timer timer = new Timer();

        public ProgressForm()
        {
            InitializeComponent();
            //FormClosing += (sender, e) => { timer.Stop(); timer.Dispose(); };
            timer.Interval = 200;
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value >= progressBar1.Maximum - 2)
                return;
            
            progressBar1.Value += 1;
            ProgressLabel.Text = String.Format("Completed {0}%", Math.Round((decimal) progressBar1.Value * 100 / progressBar1.Maximum, 0));
        }

        public void OnProgressUpdated(object sender, ProgressUpdateEvent e)
        {
            if (this.InvokeRequired)
                this.Invoke(new EventHandler<ProgressUpdateEvent>(OnProgressUpdated), new object[] { sender, e });

            timer.Start();  
            progressBar1.Value = e.Current;
            progressBar1.Maximum = e.Total;
            ProgressLabel.Text = String.Format("Completed {0}%", Math.Round((decimal)e.Current * 100 / e.Total, 0));
            StepDescriptionLabel.Text = e.ProgressMessage;

            if (e.Current == e.Total)
            {
                if (this.InvokeRequired)
                    this.Invoke(new EventHandler<ProgressUpdateEvent>(OnProgressCompleted), new object[] { sender, e });
            }
        }

        public void OnProgressCompleted(object sender, ProgressUpdateEvent e)
        {
            timer.Stop();
            progressBar1.Value = e.Current;
            progressBar1.Maximum = e.Total;
            ProgressLabel.Text = String.Format("Completed {0}%", Math.Round((decimal)e.Current * 100 / e.Total, 0));
            StepDescriptionLabel.Text = e.ProgressMessage;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
