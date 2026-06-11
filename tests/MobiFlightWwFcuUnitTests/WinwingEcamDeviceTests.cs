using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingEcamDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;
        private WinwingEcamDevice device = null!;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
            device = new WinwingEcamDevice(mockMessageSender);
        }

        [TestCleanup]
        public void Cleanup()
        {
            device?.Stop();
        }

        #region Basic Properties Tests

        [TestMethod]
        public void Name_ReturnsCorrectDeviceName()
        {
            Assert.AreEqual("WinWing ECAM", device.Name);
        }

        [TestMethod]
        public void GetDisplayNames_ReturnsEmptyList()
        {
            Assert.IsEmpty(device.GetDisplayNames());
        }

        [TestMethod]
        public void GetInternalDisplayNames_ReturnsEmptyList()
        {
            Assert.IsEmpty(device.GetInternalDisplayNames());
        }

        [TestMethod]
        public void GetLedNames_ContainsAll15LedsAnd2BrightnessChannels()
        {
            var ledNames = device.GetLedNames();
            Assert.IsNotNull(ledNames);
            Assert.HasCount(17, ledNames);

            // 15 hardware LEDs
            Assert.Contains("EMER_CANC", ledNames);
            Assert.Contains("ENG", ledNames);
            Assert.Contains("BLEED", ledNames);
            Assert.Contains("PRESS", ledNames);
            Assert.Contains("ELEC", ledNames);
            Assert.Contains("HYD", ledNames);
            Assert.Contains("FUEL", ledNames);
            Assert.Contains("APU", ledNames);
            Assert.Contains("COND", ledNames);
            Assert.Contains("DOOR", ledNames);
            Assert.Contains("WHEEL", ledNames);
            Assert.Contains("FCTL", ledNames);
            Assert.Contains("CLR_L", ledNames);
            Assert.Contains("STS", ledNames);
            Assert.Contains("CLR_R", ledNames);

            // 2 brightness channels exposed as outputs
            Assert.Contains("Backlight Percentage", ledNames);
            Assert.Contains("LED Percentage", ledNames);
        }

        #endregion

        #region Connect / Shutdown / Stop

        [TestMethod]
        public void Connect_SetsBacklightTo50()
        {
            device.Connect();

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("50", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void Shutdown_TurnsOffBacklightAndAllLeds()
        {
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            // Backlight => 0
            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[0].Brightness);

            // 15 LEDs => off
            Assert.HasCount(15, mockMessageSender.LightControlCommands);
            foreach (var lc in mockMessageSender.LightControlCommands)
            {
                Assert.AreEqual((byte)0, lc.Value);
            }
        }

        [TestMethod]
        public void Stop_TurnsOffAll15LedsAndSendsNoDisplayFrames()
        {
            device.Stop();

            Assert.HasCount(15, mockMessageSender.LightControlCommands);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region LED Tests

        [TestMethod]
        public void SetLed_EmerCanc_On_SendsLightControlMessage()
        {
            device.SetLed("EMER_CANC", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x03, mockMessageSender.LightControlCommands[0].Type);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_ClrR_OffAfterOn_SendsZeroValue()
        {
            device.SetLed("CLR_R", 1);
            device.SetLed("CLR_R", 0);

            Assert.HasCount(2, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x11, mockMessageSender.LightControlCommands[1].Type);
            Assert.AreEqual((byte)0, mockMessageSender.LightControlCommands[1].Value);
        }

        [TestMethod]
        public void SetLed_Hyd_NonZeroValueIsAdjustedToOne()
        {
            device.SetLed("HYD", 200);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x08, mockMessageSender.LightControlCommands[0].Type);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_SameStateTwice_OnlySendsOnce()
        {
            device.SetLed("ENG", 1);
            device.SetLed("ENG", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_NullOrEmptyName_SendsNothing()
        {
            device.SetLed(null, 1);
            device.SetLed("", 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_UnknownName_ThrowsKeyNotFoundException()
        {
            // SetLed indexes LedCurrentValuesCache before the dictionary lookup
            // so unknown LED names raise KeyNotFoundException.
            Assert.ThrowsExactly<System.Collections.Generic.KeyNotFoundException>(
                () => device.SetLed("DOES_NOT_EXIST", 1));
        }

        #endregion

        #region Brightness Tests

        [TestMethod]
        [DataRow("Backlight Percentage", (byte)0x00)]
        [DataRow("LED Percentage",       (byte)0x01)]
        public void SetLed_BrightnessChannel_SendsBrightnessMessage(string name, byte expectedType)
        {
            device.SetLed(name, 60);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(expectedType, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("60", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        #endregion
    }
}
