using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    /// <summary>
    /// Settings for flashing Arduino devices with avrdude.
    /// </summary>
    public class AvrDudeSettings
    {
        /// <summary>
        /// Baud rate to use with AvrDude.
        /// </summary>
        public String BaudRate;

        /// <summary>
        /// AvrDude device type for the device.
        /// </summary>
        public String Device;

        /// <summary>
        /// Base name for firmware files. The final filename is of the form {FirmwareBaseName}_{Version}.hex.
        /// </summary>
        public String FirmwareBaseName;

        /// <summary>
        /// AvrDude programmer to use for the device.
        /// </summary>
        public String Programmer;

        /// <summary>
        /// Provides the name of the firmware file for a given firmware version.
        /// </summary>
        /// <param name="latestFirmwareVersion">The version of the firmware, for example "1.14.0".</param>
        /// <returns>The firmware file name using FirmwareBaseName and the specified firmware version.</returns>
        public string GetFirmwareName(string latestFirmwareVersion)
        {
            return $"{FirmwareBaseName}_{latestFirmwareVersion.Replace('.', '_')}.hex";
        }
    }

    /// <summary>
    /// Connection-related settings for the board.
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// Number of milliseconds to wait before loading the configuration after initially connecting to the board.
        /// </summary>
        public int ConnectionDelay;

        /// <summary>
        /// Number of milliseconds to wait after a firmware update before attempting to reconnect to the board.
        /// </summary>
        public int DelayAfterFirmwareUpdate = 0;

        /// <summary>
        /// True if DTR should be enabled when connecting to the board over serial.
        /// </summary>
        public Boolean DtrEnable;

        /// <summary>
        /// Maximum size of EEPROM storage, in bytes.
        /// </summary>
        public int EEPROMSize;

        /// <summary>
        /// True if multiple attempts should be made when connecting to the board.
        /// </summary>
        public Boolean ExtraConnectionRetry;

        /// <summary>
        /// True if a force reset should be done to the board before attempting to upload the firmware.
        /// </summary>
        public Boolean ForceResetOnFirmwareUpdate;

        /// <summary>
        /// Maximum size of a CmdMessenger message, in bytes.
        /// </summary>
        public int MessageSize;
    }

    /// <summary>
    /// General board information properties.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// True if the board supports loading firmware via MobiFlight.
        /// </summary>
        public Boolean CanInstallFirmware;

        /// <summary>
        /// The USB friendly name for the board as specified by the board manufacturer.
        /// </summary>
        public String FriendlyName;

        /// <summary>
        /// The latest supported version of the firmware.
        /// </summary>
        public String LatestFirmwareVersion;

        /// <summary>
        /// The type of the board as provided by the MobiFlight firmware.
        /// </summary>
        public String MobiFlightType;
    }

    /// <summary>
    /// Maximum number of each type of module supported by the board.
    /// </summary>
    public class ModuleLimits
    {
        /// <summary>
        /// Maximum number of analog inputs supported by the board.
        /// </summary>
        public int MaxAnalogInputs = 0;

        /// <summary>
        /// Maximum number of buttons supported by the board.
        /// </summary>
        public int MaxButtons = 0;

        /// <summary>
        /// Maximum number of encoders supported by the board.
        /// </summary>
        public int MaxEncoders = 0;

        /// <summary>
        /// Maximum number of I2C LCDs supported by the board.
        /// </summary>
        public int MaxLcdI2C = 0;

        /// <summary>
        /// Maximum number of LED segments supported by the board.
        /// </summary>
        public int MaxLedSegments = 0;

        /// <summary>
        /// Maximum number of outputs supported by the board.
        /// </summary>
        public int MaxOutputs = 0;

        /// <summary>
        /// Maximum number of servos supported by the board.
        /// </summary>
        public int MaxServos = 0;

        /// <summary>
        /// Maximum number of steppers supported by the board.
        /// </summary>
        public int MaxSteppers = 0;
    }

    public class Board
    {
        /// <summary>
        /// Settings related to updating the firmware via AvrDude.
        /// </summary>
        public AvrDudeSettings AvrDudeSettings;

        /// <summary>
        /// Connection settings for the board.
        /// </summary>
        public Connection Connection;

        /// <summary>
        /// A list of regular expressions of USB hardware IDs that use this board definition.
        /// </summary>
        public List<String> HardwareIds;

        /// <summary>
        ///  General board information.
        /// </summary>
        public Info Info;

        /// <summary>
        /// Module limits for the board.
        /// </summary>
        public ModuleLimits ModuleLimits;

        /// <summary>
        /// List of pins supported by the board.
        /// </summary>
        public List<MobiFlightPin> Pins;

        public override string ToString()
        {
            return $"{Info.MobiFlightType} ({Info.FriendlyName})";
        }
    }
}
