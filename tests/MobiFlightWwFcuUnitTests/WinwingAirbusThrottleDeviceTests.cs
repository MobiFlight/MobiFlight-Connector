using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingAirbusThrottleDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
        }

        private WinwingAirbusThrottleDevice CreateDevice(string throttleType)
        {
            return new WinwingAirbusThrottleDevice(mockMessageSender, throttleType);
        }

        #region Basic Properties Tests

        [TestMethod]
        [DataRow("Airbus Throttle Left",  "WinWing Airbus Throttle Left")]
        [DataRow("Airbus Throttle Right", "WinWing Airbus Throttle Right")]
        public void Name_PrefixesWinWingToThrottleType(string throttleType, string expectedName)
        {
            var device = CreateDevice(throttleType);
            Assert.AreEqual(expectedName, device.Name);
        }

        [TestMethod]
        public void GetDisplayNames_ContainsAll3DisplayNames()
        {
            var device = CreateDevice("Airbus Throttle Right");
            var names = device.GetDisplayNames();

            Assert.HasCount(3, names);
            Assert.Contains("Trim Value", names);
            Assert.Contains("Trim Dashes On/Off", names);
            Assert.Contains("LCD Test On/Off", names);
        }

        [TestMethod]
        public void GetInternalDisplayNames_ReturnsEmptyList()
        {
            var device = CreateDevice("Airbus Throttle Right");
            Assert.IsEmpty(device.GetInternalDisplayNames());
        }

        [TestMethod]
        public void GetLedNames_ContainsAll4LedsAnd5OutputChannels()
        {
            var device = CreateDevice("Airbus Throttle Right");
            var ledNames = device.GetLedNames();

            Assert.HasCount(9, ledNames);
            Assert.Contains("FAULT_1", ledNames);
            Assert.Contains("FIRE_1", ledNames);
            Assert.Contains("FAULT_2", ledNames);
            Assert.Contains("FIRE_2", ledNames);
            Assert.Contains("Vibration 1 Percentage", ledNames);
            Assert.Contains("Vibration 2 Percentage", ledNames);
            Assert.Contains("Backlight Percentage", ledNames);
            Assert.Contains("LED Percentage", ledNames);
            Assert.Contains("LCD Percentage", ledNames);
        }

        #endregion

        #region Connect / Shutdown / Stop

        [TestMethod]
        public void Connect_SendsExactlyOneDisplayFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.Connect();

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void Connect_SetsBacklightOnBothAddressesAndLcdBrightnessAndZeroVibrations()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.Connect();

            // SetBacklightBrightness writes brightness twice (throttle + PAC), then SetLcdBrightness once = 3.
            Assert.HasCount(3, mockMessageSender.BrightnessCommands);

            // First two come from SetBacklightBrightness(20)
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("20", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[1].Type);
            Assert.AreEqual("20", mockMessageSender.BrightnessCommands[1].Brightness);

            // Third is SetLcdBrightness(100) on PAC, type 0x02
            Assert.AreEqual(0x02, mockMessageSender.BrightnessCommands[2].Type);
            Assert.AreEqual("100", mockMessageSender.BrightnessCommands[2].Brightness);

            // Two zero vibrations
            Assert.HasCount(2, mockMessageSender.VibrationCommands);
            Assert.AreEqual((byte)0x0e, mockMessageSender.VibrationCommands[0].Type);
            Assert.AreEqual((byte)0, mockMessageSender.VibrationCommands[0].Level);
            Assert.AreEqual((byte)0x10, mockMessageSender.VibrationCommands[1].Type);
            Assert.AreEqual((byte)0, mockMessageSender.VibrationCommands[1].Level);
        }

        [TestMethod]
        public void Connect_SendsCorrectInitFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.Connect();

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void Shutdown_EmptiesDisplayAndZerosBrightnessAndVibrationsAndLeds()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            // EmptyDisplay -> LcdTest("AllOff") -> 1 display frame
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // SetBacklightBrightness(0) -> 2 brightness msgs, SetLcdBrightness(0) -> 1
            Assert.HasCount(3, mockMessageSender.BrightnessCommands);
            foreach (var b in mockMessageSender.BrightnessCommands)
            {
                Assert.AreEqual("0", b.Brightness);
            }

            // Two vibrations to zero
            Assert.HasCount(2, mockMessageSender.VibrationCommands);
            foreach (var v in mockMessageSender.VibrationCommands)
            {
                Assert.AreEqual((byte)0, v.Level);
            }

            // 4 LEDs off
            Assert.HasCount(4, mockMessageSender.LightControlCommands);
            foreach (var lc in mockMessageSender.LightControlCommands)
            {
                Assert.AreEqual((byte)0, lc.Value);
            }
        }

        [TestMethod]
        public void Stop_TurnsOffAllLedsAndBothVibrationsAndSendsNoDisplayFrames()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.Stop();

            Assert.HasCount(4, mockMessageSender.LightControlCommands);
            Assert.HasCount(2, mockMessageSender.VibrationCommands);
            foreach (var v in mockMessageSender.VibrationCommands)
            {
                Assert.AreEqual((byte)0, v.Level);
            }
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region LED Tests

        [TestMethod]
        public void SetLed_Fault1_On_SendsLightControlMessage()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("FAULT_1", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)0x03, mockMessageSender.LightControlCommands[0].Type);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_Fire2_OffAfterOn_SendsZeroValue()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("FIRE_2", 1);
            device.SetLed("FIRE_2", 0);

            Assert.HasCount(2, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)0x06, mockMessageSender.LightControlCommands[1].Type);
            Assert.AreEqual((byte)0, mockMessageSender.LightControlCommands[1].Value);
        }

        [TestMethod]
        public void SetLed_NonZero_AdjustedToOne()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("FAULT_2", 200);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)0x05, mockMessageSender.LightControlCommands[0].Type);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_SameStateTwice_OnlySendsOnce()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("FIRE_1", 1);
            device.SetLed("FIRE_1", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_NullOrEmptyName_SendsNothing()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed(null, 1);
            device.SetLed("", 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_UnknownName_ThrowsKeyNotFoundException()
        {
            var device = CreateDevice("Airbus Throttle Right");
            Assert.ThrowsExactly<System.Collections.Generic.KeyNotFoundException>(
                () => device.SetLed("DOES_NOT_EXIST", 1));
        }

        #endregion

        #region Vibration / Brightness Output Tests

        [TestMethod]
        public void SetLed_Vibration1Percentage_SendsVibrationOnTypeE()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("Vibration 1 Percentage", 70);

            Assert.HasCount(1, mockMessageSender.VibrationCommands);
            Assert.AreEqual((byte)0x0e, mockMessageSender.VibrationCommands[0].Type);
            Assert.AreEqual((byte)70, mockMessageSender.VibrationCommands[0].Level);
        }

        [TestMethod]
        public void SetLed_Vibration2Percentage_SendsVibrationOnType10()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("Vibration 2 Percentage", 80);

            Assert.HasCount(1, mockMessageSender.VibrationCommands);
            Assert.AreEqual((byte)0x10, mockMessageSender.VibrationCommands[0].Type);
            Assert.AreEqual((byte)80, mockMessageSender.VibrationCommands[0].Level);
        }

        [TestMethod]
        public void SetLed_BacklightPercentage_WritesToBothThrottleAndPac()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("Backlight Percentage", 60);

            Assert.HasCount(2, mockMessageSender.BrightnessCommands);
            Assert.AreEqual((byte)0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("60", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.AreEqual((byte)0x00, mockMessageSender.BrightnessCommands[1].Type);
            Assert.AreEqual("60", mockMessageSender.BrightnessCommands[1].Brightness);

            // Different destinations: throttle vs pac
            CollectionAssert.AreNotEqual(
                mockMessageSender.BrightnessCommands[0].DestinationAddress,
                mockMessageSender.BrightnessCommands[1].DestinationAddress);
        }

        [TestMethod]
        public void SetLed_LedPercentage_WritesToThrottleOnTypeTwo()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("LED Percentage", 40);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual((byte)0x02, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("40", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        [TestMethod]
        public void SetLed_LcdPercentage_WritesToPacOnTypeTwo()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetLed("LCD Percentage", 90);

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual((byte)0x02, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("90", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        #endregion

        #region Trim Value Display Tests

        [TestMethod]
        public void SetDisplay_Trim_PositiveValue_SendsOneFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", "5");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_Trim_NegativeValue_SendsOneFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", "-2");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_Trim_FractionalValue_SendsOneFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", "-0.3");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_Trim_LargeMagnitude_SendsOneFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", "-24.3");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_Trim_Zero_SendsOneFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", "0");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_Trim_SameValueTwice_SecondCallIsNoop()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", "5");
            device.SetDisplay("Trim Value", "5");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_Trim_NullOrWhiteSpace_SendsNothing()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", null);
            device.SetDisplay("Trim Value", "");
            device.SetDisplay("Trim Value", "   ");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Trim Dashes Tests

        [TestMethod]
        public void SetDisplay_TrimDashesOn_BeforeAnyTrim_DoesNotSendDisplayFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Dashes On/Off", "1");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_TrimDashesOn_AfterTrim_RerendersAsDashes()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("Trim Value", "5");
            device.SetDisplay("Trim Dashes On/Off", "1");

            // First the normal value, then a re-render with dashes.
            Assert.HasCount(2, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[1].Commands, expectedCommands);
        }

        #endregion

        #region Annunciator Light Tests

        [TestMethod]
        public void SetDisplay_LcdTest_On_SendsAllOnFrame()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("LCD Test On/Off", "1");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_LcdTest_Off_ResendsCurrentBuffer()
        {
            var device = CreateDevice("Airbus Throttle Right");
            device.SetDisplay("LCD Test On/Off", "0");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x0E, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00 },
                new byte[] { 0x01, 0xB9, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        #endregion

        #region Helpers

        private void CompareDisplayCommands(List<byte[]> sentCommands, List<byte[]> expectedCommands)
        {
            Assert.HasCount(expectedCommands.Count, sentCommands);

            for (int i = 0; i < expectedCommands.Count; i++)
            {
                CollectionAssert.AreEqual(expectedCommands[i], sentCommands[i]);
            }
        }

        #endregion
    }
}
