using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingAirbusSidestickDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
        }

        private WinwingAirbusSidestickDevice CreateDevice(string stickType)
        {
            return new WinwingAirbusSidestickDevice(mockMessageSender, stickType);
        }

        #region Basic Properties Tests

        [TestMethod]
        [DataRow("Airbus Sidestick Left",  "WinWing Airbus Sidestick Left")]
        [DataRow("Airbus Sidestick Right", "WinWing Airbus Sidestick Right")]
        public void Name_PrefixesWinWingToStickType(string stickType, string expectedName)
        {
            var device = CreateDevice(stickType);
            Assert.AreEqual(expectedName, device.Name);
        }

        [TestMethod]
        public void GetDisplayNames_ReturnsEmptyList()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            Assert.IsEmpty(device.GetDisplayNames());
        }

        [TestMethod]
        public void GetInternalDisplayNames_ReturnsEmptyList()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            Assert.IsEmpty(device.GetInternalDisplayNames());
        }

        [TestMethod]
        public void GetLedNames_ContainsThreeOutputChannels()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            var ledNames = device.GetLedNames();

            Assert.HasCount(3, ledNames);
            Assert.Contains("Vibration Percentage", ledNames);
            Assert.Contains("Backlight Percentage", ledNames);
            Assert.Contains("Backlight Pulse On/Off", ledNames);
        }

        #endregion

        #region Connect / Shutdown / Stop

        [TestMethod]
        public void Connect_SetsBacklightTo20AndVibrationToZero()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            device.Connect();

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("20", mockMessageSender.BrightnessCommands[0].Brightness);

            Assert.HasCount(1, mockMessageSender.VibrationCommands);
            Assert.AreEqual((byte)0, mockMessageSender.VibrationCommands[0].Level);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void Connect_RightStick_RoutesVibrationToRightAddress()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            device.Connect();

            var dest = mockMessageSender.VibrationCommands[0].DestinationAddress;
            Assert.HasCount(2, dest);
            Assert.AreEqual((byte)0x08, dest[0]);
            Assert.AreEqual((byte)0xbf, dest[1]);
        }

        [TestMethod]
        public void Connect_LeftStick_RoutesVibrationToLeftAddress()
        {
            var device = CreateDevice("Airbus Sidestick Left");
            device.Connect();

            var dest = mockMessageSender.VibrationCommands[0].DestinationAddress;
            Assert.HasCount(2, dest);
            Assert.AreEqual((byte)0x07, dest[0]);
            Assert.AreEqual((byte)0xbf, dest[1]);
        }

        [TestMethod]
        public void Shutdown_TurnsOffBacklightAndVibration()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.HasCount(1, mockMessageSender.VibrationCommands);
            Assert.AreEqual((byte)0, mockMessageSender.VibrationCommands[0].Level);
        }

        [TestMethod]
        public void Stop_TurnsOffVibrationOnly()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            device.Stop();

            Assert.HasCount(1, mockMessageSender.VibrationCommands);
            Assert.AreEqual((byte)0, mockMessageSender.VibrationCommands[0].Level);
            Assert.IsEmpty(mockMessageSender.BrightnessCommands);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Output / LED Tests

        [TestMethod]
        public void SetLed_BacklightPercentage_SendsBrightnessMessage()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            device.Connect();
            mockMessageSender.Reset();

            device.SetLed("Backlight Percentage", 90);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("90", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        [TestMethod]
        public void SetLed_VibrationPercentage_SendsVibrationMessage()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            device.Connect();
            mockMessageSender.Reset();

            device.SetLed("Vibration Percentage", 50);

            Assert.HasCount(1, mockMessageSender.VibrationCommands);
            Assert.AreEqual((byte)50, mockMessageSender.VibrationCommands[0].Level);
        }

        [TestMethod]
        public void SetLed_BacklightPulseOn_SendsPulseLightMessage()
        {
            var device = CreateDevice("Airbus Sidestick Right");

            device.SetLed("Backlight Pulse On/Off", 1);

            Assert.HasCount(1, mockMessageSender.PulseLightCommands);
            Assert.IsTrue(mockMessageSender.PulseLightCommands[0].IsOn);
        }

        [TestMethod]
        public void SetLed_BacklightPulse_OffAfterOn_SendsTwoMessages()
        {
            var device = CreateDevice("Airbus Sidestick Right");

            device.SetLed("Backlight Pulse On/Off", 1);
            device.SetLed("Backlight Pulse On/Off", 0);

            Assert.HasCount(2, mockMessageSender.PulseLightCommands);
            Assert.IsTrue(mockMessageSender.PulseLightCommands[0].IsOn);
            Assert.IsFalse(mockMessageSender.PulseLightCommands[1].IsOn);
        }

        [TestMethod]
        public void SetLed_SameStateTwice_SendsOnlyOnce()
        {
            var device = CreateDevice("Airbus Sidestick Right");

            device.SetLed("Backlight Percentage", 30);
            device.SetLed("Backlight Percentage", 30);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
        }

        [TestMethod]
        public void SetLed_NullOrEmptyName_SendsNothing()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            device.SetLed(null, 1);
            device.SetLed("", 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
            Assert.IsEmpty(mockMessageSender.BrightnessCommands);
            Assert.IsEmpty(mockMessageSender.VibrationCommands);
            Assert.IsEmpty(mockMessageSender.PulseLightCommands);
        }

        [TestMethod]
        public void SetLed_UnknownName_ThrowsKeyNotFoundException()
        {
            var device = CreateDevice("Airbus Sidestick Right");
            Assert.ThrowsExactly<System.Collections.Generic.KeyNotFoundException>(
                () => device.SetLed("DOES_NOT_EXIST", 1));
        }

        #endregion
    }
}
