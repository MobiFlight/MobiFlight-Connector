using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace MobiFlightInstaller
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
                textBox.Invoke(new logCallback(log), new object[] { message, severity });
            }
            else
            {
                textBox.Text = DateTime.Now + "(" + DateTime.Now.Millisecond + ")" + ": " + message + Environment.NewLine + textBox.Text;
            }
        }
    }

    public class LogAppenderFile : ILogAppender
    {
        private String FileName = "install.log.txt";
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void logCallback(string message, LogSeverity severity);

        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        public LogAppenderFile(bool EraseFile)
        {
            if (EraseFile)
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
            }
            
        }

        public void log(string message, LogSeverity severity)
        {
            // Set Status to Locked
            _readWriteLock.EnterWriteLock();
            try
            {
                String msg = DateTime.Now + "(" + DateTime.Now.Millisecond + ")" + ": " + message;
                // Append text to the file
                using (StreamWriter sw = File.AppendText(FileName))
                {
                    sw.WriteLine(msg);
                    sw.Close();
                }
            }
            finally
            {
                // Release lock
                _readWriteLock.ExitWriteLock();
            }
        }
    }
}

