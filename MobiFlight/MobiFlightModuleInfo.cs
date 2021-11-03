using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public abstract class MobiFlightCapabilities
    {
        public int MaxOutputs = 0;
        public int MaxButtons = 0;
        public int MaxLedSegments = 0;
        public int MaxEncoders = 0;
        public int MaxSteppers = 0;
        public int MaxServos = 0;
        public int MaxLcdI2C = 0;
        public int MaxAnalogInputs = 0;
    }

    public class MegaCapabilities : MobiFlightCapabilities
    {
        public MegaCapabilities()
        {
            MaxOutputs = 40;
            MaxButtons = 68;
            MaxLedSegments = 4;
            MaxEncoders = 20;
            MaxSteppers = 10;
            MaxServos = 10;
            MaxLcdI2C = 2;
            MaxAnalogInputs = 5;
        }
    }

    public class MicroCapabilities : MobiFlightCapabilities
    {
        public MicroCapabilities()
        {
            MaxOutputs = 10;
            MaxButtons = 16;
            MaxLedSegments = 1;
            MaxEncoders = 4;
            MaxSteppers = 2;
            MaxServos = 2;
            MaxLcdI2C = 2;
            MaxAnalogInputs = 2;
        }
    }

    public class UnoCapabilities : MobiFlightCapabilities
    {
        public UnoCapabilities()
        {
            MaxOutputs = 8;
            MaxButtons = 8;
            MaxLedSegments =1;
            MaxEncoders = 2;
            MaxSteppers = 2;
            MaxServos = 2;
            MaxLcdI2C = 2;
            MaxAnalogInputs = 2;
        }
    }

    public class MobiFlightModuleInfo : IModuleInfo
    {
        public const String TYPE_UNKNOWN = "unknown";

        public const String LatestFirmwareMega = "1.11.3";
        public const String LatestFirmwareMicro = "1.11.3";
        public const String LatestFirmwareUno = "1.11.3";

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
        public static readonly List<MobiFlightPin> MEGA_PINS = new List<MobiFlightPin>() {
            { new MobiFlightPin() { Pin =  2, isPWM = true } },
            { new MobiFlightPin() { Pin =  3, isPWM = true } },
            { new MobiFlightPin() { Pin =  4, isPWM = true } },
            { new MobiFlightPin() { Pin =  5, isPWM = true } },
            { new MobiFlightPin() { Pin =  6, isPWM = true } },
            { new MobiFlightPin() { Pin =  7, isPWM = true } },
            { new MobiFlightPin() { Pin =  8, isPWM = true } },
            { new MobiFlightPin() { Pin =  9, isPWM = true } },
            // 10-19
            { new MobiFlightPin() { Pin = 10, isPWM = true } },
            { new MobiFlightPin() { Pin = 11, isPWM = true } },
            { new MobiFlightPin() { Pin = 12, isPWM = true } },
            { new MobiFlightPin() { Pin = 13, isPWM = true } },
            { new MobiFlightPin() { Pin = 14 } },
            { new MobiFlightPin() { Pin = 15 } },
            { new MobiFlightPin() { Pin = 16 } },
            { new MobiFlightPin() { Pin = 17 } },
            { new MobiFlightPin() { Pin = 18 } },
            { new MobiFlightPin() { Pin = 19 } },
            // 20-29
            { new MobiFlightPin() { Pin = 20, isI2C = true } },
            { new MobiFlightPin() { Pin = 21, isI2C = true } },
            { new MobiFlightPin() { Pin = 22 } },
            { new MobiFlightPin() { Pin = 23 } },
            { new MobiFlightPin() { Pin = 24 } },
            { new MobiFlightPin() { Pin = 25 } },
            { new MobiFlightPin() { Pin = 26 } },
            { new MobiFlightPin() { Pin = 27 } },
            { new MobiFlightPin() { Pin = 28 } },
            { new MobiFlightPin() { Pin = 29 } },
            // 30-39
            { new MobiFlightPin() { Pin = 30 } },
            { new MobiFlightPin() { Pin = 31 } },
            { new MobiFlightPin() { Pin = 32 } },
            { new MobiFlightPin() { Pin = 33 } },
            { new MobiFlightPin() { Pin = 34 } },
            { new MobiFlightPin() { Pin = 35 } },
            { new MobiFlightPin() { Pin = 36 } },
            { new MobiFlightPin() { Pin = 37 } },
            { new MobiFlightPin() { Pin = 38 } },
            { new MobiFlightPin() { Pin = 39 } },
            // 40-49
            { new MobiFlightPin() { Pin = 40 } },
            { new MobiFlightPin() { Pin = 41 } },
            { new MobiFlightPin() { Pin = 42 } },
            { new MobiFlightPin() { Pin = 43 } },
            { new MobiFlightPin() { Pin = 44, isPWM = true } },
            { new MobiFlightPin() { Pin = 45, isPWM = true } },
            { new MobiFlightPin() { Pin = 46, isPWM = true } },
            { new MobiFlightPin() { Pin = 47 } },
            { new MobiFlightPin() { Pin = 48 } },
            { new MobiFlightPin() { Pin = 49 } },
            // 50-59
            { new MobiFlightPin() { Pin = 50 } },
            { new MobiFlightPin() { Pin = 51 } },
            { new MobiFlightPin() { Pin = 52 } },
            { new MobiFlightPin() { Pin = 53 } },
            { new MobiFlightPin() { Pin = 54, isAnalog = true, Name = "A0" } },
            { new MobiFlightPin() { Pin = 55, isAnalog = true, Name = "A1" } },
            { new MobiFlightPin() { Pin = 56, isAnalog = true, Name = "A2" } },
            { new MobiFlightPin() { Pin = 57, isAnalog = true, Name = "A3" } },
            { new MobiFlightPin() { Pin = 58, isAnalog = true, Name = "A4" } },
            { new MobiFlightPin() { Pin = 59, isAnalog = true, Name = "A5" } },
            // 60-69
            { new MobiFlightPin() { Pin = 60, isAnalog = true, Name = "A6" } },
            { new MobiFlightPin() { Pin = 61, isAnalog = true, Name = "A7" } },
            { new MobiFlightPin() { Pin = 62, isAnalog = true, Name = "A8" } },
            { new MobiFlightPin() { Pin = 63, isAnalog = true, Name = "A9" } },
            { new MobiFlightPin() { Pin = 64, isAnalog = true, Name = "A10" } },
            { new MobiFlightPin() { Pin = 65, isAnalog = true, Name = "A11" } },
            { new MobiFlightPin() { Pin = 66, isAnalog = true, Name = "A12" } },
            { new MobiFlightPin() { Pin = 67, isAnalog = true, Name = "A13" } },
            { new MobiFlightPin() { Pin = 68, isAnalog = true, Name = "A14" } },
            { new MobiFlightPin() { Pin = 69, isAnalog = true, Name = "A15" } }
        };

        public static readonly List<MobiFlightPin> MICRO_PINS = new List<MobiFlightPin>() {
            { new MobiFlightPin() { Pin =  0 } },
            { new MobiFlightPin() { Pin =  1 } },
            { new MobiFlightPin() { Pin =  2, isI2C = true } },
            { new MobiFlightPin() { Pin =  3, isPWM = true, isI2C = true} },
            { new MobiFlightPin() { Pin =  4, isAnalog = true } },
            { new MobiFlightPin() { Pin =  5, isPWM = true } },
            { new MobiFlightPin() { Pin =  6, isPWM = true, isAnalog = true } },
            { new MobiFlightPin() { Pin =  7 } },
            { new MobiFlightPin() { Pin =  8, isAnalog = true } },
            { new MobiFlightPin() { Pin =  9, isPWM = true, isAnalog = true } },
            // 10-19
            { new MobiFlightPin() { Pin = 10, isPWM = true } },
            { new MobiFlightPin() { Pin = 14 } },
            { new MobiFlightPin() { Pin = 15 } },
            { new MobiFlightPin() { Pin = 16 } },
            { new MobiFlightPin() { Pin = 17 } },
            { new MobiFlightPin() { Pin = 18, isAnalog = true, Name = "A0" } },
            { new MobiFlightPin() { Pin = 19, isAnalog = true, Name = "A1" } },
            // 20-21
            { new MobiFlightPin() { Pin = 20, isAnalog = true, Name = "A2" } },
            { new MobiFlightPin() { Pin = 21, isAnalog = true, Name = "A3" } }
        };

        public static readonly List<MobiFlightPin> UNO_PINS = new List<MobiFlightPin>() {
            { new MobiFlightPin() { Pin =  2 } },
            { new MobiFlightPin() { Pin =  3, isPWM = true } },
            { new MobiFlightPin() { Pin =  4 } },
            { new MobiFlightPin() { Pin =  5, isPWM = true } },
            { new MobiFlightPin() { Pin =  6, isPWM = true } },
            { new MobiFlightPin() { Pin =  7 } },
            { new MobiFlightPin() { Pin =  8 } },
            { new MobiFlightPin() { Pin =  9, isPWM = true } },
            // 10-19
            { new MobiFlightPin() { Pin = 10, isPWM = true } },
            { new MobiFlightPin() { Pin = 11, isPWM = true } },
            { new MobiFlightPin() { Pin = 12 } },
            { new MobiFlightPin() { Pin = 13 } },
            { new MobiFlightPin() { Pin = 14, isAnalog = true, Name = "A0" } },
            { new MobiFlightPin() { Pin = 15, isAnalog = true, Name = "A1" } },
            { new MobiFlightPin() { Pin = 16, isAnalog = true, Name = "A2" } },
            { new MobiFlightPin() { Pin = 17, isAnalog = true, Name = "A3" } },
            { new MobiFlightPin() { Pin = 18, isAnalog = true, Name = "A4", isI2C = true } },
            { new MobiFlightPin() { Pin = 19, isAnalog = true, Name = "A5", isI2C = true } },
        };

        // different vendor and product ids for 
        // board detection
        public static readonly String[] VIDPID_MICRO = {
            "VID_1B4F&PID_9206",
            "VID_2341&PID_8036",  // Arduino Pro Micro
            "VID_2341&PID_8037"   // https://bitbucket.org/mobiflight/mobiflightfc/issues/324/add-new-micro-pro-vid_2341-pid_8037
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

        public MobiFlightCapabilities GetCapabilities()
        {
            MobiFlightCapabilities result = null;

            switch(Type)
            {
                case TYPE_MEGA:
                    result = new MegaCapabilities();
                    break;

                case TYPE_MICRO:
                    result = new MicroCapabilities();
                    break;

                case TYPE_UNO:
                    result = new UnoCapabilities();
                    break;
            }

            return result;
        }
    }
}
