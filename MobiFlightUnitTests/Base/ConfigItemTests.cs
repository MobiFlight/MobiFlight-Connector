using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Modifier;
using MobiFlight.OutputConfig;
using Newtonsoft.Json;
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

        [TestMethod()]
        public void Controller_SetViaProperty_UpdatesModuleSerial()
        {
            // Arrange
            var outputConfigItem = new OutputConfigItem();
            var controller = new Controller("TestBoard", "SN-123-456");

            // Act
            outputConfigItem.Controller = controller;

            // Assert
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.AreEqual("TestBoard/ SN-123-456", outputConfigItem.ModuleSerial);
#pragma warning restore CS0618
        }

        [TestMethod()]
        public void ModuleSerial_SetViaProperty_CreatesController()
        {
            // Arrange
            var outputConfigItem = new OutputConfigItem();

            // Act
#pragma warning disable CS0618 // Type or member is obsolete
            outputConfigItem.ModuleSerial = "ProtoBoard-v2/ SN-5FC-1CF";
#pragma warning restore CS0618

            // Assert
            Assert.IsNotNull(outputConfigItem.Controller);
            Assert.AreEqual("ProtoBoard-v2", outputConfigItem.Controller.Name);
            Assert.AreEqual("SN-5FC-1CF", outputConfigItem.Controller.Serial);
        }

        [TestMethod()]
        public void Controller_JsonSerialization_SerializesControllerProperty()
        {
            // Arrange
            var outputConfigItem = CreateOutputConfigItem() as OutputConfigItem;
            outputConfigItem.Controller = new Controller("TestBoard", "SN-123-456");

            // Act
            var json = JsonConvert.SerializeObject(outputConfigItem, Formatting.Indented);

            // Assert
            Assert.IsTrue(json.Contains("\"Controller\""));
            Assert.IsTrue(json.Contains("\"Name\": \"TestBoard\""));
            Assert.IsTrue(json.Contains("\"Serial\": \"SN-123-456\""));
            Assert.IsFalse(json.Contains("ModuleSerial")); // ModuleSerial should NOT be in JSON
        }

        [TestMethod()]
        public void Controller_JsonDeserialization_PopulatesControllerFromModuleSerial()
        {
            // Arrange
            var json = @"{
                ""Type"": ""OutputConfigItem"",
                ""ModuleSerial"": ""ProtoBoard-v2/ SN-5FC-1CF"",
                ""Name"": ""Test1"",
                ""Active"": true
            }";

            // Act
            var outputConfigItem = JsonConvert.DeserializeObject<OutputConfigItem>(json);

            // Assert
            Assert.IsNotNull(outputConfigItem.Controller);
            Assert.AreEqual("ProtoBoard-v2", outputConfigItem.Controller.Name);
            Assert.AreEqual("SN-5FC-1CF", outputConfigItem.Controller.Serial);
        }

        [TestMethod()]
        public void Controller_JsonDeserialization_PopulatesFromControllerProperty()
        {
            // Arrange
            var json = @"{
                ""Type"": ""OutputConfigItem"",
                ""Controller"": {
                    ""Name"": ""TestBoard"",
                    ""Serial"": ""SN-123-456""
                },
                ""Name"": ""Test1"",
                ""Active"": true
            }";

            // Act
            var outputConfigItem = JsonConvert.DeserializeObject<OutputConfigItem>(json);

            // Assert
            Assert.IsNotNull(outputConfigItem.Controller);
            Assert.AreEqual("TestBoard", outputConfigItem.Controller.Name);
            Assert.AreEqual("SN-123-456", outputConfigItem.Controller.Serial);
        }

        [TestMethod()]
        public void Equals_WithSameController_ReturnsTrue()
        {
            // Arrange
            var item1 = CreateOutputConfigItem() as OutputConfigItem;
            item1.Controller = new Controller("TestBoard", "SN-123");

            var item2 = item1.Clone() as OutputConfigItem;

            // Assert
            Assert.IsTrue(item1.Equals(item2));
        }

        [TestMethod()]
        public void Equals_WithDifferentController_ReturnsFalse()
        {
            // Arrange
            var item1 = CreateOutputConfigItem() as OutputConfigItem;
            item1.Controller = new Controller("TestBoard1", "SN-123");

            var item2 = item1.Clone() as OutputConfigItem;
            item2.Controller = new Controller("TestBoard2", "SN-456");

            // Assert
            Assert.IsFalse(item1.Equals(item2));
        }

        [TestMethod()]
        public void Clone_CopiesController()
        {
            // Arrange
            var original = CreateOutputConfigItem() as OutputConfigItem;
            original.Controller = new Controller("TestBoard", "SN-123");

            // Act
            var clone = original.Clone() as OutputConfigItem;

            // Assert
            Assert.IsNotNull(clone.Controller);
            Assert.AreEqual(original.Controller.Name, clone.Controller.Name);
            Assert.AreEqual(original.Controller.Serial, clone.Controller.Serial);
            // Ensure it's a deep copy
            clone.Controller.Name = "ModifiedBoard";
            Assert.AreEqual("TestBoard", original.Controller.Name);
        }
    }
}