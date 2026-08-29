using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace MobiFlight.Base.Tests
{
    [TestClass]
    public class LogTests
    {
        private Mock<ILogAppender> _appender;
        private bool _originalEnabled;
        private LogSeverity _originalSeverity;

        [TestInitialize]
        public void SetUp()
        {
            _appender = new Mock<ILogAppender>();
            _originalEnabled = Log.Instance.Enabled;
            _originalSeverity = Log.Instance.Severity;

            Log.Instance.ClearAppenders();
            Log.Instance.AddAppender(_appender.Object);
            Log.Instance.Enabled = true;
        }

        [TestCleanup]
        public void TearDown()
        {
            Log.Instance.ClearAppenders();
            Log.Instance.Enabled = _originalEnabled;
            Log.Instance.Severity = _originalSeverity;
        }

        [TestMethod]
        public void GetCallingMethod_ShouldCombineSourceFileAndMemberName()
        {
            var callingMethod = Log.Instance.GetCallingMethod(
                "HandleMessage",
                @"C:\\Source\\MobiFlight\\MessageHandler.cs");

            Assert.AreEqual("MessageHandler.HandleMessage()", callingMethod);
        }

        [TestMethod]
        public void Log_ShouldIncludeCallerMemberAndSourceFile_WhenDebugLogging()
        {
            Log.Instance.Severity = LogSeverity.Debug;

            LogFromHelper();

            _appender.Verify(appender => appender.log(
                It.Is<string>(message =>
                    message.StartsWith("LogTests.LogFromHelper()#") &&
                    message.EndsWith(": caller metadata")),
                LogSeverity.Debug),
                Times.Once);
        }

        [TestMethod]
        public void Log_WithException_ShouldIncludeMessageAndException()
        {
            Log.Instance.Severity = LogSeverity.Info;
            var exception = new InvalidOperationException("test failure");

            Log.Instance.log(exception, "Unable to process message.", LogSeverity.Error);

            _appender.Verify(appender => appender.log(
                "Unable to process message. System.InvalidOperationException: test failure",
                LogSeverity.Error),
                Times.Once);
        }

        private static void LogFromHelper()
        {
            Log.Instance.log("caller metadata", LogSeverity.Debug);
        }
    }
}
