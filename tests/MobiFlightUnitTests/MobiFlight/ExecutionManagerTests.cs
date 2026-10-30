using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.BrowserMessages.Outgoing;
using MobiFlight.Execution;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.ProSim;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Tests
{
    [TestClass]
    public class ExecutionManagerTests
    {
        private ExecutionManager _executionManager;
        private Mock<XplaneCacheInterface> _mockXplaneCache;
        private Mock<SimConnectCacheInterface> _mockSimConnectCache;
        private Mock<FSUIPCCacheInterface> _mockFsuipcCache;
        private Mock<ProSimCacheInterface> _mockProSimCache;
        private Mock<IMessagePublisher> _mockMessagePublisher;
        private Action<string> _OnMessageReceivedCallback;

        private List<T> GetPublishedMessages<T>()
            where T : class
        {
            return _mockMessagePublisher.Invocations
                .Where(invocation => invocation.Method.Name == "Publish")
                .Select(invocation => invocation.Arguments[0])
                .OfType<T>()
                .ToList();
        }

        private void AssertRejectedUndoPreservesHistory(string expectedAction, Action assertProjectUnchanged)
        {
            var statesBefore = GetPublishedMessages<ConfigListUndoState>();

            Assert.IsNotEmpty(statesBefore);

            var historyBefore = statesBefore.Last();

            Assert.IsTrue(historyBefore.CanUndo);
            Assert.IsNotEmpty(historyBefore.History);
            Assert.AreEqual(expectedAction, historyBefore.History.Last());

            var notificationsBefore = GetPublishedMessages<Notification>()
                .Count(notification => notification.Event == "ConfigListUndoFailed");

            MessageExchange.Instance.Publish(new CommandUndoConfigList {Steps = 1});
            assertProjectUnchanged();

            var statesAfter = GetPublishedMessages<ConfigListUndoState>();

            Assert.HasCount(statesBefore.Count, statesAfter, "A rejected Undo must not publish a changed history state.");

            var historyAfter = statesAfter.Last();

            Assert.IsTrue(historyAfter.CanUndo);

            CollectionAssert.AreEqual(historyBefore.History, historyAfter.History);

            Assert.AreEqual(historyBefore.HistoryStart, historyAfter.HistoryStart);

            var notificationsAfter = GetPublishedMessages<Notification>()
                .Where(notification => notification.Event == "ConfigListUndoFailed")
                .ToList();

            Assert.HasCount(notificationsBefore + 1, notificationsAfter);

            var lastNotification = notificationsAfter.Last();

            Assert.AreEqual(expectedAction, lastNotification.Context["Action"]);

            Assert.IsTrue(lastNotification.Context.ContainsKey("Reason"));

            Assert.IsFalse(string.IsNullOrWhiteSpace(lastNotification.Context["Reason"]));
        }

        [TestInitialize]
        public void Setup()
        {
            _mockXplaneCache = new Mock<XplaneCacheInterface>();
            _mockSimConnectCache = new Mock<SimConnectCacheInterface>();
            _mockFsuipcCache = new Mock<FSUIPCCacheInterface>();
            _mockProSimCache = new Mock<ProSimCacheInterface>();

            _executionManager = new ExecutionManager(
                IntPtr.Zero,
                _mockXplaneCache.Object,
                _mockSimConnectCache.Object,
                _mockFsuipcCache.Object,
                _mockProSimCache.Object);

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

        [TestCleanup]
        public void TestCleanup()
        {
            // Dispose of the ExecutionManager to ensure proper cleanup
            _executionManager.Stop();
            _executionManager.Shutdown();
            _executionManager = null;
            MessageExchange.Instance.ClearSubscriptions();
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
            Assert.DoesNotContain(configItem1, _executionManager.ConfigItems);
            Assert.Contains(configItem2, _executionManager.ConfigItems);
        }

        [TestMethod]
        public void CommandConfigBulkAction_Toggle_TogglesItems()
        {
            /// ---
            // Case 1: toggle false to true
            /// ----

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

            /// ---
            // Case 2: toggle true to false
            /// ----

            // Arrange
            configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            _executionManager.Project = project;

            message = new CommandConfigBulkAction
            {
                Action = "toggle",
                Items = new List<ConfigItem> { configItem1, configItem2 }
            };

            // Act
            MessageExchange.Instance.Publish(message);

            /// ---
            // Case 3: toggle true to false
            /// ----

            // Assert
            Assert.IsFalse(configItem1.Active);
            Assert.IsFalse(configItem2.Active);

            // Arrange
            configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true };
            configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            _executionManager.Project = project;

            message = new CommandConfigBulkAction
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
        public void CommandActiveConfigFile_Test()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem1, configItem2 } });
            _executionManager.Project = project;

            Assert.AreEqual(0, _executionManager.ActiveConfigIndex);

            var message = new CommandActiveConfigFile
            {
                index = 1
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.AreEqual(1, _executionManager.ActiveConfigIndex);
        }

        [TestMethod]
        public void CommandConfigContextMenu_Duplicate_DuplicatesConfigItemCorrectly()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var firstConfigItemGuid = configItem1.GUID;

            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile()
            {
                Label = "First Config",
                ConfigItems = { configItem1 }
            });

            _executionManager.Project = project;
            Assert.AreEqual(0, _executionManager.ActiveConfigIndex);
            Assert.HasCount(1, project.ConfigFiles[0].ConfigItems);

            var message = new CommandConfigContextMenu
            {
                Action = "duplicate",
                Item = project.ConfigFiles[0].ConfigItems.First() as ConfigItem
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.HasCount(2, project.ConfigFiles[0].ConfigItems);
            Assert.AreEqual(firstConfigItemGuid, project.ConfigFiles[0].ConfigItems[0].GUID);
            Assert.AreNotEqual(firstConfigItemGuid, project.ConfigFiles[0].ConfigItems[1].GUID, "Duplicated config items must have unique IDs");
        }

        [TestMethod]
        public void CommandFileContextMenu_Remove_RemovesConfig()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile()
            {
                Label = "First Config",
                ConfigItems = { configItem1, configItem2 }
            });
            project.ConfigFiles.Add(new ConfigFile()
            {
                Label = "Second Config",
                ConfigItems = { configItem1, configItem2 }
            });

            _executionManager.Project = project;

            Assert.AreEqual(0, _executionManager.ActiveConfigIndex);

            var message = new CommandFileContextMenu
            {
                Action = CommandFileContextMenuAction.remove,
                Index = 1,
                File = project.ConfigFiles[1]
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.HasCount(1, project.ConfigFiles);
            Assert.AreEqual("First Config", project.ConfigFiles[0].Label);
        }

        [TestMethod]
        public void CommandFileContextMenu_Rename_RenamesConfig()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Active = false };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile()
            {
                Label = "First Config",
                ConfigItems = { configItem1, configItem2 }
            });
            project.ConfigFiles.Add(new ConfigFile()
            {
                Label = "Second Config",
                ConfigItems = { configItem1, configItem2 }
            });

            _executionManager.Project = project;

            Assert.AreEqual(0, _executionManager.ActiveConfigIndex);

            var message = new CommandFileContextMenu
            {
                Action = CommandFileContextMenuAction.rename,
                Index = 1,
                File = new ConfigFile
                {
                    Label = "Renamed Config",
                    ConfigItems = { configItem1, configItem2 }
                }
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.HasCount(2, project.ConfigFiles);
            Assert.AreEqual("Renamed Config", project.ConfigFiles[1].Label);
        }

        [TestMethod]
        public void GetAvailableVariables_ReturnsVariablesFromActiveConfigFile()
        {
            // Arrange
            var variables1 = new Dictionary<string, MobiFlightVariable>
            {
                { "varA", new MobiFlightVariable() }
            };
            var variables2 = new Dictionary<string, MobiFlightVariable>
            {
                { "varB", new MobiFlightVariable() }
            };

            var ConfigItems1 = new List<IConfigItem>()
                {
                    new OutputConfigItem {
                        GUID = "1",
                        Active = true,
                        Name = "Output1",
                        Source = new VariableSource() {
                            MobiFlightVariable = new MobiFlightVariable() {
                                Name = "varA"
                            }
                        }
                    }
                };

            var ConfigItems2 = new List<IConfigItem>()
                {
                    new OutputConfigItem {
                        GUID = "1",
                        Active = true,
                        Name = "Output1",
                        Source = new VariableSource() {
                            MobiFlightVariable = new MobiFlightVariable() {
                                Name = "varB"
                            }
                        }
                    }
                };

            var configFile1 = new ConfigFile()
            {
                ConfigItems = ConfigItems1
            };

            var configFile2 = new ConfigFile()
            {
                ConfigItems = ConfigItems2
            };

            var project = new Project();
            project.ConfigFiles.Add(configFile1);
            project.ConfigFiles.Add(configFile2);

            _executionManager.Project = project;

            // Act
            var result = _executionManager.GetAvailableVariables();

            // Assert
            Assert.HasCount(1, result);
            Assert.IsTrue(result.ContainsKey("varA"));

            var message = new CommandActiveConfigFile
            {
                index = 1
            };

            // Act
            MessageExchange.Instance.Publish(message);

            result = _executionManager.GetAvailableVariables();

            // Assert
            Assert.HasCount(1, result);
            Assert.IsTrue(result.ContainsKey("varB"));
        }

        [TestMethod]
        public void CommandRefreshPresets_WhenProSimIsConnected_RefreshesBeforePublishingProSimDataRefDefinitions()
        {
            // Arrange
            var dataRefDescriptions = new Dictionary<string, DataRefDescription>
            {
                {
                    "aircraft/test/path",
                    new DataRefDescription
                    {
                        Name = "aircraft/test/path",
                        CanWrite = true,
                        DataType = "System.Boolean"
                    }
                }
            };
            var refreshCompletion = new TaskCompletionSource<bool>();
            var publishSignal = new ManualResetEventSlim(false);
            var calls = new List<string>();
            ProSimDataRefDefinitionUpdate publishedUpdate = null;

            _mockProSimCache.Setup(x => x.IsConnected()).Returns(true);
            _mockProSimCache
                .Setup(x => x.RefreshDataDefinitionsAsync())
                .Returns(() =>
                {
                    calls.Add("refresh");
                    return refreshCompletion.Task;
                });
            _mockProSimCache
                .Setup(x => x.GetDataRefDescriptions())
                .Returns(() =>
                {
                    calls.Add("get");
                    return dataRefDescriptions;
                });

            _mockMessagePublisher
                .Setup(publisher => publisher.Publish(It.IsAny<object>()))
                .Callback<object>(publishedMessage =>
                {
                    if (publishedMessage is ProSimDataRefDefinitionUpdate update)
                    {
                        calls.Add("publish");
                        publishedUpdate = update;
                        publishSignal.Set();
                    }

                    var jsonMessage = JsonConvert.SerializeObject(new Message<object>(publishedMessage.GetType().Name, publishedMessage));
                    _OnMessageReceivedCallback?.Invoke(jsonMessage);
                });

            var message = new CommandRefreshPresets
            {
                type = PresetType.PROSIM
            };

            // Act
            MessageExchange.Instance.Publish(message);

            Assert.IsFalse(publishSignal.Wait(50), "Definitions should not publish before the ProSim refresh completes.");
            refreshCompletion.SetResult(true);

            // Assert
            Assert.IsTrue(publishSignal.Wait(1000), "Definitions should publish after the ProSim refresh completes.");
            Assert.IsNotNull(publishedUpdate);
            Assert.AreSame(dataRefDescriptions, publishedUpdate.DataRefs);
            Assert.IsTrue(publishedUpdate.DataRefs.ContainsKey("aircraft/test/path"));
            CollectionAssert.AreEqual(new[] { "refresh", "get", "publish" }, calls);
            _mockProSimCache.Verify(x => x.RefreshDataDefinitionsAsync(), Times.Once);
        }

        [TestMethod]
        public void CommandRefreshPresets_WhenProSimIsDisconnected_DoesNotPublishStaleProSimDataRefDefinitions()
        {
            // Arrange
            _mockProSimCache.Setup(x => x.IsConnected()).Returns(false);

            var message = new CommandRefreshPresets
            {
                type = PresetType.PROSIM
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            _mockProSimCache.Verify(x => x.RefreshDataDefinitionsAsync(), Times.Never);
            _mockProSimCache.Verify(x => x.GetDataRefDescriptions(), Times.Never);
            _mockMessagePublisher.Verify(
                publisher => publisher.Publish(It.IsAny<ProSimDataRefDefinitionUpdate>()),
                Times.Never);
        }

        [TestMethod]
        public void CommandRefreshPresets_WhenProSimRefreshFailsWithEmptyCache_DoesNotPublishProSimDataRefDefinitions()
        {
            // Arrange
            _mockProSimCache.Setup(x => x.IsConnected()).Returns(true);
            _mockProSimCache.Setup(x => x.RefreshDataDefinitionsAsync()).Returns(Task.FromResult(false));
            _mockProSimCache
                .Setup(x => x.GetDataRefDescriptions())
                .Returns(new Dictionary<string, DataRefDescription>());

            var message = new CommandRefreshPresets
            {
                type = PresetType.PROSIM
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            _mockProSimCache.Verify(x => x.RefreshDataDefinitionsAsync(), Times.Once);
            _mockMessagePublisher.Verify(
                publisher => publisher.Publish(It.IsAny<ProSimDataRefDefinitionUpdate>()),
                Times.Never);
        }

        [TestMethod]
        public void CommandRefreshPresets_WhenProSimRefreshFailsWithCachedDefinitions_PublishesCachedDefinitions()
        {
            // Arrange
            var cachedDefinitions = new Dictionary<string, DataRefDescription>
            {
                {
                    "aircraft/test/path",
                    new DataRefDescription
                    {
                        Name = "aircraft/test/path",
                        CanWrite = true,
                        DataType = "System.Boolean"
                    }
                }
            };
            var publishSignal = new ManualResetEventSlim(false);
            ProSimDataRefDefinitionUpdate publishedUpdate = null;

            _mockProSimCache.Setup(x => x.IsConnected()).Returns(true);
            _mockProSimCache.Setup(x => x.RefreshDataDefinitionsAsync()).Returns(Task.FromResult(false));
            _mockProSimCache.Setup(x => x.GetDataRefDescriptions()).Returns(cachedDefinitions);

            _mockMessagePublisher
                .Setup(publisher => publisher.Publish(It.IsAny<object>()))
                .Callback<object>(publishedMessage =>
                {
                    if (publishedMessage is ProSimDataRefDefinitionUpdate update)
                    {
                        publishedUpdate = update;
                        publishSignal.Set();
                    }

                    var jsonMessage = JsonConvert.SerializeObject(new Message<object>(publishedMessage.GetType().Name, publishedMessage));
                    _OnMessageReceivedCallback?.Invoke(jsonMessage);
                });

            var message = new CommandRefreshPresets
            {
                type = PresetType.PROSIM
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.IsTrue(publishSignal.Wait(1000), "Cached definitions should be published as a fallback when the refresh fails.");
            Assert.IsNotNull(publishedUpdate);
            Assert.AreSame(cachedDefinitions, publishedUpdate.DataRefs);
            _mockProSimCache.Verify(x => x.RefreshDataDefinitionsAsync(), Times.Once);
        }

        [TestMethod]
        public void OnAircraftChanged_SimConnectCache_InvokesOnSimAircraftChanged()
        {
            const string aircraftName = "Cessna 172";

            _mockXplaneCache.Setup(x => x.IsConnected()).Returns(false);
            _mockFsuipcCache.Setup(x => x.IsConnected()).Returns(false);
            _mockSimConnectCache.Setup(x => x.IsConnected()).Returns(true);

            string eventAircraftName = "";

            _executionManager.OnSimAircraftChanged += (_, name) => eventAircraftName = name;

            _mockSimConnectCache.Raise(x => x.AircraftChanged += null, _mockSimConnectCache.Object, aircraftName);

            Assert.AreEqual(aircraftName, eventAircraftName);
        }

        [TestMethod]
        public void OnAircraftChanged_XPlaneCache_InvokesOnSimAircraftChanged()
        {
            const string aircraftName = "Airbus A330";

            _mockXplaneCache.Setup(x => x.IsConnected()).Returns(true);
            _mockFsuipcCache.Setup(x => x.IsConnected()).Returns(false);
            _mockSimConnectCache.Setup(x => x.IsConnected()).Returns(false);

            string eventAircraftName = "";

            _executionManager.OnSimAircraftChanged += (_, name) => eventAircraftName = name;

            _mockXplaneCache.Raise(x => x.AircraftChanged += null, _mockXplaneCache.Object, aircraftName);

            Assert.AreEqual(aircraftName, eventAircraftName);
        }

        [TestMethod]
        public void OnAircraftChanged_FSUIPCCache_InvokesOnSimAircraftChanged()
        {
            const string aircraftName = "Lockheed F-35";

            _mockXplaneCache.Setup(x => x.IsConnected()).Returns(false);
            _mockFsuipcCache.Setup(x => x.IsConnected()).Returns(true);
            _mockSimConnectCache.Setup(x => x.IsConnected()).Returns(false);

            string eventAircraftName = "";

            _executionManager.OnSimAircraftChanged += (_, name) => eventAircraftName = name;

            _mockFsuipcCache.Raise(x => x.AircraftChanged += null, _mockFsuipcCache.Object, aircraftName);

            Assert.AreEqual(aircraftName, eventAircraftName);
        }

        [TestMethod]
        public void OnAircraftChanged_FSUIPCAndXPlane_IgnoresFSUIPCAircraftName()
        {
            const string xPlaneAircraftName = "Airbus A320";
            const string fsuipcAircraftName = "A320";

            _mockXplaneCache.Setup(x => x.IsConnected()).Returns(true);
            _mockFsuipcCache.Setup(x => x.IsConnected()).Returns(true);
            _mockSimConnectCache.Setup(x => x.IsConnected()).Returns(false);

            string eventAircraftName = "";

            _executionManager.OnSimAircraftChanged += (_, name) => eventAircraftName = name;

            _mockXplaneCache.Raise(x => x.AircraftChanged += null, _mockXplaneCache.Object, xPlaneAircraftName);
            _mockFsuipcCache.Raise(x => x.AircraftChanged += null, _mockFsuipcCache.Object, fsuipcAircraftName);

            Assert.AreEqual(xPlaneAircraftName, eventAircraftName);
        }

        [TestMethod]
        public void OnAircraftChanged_FSUIPCAndSimConnect_IgnoresFSUIPCAircraftName()
        {
            const string simConnectAircraftName = "Airbus A320";
            const string fsuipcAircraftName = "A320";

            _mockXplaneCache.Setup(x => x.IsConnected()).Returns(false);
            _mockFsuipcCache.Setup(x => x.IsConnected()).Returns(true);
            _mockSimConnectCache.Setup(x => x.IsConnected()).Returns(true);

            string eventAircraftName = "";

            _executionManager.OnSimAircraftChanged += (_, name) => eventAircraftName = name;

            _mockSimConnectCache.Raise(x => x.AircraftChanged += null, _mockSimConnectCache.Object, simConnectAircraftName);
            _mockFsuipcCache.Raise(x => x.AircraftChanged += null, _mockFsuipcCache.Object, fsuipcAircraftName);

            Assert.AreEqual(simConnectAircraftName, eventAircraftName);
        }

        [TestMethod]
        public void mobiFlightCache_OnButtonPressed_LogMessageForInputEventPresent()
        {
            // Arrange
            var mockLogAppender = new Mock<ILogAppender>();
            Log.Instance.ClearAppenders();
            Log.Instance.AddAppender(mockLogAppender.Object);
            Log.Instance.Enabled = true;
            Log.Instance.Severity = LogSeverity.Info;

            // Create test input event args
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-000-001",
                    Name = "TestController",
                },
                Device = new DeviceReference()
                {
                    Name = "TestDevice:1",
                    Label = "Test Button",
                    Type = DeviceType.InputMultiplexer,
                },
                InputType = DeviceType.Button,
                Value = 1
            };

            var expectedLogMessage = $"{inputEventArgs.GetMsgEventLabel()}";

            // Use reflection to get the private method
            var methodInfo = typeof(ExecutionManager).GetMethod("mobiFlightCache_OnButtonPressed",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(methodInfo, "mobiFlightCache_OnButtonPressed method should exist");

            // Act
            methodInfo.Invoke(_executionManager, new object[] { _mockXplaneCache.Object, inputEventArgs });

            // Assert
            mockLogAppender.Verify(
                appender => appender.log(expectedLogMessage, LogSeverity.Info),
                Times.Once,
                "Expected log message should be logged once with Info severity"
            );
        }

        [TestMethod]
        public void UpdateInputPreconditions_RemovesStatus_WhenPreconditionsAreEmpty()
        {
            var configItem = new InputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Active = true,
                Name = "TestInput"
            };
            configItem.Status[ConfigItemStatusType.Precondition] = "not satisfied";
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem } });
            _executionManager.Project = project;

            var updateInputPreconditions = typeof(ExecutionManager).GetMethod("UpdateInputPreconditions",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var updatedValuesField = typeof(ExecutionManager).GetField("updatedValues",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var updatedValues = (ConcurrentDictionary<string, IConfigItem>)updatedValuesField.GetValue(_executionManager);

            updateInputPreconditions.Invoke(_executionManager, null);

            Assert.IsFalse(configItem.Status.ContainsKey(ConfigItemStatusType.Precondition));
            Assert.IsTrue(updatedValues.ContainsKey(configItem.GUID));
        }

        [TestMethod]
        public void FrontendUpdateTimer_Execute_ConcurrentDictionaryModification_ShouldNotThrowInvalidOperationException()
        {
            // Arrange
            const int numberOfConcurrentThreads = 20;
            const int operationsPerThread = 25;
            var exceptions = new ConcurrentBag<Exception>();
            var tasks = new List<Task>();

            // Set up a project with some config items to ensure we have data to work with
            var configItem = new InputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true, Name = "TestInput" };
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile() { ConfigItems = { configItem } });
            _executionManager.Project = project;

            // Get references to private members using reflection
            var updatedValuesField = typeof(ExecutionManager).GetField("updatedValues",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var updatedValues = (ConcurrentDictionary<string, IConfigItem>)updatedValuesField.GetValue(_executionManager);

            var frontendUpdateMethod = typeof(ExecutionManager).GetMethod("FrontendUpdateTimer_Execute",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act - Create the exact race condition that caused the original exception
            for (int i = 0; i < numberOfConcurrentThreads; i++)
            {
                var threadId = i;

                // Task 1: Simulate the frontend timer execution
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < operationsPerThread; j++)
                        {
                            // Add some test data to ensure the dictionary has content
                            var testItem = new InputConfigItem
                            {
                                GUID = $"frontend-{threadId}-{j}",
                                Active = true,
                                Name = $"FrontendTest{threadId}_{j}"
                            };

                            // Simulate adding data (like what happens in mobiFlightCache_OnButtonPressed)
                            lock (updatedValues)
                            {
                                updatedValues[testItem.GUID] = testItem;
                            }

                            // Execute the frontend timer method - this is where the original exception occurred
                            frontendUpdateMethod.Invoke(_executionManager, new object[] { null, EventArgs.Empty });

                            // Small delay to increase concurrency
                            if (j % 5 == 0) Thread.Sleep(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Unwrap TargetInvocationException to get the actual exception
                        var actualException = ex is TargetInvocationException tie ? tie.InnerException : ex;
                        exceptions.Add(actualException);
                    }
                }));

                // Task 2: Simulate other threads modifying updatedValues concurrently
                // This represents input events, config execution, etc.
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        for (int j = 0; j < operationsPerThread; j++)
                        {
                            var testItem = new InputConfigItem
                            {
                                GUID = $"concurrent-{threadId}-{j}",
                                Active = true,
                                Name = $"ConcurrentTest{threadId}_{j}"
                            };

                            // Add items to the dictionary
                            lock (updatedValues)
                            {
                                updatedValues[testItem.GUID] = testItem;
                            }

                            Thread.Sleep(1); // Small delay to increase chance of race condition

                            // Modify existing items
                            lock (updatedValues)
                            {
                                if (updatedValues.ContainsKey(testItem.GUID))
                                {
                                    updatedValues[testItem.GUID] = new InputConfigItem
                                    {
                                        GUID = testItem.GUID,
                                        Active = false,
                                        Name = $"Modified{threadId}_{j}"
                                    };
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }));
            }

            // Wait for all tasks to complete
            var allTasksCompleted = Task.WaitAll(tasks.ToArray(), TimeSpan.FromSeconds(30));

            // Assert
            Assert.IsTrue(allTasksCompleted, "All tasks should complete within the timeout period");

            // Check specifically for the InvalidOperationException that was originally thrown
            var collectionModifiedExceptions = exceptions
                .Where(ex => ex is InvalidOperationException &&
                            ex.Message.Contains("Collection was modified"))
                .ToList();

            Assert.IsEmpty(collectionModifiedExceptions,
                $"Should not throw 'Collection was modified' InvalidOperationException. " +
                $"Found {collectionModifiedExceptions.Count} such exceptions. " +
                $"This indicates the ToList() operation is not properly protected by the lock.");

            // Verify no other unexpected exceptions occurred
            if (exceptions.Any())
            {
                var exceptionSummary = string.Join("; ",
                    exceptions.GroupBy(e => e.GetType().Name)
                             .Select(g => $"{g.Key}: {g.Count()}"));
                Assert.Fail($"Unexpected exceptions occurred: {exceptionSummary}");
            }
        }

        [TestMethod]
        public void ExecuteConfig_WithNoControllersConnected_ShouldStillExecuteConfigItems()
        {
            // Arrange
            var outputConfigItem = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Active = true,
                Name = "TestOutput",
                Source = new VariableSource()
                {
                    MobiFlightVariable = new MobiFlightVariable() { Name = "TestVar", Number = 123.45 }
                },
                Device = new OutputConfig.CustomDevice() { CustomName = "TestDevice" },
                DeviceType = "InputAction" // Special type that doesn't require physical devices
            };

            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile()
            {
                ConfigItems = { outputConfigItem }
            });
            _executionManager.Project = project;

            // Verify no controllers are connected
            Assert.IsFalse(_executionManager.ModulesAvailable(), "No MobiFlight modules and/or Arcaze Boards should be connected");
            Assert.IsFalse(_executionManager.GetJoystickManager().JoysticksConnected(), "No joysticks should be connected");
            Assert.IsFalse(_executionManager.GetMidiBoardManager().AreMidiBoardsConnected(), "No midi controllers should be connected.");

            // Set up the variable so the config item has data to read
            _executionManager.getMobiFlightModuleCache().SetMobiFlightVariable(
                new MobiFlightVariable() { Name = "TestVar", Number = 123.45 });

            // Get access to the updatedValues dictionary via reflection
            var updatedValuesField = typeof(ExecutionManager).GetField("updatedValues",
                BindingFlags.NonPublic | BindingFlags.Instance);
            var updatedValues = (ConcurrentDictionary<string, IConfigItem>)updatedValuesField.GetValue(_executionManager);

            var initialUpdatedValuesCount = updatedValues.Count;

            // Act - Instead of relying on timer, directly call ExecuteConfig via reflection
            _executionManager.Start(); // This sets up the execution manager state

            // Use reflection to call the private ExecuteConfig method directly
            var executeConfigMethod = typeof(ExecutionManager).GetMethod("ExecuteConfig",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(executeConfigMethod, "ExecuteConfig method should exist");

            executeConfigMethod.Invoke(_executionManager, null);

            // Assert - The config item should be processed and cloned into updatedValues
            Assert.IsTrue(updatedValues.ContainsKey(outputConfigItem.GUID),
                "Config item should be cloned and added to updatedValues when processed");

            var clonedConfigItem = updatedValues[outputConfigItem.GUID] as OutputConfigItem;
            Assert.IsNotNull(clonedConfigItem, "Updated config item should be an OutputConfigItem");
            Assert.AreEqual("123.45", clonedConfigItem.Value,
                "Cloned config item should display the correct variable value");
            Assert.AreEqual("123.45", clonedConfigItem.RawValue,
                "Cloned config item should have the correct raw value");
        }

        [TestMethod]
        public void CommandResortConfigItem_MovesItemsBetweenFiles()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item1", Active = true };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item2", Active = true };
            var configItem3 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item3", Active = true };

            var sourceFile = new ConfigFile() { ConfigItems = { configItem1, configItem2 } };
            var targetFile = new ConfigFile() { ConfigItems = { configItem3 } };

            var project = new Project();
            project.ConfigFiles.Add(sourceFile);  // Index 0
            project.ConfigFiles.Add(targetFile);  // Index 1
            _executionManager.Project = project;

            var message = new CommandResortConfigItem
            {
                Items = new[] { new OutputConfigItem { GUID = configItem1.GUID } },
                SourceFileIndex = 0,
                TargetFileIndex = 1,
                NewIndex = 0
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.HasCount(1, sourceFile.ConfigItems, "Source file should have one less item");
            Assert.HasCount(2, targetFile.ConfigItems, "Target file should have one more item");
            Assert.AreEqual(targetFile.ConfigItems[0].GUID, configItem1.GUID, "Item should be moved to target file at correct index");
            Assert.AreEqual(targetFile.ConfigItems[1].GUID, configItem3.GUID, "Existing item should be shifted down");
            Assert.DoesNotContain(configItem1, sourceFile.ConfigItems, "Item should be removed from source file");
            Assert.Contains(configItem2, sourceFile.ConfigItems, "Other items should remain in source file");
        }

        [TestMethod]
        public void CommandResortConfigItem_ReordersWithinSameFile()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item1", Active = true };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item2", Active = true };
            var configItem3 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item3", Active = true };

            var configFile = new ConfigFile() { ConfigItems = { configItem1, configItem2, configItem3 } };

            var project = new Project();
            project.ConfigFiles.Add(configFile);  // Index 0
            _executionManager.Project = project;

            // Move item2 to position 0 (before item1)
            var message = new CommandResortConfigItem
            {
                Items = new[] { new OutputConfigItem { GUID = configItem2.GUID } },
                SourceFileIndex = 0,
                TargetFileIndex = 0,  // Same file
                NewIndex = 0
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.HasCount(3, configFile.ConfigItems, "File should still have same number of items");
            Assert.AreEqual(configFile.ConfigItems[0].GUID, configItem2.GUID, "Item2 should be moved to position 0");
            Assert.AreEqual(configFile.ConfigItems[1].GUID, configItem1.GUID, "Item1 should be shifted to position 1");
            Assert.AreEqual(configFile.ConfigItems[2].GUID, configItem3.GUID, "Item3 should remain at position 2");
        }

        [TestMethod]
        public void CommandResortConfigItem_HandlesMultipleItems()
        {
            // Arrange
            var configItem1 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item1", Active = true };
            var configItem2 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item2", Active = true };
            var configItem3 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item3", Active = true };
            var configItem4 = new OutputConfigItem { GUID = Guid.NewGuid().ToString(), Name = "Item4", Active = true };

            var sourceFile = new ConfigFile() { ConfigItems = { configItem1, configItem2, configItem3 } };
            var targetFile = new ConfigFile() { ConfigItems = { configItem4 } };

            var project = new Project();
            project.ConfigFiles.Add(sourceFile);  // Index 0
            project.ConfigFiles.Add(targetFile);  // Index 1
            _executionManager.Project = project;

            // Move multiple items (item1 and item3) to target file
            var message = new CommandResortConfigItem
            {
                Items = new[]
                {
                    new OutputConfigItem { GUID = configItem1.GUID },
                    new OutputConfigItem { GUID = configItem3.GUID }
                },
                SourceFileIndex = 0,
                TargetFileIndex = 1,
                NewIndex = 1  // Insert after configItem4
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.HasCount(1, sourceFile.ConfigItems, "Source file should have 2 less items");
            Assert.AreEqual(sourceFile.ConfigItems[0].GUID, configItem2.GUID, "Only item2 should remain in source file");

            Assert.HasCount(3, targetFile.ConfigItems, "Target file should have 2 more items");
            Assert.AreEqual(targetFile.ConfigItems[0].GUID, configItem4.GUID, "Original target item should remain at position 0");
            Assert.AreEqual(targetFile.ConfigItems[1].GUID, configItem1.GUID, "First moved item should be at position 1");
            Assert.AreEqual(targetFile.ConfigItems[2].GUID, configItem3.GUID, "Second moved item should be at position 2");
        }

        [TestMethod]
        public void ExecuteConfig_WithErrorInSingleConfig_ShouldContinueExecutingOtherConfigs()
        {
            // Arrange
            var variable1 = new MobiFlightVariable() { Name = "Var1", Number = 100 };
            var variable2 = new MobiFlightVariable() { Name = "Var2", Number = 200 };
            var variable3 = new MobiFlightVariable() { Name = "Var3", Number = 300 };

            var configItem1 = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Active = true,
                Name = "Config1",
                Source = new VariableSource() { MobiFlightVariable = variable1 },
                DeviceType = "InputAction"
            };

            var configItem2 = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Active = true,
                Name = "Config2_WithError",
                Source = new VariableSource() { MobiFlightVariable = variable2 },
                Controller = SerialNumber.CreateController("Test / SN-123"),
                DeviceType = MobiFlightOutput.TYPE,
                Device = new OutputConfig.Output { Pin = "1" }
            };

            var configItem3 = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Active = true,
                Name = "Config3",
                Source = new VariableSource() { MobiFlightVariable = variable3 },
                DeviceType = "InputAction"
            };

            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile()
            {
                ConfigItems = { configItem1, configItem2, configItem3 }
            });
            _executionManager.Project = project;

            // Set up the variables in cache
            _executionManager.getMobiFlightModuleCache().SetMobiFlightVariable(variable1);
            _executionManager.getMobiFlightModuleCache().SetMobiFlightVariable(variable2);
            _executionManager.getMobiFlightModuleCache().SetMobiFlightVariable(variable3);

            // Start execution manager
            _executionManager.Start();

            // Use reflection to call ExecuteConfig directly
            var executeConfigMethod = typeof(ExecutionManager).GetMethod("ExecuteConfig",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(executeConfigMethod, "ExecuteConfig method should exist");

            // Get access to the updatedValues dictionary
            var updatedValuesField = typeof(ExecutionManager).GetField("updatedValues",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var updatedValues = (ConcurrentDictionary<string, IConfigItem>)updatedValuesField.GetValue(_executionManager);

            // Act
            executeConfigMethod.Invoke(_executionManager, null);

            // Assert
            // All three configs should be in updatedValues, showing they were all processed
            Assert.IsTrue(updatedValues.ContainsKey(configItem1.GUID), "Config1 should be processed");
            Assert.IsTrue(updatedValues.ContainsKey(configItem2.GUID), "Config2 (with error) should be processed");
            Assert.IsTrue(updatedValues.ContainsKey(configItem3.GUID), "Config3 should be processed");

            // Config1 and Config3 should have their values set correctly
            var updatedConfig1 = updatedValues[configItem1.GUID] as OutputConfigItem;
            var updatedConfig3 = updatedValues[configItem3.GUID] as OutputConfigItem;
            Assert.AreEqual("100", updatedConfig1.Value, "Config1 should have correct value");
            Assert.AreEqual("300", updatedConfig3.Value, "Config3 should have correct value");

            // Config2 should be processed even though it references a non-existent module
            // It won't have an error status because no module with that serial exists,
            // so ExecuteDisplay returns early without throwing
            var updatedConfig2 = updatedValues[configItem2.GUID] as OutputConfigItem;
            Assert.AreEqual("200", updatedConfig2.Value, "Config2 should have value from source");

            // Execution manager should still be running
            Assert.IsTrue(_executionManager.IsStarted(), "ExecutionManager should still be running after error");

            // Clean up
            _executionManager.Stop();
        }

        [TestMethod]
        public void ExecuteConfig_ChangedConfigItemBecomesEffectiveWithoutStopAndRun()
        {
            // Arrange - Input Config Item version #1
            var Controller = new Controller()
            {
                Serial = "SN-000-001",
                Name = "TestController",
            };

            var Device = new DeviceReference()
            {
                Name = "TestDevice:1",
                Label = "Test Button",
                Type = DeviceType.Button,
            };

            var configItem = new InputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Active = true,
                Name = "Test item version #1",
                Controller = Controller,
                Device = new Button() { Name = Device.Name },
                button = new InputConfig.ButtonInputConfig()
                {

                    onPress = new VariableInputAction()
                    {
                        Variable = new MobiFlightVariable() { Name = "Var1", Number = 100 },
                    }
                }
            };

            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile()
            {
                ConfigItems = { configItem }
            });

            // Create test input event args
            var inputEventArgs = new InputEventArgs
            {
                Controller = Controller,
                Device = Device,
                InputType = DeviceType.Button,
                Value = (int)MobiFlightButton.InputEvent.PRESS
            };

            // Set up logging to verify expected log messages when config item is executed
            var mockLogAppender = new Mock<ILogAppender>();
            Log.Instance.ClearAppenders();
            Log.Instance.AddAppender(mockLogAppender.Object);
            Log.Instance.Enabled = true;
            Log.Instance.Severity = LogSeverity.Info;

            var expectedLogMessage = $"{Controller.Name} => Executing \"{configItem.Name}\". ({inputEventArgs.GetEventActionLabel()})";

            _executionManager.Project = project;

            // Use reflection to get the private method
            var methodInfo = typeof(ExecutionManager).GetMethod("mobiFlightCache_OnButtonPressed",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(methodInfo, "mobiFlightCache_OnButtonPressed method should exist");

            // Start execution manager
            _executionManager.Start();

            // Act - Trigger the input event to execute the config item version #1
            methodInfo.Invoke(_executionManager, new object[] { _mockXplaneCache.Object, inputEventArgs });

            // Assert
            mockLogAppender.Verify(
                appender => appender.log(expectedLogMessage, LogSeverity.Info),
                Times.Once,
                "Expected log message should be logged once with Info severity"
            );

            // Update config item as real copy and updated values
            var newConfig = configItem.Clone() as InputConfigItem;
            newConfig.Name = "Test item version 2";

            // update expected log message for version #2
            expectedLogMessage = $"{Controller.Name} => Executing \"{newConfig.Name}\". ({inputEventArgs.GetEventActionLabel()})";

            // Apply the updated configuration
            _executionManager.HandleCommandUpdateConfigItem(newConfig);

            // Trigger the input event again to see if the updated config item is effective
            methodInfo.Invoke(_executionManager, new object[] { _mockXplaneCache.Object, inputEventArgs });

            // Assert
            mockLogAppender.Verify(
                appender => appender.log(expectedLogMessage, LogSeverity.Info),
                Times.Once,
                "Expected log message should be logged once with Info severity"
            );

            // Clean up
            _executionManager.Stop();
        }
        [TestMethod]
        public void Execute_WhenActionIsNotConfigured_DoesNotLogExecution()
        {
            // Arrange
            var controller = new Controller
            {
                Serial = "SN-000-001",
                Name = "TestController"
            };

            var device = new DeviceReference
            {
                Name = "TestDevice:1",
                Label = "Test Button",
                Type = DeviceType.Button
            };

            var configItem = new InputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Active = true,
                Name = "Test config without action",
                Controller = controller,
                Device = new Button { Name = device.Name },

                // No onRelease action configured
                button = new InputConfig.ButtonInputConfig()
            };

            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile
            {
                ConfigItems = { configItem }
            });

            var inputEventArgs = new InputEventArgs
            {
                Controller = controller,
                Device = device,
                InputType = DeviceType.Button,
                Value = (int)MobiFlightButton.InputEvent.RELEASE
            };

            var mockLogAppender = new Mock<ILogAppender>();

            Log.Instance.ClearAppenders();
            Log.Instance.AddAppender(mockLogAppender.Object);
            Log.Instance.Enabled = true;
            Log.Instance.Severity = LogSeverity.Info;

            _executionManager.Project = project;

            var methodInfo = typeof(ExecutionManager).GetMethod(
                "mobiFlightCache_OnButtonPressed",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            Assert.IsNotNull(methodInfo);

            _executionManager.Start();

            // Act
            // The RELEASE event has no configured action, so it should be skipped without logging execution.
            methodInfo.Invoke(
                _executionManager,
                new object[] { _mockXplaneCache.Object, inputEventArgs });

            // Assert
            mockLogAppender.Verify(
                appender => appender.log(
                    It.Is<string>(msg =>
                        msg.Contains($"Executing \"{configItem.Name}\"")),
                    LogSeverity.Info),
                Times.Never,
                "No execution log should be written when no action is configured."
            );

            _executionManager.Stop();
        }

        [TestMethod]
        public void ProjectChange_DuringInputEventExecutorsEnumeration_ShouldCompleteWithoutThrowing()
        {
            var configItem1 = new InputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true, Name = "Config1" };
            var configFile1 = new ConfigFile() { ConfigItems = { configItem1 } };
            var project1 = new Project();
            project1.ConfigFiles.Add(configFile1);
            _executionManager.Project = project1;

            var field = typeof(ExecutionManager).GetField("_inputEventExecutors", BindingFlags.NonPublic | BindingFlags.Instance);
            var initialExecutors = (ConcurrentDictionary<ConfigFile, InputEventExecutor>)field.GetValue(_executionManager);

            var result = initialExecutors.Count;
            Assert.AreEqual(1, result);

            using (var enumerator = initialExecutors.Values.GetEnumerator())
            {
                Assert.IsTrue(enumerator.MoveNext());
                var firstExecutor = enumerator.Current;

                var configItem2 = new InputConfigItem { GUID = Guid.NewGuid().ToString(), Active = true, Name = "Config2" };
                var configFile2 = new ConfigFile() { ConfigItems = { configItem2 } };
                var project2 = new Project();
                project2.ConfigFiles.Add(configFile2);

                _executionManager.Project = project2;

                Assert.IsFalse(enumerator.MoveNext());
                Assert.IsNotNull(firstExecutor);

                var newExecutors = (ConcurrentDictionary<ConfigFile, InputEventExecutor>)field.GetValue(_executionManager);
                Assert.AreSame(initialExecutors, newExecutors);

                var result2 = newExecutors.Count;
                Assert.AreEqual(1, result2);
                Assert.IsTrue(newExecutors.ContainsKey(configFile2));
                Assert.IsFalse(newExecutors.ContainsKey(configFile1));
            }
        }

        [TestMethod]
        public void InitInputEventExecutor_WithNoConfigFiles_ShouldReplaceWithEmptyDictionary()
        {
            var project = new Project();
            project.ConfigFiles.Add(new ConfigFile());
            _executionManager.Project = project;

            var field = typeof(ExecutionManager).GetField("_inputEventExecutors", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = ((ConcurrentDictionary<ConfigFile, InputEventExecutor>)field.GetValue(_executionManager)).Count;
            Assert.AreEqual(1, result);

            var emptyProject = new Project();
            _executionManager.Project = emptyProject;

            var executors = (ConcurrentDictionary<ConfigFile, InputEventExecutor>)field.GetValue(_executionManager);
            Assert.IsNotNull(executors);
            var result2 = executors.Count;
            Assert.AreEqual(0, result2);
        }

        [TestMethod]
        public void UndoToggle_MissingItem_PreservesHistory()
        {
            // Arrange
            var item = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Test Item",
                Active = true
            };

            var project = new Project();

            project.ConfigFiles.Add(new ConfigFile
            {
                ConfigItems = { item }
            });

            _executionManager.Project = project;

            // Record a Toggle action.
            MessageExchange.Instance.Publish(
                new CommandConfigContextMenu
                {
                    Action = "toggle",
                    Item = item
                }
            );

            Assert.IsFalse(item.Active);

            // Simulate an unexpected external modification.
            project.ConfigFiles[0].ConfigItems.Remove(item);

            // Act
            MessageExchange.Instance.Publish(
                new CommandUndoConfigList
                {
                    Steps = 1
                }
            );

            // Assert
            // The failed Undo must not alter the project.
            Assert.HasCount(0, project.ConfigFiles[0].ConfigItems);

            // The previous History must still exist.
            var lastUndoState = _mockMessagePublisher.Invocations
                .Where(invocation =>
                    invocation.Method.Name == "Publish")
                .Select(invocation => invocation.Arguments[0])
                .OfType<ConfigListUndoState>()
                .Last();

            Assert.IsTrue(lastUndoState.CanUndo);
            Assert.HasCount(1, lastUndoState.History);
            Assert.AreEqual("Toggle", lastUndoState.History[0]);
        }

        [TestMethod]
        public void UndoDelete_ConflictingGuid_PreservesHistory()
        {
            var original = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Original",
                Active = true
            };

            var file = new ConfigFile
            {
                ConfigItems = { original }
            };

            var project = new Project();
            project.ConfigFiles.Add(file);
            _executionManager.Project = project;

            // Record a Delete operation.
            MessageExchange.Instance.Publish(
                new CommandConfigContextMenu
                {
                    Action = "delete",
                    Item = original
                }
            );

            Assert.HasCount(0, file.ConfigItems);

            // Simulate an external change that reuses the deleted GUID.
            var conflictingItem = new OutputConfigItem
            {
                GUID = original.GUID,
                Name = "Conflicting Item",
                Active = false
            };

            file.ConfigItems.Add(conflictingItem);

            AssertRejectedUndoPreservesHistory("Delete", () =>
                {
                    Assert.HasCount(1, file.ConfigItems);
                    Assert.AreSame(conflictingItem, file.ConfigItems[0]);
                    Assert.AreEqual("Conflicting Item", file.ConfigItems[0].Name);
                }
            );
        }

        [TestMethod]
        public void UndoBulkDelete_ConflictingGuid_PreservesHistory()
        {
            var itemA = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item A",
                Active = true
            };

            var itemB = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item B",
                Active = true
            };

            var file = new ConfigFile
            {
                ConfigItems = { itemA, itemB }
            };

            var project = new Project();
            project.ConfigFiles.Add(file);
            _executionManager.Project = project;

            MessageExchange.Instance.Publish(
                new CommandConfigBulkAction
                {
                    Action = "delete",
                    Items = new List<ConfigItem>
                    {
                        itemA,
                        itemB
                    }
                }
            );

            Assert.HasCount(0, file.ConfigItems);

            // Create a conflict with only one deleted item.
            var conflictingItem = new OutputConfigItem
            {
                GUID = itemB.GUID,
                Name = "External Item",
                Active = true
            };

            file.ConfigItems.Add(conflictingItem);

            AssertRejectedUndoPreservesHistory("BulkDelete", () =>
                {
                    // Neither item should be restored.
                    Assert.HasCount(1, file.ConfigItems);

                    Assert.AreSame(conflictingItem, file.ConfigItems[0]);

                    Assert.IsFalse(file.ConfigItems.Any(item => item.GUID == itemA.GUID));
                }
            );
        }

        [TestMethod]
        public void UndoAdd_MissingItem_PreservesHistory()
        {
            var file = new ConfigFile();

            var project = new Project();
            project.ConfigFiles.Add(file);
            _executionManager.Project = project;

            MessageExchange.Instance.Publish(
                new CommandAddConfigItem
                {
                    Type = "InputConfig",
                    Name = "New Input"
                }
            );

            Assert.HasCount(1, file.ConfigItems);

            // Simulate an external removal of the added item.
            file.ConfigItems.Clear();

            AssertRejectedUndoPreservesHistory("Add", () =>
                {
                    Assert.HasCount(0, file.ConfigItems);
                }
            );
        }

        [TestMethod]
        public void UndoDuplicate_MissingItem_PreservesHistory()
        {
            var original = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Original",
                Active = true
            };

            var file = new ConfigFile
            {
                ConfigItems = { original }
            };

            var project = new Project();
            project.ConfigFiles.Add(file);
            _executionManager.Project = project;

            MessageExchange.Instance.Publish(
                new CommandConfigContextMenu
                {
                    Action = "duplicate",
                    Item = original
                }
            );

            Assert.HasCount(2, file.ConfigItems);

            var duplicated = file.ConfigItems.Single(
                item => item.GUID != original.GUID
            );

            // Simulate an external removal of the duplicated item.
            file.ConfigItems.Remove(duplicated);

            AssertRejectedUndoPreservesHistory("Duplicate", () =>
                {
                    Assert.HasCount(1, file.ConfigItems);

                    // The original item must remain untouched.
                    Assert.AreSame(original, file.ConfigItems[0]);
                }
            );
        }

        [TestMethod]
        public void UndoResort_TargetProfileChanged_PreservesHistory()
        {
            var itemA = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item A"
            };

            var itemB = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item B"
            };

            var itemC = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item C"
            };

            var sourceFile = new ConfigFile
            {
                Label = "Source",
                ConfigItems = { itemA, itemB }
            };

            var targetFile = new ConfigFile
            {
                Label = "Target",
                ConfigItems = { itemC }
            };

            var project = new Project();
            project.ConfigFiles.Add(sourceFile);
            project.ConfigFiles.Add(targetFile);

            _executionManager.Project = project;

            // Move Item A to the target profile.
            MessageExchange.Instance.Publish(
                new CommandResortConfigItem
                {
                    Items = new[] {new OutputConfigItem {GUID = itemA.GUID}},
                    SourceFileIndex = 0,
                    TargetFileIndex = 1,
                    NewIndex = 0
                }
            );

            Assert.HasCount(1, sourceFile.ConfigItems);
            Assert.HasCount(2, targetFile.ConfigItems);

            // Simulate an unexpected replacement of the target profile.
            var replacementFile = new ConfigFile
            {
                Label = "Replacement"
            };

            project.ConfigFiles[1] = replacementFile;

            AssertRejectedUndoPreservesHistory("Resort", () =>
                {
                    Assert.AreSame(replacementFile, project.ConfigFiles[1]);

                    Assert.HasCount(1, sourceFile.ConfigItems);
                    Assert.HasCount(0, replacementFile.ConfigItems);

                    // The failed Undo must not modify the original
                    // target object either.
                    Assert.HasCount(2, targetFile.ConfigItems);

                    Assert.AreEqual(itemA.GUID, targetFile.ConfigItems[0].GUID);
                }
            );
        }

        [TestMethod]
        public void UndoResort_GuidSetChanged_PreservesHistory()
        {
            var itemA = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item A"
            };

            var itemB = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item B"
            };

            var itemC = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Item C"
            };

            var file = new ConfigFile
            {
                ConfigItems = { itemA, itemB, itemC }
            };

            var project = new Project();
            project.ConfigFiles.Add(file);
            _executionManager.Project = project;

            // Move Item B before Item A.
            MessageExchange.Instance.Publish(
                new CommandResortConfigItem
                {
                    Items = new[] {new OutputConfigItem {GUID = itemB.GUID}},
                    SourceFileIndex = 0,
                    TargetFileIndex = 0,
                    NewIndex = 0
                }
            );

            CollectionAssert.AreEqual(
                new[] { itemB.GUID, itemA.GUID, itemC.GUID },
                file.ConfigItems.Select(item => item.GUID).ToArray()
            );

            // Simulate an unexpected external addition.
            var unexpectedItem = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Unexpected"
            };

            file.ConfigItems.Add(unexpectedItem);

            var orderBeforeUndo = file.ConfigItems.Select(item => item.GUID).ToArray();

            AssertRejectedUndoPreservesHistory("Resort", () =>
                {
                    var orderAfterUndo = file.ConfigItems.Select(item => item.GUID).ToArray();

                    // Failed Undo must not reorder any items.
                    CollectionAssert.AreEqual(orderBeforeUndo, orderAfterUndo);

                    Assert.HasCount(4, file.ConfigItems);
                }
            );
        }

        [TestMethod]
        public void UndoToggleAndItemEdited_RestoresItemsInReverseOrder()
        {
            // Arrange
            var original = new InputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Original Input",
                Active = true
            };

            var file = new ConfigFile
            {
                ConfigItems = { original }
            };

            var project = new Project();
            project.ConfigFiles.Add(file);

            _executionManager.Project = project;

            int recordedActions = 0;
            int undoneActions = 0;

            _executionManager.OnConfigListActionRecorded += (sender, args) =>
            {
                recordedActions++;
            };

            _executionManager.OnConfigListActionUndone += (sender, args) =>
            {
                undoneActions++;
            };

            // Helper to verify the latest published Global History.
            void AssertHistory(params string[] expectedActions)
            {
                var states = GetPublishedMessages<ConfigListUndoState>();

                Assert.IsNotEmpty(states);

                var latestState = states.Last();

                CollectionAssert.AreEqual(expectedActions, latestState.History);

                Assert.AreEqual(expectedActions.Length > 0, latestState.CanUndo);

                Assert.AreEqual(0L, latestState.HistoryStart);
            }

            // Toggle the original ConfigItem.
            MessageExchange.Instance.Publish(
                new CommandConfigContextMenu
                {
                    Action = "toggle",
                    Item = original
                }
            );

            Assert.IsFalse(file.ConfigItems[0].Active);
            Assert.AreEqual("Original Input", file.ConfigItems[0].Name);

            AssertHistory("Toggle");
            Assert.AreEqual(1, recordedActions);

            // Edit and Apply the ConfigItem.
            // The editor creates a modified copy instead of
            // mutating the committed ConfigItem directly.
            var editedItem = (InputConfigItem)file.ConfigItems[0].Clone();

            editedItem.Name = "Edited Input";

            // This is the real backend handler used by
            // CommandUpdateConfigItem when the user clicks Apply.
            _executionManager.HandleCommandUpdateConfigItem(editedItem);

            Assert.AreEqual("Edited Input", file.ConfigItems[0].Name);
            Assert.IsFalse(file.ConfigItems[0].Active);

            AssertHistory("Toggle", "ItemEdited");
            Assert.AreEqual(2, recordedActions);

            // Undo ItemEdited.
            int fullUpdatesBeforeUndo =
                GetPublishedMessages<ConfigValueFullUpdate>().Count;

            MessageExchange.Instance.Publish(
                new CommandUndoConfigList
                {
                    Steps = 1
                }
            );

            // The original name must be restored.
            Assert.AreEqual("Original Input", file.ConfigItems[0].Name);

            // The earlier Toggle must remain applied.
            Assert.IsFalse(file.ConfigItems[0].Active);

            // Only ItemEdited should be removed from history.
            AssertHistory("Toggle");

            Assert.AreEqual(2, recordedActions);
            Assert.AreEqual(1, undoneActions);

            Assert.HasCount(
                fullUpdatesBeforeUndo + 1,
                GetPublishedMessages<ConfigValueFullUpdate>());

            // Undo Toggle.
            MessageExchange.Instance.Publish(
                new CommandUndoConfigList
                {
                    Steps = 1
                }
            );

            // The original configuration must be fully restored.
            Assert.AreEqual("Original Input", file.ConfigItems[0].Name);
            Assert.IsTrue(file.ConfigItems[0].Active);

            Assert.AreEqual(original.GUID, file.ConfigItems[0].GUID);

            // Both actions have now been undone.
            AssertHistory();

            Assert.AreEqual(2, recordedActions);
            Assert.AreEqual(2, undoneActions);

            Assert.HasCount(
                fullUpdatesBeforeUndo + 2,
                GetPublishedMessages<ConfigValueFullUpdate>());

            // No undo failure should have occurred.
            var failureNotifications = GetPublishedMessages<Notification>()
                .Where(notification =>
                    notification.Event == "ConfigListUndoFailed")
                .ToList();

            Assert.HasCount(0, failureNotifications);
        }

        [TestMethod]
        public void UndoMultipleSteps_RestoresOriginalConfigItem()
        {
            // Arrange
            var original = new InputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Original Input",
                Active = true
            };

            var file = new ConfigFile
            {
                ConfigItems = { original }
            };

            var project = new Project();
            project.ConfigFiles.Add(file);

            _executionManager.Project = project;

            // Toggle
            MessageExchange.Instance.Publish(
                new CommandConfigContextMenu
                {
                    Action = "toggle",
                    Item = original
                }
            );

            Assert.IsFalse(file.ConfigItems[0].Active);

            // Edit
            var editedItem = (InputConfigItem)file.ConfigItems[0].Clone();

            editedItem.Name = "Edited Input";

            _executionManager.HandleCommandUpdateConfigItem(editedItem);

            Assert.AreEqual("Edited Input", file.ConfigItems[0].Name);
            Assert.IsFalse(file.ConfigItems[0].Active);

            // Verify that both actions were recorded.
            var historyBeforeUndo = GetPublishedMessages<ConfigListUndoState>().Last();

            CollectionAssert.AreEqual(new[] { "Toggle", "ItemEdited" }, historyBeforeUndo.History);

            // Undo both actions in one command.
            int fullUpdatesBeforeUndo = GetPublishedMessages<ConfigValueFullUpdate>().Count;

            MessageExchange.Instance.Publish(
                new CommandUndoConfigList
                {
                    Steps = 2
                }
            );

            // Verify complete restoration.
            Assert.HasCount(1, file.ConfigItems);

            Assert.AreEqual("Original Input", file.ConfigItems[0].Name);

            Assert.IsTrue(file.ConfigItems[0].Active);

            Assert.AreEqual(original.GUID, file.ConfigItems[0].GUID);

            // Global History must now be empty.
            var historyAfterUndo =
                GetPublishedMessages<ConfigListUndoState>().Last();

            Assert.IsFalse(historyAfterUndo.CanUndo);

            Assert.HasCount(0, historyAfterUndo.History);

            Assert.AreEqual(0L, historyAfterUndo.HistoryStart);

            // Each restored step must synchronize the frontend.
            Assert.HasCount(
                fullUpdatesBeforeUndo + 2,
                GetPublishedMessages<ConfigValueFullUpdate>());

            // No validation failure should have occurred.
            var failureNotifications = GetPublishedMessages<Notification>()
                .Where(notification =>
                    notification.Event == "ConfigListUndoFailed")
                .ToList();

            Assert.HasCount(0, failureNotifications);
        }

        [TestMethod]
        public void UndoDelete_RestoresOriginalItemAtOriginalIndex()
        {
            var first = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "First",
                Active = true
            };

            var deleted = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Deleted Item",
                Active = false
            };

            var last = new OutputConfigItem
            {
                GUID = Guid.NewGuid().ToString(),
                Name = "Last",
                Active = true
            };

            var file = new ConfigFile
            {
                ConfigItems = { first, deleted, last }
            };

            var project = new Project();
            project.ConfigFiles.Add(file);
            _executionManager.Project = project;

            // Delete the middle item.
            MessageExchange.Instance.Publish(
                new CommandConfigContextMenu
                {
                    Action = "delete",
                    Item = deleted
                }
            );

            Assert.HasCount(2, file.ConfigItems);
            Assert.AreEqual(first.GUID, file.ConfigItems[0].GUID);
            Assert.AreEqual(last.GUID, file.ConfigItems[1].GUID);

            var history = GetPublishedMessages<ConfigListUndoState>().Last();

            CollectionAssert.AreEqual(
                new[] { "Delete" },
                history.History
            );

            // Undo deletion.
            MessageExchange.Instance.Publish(
                new CommandUndoConfigList { Steps = 1 }
            );

            // Verify original order and item properties.
            Assert.HasCount(3, file.ConfigItems);

            CollectionAssert.AreEqual(
                new[] { first.GUID, deleted.GUID, last.GUID },
                file.ConfigItems.Select(item => item.GUID).ToArray()
            );

            var restored = file.ConfigItems[1];

            Assert.AreEqual("Deleted Item", restored.Name);
            Assert.IsFalse(restored.Active);

            // Verify that Undo consumed the history entry.
            history = GetPublishedMessages<ConfigListUndoState>().Last();

            Assert.IsFalse(history.CanUndo);
            Assert.HasCount(0, history.History);

            Assert.IsFalse(
                GetPublishedMessages<Notification>()
                    .Any(n => n.Event == "ConfigListUndoFailed")
            );
        }

    }
}
