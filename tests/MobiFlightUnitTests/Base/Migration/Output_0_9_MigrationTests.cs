using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.Base.Migration;
using Newtonsoft.Json.Linq;

namespace MobiFlightUnitTests.Base.Migration
{
    [TestClass]
    public class Output_0_9_MigrationTests
    {
        [TestMethod]
        public void Apply_WithOutputDevices_MigratesPropertiesToShortNames()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""Device"": {
                                    ""Type"": ""Output"",
                                    ""DisplayPin"": ""LED1"",
                                    ""DisplayPinBrightness"": 255,
                                    ""DisplayPinPWM"": true,
                                    ""Name"": ""Test Output""
                                },
                                ""Name"": ""Test Config""
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            var device = result["ConfigFiles"][0]["ConfigItems"][0]["Device"];

            // New properties should exist
            Assert.AreEqual("LED1", device["pin"].ToString());
            Assert.AreEqual(255, device["brightness"].Value<int>());
            Assert.IsTrue(device["pwmMode"].Value<bool>());

            // Old properties should be removed
            Assert.IsNull(device["DisplayPin"]);
            Assert.IsNull(device["DisplayPinBrightness"]);
            Assert.IsNull(device["DisplayPinPWM"]);

            // Non-migrated properties should remain
            Assert.AreEqual("Test Output", device["Name"].ToString());
            Assert.AreEqual("Output", device["Type"].ToString());
        }

        [TestMethod]
        public void Apply_WithMultipleOutputDevices_MigratesAllOutputs()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""Device"": {
                                    ""Type"": ""Output"",
                                    ""DisplayPin"": ""LED1"",
                                    ""DisplayPinBrightness"": 200
                                }
                            },
                            {
                                ""Device"": {
                                    ""Type"": ""Output"",
                                    ""DisplayPin"": ""LED2"",
                                    ""DisplayPinPWM"": false
                                }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            var device1 = result["ConfigFiles"][0]["ConfigItems"][0]["Device"];
            var device2 = result["ConfigFiles"][0]["ConfigItems"][1]["Device"];

            Assert.AreEqual("LED1", device1["pin"].ToString());
            Assert.AreEqual(200, device1["brightness"].Value<int>());
            Assert.IsNull(device1["DisplayPin"]);

            Assert.AreEqual("LED2", device2["pin"].ToString());
            Assert.IsFalse(device2["pwmMode"].Value<bool>());
            Assert.IsNull(device2["DisplayPin"]);
        }

        [TestMethod]
        public void Apply_WithPartialProperties_MigratesOnlyExistingProperties()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""Device"": {
                                    ""Type"": ""Output"",
                                    ""DisplayPin"": ""LED1"",
                                    ""Name"": ""Partial Output""
                                }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            var device = result["ConfigFiles"][0]["ConfigItems"][0]["Device"];

            Assert.AreEqual("LED1", device["pin"].ToString());
            Assert.IsNull(device["brightness"]); // Should not exist if not in source
            Assert.IsNull(device["pwmMode"]); // Should not exist if not in source
            Assert.IsNull(device["DisplayPin"]); // Should be removed
        }

        [TestMethod]
        public void Apply_WithNonOutputDevices_DoesNotMigrate()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""Device"": {
                                    ""Type"": ""Input"",
                                    ""DisplayPin"": ""BTN1"",
                                    ""DisplayPinBrightness"": 128
                                }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            var device = result["ConfigFiles"][0]["ConfigItems"][0]["Device"];

            // Original properties should remain unchanged for non-Output devices
            Assert.AreEqual("BTN1", device["DisplayPin"].ToString());
            Assert.AreEqual(128, device["DisplayPinBrightness"].Value<int>());
            Assert.IsNull(device["pin"]);
            Assert.IsNull(device["brightness"]);
        }

        [TestMethod]
        public void Apply_WithEmptyDocument_ReturnsEmptyDocument()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{}");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Apply_WithNoConfigFiles_ReturnsUnchangedDocument()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""Name"": ""Test Project"",
                ""SomeOtherProperty"": ""Value""
            }");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            Assert.AreEqual("Test Project", result["Name"].ToString());
            Assert.AreEqual("Value", result["SomeOtherProperty"].ToString());
        }

        [TestMethod]
        public void Apply_WithEmptyConfigItems_ReturnsUnchangedDocument()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": []
                    }
                ]
            }");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            Assert.IsNotNull(result["ConfigFiles"]);
            Assert.AreEqual(0, ((JArray)result["ConfigFiles"][0]["ConfigItems"]).Count);
        }

        [TestMethod]
        public void Apply_WithMultipleConfigFiles_MigratesAllConfigFiles()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""Device"": {
                                    ""Type"": ""Output"",
                                    ""DisplayPin"": ""LED1""
                                }
                            }
                        ]
                    },
                    {
                        ""ConfigItems"": [
                            {
                                ""Device"": {
                                    ""Type"": ""Output"",
                                    ""DisplayPinBrightness"": 100
                                }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert
            var device1 = result["ConfigFiles"][0]["ConfigItems"][0]["Device"];
            var device2 = result["ConfigFiles"][1]["ConfigItems"][0]["Device"];

            Assert.AreEqual("LED1", device1["pin"].ToString());
            Assert.AreEqual(100, device2["brightness"].Value<int>());
        }

        [TestMethod]
        public void Apply_DoesNotModifyOriginalDocument()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""Device"": {
                                    ""Type"": ""Output"",
                                    ""DisplayPin"": ""LED1""
                                }
                            }
                        ]
                    }
                ]
            }");

            var originalPin = inputDocument["ConfigFiles"][0]["ConfigItems"][0]["Device"]["DisplayPin"].ToString();

            // Act
            var result = Output_V_0_9_Migration.Apply(inputDocument);

            // Assert - Original document should be unchanged
            Assert.AreEqual("LED1", originalPin);
            Assert.AreEqual("LED1", inputDocument["ConfigFiles"][0]["ConfigItems"][0]["Device"]["DisplayPin"].ToString());

            // Result should be different
            Assert.IsNull(result["ConfigFiles"][0]["ConfigItems"][0]["Device"]["DisplayPin"]);
            Assert.AreEqual("LED1", result["ConfigFiles"][0]["ConfigItems"][0]["Device"]["pin"].ToString());
        }
    }
}