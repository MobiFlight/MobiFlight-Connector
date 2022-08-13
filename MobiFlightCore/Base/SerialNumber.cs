using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
