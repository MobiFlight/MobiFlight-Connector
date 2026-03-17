using MobiFlight.BrowserMessages;
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

        [TestCleanup]
        public void TestCleanup()
        {
            // Clear the singleton state between tests
            messageExchange.ClearSubscriptions();
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

        [TestMethod()]
        public void ClearSubscriptionsTest()
        {
            // Arrange
            var testEvent = new Test { Property1 = "TestValue1" };
            var messageJson = JsonConvert.SerializeObject(new Message<object>("Test", testEvent));
            bool isSubscriber1Invoked = false;
            Action<Test> subscriberAction1 = receivedEvent =>
            {
                isSubscriber1Invoked = true;
            };

            // Subscribe multiple callbacks
            messageExchange.Subscribe<Test>(subscriberAction1);
            
            // Verify subscriptions work before clearing
            capturedCallback(messageJson);

            Assert.IsTrue(isSubscriber1Invoked, "Subscriber 1 should be invoked before clearing");

            // Reset flags for the actual test
            isSubscriber1Invoked = false;

            // Act
            messageExchange.ClearSubscriptions();

            // Simulate receiving a message after clearing
            capturedCallback(messageJson);

            // Assert
            Assert.IsFalse(isSubscriber1Invoked, "Subscriber 1 should not be invoked after clearing subscriptions");
            
            // Verify that new subscriptions can still be added after clearing
            bool newSubscriberInvoked = false;
            Action<Test> newSubscriberAction = receivedEvent =>
            {
                newSubscriberInvoked = true;
            };

            messageExchange.Subscribe<Test>(newSubscriberAction);
            capturedCallback(messageJson);

            Assert.IsTrue(newSubscriberInvoked, "New subscriber should work after clearing subscriptions");
        }
    }

    public class Test
    {
        public string Property1 { get; set; }
    }
}