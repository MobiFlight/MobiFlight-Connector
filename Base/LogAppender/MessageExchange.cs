using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Outgoing;
using System;

namespace MobiFlight.Base.LogAppender
{
    public class MessageExchange : ILogAppender
    {   public void log(string message, LogSeverity severity)
        {
            var m = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Severity = severity.ToString()
            };

            BrowserMessages.MessageExchange.Instance.Publish<LogEntry>(m);
        }
    }
}
