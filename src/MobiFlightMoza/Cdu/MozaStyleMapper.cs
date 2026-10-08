using System.Collections.Generic;

namespace MobiFlightMoza.Cdu
{
    /// <summary>
    /// Maps a MobiFlight CDU colour letter to a MOZA style-byte colour code (0-8, selecting
    /// one of the device's 9 palette slots). Plain bit composition for the style byte itself
    /// (bit7 size, bits3..0 colour code) - not WinCtrl's additive-offset scheme.
    /// </summary>
    internal static class MozaStyleMapper
    {
        public const byte DefaultColorCode = 7; // white

        // 'o' (blue) and 'k' (khaki) have no dedicated MOZA slot and approximate to the
        // nearest one (cyan, amber). Everything else maps onto its own named slot.
        private static readonly Dictionary<char, byte> ColorCodes = new()
        {
            ['w'] = 7,
            ['c'] = 1,
            ['r'] = 2,
            ['y'] = 3,
            ['g'] = 4,
            ['m'] = 5,
            ['a'] = 6,
            ['e'] = 8,
            ['o'] = 1,
            ['k'] = 6,
        };

        public static byte ToColorCode(char mobiFlightColor)
            => ColorCodes.TryGetValue(mobiFlightColor, out byte code) ? code : DefaultColorCode;

        public static byte ToStyleByte(CduCell cell)
            => (byte)((cell.IsSmall ? 0x00 : 0x80) | ToColorCode(cell.ColorLetter));
    }
}
