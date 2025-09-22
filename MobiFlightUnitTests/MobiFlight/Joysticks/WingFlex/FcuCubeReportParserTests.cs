using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Joysticks.WingFlex;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks.WingFlex.Tests
{
    [TestClass]
    public class FcuCubeReportParserTests
    {
        private FcuCubeReport _report;

        [TestInitialize]
        public void SetUp()
        {
            _report = new FcuCubeReport();
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
            Assert.IsInstanceOfType(result, typeof(FcuCubeReport));
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
            Assert.IsTrue(result.Length >= 23, "Output buffer should be at least 23 bytes");
            
            // Check header bytes (with report ID offset)
            Assert.AreEqual(0xF2, result[0], "Header byte 0 should be 0xF2");
            Assert.AreEqual(0xE1, result[1], "Header byte 1 should be 0xE1");
            Assert.AreEqual(0x03, result[2], "Header byte 2 should be 0x03");
        }

        [TestMethod]
        public void FromOutputDeviceState_LEDOutput_SetsBitCorrectly()
        {
            // Arrange
            var devices = new List<JoystickOutputDevice>
            {
                new JoystickOutputDevice 
                { 
                    Name = "Test LED", 
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
        public void FromOutputDeviceState_LcdDisplay_HandlesMaxValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            { 
                Type = DeviceType.LcdDisplay, 
                Byte = 17, 
                Text = "32767" // Max Int16 (since code uses Int16.TryParse)
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // 32767 = 0x7FFF
            Assert.AreEqual(0x7F, result[17], "High byte should be 0x7F");
            Assert.AreEqual(0xFF, result[18], "Low byte should be 0xFF");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_HandlesZeroValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            { 
                Type = DeviceType.LcdDisplay, 
                Byte = 19, 
                Text = "0" 
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            Assert.AreEqual(0x00, result[19], "High byte should be 0x00");
            Assert.AreEqual(0x00, result[20], "Low byte should be 0x00");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_HandlesNegativeValue()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            { 
                Type = DeviceType.LcdDisplay, 
                Byte = 15, 
                Text = "-1" 
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // -1 = 0xFFFF in two's complement
            Assert.AreEqual(0xFF, result[15], "High byte should be 0xFF");
            Assert.AreEqual(0xFF, result[16], "Low byte should be 0xFF");
        }

        [TestMethod]
        public void FromOutputDeviceState_LcdDisplay_InvalidText_SkipsProcessing()
        {
            // Arrange
            var lcdDisplay = new JoystickOutputDisplay
            { 
                Type = DeviceType.LcdDisplay, 
                Byte = 15, 
                Text = "ABC" // Non-numeric
            };
            var devices = new List<JoystickOutputDevice> { lcdDisplay };

            // Act
            var result = _report.FromOutputDeviceState(devices);

            // Assert
            // Should remain at initialized values (0x00)
            Assert.AreEqual(0x00, result[15], "High byte should remain 0x00 for invalid text");
            Assert.AreEqual(0x00, result[16], "Low byte should remain 0x00 for invalid text");
        }

        #endregion

        #region Two's Complement Tests

        [TestMethod]
        public void TwosComplement_PositiveValue_ReturnsCorrectBytes()
        {
            // Test positive encoder values (right rotation)
            sbyte positiveValue = 5;
            byte byteValue = (byte)positiveValue;
            
            Assert.AreEqual(5, byteValue, "Positive value should remain unchanged");
            Assert.IsTrue(positiveValue > 0, "Should be detected as positive");
        }

        [TestMethod]
        public void TwosComplement_NegativeValue_ReturnsCorrectBytes()
        {
            // Test negative encoder values (left rotation)
            sbyte negativeValue = -5;
            byte byteValue = (byte)negativeValue;
            
            Assert.AreEqual(251, byteValue, "Negative value should be 251 in two's complement"); // 256 - 5 = 251
            Assert.IsTrue(((sbyte)byteValue) < 0, "Should be detected as negative when cast back to sbyte");
        }

        [TestMethod]
        public void TwosComplement_ExtremValues_HandleCorrectly()
        {
            // Test extreme values
            sbyte maxPositive = 127;
            sbyte maxNegative = -128;
            
            Assert.AreEqual(127, (byte)maxPositive, "Max positive should be 127");
            Assert.AreEqual(128, (byte)maxNegative, "Max negative should be 128");
            
            Assert.IsTrue(((sbyte)(byte)maxPositive) > 0, "Max positive should remain positive");
            Assert.IsTrue(((sbyte)(byte)maxNegative) < 0, "Max negative should remain negative");
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
            buffer[3] = 0x03;
            
            // Set data type and length markers
            buffer[4] = 0x02; // Data type total
            buffer[5] = 0x01; // Bit type
            buffer[6] = 0x02; // Data length for buttons (2 bytes)
            
            // Buttons in bytes 7-9 (mapped to 6-8 in comments)
            buffer[7] = 0x00;
            buffer[8] = 0x00;
            buffer[9] = 0x00;
            
            // Encoder section header
            buffer[10] = 0x02; // Single byte type
            buffer[11] = 0x06; // 6 bytes of encoder data
            
            // Encoder values (bytes 12-15 mapped to 11-14 in comments)
            buffer[12] = 0x00; // SPD encoder
            buffer[13] = 0x00; // HDG encoder
            buffer[14] = 0x00; // ALT encoder
            buffer[15] = 0x00; // VS encoder
            
            // Brightness values
            buffer[16] = 0xFF; // Background brightness
            
            return buffer;
        }

        #endregion
    }
}