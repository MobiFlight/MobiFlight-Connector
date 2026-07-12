using System;

namespace MobiFlight.BrowserMessages.Outgoing
{
    public class LogEntry
    {
        public string Id;
        public DateTime Timestamp;
        public string Message;
        public LogSeverity Severity;
    }
}