namespace MobiFlight.Transport.Tests
{
    [TestClass]
    public class SerialTransportTests
    {
        private const string MissingPort = "COM987";

        private static SerialTransport CreateTransport(string portName)
        {
            return new SerialTransport
            {
                CurrentSerialSettings = { PortName = portName, BaudRate = 115200 }
            };
        }

        [TestMethod]
        public void Connect_ShouldReturnFalse_WhenPortDoesNotExist()
        {
            // Arrange
            var transport = CreateTransport(MissingPort);

            // Act
            var connected = transport.Connect();

            // Assert
            Assert.IsFalse(connected);
        }

        [TestMethod]
        public void Connect_ShouldSetPortNotFoundStatus_WhenPortDoesNotExist()
        {
            // Arrange
            var transport = CreateTransport(MissingPort);

            // Act
            transport.Connect();

            // Assert
            Assert.AreEqual(Status.PortNotFound, transport.Status);
        }

        [TestMethod]
        public void Connect_ShouldRaiseConnectionFailedOnce_WhenPortDoesNotExist()
        {
            // Arrange
            var transport = CreateTransport(MissingPort);
            var failures = new List<string>();
            transport.ConnectionFailed += (sender, args) => failures.Add(args.Item1);

            // Act
            transport.Connect();

            // Assert
            Assert.HasCount(1, failures);
            StringAssert.Contains(failures[0], MissingPort);
        }
    }
}