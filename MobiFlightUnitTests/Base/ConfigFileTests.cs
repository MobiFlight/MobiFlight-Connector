using MobiFlight.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.OutputConfig;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MobiFlight.Base.Tests
{
    [TestClass]
    public class ConfigFileTests
    {
        private const string TestFileName = "testConfigFile.json";

        private OutputConfigItem CreateOutputConfigItem()
        {
            return new OutputConfigItem()
            {
                Device = new LedModule() { DisplayLedAddress = "0", DisplayLedConnector = 1 }
            };
        }

        private InputConfigItem CreateInputConfigItem()
        {
            return new InputConfigItem()
            {
            };
        }

        [TestInitialize]
        public void TestInitialize()
        {
            // Clean up before each test
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Clean up after each test
            if (File.Exists(TestFileName))
            {
                File.Delete(TestFileName);
            }
        }

        [TestMethod]
        public void ConfigFile_Creation_Test()
        {
            var configFile = new ConfigFile
            {
                FileName = TestFileName,
                ReferenceOnly = false,
                EmbedContent = true,
                ConfigItems = new List<IConfigItem>()
            };

            Assert.AreEqual(TestFileName, configFile.FileName);
            Assert.IsFalse(configFile.ReferenceOnly);
            Assert.IsTrue(configFile.EmbedContent);
            Assert.IsNotNull(configFile.ConfigItems);
        }

        [TestMethod]
        public void ConfigFile_Serialization_Test()
        {
            var configFile = new ConfigFile
            {
                FileName = TestFileName,
                ReferenceOnly = false,
                EmbedContent = true,
                ConfigItems = new List<IConfigItem>()
            };

            var json = configFile.ToJson();
            var deserializedConfigFile = JsonConvert.DeserializeObject<ConfigFile>(json);

            Assert.AreEqual(configFile.FileName, deserializedConfigFile.FileName);
            Assert.AreEqual(configFile.ReferenceOnly, deserializedConfigFile.ReferenceOnly);
            Assert.AreEqual(configFile.EmbedContent, deserializedConfigFile.EmbedContent);
            Assert.AreEqual(configFile.ConfigItems.Count, deserializedConfigFile.ConfigItems.Count);

            configFile.ConfigItems.Add(CreateOutputConfigItem());
            configFile.ConfigItems.Add(CreateInputConfigItem());

            json = configFile.ToJson();
            deserializedConfigFile = JsonConvert.DeserializeObject<ConfigFile>(json);

            Assert.AreEqual(configFile.FileName, deserializedConfigFile.FileName);
            Assert.AreEqual(configFile.ReferenceOnly, deserializedConfigFile.ReferenceOnly);
            Assert.AreEqual(configFile.EmbedContent, deserializedConfigFile.EmbedContent);
            Assert.AreEqual(configFile.ConfigItems.Count, deserializedConfigFile.ConfigItems.Count);
            Assert.IsTrue(configFile.ConfigItems.Count == 2);

            Assert.AreEqual((configFile.ConfigItems[0] as OutputConfigItem).Device, (deserializedConfigFile.ConfigItems[0] as OutputConfigItem).Device);
            Assert.AreEqual((configFile.ConfigItems[1] as InputConfigItem).Device, (deserializedConfigFile.ConfigItems[1] as InputConfigItem).Device);
        }

        [TestMethod]
        public void ConfigFile_OpenFile_Test()
        {
            var configFile = new ConfigFile
            {
                FileName = TestFileName,
                ReferenceOnly = false,
                EmbedContent = false,
                ConfigItems = new List<IConfigItem>()
            };

            var json = configFile.ToJson();
            File.WriteAllText(TestFileName, json);

            var newConfigFile = new ConfigFile { FileName = TestFileName, EmbedContent = false };
            newConfigFile.OpenFile();

            Assert.AreEqual(configFile.FileName, newConfigFile.FileName);
            Assert.AreEqual(configFile.ReferenceOnly, newConfigFile.ReferenceOnly);
            Assert.AreEqual(configFile.EmbedContent, newConfigFile.EmbedContent);
            Assert.AreEqual(configFile.ConfigItems.Count, newConfigFile.ConfigItems.Count);
        }

        [TestMethod]
        public void ConfigFile_SaveFile_Test()
        {
            var configFile = new ConfigFile
            {
                FileName = TestFileName,
                ReferenceOnly = false,
                EmbedContent = false,
                ConfigItems = new List<IConfigItem>()
            };

            configFile.SaveFile();

            Assert.IsTrue(File.Exists(TestFileName));

            var json = File.ReadAllText(TestFileName);
            var deserializedConfigFile = JsonConvert.DeserializeObject<ConfigFile>(json);

            Assert.AreEqual(configFile.FileName, deserializedConfigFile.FileName);
            Assert.AreEqual(configFile.ReferenceOnly, deserializedConfigFile.ReferenceOnly);
            Assert.AreEqual(configFile.EmbedContent, deserializedConfigFile.EmbedContent);
            Assert.AreEqual(configFile.ConfigItems.Count, deserializedConfigFile.ConfigItems.Count);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var configFile = new ConfigFile
            {
                FileName = TestFileName,
                ReferenceOnly = false,
                EmbedContent = false,
                ConfigItems = new List<IConfigItem>()
                {
                    CreateOutputConfigItem(),
                    CreateInputConfigItem()
                }
            };

            var configFile2 = new ConfigFile
            {
                FileName = TestFileName,
                ReferenceOnly = false,
                EmbedContent = false,
                ConfigItems = new List<IConfigItem>()
                {
                    configFile.ConfigItems[0].Clone() as OutputConfigItem,
                    configFile.ConfigItems[1].Clone() as InputConfigItem
                }
            };

            Assert.IsTrue(configFile.Equals(configFile2));
            Assert.AreEqual(configFile, configFile2);
        }
    }
}