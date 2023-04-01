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
