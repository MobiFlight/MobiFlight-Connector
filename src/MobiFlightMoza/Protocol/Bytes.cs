using System.Collections.Generic;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Little/big-endian integer pack and read helpers, so wire-format code reads as
    /// "field:type LE/BE" instead of hand-rolled shift-and-mask sequences. Reads take
    /// <see cref="IReadOnlyList{T}"/> so callers can read directly out of a growable
    /// <see cref="List{T}"/> receive buffer without copying it to an array first.
    /// </summary>
    internal static class Bytes
    {
        public static byte[] U16Le(ushort value) => [(byte)value, (byte)(value >> 8)];
        public static byte[] U16Be(ushort value) => [(byte)(value >> 8), (byte)value];

        public static byte[] U32Le(uint value) =>
            [(byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24)];
        public static byte[] U32Be(uint value) =>
            [(byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value];

        public static byte[] U64Le(ulong value) =>
        [
            (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24),
            (byte)(value >> 32), (byte)(value >> 40), (byte)(value >> 48), (byte)(value >> 56),
        ];

        public static ushort ReadU16Le(IReadOnlyList<byte> data, int offset) => (ushort)(data[offset] | (data[offset + 1] << 8));
        public static ushort ReadU16Be(IReadOnlyList<byte> data, int offset) => (ushort)((data[offset] << 8) | data[offset + 1]);

        public static uint ReadU32Le(IReadOnlyList<byte> data, int offset) =>
            (uint)(data[offset] | (data[offset + 1] << 8) | (data[offset + 2] << 16) | (data[offset + 3] << 24));
        public static uint ReadU32Be(IReadOnlyList<byte> data, int offset) =>
            (uint)((data[offset] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | data[offset + 3]);
    }
}
