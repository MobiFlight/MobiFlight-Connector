using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MobiFlight.BrowserMessages.Outgoing.Tests
{
    [TestClass]
    public class LogEntryTests
    {
        [TestMethod]
        public void LogEntry_Json_SerializesCorrectly()
        {
            LogEntry logEntry = new LogEntry()
            {
                Id = "123",
                Severity = LogSeverity.Debug,
                Timestamp = new DateTime(2024, 6, 1, 12, 0, 0),
                Message = "Test message"
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(logEntry);

            Assert.Contains("\"Id\":\"123\"", json);
            Assert.Contains("\"Severity\":\"debug\"", json);
            Assert.Contains("\"Timestamp\":\"2024-06-01T12:00:00\"", json);
            Assert.Contains("\"Message\":\"Test message\"", json);

            logEntry.Severity = LogSeverity.Info;
            json = Newtonsoft.Json.JsonConvert.SerializeObject(logEntry);
            Assert.Contains("\"Severity\":\"info\"", json);

            logEntry.Severity = LogSeverity.Warn;
            json = Newtonsoft.Json.JsonConvert.SerializeObject(logEntry);
            Assert.Contains("\"Severity\":\"warn\"", json);

            logEntry.Severity = LogSeverity.Error;
            json = Newtonsoft.Json.JsonConvert.SerializeObject(logEntry);
            Assert.Contains("\"Severity\":\"error\"", json);
        }
    }
}