using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Outgoing;
using Moq;
using System;

namespace MobiFlight.Base.LogAppender.Tests
{
    [TestClass]
    public class MessageExchangeAppenderTests
    {
        private Mock<IMessagePublisher> mockPublisher;
        private MessageExchange messageExchange;
        private Action<string> capturedCallback;

        [TestInitialize]
        public void TestInitialize()
        {
            // Reset the singleton instance if possible, or ensure tests run in isolation
            // This depends on your implementation of MessageExchange
            messageExchange = MessageExchange.Instance;

            mockPublisher = new Mock<IMessagePublisher>();

            // Capture the callback passed to OnMessageReceived
            mockPublisher
                .Setup(p => p.OnMessageReceived(It.IsAny<Action<string>>()))
                .Callback<Action<string>>(callback =>
                {
                    capturedCallback = callback;
                });

            // Act: Set the publisher, which should register the captured callback
            messageExchange.SetPublisher(mockPublisher.Object);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clear the singleton state between tests
            messageExchange.ClearSubscriptions();
        }

        [TestMethod]
        public void MessageExchangeAppender_PublishesOnlyWhenFrontendAvailable()
        {
            var logAppender = new MessageExchangeAppender();
            logAppender.log("Test message", LogSeverity.Info);

            // Verify that the message was not published
            mockPublisher.Verify(p => p.Publish(It.IsAny<LogEntry>()), Times.Never);

            // Simulate frontend availability
            logAppender.FrontendAvailable = true;

            // Dequeue should happen with next timer tick, but we can manually invoke the timer callback for testing
            // Simulate the timer tick
            var processTimerTickMethod = typeof(MessageExchangeAppender).GetMethod("ProcessTimer_Tick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            processTimerTickMethod.Invoke(null, new object[] { logAppender });

            mockPublisher.Verify(p => p.Publish(It.Is<LogEntry>(
                entry => entry.Message == "Test message" &&
                entry.Severity == LogSeverity.Info)
                ),
                Times.Once
            );
        }
    }
}