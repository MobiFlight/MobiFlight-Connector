// MobiFlightUnitTests\MobiFlight\BrowserMessages\Publisher\CompositePublisherTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Publisher;
using Moq;
using System;

namespace MobiFlightUnitTests.MobiFlight.BrowserMessages.Publisher
{
    [TestClass]
    public class CompositePublisherTests
    {
        private CompositePublisher compositePublisher;
        private Mock<IMessagePublisher> mockPublisher1;
        private Mock<IMessagePublisher> mockPublisher2;

        [TestInitialize]
        public void Setup()
        {
            compositePublisher = new CompositePublisher();
            mockPublisher1 = new Mock<IMessagePublisher>();
            mockPublisher2 = new Mock<IMessagePublisher>();
        }

        [TestMethod]
        public void AddPublisher_ShouldAddNewPublisher()
        {
            // Act
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.Publish("test message");

            // Assert
            mockPublisher1.Verify(p => p.Publish("test message"), Times.Once);
        }

        [TestMethod]
        public void AddPublisher_ShouldReplaceExistingPublisher()
        {
            // Arrange
            var oldPublisher = new Mock<IMessagePublisher>();
            compositePublisher.AddPublisher("test1", oldPublisher.Object);

            // Act
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.Publish("test message");

            // Assert
            oldPublisher.Verify(p => p.Publish(It.IsAny<string>()), Times.Never);
            mockPublisher1.Verify(p => p.Publish("test message"), Times.Once);
        }

        [TestMethod]
        public void RemovePublisher_ShouldRemovePublisher()
        {
            // Arrange
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);

            // Act
            var removed = compositePublisher.RemovePublisher("test1");
            compositePublisher.Publish("test message");

            // Assert
            Assert.AreEqual(mockPublisher1.Object, removed);
            mockPublisher1.Verify(p => p.Publish(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void RemovePublisher_ShouldReturnNullForNonExistentPublisher()
        {
            // Act
            var removed = compositePublisher.RemovePublisher("nonexistent");

            // Assert
            Assert.IsNull(removed);
        }

        [TestMethod]
        public void Publish_ShouldPublishToAllPublishers()
        {
            // Arrange
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.AddPublisher("test2", mockPublisher2.Object);

            // Act
            compositePublisher.Publish("test message");

            // Assert
            mockPublisher1.Verify(p => p.Publish("test message"), Times.Once);
            mockPublisher2.Verify(p => p.Publish("test message"), Times.Once);
        }

        [TestMethod]
        public void Publish_ShouldContinueOnException()
        {
            // Arrange
            mockPublisher1.Setup(p => p.Publish(It.IsAny<string>()))
                          .Throws(new Exception("Test exception"));
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.AddPublisher("test2", mockPublisher2.Object);

            // Act
            compositePublisher.Publish("test message");

            // Assert - publisher2 should still receive the message
            mockPublisher2.Verify(p => p.Publish("test message"), Times.Once);
        }

        [TestMethod]
        public void OnMessageReceived_ShouldRegisterCallbackWithAllActivePublishers()
        {
            // Arrange
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.AddPublisher("test2", mockPublisher2.Object);
            Action<string> callback = (msg) => { };

            // Act
            compositePublisher.OnMessageReceived(callback);

            // Assert
            mockPublisher1.Verify(p => p.OnMessageReceived(callback), Times.Once);
            mockPublisher2.Verify(p => p.OnMessageReceived(callback), Times.Once);
        }

        [TestMethod]
        public void OnMessageReceived_ShouldNotRegisterWithPausedPublishers()
        {
            // Arrange
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.AddPublisher("test2", mockPublisher2.Object);
            compositePublisher.PausePublisher("test2");
            Action<string> callback = (msg) => { };

            // Act
            compositePublisher.OnMessageReceived(callback);

            // Assert
            mockPublisher1.Verify(p => p.OnMessageReceived(callback), Times.Once);
            mockPublisher2.Verify(p => p.OnMessageReceived(callback), Times.Never);
        }

        [TestMethod]
        public void OnMessageReceived_ShouldContinueOnException()
        {
            // Arrange
            mockPublisher1.Setup(p => p.OnMessageReceived(It.IsAny<Action<string>>()))
                          .Throws(new Exception("Test exception"));
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.AddPublisher("test2", mockPublisher2.Object);
            Action<string> callback = (msg) => { };

            // Act
            compositePublisher.OnMessageReceived(callback);

            // Assert - publisher2 should still receive the callback
            mockPublisher2.Verify(p => p.OnMessageReceived(callback), Times.Once);
        }

        [TestMethod]
        public void PausePublisher_ShouldPausePublisher()
        {
            // Arrange
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            Action<string> callback = (msg) => { };

            // Act
            compositePublisher.PausePublisher("test1");
            compositePublisher.OnMessageReceived(callback);

            // Assert
            mockPublisher1.Verify(p => p.OnMessageReceived(It.IsAny<Action<string>>()), Times.Never);
        }

        [TestMethod]
        public void PausePublisher_ShouldNotAddDuplicatePause()
        {
            // Arrange
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            Action<string> callback1 = (msg) => { };
            Action<string> callback2 = (msg) => { };

            // Act
            compositePublisher.PausePublisher("test1");
            compositePublisher.PausePublisher("test1"); // Pause again
            compositePublisher.OnMessageReceived(callback1);
            compositePublisher.ResumePublisher("test1");
            compositePublisher.OnMessageReceived(callback2);

            // Assert - should be paused once, resumed once, then active
            mockPublisher1.Verify(p => p.OnMessageReceived(callback1), Times.Never);
            mockPublisher1.Verify(p => p.OnMessageReceived(callback2), Times.Once);
        }

        [TestMethod]
        public void ResumePublisher_ShouldResumePublisher()
        {
            // Arrange
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.PausePublisher("test1");
            Action<string> callback = (msg) => { };

            // Act
            compositePublisher.ResumePublisher("test1");
            compositePublisher.OnMessageReceived(callback);

            // Assert
            mockPublisher1.Verify(p => p.OnMessageReceived(callback), Times.Once);
        }

        [TestMethod]
        public void ResumePublisher_ShouldHandleNonExistentPublisher()
        {
            // Act - should not throw
            compositePublisher.ResumePublisher("nonexistent");

            // Assert - if we get here, no exception was thrown
        }

        [TestMethod]
        public void Publish_WithGenericType_ShouldPublishToAllPublishers()
        {
            // Arrange
            var testObject = new { Message = "test" };
            compositePublisher.AddPublisher("test1", mockPublisher1.Object);
            compositePublisher.AddPublisher("test2", mockPublisher2.Object);

            // Act
            compositePublisher.Publish(testObject);

            // Assert
            mockPublisher1.Verify(p => p.Publish(testObject), Times.Once);
            mockPublisher2.Verify(p => p.Publish(testObject), Times.Once);
        }

        [TestMethod]
        public void PauseAndResume_Integration_ShouldWorkCorrectly()
        {
            // Arrange
            compositePublisher.AddPublisher("auth", mockPublisher1.Object);
            compositePublisher.AddPublisher("frontend", mockPublisher2.Object);

            // Pause auth
            compositePublisher.PausePublisher("auth");

            Action<string> callback1 = (msg) => { };
            compositePublisher.OnMessageReceived(callback1);

            // Act - Resume auth
            compositePublisher.ResumePublisher("auth");
            Action<string> callback2 = (msg) => { };
            compositePublisher.OnMessageReceived(callback2);

            // Assert
            mockPublisher1.Verify(p => p.OnMessageReceived(callback1), Times.Never);
            mockPublisher1.Verify(p => p.OnMessageReceived(callback2), Times.Once);
            mockPublisher2.Verify(p => p.OnMessageReceived(callback1), Times.Once);
            mockPublisher2.Verify(p => p.OnMessageReceived(callback2), Times.Once);
        }
    }
}