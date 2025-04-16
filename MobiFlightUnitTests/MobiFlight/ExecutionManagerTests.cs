using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass]
    public class ExecutionManagerTests
    {
        private ExecutionManager _executionManager;
        private Mock<IMessagePublisher> _mockMessagePublisher;
        private Action<string> _OnMessageReceivedCallback;

        [TestInitialize]
        public void Setup()
        {
            _executionManager = new ExecutionManager(IntPtr.Zero);
            _mockMessagePublisher = new Mock<IMessagePublisher>();

            // Capture the callback passed to OnMessageReceived
            _mockMessagePublisher
                .Setup(p => p.OnMessageReceived(It.IsAny<Action<string>>()))
                .Callback<Action<string>>(callback =>
                {
                    _OnMessageReceivedCallback = callback;
                });

            // Set up the mock to serialize and pass the message to OnMessageReceived
            _mockMessagePublisher
                .Setup(p => p.Publish(It.IsAny<object>()))
                .Callback<object>(message =>
                {
                    var jsonMessage = JsonConvert.SerializeObject(new Message<object>(message.GetType().Name, message));
                    _OnMessageReceivedCallback?.Invoke(jsonMessage);
                });

            // Set the publisher, which should register the captured callback
            MessageExchange.Instance.SetPublisher(_mockMessagePublisher.Object);
        }

        [TestMethod]
        public void CommandConfigBulkAction_Delete_RemovesItems()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            _executionManager.Project = project;


            var message = new CommandConfigBulkAction
            {
                Action = "delete",
                Items = new List<ConfigItem> { configItem1 }
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.IsFalse(_executionManager.ConfigItems.Contains(configItem1));
            Assert.IsTrue(_executionManager.ConfigItems.Contains(configItem2));
        }

        [TestMethod]
        public void CommandConfigBulkAction_Toggle_TogglesItems_FalseTrue()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            _executionManager.Project = project;

            var message = new CommandConfigBulkAction
            {
                Action = "toggle",
                Items = new List<ConfigItem> { configItem1, configItem2 }
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.IsTrue(configItem1.Active);
            Assert.IsTrue(configItem2.Active);
        }

        [TestMethod]
        public void CommandConfigBulkAction_Toggle_TogglesItems_TrueFalse()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            _executionManager.Project = project;

            var message = new CommandConfigBulkAction
            {
                Action = "toggle",
                Items = new List<ConfigItem> { configItem1, configItem2 }
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.IsFalse(configItem1.Active);
            Assert.IsFalse(configItem2.Active);
        }

        [TestMethod]
        public void CommandConfigBulkAction_Toggle_TogglesItems_Mixed()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            _executionManager.Project = project;

            var message = new CommandConfigBulkAction
            {
                Action = "toggle",
                Items = new List<ConfigItem> { configItem1, configItem2 }
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.IsFalse(configItem1.Active);
            Assert.IsFalse(configItem2.Active);
        }
    }
}