using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;

namespace MobiFlight.BrowserMessages.Tests
{
    [TestClass()]
    public class MessageExchangeTests
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

        [TestMethod()]
        public void SetPublisherTest()
        {
            // Arrange
            // Initialization is done in TestInitialize

            // Act
            // Already performed in TestInitialize

            // Assert
            mockPublisher.Verify(p => p.OnMessageReceived(It.IsAny<Action<string>>()), Times.Once);
            Assert.IsNotNull(capturedCallback, "The callback should have been captured.");
        }

        [TestMethod()]
        public void PublishTest()
        {
            // Arrange
            var testEvent = new Test { Property1 = "TestValue" };

            // Act
            messageExchange.Publish(testEvent);

            // Assert
            mockPublisher.Verify(p => p.Publish(It.Is<Test>(e => e.Property1 == testEvent.Property1)), Times.Once);
        }

        [TestMethod()]
        public void SubscribeTest()
        {
            // Arrange
            var testEvent = new Test { Property1 = "TestValue" };
            var messageJson = JsonConvert.SerializeObject(new Message<object>("Test", testEvent));

            bool isSubscriberInvoked = false;
            Action<Test> subscriberAction = receivedEvent =>
            {
                isSubscriberInvoked = true;
                Assert.AreEqual(testEvent.Property1, receivedEvent.Property1);
            };

            // Act
            messageExchange.Subscribe<Test>(subscriberAction);
            // Simulate receiving a message
            capturedCallback(messageJson);

            // Assert
            Assert.IsTrue(isSubscriberInvoked, "Subscriber should have been invoked.");
        }

        [TestMethod()]
        public void UnsubscribeTest()
        {
            // Arrange
            var testEvent = new Test { Property1 = "TestValue" };
            var messageJson = JsonConvert.SerializeObject(new Message<object>("Test", testEvent));

            bool isSubscriberInvoked = false;
            Action<Test> subscriberAction = receivedEvent =>
            {
                isSubscriberInvoked = true;
                Assert.AreEqual(testEvent.Property1, receivedEvent.Property1);
            };

            messageExchange.Subscribe<Test>(subscriberAction);
            messageExchange.Unsubscribe(subscriberAction);

            // Act
            // Simulate receiving a message
            capturedCallback(messageJson);

            // Assert
            Assert.IsFalse(isSubscriberInvoked, "Subscriber should not have been invoked after unsubscription.");
        }
    }

    public class Test
    {
        public string Property1 { get; set; }
    }
}