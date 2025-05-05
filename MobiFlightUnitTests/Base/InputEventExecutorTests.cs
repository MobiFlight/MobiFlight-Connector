using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
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
        private Mock<JoystickManager> _mockJoystickManager;
        private Mock<ArcazeCache> _mockArcazeCache;
        private List<IConfigItem> _configItems;
        private InputEventExecutor _executor;
        private Mock<ILogAppender> _mockLogAppender;

        [TestInitialize]
        public void SetUp()
        {
            _mockInputActionExecutionCache = new Mock<InputActionExecutionCache>();
            _mockFsuipcCache = new Mock<Fsuipc2Cache>();
            _mockSimConnectCache = new Mock<SimConnectCacheInterface>();
            _mockXplaneCache = new Mock<XplaneCache>();
            _mockMobiFlightCache = new Mock<MobiFlightCache>();
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
                _mockJoystickManager.Object,
                _mockArcazeCache.Object
            );

            // Create a mock log appender
            _mockLogAppender = new Mock<ILogAppender>();
            Log.Instance.Enabled = true; // Enable logging
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
            Assert.AreEqual(0, result.Count);

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"No config found.")), LogSeverity.Warn),
                Times.Once
            );
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
            Assert.AreEqual(0, result.Count);

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
            Assert.AreEqual(1, result.Count);
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

            // Create a simple button event
            InputEventArgs inputEventArgs = CreateButtonEventArgs("123", buttonId, true);

            var activeConfigItem = CreateInputConfigItemWithButton(
                name: "TestConfig",
                moduleSerial: "testcontroller / 123",
                deviceName: buttonId,
                active: true,
                command: "(>K:TestCommand:#)"
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
            Assert.AreEqual(1, result.Count, "Only one item should be executed.");
            Assert.IsTrue(result.ContainsKey(activeConfigItem.GUID), "The wrong config item was executed.");

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains($@"Executing ""{activeConfigItem.Name}"". (PRESS)")), LogSeverity.Info),
                Times.Once,
                "The config item should be executed with an OnPress event."
            );

            _mockSimConnectCache.Verify(
                cache => cache.SetSimVar(It.Is<string>(str => str == "(A:TestCommand:FinalValue)")),
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
                        PreconditionType = "variable",
                        PreconditionActive = true,
                        PreconditionRef = "TestRef",
                        PreconditionValue = "OtherValue"
                    }
                }
            };

            _configItems.Add(configItem);

            // Act
            var result = _executor.Execute(inputEventArgs, isStarted: true);

            // Assert
            Assert.AreEqual(0, result.Count);

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
            Assert.AreEqual(0, result.Count);

            _mockLogAppender.Verify(
                appender => appender.log(It.Is<string>(msg => msg.Contains("skipping, MobiFlight not running.")), LogSeverity.Warn),
                Times.Once
            );
        }
    }
}