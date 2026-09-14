namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Pure pack of a single settings frame: FF | Size:u32 LE | CRC32:u32 LE |
    /// SettingId:i32 LE | Data. Size = 4 + len(Data); the CRC covers SettingId+Data.
    /// </summary>
    internal static class MozaSettingsFrame
    {
        public static byte[] PackSettingFrame(int settingId, byte[] data)
        {
            data ??= [];
            byte[] body =
            [
                (byte)settingId, (byte)(settingId >> 8), (byte)(settingId >> 16), (byte)(settingId >> 24),
                .. data,
            ];

            uint size = (uint)body.Length;
            uint crc = Crc32.Compute(body);
            return
            [
                0xFF,
                (byte)size, (byte)(size >> 8), (byte)(size >> 16), (byte)(size >> 24),
                (byte)crc, (byte)(crc >> 8), (byte)(crc >> 16), (byte)(crc >> 24),
                .. body,
            ];
        }

        // Setting 0x02: time synchronization.
        public static byte[] PackTimeSync(long unixSeconds, int utcOffsetSeconds)
        {
            ulong seconds = (ulong)unixSeconds;
            byte[] data =
            [
                (byte)seconds, (byte)(seconds >> 8), (byte)(seconds >> 16), (byte)(seconds >> 24),
                (byte)(seconds >> 32), (byte)(seconds >> 40), (byte)(seconds >> 48), (byte)(seconds >> 56),
                (byte)utcOffsetSeconds, (byte)(utcOffsetSeconds >> 8), (byte)(utcOffsetSeconds >> 16), (byte)(utcOffsetSeconds >> 24),
            ];
            return PackSettingFrame(0x02, data);
        }

        // Setting 0x07: settings protocol version negotiation.
        public static byte[] PackProtocolVersion(uint version, int reserved)
        {
            byte[] data =
            [
                (byte)version, (byte)(version >> 8), (byte)(version >> 16), (byte)(version >> 24),
                (byte)reserved, (byte)(reserved >> 8), (byte)(reserved >> 16), (byte)(reserved >> 24),
            ];
            return PackSettingFrame(0x07, data);
        }
    }
}
