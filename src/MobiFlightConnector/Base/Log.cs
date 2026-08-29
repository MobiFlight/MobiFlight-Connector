using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
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

        public string GetCallingMethod(
            string memberName,
            string sourceFile)
        {
            var sourceFileName = Path.GetFileNameWithoutExtension(sourceFile);
            return $"{sourceFileName}.{memberName}()";

        }

        public void ClearAppenders()
        {
            appenderList.Clear();
        }

        public void log(
            String message,
            LogSeverity severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if (!Enabled) return;
            if ((int)severity < (int)Severity) return;

            if (Severity == LogSeverity.Debug)
            {
                message = $"{GetCallingMethod(memberName, sourceFilePath)}#{lineNumber}: {message}";
            }

            foreach (ILogAppender appender in appenderList)
            {
                appender.log(message, severity);
            }
        }
        
        public void log(
            Exception exception, 
            string message, 
            LogSeverity severity,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int lineNumber = 0) => log($"{message} {exception}", severity, memberName, sourceFilePath, lineNumber);

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

        public void CopyToClipboard()
        {
            if (File.Exists(FileName))
            {
                string fileContents = File.ReadAllText(FileName);
                System.Windows.Forms.Clipboard.SetText(fileContents);
            }
            else
            {
                // File doesn't exist so throw an exception.
                throw new FileLoadException(FileName);
            }
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