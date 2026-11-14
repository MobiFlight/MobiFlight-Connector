using MobiFlightMoza.Protocol;
using MobiFlightMoza.Tests;

namespace MobiFlightMoza.Protocol.Tests
{
    [TestClass]
    public class NetworkPackageTests
    {
        [TestMethod]
        public void Pack_RoundTripsThroughFeed()
        {
            // Arrange
            byte[] payload = [1, 2, 3, 4, 5];
            byte[] wire = NetworkPackage.Pack(0x30, payload);
            var extractor = new NetworkPackageExtractor();

            // Act
            var packages = extractor.Feed(wire);

            // Assert
            Assert.HasCount(1, packages);
            Assert.AreEqual((byte)0x30, packages[0].PackageId);
            CollectionAssert.AreEqual(payload, packages[0].Payload);
        }

        [TestMethod]
        public void Feed_SplitAcrossTwoReads_YieldsOneCompletePackage()
        {
            // Arrange
            byte[] wire = NetworkPackage.Pack(0x33, [9, 8, 7]);
            var extractor = new NetworkPackageExtractor();

            // Act
            var firstChunk = extractor.Feed(wire[..3]);
            var secondChunk = extractor.Feed(wire[3..]);

            // Assert
            Assert.IsEmpty(firstChunk);
            Assert.HasCount(1, secondChunk);
        }

        [TestMethod]
        public void Feed_TwoPackagesInOneRead_YieldsBoth()
        {
            // Arrange
            byte[] wire = [.. NetworkPackage.Pack(0x30, [1]), .. NetworkPackage.Pack(0x31, [2])];
            var extractor = new NetworkPackageExtractor();

            // Act
            var packages = extractor.Feed(wire);

            // Assert
            Assert.HasCount(2, packages);
            Assert.AreEqual((byte)0x30, packages[0].PackageId);
            Assert.AreEqual((byte)0x31, packages[1].PackageId);
        }

        [TestMethod]
        public void Pack_EmptyPayload_ProducesFiveByteHeader()
        {
            // Act
            byte[] wire = NetworkPackage.Pack(0x03, []);

            // Assert
            CollectionAssert.AreEqual(TestHex.Parse("03 00 00 00 00"), wire);
        }
    }
}
