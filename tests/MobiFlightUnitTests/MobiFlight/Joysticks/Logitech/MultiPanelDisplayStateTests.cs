
using System;

namespace MobiFlight.Joysticks.Logitech.Tests
{
    [TestClass]
    public class MultiPanelDisplayStateTests
    {
        private const byte Off = 0x0F;
        private const byte Dash = 0xDE;

        [TestMethod]
        public void SetDisplay_AltSelector_EncodesAltAndVsOnBothRows()
        {
            var display = new MultiPanelDisplayState();

            display.SetDisplay(MultiPanelSelector.Alt, 123, 45, 67, 89, 12);

            var report = new byte[13];
            display.WriteInto(report);

            CollectionAssert.AreEqual(
            new byte[] { Off, Off, 1, 2, 3 },
            report[1..6]);

            CollectionAssert.AreEqual(
                new byte[] { Off, Off, Off, 4, 5 },
                report[6..11]);
        }

        [TestMethod]
        public void SetDisplay_VsSelector_EncodesAltAndVsOnBothRows()
        {
            var display = new MultiPanelDisplayState();

            display.SetDisplay(MultiPanelSelector.Vs, 12345, -1234, 0, 0, 0);

            var report = new byte[13];
            display.WriteInto(report);

            CollectionAssert.AreEqual(
                new byte[] { 1, 2, 3, 4, 5 },
                report[1..6]);

            CollectionAssert.AreEqual(
                new byte[] { Dash, 1, 2, 3, 4 },
                report[6..11]);
        }

        [TestMethod]
        [DataRow(2, 67)] // IAS
        [DataRow(3, 89)] // HDG
        [DataRow(4, 12)] // CRS
        public void SetDisplay_ThreeDigitSelector_UsesTopRowAndBlanksBottom(
            int selectorValue,
            int expectedValue)
        {
            var selector = (MultiPanelSelector)selectorValue;
            var display = new MultiPanelDisplayState();

            display.SetDisplay(selector, 123, 456, 67, 89, 12);

            var report = new byte[13];
            display.WriteInto(report);

            var expectedDigits = expectedValue.ToString();
            var expectedTop = new byte[] { Off, Off, Off, Off, Off };

            for (int i = 0; i < expectedDigits.Length; i++)
            {
                expectedTop[5 - expectedDigits.Length + i] =
                    (byte)(expectedDigits[i] - '0');
            }

            CollectionAssert.AreEqual(expectedTop, report[1..6]);
            CollectionAssert.AreEqual(
                new byte[] { Off, Off, Off, Off, Off },
                report[6..11]);
        }

        [TestMethod]
        public void SetDisplay_ValuesExceedingDigitCapacity_KeepRightmostDigits()
        {
            var display = new MultiPanelDisplayState();

            display.SetDisplay(MultiPanelSelector.Alt, 123456, 0, 0, 0, 0);

            var report = new byte[13];
            display.WriteInto(report);

            CollectionAssert.AreEqual(
                new byte[] { 2, 3, 4, 5, 6 },
                report[1..6]);
        }

        [TestMethod]
        public void SetDisplay_NegativeVsValue_UsesDashBeforeFourDigits()
        {
            var display = new MultiPanelDisplayState();

            display.SetDisplay(MultiPanelSelector.Vs, 100, -42, 0, 0, 0);

            var report = new byte[13];
            display.WriteInto(report);

            CollectionAssert.AreEqual(
                new byte[] { Off, Off, Dash, 4, 2 },
                report[6..11]);
        }

        [TestMethod]
        public void SetDisplay_NonNegativeVsValue_DoesNotUseDash()
        {
            var display = new MultiPanelDisplayState();

            display.SetDisplay(MultiPanelSelector.Vs, 100, 42, 0, 0, 0);

            var report = new byte[13];
            display.WriteInto(report);

            CollectionAssert.AreEqual(
                new byte[] { Off, Off, Off, 4, 2 },
                report[6..11]);
        }

        [TestMethod]
        public void SetDisplay_InvalidSelector_Throws()
        {
            var display = new MultiPanelDisplayState();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => display.SetDisplay(
                    (MultiPanelSelector)99, 0, 0, 0, 0, 0));
        }

        [TestMethod]
        public void WriteInto_WritesRowsAndPreservesOtherReportBytes()
        {
            var display = new MultiPanelDisplayState();
            display.SetDisplay(MultiPanelSelector.Hdg, 0, 0, 0, 315, 0);

            var report = new byte[13];
            Array.Fill(report, (byte)0xAA);

            display.WriteInto(report);

            Assert.AreEqual((byte)0xAA, report[0]);
            Assert.AreEqual((byte)0xAA, report[11]);
            Assert.AreEqual((byte)0xAA, report[12]);

            CollectionAssert.AreEqual(
                new byte[] { Off, Off, 3, 1, 5 },
                report[1..6]);

            CollectionAssert.AreEqual(
                new byte[] { Off, Off, Off, Off, Off },
                report[6..11]);
        }

        [TestMethod]
        public void WriteInto_ShortBuffer_ThrowsArgumentException()
        {
            var display = new MultiPanelDisplayState();

            Assert.ThrowsExactly<ArgumentException>(
                () => display.WriteInto(new byte[10]));
        }
    }
}
