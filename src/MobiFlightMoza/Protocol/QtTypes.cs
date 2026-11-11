using System.Collections.Generic;
using System.Text;

namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Qt's own string/binary container formats, carried inside some settings values.
    /// Unlike everything else in this protocol, these length prefixes are big-endian.
    /// </summary>
    internal static class QtTypes
    {
        public static byte[] PackQString(string value)
        {
            byte[] encoded = Encoding.BigEndianUnicode.GetBytes(value ?? "");
            return [.. Bytes.U32Be((uint)encoded.Length), .. encoded];
        }

        public static byte[] PackQStringList(IEnumerable<string> values)
        {
            List<string> list = [.. values];
            List<byte> result = [.. Bytes.U32Be((uint)list.Count)];
            foreach (string value in list)
            {
                result.AddRange(PackQString(value));
            }
            return [.. result];
        }

        // An empty QString is four zero bytes; there is no separate encoding for "absent".
        public static bool TryReadQString(byte[] data, int offset, out string value, out int consumed)
        {
            value = null;
            consumed = 0;
            if (data == null || offset < 0 || offset + 4 > data.Length) return false;

            uint length = Bytes.ReadU32Be(data, offset);
            if (offset + 4 + length > data.Length) return false;

            value = Encoding.BigEndianUnicode.GetString(data, offset + 4, (int)length);
            consumed = 4 + (int)length;
            return true;
        }

        public static byte[] PackQByteArray(byte[] data)
        {
            data ??= [];
            return [.. Bytes.U32Be((uint)data.Length), .. data];
        }

        // Qt's null-QByteArray marker (FF FF FF FF) is read as zero-length data, consuming
        // only its 4-byte length prefix - the same as a normal empty QByteArray on the wire.
        public static bool TryReadQByteArray(byte[] data, int offset, out byte[] value, out int consumed)
        {
            value = null;
            consumed = 0;
            if (data == null || offset < 0 || offset + 4 > data.Length) return false;

            uint length = Bytes.ReadU32Be(data, offset);
            if (length == 0xFFFFFFFF)
            {
                value = [];
                consumed = 4;
                return true;
            }

            if (offset + 4 + length > data.Length) return false;
            value = data[(offset + 4)..(offset + 4 + (int)length)];
            consumed = 4 + (int)length;
            return true;
        }
    }
}
