using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MobiFlight.Joysticks.WingFlex.Tests
{
    [TestClass]
    public class Dap500ReportTests
    {
        private Dap500Report _report;

        [TestInitialize]
        public void SetUp()
        {
            _report = new Dap500Report();
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
            Assert.IsInstanceOfType(result, typeof(Dap500Report));
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

        #region Dap Input Report Tests
        [TestMethod]
        public void DapInputReport_SensorValueReportedCorrectly()
        {
            // Arrange
            var inputBuffer = CreateValidInputBuffer();
            inputBuffer[4] = 128;

            // Act
            var result = _report.Parse(inputBuffer).ToJoystickState();
            Assert.IsNotNull(result);
            Assert.AreEqual(128, result.X);

            // Verify with another value to make sure
            // we are looking at the correct byte in the buffer
            inputBuffer[4] = 15;
            result = _report.Parse(inputBuffer).ToJoystickState();
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.X);
        }
        #endregion

        #region DapConfig Tests
        [TestMethod]
        public void DapConfig_HasCorrectDefaultValues()
        {
            // Arrange
            var config = new DapConfig();
            // Assert
            Assert.IsNotNull(config);
            Assert.IsFalse(config.AutoBackLightEnabled);
            Assert.IsFalse(config.LightSensorEnabled);
            Assert.AreEqual(5, config.AutoStandByTimeout);
        }

        [TestMethod]
        public void DapConfig_ToData_ReturnsExpectedValue_ForDefaultValues()
        {
            // Arrange
            var config = new DapConfig();
            
            // Act
            var byteData = config.ToData;
            var expectedReportId = (byte)4;
            
            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(expectedReportId, DapConfig.ReportId);
            Assert.AreEqual(DapConfig.ReportId, byteData[0]);
            Assert.AreEqual(0, byteData[1]);
            Assert.AreEqual(0, byteData[2]);
            Assert.AreEqual(0, byteData[3]);
            Assert.AreEqual(5, byteData[4]);

            Assert.HasCount(5, byteData);
        }

        [TestMethod]
        public void DapConfig_ToData_ReturnsExpectedValue_ForAutoBackLightEnabled()
        {
            // Arrange
            var config = new DapConfig()
            {
                AutoBackLightEnabled = true
            };

            // Act
            var byteData = config.ToData;
            var expectedReportId = (byte)4;
            
            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(expectedReportId, DapConfig.ReportId);
            Assert.AreEqual(DapConfig.ReportId, byteData[0]);
            Assert.AreEqual(0b00000001, byteData[1]);
            Assert.AreEqual(0, byteData[2]);
            Assert.AreEqual(0, byteData[3]);
            Assert.AreEqual(5, byteData[4]);

            Assert.HasCount(5, byteData);
        }

        [TestMethod]
        public void DapConfig_ToData_ReturnsExpectedValue_ForLightSensorEnabled()
        {
            // Arrange
            var config = new DapConfig()
            {
                LightSensorEnabled = true
            };

            // Act
            var byteData = config.ToData;
            var expectedReportId = (byte)4;
            
            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(expectedReportId, DapConfig.ReportId);
            Assert.AreEqual(DapConfig.ReportId, byteData[0]);
            Assert.AreEqual(0b00000010, byteData[1]);
            Assert.AreEqual(0, byteData[2]);
            Assert.AreEqual(0, byteData[3]);
            Assert.AreEqual(5, byteData[4]);

            Assert.HasCount(5, byteData);
        }

        [TestMethod]
        public void DapConfig_ToData_ReturnsExpectedValue_ForAutoStandByTimeout()
        {
            // Arrange
            var config = new DapConfig()
            {
                AutoStandByTimeout = ushort.MaxValue
            };

            // Act
            var byteData = config.ToData;
            
            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(DapConfig.ReportId, byteData[0]);
            Assert.AreEqual(0, byteData[1]);
            Assert.AreEqual(0, byteData[2]);
            Assert.AreEqual(0xFF, byteData[3]);
            Assert.AreEqual(0xFF, byteData[4]);

            // Arange
            config.AutoStandByTimeout = 511;

            // Act
            byteData = config.ToData;
            
            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(DapConfig.ReportId, byteData[0]);
            Assert.AreEqual(0, byteData[1]);
            Assert.AreEqual(0, byteData[2]);
            Assert.AreEqual(0x1, byteData[3]);
            Assert.AreEqual(0xFF, byteData[4]);
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a valid FCU Cube input buffer for testing purposes
        /// </summary>
        private byte[] CreateValidInputBuffer()
        {
            var buffer = new byte[5] {
                0x01, // Report ID
                0x00,
                0x00,
                0x00,
                0x00
            };

            return buffer;
        }

        #endregion
    }
}