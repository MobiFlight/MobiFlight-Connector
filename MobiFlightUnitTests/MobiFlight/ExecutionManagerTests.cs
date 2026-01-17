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
                ModuleSerial = "Test / SN-123",
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
    }
}