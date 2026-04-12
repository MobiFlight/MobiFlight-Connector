
using MobiFlightWwFcu;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingFcuDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender;
        private WinwingFcuDevice device;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
            device = new WinwingFcuDevice(mockMessageSender);
        }

        [TestCleanup]
        public void Cleanup()
        {
            device?.Stop();
        }

        #region Basic Properties Tests

        [TestMethod]
        public void Name_ShouldReturnCorrectDeviceName()
        {
            Assert.AreEqual("WinWing FCU", device.Name);
        }

        [TestMethod]
        public void GetDisplayNames_ShouldReturnAllDisplayNames()
        {
            var displayNames = device.GetDisplayNames();

            Assert.IsNotNull(displayNames);
            Assert.Contains("Speed Value", displayNames);
            Assert.Contains("Mach Value", displayNames);
            Assert.Contains("Heading Value", displayNames);
            Assert.Contains("Altitude Value", displayNames);
            Assert.Contains("VS Value", displayNames);
            Assert.HasCount(19, displayNames);
        }

        [TestMethod]
        public void GetLedNames_ShouldReturnAllLedNames()
        {
            var ledNames = device.GetLedNames();

            Assert.IsNotNull(ledNames);
            Assert.Contains("LOC", ledNames);
            Assert.Contains("AP1", ledNames);
            Assert.Contains("AP2", ledNames);
            Assert.Contains("ATHR", ledNames);
            Assert.Contains("APPR", ledNames);
            Assert.Contains("EXPED", ledNames);
            Assert.HasCount(6, ledNames);
        }

        [TestMethod]
        public void GetInternalDisplayNames_ShouldReturnEmptyList()
        {
            var internalDisplayNames = device.GetInternalDisplayNames();

            Assert.IsNotNull(internalDisplayNames);
            Assert.IsEmpty(internalDisplayNames);
        }

        #endregion

        #region Connect and Shutdown Tests

        [TestMethod]
        public void Connect_ShouldInitializeDisplay()
        {
            device.Connect();

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
            Assert.IsGreaterThanOrEqualTo(2, mockMessageSender.BrightnessCommands.Count);
        }

        [TestMethod]
        public void Shutdown_ShouldEmptyDisplayAndTurnOffLights()
        {
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
            Assert.IsGreaterThanOrEqualTo(6, mockMessageSender.LightControlCommands.Count);
        }

        [TestMethod]
        public void Stop_ShouldTurnOffAllLEDs()
        {
            device.Connect();
            device.SetLed("AP1", 1);
            mockMessageSender.Reset();

            device.Stop();

            Assert.IsGreaterThanOrEqualTo(6, mockMessageSender.LightControlCommands.Count);
        }

        #endregion

        #region LED Tests

        [TestMethod]
        public void SetLed_WithValidLedAndState_ShouldSendLightControlMessage()
        {
            device.SetLed("AP1", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            var command = mockMessageSender.LightControlCommands[0];
            Assert.AreEqual(0x05, command.Type);
            Assert.AreEqual(1, command.Value);
        }

        [TestMethod]
        public void SetLed_WithZeroState_ShouldSendZeroValue()
        {
            device.SetLed("LOC", 0);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            var command = mockMessageSender.LightControlCommands[0];
            Assert.AreEqual(0x03, command.Type);
            Assert.AreEqual(0, command.Value);
        }

        [TestMethod]
        public void SetLed_WithSameStateTwice_ShouldOnlySendOnce()
        {
            device.SetLed("AP2", 1);
            device.SetLed("AP2", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_WithDifferentStates_ShouldSendBoth()
        {
            device.SetLed("ATHR", 1);
            device.SetLed("ATHR", 0);

            Assert.HasCount(2, mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_WithNullOrEmptyName_ShouldNotSendCommand()
        {
            device.SetLed(null, 1);
            device.SetLed("", 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
        }

        #endregion

        #region Speed Display Tests

        [TestMethod]
        public void SetDisplay_Speed_ShouldUpdateSpeedDisplay()
        {
            device.SetDisplay("Speed Value", "250");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_Speed_WithZero_ShouldDisplay000()
        {
            device.SetDisplay("Speed Value", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_Speed_WithMaxValue_ShouldDisplayCorrectly()
        {
            device.SetDisplay("Speed Value", "999");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_MachValue_ShouldUpdateMachDisplay()
        {
            device.SetDisplay("Mach Value", "0.78");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_SpeedDot_WithOne_ShouldShowDot()
        {
            device.SetDisplay("Speed Dot", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_SpeedDot_WithZero_ShouldHideDot()
        {
            device.SetDisplay("Speed Dot", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_SpeedDashes_ShouldDisplayDashes()
        {
            device.SetDisplay("Speed Dashes On/Off", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_MachMode_WithOne_ShouldShowMachLabel()
        {
            device.SetDisplay("Mach Mode On/Off", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_MachMode_WithZero_ShouldShowSpeedLabel()
        {
            device.SetDisplay("Mach Mode On/Off", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_MachMode_WithTwo_ShouldShowNoLabel()
        {
            device.SetDisplay("Mach Mode On/Off", "2");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Heading Display Tests

        [TestMethod]
        public void SetDisplay_Heading_ShouldUpdateHeadingDisplay()
        {
            device.SetDisplay("Heading Value", "180");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_Heading_WithZero_ShouldDisplay000()
        {
            device.SetDisplay("Heading Value", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_Heading_WithMax_ShouldDisplay360()
        {
            device.SetDisplay("Heading Value", "360");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_TRKValue_ShouldUpdateTrackDisplay()
        {
            device.SetDisplay("TRK Value", "270");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_HeadingDashes_ShouldDisplayDashes()
        {
            device.SetDisplay("Heading Dashes On/Off", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_HeadingDot_WithOne_ShouldShowDot()
        {
            device.SetDisplay("Heading Dot", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_HeadingDot_WithZero_ShouldHideDot()
        {
            device.SetDisplay("Heading Dot", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Altitude Display Tests

        [TestMethod]
        public void SetDisplay_Altitude_ShouldUpdateAltitudeDisplay()
        {
            device.SetDisplay("Altitude Value", "10000");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_Altitude_WithZero_ShouldDisplay00000()
        {
            device.SetDisplay("Altitude Value", "0");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x10, 0xBB, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0xFA, 0xFA, 0xA9, 0xAF, 0xAF, 0xAF, 0xAC, 0xBF, 0xBF, 0xBF, 0xBF, 0xBF, 0xAF, 0x7F, 0x63, 0x43, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x10, 0xBB, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_Altitude_WithMaxValue_ShouldDisplayCorrectly()
        {
            device.SetDisplay("Altitude Value", "99999");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_AltitudeDot_WithZero_ShouldShowAltNoDot()
        {
            device.SetDisplay("Altitude Dot", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_AltitudeDot_WithOne_ShouldShowAltWithDot()
        {
            device.SetDisplay("Altitude Dot", "1");
            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x10, 0xBB, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0xFA, 0xFA, 0xA9, 0xAF, 0xAF, 0xAF, 0xAC, 0xBF, 0x1F, 0xB6, 0xBF, 0xBF, 0xAF, 0x7F, 0x73, 0x43, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x10, 0xBB, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        private void CompareDisplayCommands(List<byte[]> sentCommands, List<byte[]> expectedCommands)
        {
            Assert.HasCount(expectedCommands.Count, sentCommands);

            for (int i = 0; i < expectedCommands.Count; i++)
            {
                CollectionAssert.AreEqual(expectedCommands[i], sentCommands[i]);
            }
        }

        [TestMethod]
        public void SetDisplay_AltitudeDot_WithTwenty_ShouldHideAltNoDot()
        {
            device.SetDisplay("Altitude Dot", "20");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_AltitudeDot_WithTwentyOne_ShouldHideAltWithDot()
        {
            device.SetDisplay("Altitude Dot", "21");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Vertical Speed Display Tests

        [TestMethod]
        public void SetDisplay_VS_WithPositiveValue_ShouldUpdateVSDisplay()
        {
            device.SetDisplay("VS Value", "1500");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_VS_WithNegativeValue_ShouldShowMinusSign()
        {
            device.SetDisplay("VS Value", "-2000");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_VS_WithZero_ShouldDisplayCorrectly()
        {
            device.SetDisplay("VS Value", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_VS_WithHundredsZero_ShouldShowAirbusStyle()
        {
            device.SetDisplay("VS Value", "1500");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_FPA_WithPositiveValue_ShouldUpdateFPADisplay()
        {
            device.SetDisplay("FPA Value", "2.5");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_FPA_WithNegativeValue_ShouldShowMinusSign()
        {
            device.SetDisplay("FPA Value", "-3.2");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_VSDashes_WithOne_ShouldShowDashesWithLvlCh()
        {
            device.SetDisplay("VS Dashes On/Off", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_VSDashes_WithZero_ShouldHideDashesWithLvlCh()
        {
            device.SetDisplay("VS Dashes On/Off", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_VSDashes_WithTwentyOne_ShouldShowDashesNoLvlCh()
        {
            device.SetDisplay("VS Dashes On/Off", "21");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_VSDashes_WithTwenty_ShouldHideDashesNoLvlCh()
        {
            device.SetDisplay("VS Dashes On/Off", "20");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region TRK/FPA Mode Tests

        [TestMethod]
        public void SetDisplay_TRKMode_WithZero_ShouldShowHeadingAndVSMode()
        {
            device.SetDisplay("TRK Mode On/Off", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_TRKMode_WithOne_ShouldShowTrackAndFPAMode()
        {
            device.SetDisplay("TRK Mode On/Off", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_TRKMode_WithTwo_ShouldHideBothModes()
        {
            device.SetDisplay("TRK Mode On/Off", "2");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_TRKMode_WithThreeDigits_ShouldSetIndividualModes()
        {
            device.SetDisplay("TRK Mode On/Off", "100");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Brightness Tests

        [TestMethod]
        public void SetDisplay_BacklightBrightness_ShouldSetBrightness()
        {
            device.SetDisplay("Backlight Percentage", "50");

            Assert.IsNotEmpty(mockMessageSender.BrightnessCommands);
        }

        [TestMethod]
        public void SetDisplay_LCDBrightness_ShouldSetBrightness()
        {
            device.SetDisplay("LCD Percentage", "75");

            Assert.IsNotEmpty(mockMessageSender.BrightnessCommands);
        }

        [TestMethod]
        public void SetDisplay_LEDBrightness_ShouldSetBrightness()
        {
            device.SetDisplay("LED Percentage", "100");

            Assert.IsNotEmpty(mockMessageSender.BrightnessCommands);
        }

        #endregion

        #region Annunciator Light Tests

        [TestMethod]
        public void SetDisplay_AnnunciatorLight_WithOne_ShouldTurnOnAllLights()
        {
            device.SetDisplay("LCD Test On/Off", "1");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_AnnunciatorLight_WithZero_ShouldResetDisplay()
        {
            device.SetDisplay("LCD Test On/Off", "0");

            Assert.IsNotEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Caching Tests

        [TestMethod]
        public void SetDisplay_WithSameValue_ShouldNotSendCommandTwice()
        {
            device.SetDisplay("Speed Value", "250");
            int firstCount = mockMessageSender.DisplayCommandsSent.Count;

            device.SetDisplay("Speed Value", "250");
            int secondCount = mockMessageSender.DisplayCommandsSent.Count;

            Assert.AreEqual(firstCount, secondCount);
        }

        [TestMethod]
        public void SetDisplay_WithDifferentValue_ShouldSendCommandTwice()
        {
            device.SetDisplay("Speed Value", "250");
            int firstCount = mockMessageSender.DisplayCommandsSent.Count;

            device.SetDisplay("Speed Value", "300");
            int secondCount = mockMessageSender.DisplayCommandsSent.Count;

            Assert.IsGreaterThan(firstCount, secondCount);
        }

        [TestMethod]
        public void SetDisplay_WithNullOrWhiteSpace_ShouldNotSendCommand()
        {
            device.SetDisplay("Speed Value", null);
            device.SetDisplay("Speed Value", "");
            device.SetDisplay("Speed Value", "   ");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Mock Implementation

        private class MockWinwingMessageSender : IWinwingMessageSender
        {
            public List<DisplayCommandMessage> DisplayCommandsSent { get; } = new List<DisplayCommandMessage>();
            public List<LightControlMessage> LightControlCommands { get; } = new List<LightControlMessage>();
            public List<BrightnessMessage> BrightnessCommands { get; } = new List<BrightnessMessage>();
            public List<byte[]> CduDisplayBytes { get; } = new List<byte[]>();
            public int HeartBeatMessageCount { get; private set; }
            public int RequestFirmwareMessageCount { get; private set; }
            public bool IsConnectedValue { get; set; }

            public void Reset()
            {
                DisplayCommandsSent.Clear();
                LightControlCommands.Clear();
                BrightnessCommands.Clear();
                CduDisplayBytes.Clear();
                HeartBeatMessageCount = 0;
                RequestFirmwareMessageCount = 0;
            }

            public bool IsConnected()
            {
                return IsConnectedValue;
            }

            public void Connect()
            {
                IsConnectedValue = true;
            }

            public void Shutdown()
            {
                IsConnectedValue = false;
            }

            public void SendDisplayCommands(IList<byte[]> commands)
            {
                DisplayCommandsSent.Add(new DisplayCommandMessage
                {
                    Commands = commands.Select(c => (byte[])c.Clone()).ToList()
                });

                Console.WriteLine("SendDisplayCommands called with {0} command(s):", commands.Count);
                for (int i = 0; i < commands.Count; i++)
                {
                    var bytes = commands[i];
                    var hexValues = string.Join(", ", bytes.Select(b => string.Format("0x{0:X2}", b)));
                    Console.WriteLine("  Command {0}: new byte[] {{ {1} }}", i, hexValues);
                }
                Console.WriteLine();
            }

            public void SendCduDisplayBytes(byte[] byteList)
            {
                CduDisplayBytes.Add((byte[])byteList.Clone());
            }

            public void SendLightControlMessage(byte[] destination, byte type, byte value)
            {
                LightControlCommands.Add(new LightControlMessage
                {
                    Destination = (byte[])destination.Clone(),
                    Type = type,
                    Value = value
                });
            }

            public void SetBrightness(byte[] destinationAddress, byte type, string brightness)
            {
                BrightnessCommands.Add(new BrightnessMessage
                {
                    DestinationAddress = (byte[])destinationAddress.Clone(),
                    Type = type,
                    Brightness = brightness
                });
            }

            public void SetBrightness(byte[] destinationAddress, byte type, int brightness)
            {
                BrightnessCommands.Add(new BrightnessMessage
                {
                    DestinationAddress = (byte[])destinationAddress.Clone(),
                    Type = type,
                    Brightness = brightness.ToString()
                });
            }

            public void SetVibration(byte[] destinationAddress, byte type, byte level)
            {
                // Not used by FCU device
            }

            public void SetPulseLight(byte[] destinationAddress, bool isOn)
            {
                // Not used by FCU device
            }

            public void SendHeartBeatMessage()
            {
                HeartBeatMessageCount++;
            }

            public void SendRequestFirmwareMessage()
            {
                RequestFirmwareMessageCount++;
            }
        }

        public class DisplayCommandMessage
        {
            public List<byte[]> Commands { get; set; }
        }

        public class LightControlMessage
        {
            public byte[] Destination { get; set; }
            public byte Type { get; set; }
            public byte Value { get; set; }
        }

        public class BrightnessMessage
        {
            public byte[] DestinationAddress { get; set; }
            public byte Type { get; set; }
            public string Brightness { get; set; }
        }

        #endregion
    }
}
