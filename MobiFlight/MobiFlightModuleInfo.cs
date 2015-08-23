using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class MobiFlightModuleInfo : IModuleInfo
    {
        public const String TYPE_UNKNOWN = "unknown";
        public const String TYPE_ARDUINO_MICRO = "Arduino Micro Pro";
        public const String TYPE_ARDUINO_MEGA = "Arduino Mega 2560";
        public const String TYPE_MICRO = "MobiFlight Micro";
        public const String TYPE_MEGA = "MobiFlight Mega";
                        
        public const int MESSAGE_MAX_SIZE_MICRO = 64;
        public const int MESSAGE_MAX_SIZE_MEGA = 64;
        public const int EEPROM_SIZE_MICRO = 512;
        public const int EEPROM_SIZE_MEGA = 1024;

        String _version = "n/a";
        public String Type   { get; set; }
        public String Serial { get; set; }
        public String Port   { get; set; }
        public String Name   { get; set; }
        public String Config { get; set; }

        public String Version
        {
            get { return _version;  }
            set { _version = value; }
        }

        public bool HasMfFirmware()
        {
            return (Type == TYPE_MICRO) || (Type == TYPE_MEGA);
        }

        public void SetTypeByName(String FriendlyName)
        {
            Name = Type = TYPE_UNKNOWN;

            if (FriendlyName.Contains("Pro Micro"))
            {
                Name = TYPE_ARDUINO_MICRO;
                Type = TYPE_ARDUINO_MICRO;
            }

            if (FriendlyName.Contains("Mega 2560"))
            {
                Name = TYPE_ARDUINO_MEGA;
                Type = TYPE_ARDUINO_MEGA;
            }
        }
    }
}
