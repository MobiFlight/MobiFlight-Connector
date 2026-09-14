using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class MozaSettingsFrameTests
    {
        #region Golden vectors (known-good frames captured from a real device session)

        [TestMethod]
        public void PackSettingFrame_DisplayMode1_MatchesKnownGoodBytes()
        {
            // Act
            byte[] frame = MozaSettingsFrame.PackSettingFrame(0x18, [0x01]);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("ff 05 00 00 00 c8 1b b5 e1 18 00 00 00 01"), frame);
        }

        [TestMethod]
        public void PackSettingFrame_DisplayOffsetTenNegativeTwo_MatchesKnownGoodBytes()
        {
            // Arrange
            // Setting 0x1B (displayOffset): x:i32 LE + y:i32 LE = (10, -2).
            byte[] data = TestHex.Parse("0a 00 00 00 fe ff ff ff");

            // Act
            byte[] frame = MozaSettingsFrame.PackSettingFrame(0x1B, data);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("ff 0c 00 00 00 d3 d4 98 f1 1b 00 00 00 0a 00 00 00 fe ff ff ff"), frame);
        }

        [TestMethod]
        public void PackSettingFrame_TextColorRowZero_MatchesKnownGoodBytes()
        {
            // Arrange
            // Setting 0x17 (textColor): row:i32 LE=0 + ARGB=FF 11 22 33.
            byte[] data = TestHex.Parse("00 00 00 00 ff 11 22 33");

            // Act
            byte[] frame = MozaSettingsFrame.PackSettingFrame(0x17, data);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("ff 0c 00 00 00 9c 2a d6 d8 17 00 00 00 00 00 00 00 ff 11 22 33"), frame);
        }

        [TestMethod]
        public void PackTimeSync_KnownUnixSecondsAndOffset_MatchesKnownGoodBytes()
        {
            // Act
            byte[] frame = MozaSettingsFrame.PackTimeSync(1735689600, 28800);

            // Assert
            CollectionAssert.AreEqual(
                TestHex.Parse("ff 10 00 00 00 ef 8f 62 73 02 00 00 00 80 85 74 67 00 00 00 00 80 70 00 00"),
                frame);
        }

        [TestMethod]
        public void PackProtocolVersion_Three_MatchesKnownGoodBytes()
        {
            // Act
            byte[] frame = MozaSettingsFrame.PackProtocolVersion(3, 0);

            // Assert
            CollectionAssert.AreEqual(
                TestHex.Parse("ff 0c 00 00 00 03 28 c2 81 07 00 00 00 03 00 00 00 00 00 00 00"),
                frame);
        }

        #endregion

        #region Structure

        [TestMethod]
        public void PackSettingFrame_SizeEqualsFourPlusDataLength()
        {
            // Act
            byte[] frame = MozaSettingsFrame.PackSettingFrame(1, [1, 2, 3]);

            // Assert
            uint size = (uint)(frame[1] | (frame[2] << 8) | (frame[3] << 16) | (frame[4] << 24));
            Assert.AreEqual(4u + 3u, size);
        }

        [TestMethod]
        public void PackSettingFrame_NullData_TreatedAsEmpty()
        {
            // Act
            byte[] frame = MozaSettingsFrame.PackSettingFrame(1, null);

            // Assert
            Assert.AreEqual((byte)0xFF, frame[0]);
            Assert.HasCount(9 + 4, frame); // FF + Size + CRC + SettingId, no Data
        }

        #endregion
    }
}
