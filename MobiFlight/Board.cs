using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MobiFlight
{
    public enum BoardPartnerLevel
    {
        Core,
        Partner,
        Community
    }

    /// <summary>
    /// Additional community information
    /// </summary>
    public class Community
    {
        /// <summary>
        /// The project's name
        /// </summary>
        public String Project;

        /// <summary>
        /// The project's website
        /// </summary>
        public String Website;

        /// <summary>
        /// The project's documentation
        /// </summary>
        public String Docs;

        /// <summary>
        /// The project's support, e.g. Discord Server link
        /// </summary>
        public String Support;
    }

    /// <summary>
    /// Settings for flashing Arduino devices with avrdude.
    /// </summary>
    public class AvrDudeSettings
    {
        /// <summary>
        /// Number of times AvrDude should retry connecting to the device. If null then -x attempts will not be passed to avrdude.
        /// </summary>
        public int? Attempts;

        /// <summary>
        /// Baud rate to use with AvrDude. Deprecated. Use BaudRates instead.
        /// </summary>
        [Obsolete]
        public String BaudRate;

        /// <summary>
        /// All supported baud rates to use with AvrDude.
        /// </summary>
        public List<String> BaudRates;

        /// <summary>
        /// AvrDude device type for the device.
        /// </summary>
        public String Device;

        /// <summary>
        /// Base name for firmware files. Deprecated. Use Info.FirmwareBaseName instead.
        /// </summary>
        [Obsolete]
        public String FirmwareBaseName;

        /// <summary>
        /// AvrDude programmer to use for the device.
        /// </summary>
        public String Programmer;

        /// <summary>
        /// Number of milliseconds to wait before we assume the AvrDude process failed and has to get killed.
        /// </summary>
        public int Timeout;

        /// <summary>
        /// Firmware filename to reset the board. No longer used. Use Info.ResetFirmwareFile instead.
        /// </summary>
        [Obsolete]
        public String ResetFirmwareFile;
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

        /// <summary>
        /// Number of milliseconds to wait for the firmware update to complete before attempting to call GetInfo on the board.
        /// </summary>
        public int TimeoutForFirmwareUpdate = 15000;
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
        /// True if the board can be reset to factory default by MobiFlight.
        /// </summary>
        public Boolean CanResetBoard;

        /// <summary>
        /// Base name for firmware files. The final filename is of the form {FirmwareBaseName}_{Version}.{FirmwareExtension}.
        /// </summary>
        public String FirmwareBaseName;

        /// <summary>
        /// File extension for firmware files. The final filename is of the form {FirmwareBaseName}_{Version}.{FirmwareExtension}.
        /// </summary>
        public String FirmwareExtension;

        /// <summary>
        /// The USB friendly name for the board as specified by the board manufacturer.
        /// This is only used for initial type detecting when no MF firmware is present.
        /// This doesn't really make sense for Custom boards.
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

        /// <summary>
        /// This Label is used instead of the internal MobiFlightType
        /// </summary>
        public String MobiFlightTypeLabel;

        /// <summary>
        /// Firmware filename to reset the board.
        /// </summary>
        public String ResetFirmwareFile;

        /// <summary>
        /// A list of supported custom device types by this board.
        /// </summary>
        public List<String> CustomDeviceTypes = new List<string>();

        /// <summary>
        /// Additional community information
        /// </summary>
        public Community Community;

        /// <summary>
        /// The image resource
        /// </summary>
        public Image BoardIcon;

        /// <summary>
        /// The image resource
        /// </summary>
        public Image BoardPicture;

        /// <summary>
        /// Provides the name of the firmware file for a given firmware version.
        /// </summary>
        /// <param name="latestFirmwareVersion">The version of the firmware, for example "1.14.0".</param>
        /// <returns>The firmware file name using FirmwareBaseName and the specified firmware version.</returns>
        public string GetFirmwareName(string latestFirmwareVersion)
        {
            return $"{FirmwareBaseName}_{latestFirmwareVersion.Replace('.', '_')}.{FirmwareExtension.TrimStart('.')}";
        }
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
        /// Maximum number of input shifters supported by the board.
        /// </summary>
        public int MaxInputShifters = 0;

        /// <summary>
        /// Maximum number of Mux digital input blocks supported by the board.
        /// </summary>
        public int MaxInputMultiplexer = 0;

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
        /// Maximum number of output shifters supported by the board.
        /// </summary>
        public int MaxShifters = 0;

        /// <summary>
        /// Maximum number of steppers supported by the board.
        /// </summary>
        public int MaxSteppers = 0;

        /// <summary>
        /// Maximum number of custom devices supported by the board.
        /// </summary>
        public int MaxCustomDevices = 0;
    }

    public class UsbDriveSettings
    {
        /// <summary>
        /// The name of a file that must be present in the root directory of the USB drive for it to be considered
        /// a match and able to be flashed.
        /// </summary>
        public string VerificationFileName;

        /// <summary>
        /// Volume label of the USB drive when mounted in Windows.
        /// </summary>
        public String VolumeLabel;
    }

    public class Board : IMigrateable
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

        /// <summary>
        /// Settings relating to updating the firmware via a mounted USB drive.
        /// </summary>
        public UsbDriveSettings UsbDriveSettings;

        /// <summary>
        /// Base path for custom firmware
        /// </summary>
        public String BasePath;

        /// <summary>
        /// Returns the correct Partner Level
        /// </summary>
        /// <returns></returns>
        [JsonIgnore]
        public BoardPartnerLevel PartnerLevel
        {
            get
            {
                if (BasePath == "" && Info.MobiFlightType.Contains("MobiFlight"))
                    return BoardPartnerLevel.Core;

                var partners = new List<string>() {
                    "CoreFlightTech",
                    "kavSimulations",
                    "miniCOCKPIT"
                };
                var partner = partners.Find(p => BasePath?.Contains(p) ?? false) != null;
                if (partner) return BoardPartnerLevel.Partner;

                return BoardPartnerLevel.Community;
            }
        }


        /// <summary>
        /// Migrates board definitions from older versions to newer versions.
        /// </summary>
#pragma warning disable CS0612 // Type or member is obsolete
        public void Migrate()
        {
            // Migrate AvrDudeSettings from older versions.
            if (AvrDudeSettings != null)
            {
                // Older versions of boards only specified a single baud rate. Handle the case where
                // an old file was loaded by migrating the BaudRate value into the BaudRates array.
                if (!String.IsNullOrEmpty(AvrDudeSettings.BaudRate) && AvrDudeSettings.BaudRates == null)
                {
                    AvrDudeSettings.BaudRates = new List<string>()
                    {
                        AvrDudeSettings.BaudRate
                    };
                }

                // Older versions of boards specified the reset firmware filename in the AvrDudeSettings.
                if (!String.IsNullOrEmpty(AvrDudeSettings.ResetFirmwareFile) && String.IsNullOrEmpty(Info.ResetFirmwareFile))
                {
                    Info.ResetFirmwareFile = AvrDudeSettings.ResetFirmwareFile;
                }

                // Older versions of boards specified the firmware base name in the AvrDudeSettings.
                if (!String.IsNullOrEmpty(AvrDudeSettings.FirmwareBaseName) && String.IsNullOrEmpty(Info.FirmwareBaseName))
                {
                    Info.FirmwareBaseName = AvrDudeSettings.FirmwareBaseName;
                }

                // Older versions of boards didn't specify the firmware extension in the AvrDudeSettings. Assume it is "hex"
                // if missing which is what was used in the old code for all AVR-based boards.
                if (!String.IsNullOrEmpty(Info.FirmwareExtension))
                {
                    Info.FirmwareExtension = "hex";
                }
            }
        }
#pragma warning restore CS0612 // Type or member is obsolete

        public override string ToString()
        {
            return $"{Info.MobiFlightType} ({Info.FriendlyName})";
        }

        /// <summary>
        /// Get the name for the Default Config
        /// </summary>
        /// <returns>The </returns>
        public string GetDefaultDeviceConfigFilePath()
        {
            return $@"{BasePath}\config\{Info.FirmwareBaseName}.mfmc";
        }
    }
}