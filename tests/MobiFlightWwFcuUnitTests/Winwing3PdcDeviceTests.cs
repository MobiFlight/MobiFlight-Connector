using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class Winwing3PdcDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
        }

        private Winwing3PdcDevice CreateDevice(string pdcType)
        {
            return new Winwing3PdcDevice(mockMessageSender, pdcType);
        }

        #region Basic Properties Tests

        [TestMethod]
        [DataRow("3N PDC Left",  "WinWing 3N PDC Left")]
        [DataRow("3N PDC Right", "WinWing 3N PDC Right")]
        [DataRow("3M PDC Left",  "WinWing 3M PDC Left")]
        [DataRow("3M PDC Right", "WinWing 3M PDC Right")]
        public void Name_PrefixesWinWingToPdcType(string pdcType, string expectedName)
        {
            var device = CreateDevice(pdcType);
            Assert.AreEqual(expectedName, device.Name);
        }

        [TestMethod]
        public void GetDisplayNames_ReturnsEmptyList()
        {
            var device = CreateDevice("3N PDC Left");
            Assert.IsEmpty(device.GetDisplayNames());
        }

        [TestMethod]
        public void GetInternalDisplayNames_ReturnsEmptyList()
        {
            var device = CreateDevice("3N PDC Left");
            Assert.IsEmpty(device.GetInternalDisplayNames());
        }

        [TestMethod]
        public void GetLedNames_ContainsOnlyBacklightChannel()
        {
            var device = CreateDevice("3N PDC Left");
            var ledNames = device.GetLedNames();

            Assert.HasCount(1, ledNames);
            Assert.Contains("Backlight Percentage", ledNames);
        }

        #endregion

        #region Connect / Shutdown / Stop

        [TestMethod]
        public void Connect_SetsBacklightTo50()
        {
            var device = CreateDevice("3N PDC Left");
            device.Connect();

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("50", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void Shutdown_SetsBacklightToZero()
        {
            var device = CreateDevice("3N PDC Right");
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
            var device = CreateDevice("3M PDC Left");
            device.Stop();

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
            Assert.IsEmpty(mockMessageSender.BrightnessCommands);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        [DataRow("3N PDC Left",  (byte)0x60, (byte)0xbb)]
        [DataRow("3N PDC Right", (byte)0x60, (byte)0xbb)]
        [DataRow("3M PDC Left",  (byte)0x50, (byte)0xbb)]
        [DataRow("3M PDC Right", (byte)0x50, (byte)0xbb)]
        public void Connect_RoutesBrightnessToCorrectDestination(string pdcType, byte b0, byte b1)
        {
            var device = CreateDevice(pdcType);
            device.Connect();

            var dest = mockMessageSender.BrightnessCommands[0].DestinationAddress;
            Assert.HasCount(2, dest);
            Assert.AreEqual(b0, dest[0]);
            Assert.AreEqual(b1, dest[1]);
        }

        #endregion

        #region LED / Output Tests

        [TestMethod]
        public void SetLed_BacklightPercentage_SendsBrightnessMessage()
        {
            var device = CreateDevice("3N PDC Left");
            device.Connect();
            mockMessageSender.Reset();

            device.SetLed("Backlight Percentage", 80);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("80", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        [TestMethod]
        public void SetLed_NullOrEmptyName_SendsNothing()
        {
            var device = CreateDevice("3N PDC Left");
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
            var device = CreateDevice("3N PDC Left");
            Assert.ThrowsExactly<System.Collections.Generic.KeyNotFoundException>(
                () => device.SetLed("DOES_NOT_EXIST", 1));
        }

        #endregion
    }
}
