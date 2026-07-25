using MobiFlightWwFcu;
using MobiFlightWwFcuUnitTests.Mocks;
using Newtonsoft.Json.Linq;

namespace MobiFlightWwFcuUnitTests
{
    [TestClass]
    public class WinwingCduDeviceTests
    {
        private MockWinwingMessageSender mockMessageSender = null!;

        [TestInitialize]
        public void Setup()
        {
            mockMessageSender = new MockWinwingMessageSender();
        }

        private WinwingCduDevice CreateDevice(WinwingCduType type)
        {
            return new WinwingCduDevice(mockMessageSender, type);
        }

        private WinwingCduDevice CreateMcdu() => CreateDevice(WinwingCduType.MCDU);

        private void CompareDisplayCommands(List<byte[]> sent, List<byte[]> expected)
        {
            Assert.HasCount(expected.Count, sent);
            for (int i = 0; i < expected.Count; i++)
            {
                CollectionAssert.AreEqual(expected[i], sent[i]);
            }
        }

        #region GetFormatBytes — color mapping (existing)

        [TestMethod]
        [DataRow('a', 0x21, 0x00)]
        [DataRow('w', 0x42, 0x00)]
        [DataRow('c', 0x63, 0x00)]
        [DataRow('g', 0x84, 0x00)]
        [DataRow('m', 0xa5, 0x00)]
        [DataRow('r', 0xc6, 0x00)]
        [DataRow('y', 0xe7, 0x00)]
        [DataRow('o', 0x08, 0x01)]
        [DataRow('e', 0x29, 0x01)]
        [DataRow('k', 0x4a, 0x01)]
        public void GetFormatByts_Large_Expected(
            char color,
            int lowByte,
            int highByte)
        {
            var token = JToken.Parse($"[\"A\", \"{color}\", 0]");
            var (actualLowByte, actualHighByte) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(lowByte, actualLowByte);
            Assert.AreEqual(highByte, actualHighByte);
        }


        [TestMethod]
        [DataRow('a', 0x8c, 0x01)]
        [DataRow('w', 0xad, 0x01)]
        [DataRow('c', 0xce, 0x01)]
        [DataRow('g', 0xef, 0x01)]
        [DataRow('m', 0x10, 0x02)]
        [DataRow('r', 0x31, 0x02)]
        [DataRow('y', 0x52, 0x02)]
        [DataRow('o', 0x73, 0x02)]
        [DataRow('e', 0x94, 0x02)]
        [DataRow('k', 0xb5, 0x02)]
        public void GetFormatByts_Small_Expected(
            char color,
            int lowByte,
            int highByte)
        {
            var token = JToken.Parse($"[\"A\", \"{color}\", 1]");
            var (actualLowByte, actualHighByte) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(lowByte, (int)actualLowByte);
            Assert.AreEqual(highByte, actualHighByte);
        }

        #endregion

        #region GetFormatBytes — extension cases

        [TestMethod]
        public void GetFormatBytes_InvertedFlag_AddsColorInvertOffset()
        {
            // Large white normally is 0x42,0x00. Inverted adds 0x1B → 0x5D,0x00.
            var token = JToken.Parse("[\"A\", \"w\", 0, true]");
            var (low, high) = WinwingCduDevice.GetFormatBytes(token, out var c);

            Assert.AreEqual(0x5D, (int)low);
            Assert.AreEqual(0x00, high);
            Assert.AreEqual('A', c);
        }

        [TestMethod]
        public void GetFormatBytes_NotInvertedFlag_DoesNotAddOffset()
        {
            var token = JToken.Parse("[\"A\", \"w\", 0, false]");
            var (low, high) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(0x42, (int)low);
            Assert.AreEqual(0x00, high);
        }

        [TestMethod]
        public void GetFormatBytes_ThreeElementToken_DefaultsToNotInverted()
        {
            // Backwards compatibility: 3-element tokens stay non-inverted.
            var token = JToken.Parse("[\"A\", \"w\", 0]");
            var (low, _) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(0x42, (int)low);
        }

        [TestMethod]
        public void GetFormatBytes_UnknownColorChar_FallsBackToGrey()
        {
            // 'z' is not a known color → falls back to Grey (0x29, 0x01) for large.
            var token = JToken.Parse("[\"A\", \"z\", 0]");
            var (low, high) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(0x29, (int)low);
            Assert.AreEqual(0x01, high);
        }

        [TestMethod]
        public void GetFormatBytes_EmptyToken_ReturnsSpaceWithDefaultWhite()
        {
            // No values → currentChar ' ', color stays at the local default Color.White (0x42).
            var token = JToken.Parse("[]");
            var (low, high, ch) = ExtractGetFormatBytes(token);

            Assert.AreEqual(' ', ch);
            Assert.AreEqual(0x42, (int)low);
            Assert.AreEqual(0x00, high);
        }

        private static (byte low, byte high, char c) ExtractGetFormatBytes(JToken token)
        {
            var (l, h) = WinwingCduDevice.GetFormatBytes(token, out var ch);
            return (l, h, ch);
        }

        [TestMethod]
        public void GetFormatBytes_SmallInverted_AddsBothOffsets()
        {
            // Large white (0x42) + small (0x16B) + invert (0x1B) = 0x42 + 0x16B + 0x1B = 0x1C8
            // → low 0xC8, high 0x01.
            var token = JToken.Parse("[\"A\", \"w\", 1, true]");
            var (low, high) = WinwingCduDevice.GetFormatBytes(token, out _);

            Assert.AreEqual(0xC8, (int)low);
            Assert.AreEqual(0x01, high);
        }

        #endregion

        #region Constructor / per-CduType identity

        [TestMethod]
        [DataRow((int)WinwingCduType.MCDU,  "WinWing MCDU")]
        [DataRow((int)WinwingCduType.PFP3N, "WinWing PFP3N")]
        [DataRow((int)WinwingCduType.PFP4,  "WinWing PFP4")]
        [DataRow((int)WinwingCduType.PFP7,  "WinWing PFP7")]
        public void Name_PerCduType_ReturnsExpected(int typeAsInt, string expectedName)
        {
            var device = CreateDevice((WinwingCduType)typeAsInt);
            Assert.AreEqual(expectedName, device.Name);
        }

        [TestMethod]
        public void GetLedNames_Mcdu_ReturnsNineLeds()
        {
            var device = CreateMcdu();
            var leds = device.GetLedNames();

            Assert.HasCount(9, leds);
            Assert.Contains("FAIL", leds);
            Assert.Contains("FM", leds);
            Assert.Contains("MCDU", leds);
            Assert.Contains("MENU", leds);
            Assert.Contains("FM1", leds);
            Assert.Contains("IND", leds);
            Assert.Contains("RDY", leds);
            Assert.Contains("STATUS", leds);
            Assert.Contains("FM2", leds);
        }

        [TestMethod]
        [DataRow((int)WinwingCduType.PFP3N, "CALL")]
        [DataRow((int)WinwingCduType.PFP4,  "DSPY")]
        [DataRow((int)WinwingCduType.PFP7,  "DSPY")]
        public void GetLedNames_PfpVariants_ReturnFiveLedsIncludingTypeSpecific(
            int typeAsInt,
            string typeSpecificLed)
        {
            var device = CreateDevice((WinwingCduType)typeAsInt);
            var leds = device.GetLedNames();

            Assert.HasCount(5, leds);
            Assert.Contains("FAIL", leds);
            Assert.Contains("MSG", leds);
            Assert.Contains("OFST", leds);
            Assert.Contains("EXEC", leds);
            Assert.Contains(typeSpecificLed, leds);
        }

        [TestMethod]
        public void GetDisplayNames_ContainsThreeBrightnessNames()
        {
            var device = CreateMcdu();
            var names = device.GetDisplayNames();

            Assert.HasCount(3, names);
            Assert.Contains("Backlight Percentage", names);
            Assert.Contains("LCD Percentage", names);
            Assert.Contains("LED Percentage", names);
        }

        [TestMethod]
        public void GetInternalDisplayNames_ContainsFontDataAndCduData()
        {
            var device = CreateMcdu();
            var names = device.GetInternalDisplayNames();

            Assert.HasCount(2, names);
            Assert.Contains("Font Data", names);
            Assert.Contains("Cdu Data", names);
        }

        #endregion

        #region SetLed

        [TestMethod]
        public void SetLed_StateNonZero_NormalizesValueToOne()
        {
            var device = CreateMcdu();

            device.SetLed("FAIL", 200);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_StateZero_SendsValueZero()
        {
            var device = CreateMcdu();

            // Cache starts at 255, so setting to 0 emits a frame.
            device.SetLed("FAIL", 0);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)0, mockMessageSender.LightControlCommands[0].Value);
        }

        [TestMethod]
        public void SetLed_TargetsMcduDestinationAddress()
        {
            var device = CreateMcdu();

            device.SetLed("FAIL", 1);

            CollectionAssert.AreEqual(
                new byte[] { 0x32, 0xbb },
                mockMessageSender.LightControlCommands[0].Destination);
        }

        [TestMethod]
        [DataRow("FAIL",   0x08)]
        [DataRow("FM",     0x09)]
        [DataRow("MCDU",   0x0a)]
        [DataRow("MENU",   0x0b)]
        [DataRow("FM1",    0x0c)]
        [DataRow("IND",    0x0d)]
        [DataRow("RDY",    0x0e)]
        [DataRow("STATUS", 0x0f)]
        [DataRow("FM2",    0x10)]
        public void SetLed_McduLeds_SendExpectedTypeByte(string ledName, int expectedType)
        {
            var device = CreateMcdu();

            device.SetLed(ledName, 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)expectedType, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        [DataRow("CALL", 0x03)]
        [DataRow("FAIL", 0x04)]
        [DataRow("MSG",  0x05)]
        [DataRow("OFST", 0x06)]
        [DataRow("EXEC", 0x07)]
        public void SetLed_Pfp3nLeds_SendExpectedTypeByte(string ledName, int expectedType)
        {
            var device = CreateDevice(WinwingCduType.PFP3N);

            device.SetLed(ledName, 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)expectedType, mockMessageSender.LightControlCommands[0].Type);
        }

        [TestMethod]
        public void SetLed_SameStateTwice_OnlySendsOnce()
        {
            var device = CreateMcdu();

            device.SetLed("FAIL", 1);
            device.SetLed("FAIL", 1);

            Assert.HasCount(1, mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_DifferentStateAfterFirst_SendsAgain()
        {
            var device = CreateMcdu();

            device.SetLed("FAIL", 1);
            device.SetLed("FAIL", 0);

            Assert.HasCount(2, mockMessageSender.LightControlCommands);
            Assert.AreEqual((byte)1, mockMessageSender.LightControlCommands[0].Value);
            Assert.AreEqual((byte)0, mockMessageSender.LightControlCommands[1].Value);
        }

        [TestMethod]
        public void SetLed_NullName_DoesNothing()
        {
            var device = CreateMcdu();

            device.SetLed(null!, 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
        }

        [TestMethod]
        public void SetLed_EmptyName_DoesNothing()
        {
            var device = CreateMcdu();

            device.SetLed(string.Empty, 1);

            Assert.IsEmpty(mockMessageSender.LightControlCommands);
        }

        #endregion

        #region SetDisplay — brightness

        [TestMethod]
        [DataRow("Backlight Percentage", 0x00)]
        [DataRow("LCD Percentage",       0x01)]
        [DataRow("LED Percentage",       0x02)]
        public void SetDisplay_Brightness_SendsExpectedTypeByte(string name, int expectedType)
        {
            var device = CreateMcdu();

            device.SetDisplay(name, "50");

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
            Assert.AreEqual((byte)expectedType, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("50", mockMessageSender.BrightnessCommands[0].Brightness);
        }

        [TestMethod]
        public void SetDisplay_BrightnessTargetsMcduDestinationAddress()
        {
            var device = CreateMcdu();

            device.SetDisplay("Backlight Percentage", "50");

            CollectionAssert.AreEqual(
                new byte[] { 0x32, 0xbb },
                mockMessageSender.BrightnessCommands[0].DestinationAddress);
        }

        [TestMethod]
        public void SetDisplay_SameBrightnessValueTwice_OnlySendsOnce()
        {
            var device = CreateMcdu();

            device.SetDisplay("Backlight Percentage", "50");
            device.SetDisplay("Backlight Percentage", "50");

            Assert.HasCount(1, mockMessageSender.BrightnessCommands);
        }

        [TestMethod]
        public void SetDisplay_DifferentBrightnessAfterFirst_SendsAgain()
        {
            var device = CreateMcdu();

            device.SetDisplay("Backlight Percentage", "50");
            device.SetDisplay("Backlight Percentage", "75");

            Assert.HasCount(2, mockMessageSender.BrightnessCommands);
            Assert.AreEqual("50", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.AreEqual("75", mockMessageSender.BrightnessCommands[1].Brightness);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\t")]
        public void SetDisplay_NullEmptyOrWhitespaceValue_DoesNothing(string? value)
        {
            var device = CreateMcdu();

            device.SetDisplay("Backlight Percentage", value!);

            Assert.IsEmpty(mockMessageSender.BrightnessCommands);
            Assert.IsEmpty(mockMessageSender.DisplayCommandsSent);
            Assert.IsEmpty(mockMessageSender.CduDisplayBytes);
        }

        #endregion

        #region SetDisplay — Cdu Data passthrough

        [TestMethod]
        public void SetDisplay_CduData_SingleChar_OnlyGetsFirstMarker()
        {
            // The conversion uses if/else-if for the marker: when there is only
            // one cell, i == 0 wins → only +0x01 is added (the last-marker branch
            // is skipped). White large (0x42) → 0x43.
            var device = CreateMcdu();

            device.SetDisplay("Cdu Data",
                "{\"Target\":\"Display\",\"Data\":[[\"A\",\"w\",0]]}");

            Assert.HasCount(1, mockMessageSender.CduDisplayBytes);
            CollectionAssert.AreEqual(
                new byte[] { 0x43, 0x00, (byte)'A' },
                mockMessageSender.CduDisplayBytes[0]);
        }

        [TestMethod]
        public void SetDisplay_CduData_TwoChars_FirstHasPlus1AndLastHasPlus2()
        {
            var device = CreateMcdu();

            device.SetDisplay("Cdu Data",
                "{\"Target\":\"Display\",\"Data\":[[\"A\",\"w\",0],[\"B\",\"w\",0]]}");

            Assert.HasCount(1, mockMessageSender.CduDisplayBytes);
            var bytes = mockMessageSender.CduDisplayBytes[0];
            Assert.HasCount(6, bytes);

            // First cell: white (0x42) + 0x01 = 0x43, char 'A'
            Assert.AreEqual(0x43, bytes[0]);
            Assert.AreEqual(0x00, bytes[1]);
            Assert.AreEqual((byte)'A', bytes[2]);

            // Last cell: white (0x42) + 0x02 = 0x44, char 'B'
            Assert.AreEqual(0x44, bytes[3]);
            Assert.AreEqual(0x00, bytes[4]);
            Assert.AreEqual((byte)'B', bytes[5]);
        }

        [TestMethod]
        public void SetDisplay_CduData_ThreeChars_MiddleCharIsUnmarked()
        {
            var device = CreateMcdu();

            device.SetDisplay("Cdu Data",
                "{\"Target\":\"Display\",\"Data\":[[\"A\",\"w\",0],[\"B\",\"w\",0],[\"C\",\"w\",0]]}");

            Assert.HasCount(1, mockMessageSender.CduDisplayBytes);
            var bytes = mockMessageSender.CduDisplayBytes[0];
            Assert.HasCount(9, bytes);

            // Middle cell: white (0x42), no marker offset, char 'B'
            Assert.AreEqual(0x42, bytes[3]);
            Assert.AreEqual(0x00, bytes[4]);
            Assert.AreEqual((byte)'B', bytes[5]);
        }

        [TestMethod]
        public void SetDisplay_CduData_ExpectedByteSequence_TwoChars()
        {
            // Byte-exact lock-in via the TRX two-stage workflow.
            // 'X' red large + first marker = 0xC6 + 0x01 = 0xC7.
            // 'Y' green small + last marker = 0xEF + 0x02 = 0xF1, high byte 0x01.
            var device = CreateMcdu();

            device.SetDisplay("Cdu Data",
                "{\"Target\":\"Display\",\"Data\":[[\"X\",\"r\",0],[\"Y\",\"g\",1]]}");

            Assert.HasCount(1, mockMessageSender.CduDisplayBytes);
            CollectionAssert.AreEqual(
                new byte[] { 0xC7, 0x00, 0x58, 0xF1, 0x01, 0x59 },
                mockMessageSender.CduDisplayBytes[0]);
        }

        #endregion

        #region Stop / Shutdown

        [TestMethod]
        public void Stop_TurnsOffAllNineMcduLeds()
        {
            var device = CreateMcdu();

            device.Stop();

            Assert.HasCount(9, mockMessageSender.LightControlCommands);
            foreach (var cmd in mockMessageSender.LightControlCommands)
            {
                Assert.AreEqual((byte)0, cmd.Value);
            }
        }

        [TestMethod]
        public void Shutdown_SendsClearFramesPlusBrightnessZeroPlusAllLedsOff()
        {
            var device = CreateMcdu();

            device.Shutdown();

            // EmptyDisplay → 1 SendDisplayCommands call with the ClearCommandSequence (6 frames).
            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            Assert.HasCount(6, mockMessageSender.DisplayCommandsSent[0].Commands);

            // Two brightness=0 calls (backlight then LCD).
            Assert.HasCount(2, mockMessageSender.BrightnessCommands);
            Assert.AreEqual((byte)0x00, mockMessageSender.BrightnessCommands[0].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[0].Brightness);
            Assert.AreEqual((byte)0x01, mockMessageSender.BrightnessCommands[1].Type);
            Assert.AreEqual("0", mockMessageSender.BrightnessCommands[1].Brightness);

            // All 9 LEDs off.
            Assert.HasCount(9, mockMessageSender.LightControlCommands);
            foreach (var cmd in mockMessageSender.LightControlCommands)
            {
                Assert.AreEqual((byte)0, cmd.Value);
            }
        }

        [TestMethod]
        public void Shutdown_McduClearFrames_ExpectedByteSequence()
        {
            // Byte-exact lock-in via the TRX two-stage workflow.
            var device = CreateMcdu();

            device.Shutdown();

            Assert.HasCount(1, mockMessageSender.DisplayCommandsSent);
            List<byte[]> expectedCommands = new List<byte[]>()
            {
                new byte[] { 0x32, 0xBB, 0x00, 0x00, 0x04, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x0E },
                new byte[] { 0x32, 0xBB, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x32, 0xBB, 0x00, 0x00, 0x12, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xFF, 0x06, 0x07, 0x0D },
                new byte[] { 0x32, 0xBB, 0x00, 0x00, 0x13, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xFF, 0x06, 0x07, 0x0D },
                new byte[] { 0x32, 0xBB, 0x00, 0x00, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x02, 0xE0, 0x01 },
                new byte[] { 0x32, 0xBB, 0x00, 0x00, 0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };
            CompareDisplayCommands(mockMessageSender.DisplayCommandsSent[0].Commands, expectedCommands);
        }

        #endregion
    }
}
