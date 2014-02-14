using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public class MobiFlightModuleInfo : IModuleInfo
    {
        public static String TYPE_UNKNOWN = "unknown";
        public static String TYPE_MICRO = "MF Micro";
        public static String TYPE_MEGA = "MF Mega";
        ushort _version = 100;
        public String Type   { get; set; }
        public String Serial { get; set; }
        public String Port   { get; set; }
        public String Name   { get; set; }

        public ushort Version
        {
            get { return _version;  }
            set { _version = value; }
        }
    }
}
