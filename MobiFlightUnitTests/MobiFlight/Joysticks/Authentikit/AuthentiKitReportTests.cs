using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.AuthentiKit.Tests
{
    [TestClass]
    public class AuthentiKitReportTests
    {
        private AuthentiKitReport _report;

        [TestInitialize]
        public void SetUp()
        {
            _report = new AuthentiKitReport();
        }

        #region Parse Tests

        [TestMethod]
        public void Parse_ValidInputBuffer_ReturnsNewReportInstance()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Act
            var result = _report.Parse(inputBuffer);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotSame(_report, result);
            Assert.IsInstanceOfType(result, typeof(AuthentiKitReport));
        }

        [TestMethod]
        public void Parse_NullInputBuffer_ThrowsArgumentException()
        {
            // Arrange
            byte[] inputBuffer = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _report.Parse(inputBuffer));
        }

        [TestMethod]
        public void Parse_EmptyInputBuffer_ThrowsArgumentException()
        {
            // Arrange
            var inputBuffer = new byte[0];

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _report.Parse(inputBuffer));
        }

        [TestMethod]
        public void Parse_InsufficientLengthInputBuffer_ThrowsArgumentException()
        {
            // Arrange
            var inputBuffer = new byte[4]; // Expected 5 bytes minimum

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _report.Parse(inputBuffer));
        }

        #endregion

        #region ToJoystickState - Single Axis Tests

        [TestMethod]
        public void ToJoystickState_SingleAxisX_ParsesCorrectly()
        {
            // Arrange
            // Input: 0xFF 0x0F = 0x0FFF, shifted left by 4 = 0xFFF0 (65520)
            var inputBuffer = new byte[5] { 0xFF, 0x0F, 0x00, 0x00, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0xFFF0, state.X, "Axis X should be 0xFFF0 (65520)");
        }

        [TestMethod]
        public void ToJoystickState_SingleAxisY_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x80, 0x07, 0x00, 0x00, 0x00 }; // 0x0780 << 4 = 0x7800
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis Y", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x7800, state.Y);
        }

        [TestMethod]
        public void ToJoystickState_AxisZ_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x00, 0x08, 0x00, 0x00, 0x00 }; // 0x0800 << 4 = 0x8000
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis Z", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x8000, state.Z);
        }

        [TestMethod]
        public void ToJoystickState_RotationX_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0xAA, 0x05, 0x00, 0x00, 0x00 }; // 0x05AA << 4 = 0x5AA0
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis RotationX", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x5AA0, state.RotationX);
        }

        [TestMethod]
        public void ToJoystickState_RotationY_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x11, 0x02, 0x00, 0x00, 0x00 }; // 0x0211 << 4 = 0x2110
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis RotationY", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x2110, state.RotationY);
        }

        [TestMethod]
        public void ToJoystickState_RotationZ_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x33, 0x0C, 0x00, 0x00, 0x00 }; // 0x0C33 << 4 = 0xC330
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis RotationZ", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0xC330, state.RotationZ);
        }

        [TestMethod]
        public void ToJoystickState_Slider1_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x44, 0x04, 0x00, 0x00, 0x00 }; // 0x0444 << 4 = 0x4440
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis Slider1", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x4440, state.Sliders[0]);
        }

        [TestMethod]
        public void ToJoystickState_Slider1_AlternateName_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x55, 0x05, 0x00, 0x00, 0x00 }; // 0x0555 << 4 = 0x5550
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Slider1", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x5550, state.Sliders[0]);
        }

        [TestMethod]
        public void ToJoystickState_Slider2_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x66, 0x06, 0x00, 0x00, 0x00 }; // 0x0666 << 4 = 0x6660
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis Slider2", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x6660, state.Sliders[1]);
        }

        #endregion

        #region ToJoystickState - Multiple Axes Tests

        [TestMethod]
        public void ToJoystickState_TwoAxes_ParsesCorrectly()
        {
            // Arrange
            // Axis X: bytes 0-1 = 0x00 0x08 -> 0x8000
            // Axis Y: bytes 2-3 = 0xFF 0x0F -> 0xFFF0
            var inputBuffer = new byte[5] { 0x00, 0x08, 0xFF, 0x0F, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput },
                new JoystickDevice { Name = "Axis Y", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0x8000, state.X);
            Assert.AreEqual(0xFFF0, state.Y);
        }

        [TestMethod]
        public void ToJoystickState_ExceedsBufferBoundary_StopsGracefully()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x11, 0x01, 0x22, 0x02, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput },
                new JoystickDevice { Name = "Axis Y", Type = DeviceType.AnalogInput },
                new JoystickDevice { Name = "Axis Z", Type = DeviceType.AnalogInput } // Would need bytes 4-5
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert - Should not throw, Z should remain at default
            Assert.AreEqual(0x1110, state.X);
            Assert.AreEqual(0x2220, state.Y);
            Assert.AreEqual(0, state.Z, "Axis Z should remain at default when buffer is insufficient");
        }

        #endregion

        #region ToJoystickState - Button Tests

        [TestMethod]
        public void ToJoystickState_NoAxes_ButtonsAtByte0()
        {
            // Arrange
            // Button byte starts at offset 0 (no axes * 2)
            // Byte 0: 0b00000001 = Button 0 pressed
            var inputBuffer = new byte[5] { 0x01, 0x00, 0x00, 0x00, 0x00 };
            var axes = new List<JoystickDevice>();

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 12);

            // Assert
            Assert.IsTrue(state.Buttons[0], "Button 0 should be pressed");
            Assert.IsFalse(state.Buttons[1], "Button 1 should not be pressed");
        }

        [TestMethod]
        public void ToJoystickState_OneAxis_ButtonsAtByte2()
        {
            // Arrange
            // 1 axis = 2 bytes, so buttons start at byte index 2
            // Byte 2: 0b00000101 = Buttons 0 and 2 pressed
            var inputBuffer = new byte[5] { 0x00, 0x00, 0x05, 0x00, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 12);

            // Assert
            Assert.IsTrue(state.Buttons[0], "Button 0 should be pressed");
            Assert.IsFalse(state.Buttons[1], "Button 1 should not be pressed");
            Assert.IsTrue(state.Buttons[2], "Button 2 should be pressed");
        }

        [TestMethod]
        public void ToJoystickState_MultipleButtons_ParsesCorrectly()
        {
            // Arrange
            // 1 axis = 2 bytes, buttons start at byte 2
            // Byte 2: 0xFF = buttons 0-7 all pressed
            // Byte 3: 0x0F = buttons 8-11 pressed
            var inputBuffer = new byte[5] { 0x00, 0x00, 0xFF, 0x0F, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 12);

            // Assert
            for (int i = 0; i < 12; i++)
            {
                Assert.IsTrue(state.Buttons[i], $"Button {i} should be pressed");
            }
        }

        [TestMethod]
        public void ToJoystickState_Button7And8_ParsesCorrectly()
        {
            // Arrange
            // 1 axis = 2 bytes, buttons start at byte 2
            // Byte 2: 0x80 = button 7 pressed (bit 7)
            // Byte 3: 0x01 = button 8 pressed (bit 0 of next byte)
            var inputBuffer = new byte[5] { 0x00, 0x00, 0x80, 0x01, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 12);

            // Assert
            Assert.IsFalse(state.Buttons[6], "Button 6 should not be pressed");
            Assert.IsTrue(state.Buttons[7], "Button 7 should be pressed");
            Assert.IsTrue(state.Buttons[8], "Button 8 should be pressed");
            Assert.IsFalse(state.Buttons[9], "Button 9 should not be pressed");
        }

        [TestMethod]
        public void ToJoystickState_NoButtons_AllButtonsUnpressed()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x00, 0x00, 0x00, 0x00, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 12);

            // Assert
            for (int i = 0; i < 12; i++)
            {
                Assert.IsFalse(state.Buttons[i], $"Button {i} should not be pressed");
            }
        }

        #endregion

        #region ToJoystickState - Combined Tests

        [TestMethod]
        public void ToJoystickState_AxisAndButtons_ParsesCorrectly()
        {
            // Arrange
            // Axis X: bytes 0-1 = 0xFF 0x0F -> 0xFFF0 (65520)
            // Buttons: byte 2 = 0b00000011 -> buttons 0 and 1 pressed
            var inputBuffer = new byte[5] { 0xFF, 0x0F, 0x03, 0x00, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 12);

            // Assert
            Assert.AreEqual(0xFFF0, state.X, "Axis X value incorrect");
            Assert.IsTrue(state.Buttons[0], "Button 0 should be pressed");
            Assert.IsTrue(state.Buttons[1], "Button 1 should be pressed");
            Assert.IsFalse(state.Buttons[2], "Button 2 should not be pressed");
        }

        [TestMethod]
        public void ToJoystickState_RealWorldScenario_ParsesCorrectly()
        {
            // Arrange - Simulating real device with 2 axes and buttons
            // Axis X: 0x80 0x07 = 0x0780 << 4 = 0x7800 (center position)
            // Axis Y: 0x80 0x07 = 0x0780 << 4 = 0x7800 (center position)
            // Buttons: byte 4 = 0b00010000 = button 4 pressed
            var inputBuffer = new byte[5] { 0x80, 0x07, 0x80, 0x07, 0x10 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput },
                new JoystickDevice { Name = "Axis Y", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 12);

            // Assert
            Assert.AreEqual(0x7800, state.X);
            Assert.AreEqual(0x7800, state.Y);
            Assert.IsFalse(state.Buttons[3]);
            Assert.IsTrue(state.Buttons[4]);
            Assert.IsFalse(state.Buttons[5]);
        }

        #endregion

        #region Edge Cases

        [TestMethod]
        public void ToJoystickState_ZeroAxisValue_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0x00, 0x00, 0x00, 0x00, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0, state.X);
        }

        [TestMethod]
        public void ToJoystickState_MaxAxisValue_ParsesCorrectly()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0xFF, 0xFF, 0x00, 0x00, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Axis X", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert
            Assert.AreEqual(0xFFFF0, state.X); // Max 12-bit value (0xFFF) << 4
        }

        [TestMethod]
        public void ToJoystickState_UnknownAxisName_Ignored()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0xFF, 0x0F, 0x00, 0x00, 0x00 };
            var axes = new List<JoystickDevice>
            {
                new JoystickDevice { Name = "Unknown Axis", Type = DeviceType.AnalogInput }
            };

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes);

            // Assert - All axes should remain at default (0)
            Assert.AreEqual(0, state.X);
            Assert.AreEqual(0, state.Y);
            Assert.AreEqual(0, state.Z);
        }

        [TestMethod]
        public void ToJoystickState_EmptyAxisList_OnlyParsesButtons()
        {
            // Arrange
            var inputBuffer = new byte[5] { 0xFF, 0x00, 0x00, 0x00, 0x00 };
            var axes = new List<JoystickDevice>();

            // Act
            var report = _report.Parse(inputBuffer);
            var state = report.ToJoystickState(axes, ButtonCount: 8);

            // Assert - First byte contains buttons
            for (int i = 0; i < 8; i++)
            {
                Assert.IsTrue(state.Buttons[i], $"Button {i} should be pressed");
            }
        }

        #endregion
    }
}