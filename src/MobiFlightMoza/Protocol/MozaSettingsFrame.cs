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
            // SettingId:i32 LE | Data
            byte[] body = [.. Bytes.U32Le((uint)settingId), .. data];

            uint size = (uint)body.Length;
            uint crc = Crc32.Compute(body);
            // FF | Size:u32 LE | CRC32:u32 LE | body
            return [0xFF, .. Bytes.U32Le(size), .. Bytes.U32Le(crc), .. body];
        }

        // Setting 0x02: time synchronization. UnixSeconds:u64 LE | UtcOffsetSeconds:i32 LE
        public static byte[] PackTimeSync(long unixSeconds, int utcOffsetSeconds)
            => PackSettingFrame(0x02, [.. Bytes.U64Le((ulong)unixSeconds), .. Bytes.U32Le((uint)utcOffsetSeconds)]);

        // Setting 0x07: settings protocol version negotiation. version:u32 LE | reserved:i32 LE
        public static byte[] PackProtocolVersion(uint version, int reserved)
            => PackSettingFrame(0x07, [.. Bytes.U32Le(version), .. Bytes.U32Le((uint)reserved)]);
    }
}
