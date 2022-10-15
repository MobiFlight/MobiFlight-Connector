using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace MobiFlight.Base
{
    public static class SerialNumber
    {
        public static string ExtractSerial(String s)
        {
            if (s == null) return "";

            if (!s.Contains("/")) return "";

            return s.Split('/')[1].Trim();
        }

        public static string ExtractDeviceName(String s)
        {
            if (s == null) return "";

            if (!s.Contains("/")) return "";

            return s.Split('/')[0].Trim();
        }

        public static bool IsMobiFlightSerial(string serial)
        {
            return (serial.IndexOf("SN") == 0);
        }

        public static bool IsJoystickSerial(string serial)
        {
            return (serial.IndexOf(Joystick.SerialPrefix) == 0);
        }
    }
}
