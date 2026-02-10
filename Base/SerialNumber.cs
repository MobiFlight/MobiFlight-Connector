using System;
using System.Linq;

namespace MobiFlight.Base
{
    public static class SerialNumber
    {
        public const string NOT_SET = "-";
        public const string SerialSeparator = "/ ";

        public static string ExtractSerial(String s)
        {
            string[] serialSeparator = { SerialSeparator };
            if (s == null) return "";

            if (!s.Contains(SerialSeparator)) return "";

            var tokens = s.Split(serialSeparator, StringSplitOptions.RemoveEmptyEntries);

            return tokens.Last().Trim();
        }

        public static string ExtractDeviceName(String s)
        {
            string[] serialSeparator = { SerialSeparator };
            if (s == null) return "";

            if (!s.Contains(SerialSeparator)) return "";

            var tokens = s.Split(serialSeparator, StringSplitOptions.None);
            tokens = tokens.Take(tokens.Length - 1).ToArray();

            return String.Join("", tokens).Trim();
        }

        /// <summary>
        /// Creates a Controller from a full serial string in the format "Name/ Serial"
        /// Used for XML deserialization
        /// </summary>
        public static Controller FromFullSerial(string fullSerial)
        {
            if (string.IsNullOrEmpty(fullSerial))
                return new Controller();

            var name = ExtractDeviceName(fullSerial);
            var serial = ExtractSerial(fullSerial);
            return new Controller(name, serial);
        }

        /// <summary>
        /// Converts a Controller to a full serial string in the format "Name/ Serial"
        /// Used for XML serialization
        /// </summary>
        public static string ToFullSerial(Controller controller)
        {
            if (controller == null || (string.IsNullOrEmpty(controller.Name) && string.IsNullOrEmpty(controller.Serial)))
                return "";
            
            if (string.IsNullOrEmpty(controller.Serial))
                return controller.Name;

            return $"{controller.Name}{SerialSeparator}{controller.Serial}";
        }

        /// <summary>
        /// Extracts the device type prefix from a serial number (e.g., "SN-", "JS-", "MI-")
        /// If no match - returns null
        /// </summary>
        /// <returns>Device type prefix from a serial number (e.g., "SN-", "JS-", "MI-") or null if no match</returns>
        public static string ExtractPrefix(string fullString)
        {
            var serial = ExtractSerial(fullString);
            if (serial.StartsWith(MobiFlightModule.SerialPrefix)) return MobiFlightModule.SerialPrefix;
            else if (serial.StartsWith(Joystick.SerialPrefix)) return Joystick.SerialPrefix;
            else if (serial.StartsWith(MidiBoard.SerialPrefix)) return MidiBoard.SerialPrefix;
            return null;
        }

        public static bool IsArcazeSerial(string serial)
        {
            if (serial == null || serial == "") return false;
            return !IsMidiBoardSerial(serial) && !IsMobiFlightSerial(serial) && !IsJoystickSerial(serial);
        }

        public static bool IsMobiFlightSerial(string serial)
        {
            if (serial == null || serial == "") return false;
            return (serial.IndexOf("SN") == 0);
        }

        public static bool IsJoystickSerial(string serial)
        {
            if (serial == null || serial == "") return false;
            return (serial.IndexOf(Joystick.SerialPrefix) == 0);
        }

        public static bool IsMidiBoardSerial(string serial)
        {
            if (string.IsNullOrEmpty(serial)) return false;
            return (serial.IndexOf(MidiBoard.SerialPrefix) == 0);
        }

        public static bool IsRawSerial(string serial)
        {
            return (serial != null && serial.Contains(SerialSeparator));
        }
    }
}
