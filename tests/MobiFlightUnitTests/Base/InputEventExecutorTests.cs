using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.ProSim;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using Moq;
using System.Collections.Generic;

namespace MobiFlight.Execution.Tests
{
    [TestClass]
    public class InputEventExecutorTests
    {
        private Mock<InputActionExecutionCache> _mockInputActionExecutionCache;
        private Mock<Fsuipc2Cache> _mockFsuipcCache;
        private Mock<SimConnectCacheInterface> _mockSimConnectCache;
        private Mock<XplaneCache> _mockXplaneCache;
        private Mock<MobiFlightCache> _mockMobiFlightCache;
        private Mock<ProSimCache> _mockProSimCache;
        private Mock<JoystickManager> _mockJoystickManager;
        private Mock<ArcazeCache> _mockArcazeCache;
        private List<IConfigItem> _configItems;
        private InputEventExecutor _executor;
        private Mock<ILogAppender> _mockLogAppender;
        private LogSeverity _logSeverity = LogSeverity.Error;

        [TestInitialize]
        public void SetUp()
        {
            _mockInputActionExecutionCache = new Mock<InputActionExecutionCache>();
            _mockFsuipcCache = new Mock<Fsuipc2Cache>();
            _mockSimConnectCache = new Mock<SimConnectCacheInterface>();
            _mockXplaneCache = new Mock<XplaneCache>();
            _mockMobiFlightCache = new Mock<MobiFlightCache>();
            _mockProSimCache = new Mock<ProSimCache>();
            _mockJoystickManager = new Mock<JoystickManager>();
            _mockArcazeCache = new Mock<ArcazeCache>();

            _configItems = new List<IConfigItem>()
            {
                new OutputConfigItem
                {
                    Active = true,
                    Controller = new Controller() { Name = "OutputDevice", Serial = "1123" },
                    Name = "OutputConfigItem",
                },

                new InputConfigItem
                {
                    Active = true,
                    Controller = new Controller() { Name = "InputDevice", Serial = "2123" },
                    Name = "InputConfigItem"
                }
            };

            _executor = new InputEventExecutor(
                _configItems,
                _mockInputActionExecutionCache.Object,
                _mockFsuipcCache.Object,
                _mockSimConnectCache.Object,
                _mockXplaneCache.Object,
                _mockMobiFlightCache.Object,
                _mockProSimCache.Object,
                _mockJoystickManager.Object,
                _mockArcazeCache.Object
            );

            // Create a mock log appender
            _mockLogAppender = new Mock<ILogAppender>();
            Log.Instance.Enabled = true; // Enable logging
            _logSeverity = Log.Instance.Severity; // Store the current log severity
            Log.Instance.Severity = LogSeverity.Debug; // Set the log severity to Debug
            Log.Instance.ClearAppenders();
            Log.Instance.AddAppender(_mockLogAppender.Object);
        }

        private InputConfigItem CreateInputConfigItemWithButton(string name, string moduleSerial, string deviceName, bool active, string command)
        {
            return new InputConfigItem
            {
                Active = active,
                Controller = SerialNumber.CreateController(moduleSerial),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = name,
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = command,
                        PresetId = "TestPresetId",
                    }
                }
            };
        }

        [TestCleanup]
        public void TearDown()
        {
            // Remove the mock appender after each test
            Log.Instance.ClearAppenders();
            Log.Instance.Severity = _logSeverity; // Restore the original log severity
            Log.Instance.Enabled = false; // Disable logging
        }

        [TestMethod]
        public void Execute_NoMatchingConfigItems_ReturnsEmptyDictionary()
        {
            // Arrange
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = "123" },
                Device = new DeviceReference() { Name = "Device1" },
                InputType = DeviceType.Button,
            };

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.IsEmpty(result);
        }

        [TestMethod]
        public void Execute_MatchingInactiveConfigItem_SkipsExecution()
        {
            // Arrange
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = "123" },
                InputType = DeviceType.Button,
                Device = new DeviceReference() { Name = "Device1" }
            };

            var inactiveConfigItem = new InputConfigItem
            {
                Active = false,
                Controller = SerialNumber.CreateController("/ 123"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Device1"),
                Name = "TestConfig"
            };

            _configItems.Add(inactiveConfigItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.IsEmpty(result);

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"Skipping inactive config ""{inactiveConfigItem.Name}""")), LogSeverity.Warn),
                Times.Once
            );
        }

        [TestMethod]
        public void Execute_MatchingActiveConfigItem_ExecutesSuccessfully()
        {
            // Arrange
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = "123" },
                InputType = DeviceType.Button,
                Device = new DeviceReference() { Name = "Device1" },
                Value = 1
            };

            var activeConfigItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("/ 123"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Device1"),
                Name = "TestConfig"
            };

            _configItems.Add(activeConfigItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result);
            Assert.IsTrue(result.ContainsKey(activeConfigItem.GUID));

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"Executing ""{activeConfigItem.Name}"". (RELEASE)")), LogSeverity.Info),
                Times.Once
            );
        }

        [TestMethod]
        public void Execute_ConfigItemWithConfigReference_ExecutesSuccessfully()
        {
            // Arrange
            var buttonId = "Button1";
            var baseCommand = "(>K:TestCommand:#)";

            // Create a simple button event
            InputEventArgs inputEventArgs = CreateButtonEventArgs("123", buttonId, true);

            var activeConfigItem = CreateInputConfigItemWithButton(
                name: "TestConfig",
                moduleSerial: "testcontroller / 123",
                deviceName: buttonId,
                active: true,
                command: baseCommand
            );

            // Set a non sense config reference
            // This should not have any effect on the test
            _configItems[0].ConfigRefs = new ConfigRefList()
            {
                new ConfigRef()
                {
                    Active = true,
                    Ref = "non-existing-doesnt-matter",
                    Placeholder = "K",
                    TestValue = "1"
                }
            };

            // Set a non-null value for the test
            _configItems[0].Value = "FinalValue";

            // Create a config reference 
            // that actually uses the first config item
            // and its value
            var configRef = new ConfigRef()
            {
                Active = true,
                Placeholder = "#",
                Ref = _configItems[0].GUID,
                TestValue = "TestValue"
            };

            activeConfigItem.ConfigRefs.Add(configRef);

            // Out input config item is added to the list of configs
            _configItems.Add(activeConfigItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Only one item should be executed.");
            Assert.IsTrue(result.ContainsKey(activeConfigItem.GUID), "The wrong config item was executed.");

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"Executing ""{activeConfigItem.Name}"". (PRESS)")), LogSeverity.Info),
                Times.Once,
                "The config item should be executed with an OnPress event."
            );

            _mockSimConnectCache.Verify(
                cache => cache.SetSimVar(It.Is<string>(str => str == baseCommand.Replace("#", _configItems[0].Value))),
                Times.Once,
                "A wrong command has been executed."
            );
        }

        private static InputEventArgs CreateButtonEventArgs(string serial, string deviceId, bool isOnPress)
        {
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = serial },
                InputType = DeviceType.Button,
                Device = new DeviceReference() { Name = deviceId },
                Value = isOnPress ? 0 : 1 // onPress else onRelease
            };
            return inputEventArgs;
        }

        [TestMethod]
        public void Execute_PreconditionsNotSatisfied_SkipsExecution()
        {
            // Arrange
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = "123" },
                InputType = DeviceType.Button,
                Device = new DeviceReference() { Name = "Device1" },
                Value = 1
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("/ 123"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Device1"),
                Name = "TestConfig",
                Preconditions = new PreconditionList()
                {
                    new Precondition
                    {
                        Type = "variable",
                        Active = true,
                        Ref = "TestRef",
                        Value = "OtherValue"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.IsEmpty(result);

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"Preconditions not satisfied for ""{configItem.Name}"".")), LogSeverity.Debug),
                Times.Once
            );
        }

        [TestMethod]
        public void Execute_NotStarted_SkipsExecution()
        {
            // Arrange
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = "123" },
                InputType = DeviceType.Button,
                Device = new DeviceReference() { Name = "Device1" },
                Value = 1
            };

            var activeConfigItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("/ 123"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Device1"),
                Name = "TestConfig"
            };

            _configItems.Add(activeConfigItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: false);

            // Assert
            Assert.IsEmpty(result);

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains("skipping, MobiFlight not running.")), LogSeverity.Warn),
                Times.Once
            );
        }

        #region Default Device Type Tests - Happy Path Scenarios

        [TestMethod]
        public void Execute_RegularButton_ExecutesSuccessfully()
        {
            // Arrange - Test default case: regular button with proper config
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-btn001",
                },
                Device = new DeviceReference()
                {
                    Name = "Button1"
                },
                InputType = DeviceType.Button,
                Value = 0, // PRESS event
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-btn001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Button1"),
                Name = "RegularButton",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Regular button should execute");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));
        }

        [TestMethod]
        public void Execute_Encoder_ExecutesSuccessfully()
        {
            // Arrange - Test default case: encoder with proper config
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-enc001",
                },
                Device = new DeviceReference()
                {
                    Name = "Encoder1"
                },
                InputType = DeviceType.Encoder,
                Value = 1, // Rotation value
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-enc001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_ENCODER, "Encoder1"),
                Name = "TestEncoder",
                encoder = new EncoderInputConfig()
                {
                    onLeft = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestLeft)",
                        PresetId = "TestPresetId"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Encoder should execute");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));
        }

        [TestMethod]
        public void Execute_InputShiftRegisterWithMatchingPin_ExecutesSuccessfully()
        {
            // Arrange - Test default case: input shift register with matching pin
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-isr001",
                },
                Device = new DeviceReference()
                {
                    Name = "InputShifter:5",
                    Type = DeviceType.InputShiftRegister
                },
                InputType = DeviceType.Button,
                Value = 0
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-isr001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.DEPRECATED_TYPE_INPUT_SHIFT_REGISTER, "InputShifter", 5),
                Name = "TestInputShiftRegister",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Input shift register with matching pin should execute");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));
        }

        [TestMethod]
        public void Execute_InputMultiplexerWithMatchingPin_ExecutesSuccessfully()
        {
            // Arrange - Test default case: input multiplexer with matching pin
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-mux001",
                },
                Device = new DeviceReference()
                {
                    Name = "InputMux:3",
                    Type = DeviceType.InputMultiplexer
                },
                InputType = DeviceType.Button,
                Value = 0,
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-mux001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.DEPRECATED_TYPE_INPUT_MULTIPLEXER, "InputMux", 3),
                Name = "TestInputMultiplexer",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Input multiplexer with matching pin should execute");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));
        }

        [TestMethod]
        public void Execute_AnalogInput_ExecutesSuccessfully()
        {
            // Arrange - Test default case: analog input with proper config
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-analog001",
                },
                Device = new DeviceReference()
                {
                    Name = "Analog1",
                    Type = DeviceType.AnalogInput
                },
                InputType = DeviceType.AnalogInput,
                Value = 512, // Analog value
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-analog001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_ANALOG, "Analog1"),
                Name = "TestAnalogInput",
                analog = new AnalogInputConfig()
                {
                    onChange = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Analog input should execute");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));
        }

        #endregion

        #region Hold Timer Cleanup on Skip

        private static System.Timers.Timer GetHoldTimer(ButtonInputConfig button)
        {
            return (System.Timers.Timer)typeof(ButtonInputConfig)
                .GetField("HoldTimer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .GetValue(button);
        }

        [TestMethod]
        public void Execute_ConfigDeactivatedWhileButtonHeld_StopsHoldTimer()
        {
            var serial = "SN-deact001";
            var deviceName = "Button1";

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "DeactivatedConfig",
                button = new ButtonInputConfig
                {
                    onHold = new MSFS2020CustomInputAction { Command = "(>K:HoldCommand)", PresetId = "p1" },
                    HoldDelay = 10000
                }
            };
            _configItems.Add(configItem);

            _executor.Execute(CreateButtonEventArgs(serial, deviceName, isOnPress: true), isStarted: true);

            var holdTimer = GetHoldTimer(configItem.button);
            Assert.IsTrue(holdTimer.Enabled, "Timer should be running after press.");

            configItem.Active = false;

            _executor.Execute(CreateButtonEventArgs(serial, deviceName, isOnPress: false), isStarted: true);

            Assert.IsFalse(holdTimer.Enabled, "Timer should be stopped when the config is deactivated.");
        }

        [TestMethod]
        public void Execute_PreconditionFailsWhileButtonHeld_StopsHoldTimer()
        {
            var serial = "SN-precond001";
            var deviceName = "Button1";

            _configItems[0].Value = "ExpectedValue";

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "PreconditionConfig",
                button = new ButtonInputConfig
                {
                    onHold = new MSFS2020CustomInputAction { Command = "(>K:HoldCommand)", PresetId = "p2" },
                    HoldDelay = 10000
                },
                Preconditions = new PreconditionList
                {
                    new Precondition { Type = "config", Active = true, Ref = _configItems[0].GUID, Value = "ExpectedValue" }
                }
            };
            _configItems.Add(configItem);

            _executor.Execute(CreateButtonEventArgs(serial, deviceName, isOnPress: true), isStarted: true);

            var holdTimer = GetHoldTimer(configItem.button);
            Assert.IsTrue(holdTimer.Enabled, "Timer should be running after press with precondition satisfied.");

            _configItems[0].Value = "DifferentValue";

            _executor.Execute(CreateButtonEventArgs(serial, deviceName, isOnPress: false), isStarted: true);

            Assert.IsFalse(holdTimer.Enabled, "Timer should be stopped when the precondition is not satisfied.");
        }

        #endregion

        #region Edge Cases - Stale Configs With Correct DeviceType

        [TestMethod]
        public void Execute_ButtonWithStaleEncoderConfig_ExecutesSuccessfully()
        {
            // Arrange - Edge case: button config with stale encoder config (shouldn't affect execution)
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-edge001",
                },
                Device = new DeviceReference()
                {
                    Name = "Button1",
                    Type = DeviceType.Button
                },
                InputType = DeviceType.Button,
                Value = 0
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-edge001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Button1"),
                Name = "ButtonWithStaleEncoder",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                },
                // Stale config that should be ignored
                encoder = new EncoderInputConfig()
                {
                    onLeft = new MSFS2020CustomInputAction() { Command = "(>K:StaleCommand)" }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Button with stale encoder config should still execute");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));
        }

        [TestMethod]
        public void Execute_EncoderWithStaleButtonConfig_ExecutesSuccessfully()
        {
            // Arrange - Edge case: encoder config with stale button config
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-edge002",
                },
                Device = new DeviceReference()
                {
                    Name = "Encoder1",
                    Type = DeviceType.Encoder
                },
                InputType = DeviceType.Encoder,
                Value = 1
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-edge002"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_ENCODER, "Encoder1"),
                Name = "EncoderWithStaleButton",
                encoder = new EncoderInputConfig()
                {
                    onLeft = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                },
                // Stale config that should be ignored
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction() { Command = "(>K:StaleCommand)" }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Encoder with stale button config should still execute");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));
        }

        [TestMethod]
        public void Execute_InputShiftRegisterWithWrongPinButCorrectDeviceType_Skips()
        {
            // Arrange - Edge case: correct DeviceType but wrong pin should skip
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller()
                {
                    Serial = "SN-edge003",
                },
                Device = new DeviceReference()
                {
                    // Different pin
                    Name = "InputShifter:3",
                    Type = DeviceType.InputShiftRegister,
                },
                InputType = DeviceType.Button,
                Value = 0 
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-edge003"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.DEPRECATED_TYPE_INPUT_SHIFT_REGISTER, "InputShifter", 7),
                Name = "ShiftRegisterWrongPin",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(0, result, "Input shift register with wrong pin should be skipped");
        }

        [TestMethod]
        public void Execute_MultipleConfigsSameSerialDifferentDevices_ExecutesOnlyMatching()
        {
            // Arrange - Edge case: multiple configs with same serial but different devices
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = "SN-multi001" },
                Device = new DeviceReference()
                {
                    Name = "Button2",
                    Type = DeviceType.Button
                },
                InputType = DeviceType.Button,
                Value = 0
            };

            var configItem1 = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-multi001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Button1"),
                Name = "Button1Config",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction() { Command = "(>K:Button1)" }
                }
            };

            var configItem2 = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-multi001"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Button2"), // Matching device
                Name = "Button2Config",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction() { Command = "(>K:Button2)" }
                }
            };

            _configItems.Add(configItem1);
            _configItems.Add(configItem2);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Only the matching device config should execute");
            Assert.IsTrue(result.ContainsKey(configItem2.GUID), "Should execute Button2Config");
            Assert.IsFalse(result.ContainsKey(configItem1.GUID), "Should not execute Button1Config");
        }

        #endregion
    }
}