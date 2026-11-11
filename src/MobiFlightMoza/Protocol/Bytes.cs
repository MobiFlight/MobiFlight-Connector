using System;
using System.Buffers.Binary;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Little/big-endian integer pack and read helpers, so wire-format code reads as
    /// "field:type LE/BE" instead of calling <see cref="BinaryPrimitives"/> directly at
    /// every call site. Reads take <see cref="ReadOnlySpan{T}"/> so callers can read
    /// directly out of a growable <see cref="System.Collections.Generic.List{T}"/> receive
    /// buffer (via <see cref="System.Runtime.InteropServices.CollectionsMarshal.AsSpan"/>)
    /// without copying it to an array first.
    /// </summary>
    internal static class Bytes
    {
        public static byte[] U16Le(ushort value)
        {
            byte[] buf = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(buf, value);
            return buf;
        }

        public static byte[] U16Be(ushort value)
        {
            byte[] buf = new byte[2];
            BinaryPrimitives.WriteUInt16BigEndian(buf, value);
            return buf;
        }

        public static byte[] U32Le(uint value)
        {
            byte[] buf = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(buf, value);
            return buf;
        }

        public static byte[] U32Be(uint value)
        {
            byte[] buf = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buf, value);
            return buf;
        }

        public static byte[] U64Le(ulong value)
        {
            byte[] buf = new byte[8];
            BinaryPrimitives.WriteUInt64LittleEndian(buf, value);
            return buf;
        }

        public static ushort ReadU16Le(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadUInt16LittleEndian(data[offset..]);
        public static ushort ReadU16Be(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadUInt16BigEndian(data[offset..]);

        public static uint ReadU32Le(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data[offset..]);
        public static uint ReadU32Be(ReadOnlySpan<byte> data, int offset) => BinaryPrimitives.ReadUInt32BigEndian(data[offset..]);
    }
}
