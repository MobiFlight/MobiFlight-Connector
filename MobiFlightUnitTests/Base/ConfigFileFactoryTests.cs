using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ConfigFileFactoryTests
    {
        [TestMethod()]
        public void CreateConfigFileTest()
        {
            var testFile = @"assets/Base/ConfigFile/OpenFileTest.LCD-serialization-bc.mcc";
            var configFile = ConfigFileFactory.CreateConfigFile(testFile);

            configFile.OpenFile();
            Assert.IsTrue(configFile.ConfigItems.Count > 0);

            testFile = @"assets/Base/ConfigFile/Json/OpenConfig.mfjson";
            configFile = ConfigFileFactory.CreateConfigFile(testFile);
            configFile.OpenFile();
            Assert.IsTrue(configFile.ConfigItems.Count == 3);
        }

        [TestMethod]
        public void ConfigFile_BackwardCompatibility_Test()
        {
            var testFile = @"assets/Base/ConfigFile/OpenFileTest.LCD-serialization-bc.mcc";
            var configFile = new DeprecatedConfigFile { FileName = testFile };
            configFile.OpenFile();

            var newConfigFile = new ConfigFile()
            {
                FileName = testFile,
                ReferenceOnly = configFile.ReferenceOnly,
                EmbedContent = false,
                ConfigItems = configFile.ConfigItems
            };

            var testFileOut = @"assets/Base/ConfigFile/OpenFileTest.LCD-serialization-bc.mfjson";
            newConfigFile.FileName = testFileOut;
            newConfigFile.SaveFile();

            CollectionAssert.AreEqual(configFile.ConfigItems, newConfigFile.ConfigItems);

            var configFileOut = ConfigFileFactory.CreateConfigFile(testFileOut);
            configFileOut.OpenFile();
            configFileOut.FileName = testFileOut;

            Assert.AreEqual(configFile.ConfigItems[0].Name, configFileOut.ConfigItems[0].Name);
            Assert.AreEqual(configFile.ConfigItems[0].Value, configFileOut.ConfigItems[0].Value);
            Assert.AreEqual(configFile.ConfigItems[0].RawValue, configFileOut.ConfigItems[0].RawValue);
            Assert.AreEqual(configFile.ConfigItems[0].Device, configFileOut.ConfigItems[0].Device);
            Assert.AreEqual(configFile.ConfigItems[0].GUID, configFileOut.ConfigItems[0].GUID);
            Assert.AreEqual(configFile.ConfigItems[0].Type, configFileOut.ConfigItems[0].Type);
            Assert.AreEqual(configFile.ConfigItems[0].Active, configFileOut.ConfigItems[0].Active);
            Assert.AreEqual(configFile.ConfigItems[0].ConfigRefs, configFileOut.ConfigItems[0].ConfigRefs);
            Assert.AreEqual(configFile.ConfigItems[0].Modifiers, configFileOut.ConfigItems[0].Modifiers);
            Assert.AreEqual(configFile.ConfigItems[0].ModuleSerial, configFileOut.ConfigItems[0].ModuleSerial);
            Assert.AreEqual(configFile.ConfigItems[0].Preconditions, configFileOut.ConfigItems[0].Preconditions);

            Assert.IsTrue(configFileOut.ConfigItems.Count > 0);
            for (int i = 0; i < configFile.ConfigItems.Count; i++)
            {
                Assert.AreEqual(configFile.ConfigItems[i], configFileOut.ConfigItems[i]);
            }

            Assert.AreEqual(newConfigFile, configFileOut);
        }
    }
}