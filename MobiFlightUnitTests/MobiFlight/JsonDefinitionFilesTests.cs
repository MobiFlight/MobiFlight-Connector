using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.CustomDevices;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class JsonDefinitionFileTests
    {
        [TestMethod()]
        public void BoardDefinitionFileTest()
        {
            BoardDefinitions.LoadDefinitions();

            Assert.IsFalse(BoardDefinitions.LoadingError);

            var coreBoard = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight Mega");
            Assert.IsTrue(coreBoard.PartnerLevel == BoardPartnerLevel.Core);

            var partnerBoard = BoardDefinitions.GetBoardByMobiFlightType("Kav Mega");
            Assert.IsTrue(partnerBoard.PartnerLevel == BoardPartnerLevel.Partner);

            var communityBoard = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight GenericI2C Mega");
            Assert.IsTrue(communityBoard.PartnerLevel == BoardPartnerLevel.Community);
        }

        [TestMethod()]
        public void CustomDeviceDefinitionFileTest()
        {
            CustomDeviceDefinitions.LoadDefinitions();

            Assert.IsFalse(CustomDeviceDefinitions.LoadingError);
        }

        [TestMethod()]
        public void JoystickDefinitionFileTest()
        {
            var manager = new JoystickManager();

            Assert.IsFalse(manager.LoadingError);
        }

        [TestMethod()]
        public void MidiDefinitionFileTest()
        {
            var manager = new MidiBoardManager();

            Assert.IsFalse(manager.LoadingError);
        }
    }
}