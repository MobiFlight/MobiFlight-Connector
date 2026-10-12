using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class QtTypesTests
    {
        #region QString

        [TestMethod]
        public void PackQString_Empty_IsFourZeroBytes()
        {
            // Act
            byte[] wire = QtTypes.PackQString("");

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("00 00 00 00"), wire);
        }

        [TestMethod]
        public void PackQString_Null_IsTreatedAsEmpty()
        {
            // Act
            byte[] wire = QtTypes.PackQString(null);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("00 00 00 00"), wire);
        }

        [TestMethod]
        public void PackQString_Ascii_UsesBigEndianUtf16WithBigEndianLengthPrefix()
        {
            // Act
            byte[] wire = QtTypes.PackQString("AB");

            // Assert
            // Length = 4 bytes (2 UTF-16BE code units), BE: 00 00 00 04, then 00 41 00 42.
            CollectionAssert.AreEqual(TestHex.Parse("00 00 00 04 00 41 00 42"), wire);
        }

        [TestMethod]
        public void TryReadQString_RoundTripsThroughPackQString()
        {
            // Arrange
            byte[] wire = QtTypes.PackQString("MOZA");

            // Act
            var parsed = QtTypes.TryReadQString(wire, 0, out string value, out int consumed);

            // Assert
            Assert.IsTrue(parsed);
            Assert.AreEqual("MOZA", value);
            Assert.AreEqual(wire.Length, consumed);
        }

        [TestMethod]
        public void TryReadQString_TruncatedLength_ReturnsFalse()
        {
            // Arrange
            byte[] wire = [0, 0];

            // Act
            var parsed = QtTypes.TryReadQString(wire, 0, out _, out _);

            // Assert
            Assert.IsFalse(parsed);
        }

        [TestMethod]
        public void TryReadQString_DeclaredLengthExceedsBuffer_ReturnsFalse()
        {
            // Arrange
            byte[] wire = TestHex.Parse("00 00 00 10 00 41"); // says 16 bytes, only 2 follow

            // Act
            var parsed = QtTypes.TryReadQString(wire, 0, out _, out _);

            // Assert
            Assert.IsFalse(parsed);
        }

        #endregion

        #region QStringList

        [TestMethod]
        public void PackQStringList_Empty_IsFourZeroBytes()
        {
            // Act
            byte[] wire = QtTypes.PackQStringList([]);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("00 00 00 00"), wire);
        }

        [TestMethod]
        public void PackQStringList_TwoEntries_PrefixesCountThenEachQString()
        {
            // Act
            byte[] wire = QtTypes.PackQStringList(["A", "B"]);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("00 00 00 02 00 00 00 02 00 41 00 00 00 02 00 42"), wire);
        }

        #endregion

        #region QByteArray

        [TestMethod]
        public void PackQByteArray_Empty_IsFourZeroBytes()
        {
            // Act
            byte[] wire = QtTypes.PackQByteArray([]);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("00 00 00 00"), wire);
        }

        [TestMethod]
        public void TryReadQByteArray_NullMarker_ReadsAsEmptyAndConsumesOnlyFourBytes()
        {
            // Arrange
            byte[] wire = [.. TestHex.Parse("ff ff ff ff"), 0xAA, 0xBB]; // trailing bytes must be left alone

            // Act
            var parsed = QtTypes.TryReadQByteArray(wire, 0, out byte[] value, out int consumed);

            // Assert
            Assert.IsTrue(parsed);
            Assert.IsEmpty(value);
            Assert.AreEqual(4, consumed);
        }

        [TestMethod]
        public void TryReadQByteArray_RoundTripsThroughPackQByteArray()
        {
            // Arrange
            byte[] wire = QtTypes.PackQByteArray([1, 2, 3]);

            // Act
            var parsed = QtTypes.TryReadQByteArray(wire, 0, out byte[] value, out int consumed);

            // Assert
            Assert.IsTrue(parsed);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, value);
            Assert.AreEqual(wire.Length, consumed);
        }

        #endregion
    }
}
