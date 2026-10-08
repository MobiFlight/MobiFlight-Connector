using System;
using System.Linq;

namespace MobiFlightMoza.Tests
{
    // Turns a guide example like "7e 00 00 12 9d" into a byte[].
    internal static class TestHex
    {
        public static byte[] Parse(string hex)
        {
            return [.. hex
                .Split([' '], StringSplitOptions.RemoveEmptyEntries)
                .Select(token => Convert.ToByte(token, 16))];
        }
    }
}
