using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    class UsageDetails
    {
        protected static bool enabled = false;

        public static bool Enabled()
        {
            return enabled;
        }

        public static void Enabled(bool state)
        {
            enabled = state;
        }

        public static int Started()
        {
            return Properties.Settings.Default.Started;
        }

        public static string UrlString()
        {
            if (!enabled) return "";

            return "?"
                +  "started=" + Started() + "&"
                +  "size=";
        }
    }
}
