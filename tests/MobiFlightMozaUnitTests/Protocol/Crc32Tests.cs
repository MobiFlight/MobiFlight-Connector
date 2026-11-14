using System.Text;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class Crc32Tests
    {
        [TestMethod]
        public void Compute_StandardCheckVector_ReturnsExpectedValue()
        {
            // Arrange
            var data = Encoding.ASCII.GetBytes("123456789");

            // Act
            var crc = Crc32.Compute(data);

            // Assert
            Assert.AreEqual(0xCBF43926u, crc);
        }

        [TestMethod]
        public void Compute_EmptyData_ReturnsZero()
        {
            // Arrange
            byte[] data = [];

            // Act
            var crc = Crc32.Compute(data);

            // Assert
            Assert.AreEqual(0u, crc);
        }
    }
}
