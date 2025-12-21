using Microsoft.VisualStudio.TestTools.UnitTesting;
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