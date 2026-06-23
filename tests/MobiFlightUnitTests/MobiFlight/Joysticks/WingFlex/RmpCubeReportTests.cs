using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.WingFlex.Tests
{
    [TestClass]
    public class RmpCubeReportTests
    {
        private RmpCubeReport _report;

        [TestInitialize]
        public void SetUp()
        {
            _report = new RmpCubeReport();
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
            Assert.IsInstanceOfType(result, typeof(RmpCubeReport));
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
            Assert.AreEqual(0x06, result[2], "Header byte 2 should be 0x06");
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
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 11, State = 128 }, // LCD brightness
                new JoystickOutputDevice { Type = DeviceType.Output, Byte = 12, State = 200 }  // Background brightness
            };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(128, result[11], "LCD brightness should be set correctly");
            Assert.AreEqual(200, result[12], "Background brightness should be set correctly");
        }

        #endregion

        #region LCD Display Tests
        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_9()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "9"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show nothing => 0x0000, so high byte = 0x00, low byte = 0x00
            // Right digit group will show 9 => 0x0009, so high byte = 0x00, low byte = 0x09
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[20], "Low byte should be 0x00");
            Assert.AreEqual(0x00, result[21], "High byte should be 0x00");
            Assert.AreEqual(0x09, result[22], "Low byte should be 0x09");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x01, result[15], "There should be 1 digit set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_99()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "99"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show 0 => 0x0000, so high byte = 0x00, low byte = 0x00
            // Right digit group will show 99 => 0x0063, so high byte = 0x00, low byte = 0x63
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[20], "Low byte should be 0x00");
            Assert.AreEqual(0x00, result[21], "High byte should be 0x00");
            Assert.AreEqual(0x63, result[22], "Low byte should be 0x63");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x03, result[15], "There should be 2 digits set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_999()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "999"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show 0 => 0x0000, so high byte = 0x00, low byte = 0x00
            // Right digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[20], "Low byte should be 0x00");
            Assert.AreEqual(0x03, result[21], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[22], "Low byte should be 0xE7");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x07, result[15], "There should be 3 digits set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_9999()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "9999"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show 9 => 0x0009, so high byte = 0x00, low byte = 0x09
            // Right digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x09, result[20], "Low byte should be 0x09");
            Assert.AreEqual(0x03, result[21], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[22], "Low byte should be 0xE7");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x0F, result[15], "There should be 4 digits set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_99999()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "99999"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show 99 => 0x0063, so high byte = 0x00, low byte = 0x63
            // Right digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x63, result[20], "Low byte should be 0x63");
            Assert.AreEqual(0x03, result[21], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[22], "Low byte should be 0xE7");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x1F, result[15], "There should be all 5 digits set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_999999()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "999999"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            // Right digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x03, result[19], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[20], "Low byte should be 0xE7");
            Assert.AreEqual(0x03, result[21], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[22], "Low byte should be 0xE7");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x3F, result[15], "There should be all 6 digits set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_999dot999()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "999.999"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            // Right digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x03, result[19], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[20], "Low byte should be 0xE7");
            Assert.AreEqual(0x03, result[21], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[22], "Low byte should be 0xE7");

            // Assert dot position
            Assert.AreEqual(0x03, result[13], "Dot position byte should indicate dot after 3rd digit");

            // Assert active digits
            Assert.AreEqual(0x3F, result[15], "There should be all 6 digits set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_ParsesNumericValue_99dot99dot99()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "99.99.99"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Left digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            // Right digit group will show 999 => 0x03E7, so high byte = 0x03, low byte = 0xE7
            Assert.AreEqual(0x03, result[19], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[20], "Low byte should be 0xE7");
            Assert.AreEqual(0x03, result[21], "High byte should be 0x03");
            Assert.AreEqual(0xE7, result[22], "Low byte should be 0xE7");

            // Assert dot position, only first dot is rendered
            Assert.AreEqual(0x02, result[13], "Dot position byte should indicate dot after 2rd digit");

            // Assert active digits
            Assert.AreEqual(0x3F, result[15], "There should be all 6 digits set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_HandlesZeroValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "1234"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);

            // Now set it to zero
            lcdDisplay.Text = "0";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[20], "Low byte should be 0x00");
            Assert.AreEqual(0x00, result[21], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[22], "Low byte should be 0x00");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x01, result[15], "There should be 1 digit set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_HandlesEmptyStringValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "1234"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);

            // Now set it to empty string
            lcdDisplay.Text = "";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[20], "Low byte should be 0x00");
            Assert.AreEqual(0x00, result[21], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[22], "Low byte should be 0x00");

            // Assert dot position -> no dot!
            Assert.AreEqual(0x00, result[13], "There should be no dot set");

            // Assert active digits
            Assert.AreEqual(0x00, result[15], "There should be 0 digit set");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_InvalidText_SkipsProcessing()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            {
                Name = "Active",
                Type = DeviceType.LcdDisplay,
                Byte = 19,
                Cols = 6,
                Text = "1234"
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };
            _report.FromOutputDeviceState(devices);
            lcdDisplay.Text = "ABCD";

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x01, result[20], "Low byte should be 0x01");
            Assert.AreEqual(0x00, result[21], "High byte should be 0x00");
            Assert.AreEqual(0xEA, result[22], "Low byte should be 0xEA");
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
            buffer[3] = 0x06;

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
            buffer[12] = 0x02; // 2 bytes of encoder data

            // Encoder values in bytes 12-13 (mapped to 11-12 in comments)
            buffer[12] = 0x00; // Small encoder
            buffer[13] = 0x00; // Big encoder
            
            return buffer;
        }

        #endregion
    }
}