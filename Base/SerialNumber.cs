using System;
using System.Linq;

namespace MobiFlight.Base
{
    public static class SerialNumber
    {
        public const string NOT_SET = "-";
        public const string SerialSeparator = "/";

        public static string CreateFullSerial(string deviceName, string serial)
        {
            return $"{deviceName}{SerialSeparator}{serial}";
        }

        public static string Normalize(string s)
        {
            if (!IsRawSerial(s)) return s;

            var name = ExtractDeviceName(s);
            var serial = ExtractSerial(s);

            return CreateFullSerial(name, serial);
        }

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

            return String.Join(SerialSeparator, tokens).Trim();
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
            var isValidArcazeSerialFormat = serial.Length == 12 && serial.All(char.IsDigit);

            return isValidArcazeSerialFormat;
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
            if (string.IsNullOrEmpty(serial) || !serial.Contains(SerialSeparator))
                return false;

            // Extract what would be the serial part after the last separator
            var potentialSerial = ExtractSerial(serial);

            // A valid raw serial must have a non-empty serial part
            // and it should look like an actual serial number
            if (string.IsNullOrEmpty(potentialSerial))
                return false;

            // Check if it matches known serial patterns
            return IsMobiFlightSerial(potentialSerial) ||
                   IsJoystickSerial(potentialSerial) ||
                   IsMidiBoardSerial(potentialSerial) ||
                   IsArcazeSerial(potentialSerial);
        }
    }
}
