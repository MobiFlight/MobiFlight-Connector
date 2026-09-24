using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Joysticks.Moza.Tests
{
    [TestClass]
    public class MozaSeatResolverTests
    {
        [TestMethod]
        public void ResolvePath_Zero_ReturnsCaptainPath()
        {
            Assert.AreEqual("/winwing/cdu-captain", MozaSeatResolver.ResolvePath(0));
        }

        [TestMethod]
        public void ResolvePath_One_ReturnsFirstOfficerPath()
        {
            Assert.AreEqual("/winwing/cdu-co-pilot", MozaSeatResolver.ResolvePath(1));
        }

        [TestMethod]
        public void ResolvePath_OutOfRange_FallsBackToCaptainPath()
        {
            Assert.AreEqual("/winwing/cdu-captain", MozaSeatResolver.ResolvePath(2));
        }
    }
}
