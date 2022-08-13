using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using MobiFlight.UI.Panels;

namespace MobiFlight
{
    public class LogAppenderTextBox : ILogAppender
    {
        private TextBox textBox = null;
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void logCallback(string message, LogSeverity severity);

        public LogAppenderTextBox(TextBox newTextBox)
        {
            textBox = newTextBox;
        }

        public void log(string message, LogSeverity severity)
        {
            if (textBox == null) return;

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (textBox.InvokeRequired)
            {
                textBox.BeginInvoke(new logCallback(log), new object[] { message, severity });
            }
            else
            {
                    textBox.Text = DateTime.Now + "(" + DateTime.Now.Millisecond + ")" + ": " + message + Environment.NewLine + textBox.Text;
            }
        }
    }
}

