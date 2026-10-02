using System;
using System.Collections.Generic;
namespace MobiFlight.Joysticks.Logitech.Tests
{
    [TestClass]
    public class MultiPanelReportTests
    {
        [TestMethod]
        public void DefaultConstructor_HasNoActiveButtons()
        {
            var state = new MultiPanelReport().ToJoystickState();
            for (var index = 0; index < MultiPanelReport.ButtonCount; index++)
            {
                Assert.IsFalse(state.Buttons[index], $"Button {index} should be inactive.");
            }
        }
        [TestMethod]
        public void Parse_EmptyState_HasNoActiveInputs()
        {
            var state = MultiPanelReport.Parse(new byte[] { 0x00, 0x00, 0x00 }).ToJoystickState();
            for (var index = 0; index < MultiPanelReport.ButtonCount; index++)
            {
                Assert.IsFalse(state.Buttons[index], $"Button {index} should be inactive.");
            }
        }
        [TestMethod]
        [DataRow(0x01, 0x00, 0x00, 0)]
        [DataRow(0x02, 0x00, 0x00, 1)]
        [DataRow(0x04, 0x00, 0x00, 2)]
        [DataRow(0x08, 0x00, 0x00, 3)]
        [DataRow(0x10, 0x00, 0x00, 4)]
        [DataRow(0x20, 0x00, 0x00, 5)]
        [DataRow(0x40, 0x00, 0x00, 6)]
        [DataRow(0x80, 0x00, 0x00, 7)]
        [DataRow(0x00, 0x01, 0x00, 8)]
        [DataRow(0x00, 0x02, 0x00, 9)]
        [DataRow(0x00, 0x04, 0x00, 10)]
        [DataRow(0x00, 0x08, 0x00, 11)]
        [DataRow(0x00, 0x10, 0x00, 12)]
        [DataRow(0x00, 0x20, 0x00, 13)]
        [DataRow(0x00, 0x40, 0x00, 14)]
        [DataRow(0x00, 0x80, 0x00, 15)]
        [DataRow(0x00, 0x00, 0x01, 16)]
        [DataRow(0x00, 0x00, 0x02, 17)]
        [DataRow(0x00, 0x00, 0x04, 18)]
        [DataRow(0x00, 0x00, 0x08, 19)]
        [DataRow(0x00, 0x00, 0x10, 20)]
        public void Parse_MapsReportBitsToStableButtonIds(int byte0, int byte1, int byte2, int expectedButton)
        {
            var state = MultiPanelReport.Parse(new[] { (byte)byte0, (byte)byte1, (byte)byte2 }).ToJoystickState();
            for (var index = 0; index < MultiPanelReport.ButtonCount; index++)
            {
                Assert.AreEqual(index == expectedButton, state.Buttons[index], $"Unexpected state for button {index}.");
            }
        }
        [TestMethod]
        public void Parse_PreservesSimultaneousActiveButtonStates()
        {
            var state = MultiPanelReport.Parse(new byte[] { 0x81, 0x01, 0x10 }).ToJoystickState();
            Assert.IsTrue(state.Buttons[0]);
            Assert.IsTrue(state.Buttons[7]);
            Assert.IsTrue(state.Buttons[8]);
            Assert.IsTrue(state.Buttons[20]);
            Assert.IsFalse(state.Buttons[1]);
            Assert.IsFalse(state.Buttons[9]);
            Assert.IsFalse(state.Buttons[19]);
        }
        [TestMethod]
        public void Parse_UnmappedBitsInThirdByte_AreIgnored()
        {
            // Bits 5-7 of the third byte fall outside the 21 defined buttons.
            var state = MultiPanelReport.Parse(new byte[] { 0x00, 0x00, 0xE0 }).ToJoystickState();
            for (var index = 0; index < MultiPanelReport.ButtonCount; index++)
            {
                Assert.IsFalse(state.Buttons[index], $"Button {index} should be inactive.");
            }
        }
        [TestMethod]
        public void Parse_ShortPayload_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => MultiPanelReport.Parse(new byte[] { 0x00, 0x00 }));
        }
        [TestMethod]
        public void Parse_PaddedPayload_IgnoresTrailingBytes()
        {
            var state = MultiPanelReport.Parse(new byte[] { 0x01, 0x00, 0x00, 0xFF }).ToJoystickState();
            Assert.IsTrue(state.Buttons[0]);
            Assert.IsFalse(state.Buttons[20]);
        }
        [TestMethod]
        public void Parse_CopiesPayload_SubsequentMutationDoesNotAffectParsedReport()
        {
            var payload = new byte[] { 0x01, 0x00, 0x00 };
            var report = MultiPanelReport.Parse(payload);
            payload[0] = 0x00;
            Assert.IsTrue(report.ToJoystickState().Buttons[0]);
        }

        #region ConvertCharToByte Tests
        [TestMethod]
        [DataRow('0', (byte)0)]
        [DataRow('1', (byte)1)]
        [DataRow('9', (byte)9)]
        public void ConvertCharToByte_Digit_ReturnsDigitValue(char input, byte expected)
        {
            Assert.AreEqual(expected, MultiPanelReport.ConvertCharToByte(input));
        }
        [TestMethod]
        public void ConvertCharToByte_Space_ReturnsBlankCode()
        {
            Assert.AreEqual((byte)0x0F, MultiPanelReport.ConvertCharToByte(' '));
        }
        [TestMethod]
        public void ConvertCharToByte_Dash_ReturnsDashCode()
        {
            Assert.AreEqual((byte)0xDE, MultiPanelReport.ConvertCharToByte('-'));
        }
        [TestMethod]
        [DataRow('A')]
        [DataRow('!')]
        [DataRow('z')]
        public void ConvertCharToByte_UnsupportedCharacter_ReturnsBlankCode(char input)
        {
            Assert.AreEqual((byte)0x0F, MultiPanelReport.ConvertCharToByte(input));
        }
        #endregion

        #region FromOutputDeviceState Tests
        [TestMethod]
        public void FromOutputDeviceState_EmptyList_ReturnsReportIdOnlyBuffer()
        {
            var result = MultiPanelReport.FromOutputDeviceState([]);
            Assert.HasCount(13, result);
            Assert.AreEqual((byte)1, result[0]);
            for (var index = 1; index < result.Length; index++)
            {
                Assert.AreEqual((byte)0, result[index], $"Byte {index} should be untouched.");
            }
        }
        [TestMethod]
        public void FromOutputDeviceState_LedBitsSet_UpdatesByte11()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDevice { Byte = 11, Bit = 0, State = 1 },
                new JoystickOutputDevice { Byte = 11, Bit = 3, State = 1 },
            ];
            var result = MultiPanelReport.FromOutputDeviceState(state);
            Assert.AreEqual((byte)0b0000_1001, result[11]);
        }
        [TestMethod]
        public void FromOutputDeviceState_LedStateZero_LeavesBitCleared()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDevice { Byte = 11, Bit = 2, State = 0 },
            ];
            var result = MultiPanelReport.FromOutputDeviceState(state);
            Assert.AreEqual((byte)0, result[11]);
        }
        [TestMethod]
        public void FromOutputDeviceState_NonLedByte_IsIgnored()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDevice { Byte = 5, Bit = 0, State = 1 },
            ];
            var result = MultiPanelReport.FromOutputDeviceState(state);
            for (var index = 1; index < result.Length; index++)
            {
                Assert.AreEqual((byte)0, result[index], $"Byte {index} should be untouched.");
            }
        }
        [TestMethod]
        public void FromOutputDeviceState_LcdRow_WritesCharacterBytes()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDisplay { Byte = 1, Cols = 5, Text = "12345" },
            ];
            var result = MultiPanelReport.FromOutputDeviceState(state);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3, 4, 5 }, result[1..6]);
        }
        [TestMethod]
        public void FromOutputDeviceState_LcdRow_ShortTextLeavesRemainingColumnsUnchanged()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDisplay { Byte = 1, Cols = 5, Text = "12" },
            ];
            var result = MultiPanelReport.FromOutputDeviceState(state);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 0, 0, 0 }, result[1..6]);
        }
        [TestMethod]
        public void FromOutputDeviceState_LcdRow_ConvertsSpacesAndDashes()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDisplay { Byte = 1, Cols = 2, Text = " -" },
            ];
            var result = MultiPanelReport.FromOutputDeviceState(state);
            Assert.AreEqual((byte)0x0F, result[1]);
            Assert.AreEqual((byte)0xDE, result[2]);
        }
        [TestMethod]
        public void FromOutputDeviceState_LcdByteIndexOutOfRange_Throws()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDisplay { Byte = 13, Cols = 1, Text = "1" },
            ];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => MultiPanelReport.FromOutputDeviceState(state));
        }
        [TestMethod]
        public void FromOutputDeviceState_LcdCharacterIndexOutOfRange_Throws()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDisplay { Byte = 10, Cols = 5, Text = "12345" },
            ];
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => MultiPanelReport.FromOutputDeviceState(state));
        }
        [TestMethod]
        public void FromOutputDeviceState_CombinedLedsAndLcdRows_ProducesExpectedBuffer()
        {
            List<JoystickOutputDevice> state =
            [
                new JoystickOutputDisplay { Byte = 1, Cols = 5, Text = "12345" },
                new JoystickOutputDisplay { Byte = 6, Cols = 5, Text = "67890" },
                new JoystickOutputDevice { Byte = 11, Bit = 0, State = 1 },
                new JoystickOutputDevice { Byte = 11, Bit = 2, State = 1 },
            ];
            var result = MultiPanelReport.FromOutputDeviceState(state);
            byte[] expected = [1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 0b0000_0101, 0];
            CollectionAssert.AreEqual(expected, result);
        }
        #endregion
    }
}