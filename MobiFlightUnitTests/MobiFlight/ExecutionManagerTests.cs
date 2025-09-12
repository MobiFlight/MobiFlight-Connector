using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.FSUIPC;
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

        [TestInitialize]
        public void Setup()
        {
            // disable schema validation to not exceed 1,000 limit per hour
            // https://www.newtonsoft.com/jsonschema
            JsonBackedObject.SkipSchemaValidation = true;

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

            Assert.AreEqual(_executionManager.ActiveConfigIndex, 0);

            var message = new CommandActiveConfigFile
            {
                index = 1
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.AreEqual(_executionManager.ActiveConfigIndex, 1);
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

            Assert.AreEqual(_executionManager.ActiveConfigIndex, 0);

            var message = new CommandFileContextMenu
            {
                Action = CommandFileContextMenuAction.remove,
                Index = 1,
                File = project.ConfigFiles[1]
            };

            // Act
            MessageExchange.Instance.Publish(message);

            // Assert
            Assert.AreEqual(project.ConfigFiles.Count, 1);
            Assert.AreEqual(project.ConfigFiles[0].Label, "First Config");
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

            Assert.AreEqual(_executionManager.ActiveConfigIndex, 0);

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
            Assert.AreEqual(project.ConfigFiles.Count, 2);
            Assert.AreEqual(project.ConfigFiles[1].Label, "Renamed Config");
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
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.ContainsKey("varA"));

            var message = new CommandActiveConfigFile
            {
                index = 1
            };

            // Act
            MessageExchange.Instance.Publish(message);

            result = _executionManager.GetAvailableVariables();

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.ContainsKey("varB"));
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
                Serial = "SN-000-001",
                DeviceId = "TestDevice",
                DeviceLabel = "Test Button",
                Name = "TestButton",
                Type = DeviceType.Button,
                ExtPin = 1,
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
            var updatedValues = (Dictionary<string, IConfigItem>)updatedValuesField.GetValue(_executionManager);

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

            Assert.AreEqual(0, collectionModifiedExceptions.Count,
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
            var updatedValues = (Dictionary<string, IConfigItem>)updatedValuesField.GetValue(_executionManager);
            
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
    }
}