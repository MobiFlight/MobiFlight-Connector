using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base;
using MobiFlight.OutputConfig;
using Newtonsoft.Json;
using System.IO;
using System.Xml;

namespace MobiFlight.Base.Tests
{
    [TestClass()]
    public class ConfigItemSerializationTests
    {
        [TestMethod()]
        public void JsonSerialization_RoundTrip_WithController()
        {
            // Arrange
            var original = new OutputConfigItem
            {
                Name = "Test Output",
                Active = true,
                Controller = new Controller("TestBoard", "SN-123-456"),
                Device = new LedModule 
                { 
                    DisplayLedAddress = "0", 
                    DisplayLedConnector = 1 
                }
            };

            // Act
            var json = JsonConvert.SerializeObject(original, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<OutputConfigItem>(json);

            // Assert
            Assert.AreEqual(original.Name, deserialized.Name);
            Assert.AreEqual(original.Active, deserialized.Active);
            Assert.IsNotNull(deserialized.Controller);
            Assert.AreEqual(original.Controller.Name, deserialized.Controller.Name);
            Assert.AreEqual(original.Controller.Serial, deserialized.Controller.Serial);
            
            // Verify Controller is in JSON, not ModuleSerial
            Assert.IsTrue(json.Contains("\"Controller\""));
            Assert.IsFalse(json.Contains("ModuleSerial"));
        }

        [TestMethod()]
        public void JsonDeserialization_LegacyModuleSerial_MigratesToController()
        {
            // Arrange - simulate old config file with ModuleSerial
            var legacyJson = @"{
                ""Type"": ""OutputConfigItem"",
                ""Name"": ""Legacy Output"",
                ""Active"": true,
                ""ModuleSerial"": ""ProtoBoard-v2/ SN-5FC-1CF"",
                ""Device"": {
                    ""DisplayLedAddress"": ""0"",
                    ""DisplayLedConnector"": 1,
                    ""Name"": ""LED"",
                    ""Type"": ""LedModule""
                }
            }";

            // Act
            var deserialized = JsonConvert.DeserializeObject<OutputConfigItem>(legacyJson);

            // Assert
            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Legacy Output", deserialized.Name);
            Assert.IsNotNull(deserialized.Controller);
            Assert.AreEqual("ProtoBoard-v2", deserialized.Controller.Name);
            Assert.AreEqual("SN-5FC-1CF", deserialized.Controller.Serial);
        }

        [TestMethod()]
        public void XmlSerialization_RoundTrip_UsesModuleSerial()
        {
            // Arrange
            var original = new OutputConfigItem
            {
                Name = "Test Output",
                Active = true,
                Controller = new Controller("TestBoard", "SN-123-456"),
                Device = new LedModule 
                { 
                    DisplayLedAddress = "0", 
                    DisplayLedConnector = 1 
                }
            };

            // Act - Serialize to XML
            var xmlString = "";
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
            {
                xmlWriter.WriteStartElement("config");
                original.WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
                xmlString = stringWriter.ToString();
            }

            // Assert - XML should contain ModuleSerial attribute
            Assert.IsTrue(xmlString.Contains("serial=\"TestBoard/ SN-123-456\""));
            
            // Act - Deserialize from XML
            var deserialized = new OutputConfigItem();
            using (var stringReader = new StringReader(xmlString))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                xmlReader.ReadToFollowing("config");
                xmlReader.Read(); // Move to content
                deserialized.ReadXml(xmlReader);
            }

            // Assert - Should be able to read back and access via Controller
            Assert.IsNotNull(deserialized.Controller);
            Assert.AreEqual("TestBoard", deserialized.Controller.Name);
            Assert.AreEqual("SN-123-456", deserialized.Controller.Serial);
        }

        [TestMethod()]
        public void InputConfigItem_XmlSerialization_UsesModuleSerial()
        {
            // Arrange
            var original = new InputConfigItem
            {
                Name = "Test Input",
                Active = true,
                Controller = new Controller("ProtoBoard", "SN-ABC-123"),
                DeviceType = InputConfigItem.TYPE_BUTTON,
                DeviceName = "Button 1"
            };

            // Act - Serialize to XML
            var xmlString = "";
            using (var stringWriter = new StringWriter())
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
            {
                xmlWriter.WriteStartElement("input");
                original.WriteXml(xmlWriter);
                xmlWriter.WriteEndElement();
                xmlWriter.Flush();
                xmlString = stringWriter.ToString();
            }

            // Assert - XML should contain ModuleSerial as 'serial' attribute
            Assert.IsTrue(xmlString.Contains("serial=\"ProtoBoard/ SN-ABC-123\""));
            
            // Act - Deserialize from XML
            var deserialized = new InputConfigItem();
            using (var stringReader = new StringReader(xmlString))
            using (var xmlReader = XmlReader.Create(stringReader))
            {
                xmlReader.ReadToFollowing("input");
                xmlReader.Read(); // Move to content
                deserialized.ReadXml(xmlReader);
            }

            // Assert - Should be able to read back and access via Controller
            Assert.IsNotNull(deserialized.Controller);
            Assert.AreEqual("ProtoBoard", deserialized.Controller.Name);
            Assert.AreEqual("SN-ABC-123", deserialized.Controller.Serial);
        }

        [TestMethod()]
        public void JsonSerialization_EmptyController_NotSerialized()
        {
            // Arrange
            var item = new OutputConfigItem
            {
                Name = "Test Output",
                Active = true,
                Device = new LedModule 
                { 
                    DisplayLedAddress = "0", 
                    DisplayLedConnector = 1 
                }
            };
            // Don't set Controller - it should remain null or empty

            // Act
            var json = JsonConvert.SerializeObject(item, Formatting.Indented);

            // Assert - Controller should not appear in JSON if empty
            Assert.IsFalse(json.Contains("\"Controller\""));
        }

        [TestMethod()]
        public void JsonDeserialization_MissingController_HandlesGracefully()
        {
            // Arrange - JSON without Controller or ModuleSerial
            var json = @"{
                ""Type"": ""OutputConfigItem"",
                ""Name"": ""Test Output"",
                ""Active"": true,
                ""Device"": {
                    ""DisplayLedAddress"": ""0"",
                    ""DisplayLedConnector"": 1,
                    ""Name"": ""LED"",
                    ""Type"": ""LedModule""
                }
            }";

            // Act
            var deserialized = JsonConvert.DeserializeObject<OutputConfigItem>(json);

            // Assert - Should handle missing Controller gracefully
            Assert.IsNotNull(deserialized);
            Assert.AreEqual("Test Output", deserialized.Name);
            // Controller might be null or have empty values
            if (deserialized.Controller != null)
            {
                Assert.AreEqual("", deserialized.Controller.Name);
                Assert.AreEqual("", deserialized.Controller.Serial);
            }
        }
    }
}
