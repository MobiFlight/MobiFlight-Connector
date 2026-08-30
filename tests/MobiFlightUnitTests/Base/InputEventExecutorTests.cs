using MobiFlight.Base;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.ProSim;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using Moq;

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
                Name = "TestConfig",
                button = new ButtonInputConfig
                {
                    onRelease = new VariableInputAction()
                    {
                        Variable = new MobiFlightVariable()
                        {
                            Name = "TestVariable",
                            Number = 100
                        }
                    }
                }
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
        public void Execute_LongReleaseWithoutOwnAction_LogsAndDisplaysAsRelease()
        {
            // A config with only onRelease (no onLongRelease) that receives a raw LONG_RELEASE must
            // log and display RELEASE - what actually ran - not the raw LONG_RELEASE it fell back from.
            var inputEventArgs = new InputEventArgs
            {
                Controller = new Controller() { Serial = "123" },
                InputType = DeviceType.Button,
                Device = new DeviceReference() { Name = "Device1" },
                Value = (int)MobiFlightButton.InputEvent.LONG_RELEASE
            };

            var activeConfigItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController("/ 123"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, "Device1"),
                Name = "TestConfig",
                button = new ButtonInputConfig
                {
                    onRelease = new VariableInputAction()
                    {
                        Variable = new MobiFlightVariable() { Name = "TestVariable", Number = 100 }
                    }
                }
            };

            _configItems.Add(activeConfigItem);

            var result = _executor.Execute(inputEventArgs, isStarted: true);

            Assert.HasCount(1, result);
            Assert.AreEqual("RELEASE", activeConfigItem.RawValue);

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

        #region HOLD events are gated exactly like real events

        // HOLD is now an ordinary InputEventArgs into Execute() - same Active/precondition gating.

        private static InputEventArgs CreateHoldEventArgs(string serial, string deviceId)
        {
            return new InputEventArgs
            {
                Controller = new Controller() { Serial = serial },
                InputType = DeviceType.Button,
                Device = new DeviceReference() { Name = deviceId },
                Value = (int)MobiFlightButton.InputEvent.HOLD
            };
        }

        [TestMethod]
        public void Execute_HoldEvent_DeactivatedConfig_IsSkipped()
        {
            var serial = "SN-deact001";
            var deviceName = "Button1";

            var configItem = new InputConfigItem
            {
                Active = false,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "DeactivatedConfig",
                button = new ButtonInputConfig
                {
                    onHold = new MSFS2020CustomInputAction { Command = "(>K:HoldCommand)", PresetId = "p1" }
                }
            };
            _configItems.Add(configItem);

            var result = _executor.Execute(CreateHoldEventArgs(serial, deviceName), isStarted: true);

            Assert.IsFalse(result.ContainsKey(configItem.GUID), "A HOLD event for a deactivated config should not execute.");
        }

        [TestMethod]
        public void Execute_HoldEvent_PreconditionNotSatisfied_IsSkipped()
        {
            var serial = "SN-precond001";
            var deviceName = "Button1";

            _configItems[0].Value = "DifferentValue";

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "PreconditionConfig",
                button = new ButtonInputConfig
                {
                    onHold = new MSFS2020CustomInputAction { Command = "(>K:HoldCommand)", PresetId = "p2" }
                },
                Preconditions = new PreconditionList
                {
                    new Precondition { Type = "config", Active = true, Ref = _configItems[0].GUID, Value = "ExpectedValue" }
                }
            };
            _configItems.Add(configItem);

            var result = _executor.Execute(CreateHoldEventArgs(serial, deviceName), isStarted: true);

            Assert.IsFalse(result.ContainsKey(configItem.GUID), "A HOLD event should not execute while its precondition is not satisfied.");
        }

        [TestMethod]
        public void Execute_HoldEvent_ActiveConfigWithSatisfiedPrecondition_Executes()
        {
            var serial = "SN-active001";
            var deviceName = "Button1";

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "ActiveConfig",
                button = new ButtonInputConfig
                {
                    onHold = new MSFS2020CustomInputAction { Command = "(>K:HoldCommand)", PresetId = "p3" }
                }
            };
            _configItems.Add(configItem);

            var result = _executor.Execute(CreateHoldEventArgs(serial, deviceName), isStarted: true);

            Assert.IsTrue(result.ContainsKey(configItem.GUID), "A HOLD event for an active, precondition-satisfied config should execute.");
        }

        #endregion

        #region ResolveButtonTimingsPerConfig - distinct delay settings among bound configs

        [TestMethod]
        public void ResolveButtonTimingsPerConfig_ActiveConfig_ReturnsItsDelays()
        {
            var serial = "SN-timings001";
            var deviceName = "Button1";

            var configItem = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "ModeAConfig",
                button = new ButtonInputConfig
                {
                    onHold = new MSFS2020CustomInputAction { Command = "(>K:HoldCommand)", PresetId = "p1" },
                    HoldDelay = 123,
                    RepeatDelay = 45, // below ButtonTimings.MinRepeatDelay - not enforced here, that's a config-authoring-time (UI) concern
                    LongReleaseDelay = 678
                }
            };
            _configItems.Add(configItem);

            var timings = _executor.ResolveButtonTimingsPerConfig(CreateButtonEventArgs(serial, deviceName, isOnPress: true));

            Assert.HasCount(1, timings);
            Assert.AreEqual(123, timings[0].HoldDelay);
            Assert.AreEqual(45, timings[0].RepeatDelay, "No runtime clamping - the config's own value is used as-is.");
            Assert.AreEqual(678, timings[0].LongReleaseDelay);
        }

        [TestMethod]
        public void ResolveButtonTimingsPerConfig_NoActiveConfigForButton_ReturnsEmpty()
        {
            var result = _executor.ResolveButtonTimingsPerConfig(CreateButtonEventArgs("SN-unbound", "NoSuchButton", isOnPress: true));

            Assert.HasCount(0, result);
        }

        [TestMethod]
        public void ResolveButtonTimingsPerConfig_InactiveConfig_IsStillReturned_ButExecuteSkipsIt()
        {
            // Active/Precondition gating happens exclusively in Execute() now (see
            // Execute_MatchingInactiveConfigItem_SkipsExecution) - resolving a binding only answers
            // "is a config bound to this button," not "should it currently run."
            var serial = "SN-timings002";
            var deviceName = "Button1";

            var configItem = new InputConfigItem
            {
                Active = false,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "InactiveConfig",
                button = new ButtonInputConfig { HoldDelay = 999 }
            };
            _configItems.Add(configItem);

            var result = _executor.ResolveButtonTimingsPerConfig(CreateButtonEventArgs(serial, deviceName, isOnPress: true));

            Assert.HasCount(1, result, "An inactive config is still bound to the button and must still get a timing binding.");
            Assert.AreEqual(999, result[0].HoldDelay);
        }

        [TestMethod]
        public void ResolveButtonTimingsPerConfig_MultiModePanel_BothModesAlwaysReturned_RegardlessOfCurrentMode()
        {
            // Precondition gating no longer happens at this layer - every mode's config is bound to
            // the button, always, each with its own HoldDelay. Execute() decides which one (if any)
            // actually runs, using live precondition state at each fire.
            var serial = "SN-multimode";
            var deviceName = "Button1";

            var modeSwitch = _configItems[0]; // an existing InputConfigItem used as the mode reference
            modeSwitch.Value = "ModeA";

            var modeAConfig = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "ModeAConfig",
                button = new ButtonInputConfig { HoldDelay = 200 },
                Preconditions = new PreconditionList
                {
                    new Precondition { Type = "config", Active = true, Ref = modeSwitch.GUID, Value = "ModeA" }
                }
            };
            var modeBConfig = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "ModeBConfig",
                button = new ButtonInputConfig { HoldDelay = 800 },
                Preconditions = new PreconditionList
                {
                    new Precondition { Type = "config", Active = true, Ref = modeSwitch.GUID, Value = "ModeB" }
                }
            };
            _configItems.Add(modeAConfig);
            _configItems.Add(modeBConfig);

            var timingsInModeA = _executor.ResolveButtonTimingsPerConfig(CreateButtonEventArgs(serial, deviceName, isOnPress: true));
            Assert.HasCount(2, timingsInModeA, "Both configs are bound to this button regardless of which mode is currently selected.");
            Assert.IsTrue(timingsInModeA.Any(t => t.HoldDelay == 200));
            Assert.IsTrue(timingsInModeA.Any(t => t.HoldDelay == 800));

            modeSwitch.Value = "ModeB"; // resolving timings no longer depends on this at all

            var timingsInModeB = _executor.ResolveButtonTimingsPerConfig(CreateButtonEventArgs(serial, deviceName, isOnPress: true));
            Assert.HasCount(2, timingsInModeB, "Switching modes doesn't change which configs are bound - only Execute() cares about the current mode.");
        }

        [TestMethod]
        public void ResolveButtonTimingsPerConfig_TwoSimultaneouslyActiveConfigs_ReturnsBoth()
        {
            var serial = "SN-conflict001";
            var deviceName = "Button1";

            var firstConfig = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "FirstConfig",
                button = new ButtonInputConfig { HoldDelay = 100 }
            };
            var secondConfig = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "SecondConfig",
                button = new ButtonInputConfig { HoldDelay = 900 }
            };
            _configItems.Add(firstConfig);
            _configItems.Add(secondConfig);

            var timings = _executor.ResolveButtonTimingsPerConfig(CreateButtonEventArgs(serial, deviceName, isOnPress: true));

            Assert.HasCount(2, timings);
            Assert.IsTrue(timings.Any(t => t.HoldDelay == 100));
            Assert.IsTrue(timings.Any(t => t.HoldDelay == 900));
        }

        [TestMethod]
        public void ResolveButtonTimingsPerConfig_TwoConfigsWithIdenticalSettings_ReturnsOneDistinctEntry()
        {
            var serial = "SN-identical001";
            var deviceName = "Button1";

            var firstConfig = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "FirstConfig",
                button = new ButtonInputConfig { HoldDelay = 350, RepeatDelay = 0, LongReleaseDelay = 350 }
            };
            var secondConfig = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "SecondConfig",
                button = new ButtonInputConfig { HoldDelay = 350, RepeatDelay = 0, LongReleaseDelay = 350 }
            };
            _configItems.Add(firstConfig);
            _configItems.Add(secondConfig);

            var timings = _executor.ResolveButtonTimingsPerConfig(CreateButtonEventArgs(serial, deviceName, isOnPress: true));

            Assert.HasCount(1, timings, "Two configs with identical settings collapse to one distinct entry - no config identity is carried here.");
        }

        #endregion

        #region MatchesSyntheticDelay - a HOLD/REPEAT/LONG_RELEASE only applies to a config whose own delay produced it

        [TestMethod]
        public void Execute_HoldEvent_OnlyExecutesTheConfigWhoseOwnDelayMatches()
        {
            var serial = "SN-target001";
            var deviceName = "Button1";

            var matching = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "MatchingConfig",
                button = new ButtonInputConfig { HoldDelay = 300, onHold = new MSFS2020CustomInputAction { Command = "(>K:Cmd1)", PresetId = "p1" } }
            };
            var other = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "OtherConfig",
                button = new ButtonInputConfig { HoldDelay = 900, onHold = new MSFS2020CustomInputAction { Command = "(>K:Cmd2)", PresetId = "p2" } }
            };
            _configItems.Add(matching);
            _configItems.Add(other);

            var holdEvent = CreateHoldEventArgs(serial, deviceName);
            holdEvent.SyntheticDelayMs = 300; // matches only "matching"'s own HoldDelay

            var result = _executor.Execute(holdEvent, isStarted: true);

            Assert.IsTrue(result.ContainsKey(matching.GUID), "The config whose own HoldDelay matches must execute.");
            Assert.IsFalse(result.ContainsKey(other.GUID), "A config whose own HoldDelay doesn't match must not execute.");
        }

        [TestMethod]
        public void Execute_EventWithNoSyntheticDelay_ExecutesEveryMatchingActiveConfig()
        {
            var serial = "SN-broadcast001";
            var deviceName = "Button1";

            var first = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "First",
                button = new ButtonInputConfig { HoldDelay = 300, onHold = new MSFS2020CustomInputAction { Command = "(>K:Cmd1)", PresetId = "p1" } }
            };
            var second = new InputConfigItem
            {
                Active = true,
                Controller = SerialNumber.CreateController($"TestModule / {serial}"),
                Device = InputConfigItem.CreateInputDevice(InputConfigItem.TYPE_BUTTON, deviceName),
                Name = "Second",
                button = new ButtonInputConfig { HoldDelay = 900, onHold = new MSFS2020CustomInputAction { Command = "(>K:Cmd2)", PresetId = "p2" } }
            };
            _configItems.Add(first);
            _configItems.Add(second);

            // SyntheticDelayMs left null, e.g. a real PRESS/RELEASE - nothing to match, so it broadcasts to both.
            var result = _executor.Execute(CreateHoldEventArgs(serial, deviceName), isStarted: true);

            Assert.IsTrue(result.ContainsKey(first.GUID));
            Assert.IsTrue(result.ContainsKey(second.GUID));
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
                Value = (int)MobiFlightButton.InputEvent.PRESS
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