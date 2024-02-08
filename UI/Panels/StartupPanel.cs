using MobiFlight.BrowserMessages;
using System;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class StartupPanel : UserControl
    {
        delegate void progressBarCallback(int percent);
        delegate void updateStatusTextCallback(String StatusText);
        delegate void updateProgressBarAndStatusTextCallback(string text, int percent);

        public StartupPanel()
        {
            InitializeComponent();
        }

        public void UpdateStatusText(String StatusText)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.StatusText.InvokeRequired)
            {
                this.StatusText.Invoke(new updateStatusTextCallback(UpdateStatusText), new object[] { StatusText });
            }
            else
            {
                this.StatusText.Text = StatusText;
            }
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
                if (percent > 100) percent = percent / 2;
                this.progressBar.Value = percent;
            }
        }

        public void UpdateProgressBarAndStatusText(string text, int percent)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new updateProgressBarAndStatusTextCallback(UpdateProgressBarAndStatusText), new object[] { text, percent });
            }
            else
            {
                MessageExchange.Instance.Publish(new Message<StatusBarUpdate>(new StatusBarUpdate(text, percent)));
            }
        }

        public int GetProgressBar()
        {
            return this.progressBar.Value;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void StartupPanel_Load(object sender, EventArgs e)
        {

        }
    }
    public class StatusBarUpdate
    {
        public int Value { get; set; }
        public string Text { get; set; }

        public StatusBarUpdate(string text, int value)
        {
            this.Text = text;
            this.Value = value;
        }
    }
}
