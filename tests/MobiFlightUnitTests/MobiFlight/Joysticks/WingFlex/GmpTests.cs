using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks.WingFlex.Tests
{
    [TestClass()]
    public class GmpTests
    {
        // Exposes Gmp's protected members for testing.
        private class TestableGmp : Gmp
        {
            public TestableGmp(JoystickDefinition definition) : base(null, definition) { }

            public void EnumerateOutputDevicesPublic() => EnumerateOutputDevices();

            public byte[] ProcessBrakePressureDisplayPublic(string clampedText, byte[] data, JoystickOutputDisplay light)
                => ProcessBrakePressureDisplay(clampedText, data, light);
        }

        [TestMethod()]
        public void StringToGmpDisplayBytes_MapsDigitsDashAndBlank()
        {
            var result = "12-A".StringToGmpDisplayBytes();

            CollectionAssert.AreEqual(new byte[] { 1, 2, 0xA, 0xF }, result);
        }

        [TestMethod()]
        public void StringToGmpDisplayBytes_EmptyString_ReturnsEmptyArray()
        {
            var result = "".StringToGmpDisplayBytes();

            Assert.HasCount(0, result);
        }

        [TestMethod()]
        public void IsBrakePressureLcd_ReturnsTrueForKnownBrakePressureBytes()
        {
            Assert.IsTrue(Gmp.IsBrakePressureLcd(22));
            Assert.IsTrue(Gmp.IsBrakePressureLcd(24));
            Assert.IsTrue(Gmp.IsBrakePressureLcd(26));
        }

        [TestMethod()]
        public void IsBrakePressureLcd_ReturnsFalseForOtherBytes()
        {
            Assert.IsFalse(Gmp.IsBrakePressureLcd(8));
            Assert.IsFalse(Gmp.IsBrakePressureLcd(12));
            Assert.IsFalse(Gmp.IsBrakePressureLcd(18));
        }

        [TestMethod()]
        public void ProcessClockDisplay_WritesDisplayBytesAtOffset()
        {
            var data = new byte[16];
            var display = new JoystickOutputDisplay { Byte = 8 };

            var result = Gmp.ProcessClockDisplay("12-A", data, display);

            CollectionAssert.AreEqual(new byte[] { 1, 2, 0xA, 0xF }, result.Skip(8).Take(4).ToArray());
        }

        [TestMethod()]
        public void ProcessBrakePressureDisplay_ValidNumber_WritesLittleEndianBytes()
        {
            var gmp = new TestableGmp(null);
            var data = new byte[34];
            var light = new JoystickOutputDisplay { Byte = 22 };

            var result = gmp.ProcessBrakePressureDisplayPublic("1234", data, light);

            // 1234 = 0x04D2
            Assert.AreEqual(0xD2, result[22]);
            Assert.AreEqual(0x04, result[23]);
        }

        [TestMethod()]
        public void ProcessBrakePressureDisplay_UnparsableText_WritesZero()
        {
            var gmp = new TestableGmp(null);
            var data = new byte[34];
            data[22] = 0xAA;
            data[23] = 0xBB;
            var light = new JoystickOutputDisplay { Byte = 22 };

            var result = gmp.ProcessBrakePressureDisplayPublic("----", data, light);

            Assert.AreEqual(0, result[22]);
            Assert.AreEqual(0, result[23]);
        }

        [TestMethod()]
        public void EnumerateOutputDevices_AddsOutputsAndLcdDisplays()
        {
            var definition = new JoystickDefinition
            {
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id = "led1", Label = "LED 1", Byte = 1, Bit = 0 },
                    new JoystickOutput { Id = "utc.lcd", Label = "UTC", Type = "LcdDisplay", Byte = 12, Cols = 6, Lines = 1 }
                }
            };
            var gmp = new TestableGmp(definition);

            gmp.EnumerateOutputDevicesPublic();
            var outputs = gmp.GetAvailableOutputDevices();

            Assert.HasCount(2, outputs);

            var lcd = outputs.OfType<JoystickOutputDisplay>().Single();
            Assert.AreEqual("utc.lcd", lcd.Name);
            Assert.AreEqual(6, lcd.Cols);
            Assert.AreEqual(1, lcd.Lines);
            Assert.AreEqual(12, lcd.Byte);

            var led = outputs.Single(o => o is not JoystickOutputDisplay);
            Assert.AreEqual("led1", led.Name);
        }

        [TestMethod()]
        public void GetConnectedOutputDeviceTypes_ReturnsDistinctTypes()
        {
            var definition = new JoystickDefinition
            {
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id = "led1", Byte = 1, Bit = 0 },
                    new JoystickOutput { Id = "led2", Byte = 1, Bit = 1 },
                    new JoystickOutput { Id = "utc.lcd", Type = "LcdDisplay", Byte = 12 }
                }
            };
            var gmp = new Gmp(null, definition);

            var types = gmp.GetConnectedOutputDeviceTypes().ToList();

            CollectionAssert.AreEquivalent(new[] { DeviceType.Output, DeviceType.LcdDisplay }, types);
        }

        [TestMethod()]
        public void GetConnectedOutputDeviceTypes_NoOutputs_ReturnsEmpty()
        {
            var definition = new JoystickDefinition { Outputs = null };
            var gmp = new Gmp(null, definition);

            var types = gmp.GetConnectedOutputDeviceTypes().ToList();

            Assert.HasCount(0, types);
        }
    }
}
