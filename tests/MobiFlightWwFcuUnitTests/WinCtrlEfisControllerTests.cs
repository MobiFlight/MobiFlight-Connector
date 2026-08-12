using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinCtrlEfisControllerTests
    {
        private MockWinCtrlMessageSender mockMessageSender = null!;
        private WinCtrlEfisController device = null!;

        // The EFIS-Left destination (0x0D, 0xBF) is the prefix on every command frame.
        // RefreshCommand is always identical (17 bytes, header only).
        private static readonly byte[] RefreshCommand = new byte[]
        {
            0x0D, 0xBF, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinCtrlMessageSender();
            // Use the "Left" EFIS variant for all tests (DEST_EFISL = 0x0D 0xBF).
            device = new WinCtrlEfisController(mockMessageSender, "Left");
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
            Assert.AreEqual("WinCtrl EFIS Left", device.Name);
        }

        [TestMethod]
        public void GetDisplayNames_ShouldReturnAllDisplayNames()
        {
            var displayNames = device.GetDisplayNames();

            Assert.IsNotNull(displayNames);
            Assert.Contains("hPa Value Left", displayNames);
            Assert.Contains("inHg Value Left", displayNames);
            Assert.Contains("inHg Mode On/Off Left", displayNames);
            Assert.Contains("STD Mode On/Off Left", displayNames);
            Assert.Contains("QFE Mode On/Off Left", displayNames);
            Assert.Contains("LCD Test On/Off", displayNames);
            Assert.Contains("Backlight Percentage", displayNames);
            Assert.Contains("LCD Percentage", displayNames);
            Assert.Contains("LED Percentage", displayNames);
            Assert.HasCount(9, displayNames);
        }

        [TestMethod]
        public void GetLedNames_ShouldReturnAllLedNames()
        {
            var ledNames = device.GetLedNames();

            Assert.IsNotNull(ledNames);
            Assert.Contains("FD Left", ledNames);
            Assert.Contains("LS Left", ledNames);
            Assert.Contains("CSTR Left", ledNames);
            Assert.Contains("WPT Left", ledNames);
            Assert.Contains("VORD Left", ledNames);
            Assert.Contains("NDB Left", ledNames);
            Assert.Contains("ARPT Left", ledNames);
            Assert.HasCount(7, ledNames);
        }

        [TestMethod]
        public void GetInternalDisplayNames_ShouldReturnEmptyList()
        {
            var internalDisplayNames = device.GetInternalDisplayNames();

            Assert.IsNotNull(internalDisplayNames);
            Assert.IsEmpty(internalDisplayNames);
        }

        [TestMethod]
        public void Constructor_RightVariant_ShouldUseRightDestinationAndNames()
        {
            var rightSender = new MockWinCtrlMessageSender();
            var rightDevice = new WinCtrlEfisController(rightSender, "Right");

            Assert.AreEqual("WinCtrl EFIS Right", rightDevice.Name);
            Assert.Contains("hPa Value Right", rightDevice.GetDisplayNames());
            Assert.Contains("FD Right", rightDevice.GetLedNames());
        }

        #endregion

        #region Connect and Shutdown Tests

        [TestMethod]
        public void Connect_ShouldInitializeDisplay()
        {
            device.Connect();

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            Assert.IsGreaterThanOrEqualTo(2, mockMessageSender.BrightnessCommands.Count);

            // After PrepareCommands the default reading is 1013 with QNH (byte 25 = 0x02).
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void Shutdown_ShouldEmptyDisplayAndTurnOffLights()
        {
            device.Connect();
            mockMessageSender.Reset();

            device.Shutdown();

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            Assert.IsGreaterThanOrEqualTo(2, mockMessageSender.BrightnessCommands.Count);

            // EmptyDisplay zeroes bytes 21..25 (digits + mode flag) of SetValuesCommand.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void Stop_ShouldTurnOffAllLEDs()
        {
            device.Connect();
            device.SetLed("FD Left", 1);
            mockMessageSender.Reset();

            device.Stop();

            Assert.IsGreaterThanOrEqualTo(1, mockMessageSender.LightControlCommands.Count);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region LED Tests

        [TestMethod]
        public void SetLed_FD_On_ShouldSendLightControlMessage()
        {
            device.SetLed("FD Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            var command = mockMessageSender.LightControlCommands[0];
            Assert.AreEqual(0x03, command.Type);
            Assert.AreEqual(1, command.Value);
        }

        [TestMethod]
        public void SetLed_LS_On_ShouldSendLightControlMessage()
        {
            device.SetLed("LS Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x04, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        public void SetLed_CSTR_On_ShouldSendLightControlMessage()
        {
            device.SetLed("CSTR Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x05, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        public void SetLed_WPT_On_ShouldSendLightControlMessage()
        {
            device.SetLed("WPT Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x06, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        public void SetLed_VORD_On_ShouldSendLightControlMessage()
        {
            device.SetLed("VORD Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x07, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        public void SetLed_NDB_On_ShouldSendLightControlMessage()
        {
            device.SetLed("NDB Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x08, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        public void SetLed_ARPT_On_ShouldSendLightControlMessage()
        {
            device.SetLed("ARPT Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual(0x09, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        public void SetLed_FD_Off_ShouldSendZeroValue()
        {
            device.SetLed("FD Left", 1);
            device.SetLed("FD Left", 0);

            Assert.HasCount(2, mockMessageSender.LightControlCommands);
            var command = mockMessageSender.LightControlCommands[1];
            Assert.AreEqual(0x03, command.Type);
            Assert.AreEqual(0, command.Value);
        }

        [TestMethod]
        public void SetLed_WithSameStateTwice_ShouldOnlySendOnce()
        {
            device.SetLed("FD Left", 1);
            device.SetLed("FD Left", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_WithNullOrEmptyName_ShouldNotSendCommand()
        {
            device.SetLed(null, 1);
            device.SetLed("", 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
        }

        #endregion

        #region hPa Value Tests

        [TestMethod]
        public void SetDisplay_HpaValue_With1013_ShouldSendDisplayCommand()
        {
            device.SetDisplay("hPa Value Left", "1013");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // Digits: '1'=0x60 '0'=0x7D '1'=0x60 '3'=0x7A, mode=QNH (0x02), inHg dot off.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_HpaValue_With0992_ShouldSendDisplayCommand()
        {
            device.SetDisplay("hPa Value Left", "992");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // 992 â†’ "0992": '0'=0x7D '9'=0x7B '9'=0x7B '2'=0x3E.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x7D, 0x7B, 0x7B, 0x3E, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_HpaValue_With1050_ShouldSendDisplayCommand()
        {
            device.SetDisplay("hPa Value Left", "1050");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // '1'=0x60 '0'=0x7D '5'=0x5B '0'=0x7D.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x5B, 0x7D, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_HpaValue_WhenStdActive_ShouldNotSendCommand()
        {
            device.SetDisplay("STD Mode On/Off Left", "1");
            mockMessageSender.Reset();

            device.SetDisplay("hPa Value Left", "1013");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region inHg Value Tests

        [TestMethod]
        public void SetDisplay_InHgValue_With2992_ShouldSendDisplayCommand()
        {
            device.SetDisplay("inHg Value Left", "29.92");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // 29.92 Ã— 100 â†’ "2992": '2'=0x3E, '9'+dot=0x7B|0x80=0xFB, '9'=0x7B, '2'=0x3E.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0xFB, 0x7B, 0x3E, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_InHgValue_With2950_ShouldSendDisplayCommand()
        {
            device.SetDisplay("inHg Value Left", "29.50");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // 29.50 Ã— 100 â†’ "2950": '2'=0x3E, '9'+dot=0xFB, '5'=0x5B, '0'=0x7D.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3E, 0xFB, 0x5B, 0x7D, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_InHgValue_WhenStdActive_ShouldNotSendCommand()
        {
            device.SetDisplay("STD Mode On/Off Left", "1");
            mockMessageSender.Reset();

            device.SetDisplay("inHg Value Left", "29.92");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region STD Mode Tests

        [TestMethod]
        public void SetDisplay_StdMode_On_ShouldRenderStd()
        {
            device.SetDisplay("STD Mode On/Off Left", "1");

            // STD-on path sends twice: once from SetBaroInternal, once from the trailing
            // SendDisplayCommand after ResetBaroCache. Both frames carry the same buffer.
            Assert.HasCount(2, mockMessageSender.DisplayCommandsSent);

            // Digits: 'S'=0x5B, 't'=0x0F, 'd'=0x6E, '*'=0x00, mode=NoBaro (0x00).
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x5B, 0x0F, 0x6E, 0x00, 0x00 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[1].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_StdMode_Off_ShouldResendCurrentBuffer()
        {
            device.SetDisplay("STD Mode On/Off Left", "0");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // STD-off resends the current buffer untouched, which is the post-init "1013 QNH" state.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_StdMode_OnThenHpa_ShouldNotResendHpaWhileStd()
        {
            device.SetDisplay("STD Mode On/Off Left", "1");
            int afterStd = mockMessageSender.DisplayCommandsSent.Count;

            device.SetDisplay("hPa Value Left", "1013");

            Assert.HasCount(afterStd, mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region QFE Mode Tests

        [TestMethod]
        public void SetDisplay_QfeMode_On_ShouldSendDisplayCommand()
        {
            device.SetDisplay("QFE Mode On/Off Left", "1");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // QFE flips byte 25 to 0x01; digits stay at the post-init "1013".
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x01 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_QfeMode_Off_ShouldSendDisplayCommand()
        {
            device.SetDisplay("QFE Mode On/Off Left", "0");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // QFE-off writes QnhBaro (0x02) to byte 25 â€” same as the post-init default.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_QfeMode_WhenStdActive_ShouldNotSendCommand()
        {
            device.SetDisplay("STD Mode On/Off Left", "1");
            mockMessageSender.Reset();

            device.SetDisplay("QFE Mode On/Off Left", "1");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region inHg Mode Tests

        [TestMethod]
        public void SetDisplay_InHgMode_DoesNotSendButResetsCache()
        {
            device.SetDisplay("hPa Value Left", "1013");
            int afterHpa = mockMessageSender.DisplayCommandsSent.Count;

            device.SetDisplay("inHg Mode On/Off Left", "1");
            int afterInHgMode = mockMessageSender.DisplayCommandsSent.Count;

            // inHg Mode itself sends nothing, but resets the cache so the same hPa value can be re-sent.
            Assert.AreEqual(afterHpa, afterInHgMode);

            device.SetDisplay("hPa Value Left", "1013");
            Assert.HasCount(2, mockMessageSender.DisplayCommandsSent);

            // Both hPa sends produce the same "1013 QNH" buffer.
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[1].Commands, expectedCommands);
        }

        #endregion

        #region Annunciator Light Tests

        [TestMethod]
        public void SetDisplay_AnnunciatorLight_WithOne_ShouldTurnOnAllLights()
        {
            device.SetDisplay("LCD Test On/Off", "1");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // 18-byte LCD-test frame; byte 17 carries the AllOn mode (0x23).
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x23 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        [TestMethod]
        public void SetDisplay_AnnunciatorLight_WithZero_ShouldResendCurrentBuffer()
        {
            device.SetDisplay("LCD Test On/Off", "0");
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);

            // The "off" branch resends the current SetValuesCommand (post-init "1013 QNH").
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        #endregion

        #region Caching Tests

        [TestMethod]
        public void SetDisplay_HpaValue_WithSameValue_ShouldNotSendCommandTwice()
        {
            device.SetDisplay("hPa Value Left", "1013");
            int firstCount = mockMessageSender.DisplayCommandsSent.Count;

            device.SetDisplay("hPa Value Left", "1013");
            int secondCount = mockMessageSender.DisplayCommandsSent.Count;

            Assert.AreEqual(firstCount, secondCount);
        }

        [TestMethod]
        public void SetDisplay_HpaValue_WithDifferentValue_ShouldSendCommandTwice()
        {
            device.SetDisplay("hPa Value Left", "1013");
            int firstCount = mockMessageSender.DisplayCommandsSent.Count;

            device.SetDisplay("hPa Value Left", "1020");
            int secondCount = mockMessageSender.DisplayCommandsSent.Count;

            Assert.IsGreaterThan(firstCount, secondCount);

            List<byte[]> expectedFirstCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x60, 0x7A, 0x02 },
                RefreshCommand
            };

            // 1020: '1'=0x60 '0'=0x7D '2'=0x3E '0'=0x7D.
            List<byte[]> expectedSecondCommands = new List<byte[]>()
            {
                new byte[] { 0x0D, 0xBF, 0x00, 0x00, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x60, 0x7D, 0x3E, 0x7D, 0x02 },
                RefreshCommand
            };

            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedFirstCommands);
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[1].Commands, expectedSecondCommands);
        }

        [TestMethod]
        public void SetDisplay_WithNullOrWhiteSpace_ShouldNotSendCommand()
        {
            device.SetDisplay("hPa Value Left", null);
            device.SetDisplay("hPa Value Left", "");
            device.SetDisplay("hPa Value Left", "   ");

            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
        }

        #endregion

        #region Brightness Tests

        [TestMethod]
        [DataRow("Backlight Percentage", (byte)0x00)]
        [DataRow("LCD Percentage",       (byte)0x01)]
        [DataRow("LED Percentage",       (byte)0x11)]
        public void SetDisplay_BrightnessControl_SendsBrightnessMessage(string name, byte expectedType)
        {
            device.SetDisplay(name, "50");

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual(expectedType, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("50", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        #endregion

        #region Negative Tests

        [TestMethod]
        public void SetDisplay_UnknownDisplayName_ThrowsKeyNotFoundException()
        {
            // Documents current behaviour: unknown display names hit the
            // LcdCurrentValuesCache indexer and throw. Callers must use
            // names from GetDisplayNames() only.
            Assert.ThrowsExactly<KeyNotFoundException>(
                () => device.SetDisplay("DOES_NOT_EXIST", "1"));
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
