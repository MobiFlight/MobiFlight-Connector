namespace MobiFlightMoza.Protocol
{
    /// <summary>One decoded settings value, whichever wire format it arrived in.</summary>
    internal readonly struct SettingFrame
    {
        public int SettingId { get; }
        public byte[] Data { get; }

        public SettingFrame(int settingId, byte[] data)
        {
            SettingId = settingId;
            Data = data ?? [];
        }
    }
}
