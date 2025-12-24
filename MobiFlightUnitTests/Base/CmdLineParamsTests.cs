using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Tests
{
    [TestClass()]
    public class CmdLineParamsTests
    {
        [TestMethod()]
        public void CmdLineParamsTest()
        {
            string[] cmdlineParams = new string[3] { "/autoRun", "/cfg", "c:\\test\\my.mcc" };
            CmdLineParams clp = new CmdLineParams(cmdlineParams);

            Assert.IsNotNull(clp, "Object is null.");
            Assert.IsTrue(clp.AutoRun, "AutoRun is not true.");
            Assert.AreEqual("c:\\test\\my.mcc", clp.ConfigFile, "ConfigFile doesn't match.");

            cmdlineParams = new string[3] { "/foo", "/cfg1", "c:\\test\\my.mcc" };
            clp = new CmdLineParams(cmdlineParams);

            Assert.IsNotNull(clp, "Object is null.");
            Assert.IsFalse(clp.AutoRun, "AutoRun is true.");
            Assert.IsNull(clp.ConfigFile, "ConfigFile is not null.");
        }
    }
}