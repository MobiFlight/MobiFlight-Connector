using System;

namespace MobiFlight.Joysticks.Logitech.Tests
{
    [TestClass]
    public class RadioPanelReportTests
    {
        #region Parse

        [TestMethod]
        public void Parse_EmptyState_HasNoActiveInputs()
        {
            // Arrange
            var payload = new byte[] { 0x00, 0x00, 0x00 };

            // Act
            var state = RadioPanelReport.Parse(payload).ToJoystickState();

            // Assert
            for (var index = 0; index < RadioPanelReport.ButtonCount; index++)
            {
                Assert.IsFalse(state.Buttons[index], $"Button {index} should be inactive.");
            }
        }

        [TestMethod]
        public void Parse_MapsEachReportBitToItsOwnButton()
        {
            for (var bit = 0; bit < RadioPanelReport.ButtonCount; bit++)
            {
                // Arrange
                var payload = new byte[RadioPanelReport.PayloadLength];
                payload[bit / 8] = (byte)(1 << (bit % 8));

                // Act
                var state = RadioPanelReport.Parse(payload).ToJoystickState();

                // Assert
                for (var index = 0; index < RadioPanelReport.ButtonCount; index++)
                {
                    var expected = index == bit;
                    Assert.AreEqual(expected, state.Buttons[index], $"Bit {bit} should only set button {bit}, button {index} differs.");
                }
            }
        }

        [TestMethod]
        public void Parse_SelectorsAndActStby_SetExpectedButtons()
        {
            // Arrange
            var payload = new byte[] { 0x81, 0x40, 0x00 };

            // Act
            var state = RadioPanelReport.Parse(payload).ToJoystickState();

            // Assert
            Assert.IsTrue(state.Buttons[0], "Upper COM1");
            Assert.IsTrue(state.Buttons[7], "Lower COM1");
            Assert.IsTrue(state.Buttons[14], "Upper ACT/STBY");
            Assert.IsFalse(state.Buttons[15], "Lower ACT/STBY");
        }

        [TestMethod]
        public void Parse_LowerOuterCcwPulse_SetsOnlyThatEncoderButton()
        {
            // Arrange
            var payload = new byte[] { 0x81, 0x00, 0x80 };

            // Act
            var state = RadioPanelReport.Parse(payload).ToJoystickState();

            // Assert
            Assert.IsTrue(state.Buttons[23]);
            Assert.IsFalse(state.Buttons[22]);
        }

        [TestMethod]
        public void Parse_PayloadShorterThanReport_Throws()
        {
            // Arrange
            var payload = new byte[] { 0x00, 0x00 };

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => RadioPanelReport.Parse(payload));
        }

        #endregion
    }
}
