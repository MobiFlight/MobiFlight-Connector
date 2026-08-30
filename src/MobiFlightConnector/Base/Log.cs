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

    public class LogAppenderFile : ILogAppender, IDisposable
    {
        private const int MaxQueueLength = 10000;
        private static readonly TimeSpan WriterShutdownTimeout = TimeSpan.FromSeconds(2);

        private readonly String FileName = "log.txt";
        private readonly BlockingCollection<string> queue = new BlockingCollection<string>(MaxQueueLength);
        private readonly Thread writerThread;

        public LogAppenderFile()
        {
            TryDeleteExistingLogFile();

            writerThread = new Thread(ProcessQueue)
            {
                IsBackground = true,
                Name = "LogAppenderFile"
            };
            writerThread.Start();
        }

        private void TryDeleteExistingLogFile()
        {
            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
            }
            catch
            {
                // OpenAppendStream()'s FileShare.Delete makes this succeed even while another instance still has the file open.
            }
        }

        public void CopyToClipboard()
        {
            if (!File.Exists(FileName))
                throw new FileLoadException(FileName);

            System.Windows.Forms.Clipboard.SetText(ReadWhileWriterHoldsFile());
        }

        private string ReadWhileWriterHoldsFile()
        {
            // FileShare.ReadWrite: the writer thread already holds this file open for Write.
            using var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var sr = new StreamReader(fs);
            return sr.ReadToEnd();
        }

        public void log(string message, LogSeverity severity)
        {
            TryEnqueue($"{DateTime.Now}({DateTime.Now.Millisecond}): {message}");
        }

        private void TryEnqueue(string msg)
        {
            try
            {
                queue.TryAdd(msg);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ObjectDisposedException)
            {
                // Shutting down (CompleteAdding/Dispose already ran) - drop the line instead of crashing the caller.
            }
        }

        private void ProcessQueue()
        {
            StreamWriter writer;
            try
            {
                writer = OpenAppendStream();
            }
            catch
            {
                // Can't log to file (see #757) - drain the queue so log() never blocks on it filling up.
                DrainWithoutWriting();
                return;
            }

            using (writer)
            {
                foreach (string msg in queue.GetConsumingEnumerable())
                    WriteLine(writer, msg);

                writer.Flush();
            }
        }

        private StreamWriter OpenAppendStream()
        {
            // FileShare.Delete lets a fresh LogAppenderFile instance replace this file while we still hold it open.
            var stream = new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read | FileShare.Delete);
            return new StreamWriter(stream) { AutoFlush = false };
        }

        private void DrainWithoutWriting()
        {
            foreach (var _ in queue.GetConsumingEnumerable()) { }
        }

        private void WriteLine(StreamWriter writer, string msg)
        {
            try
            {
                writer.WriteLine(msg);
                if (queue.Count == 0)
                    writer.Flush();
            }
            catch
            {
                // A broken log line shouldn't crash the app.
            }
        }

        public void Dispose()
        {
            queue.CompleteAdding();
            if (writerThread.Join(WriterShutdownTimeout))
                queue.Dispose(); // otherwise the writer thread may still be enumerating it
        }
    }
}