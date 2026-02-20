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
                DeviceName = deviceName,
                DeviceType = DeviceType.Button.ToString(),
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
                Serial = "123",
                Type = DeviceType.Button,
                DeviceId = "Device1"
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
                Serial = "123",
                Type = DeviceType.Button,
                DeviceId = "Device1"
            };

            var inactiveConfigItem = new InputConfigItem
            {
                Active = false,
                Controller = SerialNumber.CreateController("/ 123"),
                DeviceName = "Device1",
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
                Serial = "123",
                Type = DeviceType.Button,
                DeviceId = "Device1",
                Value = 1
            };

            var activeConfigItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("/ 123"),
                DeviceName = "Device1",
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
                Serial = serial,
                Type = DeviceType.Button,
                DeviceId = deviceId,
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
                Serial = "123",
                Type = DeviceType.Button,
                DeviceId = "Device1",
                Value = 1
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("/ 123"),
                DeviceName = "Device1",
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
                Serial = "123",
                Type = DeviceType.Button,
                DeviceId = "Device1",
                Value = 1
            };

            var activeConfigItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("/ 123"),
                DeviceName = "Device1",
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

        [TestMethod]
        public void Execute_ConvertedFromMultiplexerToButton_ExecutesSuccessfully()
        {
            // Arrange
            // This test reproduces the issue where a user changes from multiplexer to regular button
            // but the old inputMultiplexer config is not cleared, causing the event to be skipped
            var inputEventArgs = new InputEventArgs
            {
                Serial = "SN-a1b2c3",
                Type = DeviceType.Button,
                DeviceId = "Device1",
                Value = 0, // PRESS event
                ExtPin = null // Regular buttons don't have ExtPin
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-a1b2c3"),
                DeviceName = "Device1",
                DeviceType = InputConfigItem.TYPE_BUTTON,
                Name = "TestConfig",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                },
                // Simulate the bug: inputMultiplexer is not cleared when type changed
                inputMultiplexer = new InputMultiplexerConfig()
                {
                    DataPin = 5 // This should be null/cleared when type is Button
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Button event should be executed even if old multiplexer config exists");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"Executing ""{configItem.Name}"". (PRESS)")), LogSeverity.Info),
                Times.Once
            );
        }

        [TestMethod]
        public void Execute_ConvertedFromInputShiftRegisterToButton_ExecutesSuccessfully()
        {
            // Arrange
            // This test reproduces the issue where a user changes from input shift register to regular button
            // but the old inputShiftRegister config is not cleared, causing the event to be skipped
            var inputEventArgs = new InputEventArgs
            {
                Serial = "SN-d4e5f6",
                Type = DeviceType.Button,
                DeviceId = "Device1",
                Value = 0, // PRESS event
                ExtPin = null // Regular buttons don't have ExtPin
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-d4e5f6"),
                DeviceName = "Device1",
                DeviceType = InputConfigItem.TYPE_BUTTON,
                Name = "TestConfig",
                button = new ButtonInputConfig()
                {
                    onPress = new MSFS2020CustomInputAction()
                    {
                        Command = "(>K:TestCommand)",
                        PresetId = "TestPresetId"
                    }
                },
                // Simulate the bug: inputShiftRegister is not cleared when type changed
                inputShiftRegister = new InputShiftRegisterConfig()
                {
                    ExtPin = 3 // This should be null/cleared when type is Button
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.HasCount(1, result, "Button event should be executed even if old shift register config exists");
            Assert.IsTrue(result.ContainsKey(configItem.GUID));

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"Executing ""{configItem.Name}"". (PRESS)")), LogSeverity.Info),
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
                Serial = "SN-btn001",
                Type = DeviceType.Button,
                DeviceId = "Button1",
                Value = 0, // PRESS event
                ExtPin = null
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-btn001"),
                DeviceName = "Button1",
                DeviceType = InputConfigItem.TYPE_BUTTON,
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
                Serial = "SN-enc001",
                Type = DeviceType.Encoder,
                DeviceId = "Encoder1",
                Value = 1, // Rotation value
                ExtPin = null
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-enc001"),
                DeviceName = "Encoder1",
                DeviceType = InputConfigItem.TYPE_ENCODER,
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
                Serial = "SN-isr001",
                Type = DeviceType.Button,
                DeviceId = "InputShifter",
                Value = 0,
                ExtPin = 5 // Matching pin
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-isr001"),
                DeviceName = "InputShifter",
                DeviceType = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER,
                Name = "TestInputShiftRegister",
                inputShiftRegister = new InputShiftRegisterConfig()
                {
                    ExtPin = 5, // Same pin as event
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
                Serial = "SN-mux001",
                Type = DeviceType.Button,
                DeviceId = "InputMux",
                Value = 0,
                ExtPin = 3 // Matching pin
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-mux001"),
                DeviceName = "InputMux",
                DeviceType = InputConfigItem.TYPE_INPUT_MULTIPLEXER,
                Name = "TestInputMultiplexer",
                inputMultiplexer = new InputMultiplexerConfig()
                {
                    DataPin = 3, // Same pin as event
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
                Serial = "SN-analog001",
                Type = DeviceType.AnalogInput,
                DeviceId = "Analog1",
                Value = 512, // Analog value
                ExtPin = null
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-analog001"),
                DeviceName = "Analog1",
                DeviceType = InputConfigItem.TYPE_ANALOG,
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

        #region Edge Cases - Stale Configs With Correct DeviceType

        [TestMethod]
        public void Execute_ButtonWithStaleEncoderConfig_ExecutesSuccessfully()
        {
            // Arrange - Edge case: button config with stale encoder config (shouldn't affect execution)
            var inputEventArgs = new InputEventArgs
            {
                Serial = "SN-edge001",
                Type = DeviceType.Button,
                DeviceId = "Button1",
                Value = 0,
                ExtPin = null
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-edge001"),
                DeviceName = "Button1",
                DeviceType = InputConfigItem.TYPE_BUTTON, // Correct DeviceType
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
                Serial = "SN-edge002",
                Type = DeviceType.Encoder,
                DeviceId = "Encoder1",
                Value = 1,
                ExtPin = null
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-edge002"),
                DeviceName = "Encoder1",
                DeviceType = InputConfigItem.TYPE_ENCODER, // Correct DeviceType
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
                Serial = "SN-edge003",
                Type = DeviceType.Button,
                DeviceId = "InputShifter",
                Value = 0,
                ExtPin = 3 // Different pin
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-edge003"),
                DeviceName = "InputShifter",
                DeviceType = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER,
                Name = "ShiftRegisterWrongPin",
                inputShiftRegister = new InputShiftRegisterConfig()
                {
                    ExtPin = 7, // Different pin - should skip
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
        public void Execute_InputMultiplexerWithWrongPinButCorrectDeviceType_Skips()
        {
            // Arrange - Edge case: correct DeviceType but wrong pin should skip
            var inputEventArgs = new InputEventArgs
            {
                Serial = "SN-edge004",
                Type = DeviceType.Button,
                DeviceId = "InputMux",
                Value = 0,
                ExtPin = 2 // Different pin
            };

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-edge004"),
                DeviceName = "InputMux",
                DeviceType = InputConfigItem.TYPE_INPUT_MULTIPLEXER,
                Name = "MultiplexerWrongPin",
                inputMultiplexer = new InputMultiplexerConfig()
                {
                    DataPin = 8, // Different pin - should skip
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
            Assert.HasCount(0, result, "Input multiplexer with wrong pin should be skipped");
        }

        [TestMethod]
        public void Execute_MultipleConfigsSameSerialDifferentDevices_ExecutesOnlyMatching()
        {
            // Arrange - Edge case: multiple configs with same serial but different devices
            var inputEventArgs = new InputEventArgs
            {
                Serial = "SN-multi001",
                Type = DeviceType.Button,
                DeviceId = "Button2",
                Value = 0,
                ExtPin = null
            };

            var configItem1 = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("TestModule / SN-multi001"),
                DeviceName = "Button1", // Different device
                DeviceType = InputConfigItem.TYPE_BUTTON,
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
                DeviceName = "Button2", // Matching device
                DeviceType = InputConfigItem.TYPE_BUTTON,
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