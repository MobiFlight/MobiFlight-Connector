using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
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
        Precondition CreatePrecondition()
        {
            return new Precondition();
        }
        IConfigItem CreateOutputConfigItem()
        {
            var PreconditionList = new PreconditionList
            {
                CreatePrecondition()
            };

            var ModifierList = new ModifierList();
            ModifierList.Items.Add(new Comparison());
            ModifierList.Items.Add(new Transformation());

            var ConfigRefList = new ConfigRefList();
            ConfigRefList.Add(new ConfigRef() { Active = true, Placeholder = "#", Ref = "123", TestValue = "0" });

            return new OutputConfigItem()
            {
                Name = "Test1",
                Device = new LedModule() {  DisplayLedAddress = "0", DisplayLedConnector = 1 },
                Active = true,
                Preconditions = PreconditionList,
                Modifiers = ModifierList,
                ConfigRefs = ConfigRefList
            };
        }

        [TestMethod()]
        public void DuplicateTest()
        {
            var OutputConfigItem = CreateOutputConfigItem() as OutputConfigItem;

            var duplicatedConfigItem = OutputConfigItem.Duplicate() as OutputConfigItem;

            Assert.IsNotNull(duplicatedConfigItem);
            Assert.AreEqual(OutputConfigItem.Name, duplicatedConfigItem.Name);
            Assert.AreNotEqual(OutputConfigItem.GUID, duplicatedConfigItem.GUID);
            Assert.AreEqual(OutputConfigItem.Device, duplicatedConfigItem.Device);
            Assert.AreEqual(OutputConfigItem.Active, duplicatedConfigItem.Active);
            Assert.AreEqual(OutputConfigItem.Preconditions, duplicatedConfigItem.Preconditions);
            Assert.AreEqual(OutputConfigItem.Modifiers, duplicatedConfigItem.Modifiers);
            Assert.AreEqual(OutputConfigItem.ConfigRefs, duplicatedConfigItem.ConfigRefs);
        }

        [TestMethod()]
        public void CloneTest()
        {
            var OutputConfigItem = CreateOutputConfigItem();

            var ClonedConfigItem = OutputConfigItem.Clone() as OutputConfigItem;

            Assert.IsNotNull(ClonedConfigItem);
            Assert.AreEqual(OutputConfigItem.Name, ClonedConfigItem.Name);
            Assert.AreEqual(OutputConfigItem.GUID, ClonedConfigItem.GUID);
            Assert.AreEqual(OutputConfigItem.Device, ClonedConfigItem.Device);
            Assert.AreEqual(OutputConfigItem.Active, ClonedConfigItem.Active);
            Assert.AreEqual(OutputConfigItem.Preconditions, ClonedConfigItem.Preconditions);
            Assert.AreEqual(OutputConfigItem.Modifiers, ClonedConfigItem.Modifiers);
            Assert.AreEqual(OutputConfigItem.ConfigRefs, ClonedConfigItem.ConfigRefs);
        }

        [TestMethod()]
        public void EqualsTest()
        {
            var OutputConfigItem = CreateOutputConfigItem();
            var ConfigItems = new List<IConfigItem>() {
                OutputConfigItem
            };

            var OutputConfigItemOther = OutputConfigItem.Clone() as IConfigItem;
            var ConfigItemsOther = new List<IConfigItem>() {
                OutputConfigItemOther
            };
            CollectionAssert.AreEqual(ConfigItems, ConfigItemsOther);

            // this test failed because of wrong Equals implementation
            // Let's verify it works for OutputConfigItems
            var foundConfig = ConfigItems.Find(x => x.GUID == OutputConfigItem.GUID);
            Assert.IsNotNull(foundConfig);
            Assert.AreEqual(OutputConfigItem.GUID, foundConfig.GUID);
            Assert.IsTrue(ConfigItems.Remove(foundConfig));
            Assert.AreEqual(OutputConfigItem, foundConfig);
            // Let's verify it works for InputConfigItem, too
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