using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingRmpDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;
        private WinwingRmpDevice device = null!;

        private static readonly byte[] RefreshFrame = new byte[]
        {
            0x82, 0xBB, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
            device = new WinwingRmpDevice(mockMessageSender, "RMP Center");
        }

        [TestCleanup]
        public void Cleanup()
        {
            device.Stop();
        }

        // Data section right after construction: "Mobi" right-aligned on ACTIVE (bytes 10-15),
        // with 'M' spanning two cells as "{" + "}" and 'i' drawn as 'l'. STBY stays blank.
        private static byte[] MobiInitData()
        {
            var data = new byte[36];
            data[11] = 0x33; data[12] = 0x27; data[13] = 0x5C; data[14] = 0x7C; data[15] = 0x30; // "{}obl"
            return data;
        }

        // Builds the expected 145-byte SetValues frame 
        private static byte[] SetValuesFrame(byte[] data)
        {
            var frame = new byte[0x91];
            var header = new byte[]
            {
                0x82, 0xBB, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x00, 0x00, 0x00
            };
            header.CopyTo(frame, 0);
            data.CopyTo(frame, 17);
            return frame;
        }

        #region Basic Properties Tests

        [TestMethod]
        public void Name_CenterVariant_ReturnsWinWingRmpCenter()
        {
            Assert.AreEqual("WinWing RMP Center", device.Name);
        }

        [TestMethod]
        public void Name_LeftAndRightVariant_ContainVariantName()
        {
            Assert.AreEqual("WinWing RMP Left", new WinwingRmpDevice(mockMessageSender, "RMP Left").Name);
            Assert.AreEqual("WinWing RMP Right", new WinwingRmpDevice(mockMessageSender, "RMP Right").Name);
        }

        [TestMethod]
        public void GetDisplayNames_ContainsAll8DisplayNames()
        {
            var displayNames = device.GetDisplayNames();
            Assert.HasCount(8, displayNames);
            Assert.Contains("Active Mode", displayNames);
            Assert.Contains("Active Value", displayNames);
            Assert.Contains("Standby Mode", displayNames);
            Assert.Contains("Standby Value", displayNames);
            Assert.Contains("LCD Test On/Off", displayNames);
            Assert.Contains("Backlight Percentage", displayNames);
            Assert.Contains("LCD Percentage", displayNames);
            Assert.Contains("LED Percentage", displayNames);
        }

        [TestMethod]
        public void GetLedNames_ContainsAll14LedNames()
        {
            var ledNames = device.GetLedNames();
            Assert.HasCount(14, ledNames);
            foreach (var name in new[] { "SEL", "VHF1", "VHF2", "VHF3", "LOAD", "HF1", "HF2",
                                         "AM", "NAV", "VOR", "ILS", "GLS", "MLS", "ADF" })
            {
                Assert.Contains(name, ledNames);
            }
        }

        #endregion

        #region Connect / Shutdown / Stop

        [TestMethod]
        public void Connect_ShowsMobiBrandingAndSetsBrightness()
        {
            device.Connect();

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            Assert.HasCount(2, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual(0x01, mockMessageSender.BrightnessCommands[1].Type);

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                SetValuesFrame(MobiInitData()),
                RefreshFrame
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void Shutdown_SendsAllOffTestPatternAndTurnsEverythingOff()
        {
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x82, 0xBB, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02 },
                RefreshFrame
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);

            Assert.HasCount(2, mockMessageSender.BrightnessCommands);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[1].Brightness);

            Assert.HasCount(14, mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void Stop_TurnsOffAllLeds()
        {
            device.SetLed("VHF1", 1);
            mockMessageSender.Reset();

            device.Stop();

            Assert.HasCount(14, mockMessageSender.LightControlCommands);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region SetLed

        [TestMethod]
        public void SetLed_Vhf1On_SendsLightControlMessageWithCorrectType()
        {
            device.SetLed("VHF1", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x04, mockMessageSender.LightControlCommands[0].Type);
            Assert.AreEqual(1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_SameStateTwice_SendsOnlyOnce()
        {
            device.SetLed("ADF", 1);
            device.SetLed("ADF", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
        }

        #endregion

        #region SetDisplay — capture-anchored frame

        [TestMethod]
        public void SetDisplay_ActiveAndStandbyFrequencies_MatchesCapturedFrame()
        {
            // Reference frame from a real USB capture:
            // STBY "118.905" (bytes 4-9), ACTIVE "122.800" (bytes 10-15).
            device.SetDisplay("Active Mode", "1");
            device.SetDisplay("Active Value", "122.800");
            device.SetDisplay("Standby Mode", "1");
            device.SetDisplay("Standby Value", "118.905");

            // Mode changes to numeric modes send nothing; only the two value sets render.
            Assert.HasCount(2, mockMessageSender.DisplayCommandsSent);

            var data = new byte[36];
            data[4] = 0x06; data[5] = 0x06; data[6] = 0xFF; data[7] = 0x6F; data[8] = 0x3F; data[9] = 0x6D;
            data[10] = 0x06; data[11] = 0x5B; data[12] = 0xDB; data[13] = 0x7F; data[14] = 0x3F; data[15] = 0x3F;

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                SetValuesFrame(data),
                RefreshFrame
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[1].Commands, expectedCommands);
        }

        #endregion

        #region SetDisplay — modes

        [TestMethod]
        public void SetDisplay_StandbyCourseMode_ShowsCDashCourse()
        {
            device.SetDisplay("Standby Mode", "3");
            device.SetDisplay("Standby Value", "270");

            // " C-270" → blank, 'C'(0x39), '-'(0x40), '2'(0x5B), '7'(0x07), '0'(0x3F), no dot.
            // ACTIVE was never set and still shows the "Mobi" init branding.
            var data = MobiInitData();
            data[4] = 0x00; data[5] = 0x39; data[6] = 0x40; data[7] = 0x5B; data[8] = 0x07; data[9] = 0x3F;

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                SetValuesFrame(data),
                RefreshFrame
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_ActiveDataMode_ShowsDAtACentered()
        {
            device.SetDisplay("Active Mode", "4");

            // "*dAtA*" → blank, 'd'(0x5E), 'A'(0x77), 't'(0x78), 'A'(0x77), blank.
            // STBY was never set and stays blank.
            var data = MobiInitData();
            data[10] = 0x00; data[11] = 0x5E; data[12] = 0x77; data[13] = 0x78; data[14] = 0x77; data[15] = 0x00;

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                SetValuesFrame(data),
                RefreshFrame
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_ActiveAdfMode_ShowsRightAlignedValueWithDotAfterCell3()
        {
            device.SetDisplay("Active Mode", "2");
            device.SetDisplay("Active Value", "364.5");

            // "*364.5*" → blank, '3'(0x4F), '6'(0x7D), '4'+dot(0xE6), '5'(0x6D), blank.
            // STBY was never set and stays blank.
            var data = MobiInitData();
            data[10] = 0x00; data[11] = 0x4F; data[12] = 0x7D; data[13] = 0xE6; data[14] = 0x6D; data[15] = 0x00;

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                SetValuesFrame(data),
                RefreshFrame
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_EmptyModeAfterValue_BlanksTheDisplay()
        {
            device.SetDisplay("Active Mode", "1");
            device.SetDisplay("Active Value", "122.800");

            device.SetDisplay("Active Mode", "0");

            // ACTIVE blanked; STBY was never set and stays blank.
            var data = MobiInitData();
            data[10] = 0x00; data[11] = 0x00; data[12] = 0x00; data[13] = 0x00; data[14] = 0x00; data[15] = 0x00;

            List<byte[]> expectedCommands = new List<byte[]>()
            {
                SetValuesFrame(data),
                RefreshFrame
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[1].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_NumericModeChangeAfterValue_DoesNotRefreshWithOldValue()
        {
            device.SetDisplay("Active Mode", "1");
            device.SetDisplay("Active Value", "122.800");
            mockMessageSender.Reset();

            device.SetDisplay("Active Mode", "2");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region SetDisplay — caching

        [TestMethod]
        public void SetDisplay_SameValueTwice_SendsOnlyOnce()
        {
            device.SetDisplay("Active Mode", "1");
            mockMessageSender.Reset();

            device.SetDisplay("Active Value", "122.800");
            device.SetDisplay("Active Value", "122.800");

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_ModeChange_ClearsValueCacheSoSameValueRendersAgain()
        {
            device.SetDisplay("Active Mode", "1");
            device.SetDisplay("Active Value", "122.800");
            mockMessageSender.Reset();

            device.SetDisplay("Active Mode", "2");
            device.SetDisplay("Active Value", "122.800");

            // The mode change itself sends nothing; the (identical) value renders again.
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
        }

        [TestMethod]
        public void SetDisplay_UnknownDisplayName_ThrowsKeyNotFoundException()
        {
            Assert.ThrowsExactly<KeyNotFoundException>(() => device.SetDisplay("Unknown Name", "1"));
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
