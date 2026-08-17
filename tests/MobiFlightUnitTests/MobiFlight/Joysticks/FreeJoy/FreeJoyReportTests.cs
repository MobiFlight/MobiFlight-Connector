using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.FreeJoy.Tests
{
    /// <summary>
    /// Pins the layout of the FreeJoy external LED bitmask. Offsets are payload-relative,
    /// i.e. without the leading report ID byte, matching the protocol table in
    /// <see cref="FreeJoyReport"/>.
    /// </summary>
    [TestClass]
    public class FreeJoyReportTests
    {
        private static JoystickOutputDevice Led(int index, byte state)
        {
            return new JoystickOutputDevice()
            {
                Name = $"led.{index}",
                Label = $"LED {index}",
                Byte = (byte)(index / 8),
                Bit = (byte)(index % 8),
                State = state
            };
        }

        [TestMethod]
        public void FromOutputDeviceState_NoLights_ReturnsEmptyBitmask()
        {
            var result = FreeJoyReport.FromOutputDeviceState(new List<JoystickOutputDevice>());

            Assert.HasCount(FreeJoyReport.BitmaskLength, result);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, result);
        }

        [TestMethod]
        public void FromOutputDeviceState_NullState_ReturnsEmptyBitmask()
        {
            var result = FreeJoyReport.FromOutputDeviceState(null);

            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, result);
        }

        [TestMethod]
        public void FromOutputDeviceState_SetsOneBitPerLed()
        {
            var state = new List<JoystickOutputDevice>() { Led(0, 1), Led(9, 1), Led(23, 1) };

            var result = FreeJoyReport.FromOutputDeviceState(state);

            CollectionAssert.AreEqual(new byte[] { 0x01, 0x02, 0x80, 0x00 }, result);
        }

        [TestMethod]
        public void FromOutputDeviceState_UnlitLedsStayCleared()
        {
            var state = new List<JoystickOutputDevice>() { Led(3, 1), Led(4, 0) };

            var result = FreeJoyReport.FromOutputDeviceState(state);

            CollectionAssert.AreEqual(new byte[] { 0x08, 0x00, 0x00, 0x00 }, result);
        }

        [TestMethod]
        public void FromOutputDeviceState_AllLedsLit_FillsFirstThreeBytes()
        {
            var state = new List<JoystickOutputDevice>();
            for (var i = 0; i < FreeJoyReport.MaxLedCount; i++)
            {
                state.Add(Led(i, 1));
            }

            var result = FreeJoyReport.FromOutputDeviceState(state);

            CollectionAssert.AreEqual(new byte[] { 0xFF, 0xFF, 0xFF, 0x00 }, result);
        }

        [TestMethod]
        public void FromOutputDeviceState_LedBeyondSupportedCount_IsIgnored()
        {
            // The bitmask has room for 32 bits but the firmware only drives 24 LEDs.
            // Such an output must be dropped rather than land on a bit the board ignores.
            var state = new List<JoystickOutputDevice>() { Led(FreeJoyReport.MaxLedCount, 1) };

            var result = FreeJoyReport.FromOutputDeviceState(state);

            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, result);
        }

        [TestMethod]
        public void FromOutputDeviceState_OutOfRangeAddress_IsIgnored()
        {
            var state = new List<JoystickOutputDevice>()
            {
                new JoystickOutputDevice() { Byte = (byte)FreeJoyReport.BitmaskLength, Bit = 0, State = 1 },
                new JoystickOutputDevice() { Byte = 0, Bit = 8, State = 1 }
            };

            var result = FreeJoyReport.FromOutputDeviceState(state);

            CollectionAssert.AreEqual(new byte[] { 0, 0, 0, 0 }, result);
        }
    }
}
