using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.CustomDevices;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class JsonDefinitionFileTests
    {
        [TestInitialize]
        public void Setup() {
            // enable schema validation to not exceed 1,000 limit per hour
            // https://www.newtonsoft.com/jsonschema
            JsonBackedObject.SkipSchemaValidation = false;
        }

        [TestCleanup]
        public void Cleanup()
        {
            // enable schema validation
            // https://www.newtonsoft.com/jsonschema
            JsonBackedObject.SkipSchemaValidation = true;
        }

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