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

        ushort _version = 100;
        public String Type   { get; set; }
        public String Serial { get; set; }
        public String Port   { get; set; }
        public String Name   { get; set; }
        public String Config { get; set; }

        public ushort Version
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
