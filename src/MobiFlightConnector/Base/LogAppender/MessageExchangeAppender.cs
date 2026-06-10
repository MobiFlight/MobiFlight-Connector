using MobiFlight.BrowserMessages.Outgoing;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MobiFlight.Base.LogAppender
{
    public class MessageExchangeAppender : ILogAppender
    {
        public ConcurrentQueue<LogEntry> LogQueue = new ConcurrentQueue<LogEntry>();
        private Timer ProcessTimer;

        public MessageExchangeAppender()
        {
            
        }

        public void log(string message, LogSeverity severity)
        {
            var m = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Severity = severity.ToString()
            };

            LogQueue.Enqueue(m);
            
            if (ProcessTimer == null)
            {
                ProcessTimer = new Timer(ProcessTimer_Tick, this, 0, 100);
                return;
            }
        }

        public static void ProcessTimer_Tick(object state)
        {
            var appender = (MessageExchangeAppender)state;

            if (appender == null) { return; }

            while (appender.LogQueue.TryDequeue(out var logEntry))
            {
                BrowserMessages.MessageExchange.Instance.Publish(logEntry);
            }
        }
    }
}