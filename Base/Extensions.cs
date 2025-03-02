using System;
using System.IO;

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

        public static byte Reverse(this byte value)
        {
            byte result = 0;
            for(var i=0;i<8;i++)
            {
                if((1<<i & value) != 0)
                {
                    result |= (byte)(1<<(7-i));
                }
            }
            return (byte) result;
        }


        public static byte HammingWeight(this byte value)
        {
            byte sum = 0;

            while (value > 0)
            {
                sum += (byte)(value & 0x01);
                value >>= 1;
            }

            return sum;
        }

        public static string GetLastFolderName(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            path = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            return directoryInfo.Name;
        }

        public static int GenerateSimpleHash(this string s)
        {
            // Simple hash algorithm, folding on a string, summed 4 bytes at a time 
            long sum = 0, mul = 1;
            for (int i = 0; i < s.Length; i++)
            {
                mul = (i % 4 == 0) ? 1 : mul * 256;
                sum += (long)s[i] * mul;
            }
            return (int)(Math.Abs(sum) % int.MaxValue);
        }

        public static bool AreEqual<T>(this T a, T b) where T : class
        {
            return (a == null && b == null) || (a != null && a.Equals(b));
        }
    }
}
