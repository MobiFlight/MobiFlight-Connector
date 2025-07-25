using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ConfigFileUtilsTests
    {
        [TestMethod()]
        public void MergeConfigItemsTest()
        {
            var ConfigFile1 = new ConfigFile("file1.json");
            var ConfigFile2 = new ConfigFile("file2.json");

            ConfigFile1.ConfigItems.Add(new OutputConfigItem() {});
            ConfigFile2.ConfigItems.Add(new OutputConfigItem() {});

            var ConfigFile3 = new ConfigFile("file3.json");
            Assert.AreEqual(0, ConfigFile3.ConfigItems.Count);

            ConfigFileUtils.MergeConfigItems(ConfigFile3, ConfigFile1);
            Assert.AreEqual(ConfigFile1.ConfigItems.Count, ConfigFile3.ConfigItems.Count);

            // check the GUIDs of the items
            for (int i = 0; i < ConfigFile1.ConfigItems.Count; i++)
            {
                Assert.AreEqual(ConfigFile1.ConfigItems[i], ConfigFile3.ConfigItems[i]);
            }

            ConfigFileUtils.MergeConfigItems(ConfigFile3, ConfigFile2);
            Assert.AreEqual(ConfigFile1.ConfigItems.Count + ConfigFile2.ConfigItems.Count, ConfigFile3.ConfigItems.Count);

            for (int i = ConfigFile1.ConfigItems.Count; i < ConfigFile3.ConfigItems.Count; i++)
            {
                Assert.AreEqual(ConfigFile2.ConfigItems[i - ConfigFile1.ConfigItems.Count], ConfigFile3.ConfigItems[i]);
            }
        }
    }
}