using MobiFlight.BrowserMessages.Outgoing;
using System;

namespace MobiFlight.Base.LogAppender
{
    public class MessageExchangeAppender : ILogAppender
    {   public void log(string message, LogSeverity severity)
        {
            var m = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Severity = severity.ToString()
            };

            BrowserMessages.MessageExchange.Instance.Publish(m);
        }
    }
}
