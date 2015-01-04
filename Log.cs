using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MobiFlight
{
    public enum LogSeverity    
    {
        Debug = 0,
        Info = 1,
        Warn = 2,
        Error = 3
    }

    public sealed class Log
    {
        private static readonly Log instance = new Log();
        private List<ILogAppender> appenderList;
        private Log() { appenderList = new List<ILogAppender>(); }

        public static Log Instance
        {
            get 
            {
                return instance;
            }
        }

        public LogSeverity Severity { get; set; }

        public void log(String message, LogSeverity severity)
        {
            if (!Enabled) return;
            if ((int)severity < (int)Severity) return;

            foreach (ILogAppender appender in appenderList)
            {
                appender.log(message, severity);
            }
        }

        public void AddAppender(ILogAppender appender)
        {
            appenderList.Add(appender);
        }

        public bool Enabled { get; set; }
    }

    public interface ILogAppender 
    {
        void log(String message, LogSeverity severity);
    }

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
                logCallback d = new logCallback(log);
                textBox.FindForm().Invoke(d, new object[] { message, severity });
            }
            else
            {
                textBox.Text = DateTime.Now + ": " + message + Environment.NewLine + textBox.Text;
            }
        }
    }
}

