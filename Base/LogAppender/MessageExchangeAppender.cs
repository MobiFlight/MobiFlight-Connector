using MobiFlight.BrowserMessages.Outgoing;
using System;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace MobiFlight.Base.LogAppender
{
    public class MessageExchangeAppender : ILogAppender
    {
        private ConcurrentQueue<LogEntry> LogQueue = new ConcurrentQueue<LogEntry>();
        private readonly Timer ProcessTimer = new System.Windows.Forms.Timer();

        public MessageExchangeAppender()
        {
            ProcessTimer.Interval = 50;
            ProcessTimer.Tick += ProcessTimer_Tick;
        }

        public void log(string message, LogSeverity severity)
        {
            if (!ProcessTimer.Enabled)
            {
                ProcessTimer.Start();
            }

            var m = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Severity = severity.ToString()
            };

            LogQueue.Enqueue(m);
        }

        private void ProcessTimer_Tick(object sender, EventArgs e)
        {
            while (LogQueue.TryDequeue(out var logEntry))
            {
                BrowserMessages.MessageExchange.Instance.Publish(logEntry);
            }
        }
    }
}
