using Moq;

namespace MobiFlight.Joysticks.Moza.Tests
{
    [TestClass]
    public class MozaMxg1000Tests
    {
        private const string BacklightLabel = "Backlight";

        private static JoystickDefinition CreateDefinition(string label = BacklightLabel)
        {
            return new JoystickDefinition
            {
                Outputs = new List<JoystickOutput>
                {
                    new JoystickOutput { Id = "backlight", Label = label, Byte = 0, Bit = 0 }
                }
            };
        }

        private static (MozaMxg1000 Device, MemoryStream Stream) CreateConnectedDevice()
        {
            var stream = new MemoryStream();
            var device = new MozaMxg1000(CreateDefinition(), () => stream);
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
        public void UpdateOutputDeviceStates_ShouldWriteCapturedFrame_WhenBacklightChanges(byte state, string expectedFrame)
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
        public void UpdateOutputDeviceStates_ShouldWriteFrame_WhenLabelDiffersFromBacklightId()
        {
            // Arrange
            var stream = new MemoryStream();
            var device = new MozaMxg1000(CreateDefinition("Dimmer"), () => stream);
            device.Connect(IntPtr.Zero);

            // Act
            device.SetOutputDeviceState("Dimmer", 255);
            device.UpdateOutputDeviceStates();

            // Assert
            Assert.AreEqual("7E03451252010F47", Convert.ToHexString(stream.ToArray()));
        }

        [TestMethod]
        public void UpdateOutputDeviceStates_ShouldWriteMinimumFrame_WhenBacklightIsTurnedDown()
        {
            // Arrange
            var (device, stream) = CreateConnectedDevice();
            device.SetOutputDeviceState(BacklightLabel, 255);
            device.UpdateOutputDeviceStates();

            // Act
            device.SetOutputDeviceState(BacklightLabel, 0);
            device.UpdateOutputDeviceStates();

            // Assert
            Assert.AreEqual("7E03451252010F477E0345125201" + "0038", Convert.ToHexString(stream.ToArray()));
        }

        [TestMethod]
        public void UpdateOutputDeviceStates_ShouldNotWriteAgain_WhenStateMapsToSameLevel()
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
        public void UpdateOutputDeviceStates_ShouldWriteNothing_WhenNoOutputChanged()
        {
            // Arrange
            var (device, stream) = CreateConnectedDevice();

            // Act
            device.UpdateOutputDeviceStates();

            // Assert
            Assert.AreEqual(0, stream.Length);
        }

        #endregion

        #region Port availability

        [TestMethod]
        public void UpdateOutputDeviceStates_ShouldWriteOnceAvailable_WhenPortWasMissingAtFirst()
        {
            // Arrange
            var stream = new MemoryStream();
            var attempts = 0;
            var device = new MozaMxg1000(CreateDefinition(), () => ++attempts == 1 ? null : stream, TimeSpan.Zero);
            device.Connect(IntPtr.Zero);
            device.SetOutputDeviceState(BacklightLabel, 255);

            // Act
            device.UpdateOutputDeviceStates();
            var lengthWhilePortMissing = stream.Length;
            device.UpdateOutputDeviceStates();

            // Assert
            Assert.AreEqual(0, lengthWhilePortMissing);
            Assert.AreEqual("7E03451252010F47", Convert.ToHexString(stream.ToArray()));
        }

        [TestMethod]
        public void UpdateOutputDeviceStates_ShouldRetryOnNewStream_WhenWriteFails()
        {
            // Arrange
            var failingStream = new Mock<Stream>();
            failingStream
                .Setup(s => s.Write(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws<IOException>();
            var workingStream = new MemoryStream();
            var attempts = 0;
            var device = new MozaMxg1000(CreateDefinition(), () => ++attempts == 1 ? failingStream.Object : workingStream, TimeSpan.Zero);
            device.Connect(IntPtr.Zero);
            device.SetOutputDeviceState(BacklightLabel, 255);

            // Act
            device.UpdateOutputDeviceStates();
            device.UpdateOutputDeviceStates();

            // Assert
            Assert.AreEqual("7E03451252010F47", Convert.ToHexString(workingStream.ToArray()));
        }

        #endregion

        #region Stop and Shutdown

        [TestMethod]
        public void Stop_ShouldNotOpenPort_WhenBacklightWasNeverSet()
        {
            // Arrange
            var opened = 0;
            var device = new MozaMxg1000(CreateDefinition(), () => { opened++; return new MemoryStream(); });
            device.Connect(IntPtr.Zero);

            // Act
            device.Stop();

            // Assert
            Assert.AreEqual(0, opened);
        }

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
            Assert.AreEqual("7E03451252010F477E0345125201" + "0038", Convert.ToHexString(stream.ToArray()));
        }

        [TestMethod]
        public void Shutdown_ShouldCloseStream_WhenPortIsOpen()
        {
            // Arrange
            var (device, stream) = CreateConnectedDevice();
            device.SetOutputDeviceState(BacklightLabel, 255);
            device.UpdateOutputDeviceStates();

            // Act
            device.Shutdown();

            // Assert
            Assert.IsFalse(stream.CanWrite);
        }

        #endregion
    }
}