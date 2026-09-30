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

            if (!s.Contains(SerialSeparator)) return s;

            var tokens = s.Split(serialSeparator, StringSplitOptions.RemoveEmptyEntries);

            return tokens.Last().Trim();
        }

        public static string ExtractControllerName(String s)
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

        internal static string BuildFullSerial(Controller controller)
        {
            if (controller == null) return null;

            // this is from legacy times. Some configs may have empty name and serial set to NOT_SET.
            // In this case we want to return NOT_SET instead of " / -"
            if (string.IsNullOrEmpty(controller.Name) && !string.IsNullOrEmpty(controller.Serial)) return controller.Serial;
            
            // Joystick and MIDI board serials get a leading space before the separator,
            // MobiFlight serials and any other (e.g. legacy/generic) serials do not.
            var isJoystickOrMidiBoardSerial = SerialNumber.IsJoystickSerial(controller.Serial) || SerialNumber.IsMidiBoardSerial(controller.Serial);
            var serialSeparator = isJoystickOrMidiBoardSerial ? " " + SerialNumber.SerialSeparator : SerialNumber.SerialSeparator;
            
            return $"{controller.Name}{serialSeparator}{controller.Serial}";
        }

        public static Controller CreateController(string serial)
        {
            if (string.IsNullOrEmpty(serial) || serial == NOT_SET) return null;
            var deviceName = ExtractControllerName(serial);
            var deviceSerial = ExtractSerial(serial);
            return new Controller() { Name = deviceName, Serial = deviceSerial };
        }
    }
}