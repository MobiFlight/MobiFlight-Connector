using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class Board
    {
        public int MaxOutputs = 0;
        public int MaxButtons = 0;
        public int MaxLedSegments = 0;
        public int MaxEncoders = 0;
        public int MaxSteppers = 0;
        public int MaxServos = 0;
        public int MaxLcdI2C = 0;
        public int MaxAnalogInputs = 0;

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
        public List<String> HardwareIds;

        public int MessageSize;
        public int EEPROMSize;

        public List<MobiFlightPin> Pins;

        public override string ToString()
        {
            return $"{FriendlyName} ({MobiFlightType})";
        }
    }
}
