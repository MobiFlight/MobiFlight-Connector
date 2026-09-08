using Moq;
using Serilog.Events;

namespace MobiFlight.Base.Tests
{
    [TestClass]
    public class LogTests
    {
        private Log _log;
        private bool _originalEnabled;
        private LogSeverity _originalSeverity;
        private bool _originalLogJoystickAxis;

        [TestInitialize]
        public void SetUp()
        {
            _log = Log.Instance;
            _originalEnabled = _log.Enabled;
            _originalSeverity = _log.Severity;
            _originalLogJoystickAxis = _log.LogJoystickAxis;
            _log.ClearAppenders();
            _log.Enabled = false;
            _log.Severity = LogSeverity.Info;
        }

        [TestCleanup]
        public void TearDown()
        {
            _log.ClearAppenders();
            _log.Enabled = _originalEnabled;
            _log.Severity = _originalSeverity;
            _log.LogJoystickAxis = _originalLogJoystickAxis;
        }

        [TestMethod]
        [DataRow(LogSeverity.Debug, "DEBUG")]
        [DataRow(LogSeverity.Info, "INFO")]
        [DataRow(LogSeverity.Warn, "WARNING")]
        [DataRow(LogSeverity.Error, "ERROR")]
        public void PythonLogLevel_ReturnsExpectedValue(LogSeverity severity, string expected)
        {
            Assert.AreEqual(expected, severity.PythonLogLevel());
        }

        [TestMethod]
        public void PythonLogLevel_UnknownSeverity_ReturnsWarning()
        {
            Assert.AreEqual("WARNING", ((LogSeverity)99).PythonLogLevel());
        }

        [TestMethod]
        [DataRow("DEBUG", LogSeverity.Debug)]
        [DataRow("INFO", LogSeverity.Info)]
        [DataRow("WARNING", LogSeverity.Warn)]
        [DataRow("ERROR", LogSeverity.Error)]
        public void SeverityFromPythonLogLevel_ValidValue_ReturnsMatchingSeverity(string level, LogSeverity expected)
        {
            var converted = LogSeverityExtensions.SeverityFromPythonLogLevel(level, out var severity);

            Assert.IsTrue(converted);
            Assert.AreEqual(expected, severity);
        }

        [TestMethod]
        public void SeverityFromPythonLogLevel_InvalidValue_ReturnsFalseAndInfo()
        {
            var converted = LogSeverityExtensions.SeverityFromPythonLogLevel("TRACE", out var severity);

            Assert.IsFalse(converted);
            Assert.AreEqual(LogSeverity.Info, severity);
        }

        [TestMethod]
        [DataRow(LogSeverity.Debug, LogEventLevel.Debug)]
        [DataRow(LogSeverity.Info, LogEventLevel.Information)]
        [DataRow(LogSeverity.Warn, LogEventLevel.Warning)]
        [DataRow(LogSeverity.Error, LogEventLevel.Error)]
        public void ToSerilogLevel_ReturnsExpectedValue(LogSeverity severity, LogEventLevel expected)
        {
            Assert.AreEqual(expected, severity.ToSerilogLevel());
        }

        [TestMethod]
        public void ToSerilogLevel_UnknownSeverity_ReturnsWarning()
        {
            Assert.AreEqual(LogEventLevel.Warning, ((LogSeverity)99).ToSerilogLevel());
        }

        [TestMethod]
        public void Log_WhenDisabled_DoesNotNotifyAppender()
        {
            var appender = new Mock<ILogAppender>();
            _log.AddAppender(appender.Object);

            _log.log("message", LogSeverity.Error);

            appender.Verify(a => a.log(It.IsAny<string>(), It.IsAny<LogSeverity>()), Times.Never);
        }

        [TestMethod]
        public void Log_BelowConfiguredSeverity_DoesNotNotifyAppender()
        {
            var appender = new Mock<ILogAppender>();
            _log.AddAppender(appender.Object);
            _log.Enabled = true;
            _log.Severity = LogSeverity.Warn;

            _log.log("message", LogSeverity.Info);

            appender.Verify(a => a.log(It.IsAny<string>(), It.IsAny<LogSeverity>()), Times.Never);
        }

        [TestMethod]
        public void Log_EligibleMessage_NotifiesEveryAppender()
        {
            var firstAppender = new Mock<ILogAppender>();
            var secondAppender = new Mock<ILogAppender>();
            _log.AddAppender(firstAppender.Object);
            _log.AddAppender(secondAppender.Object);
            _log.Enabled = true;
            _log.Severity = LogSeverity.Info;

            _log.log("message", LogSeverity.Warn);

            firstAppender.Verify(a => a.log("message", LogSeverity.Warn), Times.Once);
            secondAppender.Verify(a => a.log("message", LogSeverity.Warn), Times.Once);
        }

        [TestMethod]
        public void Log_DebugMessage_IncludesCallingMethod()
        {
            var appender = new Mock<ILogAppender>();
            _log.AddAppender(appender.Object);
            _log.Enabled = true;
            _log.Severity = LogSeverity.Debug;

            _log.log("message", LogSeverity.Debug);

            appender.Verify(a => a.log(
                It.Is<string>(message => message.StartsWith("LogTests.Log_DebugMessage_IncludesCallingMethod(): ") && message.EndsWith("message")),
                LogSeverity.Debug),
                Times.Once);
        }

        [TestMethod]
        public void ClearAppenders_RemovesRegisteredAppenders()
        {
            var appender = new Mock<ILogAppender>();
            _log.AddAppender(appender.Object);
            _log.ClearAppenders();
            _log.Enabled = true;

            _log.log("message", LogSeverity.Info);

            appender.Verify(a => a.log(It.IsAny<string>(), It.IsAny<LogSeverity>()), Times.Never);
        }

        [TestMethod]
        public void LooksLikeExpression_RecognizesIndicatorsAndPlainText()
        {
            foreach (var indicator in Log.ExpressionIndicator)
            {
                Assert.IsTrue(Log.LooksLikeExpression($"value{indicator}value"));
            }

            Assert.IsFalse(Log.LooksLikeExpression("value"));
            Assert.IsFalse(Log.LooksLikeExpression(String.Empty));
        }

        [TestMethod]
        public void GetStatistics_ReturnsConfiguredLoggingValues()
        {
            _log.Enabled = true;
            _log.Severity = LogSeverity.Error;

            Dictionary<string, int> statistics = _log.GetStatistics();

            Assert.HasCount(2, statistics);
            Assert.AreEqual((int)LogSeverity.Error, statistics["Log.Level"]);
            Assert.AreEqual(1, statistics["Log.Enabled"]);
        }
    }
}
