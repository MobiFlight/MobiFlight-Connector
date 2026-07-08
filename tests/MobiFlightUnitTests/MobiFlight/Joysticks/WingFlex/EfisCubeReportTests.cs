using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.WingFlex.Tests
{
    [TestClass]
    public class EfisCubeReportTests
    {
        private EfisCubeReport _report;

        [TestInitialize]
        public void SetUp()
        {
            _report = new EfisCubeReport();
        }

        #region Parse Tests

        [TestMethod]
        public void Parse_ValidInputBuffer_ReturnsNewReportInstance()
        {
            // Arrange
            var inputBuffer = CreateValidInputBuffer();

            // Act
            var result = _report.Parse(inputBuffer);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotSame(_report, result);
            Assert.IsInstanceOfType(result, typeof(EfisCubeReport));
        }

        [TestMethod]
        public void Parse_NullInputBuffer_ThrowsArgumentException()
        {
            // Arrange
            byte[] inputBuffer = null;

            // Act & Assert - Should throw exception
            Assert.ThrowsExactly<ArgumentException>(() => _report.Parse(inputBuffer));
        }

        [TestMethod]
        public void Parse_EmptyInputBuffer_ThrowsArgumentException()
        {
            // Arrange
            var inputBuffer = new byte[0];

            // Act & Assert - Should throw exception
            Assert.ThrowsExactly<ArgumentException>(() => _report.Parse(inputBuffer));
        }

        [TestMethod]
        public void Parse_WrongLengthInputBuffer_ThrowsArgumentException()
        {
            // Arrange
            var inputBuffer = new byte[1];

            // Act & Assert - Should throw exception
            Assert.ThrowsExactly<ArgumentException>(() => _report.Parse(inputBuffer));
        }

        #endregion

        #region FromOutputDeviceState Tests

        [TestMethod]
        public void FromOutputDeviceState_EmptyList_ReturnsValidHeader()
        {
            // Arrange
            var devices = new List<JoystickOutputDevice>();

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsGreaterThanOrEqualTo(23, result.Length, "Output buffer should be at least 23 bytes");

            // Check header bytes (with report ID offset)
            Assert.AreEqual(0xF2, result[0], "Header byte 0 should be 0xF2");
            Assert.AreEqual(0xE1, result[1], "Header byte 1 should be 0xE1");
            Assert.AreEqual(0x05, result[2], "Header byte 2 should be 0x05");
        }

        [TestMethod]
        public void FromOutputDeviceState_LEDOutput_SetsBitCorrectly()
        {
            // Arrange
            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice
                {
                    Type = DeviceType.Output,
                    Byte = 6,
                    Bit = 0,
                    State = 1
                }
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x01, result[6], "Bit 0 in byte 6 should be set");
        }

        [TestMethod]
        public void FromOutputDeviceState_MultipleLEDs_SetsCorrectBits()
        {
            // Arrange
            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 6, Bit = 0, State = 1 },
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 6, Bit = 2, State = 1 },
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 7, Bit = 1, State = 1 }
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x05, result[6], "Bits 0 and 2 should be set in byte 6 (0x01 | 0x04 = 0x05)");
            Assert.AreEqual(0x02, result[7], "Bit 1 should be set in byte 7");
        }

        [TestMethod]
        public void FromOutputDeviceState_LEDOff_ClearsBit()
        {
            // Arrange - Create report instance to maintain state
            var report = new EfisCubeReport();

            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 6, Bit = 0, State = 1 }
            };

            var result1 = report.FromOutputDeviceState(devices);

            // Now turn it off
            devices[0].State = 0;

            // Act
            var result2 = report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x01, result1[6], "Bit should initially be set");
            Assert.AreEqual(0x00, result2[6], "Bit should be cleared when state is 0");
        }

        [TestMethod]
        public void FromOutputDeviceState_BrightnessControl_SetsCorrectBytes()
        {
            // Arrange
            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 11, State = 128 }, // Background brightness
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 12, State = 200 }  // LCD brightness
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(128, result[11], "Background brightness should be set correctly");
            Assert.AreEqual(200, result[12], "LCD brightness should be set correctly");
        }

        #endregion

        #region LCD Display Tests

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "BARO.value",
                Type = DeviceType.LcdDisplay,
                Byte = 15,
                Text = "1234"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // 1234 = 0x04D2, so high byte = 0x04, low byte = 0xD2
            Assert.AreEqual(0x04, result[15], "High byte should be 0x04");
            Assert.AreEqual(0xD2, result[16], "Low byte should be 0xD2");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_NonVS_HandlesMaxValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "BARO.value",
                Type = DeviceType.LcdDisplay,
                Byte = 15,
                Text = UInt16.MaxValue.ToString() // Max UInt16 (since code uses UInt16.TryParse)
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // 65535 = 0xFFFF
            Assert.AreEqual(0xFF, result[15], "High byte should be 0xFF");
            Assert.AreEqual(0xFF, result[16], "Low byte should be 0xFF");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_NonVS_HandlesZeroValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "BARO.value",
                Type = DeviceType.LcdDisplay,
                Byte = 15,
                Text = "1234"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);
            lcdDisplay.Text = "0";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[15], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[16], "Low byte should be 0x00");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_InvalidText_SkipsProcessing()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "BARO.value",
                Type = DeviceType.LcdDisplay,
                Byte = 15,
                Text = "1234"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);
            lcdDisplay.Text = "ABC";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x04, result[15], "High byte should remain 0x04 for invalid text");
            Assert.AreEqual(0xD2, result[16], "Low byte should remain 0xD2 for invalid text");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_EmptyString_BecomesZeroValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "BARO.value",
                Type = DeviceType.LcdDisplay,
                Byte = 15,
                Text = "12345"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);
            lcdDisplay.Text = ""; 

            // Act
            var result = _report.FromOutputDeviceState(devices);
            // Assert
            Assert.AreEqual(0x00, result[15], "High byte should remain 0x00 for empty string input");
            Assert.AreEqual(0x00, result[16], "Low byte should remain 0x00 for empty string input");
        }

        #endregion

        #region Power Management Tests

        [TestMethod]
        public void FromOutputDeviceState_EmptyList_PowerDefaultsToOn()
        {
            // Arrange
            var devices = new List<JoystickOutputDevice>();

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x01, result[8] & 0x01, "Power bit should default to ON when no devices are configured");
        }

        [TestMethod]
        public void FromOutputDeviceState_PowerExplicitlySetToOff_TurnsOff()
        {
            // Arrange - Explicit power control OFF (aircraft bus simulation)
            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice
                {
                    Name = "Power",
                    Type = DeviceType.Output,
                    Byte = 8,
                    Bit = 0,
                    State = 0
                },
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 6, Bit = 1, State = 1 } // AP1 Signal
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[8] & 0x01, "Power should be OFF when explicitly set to OFF");
            Assert.AreEqual(0x02, result[6] & 0x02, "AP1 signal should still be set in buffer");
        }

        [TestMethod]
        public void FromOutputDeviceState_PowerExplicitlySetToOn_RemainsOn()
        {
            // Arrange
            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice { Name = "Power", Type = DeviceType.Output, Byte = 8, Bit = 0, State = 1 },
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 6, Bit = 1, State = 1 }
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x01, result[8] & 0x01, "Power should remain ON when explicitly set to ON");
        }

        [TestMethod]
        public void FromOutputDeviceState_SingleTestOutput_PowerAutoEnabled()
        {
            // Arrange - Single LED for test mode
            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 6, Bit = 1, State = 1 } // AP1 Signal
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x01, result[8] & 0x01, "Power should be automatically enabled for single test mode");
            Assert.AreEqual(0x02, result[6] & 0x02, "AP1 signal should be set");
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a valid EFIS Cube input buffer for testing purposes
        /// </summary>
        private byte[] CreateValidInputBuffer()
        {
            var buffer = new byte[65]; // Typical size for HID report including the report ID
            buffer[0] = 0x01; // Report ID

            // Set header
            buffer[1] = 0xF2;
            buffer[2] = 0xE1;
            buffer[3] = 0x05;

            // Set data type and length markers
            buffer[4] = 0x02; // Data type total
            buffer[5] = 0x01; // Bit type
            buffer[6] = 0x04; // Data length for buttons (4 bytes)

            // Buttons in bytes 7-10 (mapped to 6-9 in comments)
            buffer[7] = 0x00;
            buffer[8] = 0x00;
            buffer[9] = 0x00;
            buffer[10] = 0x00;

            // Encoder section header
            buffer[11] = 0x02; // Single byte type
            buffer[12] = 0x01; // 1 bytes of encoder data

            // Encoder values (bytes 12-15 mapped to 11-14 in comments)
            buffer[13] = 0x00; // Baro encoder

            return buffer;
        }

        #endregion
    }
}