using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingNwsDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
        }

        private WinwingNwsDevice CreateDevice(string variantName)
        {
            return new WinwingNwsDevice(mockMessageSender, variantName);
        }

        #region Basic Properties Tests

        [TestMethod]
        [DataRow("NWS Left",  "WinWing NWS Left")]
        [DataRow("NWS Right", "WinWing NWS Right")]
        public void Name_PrefixesWinWingToVariantName(string variantName, string expectedName)
        {
            var device = CreateDevice(variantName);
            Assert.AreEqual(expectedName, device.Name);
        }

        [TestMethod]
        public void GetDisplayNames_ReturnsEmptyList()
        {
            var device = CreateDevice("NWS Left");
            Assert.IsEmpty(device.GetDisplayNames());
        }

        [TestMethod]
        public void GetInternalDisplayNames_ReturnsEmptyList()
        {
            var device = CreateDevice("NWS Left");
            Assert.IsEmpty(device.GetInternalDisplayNames());
        }

        [TestMethod]
        public void GetLedNames_ContainsOnlyBacklightChannel()
        {
            var device = CreateDevice("NWS Left");
            var ledNames = device.GetLedNames();

            Assert.HasCount(1, ledNames);
            Assert.Contains("Backlight Percentage", ledNames);
        }

        #endregion

        #region Connect / Shutdown / Stop

        [TestMethod]
        public void Connect_SetsBacklightTo50()
        {
            var device = CreateDevice("NWS Left");
            device.Connect();

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("50", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void Shutdown_SetsBacklightToZero()
        {
            var device = CreateDevice("NWS Right");
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        [TestMethod]
        public void Stop_DoesNothing()
        {
            var device = CreateDevice("NWS Left");
            device.Stop();

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
            Assert.IsEmpty(mockMessageSender.BrightnessCommands);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        [DataRow("NWS Left")]
        [DataRow("NWS Right")]
        public void Connect_RoutesBrightnessToSharedDestination(string variantName)
        {
            // Both variants share {0x60, 0xb9} (confirmed via capture, 3PDC pattern).
            var device = CreateDevice(variantName);
            device.Connect();

            var dest = mockMessageSender.BrightnessCommands[0].DestinationAddress;
            Assert.HasCount(2, dest);
            Assert.AreEqual((byte)0x60, dest[0]);
            Assert.AreEqual((byte)0xb9, dest[1]);
        }

        #endregion

        #region LED / Output Tests

        [TestMethod]
        public void SetLed_BacklightPercentage_SendsBrightnessMessage()
        {
            var device = CreateDevice("NWS Left");
            device.Connect();
            mockMessageSender.Reset();

            device.SetLed("Backlight Percentage", 80);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("80", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        [TestMethod]
        public void SetLed_SameBrightnessTwice_OnlySendsOnce()
        {
            var device = CreateDevice("NWS Left");
            device.SetLed("Backlight Percentage", 80);
            device.SetLed("Backlight Percentage", 80);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
        }

        [TestMethod]
        public void SetLed_NullOrEmptyName_SendsNothing()
        {
            var device = CreateDevice("NWS Left");
            device.SetLed(null, 1);
            device.SetLed("", 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
            Assert.IsEmpty(mockMessageSender.BrightnessCommands);
        }

        [TestMethod]
        public void SetLed_UnknownName_ThrowsKeyNotFoundException()
        {
            // SetLed indexes LedCurrentValuesCache before the dictionary lookup
            // so unknown LED names raise KeyNotFoundException.
            var device = CreateDevice("NWS Left");
            Assert.ThrowsExactly<System.Collections.Generic.KeyNotFoundException>(
                () => device.SetLed("DOES_NOT_EXIST", 1));
        }

        #endregion
    }
}
