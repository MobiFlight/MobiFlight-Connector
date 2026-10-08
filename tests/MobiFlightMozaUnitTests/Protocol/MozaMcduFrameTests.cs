using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class MozaMcduFrameTests
    {
        #region Golden vectors (literal worked examples from the protocol documentation)

        [TestMethod]
        public void PackInitConfig_MatchesKnownGoodBytes()
        {
            // Act
            byte[] payload = MozaMcduFrame.PackInitConfig(McduInitConfig.Default);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("32 0c 00 00 00 02 0e 00 18 00 02 00 00 5b 23 ee 02"), payload);
        }

        [TestMethod]
        public void PackDelta_SingleLargeWhiteCharacter_MatchesKnownGoodBytes()
        {
            // Arrange
            // Row 0, column 0, "A", large (bit7 set), white (color code 7) -> style 0x87.
            List<McduTextRow> textRows = [new(0, [new McduTextSegment(0, "A")])];
            List<McduStyleRow> styleRows = [new(0, [new McduStyleSegment(0, [0x87])])];

            // Act
            byte[] payload = MozaMcduFrame.PackDelta(2, textRows, styleRows);

            // Assert
            CollectionAssert.AreEqual(
                TestHex.Parse("31 14 00 00 00 02 02 00 00 00 03 01 00 01 00 01 01 00 41 01 00 01 00 01 87"),
                payload);
        }

        #endregion

        #region ClientCapability

        [TestMethod]
        public void TryParseClientCapability_ValidPayload_ExtractsFields()
        {
            // Arrange
            byte[] payload = TestHex.Parse("02 01 33 23 01"); // version=2, caps=1, udpPort=0x2333, pageIndex=1

            // Act
            var parsed = MozaMcduFrame.TryParseClientCapability(payload, out var capability);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual((byte)2, capability.Version);
            Assert.AreEqual((byte)1, capability.Capabilities);
            Assert.AreEqual((ushort)0x2333, capability.UdpListenPort);
            Assert.AreEqual((byte)1, capability.PageIndex);
        }

        [TestMethod]
        public void TryParseClientCapability_WrongVersion_ReturnsFalse()
        {
            // Arrange
            byte[] payload = TestHex.Parse("01 00 00 00 00");

            // Act
            var parsed = MozaMcduFrame.TryParseClientCapability(payload, out _);

            // Assert
            Assert.IsFalse(parsed);
        }

        [TestMethod]
        public void TryParseClientCapability_PageIndexOutOfRange_ReturnsFalse()
        {
            // Arrange
            byte[] payload = TestHex.Parse("02 00 00 00 02");

            // Act
            var parsed = MozaMcduFrame.TryParseClientCapability(payload, out _);

            // Assert
            Assert.IsFalse(parsed);
        }

        [TestMethod]
        public void TryParseClientCapability_WrongLength_ReturnsFalse()
        {
            // Act
            var parsed = MozaMcduFrame.TryParseClientCapability([0, 0], out _);

            // Assert
            Assert.IsFalse(parsed);
        }

        #endregion

        #region Structure

        [TestMethod]
        public void PackKeyframe_ColumnLengthCountsUtf16CodeUnitsNotUtf8Bytes()
        {
            // Arrange
            // "é" is 1 UTF-16 code unit but 2 UTF-8 bytes - ColumnLength and
            // Utf8ByteLength must diverge.
            List<McduTextRow> textRows = [new(0, [new McduTextSegment(0, "é")])];

            // Act
            byte[] payload = MozaMcduFrame.PackKeyframe(1, textRows, []);

            // Body starts after PackageId(1)+Size(4)+Version(1)+Sequence(4)+Flags(1)+TextRowCount(1)
            // +Row(1)+SegmentCount(1)+StartColumn(1) = offset 15.
            byte columnLength = payload[15];
            ushort utf8Length = (ushort)(payload[16] | (payload[17] << 8));

            // Assert
            Assert.AreEqual((byte)1, columnLength);
            Assert.AreEqual((ushort)2, utf8Length);
        }

        [TestMethod]
        public void PackKeyframe_FlagsReflectPresentSections()
        {
            // Arrange
            List<McduTextRow> textRows = [new(0, [new McduTextSegment(0, "A")])];

            // Act
            byte[] payload = MozaMcduFrame.PackKeyframe(1, textRows, []);

            // Flags byte is at PackageId(1)+Size(4)+Version(1)+Sequence(4) = offset 10.
            byte flags = payload[10];

            // Assert
            Assert.AreEqual(0x01, flags & 0x01); // text present
            Assert.AreEqual(0x00, flags & 0x02); // no styles
        }

        [TestMethod]
        public void PackKeyframe_LargeRepetitiveBody_IsCompressed()
        {
            // Arrange - a full 14x24 keyframe of blank cells, the same shape as a real
            // startup page: large and highly repetitive, so it should shrink under zlib
            // rather than grow from the format's own overhead (unlike the tiny single-
            // character golden vector above, which stays uncompressed).
            List<McduTextRow> textRows = [];
            List<McduStyleRow> styleRows = [];
            for (byte row = 0; row < 14; row++)
            {
                textRows.Add(new McduTextRow(row, [new McduTextSegment(0, new string(' ', 24))]));
                styleRows.Add(new McduStyleRow(row, [new McduStyleSegment(0, new byte[24])]));
            }

            // Act
            byte[] payload = MozaMcduFrame.PackKeyframe(1, textRows, styleRows);

            // Flags byte is at offset 10; a 4-byte big-endian uncompressed length and the
            // zlib stream follow immediately when compression is used.
            byte flags = payload[10];
            uint uncompressedLength = (uint)((payload[11] << 24) | (payload[12] << 16) | (payload[13] << 8) | payload[14]);
            byte[] compressed = payload[15..];

            using var input = new MemoryStream(compressed);
            using var zlib = new ZLibStream(input, CompressionMode.Decompress);
            using var decompressed = new MemoryStream();
            zlib.CopyTo(decompressed);
            byte[] rowsBody = decompressed.ToArray();

            // Assert
            Assert.AreEqual(0x08, flags & 0x08); // compressed
            Assert.AreEqual((uint)rowsBody.Length, uncompressedLength);
            Assert.IsLessThan(uncompressedLength, (uint)compressed.Length);
            Assert.AreEqual((byte)14, rowsBody[0]); // textRowCount: the decompressed body starts where the uncompressed layout would
        }

        #endregion
    }
}
