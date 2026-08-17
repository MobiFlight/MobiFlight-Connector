using System.Collections.Generic;

namespace MobiFlight.Joysticks.FreeJoy
{
    /// <summary>
    /// Builds the FreeJoy external LED output report.
    /// </summary>
    /// <remarks>
    /// The report is a little-endian bitmask, one bit per LED, addressed by the
    /// <c>Byte</c> and <c>Bit</c> values of the outputs in the definition file.
    /// Protocol reference:
    /// https://github.com/FreeJoy-Team/FreeJoyWiki/blob/master/rus/%D0%92%D0%BD%D0%B5%D1%88%D0%BD%D0%B5%D0%B5-%D1%83%D0%BF%D1%80%D0%B0%D0%B2%D0%BB%D0%B5%D0%BD%D0%B8%D0%B5-%D1%81%D0%B2%D0%B5%D1%82%D0%BE%D0%B4%D0%B8%D0%BE%D0%B4%D0%B0%D0%BC%D0%B8.md
    /// </remarks>
    internal static class FreeJoyReport
    {
        /// <summary>
        /// Report ID of the external LED output report.
        /// </summary>
        public const byte ReportId = 6;

        /// <summary>
        /// Length of the bitmask, as defined by the protocol.
        /// </summary>
        public const int BitmaskLength = 4;

        /// <summary>
        /// Number of LEDs the firmware can address. The bitmask has room for 32 bits,
        /// the firmware only acts on the first 24.
        /// </summary>
        public const int MaxLedCount = 24;

        /// <summary>
        /// Turns the current output device states into the LED bitmask.
        /// </summary>
        /// <param name="state">
        /// The output devices. Outputs addressing an LED beyond <see cref="MaxLedCount"/>
        /// are ignored rather than silently wrapping into another LED.
        /// </param>
        /// <returns>The bitmask, without the leading report ID byte.</returns>
        public static byte[] FromOutputDeviceState(IEnumerable<JoystickOutputDevice> state)
        {
            var bitmask = new byte[BitmaskLength];

            if (state == null) return bitmask;

            foreach (var light in state)
            {
                if (light == null || light.State == 0) continue;
                if (light.Bit > 7) continue;
                if (light.Byte >= BitmaskLength) continue;
                if (light.Byte * 8 + light.Bit >= MaxLedCount) continue;

                bitmask[light.Byte] |= (byte)(1 << light.Bit);
            }

            return bitmask;
        }
    }
}
