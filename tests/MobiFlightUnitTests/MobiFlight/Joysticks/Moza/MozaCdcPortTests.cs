namespace MobiFlight.Joysticks.Moza.Tests
{
    [TestClass]
    public class MozaCdcPortTests
    {
        private static readonly byte[] Frame = [0x7E, 0x03, 0x45];

        [TestMethod]
        public void Write_ShouldWriteFrame_WhenCalled()
        {
            // Arrange
            var stream = new MemoryStream();
            var port = new MozaCdcPort(stream);

            // Act
            port.Write(Frame);

            // Assert
            CollectionAssert.AreEqual(Frame, stream.ToArray());
        }

        [TestMethod]
        public void Write_ShouldNotWriteAgain_WhenFrameIsIdenticalToLast()
        {
            // Arrange
            var stream = new MemoryStream();
            var port = new MozaCdcPort(stream);
            port.Write(Frame);

            // Act
            port.Write([.. Frame]);

            // Assert
            Assert.AreEqual(Frame.Length, stream.Length);
        }

        [TestMethod]
        public void Write_ShouldDoNothing_WhenDisposed()
        {
            // Arrange
            var stream = new MemoryStream();
            var port = new MozaCdcPort(stream);
            port.Dispose();

            // Act
            port.Write(Frame);

            // Assert
            Assert.IsEmpty(stream.ToArray());
        }

        [TestMethod]
        public void Write_ShouldNotThrow_WhenDisposedConcurrently()
        {
            // Arrange
            var port = new MozaCdcPort(new MemoryStream());

            // Act
            var writer = Task.Run(() =>
            {
                for (var i = 0; i < 2000; i++) port.Write([(byte)i]);
            });
            port.Dispose();

            // Assert
            writer.GetAwaiter().GetResult();
        }
    }
}