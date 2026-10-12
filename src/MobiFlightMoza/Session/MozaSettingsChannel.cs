using System;
using System.Collections.Generic;
using System.Linq;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Session
{
    /// <summary>
    /// Drives the 9020 settings channel once its Reliable Stream connection is
    /// established: waits for the handshake preamble, sends time sync + protocol
    /// version, then collects reported settings for a bounded window before declaring
    /// itself ready. Settings are available incrementally as they arrive; <see cref="Ready"/>
    /// just means the collection window has closed, not that every setting showed up.
    /// </summary>
    internal sealed class MozaSettingsChannel
    {
        private const double CollectionWindowSeconds = 4.0;

        private readonly ReliableStreamMultiplexer Multiplexer;
        private readonly List<byte> PreambleBuffer = [];
        private readonly SettingsFrameExtractor Extractor = new();
        private readonly Dictionary<int, byte[]> SettingsById = [];

        private bool PastPreamble;
        private DateTime? CollectionDeadline;

        public event Action Ready;
        public event Action<int, byte[]> SettingEchoed;

        public IReadOnlyDictionary<int, byte[]> Settings => SettingsById;
        public byte? CabinPosition => FirstByteOf(0x13);
        public byte? DisplayMode => FirstByteOf(0x18);

        public MozaSettingsChannel(ReliableStreamMultiplexer multiplexer)
        {
            Multiplexer = multiplexer;
        }

        public void OnApplicationData(byte[] data, DateTime now)
        {
            if (!PastPreamble)
            {
                PreambleBuffer.AddRange(data);
                if (!SettingsHandshakeParser.TryParse(PreambleBuffer, out _, out _, out int consumed)) return;

                byte[] remainder = [.. PreambleBuffer.Skip(consumed)];
                PreambleBuffer.Clear();
                PastPreamble = true;
                Extractor.ExpectLegacyCurrentValues();
                CollectionDeadline = now.AddSeconds(CollectionWindowSeconds);

                SendTimeSyncAndVersion(now);
                if (remainder.Length > 0) ProcessSettingsBytes(remainder);
                return;
            }

            ProcessSettingsBytes(data);
            CheckCollectionDeadline(now);
        }

        public void Tick(DateTime now) => CheckCollectionDeadline(now);

        public void RequestSetting(int settingId, byte[] data)
            => Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackSettingFrame(settingId, data));

        private void SendTimeSyncAndVersion(DateTime now)
        {
            // UTC offset left at 0 - only affects the device's own clock display, not MCDU
            // rendering, so it's not worth plumbing the real local timezone through here.
            long unixSeconds = ((DateTimeOffset)now).ToUnixTimeSeconds();
            Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackTimeSync(unixSeconds, 0));
            Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackProtocolVersion(3, 0));
        }

        private void ProcessSettingsBytes(byte[] data)
        {
            foreach (var frame in Extractor.Feed(data))
            {
                SettingsById[frame.SettingId] = frame.Data;
                SettingEchoed?.Invoke(frame.SettingId, frame.Data);
            }
        }

        private void CheckCollectionDeadline(DateTime now)
        {
            if (CollectionDeadline.HasValue && now >= CollectionDeadline.Value)
            {
                CollectionDeadline = null;
                Ready?.Invoke();
            }
        }

        private byte? FirstByteOf(int settingId)
            => SettingsById.TryGetValue(settingId, out byte[] data) && data.Length > 0 ? data[0] : null;
    }
}
