using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class MozaTunnelTests
    {
        #region TryUnwrap

        [TestMethod]
        public void TryUnwrap_ReplyFrame_ExposesInnerCommandAndReplyBit()
        {
            // Arrange
            // "7e 05 c3 21 fc 12 34 34 12 fc" from guide §2.4 - inner command 0x7C with
            // its reply bit set, not the outer command's own reply bit (0xC3).
            var decoder = new SerialLinkDecoder();
            var raw = TestHex.Parse("7e 05 c3 21 fc 12 34 34 12 fc");

            // Act
            var messages = decoder.Feed(raw, raw.Length);
            MozaTunnel.TryUnwrap(messages[0], out var tunnelMessage);

            // Assert
            Assert.AreEqual(0x7C, tunnelMessage.InnerCommand);
            Assert.IsTrue(tunnelMessage.IsReply);
        }

        [TestMethod]
        public void TryUnwrap_RequestFrame_InnerReplyBitIsFalse()
        {
            // Arrange
            var decoder = new SerialLinkDecoder();
            var raw = TestHex.Parse("7e 01 43 12 00 e1");

            // Act
            var messages = decoder.Feed(raw, raw.Length);
            MozaTunnel.TryUnwrap(messages[0], out var tunnelMessage);

            // Assert
            Assert.AreEqual(0x00, tunnelMessage.InnerCommand);
            Assert.IsFalse(tunnelMessage.IsReply);
        }

        [TestMethod]
        public void TryUnwrap_NonTunnelOuterCommand_ReturnsFalse()
        {
            // Arrange
            var message = new SerialLinkMessage(0x80, 0x21, []);

            // Act
            var found = MozaTunnel.TryUnwrap(message, out var tunnelMessage);

            // Assert
            Assert.IsFalse(found);
            Assert.IsNull(tunnelMessage);
        }

        [TestMethod]
        public void TryUnwrap_EmptyPayload_ReturnsFalse()
        {
            // Arrange
            var message = new SerialLinkMessage(0x43, 0x12, []);

            // Act
            var found = MozaTunnel.TryUnwrap(message, out _);

            // Assert
            Assert.IsFalse(found);
        }

        #endregion

        #region Wrap validation

        [TestMethod]
        public void Wrap_InnerPayloadTooLong_Throws()
        {
            // Arrange
            var payload = new byte[MozaConstants.MaxTunnelInnerPayloadLength + 1];

            // Act & Assert
            Assert.ThrowsExactly<System.ArgumentException>(() => MozaTunnel.Wrap(0x01, payload));
        }

        #endregion
    }
}
