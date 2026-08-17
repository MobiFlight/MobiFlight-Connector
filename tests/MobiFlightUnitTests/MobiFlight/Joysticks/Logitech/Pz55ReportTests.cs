using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MobiFlight.Joysticks.Logitech.Tests
{
    [TestClass]
    public class Pz55ReportTests
    {
        [TestMethod]
        public void Parse_EmptyState_HasNoActiveInputs()
        {
            var state = Pz55Report.Parse(new byte[] { 0x00, 0x00, 0x00 }).ToJoystickState();

            Assert.HasCount(Pz55Report.ButtonCount, state.Buttons);
            for (var index = 0; index < state.Buttons.Length; index++)
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
        public void Parse_MapsReportBitsToStableButtonIds(int byte0, int byte1, int byte2, int expectedButton)
        {
            var state = Pz55Report.Parse(new[] { (byte)byte0, (byte)byte1, (byte)byte2 }).ToJoystickState();

            for (var index = 0; index < state.Buttons.Length; index++)
            {
                Assert.AreEqual(index == expectedButton, state.Buttons[index], $"Unexpected state for button {index}.");
            }
        }

        [TestMethod]
        public void Parse_PreservesSimultaneousMaintainedSwitchStates()
        {
            var state = Pz55Report.Parse(new byte[] { 0x03, 0x11, 0x08 }).ToJoystickState();

            Assert.IsTrue(state.Buttons[0]);
            Assert.IsTrue(state.Buttons[1]);
            Assert.IsTrue(state.Buttons[8]);
            Assert.IsTrue(state.Buttons[12]);
            Assert.IsTrue(state.Buttons[19]);
        }

        [TestMethod]
        public void Parse_ShortPayload_Throws()
        {
            Assert.ThrowsExactly<ArgumentException>(() => Pz55Report.Parse(new byte[] { 0x00, 0x00 }));
        }

        [TestMethod]
        public void Parse_PaddedPayload_IgnoresTrailingBytes()
        {
            var state = Pz55Report.Parse(new byte[] { 0x01, 0x00, 0x00, 0xFF }).ToJoystickState();

            Assert.IsTrue(state.Buttons[0]);
            Assert.IsFalse(state.Buttons[19]);
        }
    }
}
