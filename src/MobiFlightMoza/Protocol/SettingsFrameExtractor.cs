using System;
using System.Collections.Generic;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Buffered extraction of settings values from the 9020 byte stream. Three formats
    /// coexist there:
    /// <list type="bullet">
    /// <item>a one-time fixed-order legacy block sent right after the settings handshake
    /// preamble - four values with no framing at all, just their raw bytes in sequence;</item>
    /// <item>ongoing legacy-framed echoes of those same four settings (one leading ID byte,
    /// then the value's fixed-size data, no length/CRC);</item>
    /// <item>the general <c>0xFF | Size | CRC32 | SettingId | Data</c> frame everything
    /// else (and post-v3 echoes of these same four IDs) uses.</item>
    /// </list>
    /// </summary>
    internal sealed class SettingsFrameExtractor
    {
        private const uint MaxPayloadSize = 16 * 1024 * 1024;

        // (SettingId, DataSize), in the fixed order the one-time legacy block sends them.
        private static readonly (int Id, int Size)[] LegacyCurrentValueFields =
        [
            (0x01, 4), (0x05, 1), (0x04, 8), (0x06, 1),
        ];

        private static readonly Dictionary<int, int> LegacySettingFieldSizes = new()
        {
            [0x01] = 4,
            [0x05] = 1,
            [0x04] = 8,
            [0x06] = 1,
        };

        private readonly List<byte> Buffer = [];
        private bool LegacyCurrentValuesPending;

        // Set once, before the next Feed, when the one-time legacy current-value block is
        // expected next - immediately after the settings handshake preamble.
        public void ExpectLegacyCurrentValues() => LegacyCurrentValuesPending = true;

        public IReadOnlyList<SettingFrame> Feed(byte[] data)
        {
            Buffer.AddRange(data);
            List<SettingFrame> frames = [];

            if (LegacyCurrentValuesPending)
            {
                if (!TryExtractLegacyCurrentValueBlock(frames)) return frames; // wait for more data
                LegacyCurrentValuesPending = false;
            }

            ExtractLegacySettingFrames(frames);
            ExtractNewStyleSettingFrames(frames);
            return frames;
        }

        // Returns false only while genuinely waiting on more bytes for a block that has
        // already started. A non-matching first byte gives up immediately instead of
        // waiting forever, since the block might simply never come.
        private bool TryExtractLegacyCurrentValueBlock(List<SettingFrame> frames)
        {
            if (Buffer.Count == 0 || Buffer[0] != LegacyCurrentValueFields[0].Id)
            {
                return true;
            }

            int total = 0;
            foreach (var (_, size) in LegacyCurrentValueFields) total += 1 + size;
            if (Buffer.Count < total) return false;

            int offset = 0;
            List<SettingFrame> extracted = [];
            foreach (var (id, size) in LegacyCurrentValueFields)
            {
                if (Buffer[offset] != id)
                {
                    return true; // malformed - abandon the block rather than misdeliver
                }
                offset++;
                extracted.Add(new SettingFrame(id, [.. Buffer.GetRange(offset, size)]));
                offset += size;
            }

            Buffer.RemoveRange(0, total);
            frames.AddRange(extracted);
            return true;
        }

        private void ExtractLegacySettingFrames(List<SettingFrame> frames)
        {
            while (Buffer.Count > 0)
            {
                int settingId = Buffer[0];
                if (!LegacySettingFieldSizes.TryGetValue(settingId, out int size)) break;
                if (Buffer.Count < 1 + size) return; // wait for the rest

                byte[] frameData = [.. Buffer.GetRange(1, size)];
                Buffer.RemoveRange(0, 1 + size);
                frames.Add(new SettingFrame(settingId, frameData));
            }
        }

        private void ExtractNewStyleSettingFrames(List<SettingFrame> frames)
        {
            while (Buffer.Count > 0)
            {
                if (Buffer[0] != 0xFF)
                {
                    int start = Buffer.IndexOf(0xFF);
                    if (start < 0) { Buffer.Clear(); return; }
                    Buffer.RemoveRange(0, start);
                }
                if (Buffer.Count < 9) return;

                uint size = (uint)(Buffer[1] | (Buffer[2] << 8) | (Buffer[3] << 16) | (Buffer[4] << 24));
                uint receivedCrc = (uint)(Buffer[5] | (Buffer[6] << 8) | (Buffer[7] << 16) | (Buffer[8] << 24));
                if (size < 4 || size > MaxPayloadSize)
                {
                    Buffer.RemoveAt(0); // not a real frame start - drop one byte and resync
                    continue;
                }

                int total = 9 + (int)size;
                if (Buffer.Count < total) return;

                byte[] body = [.. Buffer.GetRange(9, (int)size)];
                Buffer.RemoveRange(0, total);

                uint expectedCrc = Crc32.Compute(body);
                if (receivedCrc != expectedCrc)
                {
                    throw new InvalidOperationException(
                        $"Settings frame CRC mismatch: received 0x{receivedCrc:X8}, expected 0x{expectedCrc:X8}.");
                }

                int settingId = body[0] | (body[1] << 8) | (body[2] << 16) | (body[3] << 24);
                frames.Add(new SettingFrame(settingId, body[4..]));
            }
        }
    }
}
