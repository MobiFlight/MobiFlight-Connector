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
            BoardDefinitions.Load();

            Assert.IsFalse(BoardDefinitions.LoadingError);
        }

        [TestMethod()]
        public void CustomDeviceDefinitionFileTest()
        {
            CustomDeviceDefinitions.Load();

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