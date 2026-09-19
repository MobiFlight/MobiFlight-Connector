using System;
using System.Collections.Generic;

namespace MobiFlightMoza.Protocol
{
    /// <summary>One decoded NetworkPackage: PackageId:u8 | PayloadSize:u32 LE | Payload.</summary>
    internal readonly struct NetworkPackage
    {
        public byte PackageId { get; }
        public byte[] Payload { get; }

        public NetworkPackage(byte packageId, byte[] payload)
        {
            PackageId = packageId;
            Payload = payload ?? [];
        }

        public static byte[] Pack(byte packageId, byte[] payload)
        {
            payload ??= [];
            // PackageId:u8 | PayloadSize:u32 LE | Payload
            return [packageId, .. Bytes.U32Le((uint)payload.Length), .. payload];
        }
    }

    /// <summary>
    /// Buffered NetworkPackage extraction: one Reliable Stream read may contain only part
    /// of a package, or several, so this only yields a package once its 5-byte header and
    /// full payload have both arrived.
    /// </summary>
    internal sealed class NetworkPackageExtractor
    {
        private const uint MaxPayloadSize = 16 * 1024 * 1024;

        private readonly List<byte> Buffer = [];

        public IReadOnlyList<NetworkPackage> Feed(byte[] data)
        {
            Buffer.AddRange(data);

            List<NetworkPackage> packages = [];
            while (Buffer.Count >= 5)
            {
                uint size = Bytes.ReadU32Le(Buffer, 1);
                if (size > MaxPayloadSize)
                {
                    throw new InvalidOperationException($"NetworkPackage payload size {size} exceeds the sanity limit.");
                }

                int total = 5 + (int)size;
                if (Buffer.Count < total) break;

                byte packageId = Buffer[0];
                byte[] payload = [.. Buffer.GetRange(5, (int)size)];
                Buffer.RemoveRange(0, total);
                packages.Add(new NetworkPackage(packageId, payload));
            }
            return packages;
        }
    }
}
