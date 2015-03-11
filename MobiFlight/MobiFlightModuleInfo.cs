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
        public const String PIDVID_MICRO = "VID_1B4F&PID_9206"; // Micro
        public const String PIDVID_MEGA = "VID_2341&PID_0042";  // Mega
        public const String PIDVID_MEGA_10 = "VID_2341&PID_0010";  // Mega
        public const String PIDVID_MEGA_CLONE = "VID_8087&PID_0024";  // MegaVID_8087&PID_0024
        public const String PIDVID_MEGA_CLONE_1 = "VID_1A86&PID_7523";  // MegaVID_8087&PID_0024
        
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

        public void SetTypeByVidPid(String PidVid)
        {
            switch (PidVid)
            {
                case PIDVID_MEGA_CLONE_1:
                case PIDVID_MEGA_CLONE:
                case PIDVID_MEGA_10:
                case PIDVID_MEGA:
                    Name = TYPE_ARDUINO_MEGA;
                    Type = TYPE_ARDUINO_MEGA;
                    break;
                case PIDVID_MICRO:
                    Name = TYPE_ARDUINO_MICRO;
                    Type = TYPE_ARDUINO_MICRO;
                    break;
                default:
                    Type = TYPE_UNKNOWN;
                    break;
            }
        }
    }
}
