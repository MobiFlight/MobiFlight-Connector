using System;
using System.Collections.Generic;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Decodes a byte stream into SerialLink frames. Keeps a persistent buffer across
    /// calls, since one read can contain a partial frame, several frames, or an arbitrary
    /// byte slice. Length-driven, not delimiter-driven: an escaped 0x7E inside a frame is
    /// never mistaken for the next frame's SOF.
    /// </summary>
    internal sealed class SerialLinkDecoder
    {
        private readonly List<byte> Buffer = [];

        public void Reset() => Buffer.Clear();

        public IReadOnlyList<SerialLinkMessage> Feed(byte[] data, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Buffer.Add(data[i]);
            }

            List<SerialLinkMessage> results = [];
            while (TryExtractOne(out var message, out int consumed))
            {
                Buffer.RemoveRange(0, consumed);
                if (message != null)
                {
                    results.Add(message);
                }
            }
            return results;
        }

        // Returns false only when the buffer holds no more decodable data right now
        // (either empty, or a partial frame still waiting on more bytes).
        private bool TryExtractOne(out SerialLinkMessage message, out int consumedRawBytes)
        {
            message = null;
            consumedRawBytes = 0;

            int sofIndex = Buffer.IndexOf(MozaConstants.StartOfFrame);
            if (sofIndex < 0)
            {
                consumedRawBytes = Buffer.Count;
                return consumedRawBytes > 0;
            }
            if (sofIndex > 0)
            {
                consumedRawBytes = sofIndex; // drop garbage before SOF
                return true;
            }

            List<byte> semantic = [];
            int rawIndex = 1;
            int neededSemanticCount = int.MaxValue; // unknown until Length (semantic[0]) is known

            while (semantic.Count < neededSemanticCount)
            {
                if (rawIndex >= Buffer.Count)
                {
                    return false; // wait for more data
                }

                byte b = Buffer[rawIndex];
                if (b == MozaConstants.StartOfFrame)
                {
                    if (rawIndex + 1 >= Buffer.Count)
                    {
                        return false; // wait to see whether this 0x7E is escaped
                    }
                    if (Buffer[rawIndex + 1] != MozaConstants.StartOfFrame)
                    {
                        // An unescaped 0x7E mid-frame: the frame we were parsing is
                        // malformed. Drop up to (not including) this byte and resync
                        // treating it as the next frame's SOF.
                        consumedRawBytes = rawIndex;
                        return true;
                    }
                    semantic.Add(MozaConstants.StartOfFrame);
                    rawIndex += 2;
                }
                else
                {
                    semantic.Add(b);
                    rawIndex += 1;
                }

                if (semantic.Count == 1)
                {
                    int length = semantic[0];
                    if (length > MozaConstants.MaxSerialPayloadLength)
                    {
                        consumedRawBytes = 1; // corrupt Length - drop the SOF and resync
                        return true;
                    }
                    neededSemanticCount = 4 + length; // Length, Command, DevicePair, Payload, Checksum
                }
            }

            byte declaredLength = semantic[0];
            byte command = semantic[1];
            byte devicePair = semantic[2];
            byte[] payload = [.. semantic.GetRange(3, declaredLength)];
            byte receivedChecksum = semantic[3 + declaredLength];

            int expectedChecksum = MozaConstants.ChecksumSeed;
            for (int i = 0; i < 3 + declaredLength; i++)
            {
                byte semanticByte = semantic[i];
                expectedChecksum += semanticByte;
                if (semanticByte == MozaConstants.StartOfFrame)
                {
                    expectedChecksum += semanticByte; // escaped byte is counted twice
                }
            }
            expectedChecksum &= 0xFF;

            if (receivedChecksum != expectedChecksum)
            {
                consumedRawBytes = 1; // drop the SOF and resync
                return true;
            }

            message = new SerialLinkMessage(command, devicePair, payload);
            consumedRawBytes = rawIndex;
            return true;
        }
    }
}
