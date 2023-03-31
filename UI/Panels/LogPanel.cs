using System;
using System.Reflection;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class LogPanel : UserControl, ILogAppender
    {
        private bool IsDrawingSuspended = false;
        private System.Windows.Forms.Timer CheckDrawingSuspendedTimer = new Timer();
        public LogPanel()
        {
            InitializeComponent();
            listView1.Items.Clear();
            listView1.Resize += ListView1_Resize;
            listView1.DoubleBuffered(true);
            CheckDrawingSuspendedTimer.Interval = 50;
            CheckDrawingSuspendedTimer.Tick += CheckDrawingSuspendedTimer_Tick;
        }

        private void CheckDrawingSuspendedTimer_Tick(object sender, EventArgs e)
        {
            if (IsDrawingSuspended)
            {
                IsDrawingSuspended = false;
                listView1.EndUpdate();
                CheckDrawingSuspendedTimer.Stop();
            }
        }

        private void ListView1_Resize(object sender, EventArgs e)
        {
            int Width = this.Width - this.Padding.Left - this.Padding.Right - listView1.Columns[0].Width - listView1.Columns[1].Width - listView1.Columns[2].Width - 25;
            listView1.Columns[3].Width = Width;
        }

        public void log(string message, LogSeverity severity)
        {
            if (!IsDrawingSuspended)
            {
                IsDrawingSuspended = true;
                listView1.BeginUpdate();
                CheckDrawingSuspendedTimer.Start();
            }
            ListViewItem item = new ListViewItem();
            switch(severity)
            {
                case LogSeverity.Error:
                    item.StateImageIndex = 0;
                    break;
                case LogSeverity.Warn:
                    item.StateImageIndex = 1;
                    break;
                case LogSeverity.Info:
                    item.StateImageIndex = 2;
                    break;
                case LogSeverity.Debug:
                    item.StateImageIndex = 3;
                    break;
            }
            item.SubItems.Add(severity.ToString());
            item.SubItems.Add(DateTime.Now.ToString());
            item.SubItems.Add(message);

            listView1.Items.Add(item);
            listView1.TopItem = item;
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            // Provides copy support for logged events in the log panel.
            if (e.Control && e.KeyCode == Keys.C)
            {
                var copyBuffer = new System.Text.StringBuilder();

                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    // This results in a string that mimics what's shown in the log window:
                    // Debug	2/23/2022 3:49:09 PM	Command: SetModule <1,0,3,        ,0,255;>
                    copyBuffer.AppendLine($"{item.SubItems[1].Text}\t{item.SubItems[2].Text}\t{item.SubItems[3].Text}");
                }

                // Only populate the clipboard if there's something that wound up in the copy buffer.
                if (copyBuffer.Length > 0)
                {
                    System.Windows.Forms.Clipboard.SetDataObject(copyBuffer.ToString());
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }

    public static class ControlExtensions
    {
        public static void DoubleBuffered(this Control control, bool enable)
        {
            var doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }
    }
}
