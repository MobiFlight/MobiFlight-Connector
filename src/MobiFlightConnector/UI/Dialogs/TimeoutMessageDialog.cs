using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Dialogs
{
    public partial class TimeoutMessageDialog : Form
    {
        private Timer TimeOutTimer = new Timer();
        public int Timeout = 10;
        public int Timestamp = 0;
        public DialogResult DefaultDialogResult = DialogResult.OK;
        private int TimeElapsedInMs;

        [System.ComponentModel.Category("Custom")]
        public String Message
        {
            get { return label1.Text; }
            set { if (label1.Text != value) label1.Text = value; }
        }

        public bool HasCancelButton { 
            get { return CancelButton.Visible; } 
            set { CancelButton.Visible = value;  } 
        }

        public bool HasDontShowAgain { 
            get { return checkBox1.Visible; }
            set { checkBox1.Visible = value; } 
        }
        public bool DontShowAgain { 
            get { return checkBox1.Checked; }
            set { checkBox1.Checked = value;  } 
        }

        new public static DialogResult Show(string Message, String Title)
        {
            return Show(Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        new public static DialogResult Show(string Message, String Title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            TimeoutMessageDialog tmd = new TimeoutMessageDialog();
            tmd.StartPosition = FormStartPosition.CenterParent;
            tmd.HasCancelButton = buttons == MessageBoxButtons.OKCancel;
            tmd.Message = Message;
            tmd.Text = Title;
            return tmd.ShowDialog(tmd.Parent);
        }

        new public static DialogResult Show(Control Parent, string Message, String Title, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            TimeoutMessageDialog tmd = new TimeoutMessageDialog();
            tmd.Parent = Parent;
            tmd.StartPosition = FormStartPosition.CenterParent;
            tmd.HasCancelButton = buttons == MessageBoxButtons.OKCancel;
            tmd.Message = Message;
            tmd.Text = Title;
            return tmd.ShowDialog(tmd.Parent);
        }

        public TimeoutMessageDialog()
        {
            InitializeComponent();
            HasDontShowAgain = false;
        }        

        public void ReplaceContent(Control control)
        {
            ContentPanel.Controls.Clear();
            ContentPanel.Controls.Add(control);
        }

        private void TimeoutMessageDialog_Shown(object sender, EventArgs e)
        {
            progressBar1.Maximum = Timeout * 1000;
            TimeOutTimer.Tick -= TimeOutTimer_Tick;
            TimeOutTimer.Tick += TimeOutTimer_Tick;
            TimeOutTimer.Interval += 10;
            TimeElapsedInMs = 0;
            TimeOutTimer.Start();
        }   

        private void TimeOutTimer_Tick(object sender, EventArgs e)
        {
            TimeElapsedInMs += TimeOutTimer.Interval;
            progressBar1.Value = Math.Max(progressBar1.Maximum - TimeElapsedInMs, 0);

            if (progressBar1.Maximum <= TimeElapsedInMs)
            {
                TimeOutTimer.Stop();
                this.DialogResult = DefaultDialogResult;
                this.Close();
            }
        }
    }
}
