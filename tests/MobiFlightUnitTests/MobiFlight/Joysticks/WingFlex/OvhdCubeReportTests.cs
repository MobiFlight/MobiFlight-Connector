using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.WingFlex.Tests
{
    [TestClass]
    public class OvhdCubeReportTests
    {
        private OvhdCubeReport _report;

        [TestInitialize]
        public void SetUp()
        {
            _report = new OvhdCubeReport();
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
            Assert.IsInstanceOfType(result, typeof(OvhdCubeReport));
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
            Assert.IsGreaterThanOrEqualTo(27, result.Length, "Output buffer should be at least 27 bytes");

            // Check header bytes (with report ID offset)
            Assert.AreEqual(0xF2, result[0], "Header byte 0 should be 0xF2");
            Assert.AreEqual(0xE1, result[1], "Header byte 1 should be 0xE1");
            Assert.AreEqual(0x07, result[2], "Header byte 2 should be 0x07");
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
            var report = new FcuCubeReport();

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
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 18, State = 64 }, // Background
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 19, State = 128 },// Volt L
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 20, State = 192 } // Volt R
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(64, result[18], "Background brightness should be set correctly");
            Assert.AreEqual(128, result[19], "Volt L brightness should be set correctly");
            Assert.AreEqual(192, result[20], "Volt R brightness should be set correctly");
        }

        #endregion

        #region LCD Display Tests

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_L_ParsesNumericValue_9()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 23,
                Cols = 3,
                Text = "9"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show nothing => 0x0000, so high byte = 0x00, low byte = 0x00
            // Right digit group will show 9 => 0x0009, so high byte = 0x00, low byte = 0x09
            Assert.AreEqual(0x00, result[23], "High byte should be 0x00");
            Assert.AreEqual(0x09, result[24], "Low byte should be 0x09");

            // And the Volt Display L has to be turned on
            Assert.AreEqual(0x01, result[15], "Volt Display L should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_L_ParsesNumericValue_99()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 23,
                Cols = 3,
                Text = "99"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show nothing => 0x0000, so high byte = 0x00, low byte = 0x00
            // Right digit group will show 99 => 0x0063, so high byte = 0x00, low byte = 0x63
            Assert.AreEqual(0x00, result[23], "High byte should be 0x00");
            Assert.AreEqual(0x63, result[24], "Low byte should be 0x63");

            // And the Volt Display L has to be turned on
            Assert.AreEqual(0x01, result[15], "Volt Display L should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_L_ParsesNumericValue_999()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 23,
                Cols = 3,
                Text = "999"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x03, result[23], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[24], "Low byte should be 0xE7");

            // And the Volt Display L has to be turned on
            Assert.AreEqual(0x01, result[15], "Volt Display L should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_L_ParsesNumericValue_99dot9()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 23,
                Cols = 3,
                Text = "99.9"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x03, result[23], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[24], "Low byte should be 0xE7");

            // And the Volt Display L has to be turned on
            Assert.AreEqual(0x01, result[15], "Volt Display L should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_R_ParsesNumericValue_9()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 25,
                Cols = 3,
                Text = "9"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show nothing => 0x0000, so high byte = 0x00, low byte = 0x00
            // Right digit group will show 9 => 0x0009, so high byte = 0x00, low byte = 0x09
            Assert.AreEqual(0x00, result[25], "High byte should be 0x00");
            Assert.AreEqual(0x09, result[26], "Low byte should be 0x09");

            // And the Volt Display R has to be turned on
            Assert.AreEqual(0x02, result[15], "Volt Display R should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_R_ParsesNumericValue_99()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 25,
                Cols = 3,
                Text = "99"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show nothing => 0x0000, so high byte = 0x00, low byte = 0x00
            // Right digit group will show 99 => 0x0063, so high byte = 0x00, low byte = 0x63
            Assert.AreEqual(0x00, result[25], "High byte should be 0x00");
            Assert.AreEqual(0x63, result[26], "Low byte should be 0x63");

            // And the Volt Display R has to be turned on
            Assert.AreEqual(0x02, result[15], "Volt Display R should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_R_ParsesNumericValue_999()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 25,
                Cols = 3,
                Text = "999"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x03, result[25], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[26], "Low byte should be 0xE7");

            // And the Volt Display R has to be turned on
            Assert.AreEqual(0x02, result[15], "Volt Display R should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_R_ParsesNumericValue_99dot9()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 25,
                Cols = 3,
                Text = "99.9"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x03, result[25], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[26], "Low byte should be 0xE7");

            // And the Volt Display R has to be turned on
            Assert.AreEqual(0x02, result[15], "Volt Display R should be turned on");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_HandlesZeroValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 23,
                Cols = 3,
                Text = "123"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);

            // Now set it to zero
            lcdDisplay.Text = "0";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[23], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[24], "Low byte should be 0x00");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_HandlesEmptyStringValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 23,
                Cols = 3,
                Text = "123"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);

            // Now set it to empty string
            lcdDisplay.Text = "";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[23], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[24], "Low byte should be 0x00");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_InvalidText_SkipsProcessing()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 23,
                Cols = 3,
                Text = "123"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);
            lcdDisplay.Text = "ABCD";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[23], "High byte should be 0x00");
            Assert.AreEqual(0x7B, result[24], "Low byte should be 0x7B");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a valid FCU Cube input buffer for testing purposes
        /// </summary>
        private byte[] CreateValidInputBuffer()
        {
            var buffer = new byte[65]; // Typical size for HID report including the report ID
            buffer[0] = 0x01; // Report ID

            // Set header
            buffer[1] = 0xF2;
            buffer[2] = 0xE1;
            buffer[3] = 0x07;

            // Set data type and length markers
            buffer[4] = 0x02; // Data type total
            buffer[5] = 0x01; // Bit type
            buffer[6] = 0x14; // Data length for buttons (20 bytes)

            // Buttons in bytes 7-26 (mapped to 6-25 in comments)
            for (int i = 7; i <= 26; i++)
            {
                buffer[i] = 0x00;
            }

            // Axis value
            buffer[29] = 0x00; // OVHD INTEG LT

            return buffer;
        }

        #endregion
    }
}