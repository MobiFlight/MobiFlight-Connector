using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Tests
{
    [TestClass()]
    public class JoystickTests
    {
        class TestableJoystick : Joystick
        {
            public bool HasPovButtons { get; set; } = false;
            public TestableJoystick(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
            {
            }

            protected override void EnumerateDevices()
            {
                if (HasPovButtons)
                {
                    AddPovSwitchWithName("POV Switch");
                }

                if (Definition?.Inputs == null) return;

                Definition.Inputs.ForEach(d =>
                {
                    if (d.Type == JoystickDeviceType.Axis)
                    {
                        var offsetAxisName = GetAxisNameForUsage(d.Id);
                        var axisName = $"{AxisPrefix} {offsetAxisName}";

                        Axes.Add(new JoystickDevice()
                        {
                            JoystickDeviceType = JoystickDeviceType.Axis,
                            Name = axisName,
                            Label = d.Label,
                            Type = DeviceType.AnalogInput
                        });
                        return;
                    }

                    if (d.Type == JoystickDeviceType.Button)
                    {
                        var buttonName = $"{ButtonPrefix} {d.Id}";
                        Buttons.Add(new JoystickDevice()
                        {
                            JoystickDeviceType = JoystickDeviceType.Button,
                            Name = buttonName,
                            Label = d.Label,
                            Type = DeviceType.Button
                        });
                    }
                });
            }

            protected override void EnumerateOutputDevices()
            {
                if (Definition?.Outputs == null) return;

                Definition.Outputs?.ForEach(d =>
                {
                    if (d.Type == DeviceType.LcdDisplay.ToString())
                    {

                        Lights.Add(new JoystickOutputDisplay()
                        {
                            Name = d.Id,
                            Label = d.Label,
                            Type = DeviceType.LcdDisplay,
                            Cols = d.Cols,
                            Lines = d.Lines
                        });
                        return;
                    }

                    Lights.Add(new JoystickOutputDevice()
                    {
                        Name = d.Id,
                        Label = d.Label,
                        Type = DeviceType.Output,
                        Byte = d.Byte,
                        Bit = d.Bit
                    });
                });
            }
        }

        class RecordingJoystick : TestableJoystick
        {
            public int OnStopCount { get; private set; }
            public int OnSetOutputDeviceStateCount { get; private set; }
            public int OnSetLcdDisplayCount { get; private set; }

            public RecordingJoystick(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
            {
            }

            protected override void OnStop()
            {
                OnStopCount++;
            }

            protected override void OnSetOutputDeviceState(string name, byte state)
            {
                OnSetOutputDeviceStateCount++;
            }

            protected override void OnSetLcdDisplay(string address, string value)
            {
                OnSetLcdDisplayCount++;
            }
        }

        [TestMethod()]
        public void GetAxisNameForUsage_ShouldReturnNamesForValidIds()
        {
            // Arrange - Valid axis usage IDs that map to JoystickState properties
            var validUsageIds = new List<int>
            {
                48, // X
                49, // Y
                50, // Z
                51, // RotationX
                52, // RotationY
                53, // RotationZ
                54, // Slider1
                55  // Slider2
            };

            // Act & Assert - Valid IDs should return axis names
            validUsageIds.ForEach(id =>
            {
                var axisName = Joystick.GetAxisNameForUsage(id);
                Assert.IsFalse(string.IsNullOrEmpty(axisName),
                    $"UsageMap should contain valid usage ID {id} and return a non-empty axis name.");
            });
        }

        [TestMethod()]
        public void GetAxisNameForUsage_ShouldThrowExceptionForInvalidIds()
        {
            // Arrange - Invalid usage IDs that don't map to any JoystickState property
            var invalidUsageIds = new List<int>
            {
                46, // Below valid range
                47, // Below valid range
                56, // Wheel - not supported by JoystickState
                57  // HatSwitch - not supported by JoystickState (should be POV)
            };

            invalidUsageIds.ForEach(id =>
            {
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Joystick.GetAxisNameForUsage(id));
            });
        }

        [TestMethod()]
        public void GetAxisNameForUsage_ValidIds_ReturnsExpectedNames()
        {
            // Verify exact mappings
            Assert.AreEqual("X", Joystick.GetAxisNameForUsage(48));
            Assert.AreEqual("Y", Joystick.GetAxisNameForUsage(49));
            Assert.AreEqual("Z", Joystick.GetAxisNameForUsage(50));
            Assert.AreEqual("RotationX", Joystick.GetAxisNameForUsage(51));
            Assert.AreEqual("RotationY", Joystick.GetAxisNameForUsage(52));
            Assert.AreEqual("RotationZ", Joystick.GetAxisNameForUsage(53));
            Assert.AreEqual("Slider1", Joystick.GetAxisNameForUsage(54));
            Assert.AreEqual("Slider2", Joystick.GetAxisNameForUsage(55));
        }

        [TestMethod()]
        public void GetAvailableLcdDevices_ReturnsValidDevice()
        {
            // Arrange
            SharpDX.DirectInput.Joystick dxJoystick = null;
            var definition = new JoystickDefinition
            {
                InstanceName = "Test Joystick",
                ProductId = 0x1234,
                VendorId = 0x5678,
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id="1", Label = "LED01", Type = "Output", Byte = 2, Bit = 0 },
                    new JoystickOutput { Id="2", Label = "LCD01", Type = "LcdDisplay", Cols = 16, Lines = 1 }
                }
            };
            var joystick = new TestableJoystick(dxJoystick, definition);
            // Connect is required so that devices will be enumerated
            joystick.Connect(IntPtr.Zero);

            // Act
            var devices = joystick.GetAvailableLcdDevices();
            // Assert
            Assert.IsNotNull(devices, "GetAvailableLcdDevices should not return null.");
            Assert.HasCount(1, devices, "There should be at least one available LCD device.");
            Assert.IsInstanceOfType(devices[0], typeof(Firmware.LcdDisplay), "The available device should be of type LcdDisplay.");
        }

        [TestMethod()]
        public void GetAvailableOutputDevices_ReturnsValidDevices()
        {
            // Arrange
            SharpDX.DirectInput.Joystick dxJoystick = null;
            var definition = new JoystickDefinition
            {
                InstanceName = "Test Joystick",
                ProductId = 0x1234,
                VendorId = 0x5678,
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id="1", Label = "LED01", Type = "Output", Byte = 2, Bit = 0 },
                    new JoystickOutput { Id="2", Label = "LCD01", Type = "LcdDisplay", Cols = 16, Lines = 1 }
                }
            };
            var joystick = new TestableJoystick(dxJoystick, definition);
            // Connect is required so that devices will be enumerated
            joystick.Connect(IntPtr.Zero);

            // Act
            var devices = joystick.GetAvailableOutputDevices();
            // Assert
            Assert.IsNotNull(devices, "GetAvailableOutputDevices should not return null.");
            Assert.HasCount(2, devices, "There should be at least one available output device.");

            Assert.IsInstanceOfType(devices[0], typeof(JoystickOutputDevice), "The available device should be of type JoystickOutputDevice.");
            Assert.IsInstanceOfType(devices[1], typeof(JoystickOutputDisplay), "The available device should be of type JoystickOutputDisplay.");
        }

        #region Stop Tests

        private static RecordingJoystick CreateRecordingJoystick()
        {
            return new RecordingJoystick(null, new JoystickDefinition { InstanceName = "Test Joystick" });
        }

        [TestMethod()]
        public void Stop_ShouldStopDevice_WhenDeviceIsFresh()
        {
            // Arrange
            var joystick = CreateRecordingJoystick();

            // Act
            joystick.Stop();

            // Assert
            Assert.AreEqual(1, joystick.OnStopCount, "A fresh device may show stale data, so the first Stop should run.");
        }

        [TestMethod()]
        public void Stop_ShouldNotStopAgain_WhenNothingWasWrittenSinceLastStop()
        {
            // Arrange
            var joystick = CreateRecordingJoystick();
            joystick.Stop();

            // Act
            joystick.Stop();
            joystick.Stop();

            // Assert
            Assert.AreEqual(1, joystick.OnStopCount, "Repeated Stop calls should not reach the device again.");
        }

        [TestMethod()]
        public void Stop_ShouldStopAgain_WhenOutputStateWasSetSinceLastStop()
        {
            // Arrange
            var joystick = CreateRecordingJoystick();
            joystick.Stop();
            joystick.SetOutputDeviceState("LED01", 1);

            // Act
            joystick.Stop();

            // Assert
            Assert.AreEqual(2, joystick.OnStopCount, "Writing an output state should make the next Stop run.");
        }

        [TestMethod()]
        public void Stop_ShouldStopAgain_WhenLcdDisplayWasSetSinceLastStop()
        {
            // Arrange
            var joystick = CreateRecordingJoystick();
            joystick.Stop();
            joystick.SetLcdDisplay("LCD01", "1234");

            // Act
            joystick.Stop();

            // Assert
            Assert.AreEqual(2, joystick.OnStopCount, "Writing an LCD value should make the next Stop run.");
        }

        #endregion

        #region Output Hook Tests

        [TestMethod()]
        public void SetOutputDeviceState_ShouldInvokeHook()
        {
            // Arrange
            var joystick = CreateRecordingJoystick();

            // Act
            joystick.SetOutputDeviceState("LED01", 1);

            // Assert
            Assert.AreEqual(1, joystick.OnSetOutputDeviceStateCount);
        }

        [TestMethod()]
        public void SetLcdDisplay_ShouldInvokeHook()
        {
            // Arrange
            var joystick = CreateRecordingJoystick();

            // Act
            joystick.SetLcdDisplay("LCD01", "1234");

            // Assert
            Assert.AreEqual(1, joystick.OnSetLcdDisplayCount);
        }

        #endregion
    }
}