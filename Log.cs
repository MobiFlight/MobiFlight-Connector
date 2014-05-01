using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight
{
    public enum LogSeverity    
    {
        Info = 1,
        Error = 2
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

        public LogAppenderTextBox(TextBox newTextBox)
        {
            textBox = newTextBox;
        }

        public void log(string message, LogSeverity severity)
        {
            if (textBox == null) return;
            textBox.Text += message + Environment.NewLine;
        }
    }
}

