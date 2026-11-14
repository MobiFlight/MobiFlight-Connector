using MobiFlightMoza.Cdu;

namespace MobiFlightMoza.Cdu.Tests
{
    [TestClass]
    public class McduFrameBuilderTests
    {
        [TestMethod]
        public void BuildNext_FirstCall_ProducesKeyframe()
        {
            // Arrange
            var builder = new McduFrameBuilder();
            var page = CduPage.CreateBlank();

            // Act
            byte[] frame = builder.BuildNext(page);

            // Assert
            Assert.AreEqual((byte)0x30, frame[0]); // Keyframe PackageId
        }

        [TestMethod]
        public void BuildNext_UnchangedPage_ReturnsNull()
        {
            // Arrange
            var builder = new McduFrameBuilder();
            var page = CduPage.CreateBlank();
            builder.BuildNext(page);

            // Act
            byte[] frame = builder.BuildNext(CduPage.CreateBlank());

            // Assert
            Assert.IsNull(frame);
        }

        [TestMethod]
        public void BuildNext_OneRowChanged_ProducesDelta()
        {
            // Arrange
            var builder = new McduFrameBuilder();
            builder.BuildNext(CduPage.CreateBlank());
            var changed = CduPage.CreateBlank();
            changed[3, 0] = new CduCell('A', 'w', false, false);

            // Act
            byte[] frame = builder.BuildNext(changed);

            // Assert
            Assert.AreEqual((byte)0x31, frame[0]); // Delta PackageId
        }

        [TestMethod]
        public void BuildNext_SequenceStrictlyIncreasesAcrossCalls()
        {
            // Arrange
            var builder = new McduFrameBuilder();
            byte[] first = builder.BuildNext(CduPage.CreateBlank());
            var changed = CduPage.CreateBlank();
            changed[0, 0] = new CduCell('A', 'w', false, false);

            // Act
            byte[] second = builder.BuildNext(changed);

            // Assert
            uint firstSequence = SequenceOf(first);
            uint secondSequence = SequenceOf(second);
            Assert.IsGreaterThan(firstSequence, secondSequence);
        }

        [TestMethod]
        public void Reset_RestartsSequenceAtOneAndForcesAFreshKeyframe()
        {
            // Arrange
            var builder = new McduFrameBuilder();
            builder.BuildNext(CduPage.CreateBlank());

            // Act
            builder.Reset();
            byte[] frame = builder.BuildNext(CduPage.CreateBlank());

            // Assert
            Assert.AreEqual((byte)0x30, frame[0]); // Keyframe again, not "unchanged"
            Assert.AreEqual(1u, SequenceOf(frame));
        }

        // Sequence sits right after PackageId(1)+Size(4)+Version(1) in the patch payload.
        private static uint SequenceOf(byte[] frame)
            => (uint)(frame[6] | (frame[7] << 8) | (frame[8] << 16) | (frame[9] << 24));
    }
}
