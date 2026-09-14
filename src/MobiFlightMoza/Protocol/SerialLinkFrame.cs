using System;
using System.Collections.Generic;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Encodes a SerialLink frame (guide §3.1/3.2): SOF, Length, Command, DevicePair,
    /// Payload, Checksum, with 0x7E escaped to 7E 7E from Length through Checksum.
    /// </summary>
    internal static class SerialLinkFrame
    {
        public static byte[] EncodeRaw(byte command, byte devicePair, byte[] payload)
        {
            payload ??= [];
            if (payload.Length > MozaConstants.MaxSerialPayloadLength)
            {
                throw new ArgumentException(
                    $"Payload exceeds the {MozaConstants.MaxSerialPayloadLength}-byte protocol maximum.",
                    nameof(payload));
            }

            byte[] semantic = [(byte)payload.Length, command, devicePair, .. payload];

            List<byte> transmitted = [MozaConstants.StartOfFrame];
            int sum = MozaConstants.ChecksumSeed;
            foreach (byte b in semantic)
            {
                AppendEscaped(transmitted, ref sum, b);
            }

            byte checksum = (byte)(sum & 0xFF);
            transmitted.Add(checksum);
            if (checksum == MozaConstants.StartOfFrame)
            {
                transmitted.Add(checksum);
            }

            return [.. transmitted];
        }

        // Escaped 0x7E bytes are transmitted twice and counted twice in the checksum sum.
        private static void AppendEscaped(List<byte> transmitted, ref int sum, byte value)
        {
            transmitted.Add(value);
            sum += value;
            if (value == MozaConstants.StartOfFrame)
            {
                transmitted.Add(value);
                sum += value;
            }
        }
    }
}
