using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class SettingsFrameExtractorTests
    {
        #region One-time legacy current-value block

        [TestMethod]
        public void Feed_LegacyCurrentValueBlock_ExtractsAllFourInOrder()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            extractor.ExpectLegacyCurrentValues();
            // 0x01(4 bytes) + 0x05(1) + 0x04(8) + 0x06(1)
            byte[] block = TestHex.Parse("01 32 00 00 00 05 01 04 01 00 00 00 02 00 00 00 06 01");

            // Act
            var frames = extractor.Feed(block);

            // Assert
            Assert.HasCount(4, frames);
            Assert.AreEqual(0x01, frames[0].SettingId);
            Assert.AreEqual(0x05, frames[1].SettingId);
            Assert.AreEqual(0x04, frames[2].SettingId);
            Assert.AreEqual(0x06, frames[3].SettingId);
        }

        [TestMethod]
        public void Feed_LegacyCurrentValueBlockSplitAcrossReads_WaitsThenExtracts()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            extractor.ExpectLegacyCurrentValues();
            byte[] block = TestHex.Parse("01 32 00 00 00 05 01 04 01 00 00 00 02 00 00 00 06 01");

            // Act
            var firstChunk = extractor.Feed(block[..5]);
            var secondChunk = extractor.Feed(block[5..]);

            // Assert
            Assert.IsEmpty(firstChunk);
            Assert.HasCount(4, secondChunk);
        }

        [TestMethod]
        public void Feed_ExpectingLegacyBlockButFirstByteDoesNotMatch_GivesUpImmediately()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            extractor.ExpectLegacyCurrentValues();
            // Goes straight to a new-style 0xFF frame instead of the legacy block.
            byte[] data = MozaSettingsFrame.PackSettingFrame(0x13, [0]);

            // Act
            var frames = extractor.Feed(data);

            // Assert - not blocked waiting for a legacy block that never comes
            Assert.HasCount(1, frames);
            Assert.AreEqual(0x13, frames[0].SettingId);
        }

        #endregion

        #region Ongoing legacy-framed echoes

        [TestMethod]
        public void Feed_LegacyBrightnessEcho_ExtractsWithoutFraming()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            byte[] data = TestHex.Parse("01 32 00 00 00"); // id=0x01, 4-byte value=50

            // Act
            var frames = extractor.Feed(data);

            // Assert
            Assert.HasCount(1, frames);
            Assert.AreEqual(0x01, frames[0].SettingId);
            CollectionAssert.AreEqual(TestHex.Parse("32 00 00 00"), frames[0].Data);
        }

        #endregion

        #region New-style FF-framed settings

        [TestMethod]
        public void Feed_NewStyleFrame_ExtractsSettingIdAndData()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            byte[] data = MozaSettingsFrame.PackSettingFrame(0x18, [1]);

            // Act
            var frames = extractor.Feed(data);

            // Assert
            Assert.HasCount(1, frames);
            Assert.AreEqual(0x18, frames[0].SettingId);
            CollectionAssert.AreEqual(new byte[] { 1 }, frames[0].Data);
        }

        [TestMethod]
        public void Feed_GarbageBeforeFrameStart_SkipsToNextFF()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            byte[] data = [0xAA, 0xBB, .. MozaSettingsFrame.PackSettingFrame(0x18, [1])];

            // Act
            var frames = extractor.Feed(data);

            // Assert
            Assert.HasCount(1, frames);
        }

        [TestMethod]
        public void Feed_CorruptedCrc_Throws()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            byte[] data = MozaSettingsFrame.PackSettingFrame(0x18, [1]);
            data[5] ^= 0xFF; // flip a CRC byte

            // Act & Assert
            Assert.ThrowsExactly<System.InvalidOperationException>(() => extractor.Feed(data));
        }

        [TestMethod]
        public void Feed_MixOfLegacyAndNewStyleFrames_ExtractsBoth()
        {
            // Arrange
            var extractor = new SettingsFrameExtractor();
            byte[] data = [.. TestHex.Parse("01 32 00 00 00"), .. MozaSettingsFrame.PackSettingFrame(0x18, [1])];

            // Act
            var frames = extractor.Feed(data);

            // Assert
            Assert.HasCount(2, frames);
            Assert.AreEqual(0x01, frames[0].SettingId);
            Assert.AreEqual(0x18, frames[1].SettingId);
        }

        #endregion
    }
}
