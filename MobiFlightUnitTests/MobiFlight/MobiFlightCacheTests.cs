using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlightUnitTests.Helpers;
using Moq;
using System.Collections.Generic;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class MobiFlightCacheTests
    {
        [TestMethod()]
        public void StartKeepAwake_CallsDeactivateConnectedModulePowerSave_Immediately()
        {
            // Arrange
            var cache = new TestableMobiFlightCache();
            var mockModule = new Mock<MobiFlightModule>("COM1", MobiFlightBoardTestHelper.CreateMinimalBoard());

            // Add a mock module to the cache
            cache.AddTestModule("SERIAL1", mockModule.Object);

            // Act
            cache.StartKeepAwake();

            // Assert
            mockModule.Verify(m => m.SetPowerSaveMode(false), Times.Once(),
                "SetPowerSaveMode(false) should be called immediately when StartKeepAwake is invoked");
        }

        [TestMethod()]
        public void StopKeepAwake_CallsActivateConnectedModulePowerSave()
        {
            // Arrange
            var cache = new TestableMobiFlightCache();
            var mockModule = new Mock<MobiFlightModule>("COM1", MobiFlightBoardTestHelper.CreateMinimalBoard());

            // Add a mock module to the cache
            cache.AddTestModule("SERIAL1", mockModule.Object);

            // Act
            cache.StopKeepAwake();

            // Assert
            mockModule.Verify(m => m.SetPowerSaveMode(true), Times.Once(),
                "SetPowerSaveMode(true) should be called when StopKeepAwake is invoked");
        }

        [TestMethod()]
        public void ModulePropertyChanged_RaisesModuleUpdatedEvent()
        {
            // Arrange
            var cache = new TestableMobiFlightCache();
            var module = MobiFlightBoardTestHelper.CreateTestModule();
            bool moduleUpdatedEventRaised = false;
            object moduleUpdatedSender = null;

            cache.AddTestModuleAsIfItWasDetected(module);

            cache.ControllerChanged += (sender, e) =>
            {
                moduleUpdatedEventRaised = true;
                moduleUpdatedSender = sender;
            };

            // Act - Change module name to trigger PropertyChanged
            module.Name = "Updated Name";

            // Assert
            Assert.IsTrue(moduleUpdatedEventRaised, "ModuleUpdated event should be raised when module property changes.");
            Assert.AreSame(module, moduleUpdatedSender, "Sender should be the module that changed.");
        }

        [TestMethod()]
        public void ModulePropertyChanged_Serial_RaisesModuleUpdatedEvent()
        {
            // Arrange
            var cache = new TestableMobiFlightCache();
            bool eventRaised = false;
            cache.ControllerChanged += (sender, e) => eventRaised = true;

            var module = MobiFlightBoardTestHelper.CreateTestModule();
            cache.AddTestModuleAsIfItWasDetected(module);

            // Act
            module.Serial = "SN-NEW-SERIAL";

            // Assert
            Assert.IsTrue(eventRaised, "ModuleUpdated event should be raised when Serial changes.");
        }

        [TestMethod()]
        public void ModulePropertyChanged_UnrelatedProperty_DoesNotRaiseModuleUpdatedEvent()
        {
            // Arrange
            var cache = new TestableMobiFlightCache();
            int eventCount = 0;
            cache.ControllerChanged += (sender, e) => eventCount++;

            var module =MobiFlightBoardTestHelper.CreateTestModule();
            cache.AddTestModuleAsIfItWasDetected(module);

            // Act - Change a property that shouldn't trigger ModuleUpdated
            module.HardwareId = "USB\\VID_XXXX";

            // Assert
            Assert.AreEqual(0, eventCount, "ModuleUpdated should not be raised for HardwareId changes.");
        }

        [TestMethod()]
        public void ModulePropertyChanged_SameValue_DoesNotRaiseModuleUpdatedEvent()
        {
            // Arrange
            var cache = new TestableMobiFlightCache();
            int eventCount = 0;
            cache.ControllerChanged += (sender, e) => eventCount++;
            
            var module = MobiFlightBoardTestHelper.CreateTestModule();
            module.Name = "ExistingName";

            cache.AddTestModuleAsIfItWasDetected(module);

            // Act
            module.Name = "ExistingName"; // Same value

            // Assert
            Assert.AreEqual(0, eventCount, "ModuleUpdated should not be raised when value hasn't changed.");
        }

    }
}