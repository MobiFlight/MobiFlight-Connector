using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingPto2DeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;
        private WinwingPto2Device device = null!;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
            device = new WinwingPto2Device(mockMessageSender);
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
            Assert.AreEqual("WinWing PTO2", device.Name);
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
        public void GetLedNames_ContainsAll14LedsAnd4BrightnessChannels()
        {
            var ledNames = device.GetLedNames();
            Assert.IsNotNull(ledNames);
            Assert.HasCount(18, ledNames);

            // 14 hardware LEDs
            Assert.Contains("MASTER_CAUTION", ledNames);
            Assert.Contains("JETT", ledNames);
            Assert.Contains("CTR", ledNames);
            Assert.Contains("LI", ledNames);
            Assert.Contains("LO", ledNames);
            Assert.Contains("RO", ledNames);
            Assert.Contains("RI", ledNames);
            Assert.Contains("FLAPS", ledNames);
            Assert.Contains("NOSE", ledNames);
            Assert.Contains("FULL", ledNames);
            Assert.Contains("RIGHT", ledNames);
            Assert.Contains("LEFT", ledNames);
            Assert.Contains("HALF", ledNames);
            Assert.Contains("HOOK", ledNames);

            // 4 brightness channels exposed as outputs
            Assert.Contains("Backlight Percentage", ledNames);
            Assert.Contains("Landing Gear Percentage", ledNames);
            Assert.Contains("SL Percentage", ledNames);
            Assert.Contains("FLAG Percentage", ledNames);
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
        public void Shutdown_TurnsOffAllBrightnessChannelsAndAllLeds()
        {
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            // 4 brightness channels => 0
            Assert.HasCount(4, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.AreEqual(0x01, mockMessageSender.BrightnessCommands[1].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[1].Brightness);
            Assert.AreEqual(0x02, mockMessageSender.BrightnessCommands[2].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[2].Brightness);
            Assert.AreEqual(0x03, mockMessageSender.BrightnessCommands[3].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[3].Brightness);

            // 14 LEDs => off
            Assert.HasCount(14, mockMessageSender.LightControlCommands);
            foreach (var lc in mockMessageSender.LightControlCommands)
            {
                Assert.AreEqual((byte)0, lc.Value);
            }
        }

        [TestMethod]
        public void Stop_TurnsOffAll14LedsAndSendsNoDisplayFrames()
        {
            device.Stop();

            Assert.HasCount(14, mockMessageSender.LightControlCommands);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region LED Tests

        [TestMethod]
        public void SetLed_MasterCaution_On_SendsLightControlMessage()
        {
            device.SetLed("MASTER_CAUTION", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x04, mockMessageSender.LightControlCommands[0].Type);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_Hook_OffAfterOn_SendsZeroValue()
        {
            device.SetLed("HOOK", 1);
            device.SetLed("HOOK", 0);

            Assert.HasCount(2, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x11, mockMessageSender.LightControlCommands[1].Type);
            Assert.AreEqual((byte)0, mockMessageSender.LightControlCommands[1].Value);
        }

        [TestMethod]
        public void SetLed_LO_NonZeroValueIsAdjustedToOne()
        {
            device.SetLed("LO", 200);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x08, mockMessageSender.LightControlCommands[0].Type);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_SameStateTwice_OnlySendsOnce()
        {
            device.SetLed("FLAPS", 1);
            device.SetLed("FLAPS", 1);

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
        [DataRow("Backlight Percentage",     (byte)0x00)]
        [DataRow("Landing Gear Percentage",  (byte)0x01)]
        [DataRow("SL Percentage",            (byte)0x02)]
        [DataRow("FLAG Percentage",          (byte)0x03)]
        public void SetLed_BrightnessChannel_SendsBrightnessMessage(string name, byte expectedType)
        {
            device.SetLed(name, 75);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(expectedType, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("75", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        #endregion
    }
}
