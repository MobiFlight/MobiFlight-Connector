using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    /// <summary>
    /// The text an LCD display shows for a config item, one string per row.
    /// Rendered with the same code that drives the hardware.
    /// </summary>
    public class LcdDisplayPreview
    {
        public string GUID { get; set; }
        public List<string> Lines { get; set; } = new List<string>();

        /// <summary>
        /// Splits the single string returned by <see cref="MobiFlightLcdDisplay.Apply"/>
        /// into rows of <paramref name="cols"/> characters.
        /// </summary>
        public static LcdDisplayPreview FromDisplayText(string guid, string displayText, int cols)
        {
            var preview = new LcdDisplayPreview { GUID = guid };
            displayText = displayText ?? "";

            for (var start = 0; start < displayText.Length; start += cols)
            {
                var length = System.Math.Min(cols, displayText.Length - start);
                preview.Lines.Add(displayText.Substring(start, length));
            }

            return preview;
        }
    }
}
