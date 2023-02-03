using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Base
{
    public static class Extensions
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        public static string ToCRLF(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            // handle mixed line endings correctly (avoid expansion of \n in \r\n -> \r\n\n)
            // first, convert all line-endings to only LF
            // then, convert all line-endings to CRLF
            return value.Replace("\r\n", "\n").Replace("\n", "\r\n");
        }

        public static string ToLF(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            // mixed lines are treated correctly
            return value.Replace("\r\n", "\n");
        }
    }
}
