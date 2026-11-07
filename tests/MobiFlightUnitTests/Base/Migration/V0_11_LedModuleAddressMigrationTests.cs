using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base.Migration;
using Newtonsoft.Json.Linq;

namespace MobiFlightUnitTests.Base.Migration
{
    [TestClass]
    public class V0_11_LedModuleAddressMigrationTests
    {
        private static JObject CreateDocument(string deviceJson)
        {
            return JObject.Parse($@"{{
                ""ConfigFiles"": [
                    {{
                        ""ConfigItems"": [
                            {{ ""Device"": {deviceJson} }}
                        ]
                    }}
                ]
            }}");
        }

        private static JToken GetDevice(JObject document)
        {
            return document["ConfigFiles"][0]["ConfigItems"][0]["Device"];
        }

        #region Address and connector

        [TestMethod]
        public void Apply_ShouldMergeConnectorIntoAddress_WhenConnectorIsPresent()
        {
            // Arrange
            var document = CreateDocument(@"{ ""Type"": ""LedModule"", ""DisplayLedAddress"": ""Display"", ""DisplayLedConnector"": 2 }");

            // Act
            var result = V0_11_LedModuleAddressMigration.Apply(document);

            // Assert
            var device = GetDevice(result);
            Assert.AreEqual("Display:2", device["DisplayLedAddress"].ToString());
            Assert.IsNull(device["DisplayLedConnector"]);
        }

        [TestMethod]
        public void Apply_ShouldAppendFirstModule_WhenConnectorIsMissing()
        {
            // Arrange
            var document = CreateDocument(@"{ ""Type"": ""LedModule"", ""DisplayLedAddress"": ""Display"" }");

            // Act
            var result = V0_11_LedModuleAddressMigration.Apply(document);

            // Assert
            Assert.AreEqual("Display:1", GetDevice(result)["DisplayLedAddress"].ToString());
        }

        [TestMethod]
        public void Apply_ShouldKeepAddress_WhenAddressAlreadyHasModule()
        {
            // Arrange
            var document = CreateDocument(@"{ ""Type"": ""LedModule"", ""DisplayLedAddress"": ""Display:3"" }");

            // Act
            var result = V0_11_LedModuleAddressMigration.Apply(document);

            // Assert
            Assert.AreEqual("Display:3", GetDevice(result)["DisplayLedAddress"].ToString());
        }

        [TestMethod]
        public void Apply_ShouldKeepOtherProperties_WhenMigratingLedModule()
        {
            // Arrange
            var document = CreateDocument(@"{ ""Type"": ""LedModule"", ""DisplayLedAddress"": ""Display"", ""DisplayLedConnector"": 1, ""DisplayLedModuleSize"": 5 }");

            // Act
            var result = V0_11_LedModuleAddressMigration.Apply(document);

            // Assert
            var device = GetDevice(result);
            Assert.AreEqual(5, device["DisplayLedModuleSize"].Value<int>());
            Assert.AreEqual("LedModule", device["Type"].ToString());
        }

        #endregion

        #region Other documents

        [TestMethod]
        public void Apply_ShouldNotTouchDevice_WhenDeviceIsNotLedModule()
        {
            // Arrange
            var document = CreateDocument(@"{ ""Type"": ""Output"", ""pin"": ""A01"", ""DisplayLedConnector"": 2 }");

            // Act
            var result = V0_11_LedModuleAddressMigration.Apply(document);

            // Assert
            var device = GetDevice(result);
            Assert.AreEqual("A01", device["pin"].ToString());
            Assert.AreEqual(2, device["DisplayLedConnector"].Value<int>());
        }

        [TestMethod]
        public void Apply_ShouldNotModifyInputDocument_WhenMigrating()
        {
            // Arrange
            var document = CreateDocument(@"{ ""Type"": ""LedModule"", ""DisplayLedAddress"": ""Display"", ""DisplayLedConnector"": 2 }");

            // Act
            V0_11_LedModuleAddressMigration.Apply(document);

            // Assert
            Assert.AreEqual("Display", GetDevice(document)["DisplayLedAddress"].ToString());
        }

        [TestMethod]
        public void Apply_ShouldReturnDocument_WhenThereAreNoConfigFiles()
        {
            // Arrange
            var document = JObject.Parse(@"{ ""_version"": ""0.10"" }");

            // Act
            var result = V0_11_LedModuleAddressMigration.Apply(document);

            // Assert
            Assert.AreEqual("0.10", result["_version"].ToString());
        }

        #endregion
    }
}