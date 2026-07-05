using MobiFlight.BrowserMessages.Outgoing;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MobiFlight.Base.LogAppender
{
    public class MessageExchangeAppender : ILogAppender
    {
        private readonly ConcurrentQueue<LogEntry> LogQueue = new ConcurrentQueue<LogEntry>();
        private Timer ProcessTimer;
        private long MessageCounter = 0;

        public MessageExchangeAppender()
        {

        }

        public void log(string message, LogSeverity severity)
        {
            MessageCounter++;
            var m = new LogEntry
            {
                Id = (MessageCounter).ToString(),
                Timestamp = DateTime.Now,
                Message = message,
                Severity = severity
            };

            LogQueue.Enqueue(m);

            if (ProcessTimer == null)
            {
                var newTimer = new Timer(ProcessTimer_Tick, this, 0, 100);
                // handle potential race condition during timer initialization
                if (Interlocked.CompareExchange(ref ProcessTimer, newTimer, null) != null)
                {
                    newTimer.Dispose();
                }
            }
        }

        private static void ProcessTimer_Tick(object state)
        {
            if (!(state is MessageExchangeAppender appender)) return;

            while (appender.LogQueue.TryDequeue(out var logEntry))
            {
                BrowserMessages.MessageExchange.Instance.Publish(logEntry);
            }
        }
    }
}