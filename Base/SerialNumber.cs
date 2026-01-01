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
