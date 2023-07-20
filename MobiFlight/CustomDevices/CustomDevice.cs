using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.CustomDevices
{
    /// <summary>
    /// General device information properties.
    /// </summary>
    public class Info
    {
        /// <summary>
        /// True if the board supports loading firmware via MobiFlight.
        /// </summary>
        public String Label;

        /// <summary>
        /// True if the board can be reset to factory default by MobiFlight.
        /// </summary>
        public String Type;

        /// <summary>
        /// Base name for firmware files. The final filename is of the form {FirmwareBaseName}_{Version}.{FirmwareExtension}.
        /// </summary>
        public String Author;

        /// <summary>
        /// File extension for firmware files. The final filename is of the form {FirmwareBaseName}_{Version}.{FirmwareExtension}.
        /// </summary>
        public String URL;

        /// <summary>
        /// The USB friendly name for the board as specified by the board manufacturer.
        /// </summary>
        public String Version;
    }

    /// <summary>
    /// Maximum number of each type of module supported by the board.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Maximum number of analog inputs supported by the board.
        /// </summary>
        public List<String> Pins;

        
        /// <summary>
        /// Maximum number of custom devices supported by the board.
        /// </summary>
        public bool isI2C = false;
    }

    public class MessageType
    {
        /// <summary>
        /// The name of a file that must be present in the root directory of the USB drive for it to be considered
        /// a match and able to be flashed.
        /// </summary>
        public string Id;

        /// <summary>
        /// Volume label of the USB drive when mounted in Windows.
        /// </summary>
        public String Label;

        /// <summary>
        /// Volume label of the USB drive when mounted in Windows.
        /// </summary>
        public String Description;

        public override string ToString()
        {
            return $"{Id}";
        }
    }

    public class CustomDevice
    {
        /// <summary>
        /// Settings related to updating the firmware via AvrDude.
        /// </summary>
        public Info Info;

        /// <summary>
        /// Connection settings for the board.
        /// </summary>
        public Config Config;

        /// <summary>
        /// A list of regular expressions of USB hardware IDs that use this board definition.
        /// </summary>
        public List<MessageType> MessageTypes = new List<MessageType>();

        public override string ToString()
        {
            return $"{Info.Label} ({Info.Version})";
        }
    }
}