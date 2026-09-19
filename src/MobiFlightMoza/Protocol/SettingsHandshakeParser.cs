using System;
using System.Collections.Generic;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Parses the fixed preamble the device sends right after the 9020 connection is
    /// established: <c>07 01 00 00 00 | 00 | McuIdLength | SettingCount | McuIdBytes |
    /// SupportedSettingIds...</c>. May arrive split across several reads.
    /// </summary>
    internal static class SettingsHandshakeParser
    {
        private static readonly byte[] Header = [0x07, 0x01, 0x00, 0x00, 0x00];

        public static bool TryParse(List<byte> buffer, out byte[] mcuId, out IReadOnlyList<byte> supportedSettingIds, out int consumed)
        {
            mcuId = null;
            supportedSettingIds = null;
            consumed = 0;

            if (buffer.Count < 8) return false;

            for (int i = 0; i < Header.Length; i++)
            {
                if (buffer[i] != Header[i])
                {
                    throw new InvalidOperationException("Unexpected settings preamble header.");
                }
            }
            if (buffer[5] != 0x00)
            {
                throw new InvalidOperationException($"Unexpected settings handshake marker 0x{buffer[5]:X2}.");
            }

            int mcuIdLength = buffer[6];
            int settingCount = buffer[7];
            int total = 8 + mcuIdLength + settingCount;
            if (buffer.Count < total) return false;

            mcuId = [.. buffer.GetRange(8, mcuIdLength)];
            supportedSettingIds = buffer.GetRange(8 + mcuIdLength, settingCount);
            consumed = total;
            return true;
        }
    }
}
