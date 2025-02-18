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
    public class ConfigItemTests
    {
        [TestMethod()]
        public void DuplicateTest()
        {
            var OutputConfigItem = new OutputConfigItem()
            {
                Name = "Test1"
            };

            var duplicatedConfigItem = OutputConfigItem.Duplicate() as OutputConfigItem;

            Assert.IsNotNull(duplicatedConfigItem);
            Assert.AreEqual(OutputConfigItem.Name, duplicatedConfigItem.Name);
            Assert.AreNotEqual(OutputConfigItem.GUID, duplicatedConfigItem.GUID);
        }

        [TestMethod()]
        public void CloneTest()
        {
            var OutputConfigItem = new OutputConfigItem()
            {
                Name = "Test1"
            };

            var ClonedConfigItem = OutputConfigItem.Clone() as OutputConfigItem;

            Assert.IsNotNull(ClonedConfigItem);
            Assert.AreEqual(OutputConfigItem.Name, ClonedConfigItem.Name);
            Assert.AreEqual(OutputConfigItem.GUID, ClonedConfigItem.GUID);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var OutputConfigItem = new OutputConfigItem() as IConfigItem;
            
            var ConfigItems = new List<IConfigItem>() {
                OutputConfigItem
            };

            var foundConfig = ConfigItems.Find(x => x.GUID == OutputConfigItem.GUID);
            Assert.IsNotNull(foundConfig);

            Assert.AreEqual(OutputConfigItem.GUID, foundConfig.GUID);

            Assert.IsTrue(ConfigItems.Remove(foundConfig));

            Assert.AreEqual(OutputConfigItem, foundConfig);

            var InputConfigItem = new InputConfigItem() as IConfigItem;
            
            ConfigItems.Add(InputConfigItem);
            
            foundConfig = ConfigItems.Find(x => x.GUID == InputConfigItem.GUID);
            Assert.IsNotNull(foundConfig);

            Assert.AreEqual(InputConfigItem.GUID, foundConfig.GUID);

            Assert.IsTrue(ConfigItems.Remove(foundConfig));

            Assert.AreEqual(InputConfigItem, foundConfig);

        }
    }
}