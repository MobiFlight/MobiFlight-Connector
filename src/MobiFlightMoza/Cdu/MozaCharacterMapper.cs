using System.Collections.Generic;
using System.Text;

namespace MobiFlightMoza.Cdu
{
    /// <summary>
    /// Maps Unicode symbols the MCDU websocket JSON may carry onto characters MOZA's v1
    /// built-in font actually renders. MOZA covers WGL4 (arrows,
    /// Greek, degree sign all render correctly), but not the Miscellaneous Symbols block,
    /// so the ballot box U+2610 that every CDU aircraft script emits for an empty/
    /// uninitialized field falls back to the device's own "?" glyph.
    /// </summary>
    internal static class MozaCharacterMapper
    {
        private static readonly Dictionary<char, char> Substitutions = new()
        {
            ['☐'] = '□', // U+2610 ballot box (not in WGL4) -> U+25A1 white square (is in WGL4)
        };

        public static char ToSupportedChar(char character)
            => Substitutions.GetValueOrDefault(character, character);

        public static string ToSupportedText(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            StringBuilder result = null;
            for (var i = 0; i < text.Length; i++)
            {
                var mapped = ToSupportedChar(text[i]);
                if (mapped != text[i] && result == null)
                {
                    result = new StringBuilder(text, 0, i, text.Length);
                }
                result?.Append(mapped);
            }
            return result?.ToString() ?? text;
        }
    }
}
