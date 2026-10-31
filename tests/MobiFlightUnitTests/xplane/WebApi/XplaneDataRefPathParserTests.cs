using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.xplane.WebApi;

namespace MobiFlight.xplane.WebApi.Tests
{
    [TestClass]
    public class XplaneDataRefPathParserTests
    {
        [TestMethod]
        public void Parse_NoIndex_ReturnsFullNameAndNullIndex()
        {
            var (name, index) = XplaneDataRefPathParser.Parse("sim/foo/bar");

            Assert.AreEqual("sim/foo/bar", name);
            Assert.IsNull(index);
        }

        [TestMethod]
        public void Parse_WithIndex_SplitsNameAndIndex()
        {
            var (name, index) = XplaneDataRefPathParser.Parse("sim/foo/bar[2]");

            Assert.AreEqual("sim/foo/bar", name);
            Assert.AreEqual(2, index);
        }

        [TestMethod]
        public void Parse_WithIndexZero_SplitsNameAndIndex()
        {
            var (name, index) = XplaneDataRefPathParser.Parse("sim/foo/bar[0]");

            Assert.AreEqual("sim/foo/bar", name);
            Assert.AreEqual(0, index);
        }

        [TestMethod]
        public void Parse_MalformedBrackets_ReturnsOriginalPathAndNullIndex()
        {
            var (name, index) = XplaneDataRefPathParser.Parse("sim/foo/bar[x]");

            Assert.AreEqual("sim/foo/bar[x]", name);
            Assert.IsNull(index);
        }

        [TestMethod]
        public void Parse_UnterminatedBracket_ReturnsOriginalPathAndNullIndex()
        {
            var (name, index) = XplaneDataRefPathParser.Parse("sim/foo/bar[2");

            Assert.AreEqual("sim/foo/bar[2", name);
            Assert.IsNull(index);
        }

        [TestMethod]
        public void Parse_EmptyPath_ReturnsEmptyAndNullIndex()
        {
            var (name, index) = XplaneDataRefPathParser.Parse(string.Empty);

            Assert.AreEqual(string.Empty, name);
            Assert.IsNull(index);
        }
    }
}
