using System;

namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// Parses the MOZA base unit's HID button report. The byte/bit offsets are unknown
    /// until MOZA documents the report layout - TODO once that exists, replace
    /// <see cref="PayloadLength"/> and populate <see cref="ToJoystickState"/> with a real
    /// button table, the same shape as <see cref="Logitech.SwitchPanelReport"/>. Until
    /// then buttons will not register presses (plan risk R-BUTTONREPORT).
    /// </summary>
    internal sealed class MozaButtonReport
    {
        // TODO: replace with the real HID input report length once MOZA documents it.
        public const int PayloadLength = 8;

        private MozaButtonReport() { }

        public static MozaButtonReport Parse(ReadOnlySpan<byte> payload)
        {
            if (payload.Length < PayloadLength)
            {
                throw new ArgumentException($"Invalid MOZA button payload length. Expected at least {PayloadLength}, got {payload.Length}.", nameof(payload));
            }

            return new MozaButtonReport();
        }

        // TODO: populate button bits once MOZA's HID report layout is documented.
        public JoystickState ToJoystickState() => new JoystickState();
    }
}
