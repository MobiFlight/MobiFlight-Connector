using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

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
            var mockModule = new Mock<MobiFlightModule>("COM1", CreateMinimalBoard());

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
            var mockModule = new Mock<MobiFlightModule>("COM1", CreateMinimalBoard());

            // Add a mock module to the cache
            cache.AddTestModule("SERIAL1", mockModule.Object);

            // Act
            cache.StopKeepAwake();

            // Assert
            mockModule.Verify(m => m.SetPowerSaveMode(true), Times.Once(),
                "SetPowerSaveMode(true) should be called when StopKeepAwake is invoked");
        }

        [TestMethod]
        public void UpdateConnectedModuleName_UpdatesModuleNameInCache()
        {
            // Arrange
            var cache = new TestableMobiFlightCache();
            cache.GetAvailableComModules().Add(new MobiFlightModuleInfo { Serial = "SERIAL1", Name = "OldName" });

            // Use a REAL module instance - no mocking needed!
            var cachedModule = new MobiFlightModule("COM1", CreateMinimalBoard());
            cachedModule.Serial = "SERIAL1";
            cachedModule.Version = "1.0.0"; // So HasMfFirmware() returns true
            cachedModule.Name = "OldName";
            cache.AddTestModule("SERIAL1", cachedModule);

            // Create another REAL module with the new name
            var updatedModule = new MobiFlightModule("COM2", CreateMinimalBoard());
            updatedModule.Serial = "SERIAL1";
            updatedModule.Version = "1.0.0"; // So HasMfFirmware() returns true
            updatedModule.Name = "NewName";

            // Act - call with the updated module
            cache.UpdateConnectedModuleName(updatedModule);

            // Assert - verify BOTH instances were updated by the method
            var availableComModule = cache.GetAvailableComModules().First();
            Assert.IsNotNull(availableComModule);
            Assert.AreEqual("NewName", availableComModule.Name, "MobiFlightModuleInfo.Name should be updated to 'NewName' in AvailableComModules");
            Assert.AreEqual("SERIAL1", availableComModule.Serial, "Serial should remain unchanged in AvailableComModules");

            // The cached module should now have the new name
            Assert.AreEqual("NewName", cachedModule.Name, "The cached module's Name property should be updated by UpdateConnectedModuleName");
        }

        // Testable subclass to expose internal members for testing
        private class TestableMobiFlightCache : MobiFlightCache
        {
            public void AddTestModule(string serial, MobiFlightModule module)
            {
                // Use reflection to access the private Modules dictionary
                var modulesField = typeof(MobiFlightCache).GetField("Modules",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var modules = modulesField.GetValue(this) as System.Collections.Concurrent.ConcurrentDictionary<string, MobiFlightModule>;
                modules.TryAdd(serial, module);
            }

            public List<MobiFlightModuleInfo> GetAvailableComModules()
            {
                var availableComModulesField = typeof(MobiFlightCache).GetField("AvailableComModules",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return availableComModulesField.GetValue(this) as List<MobiFlightModuleInfo>;
            }
        }
        private static Board CreateMinimalBoard()
        {
            return new Board
            {
                Info = new Info
                {
                    MobiFlightType = "TestType",
                    FriendlyName = "TestBoard",
                    FirmwareBaseName = "test",
                    FirmwareExtension = "hex"
                },
                Connection = new Connection
                {
                    ConnectionDelay = 0,
                    TimeoutForFirmwareUpdate = 15000
                },
                AvrDudeSettings = new AvrDudeSettings
                {
                    Timeout = 15000
                },
                HardwareIds = new List<string>(),
                ModuleLimits = new ModuleLimits(),
                Pins = new List<MobiFlightPin>(),
                UsbDriveSettings = new UsbDriveSettings()
            };
        }
    }
}