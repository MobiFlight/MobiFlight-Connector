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
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void OnProgressUpdated(object sender, ProgressUpdateEvent e)
        {
            if (this.InvokeRequired)
                this.Invoke(new EventHandler<ProgressUpdateEvent>(OnProgressUpdated), new object[] { sender, e });

            progressBar1.Value = e.Current;
            progressBar1.Maximum = e.Total;
            ProgressLabel.Text = String.Format("Step {0} / {1}", e.Current, e.Total);
            StepDescriptionLabel.Text = e.ProgressMessage;

            if (e.Current == e.Total)
            {
                if (this.InvokeRequired)
                    this.Invoke(new VoidDelegate(Close), new object[] { });
                else 
                    this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
