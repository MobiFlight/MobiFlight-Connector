using MobiFlight;
using MobiFlight.Config;
using System.Collections.Generic;

namespace MobiFlightUnitTests.Helpers
{
    public static class MobiFlightBoardTestHelper
    {
        /// <summary>
        /// Creates a minimal Board instance suitable for testing.
        /// </summary>
        /// <param name="mobiFlightType">The MobiFlight type identifier</param>
        /// <param name="friendlyName">The friendly name for the board</param>
        /// <returns>A configured Board instance</returns>
        public static Board CreateMinimalBoard(
            string mobiFlightType = "TestType",
            string friendlyName = "TestBoard")
        {
            return new Board
            {
                Info = new Info
                {
                    MobiFlightType = mobiFlightType,
                    FriendlyName = friendlyName,
                    FirmwareBaseName = "test",
                    FirmwareExtension = "hex",
                    CanInstallFirmware = true
                },
                Connection = new Connection
                {
                    ConnectionDelay = 0,
                    TimeoutForFirmwareUpdate = 15000,
                    DtrEnable = false,
                    ExtraConnectionRetry = false,
                    ForceResetOnFirmwareUpdate = false,
                    MessageSize = 512
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

        /// <summary>
        /// Creates a test MobiFlightModule instance with default values.
        /// </summary>
        /// <param name="port">COM port name</param>
        /// <param name="serial">Module serial number</param>
        /// <param name="name">Module name</param>
        /// <param name="version">Firmware version</param>
        /// <param name="coreVersion">Core version</param>
        /// <returns>A configured MobiFlightModule instance</returns>
        public static MobiFlightModule CreateTestModule(
            string port = "COM3",
            string serial = "SN-TEST-123",
            string name = "TestModule",
            string version = "1.0.0",
            string coreVersion = "1.0.0")
        {
            var board = CreateMinimalBoard("MegaMini", "Arduino Mega 2560");
            var module = new MobiFlightModule(port, board)
            {
                Serial = serial,
                Version = version,
                CoreVersion = coreVersion,
                Name = name,
                // Create a dummy config to prevent serial communication
                Config = new Config(),
            };

            // set connected to true
            // since it is a private setter, we need to use reflection to set it for testing purposes
            typeof(MobiFlightModule).GetProperty("Connected").SetValue(module, true);

            return module;
        }
    }
}
