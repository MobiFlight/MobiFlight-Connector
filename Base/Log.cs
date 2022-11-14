using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using MobiFlight.UI.Panels;
using System.Diagnostics;

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
        public static char[] ExpressionIndicator = { '=', '+', '-', '/', '%', '(', ')' };
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

        public string GetCallingMethod()
        {
            var stackTrace = new StackTrace();
            var callingMethod = stackTrace.GetFrame(2).GetMethod();
            var callingClass = callingMethod.ReflectedType;
            return $"{callingClass.Name}.{callingMethod.Name}()";

        }

        public void log(String message, LogSeverity severity)
        {
            if (!Enabled) return;
            if ((int)severity < (int)Severity) return;

            if (Severity == LogSeverity.Debug)
            {
                message = $"{GetCallingMethod()}: {message}";
            }

            foreach (ILogAppender appender in appenderList)
            {
                appender.log(message, severity);
            }
        }

        public void AddAppender(ILogAppender appender)
        {
            appenderList.Add(appender);
        }

        public static bool LooksLikeExpression(String expression)
        {
            return expression.IndexOfAny(ExpressionIndicator) != -1;
        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();
            result["Log.Level"] = (int)Severity;
            result["Log.Enabled"] = Enabled ? 1 : 0;
            return result;
        }

        public bool Enabled { get; set; }
        public bool LogJoystickAxis { get; set; }
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
                textBox.BeginInvoke(new logCallback(log), new object[] { message, severity });
            }
            else
            {
                    textBox.Text = DateTime.Now + "(" + DateTime.Now.Millisecond + ")" + ": " + message + Environment.NewLine + textBox.Text;
            }
        }
    }

    public class LogAppenderLogPanel : ILogAppender
    {
        private LogPanel panel = null;
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void logCallback(string message, LogSeverity severity);

        public LogAppenderLogPanel(LogPanel panel)
        {
            this.panel = panel;
        }

        public void log(string message, LogSeverity severity)
        {
            if (panel == null) return;

            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (panel.InvokeRequired)
            {
                panel.BeginInvoke(new logCallback(log), new object[] { message, severity });
            }
            else
            {
                panel.log(message, severity);
            }
        }
    }

    public class LogAppenderFile : ILogAppender
    {
        private String FileName = "log.txt";
        private StreamWriter writer = null;
        // This delegate enables asynchronous calls for setting
        // the text property on a TextBox control.
        delegate void logCallback(string message, LogSeverity severity);

        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        public LogAppenderFile()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);
        }

        public async void log(string message, LogSeverity severity)
        {
            await Task.Run(() =>
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
                catch
                {
                    // Fix for https://github.com/MobiFlight/MobiFlight-Connector/issues/757
                    // If something goes wrong writing to the log file it's just the log file, no need to crash
                    // or do anything special. Just ignore the exception and keep going.
                }
                finally
                {
                    // Release lock
                    _readWriteLock.ExitWriteLock();
                }
            });
        }
    }
}

