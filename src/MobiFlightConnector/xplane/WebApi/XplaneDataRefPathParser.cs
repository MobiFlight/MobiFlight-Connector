using System;
using System.Globalization;

namespace MobiFlight.xplane.WebApi
{
    internal static class XplaneDataRefPathParser
    {
        public static (string Name, int? Index) Parse(string path)
        {
            if (string.IsNullOrEmpty(path)) return (path, null);

            var span = path.AsSpan();
            if (span[^1] != ']') return (path, null);

            var openBracket = span.LastIndexOf('[');
            if (openBracket < 0) return (path, null);

            var digits = span[(openBracket + 1)..^1];
            if (digits.IsEmpty || !int.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var index))
                return (path, null);

            return (span[..openBracket].ToString(), index);
        }
    }
}
