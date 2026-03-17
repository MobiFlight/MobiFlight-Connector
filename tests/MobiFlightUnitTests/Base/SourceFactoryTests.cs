using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class SourceFactoryTests
    {
        [TestMethod()]
        public void CreateTest()
        {
            var source = SourceFactory.Create("unknown_sim");
            Assert.IsNull(source);
            
            source = SourceFactory.Create("msfs");
            Assert.IsNotNull(source);
            Assert.IsInstanceOfType(source, typeof(SimConnectSource));

            source = SourceFactory.Create("xplane");
            Assert.IsNotNull(source);
            Assert.IsInstanceOfType(source, typeof(XplaneSource));

            source = SourceFactory.Create("prosim");
            Assert.IsNotNull(source);
            Assert.IsInstanceOfType(source, typeof(ProSimSource));

            source = SourceFactory.Create("fsx");
            Assert.IsNotNull(source);
            Assert.IsInstanceOfType(source, typeof(FsuipcSource));

            source = SourceFactory.Create("p3d");
            Assert.IsNotNull(source);
            Assert.IsInstanceOfType(source, typeof(FsuipcSource));
        }
    }
}