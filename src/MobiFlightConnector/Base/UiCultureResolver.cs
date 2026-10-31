using System.Globalization;

namespace MobiFlight.Base
{
    internal static class UiCultureResolver
    {
        /// <summary>
        /// Maps the saved language setting (e.g. "de-DE") to its language-only culture ("de").
        /// </summary>
        // Issue 3484: a stale de-DE satellite from old installs must not be loaded
        public static CultureInfo FromSetting(string language)
        {
            return new CultureInfo(new CultureInfo(language).TwoLetterISOLanguageName);
        }
    }
}