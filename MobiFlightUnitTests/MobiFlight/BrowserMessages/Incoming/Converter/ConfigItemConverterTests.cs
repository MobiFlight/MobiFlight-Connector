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

        [TestMethod]
        public void ReadJson_OutputConfigItem_NormalizesModuleSerial()
        {
            // Old format with space after slash
            var json = "{\"Type\":\"OutputConfigItem\",\"Name\":\"Test\",\"ModuleSerial\":\"Overhead_1/ 000512500000\"}";
            var result = JsonConvert.DeserializeObject<ConfigItem>(json);

            Assert.IsInstanceOfType(result, typeof(OutputConfigItem));
            Assert.AreEqual("Overhead_1/000512500000", ((OutputConfigItem)result).ModuleSerial, "Serial should be normalized without space");

            // Old format with space before and after slash
            json = "{\"Type\":\"OutputConfigItem\",\"Name\":\"Test\",\"ModuleSerial\":\"Overhead_1 / 000512500000\"}";
            result = JsonConvert.DeserializeObject<ConfigItem>(json);

            Assert.IsInstanceOfType(result, typeof(OutputConfigItem));
            Assert.AreEqual("Overhead_1/000512500000", ((OutputConfigItem)result).ModuleSerial, "Serial should be normalized without space");
        }

        [TestMethod]
        public void ReadJson_InputConfigItem_NormalizesModuleSerial()
        {
            var json = "{\"Type\":\"InputConfigItem\",\"Name\":\"Test\",\"ModuleSerial\":\"Device/ Serial123\"}";
            var result = JsonConvert.DeserializeObject<ConfigItem>(json);

            Assert.IsInstanceOfType(result, typeof(InputConfigItem));
            Assert.AreEqual("Device/Serial123", ((InputConfigItem)result).ModuleSerial, "Serial should be normalized without space");

            json = "{\"Type\":\"InputConfigItem\",\"Name\":\"Test\",\"ModuleSerial\":\"Device / Serial123\"}";
            result = JsonConvert.DeserializeObject<ConfigItem>(json);

            Assert.IsInstanceOfType(result, typeof(InputConfigItem));
            Assert.AreEqual("Device/Serial123", ((InputConfigItem)result).ModuleSerial, "Serial should be normalized without space");
        }
    }
}