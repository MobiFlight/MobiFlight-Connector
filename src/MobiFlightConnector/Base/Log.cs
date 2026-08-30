using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Forms;

namespace MobiFlight
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogSeverity
    {
        [EnumMember(Value = "debug")]
        Debug = 0,
        [EnumMember(Value = "info")]
        Info = 1,
        [EnumMember(Value = "warn")]
        Warn = 2,
        [EnumMember(Value = "error")]
        Error = 3
    }

    public static class LogSeverityExtensions
    {
        public static String PythonLogLevel(this LogSeverity severity)
        {
            switch (severity)
            {
                case LogSeverity.Debug:
                    return "DEBUG";
                case LogSeverity.Info:
                    return "INFO";
                case LogSeverity.Warn:
                    return "WARNING";
                case LogSeverity.Error:
                    return "ERROR";
                default:
                    return "WARNING";
            }
        }

        public static bool SeverityFromPythonLogLevel(string logLevel, out LogSeverity severity)
        {
            switch (logLevel)
            {
                case "DEBUG":
                    severity = LogSeverity.Debug;
                    return true;
                case "INFO":
                    severity = LogSeverity.Info;
                    return true;
                case "WARNING":
                    severity = LogSeverity.Warn;
                    return true;
                case "ERROR":
                    severity = LogSeverity.Error;
                    return true;
                default:
                    severity = LogSeverity.Info;
                    return false;
            }
        }
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

        public void ClearAppenders()
        {
            appenderList.Clear();
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

    // Writes log lines from a single dedicated background thread instead of spawning a
    // Task.Run (and opening/closing the file) per log call. That old approach caused
    // ThreadPool starvation under LogSeverity.Debug, where startup could take minutes.
    public class LogAppenderFile : ILogAppender, IDisposable
    {
        private const int MaxQueueLength = 10000;

        private readonly String FileName = "log.txt";
        private readonly BlockingCollection<string> queue = new BlockingCollection<string>(MaxQueueLength);
        private readonly Thread writerThread;

        public LogAppenderFile()
        {
            if (File.Exists(FileName))
                File.Delete(FileName);

            writerThread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "LogAppenderFile"
            };
            writerThread.Start();
        }

        public void CopyToClipboard()
        {
            if (File.Exists(FileName))
            {
                // FileShare.ReadWrite lets us read the file while the writer thread still has it open.
                string fileContents;
                using (var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    fileContents = sr.ReadToEnd();
                }
                System.Windows.Forms.Clipboard.SetText(fileContents);
            }
            else
            {
                // File doesn't exist so throw an exception.
                throw new FileLoadException(FileName);
            }
        }

        public void log(string message, LogSeverity severity)
        {
            String msg = DateTime.Now + "(" + DateTime.Now.Millisecond + ")" + ": " + message;
            try
            {
                // Never let logging block the caller. If the writer thread can't keep up
                // (queue full) or has already shut down, just drop the line.
                queue.TryAdd(msg);
            }
            catch (InvalidOperationException)
            {
                // CompleteAdding() has already been called (shutting down).
            }
        }

        private void ProcessQueue()
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read))
                {
                    AutoFlush = false
                };
            }
            catch
            {
                // Fix for https://github.com/MobiFlight/MobiFlight-Connector/issues/757
                // If the log file can't even be opened, just drain the queue so callers
                // never block on a full queue, and give up on writing anything.
                foreach (var _ in queue.GetConsumingEnumerable()) { }
                return;
            }

            using (sw)
            {
                foreach (string msg in queue.GetConsumingEnumerable())
                {
                    try
                    {
                        sw.WriteLine(msg);
                        // Flush once we've drained the current backlog rather than on every
                        // line, so a burst of messages doesn't cost a flush each.
                        if (queue.Count == 0) sw.Flush();
                    }
                    catch
                    {
                        // Same rationale as above: a broken log line shouldn't crash the app.
                    }
                }
                sw.Flush();
            }
        }

        public void Dispose()
        {
            queue.CompleteAdding();
            writerThread.Join(2000);
            queue.Dispose();
        }
    }
}