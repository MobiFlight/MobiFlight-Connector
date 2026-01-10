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
                    ModuleSerial = "OutputDevice / 1123",
                    Name = "OutputConfigItem",
                },

                new InputConfigItem
                {
                    Active = true,
                    ModuleSerial = "InputDevice / 2123",
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
                ModuleSerial = moduleSerial,
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
                ModuleSerial = "/ 123",
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
                ModuleSerial = "/ 123",
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
                ModuleSerial = "/ 123",
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
                ModuleSerial = "/ 123",
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
                ModuleSerial = "TestModule / SN-a1b2c3",
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
                ModuleSerial = "TestModule / SN-d4e5f6",
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

        #region MatchesControllerAndDeviceName Tests

        [TestMethod]
        public void MatchesControllerAndDeviceName_NullModuleSerial_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem { ModuleSerial = null };
            var e = new InputEventArgs { Serial = "SN-123" };

            // Act
            var result = InputEventExecutor.MatchesControllerAndDeviceName(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when ModuleSerial is null");
        }

        [TestMethod]
        public void MatchesControllerAndDeviceName_SerialDoesNotMatch_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem { ModuleSerial = "TestModule / SN-abc" };
            var e = new InputEventArgs { Serial = "SN-xyz" };

            // Act
            var result = InputEventExecutor.MatchesControllerAndDeviceName(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when serial doesn't match");
        }

        [TestMethod]
        public void MatchesControllerAndDeviceName_DeviceNameMatches_ReturnsTrue()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                ModuleSerial = "TestModule / SN-123",
                DeviceName = "Button1"
            };
            var e = new InputEventArgs 
            { 
                Serial = "SN-123",
                DeviceId = "Button1"
            };

            // Act
            var result = InputEventExecutor.MatchesControllerAndDeviceName(cfg, e);

            // Assert
            Assert.IsTrue(result, "Should return true when device name matches");
        }

        [TestMethod]
        public void MatchesControllerAndDeviceName_JoystickWithLabelMatch_ReturnsTrue()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                ModuleSerial = "JS-1 / SN-123", // Joystick serial
                DeviceName = "Button 1 Label"
            };
            var e = new InputEventArgs 
            { 
                Serial = "SN-123",
                DeviceId = "Button1",
                DeviceLabel = "Button 1 Label"
            };

            // Act
            var result = InputEventExecutor.MatchesControllerAndDeviceName(cfg, e);

            // Assert
            Assert.IsTrue(result, "Should return true for joystick with label match");
        }

        [TestMethod]
        public void MatchesControllerAndDeviceName_DeviceNameMismatchNonJoystick_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                ModuleSerial = "TestModule / SN-123",
                DeviceName = "Button1"
            };
            var e = new InputEventArgs 
            { 
                Serial = "SN-123",
                DeviceId = "Button2"
            };

            // Act
            var result = InputEventExecutor.MatchesControllerAndDeviceName(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when device names don't match");
        }

        #endregion

        #region ShouldSkipDueToInputShiftRegisterPinMismatch Tests

        [TestMethod]
        public void ShouldSkipDueToInputShiftRegisterPinMismatch_NotButtonEvent_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER,
                inputShiftRegister = new InputShiftRegisterConfig { ExtPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Encoder, // Not a button
                ExtPin = 3
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputShiftRegisterPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false for non-button events");
        }

        [TestMethod]
        public void ShouldSkipDueToInputShiftRegisterPinMismatch_NotInputShiftRegisterConfig_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_BUTTON, // Not shift register
                inputShiftRegister = new InputShiftRegisterConfig { ExtPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 3
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputShiftRegisterPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when device type is not InputShiftRegister");
        }

        [TestMethod]
        public void ShouldSkipDueToInputShiftRegisterPinMismatch_NullConfig_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER,
                inputShiftRegister = null // No config
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 3
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputShiftRegisterPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when inputShiftRegister config is null");
        }

        [TestMethod]
        public void ShouldSkipDueToInputShiftRegisterPinMismatch_PinMatches_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER,
                inputShiftRegister = new InputShiftRegisterConfig { ExtPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 5 // Pins match
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputShiftRegisterPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when pins match");
        }

        [TestMethod]
        public void ShouldSkipDueToInputShiftRegisterPinMismatch_PinMismatch_ReturnsTrue()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_SHIFT_REGISTER,
                inputShiftRegister = new InputShiftRegisterConfig { ExtPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 3 // Pins don't match
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputShiftRegisterPinMismatch(cfg, e);

            // Assert
            Assert.IsTrue(result, "Should return true when all conditions met and pins don't match");
        }

        #endregion

        #region ShouldSkipDueToInputMultiplexerPinMismatch Tests

        [TestMethod]
        public void ShouldSkipDueToInputMultiplexerPinMismatch_NotButtonEvent_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_MULTIPLEXER,
                inputMultiplexer = new InputMultiplexerConfig { DataPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Encoder, // Not a button
                ExtPin = 3
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputMultiplexerPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false for non-button events");
        }

        [TestMethod]
        public void ShouldSkipDueToInputMultiplexerPinMismatch_NotInputMultiplexerConfig_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_BUTTON, // Not multiplexer
                inputMultiplexer = new InputMultiplexerConfig { DataPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 3
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputMultiplexerPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when device type is not InputMultiplexer");
        }

        [TestMethod]
        public void ShouldSkipDueToInputMultiplexerPinMismatch_NullConfig_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_MULTIPLEXER,
                inputMultiplexer = null // No config
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 3
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputMultiplexerPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when inputMultiplexer config is null");
        }

        [TestMethod]
        public void ShouldSkipDueToInputMultiplexerPinMismatch_PinMatches_ReturnsFalse()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_MULTIPLEXER,
                inputMultiplexer = new InputMultiplexerConfig { DataPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 5 // Pins match
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputMultiplexerPinMismatch(cfg, e);

            // Assert
            Assert.IsFalse(result, "Should return false when pins match");
        }

        [TestMethod]
        public void ShouldSkipDueToInputMultiplexerPinMismatch_PinMismatch_ReturnsTrue()
        {
            // Arrange
            var cfg = new InputConfigItem 
            { 
                DeviceType = InputConfigItem.TYPE_INPUT_MULTIPLEXER,
                inputMultiplexer = new InputMultiplexerConfig { DataPin = 5 }
            };
            var e = new InputEventArgs 
            { 
                Type = DeviceType.Button,
                ExtPin = 3 // Pins don't match
            };

            // Act
            var result = InputEventExecutor.ShouldSkipDueToInputMultiplexerPinMismatch(cfg, e);

            // Assert
            Assert.IsTrue(result, "Should return true when all conditions met and pins don't match");
        }

        #endregion
    }
}