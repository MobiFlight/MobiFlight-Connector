using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class MozaPanelCommandTests
    {
        #region Captured frames

        [TestMethod]
        public void Encode_BoeingExecLedOn_MatchesCapturedFrame()
        {
            // Arrange
            // Frame captured from MOZA Cockpit / the SDK switching EXEC on a Boeing CDU (0x1306).
            byte[] expected = TestHex.Parse("7e 03 45 12 62 02 01 4a");

            // Act
            byte[] wire = MozaPanelCommand.Encode(MozaPanelCommand.Led((byte)MozaBoeingCduLed.Exec, true));

            // Assert
            CollectionAssert.AreEqual(expected, wire);
        }

        [TestMethod]
        public void Encode_BoeingExecLedOff_MatchesCapturedFrame()
        {
            // Arrange
            byte[] expected = TestHex.Parse("7e 03 45 12 62 02 00 49");

            // Act
            byte[] wire = MozaPanelCommand.Encode(MozaPanelCommand.Led((byte)MozaBoeingCduLed.Exec, false));

            // Assert
            CollectionAssert.AreEqual(expected, wire);
        }

        [TestMethod]
        public void Encode_KeyBacklightOff_MatchesCapturedFrame()
        {
            // Arrange
            byte[] expected = TestHex.Parse("7e 03 45 12 52 01 00 38");

            // Act
            byte[] wire = MozaPanelCommand.Encode(MozaPanelCommand.Brightness(MozaLightGroup.KeyBacklight, (int)MozaBrightness.Off));

            // Assert
            CollectionAssert.AreEqual(expected, wire);
        }

        [TestMethod]
        public void Encode_KeyBacklightLevel9_MatchesCapturedFrame()
        {
            // Arrange
            byte[] expected = TestHex.Parse("7e 03 45 12 52 01 09 41");

            // Act
            byte[] wire = MozaPanelCommand.Encode(MozaPanelCommand.Brightness(MozaLightGroup.KeyBacklight, 9));

            // Assert
            CollectionAssert.AreEqual(expected, wire);
        }

        #endregion

        #region Payloads

        [TestMethod]
        public void Led_McduMenuOn_AddressesIndex7()
        {
            // Act
            byte[] payload = MozaPanelCommand.Led((byte)MozaMcduLed.McduMenu, true);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x62, 0x07, 0x01 }, payload);
        }

        [TestMethod]
        public void LightMode_AnnunciatorsManual_BuildsModePayload()
        {
            // Act
            byte[] payload = MozaPanelCommand.LightMode(MozaLightGroup.Annunciators, MozaLightMode.Manual);

            // Assert
            CollectionAssert.AreEqual(new byte[] { 0x51, 0x03, 0x00 }, payload);
        }

        [TestMethod]
        public void Brightness_AboveMaximum_IsClampedToFull()
        {
            // Act
            byte[] payload = MozaPanelCommand.Brightness(MozaLightGroup.All, 200);

            // Assert
            Assert.AreEqual((byte)MozaBrightness.Full, payload[2]);
        }

        [TestMethod]
        public void Brightness_BelowZero_IsClampedToOff()
        {
            // Act
            byte[] payload = MozaPanelCommand.Brightness(MozaLightGroup.Display, -5);

            // Assert
            Assert.AreEqual((byte)MozaBrightness.Off, payload[2]);
        }

        #endregion

        #region TargetKey

        [TestMethod]
        public void TargetKey_SameLedDifferentState_IsEqual()
        {
            // Act
            int on = MozaPanelCommand.TargetKey(MozaPanelCommand.Led((byte)MozaMcduLed.Fail, true));
            int off = MozaPanelCommand.TargetKey(MozaPanelCommand.Led((byte)MozaMcduLed.Fail, false));

            // Assert
            Assert.AreEqual(on, off);
        }

        [TestMethod]
        public void TargetKey_DifferentLeds_Differ()
        {
            // Act
            int fail = MozaPanelCommand.TargetKey(MozaPanelCommand.Led((byte)MozaMcduLed.Fail, true));
            int fm = MozaPanelCommand.TargetKey(MozaPanelCommand.Led((byte)MozaMcduLed.Fm, true));

            // Assert
            Assert.AreNotEqual(fail, fm);
        }

        #endregion
    }
}
