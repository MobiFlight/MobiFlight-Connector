using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class MobiFlightDeviceInfo
    {
        public static String TYPE_UNKNOWN = "unknown";
        public static String TYPE_MICRO = "MF Micro";
        public static String TYPE_MEGA = "MF Mega";
        public String Type { get; set; }
        public String Port { get; set; }
        public String Name { get; set; }
    }
}
