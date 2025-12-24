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
            Assert.AreEqual(BoardPartnerLevel.Core, coreBoard.PartnerLevel);

            var partnerBoard = BoardDefinitions.GetBoardByMobiFlightType("Kav Mega");
            Assert.AreEqual(BoardPartnerLevel.Partner, partnerBoard.PartnerLevel);

            var communityBoard = BoardDefinitions.GetBoardByMobiFlightType("MobiFlight GenericI2C Mega");
            Assert.AreEqual(BoardPartnerLevel.Community, communityBoard.PartnerLevel);
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