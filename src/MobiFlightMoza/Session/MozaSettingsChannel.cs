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

        public void RequestSetting(int settingId, byte[] data, DateTime now)
            => Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackSettingFrame(settingId, data));

        private void SendTimeSyncAndVersion(DateTime now)
        {
            // Purely informational for the device's own clock display - doesn't affect
            // MCDU rendering - so the UTC offset is left at 0 rather than plumbing the
            // real local timezone through a pure, clock-free class. Uses the caller's
            // `now`, never reads the clock itself. The implicit DateTime->DateTimeOffset
            // conversion resolves Local/Unspecified Kind via the system's real UTC offset,
            // and Utc Kind (as tests use) via offset zero - never throws either way.
            long unixSeconds = ((DateTimeOffset)now).ToUnixTimeSeconds();
            Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackTimeSync(unixSeconds, 0));
            Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackProtocolVersion(3, 0));

            // Empty dynamic property table (0x08) and action table (0x0B) writes - a USB
            // capture of MOZA's own Cockpit app showed it always sends these two (with no real
            // content, just four zero bytes each) right alongside time sync/protocol version,
            // even though it has no dynamic pages/actions configured. An integration that only
            // ever renders the fixed MCDU page doesn't need real content in them either way, but
            // sending them at all - matching what the one client proven to reconnect cleanly
            // does - is the experiment: maybe the device treats seeing both as "client setup is
            // complete" before it will hand off rendering to a new session.
            Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackSettingFrame(0x08, [0, 0, 0, 0]));
            Multiplexer.TrySend(MozaConstants.ServicePortSettings, MozaSettingsFrame.PackSettingFrame(0x0B, [0, 0, 0, 0]));
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
