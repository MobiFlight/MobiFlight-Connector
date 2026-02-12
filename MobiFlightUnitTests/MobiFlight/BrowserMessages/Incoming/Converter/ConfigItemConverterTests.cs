using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.Base.Serialization.Json;
using Newtonsoft.Json;
using System;

namespace MobiFlight.BrowserMessages.Incoming.Converter.Tests
{
    [TestClass()]
    public class ConfigItemConverterTests
    {
        [TestMethod]
        public void CanConvertTest()
        {
            var converter = new ConfigItemConverter();
            Assert.IsTrue(converter.CanConvert(typeof(InputConfigItem)));
            Assert.IsTrue(converter.CanConvert(typeof(OutputConfigItem)));
            Assert.IsFalse(converter.CanConvert(typeof(string)));
        }

        [TestMethod]
        public void ReadJson_InputConfigItem_DeserializesCorrectly()
        {
            var json = "{\"Type\":\"InputConfigItem\",\"Name\":\"SomeValue\"}";
            var result = JsonConvert.DeserializeObject<ConfigItem>(json);

            Assert.IsInstanceOfType(result, typeof(InputConfigItem));
            Assert.AreEqual("SomeValue", ((InputConfigItem)result).Name);
        }

        [TestMethod]
        public void ReadJson_OutputConfigItem_DeserializesCorrectly()
        {
            var json = "{\"Type\":\"OutputConfigItem\",\"Name\":\"SomeValue\"}";
            var result = JsonConvert.DeserializeObject<ConfigItem>(json);

            Assert.IsInstanceOfType(result, typeof(OutputConfigItem));
            Assert.AreEqual("SomeValue", ((OutputConfigItem)result).Name);
        }

        [TestMethod]
        public void ReadJson_UnsupportedType_ThrowsNotSupportedException()
        {
            var json = "{\"Type\":\"UnsupportedConfigItem\"}";
            Assert.Throws<NotSupportedException>(() => JsonConvert.DeserializeObject<ConfigItem>(json));
        }

        [TestMethod]
        public void WriteJson_InputConfigItem_SerializesCorrectly()
        {
            var item = new InputConfigItem { Name = "SomeValue" };
            var json = JsonConvert.SerializeObject(item);

            StringAssert.Contains(json, "\"Type\":\"InputConfigItem\"");
            StringAssert.Contains(json, "\"Name\":\"SomeValue\"");
        }

        [TestMethod]
        public void WriteJson_OutputConfigItem_SerializesCorrectly()
        {
            var item = new OutputConfigItem { Name = "SomeValue" };
            var json = JsonConvert.SerializeObject(item);

            StringAssert.Contains(json, "\"Type\":\"OutputConfigItem\"");
            StringAssert.Contains(json, "\"Name\":\"SomeValue\"");
        }

        [TestMethod()]
        public void ReadJson_WithModuleSerial_MigratesToController()
        {
            // Arrange
            var json = @"{
                ""Type"": ""OutputConfigItem"",
                ""ModuleSerial"": ""ProtoBoard-v2/ SN-5FC-1CF"",
                ""Name"": ""Test1"",
                ""Active"": true
            }";

            // Act
            var configItem = JsonConvert.DeserializeObject<IConfigItem>(json);

            // Assert
            Assert.IsNotNull(configItem);
            Assert.IsInstanceOfType(configItem, typeof(OutputConfigItem));
            Assert.IsNotNull(configItem.Controller);
            Assert.AreEqual("ProtoBoard-v2", configItem.Controller.Name);
            Assert.AreEqual("SN-5FC-1CF", configItem.Controller.Serial);
        }

        [TestMethod()]
        public void ReadJson_WithController_UsesControllerProperty()
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
            var configItem = JsonConvert.DeserializeObject<IConfigItem>(json);

            // Assert
            Assert.IsNotNull(configItem);
            Assert.IsInstanceOfType(configItem, typeof(OutputConfigItem));
            Assert.IsNotNull(configItem.Controller);
            Assert.AreEqual("TestBoard", configItem.Controller.Name);
            Assert.AreEqual("SN-123-456", configItem.Controller.Serial);
        }

        [TestMethod()]
        public void ReadJson_WithBothModuleSerialAndController_PrefersController()
        {
            // Arrange - this shouldn't normally happen, but test priority
            var json = @"{
                ""Type"": ""OutputConfigItem"",
                ""ModuleSerial"": ""OldBoard/ SN-OLD"",
                ""Controller"": {
                    ""Name"": ""NewBoard"",
                    ""Serial"": ""SN-NEW""
                },
                ""Name"": ""Test1"",
                ""Active"": true
            }";

            // Act
            var configItem = JsonConvert.DeserializeObject<IConfigItem>(json);

            // Assert
            Assert.IsNotNull(configItem);
            Assert.IsNotNull(configItem.Controller);
            // Should use Controller since it takes precedence
            Assert.AreEqual("NewBoard", configItem.Controller.Name);
            Assert.AreEqual("SN-NEW", configItem.Controller.Serial);
        }

        [TestMethod()]
        public void WriteJson_WithController_SerializesControllerNotModuleSerial()
        {
            // Arrange
            var configItem = new OutputConfigItem
            {
                Name = "Test1",
                Active = true,
                Controller = new Controller() { Name = "TestBoard", Serial = "SN-123-456" }
            };

            // Act
            var json = JsonConvert.SerializeObject(configItem, Formatting.Indented);

            // Assert
            Assert.Contains("\"Controller\"", json);
            Assert.Contains("\"Name\": \"TestBoard\"", json);
            Assert.Contains("\"Serial\": \"SN-123-456\"", json);
        }

        [TestMethod()]
        public void ReadJson_EmptyModuleSerial_ControllerIsNull()
        {
            // Arrange
            var json = @"{
                ""Type"": ""OutputConfigItem"",
                ""ModuleSerial"": """",
                ""Name"": ""Test1"",
                ""Active"": true
            }";

            // Act
            var configItem = JsonConvert.DeserializeObject<IConfigItem>(json);

            // Assert
            Assert.IsNotNull(configItem);
            // Empty ModuleSerial should result in null controller
            Assert.IsNull(configItem.Controller);
        }
    }
}