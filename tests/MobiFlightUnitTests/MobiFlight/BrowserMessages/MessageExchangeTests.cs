using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Threading;

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
            // Clear the singleton state between tests. SetSynchronizationContext(null) matters
            // beyond this test class - MessageExchange is a process-wide singleton, so a leaked
            // UI context here would silently break message delivery in every other test class
            // that shares it, without anything in that other class looking wrong.
            messageExchange.ClearSubscriptions();
            messageExchange.SetSynchronizationContext(null);
            messageExchange.SetSynchronizationContextProvider(null);
        }

        /// <summary>
        /// A SynchronizationContext backed by one dedicated pump thread, so a test can assert
        /// that Post() actually ran a callback on a different, identifiable thread - not just
        /// that some delegate got invoked.
        /// </summary>
        private class SingleThreadSyncContext : SynchronizationContext, IDisposable
        {
            private readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();
            public int ThreadId { get; }

            public SingleThreadSyncContext()
            {
                var ready = new ManualResetEventSlim();
                var threadId = -1;
                var thread = new Thread(() =>
                {
                    threadId = Thread.CurrentThread.ManagedThreadId;
                    ready.Set();
                    foreach (var action in _queue.GetConsumingEnumerable())
                    {
                        action();
                    }
                })
                { IsBackground = true };
                thread.Start();
                ready.Wait();
                ThreadId = threadId;
            }

            public override void Post(SendOrPostCallback d, object state) => _queue.Add(() => d(state));

            public void Dispose() => _queue.CompleteAdding();
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
        public void SubscribeTest_RunsInline_EvenWhenUiContextIsSet()
        {
            // A UI context is set, as production code does via MainForm.InitializeMessaging -
            // but a plain Subscribe handler must never use it. This is the property that makes
            // it safe to invert the old blanket-marshal default (see MessageExchange.Subscribe).
            using var uiContext = new SingleThreadSyncContext();
            messageExchange.SetSynchronizationContext(uiContext);

            var testEvent = new Test { Property1 = "TestValue" };
            var messageJson = JsonConvert.SerializeObject(new Message<object>("Test", testEvent));

            var callingThreadId = Thread.CurrentThread.ManagedThreadId;
            int observedThreadId = -1;
            messageExchange.Subscribe<Test>(receivedEvent =>
            {
                observedThreadId = Thread.CurrentThread.ManagedThreadId;
            });

            capturedCallback(messageJson);

            // No Post() involved, so this is safe to assert synchronously right after the call.
            Assert.AreEqual(callingThreadId, observedThreadId, "Subscribe should run inline on the publishing thread, not the UI context.");
        }

        [TestMethod()]
        public void SubscribeOnUiThreadTest_MarshalsOntoCapturedUiContext()
        {
            using var uiContext = new SingleThreadSyncContext();
            messageExchange.SetSynchronizationContext(uiContext);

            var testEvent = new Test { Property1 = "TestValue" };
            var messageJson = JsonConvert.SerializeObject(new Message<object>("Test", testEvent));

            var callingThreadId = Thread.CurrentThread.ManagedThreadId;
            var invoked = new ManualResetEventSlim();
            int observedThreadId = -1;
            messageExchange.SubscribeOnUiThread<Test>(receivedEvent =>
            {
                observedThreadId = Thread.CurrentThread.ManagedThreadId;
                invoked.Set();
            });

            capturedCallback(messageJson);

            Assert.IsTrue(invoked.Wait(1000), "SubscribeOnUiThread handler should have run.");
            Assert.AreEqual(uiContext.ThreadId, observedThreadId, "Handler should run on the captured UI thread.");
            Assert.AreNotEqual(callingThreadId, observedThreadId, "Handler must not run inline on the publishing thread.");
        }

        [TestMethod()]
        public void SubscribeOnUiThreadTest_SyncContextProviderWinsOverCapturedContext()
        {
            // Mirrors MainFormTests.TestMessagePublisher.SimulateIncomingMessage, which relies on
            // SetSynchronizationContextProvider(() => null) forcing inline dispatch regardless of
            // whatever UI context is captured.
            using var uiContext = new SingleThreadSyncContext();
            messageExchange.SetSynchronizationContext(uiContext);
            messageExchange.SetSynchronizationContextProvider(() => null);

            var testEvent = new Test { Property1 = "TestValue" };
            var messageJson = JsonConvert.SerializeObject(new Message<object>("Test", testEvent));

            var callingThreadId = Thread.CurrentThread.ManagedThreadId;
            int observedThreadId = -1;
            messageExchange.SubscribeOnUiThread<Test>(receivedEvent =>
            {
                observedThreadId = Thread.CurrentThread.ManagedThreadId;
            });

            capturedCallback(messageJson);

            Assert.AreEqual(callingThreadId, observedThreadId, "SetSynchronizationContextProvider(() => null) should win over the captured UI context.");
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

        [TestMethod()]
        public void SubscribeTest_CommandRefreshPresets_DeserializesEnumMemberPayloadToProSim()
        {
            // Arrange
            var messageJson = "{\"key\":\"CommandRefreshPresets\",\"payload\":{\"type\":\"prosim\"}}";
            var receivedMessage = default(CommandRefreshPresets);

            messageExchange.Subscribe<CommandRefreshPresets>(message =>
            {
                receivedMessage = message;
            });

            // Act
            capturedCallback(messageJson);

            // Assert
            Assert.IsNotNull(receivedMessage, "The refresh command payload should deserialize successfully.");
            Assert.AreEqual(PresetType.PROSIM, receivedMessage.type, "The EnumMember value 'prosim' should deserialize to PresetType.PROSIM.");
        }
    }

    public class Test
    {
        public string Property1 { get; set; }
    }
}
