using System;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// The CDU's lighting groups ("strips") as addressed by the panel commands. Cockpit calls
    /// them Backlight, Display and Announciators; <see cref="All"/> addresses every group.
    /// </summary>
    public enum MozaLightGroup : byte
    {
        All = 0,
        KeyBacklight = 1,
        Display = 2,
        Annunciators = 3,
    }

    /// <summary>
    /// Light mode of a lighting group. Brightness values are only applied in <see cref="Manual"/>;
    /// in the other modes MOZA Cockpit streams the brightness itself.
    /// </summary>
    public enum MozaLightMode : byte
    {
        Manual = 0,
        Telemetry = 1,
        Device = 2,
    }

    /// <summary>
    /// Brightness levels of a lighting group (the Cockpit slider range is 0..15). Any value in
    /// between is valid as well.
    /// </summary>
    public enum MozaBrightness : byte
    {
        Off = 0,
        Half = 8,
        Full = 15,
    }

    /// <summary>LED indices of the Airbus MCDU (PID 0x1305) for <c>SetLed</c>.</summary>
    public enum MozaMcduLed : byte
    {
        Fm1 = 0,
        Ind = 1,
        Rdy = 2,
        Dash = 3,
        Fm2 = 4,
        Fail = 5,
        Fm = 6,
        // One LED, labelled "MCDU MENU".
        McduMenu = 7,
    }

    /// <summary>LED indices of the Boeing CDU (PID 0x1306) for <c>SetLed</c>.</summary>
    public enum MozaBoeingCduLed : byte
    {
        Call = 0,
        Fail = 1,
        Exec = 2,
        Msg = 3,
        Ofst = 4,
    }

    /// <summary>
    /// Plain SerialLink commands to the CDU's control MCU (outside the 0x43 tunnel), as captured
    /// from MOZA Cockpit / the MOZA SDK: <c>7E | len | 0x45 | 0x12 | payload | checksum</c>, acked
    /// by the device with an empty <c>0xC5</c> reply. Group/index layout per payload:
    /// <c>[51 group mode]</c> light mode, <c>[52 group 0..15]</c> brightness,
    /// <c>[62 index on]</c> one LED.
    /// </summary>
    public static class MozaPanelCommand
    {
        public const byte WriteCommand = 0x45;
        public const int MaxBrightness = 15;

        private const byte LightModeGroup = 0x51;
        private const byte BrightnessGroup = 0x52;
        private const byte LedGroup = 0x62;

        public static byte[] LightMode(MozaLightGroup group, MozaLightMode mode)
        {
            return [LightModeGroup, (byte)group, (byte)mode];
        }

        public static byte[] Brightness(MozaLightGroup group, int level)
        {
            return [BrightnessGroup, (byte)group, (byte)Math.Clamp(level, 0, MaxBrightness)];
        }

        public static byte[] Led(byte index, bool on)
        {
            return [LedGroup, index, (byte)(on ? 1 : 0)];
        }

        /// <summary>Key that identifies what a payload controls, so only the latest value per target is kept.</summary>
        public static int TargetKey(byte[] payload)
        {
            return (payload[0] << 8) | payload[1];
        }

        public static byte[] Encode(byte[] payload)
        {
            return SerialLinkFrame.EncodeRaw(WriteCommand, MozaConstants.DevicePairToDevice, payload);
        }
    }
}
