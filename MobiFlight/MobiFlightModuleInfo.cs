using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class MobiFlightModuleInfo : IModuleInfo
    {
        public const String TYPE_UNKNOWN = "unknown";

        public const String LatestFirmwareMega = "1.9.10";
        public const String LatestFirmwareMicro = "1.9.10";
        public const String LatestFirmwareUno = "1.9.10";

        // these types are used for standard stock arduino boards
        public const String TYPE_ARDUINO_MICRO = "Arduino Micro Pro";
        public const String TYPE_ARDUINO_MEGA = "Arduino Mega 2560";
        public const String TYPE_ARDUINO_UNO = "Arduino Uno";

        // these types are used once the MF firmware is installed
        public const String TYPE_MICRO = "MobiFlight Micro";
        public const String TYPE_MEGA = "MobiFlight Mega";
        public const String TYPE_UNO = "MobiFlight Uno";

        // message size is used for building
        // correct chunk sizes for messages
        // to the arduino boards
        public const int MESSAGE_MAX_SIZE_MICRO = 32;
        public const int MESSAGE_MAX_SIZE_UNO = 32;
        public const int MESSAGE_MAX_SIZE_MEGA = 64;

        // this is used to check for 
        // maximum config length and
        // alert the user in the UI if exceeded
        public const int EEPROM_SIZE_MICRO = 256;
        public const int EEPROM_SIZE_UNO = 256;
        public const int EEPROM_SIZE_MEGA = 1024;

        // this is not yet used
        // available pins
        public static readonly byte[] MEGA_PINS = {
                   2, 3, 4, 5, 6, 7, 8, 9,
            10,11,12,13,14,15,16,17,18,19,
            20,21,22,23,24,25,26,27,28,29,
            30,31,32,33,34,35,36,37,38,39,
            40,41,42,43,44,45,46,47,48,49,
            50,51,52,53,
            // Analog Pins
                        54,55,56,57,58,59,
            60,61,62,63,64,65,66,67,68,69
        };

        public static readonly byte[] MICRO_PINS = {
                   2, 3, 4, 5, 6, 7, 8, 9,
            10,11,12,13,14,15,16
        };

        public static readonly byte[] UNO_PINS = {
                   2, 3, 4, 5, 6, 7, 8, 9,
            10,11,12,13
        };

        public static readonly byte[] MEGA_PWM = {
                   2, 3, 4, 5, 6, 7, 8, 9,
            10,11,12,13
        };

        public static readonly byte[] MICRO_PWM = {
                   2, 3, 4, 5, 6, 7, 8, 9,
            10
        };

        public static readonly byte[] UNO_PWM = {
                   2, 3, 4, 5, 6, 7, 8, 9,
            10,11,12,13
        };

        public static readonly byte[] MEGA_PINS_ANALOG = {
                        54,55,56,57,58,59,
            60,61,62,63,64,65,66,67,68,69
        };

        public static readonly byte[] MICRO_PINS_ANALOG = {
                        18,19,20,21,
            4,6,8,9,10
        };

        public static readonly byte[] UNO_PINS_ANALOG = {
                        14,15,16,7,18,19
        };

        // different vendor and product ids for 
        // board detection
        public static readonly String[] VIDPID_MICRO = {
            "VID_1B4F&PID_9206",
            "VID_2341&PID_8036"  // Arduino Pro Micro
        };

        public static readonly String[] VIDPID_UNO = {
            //"VID_1A86&PID_7523", // this is actually an CH-340 and can be a Mega OR an UNO
            "VID_2341&PID_0043",
            "VID_2A03&PID_0043"  // https://www.mobiflight.com/forum/topic/680.html
        };

        public static readonly String[] VIDPID_MEGA = {
            "VID_2341&PID_0010",
            "VID_2341&PID_0042",
            "VID_2341&PID_0001",            // was reported on youtube video
            "VID_8087&PID_0024",
            "VID_1A86&PID_7523",            // this is actually an CH-340 and can be a Mega OR an UNO
            "VID_2A03&PID_0042",            // http://www.mobiflight.de/forum/message/983.html
            "VID_0403&PID_6001",            // http://www.mobiflight.de/forum/topic/570.html
            "VID_0403\\+PID_6001\\+.+",     // https://bitbucket.org/mobiflight/mobiflightfc/issues/265
                                            // https://bitbucket.org/mobiflight/mobiflightfc/issues/280/ftdi-driver-board-is-not-connected
            // added from https://github.com/arduino/Arduino/blob/1.8.0/hardware/arduino/avr/boards.txt#L51-L58
            "VID_2A03&PID_0010",
            "VID_2341&PID_0210",
            "VID_2341&PID_0242",
            "VID_10C4&PID_EA60"             // https://www.mobiflight.com/forum/message/20158.html
        };

        String _version = "n/a";
        public String Type { get; set; }
        public String Serial { get; set; }
        public String Port { get; set; }
        public String Name { get; set; }
        public String Config { get; set; }

        public String Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public bool HasMfFirmware()
        {
            return (Type == TYPE_MICRO) || (Type == TYPE_MEGA) || (Type == TYPE_UNO);
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

        public void SetTypeByVidPid(String VidPid)
        {
            Type = TYPE_UNKNOWN;

            if (VIDPID_MEGA.Contains(VidPid))
            {
                Name = TYPE_ARDUINO_MEGA;
                Type = TYPE_ARDUINO_MEGA;
            }
            else if (VIDPID_MICRO.Contains(VidPid))
            {
                Name = TYPE_ARDUINO_MICRO;
                Type = TYPE_ARDUINO_MICRO;
            }

            else if (VIDPID_UNO.Contains(VidPid))
            {
                Name = TYPE_ARDUINO_UNO;
                Type = TYPE_ARDUINO_UNO;
            }
        }
    }
}
