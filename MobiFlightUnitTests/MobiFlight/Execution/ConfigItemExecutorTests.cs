using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Execution;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.Modifier;
using MobiFlight.ProSim;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using Moq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass]
    public class ConfigItemExecutorTests
    {
        private Mock<ArcazeCache> mockArcazeCache;
        private Mock<FSUIPCCacheInterface> mockFsuipcCache;
        private Mock<SimConnectCacheInterface> mockSimConnectCache;
        private Mock<XplaneCacheInterface> mockXplaneCache;
        private Mock<MobiFlightCacheInterface> mockMobiFlightCache;
        private Mock<ProSimCacheInterface> mockProSimCache;
        private Mock<JoystickManager> mockJoystickManager;
        private Mock<MidiBoardManager> mockMidiBoardManager;
        private Mock<InputActionExecutionCache> mockInputActionExecutionCache;
        private List<IConfigItem> configItems;
        private OutputConfigItem configItemInTestMode;
        private ConfigItemExecutor executor;

        [TestInitialize]
        public void Setup()
        {
            mockArcazeCache = new Mock<ArcazeCache>();
            mockFsuipcCache = new Mock<FSUIPCCacheInterface>();
            mockSimConnectCache = new Mock<SimConnectCacheInterface>();
            mockXplaneCache = new Mock<XplaneCacheInterface>();
            mockMobiFlightCache = new Mock<MobiFlightCacheInterface>();
            mockProSimCache = new Mock<ProSimCacheInterface>();
            mockJoystickManager = new Mock<JoystickManager>();
            mockMidiBoardManager = new Mock<MidiBoardManager>();
            mockInputActionExecutionCache = new Mock<InputActionExecutionCache>();
            configItems = new List<IConfigItem>();
            configItemInTestMode = new OutputConfigItem();

            executor = new ConfigItemExecutor(
                configItems,
#if ARCAZE
                mockArcazeCache.Object,
#endif
                mockFsuipcCache.Object,
                mockSimConnectCache.Object,
                mockXplaneCache.Object,
                mockMobiFlightCache.Object,
                mockProSimCache.Object,
                mockJoystickManager.Object,
                mockMidiBoardManager.Object,
                mockInputActionExecutionCache.Object,
                configItemInTestMode
            );
        }

        [TestMethod]
        public void Execute_ShouldNotExecute_WhenConfigItemIsInactive()
        {
            // Arrange
            var cfg = new OutputConfigItem { Active = false };
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual(0, updatedValues.Count);
        }

        [TestMethod]
        public void Execute_ShouldUpdateStatus_WhenFsuipcNotConnected()
        {
            // Arrange
            var cfg = new OutputConfigItem { Active = true, Source = new FsuipcSource() };
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();
            mockFsuipcCache.Setup(c => c.IsConnected()).Returns(false);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual("FSUIPC_NOT_AVAILABLE", cfg.Status[ConfigItemStatusType.Source]);

            // Arrange
            mockFsuipcCache.Setup(c => c.IsConnected()).Returns(true);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            // verify that are status is cleared
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual(0, cfg.Status.Count);
        }

        [TestMethod]
        public void Execute_ShouldUpdateStatus_WhenSimConnectNotConnected()
        {
            // Arrange
            var cfg = new OutputConfigItem { Active = true, Source = new SimConnectSource() };
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();
            mockSimConnectCache.Setup(c => c.IsConnected()).Returns(false);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual("SIMCONNECT_NOT_AVAILABLE", cfg.Status[ConfigItemStatusType.Source]);

            // Arrange
            mockSimConnectCache.Setup(c => c.IsConnected()).Returns(true);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            // verify that are status is cleared
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual(0, cfg.Status.Count);
        }

        [TestMethod]
        public void Execute_ShouldUpdateStatus_WhenXplaneNotConnected()
        {
            // Arrange
            var cfg = new OutputConfigItem { Active = true, Source = new XplaneSource() };
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();
            mockXplaneCache.Setup(c => c.IsConnected()).Returns(false);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual("XPLANE_NOT_AVAILABLE", cfg.Status[ConfigItemStatusType.Source]);

            
            // Arrange
            mockXplaneCache.Setup(c => c.IsConnected()).Returns(true);
            
            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            // verify that are status is cleared
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual(0, cfg.Status.Count);
        }

        [TestMethod]
        public void Execute_ShouldUpdateStatus_WhenProSimNotConnected()
        {
            // Arrange
            var cfg = new OutputConfigItem { Active = true, Source = new ProSimSource() };
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();
            mockProSimCache.Setup(c => c.IsConnected()).Returns(false);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual("PROSIM_NOT_AVAILABLE", cfg.Status[ConfigItemStatusType.Source]);

            // Arrange
            mockProSimCache.Setup(c => c.IsConnected()).Returns(true);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            // verify that are status is cleared
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual(0, cfg.Status.Count);
        }

        [TestMethod]
        public void Execute_ShouldUpdateStatus_WhenModifierHasIssue()
        {
            // Arrange
            var variable = new MobiFlightVariable() { Number = 1, Text = "Test" };
            var cfg = new OutputConfigItem
            {
                Active = true,
                Source = new VariableSource()
                {
                    MobiFlightVariable = variable
                }
            };
            cfg.Modifiers.Items.Add(new Transformation() { Active = true, Expression = "$+_1" });
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();
            mockMobiFlightCache.Setup(m => m.GetMobiFlightVariable(It.IsAny<string>())).Returns(variable);

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual(1, updatedValues.Count);
            Assert.IsTrue(cfg.Status[ConfigItemStatusType.Modifier].Contains("error occurred on parsing your value formula"));

            cfg.Modifiers.Items.Clear();
            cfg.Modifiers.Items.Add(new Transformation() { Active = true, Expression = "$+1" });
            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual(0, cfg.Status.Count);
        }

        [TestMethod]
        public void ExecuteTestOn_ShouldExecuteDisplay_WhenDeviceTypeIsStepper()
        {
            // Arrange
            var cfg = new OutputConfigItem { ModuleSerial = "Test / SN-123", DeviceType = MobiFlightStepper.TYPE, Device = new OutputConfig.Stepper { TestValue = 100 } };

            // Act
            executor.ExecuteTestOn(cfg);

            // Assert
            mockMobiFlightCache.Verify(m => m.SetStepper(It.IsAny<string>(), It.IsAny<string>(), "100", It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<short>(), It.IsAny<short>()), Times.Once);
        }

        [TestMethod]
        public void ExecuteTestOff_ShouldExecuteDisplay_WhenDeviceTypeIsServo()
        {
            // Arrange
            var cfg = new OutputConfigItem { ModuleSerial = "Test / SN-123", DeviceType = MobiFlightServo.TYPE, Device = new OutputConfig.Servo { Min = "0", Address = "1", Max = "180", MaxRotationPercent = "100", Name = "TestServo" } };

            // Act
            executor.ExecuteTestOff(cfg);

            // Assert
            mockMobiFlightCache.Verify(m => m.SetServo(It.IsAny<string>(), It.IsAny<string>(), "0", 0, It.IsAny<int>(), It.IsAny<byte>()), Times.Once);
        }

        [TestMethod]
        public void Execute_ShouldHandleConfigPreconditionCorrectly_WhereConfigReferenceExists()
        {
            // Arrange
            var config = new OutputConfigItem { Active = true, Preconditions = new PreconditionList(), Value = "100" };
            configItems.Add(config);

            var precondition = new Precondition() { PreconditionType = "config", PreconditionRef = config.GUID, PreconditionValue = "90", PreconditionOperand = "=" };
            var preconditionList = new PreconditionList();
            preconditionList.Add(precondition);

            var cfg = new OutputConfigItem { Active = true, Preconditions = preconditionList };
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();

            // Act
            executor.Execute(cfg, updatedValues);

            // Assert
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual("not satisfied", cfg.Status[ConfigItemStatusType.Precondition]);
            precondition.PreconditionValue = "100";

            // Act
            updatedValues.Clear();
            executor.Execute(cfg, updatedValues);
            // Assert
            Assert.AreEqual(1, updatedValues.Count);
            Assert.AreEqual(false, cfg.Status.ContainsKey(ConfigItemStatusType.Precondition));
        }

        [TestMethod]
        public void Execute_ShouldHandleConfigPreconditionCorrectly_WhereConfigReferenceNotExists()
        {
            // Arrange
            var config = new OutputConfigItem { Active = true, Preconditions = new PreconditionList(), Value = "100" };
            configItems.Add(config);

            // create another config with different GUID
            config = new OutputConfigItem { Active = true, Preconditions = new PreconditionList(), Value = "100" };

            var precondition = new Precondition() { PreconditionType = "config", PreconditionRef = config.GUID, PreconditionValue = "90", PreconditionOperand = "=" };
            var preconditionList = new PreconditionList();
            preconditionList.Add(precondition);

            var cfg = new OutputConfigItem { Active = true, Preconditions = preconditionList };
            var updatedValues = new ConcurrentDictionary<string, IConfigItem>();

            // Act
            try
            {
                executor.Execute(cfg, updatedValues);
            }
            catch (Exception e)
            {
                Assert.AreEqual("Config reference not found", e.Message);
            }
        }
    }
}
