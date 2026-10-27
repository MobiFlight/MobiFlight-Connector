namespace MobiFlight.Joysticks.Moza.Tests
{
    [TestClass]
    public class MozaMxg1000Tests
    {
        private const string BacklightLabel = "Backlight";

        private static JoystickDefinition CreateDefinition()
        {
            return new JoystickDefinition
            {
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id = "backlight", Label = BacklightLabel, Byte = 0, Bit = 0 }
                }
            };
        }

        private static (MozaMxg1000 Device, MemoryStream Stream) CreateConnectedDevice()
        {
            var stream = new MemoryStream();
            var device = new MozaMxg1000(CreateDefinition(), () => new MozaCdcPort(stream));
            device.Connect(IntPtr.Zero);
            return (device, stream);
        }

        #region ToBrightnessLevel

        [TestMethod]
        [DataRow((byte)0, (byte)0)]
        [DataRow((byte)8, (byte)0)]
        [DataRow((byte)9, (byte)1)]
        [DataRow((byte)127, (byte)7)]
        [DataRow((byte)128, (byte)8)]
        [DataRow((byte)255, (byte)15)]
        public void ToBrightnessLevel_ShouldScaleToPanelRange_WhenStateIsGiven(byte state, byte expectedLevel)
        {
            // Arrange

            // Act
            var level = MozaMxg1000.ToBrightnessLevel(state);

            // Assert
            Assert.AreEqual(expectedLevel, level);
        }

        #endregion

        #region Backlight output

        [TestMethod]
        [DataRow((byte)255, "7E03451252010F47")]
        [DataRow((byte)128, "7E03451252010840")]
        public void UpdateOutputDeviceStates_ShouldSendBrightnessToPanel_WhenBacklightIsSet(byte state, string expectedFrame)
        {
            // Arrange
            var (device, stream) = CreateConnectedDevice();

            // Act
            device.SetOutputDeviceState(BacklightLabel, state);
            device.UpdateOutputDeviceStates();

            // Assert
            Assert.AreEqual(expectedFrame, Convert.ToHexString(stream.ToArray()));
        }

        [TestMethod]
        public void UpdateOutputDeviceStates_ShouldSendNothing_WhenBrightnessLevelIsUnchanged()
        {
            // Arrange
            var (device, stream) = CreateConnectedDevice();
            device.SetOutputDeviceState(BacklightLabel, 128);
            device.UpdateOutputDeviceStates();
            var lengthAfterFirstWrite = stream.Length;

            // Act
            device.SetOutputDeviceState(BacklightLabel, 129);
            device.UpdateOutputDeviceStates();

            // Assert
            Assert.AreEqual(lengthAfterFirstWrite, stream.Length);
        }

        [TestMethod]
        public void UpdateOutputDeviceStates_ShouldIgnoreBacklight_WhenCdcPortIsNotAvailable()
        {
            // Arrange
            var device = new MozaMxg1000(CreateDefinition(), () => null);
            device.Connect(IntPtr.Zero);

            // Act
            device.SetOutputDeviceState(BacklightLabel, 255);
            device.UpdateOutputDeviceStates();

            // Assert
        }

        #endregion

        #region Stop and Shutdown

        [TestMethod]
        public void Stop_ShouldTurnBacklightOff_WhenBacklightWasSet()
        {
            // Arrange
            var (device, stream) = CreateConnectedDevice();
            device.SetOutputDeviceState(BacklightLabel, 255);
            device.UpdateOutputDeviceStates();

            // Act
            device.Stop();

            // Assert
            Assert.AreEqual("7E03451252010F477E03451252010038", Convert.ToHexString(stream.ToArray()));
        }

        [TestMethod]
        public void Shutdown_ShouldReleaseCdcPort()
        {
            // Arrange
            var (device, stream) = CreateConnectedDevice();

            // Act
            device.Shutdown();

            // Assert
            Assert.IsFalse(stream.CanWrite);
        }

        #endregion
    }
}