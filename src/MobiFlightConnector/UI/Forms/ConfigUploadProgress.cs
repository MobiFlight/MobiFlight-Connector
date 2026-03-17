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
    public partial class ConfigUploadProgress : Form
    {
        delegate void setTextCallback(string text);
        delegate void setProgressCallback(int value);

        public ConfigUploadProgress()
        {
            InitializeComponent();
        }

        public int Progress
        {
            get { return progressBar1.Value; }
            set
            {
                if (progressBar1.InvokeRequired) progressBar1.Invoke(new setProgressCallback(_setProgress), new object[] { value });
                else
                    progressBar1.Value = value;
            }
        }

        private void _setProgress(int value)
        {
            Progress = value;
        }

        public string Status
        {
            get { return label1.Text; }
            set {
                if (label1.InvokeRequired) label1.Invoke(new setTextCallback(_setStatus), new object[] { value });
                else
                    label1.Text = value;
            }
        }

        private void _setStatus (String text)
        {
            Status = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
