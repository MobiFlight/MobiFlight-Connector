using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class FirmwareUpdateSettings
    {
        /// <summary>
        /// AvrDude device type for the device.
        /// </summary>
        public String Device;
        /// <summary>
        /// Baud rate to use with AvrDude
        /// </summary>
        public String BaudRate;
        /// <summary>
        /// AvrDude programmer to use for the device.
        /// </summary>
        public String Programmer;
        /// <summary>
        /// Base name for firmware files. The final filename is of the form {FirmwareBaseName}_{Version}.hex.
        /// </summary>
        public String FirmwareBaseName;
    }

    public class Board
    {
        /// <summary>
        /// Maximum number of outputs supported by the board.
        /// </summary>
        public int MaxOutputs = 0;
        /// <summary>
        /// Maximum number of buttons supported by the board.
        /// </summary>
        public int MaxButtons = 0;
        /// <summary>
        /// Maximum number of LED segments supported by the board.
        /// </summary>
        public int MaxLedSegments = 0;
        /// <summary>
        /// Maximum number of encoders supported by the board.
        /// </summary>
        public int MaxEncoders = 0;
        /// <summary>
        /// Maximum number of steppers supported by the board.
        /// </summary>
        public int MaxSteppers = 0;
        /// <summary>
        /// Maximum number of servos supported by the board.
        /// </summary>
        public int MaxServos = 0;
        /// <summary>
        /// Maximum number of I2C LCDs supported by the board.
        /// </summary>
        public int MaxLcdI2C = 0;
        /// <summary>
        /// Maximum number of analog inputs supported by the board.
        /// </summary>
        public int MaxAnalogInputs = 0;

        /// <summary>
        /// True if multiple attempts should be made when connecting to the board.
        /// </summary>
        public Boolean ExtraConnectionRetry;
        /// <summary>
        /// True if the board supports loading firmware via MobiFlight.
        /// </summary>
        public Boolean CanInstallFirmware;
        /// <summary>
        /// Number of milliseconds to wait before loading the configuration after initially connecting to the board.
        /// </summary>
        public int ConnectionDelay;
        /// <summary>
        /// True if DTR should be enabled when connecting to the board over serial.
        /// </summary>
        public Boolean DtrEnable;
        /// <summary>
        /// The latest supported version of the firmware.
        /// </summary>
        public String LatestFirmwareVersion;
        /// <summary>
        /// The USB friendly name for the board as specified by the board manufacturer.
        /// </summary>
        public String FriendlyName;
        /// <summary>
        /// The type of the board as provided by the MobiFlight firmware.
        /// </summary>
        public String MobiFlightType;
        /// <summary>
        /// A list of regular expressions of USB hardware IDs that use this board definition.
        /// </summary>
        public List<String> HardwareIds;
        /// <summary>
        /// Maximum size of a CmdMessenger message, in bytes.
        /// </summary>
        public int MessageSize;
        /// <summary>
        /// Maximum size of EEPROM storage, in bytes.
        /// </summary>
        public int EEPROMSize;

        /// <summary>
        /// Settings related to updating the firmware.
        /// </summary>
        public FirmwareUpdateSettings FirmwareUpdateSettings;

        public List<MobiFlightPin> Pins;

        public string GetFirmwareName()
        {
            return $"{FirmwareUpdateSettings.FirmwareBaseName}_{LatestFirmwareVersion.Replace('.', '_')}.hex";
        }
        public override string ToString()
        {
            return $"{FriendlyName}";
        }
    }
}
