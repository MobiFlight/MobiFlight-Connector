namespace MobiFlight.Joysticks.Logitech
{
    /// <summary>
    /// Position of a Radio Panel rotary selector. The values match the selector
    /// positions in the input report (0 = COM1 ... 6 = XPDR).
    /// </summary>
    internal enum RadioPanelMode
    {
        None = -1,
        Com1 = 0,
        Com2 = 1,
        Nav1 = 2,
        Nav2 = 3,
        Adf = 4,
        Dme = 5,
        Xpdr = 6
    }

    internal static class RadioPanelModes
    {
        public const int Count = 7;

        public static readonly RadioPanelMode[] All =
        [
            RadioPanelMode.Com1, RadioPanelMode.Com2, RadioPanelMode.Nav1, RadioPanelMode.Nav2,
            RadioPanelMode.Adf, RadioPanelMode.Dme, RadioPanelMode.Xpdr
        ];

        /// <summary>Name used in output ids and input labels, e.g. "COM1".</summary>
        public static string GetName(RadioPanelMode mode) => mode switch
        {
            RadioPanelMode.Com1 => "COM1",
            RadioPanelMode.Com2 => "COM2",
            RadioPanelMode.Nav1 => "NAV1",
            RadioPanelMode.Nav2 => "NAV2",
            RadioPanelMode.Adf => "ADF",
            RadioPanelMode.Dme => "DME",
            RadioPanelMode.Xpdr => "XPDR",
            _ => string.Empty
        };

        /// <summary>COM and NAV show radio frequencies, the other modes show plain numbers.</summary>
        public static bool ShowsFrequency(RadioPanelMode mode) =>
            mode is RadioPanelMode.Com1 or RadioPanelMode.Com2 or RadioPanelMode.Nav1 or RadioPanelMode.Nav2;
    }
}
