using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Base;
using MobiFlight.Base.Migration;
using MobiFlight.InputConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MobiFlightUnitTests.Base.Migration
{
    [TestClass]
    public class V0_10_InputConfigItemDeviceMigrationTests
    {
        [TestMethod()]
        public void ReadJson_InputMultiplexer_WithBetaFormat_DeserializesCorrectly()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                ""ConfigFiles"": [
                    {
                        ""ConfigItems"": [
                            {
                                ""inputMultiplexer"": {
                                    ""DataPin"": 13,
                                    ""onPress"": {
                                        ""Command"": ""(L:S_OH_GPWS_TERR) ! (>L:S_OH_GPWS_TERR) "",
                                        ""PresetId"": ""f8fe1296-5c0e-409a-b2d3-608165240314"",
                                        ""Type"": ""MSFS2020CustomInputAction""
                                    },
                                    ""onRelease"": {
                                        ""Command"": ""(L:S_OH_GPWS_TERR) ! (>L:S_OH_GPWS_TERR) "",
                                        ""PresetId"": ""f8fe1296-5c0e-409a-b2d3-608165240314"",
                                        ""Type"": ""MSFS2020CustomInputAction""
                                    },
                                    ""onLongRelease"": null,
                                    ""onHold"": null,
                                    ""LongReleaseDelay"": 350,
                                    ""HoldDelay"": 350,
                                    ""RepeatDelay"": 0
                                },
                                ""Device"": {
                                    ""Type"": ""InputMultiplexer"",
                                    ""Name"": ""Multiplexer:13""
                                },
                                ""GUID"": ""2ec17354-70be-4d58-86bf-5b944fe4533d"",
                                ""Active"": true,
                                ""Name"": ""TERR PB"",
                                ""Type"": ""InputConfigItem"",
                                ""Controller"": {
                                    ""Name"": ""Overhead Left"",
                                    ""Serial"": ""SN-66E-7CA"",
                                    ""Devices"": []
                                }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);
            var rawConfigItem = result["ConfigFiles"][0]["ConfigItems"][0];
            var configItem = JsonConvert.DeserializeObject<IConfigItem>(rawConfigItem.ToString());
            // Assert
            Assert.IsNotNull(configItem);
            Assert.IsInstanceOfType(configItem, typeof(InputConfigItem));
            Assert.IsNotNull(configItem.Device);
            Assert.AreEqual("Multiplexer:13", configItem.Device.Name);
            Assert.AreEqual("Button", configItem.Device.Type);
        }

        [TestMethod]
        public void Apply_InputMultiplexerActionWithDataPin_MigratesCompletely()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [
                    {
                        'ConfigItems': [
                            {
                              'inputMultiplexer': {
                                'DataPin': 0,
                                'onPress': {
                                  'Type': 'MSFS2020CustomInputAction',
                                  'Command': '(>K:A32NX.FCU_AP_1_PUSH)',
                                  'PresetId': '7f471277-ed45-481b-aa59-6a305bc74465'
                                },
                                'onRelease': null,
                                'onLongRelease': null,
                                'onHold': null,
                                'LongReleaseDelay': 350,
                                'HoldDelay': 350,
                                'RepeatDelay': 0
                              },
                              'Device': {
                                'SubIndex': 1,
                                'Type': 'InputMultiplexer',
                                'Name': 'Multiplexer'
                              },
                              'DeviceType': 'InputMultiplexer',
                              'DeviceName': 'Multiplexer',
                              'GUID': '7a4d6020-c0f9-4017-9157-6c41e8d95d7e',
                              'Active': true,
                              'Name': 'New Input Config',
                              'Type': 'InputConfigItem',
                              'Controller': {
                                'Name': 'MobiFlight Mega',
                                'Serial': 'SN-3F1-FDD'
                              }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsNull(configItem["inputMultiplexer"]);

            var button = configItem["button"];
            Assert.IsNotNull(button);
            Assert.AreEqual("MSFS2020CustomInputAction", button["onPress"]["Type"].ToString());
            Assert.AreEqual("(>K:A32NX.FCU_AP_1_PUSH)", button["onPress"]["Command"].ToString());
            Assert.AreEqual("7f471277-ed45-481b-aa59-6a305bc74465", button["onPress"]["PresetId"].ToString());

            var device = configItem["Device"];
            Assert.IsNull(device["SubIndex"], $"SubIndex is {device["SubIndex"]}");
            Assert.IsNotNull(device["Name"]);
            Assert.AreEqual("Multiplexer:1", device["Name"].ToString());
            Assert.IsNotNull(device["Type"]);
            Assert.AreEqual("Button", device["Type"].ToString());
            Assert.IsNull(configItem["DeviceType"]);
            Assert.IsNull(configItem["DeviceName"]);
        }

        [TestMethod]
        public void Apply_InputMultiplexerActionWithDataPin_MigratesBetaVersion()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [
                    {
                        'ConfigItems': [
                            {
                              'inputMultiplexer': {
                                'DataPin': 1,
                                'onPress': {
                                  'Type': 'MSFS2020CustomInputAction',
                                  'Command': '(>K:A32NX.FCU_AP_1_PUSH)',
                                  'PresetId': '7f471277-ed45-481b-aa59-6a305bc74465'
                                },
                                'onRelease': null,
                                'onLongRelease': null,
                                'onHold': null,
                                'LongReleaseDelay': 350,
                                'HoldDelay': 350,
                                'RepeatDelay': 0
                              },
                              'Device': {
                                'Type': 'InputMultiplexer',
                                'Name': 'Multiplexer:1'
                              },
                              'GUID': '7a4d6020-c0f9-4017-9157-6c41e8d95d7e',
                              'Active': true,
                              'Name': 'New Input Config',
                              'Type': 'InputConfigItem',
                              'Controller': {
                                'Name': 'MobiFlight Mega',
                                'Serial': 'SN-3F1-FDD'
                              }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsNull(configItem["inputMultiplexer"]);

            var button = configItem["button"];
            Assert.IsNotNull(button);
            Assert.AreEqual("MSFS2020CustomInputAction", button["onPress"]["Type"].ToString());
            Assert.AreEqual("(>K:A32NX.FCU_AP_1_PUSH)", button["onPress"]["Command"].ToString());
            Assert.AreEqual("7f471277-ed45-481b-aa59-6a305bc74465", button["onPress"]["PresetId"].ToString());

            var device = configItem["Device"];
            Assert.IsNull(device["SubIndex"], $"SubIndex is {device["SubIndex"]}");
            Assert.IsNotNull(device["Name"]);
            Assert.AreEqual("Multiplexer:1", device["Name"].ToString());
            Assert.IsNotNull(device["Type"]);
            Assert.AreEqual("Button", device["Type"].ToString());
            Assert.IsNull(configItem["DeviceType"]);
            Assert.IsNull(configItem["DeviceName"]);
        }

        [TestMethod]
        public void Apply_InputMultiplexerActionWithDataPin_WithoutDeviceTypeAndName_DontBreak()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [
                    {
                        'ConfigItems': [
                            {
                              'inputMultiplexer': {
                                'DataPin': 0,
                                'onPress': {
                                  'Type': 'MSFS2020CustomInputAction',
                                  'Command': '(>K:A32NX.FCU_AP_1_PUSH)',
                                  'PresetId': '7f471277-ed45-481b-aa59-6a305bc74465'
                                },
                                'onRelease': null,
                                'onLongRelease': null,
                                'onHold': null,
                                'LongReleaseDelay': 350,
                                'HoldDelay': 350,
                                'RepeatDelay': 0
                              },
                              'Device': {
                                'SubIndex': 1,
                                'Type': 'InputMultiplexer',
                                'Name': 'Multiplexer'
                              },
                              'GUID': '7a4d6020-c0f9-4017-9157-6c41e8d95d7e',
                              'Active': true,
                              'Name': 'New Input Config',
                              'Type': 'InputConfigItem',
                              'Controller': {
                                'Name': 'MobiFlight Mega',
                                'Serial': 'SN-3F1-FDD'
                              }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsNull(configItem["inputMultiplexer"]);
            Assert.IsNotNull(configItem["button"]);

            var device = configItem["Device"];
            Assert.IsNull(device["SubIndex"], $"SubIndex is {device["SubIndex"]}");
            Assert.IsNotNull(device["Name"]);
            Assert.AreEqual("Multiplexer:1", device["Name"].ToString());
            Assert.IsNotNull(device["Type"]);
            Assert.AreEqual(InputConfigItem.TYPE_BUTTON.ToString(), device["Type"].ToString());
            Assert.IsFalse((configItem as JObject).ContainsKey("DeviceType"));
            Assert.IsFalse((configItem as JObject).ContainsKey("DeviceName"));
        }

        [TestMethod]
        public void Apply_ConfigMigration_AppliesToButton()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [
                    {
                        'ConfigItems': [
                            {
                              'button': {
                                'onPress': {
                                  'Type': 'MSFS2020CustomInputAction',
                                  'Command': '(>K:A32NX.FCU_AP_1_PUSH)',
                                  'PresetId': '7f471277-ed45-481b-aa59-6a305bc74465'
                                },
                                'onRelease': null,
                                'onLongRelease': null,
                                'onHold': null,
                                'LongReleaseDelay': 350,
                                'HoldDelay': 350,
                                'RepeatDelay': 0
                              },
                              'Device': {
                                'Type': 'Button',
                                'Name': 'Button 1'
                              },
                              'DeviceType': 'Button',
                              'DeviceName': 'Button 1',
                              'GUID': '7a4d6020-c0f9-4017-9157-6c41e8d95d7e',
                              'Active': true,
                              'Name': 'New Input Config',
                              'Type': 'InputConfigItem',
                              'Controller': {
                                'Name': 'MobiFlight Mega',
                                'Serial': 'SN-3F1-FDD'
                              }
                            }
                        ]
                    }
                ]
            }");

            // Act
            var result = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsFalse((configItem as JObject).ContainsKey("DeviceType"));
            Assert.IsFalse((configItem as JObject).ContainsKey("DeviceName"));
        }

        [TestMethod]
        public void Apply_ConfigMigration_AppliesToOutputConfig()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [
                    {
                        'ConfigItems': [
                            {
                                ""Source"": {
                                    ""SimConnectValue"": {
                                        ""UUID"": ""0c24f12b-0016-4da7-b941-587a581e5686"",
                                        ""Value"": ""(L:I_FCU_TRACK_FPA_MODE)"",
                                        ""VarType"": 2
                                    },
                                    ""Type"": ""SimConnectSource""
                                },
                                ""TestValue"": {
                                    ""type"": 1,
                                    ""Float64"": 1.0,
                                    ""String"": null
                                },
                                ""Device"": {
                                    ""Name"": ""TRK Mode On/Off"",
                                    ""Address"": ""TRK Mode On/Off"",
                                    ""Lines"": [],
                                    ""Type"": ""LcdDisplay""
                                },
                                ""DeviceType"": ""LcdDisplay"",
                                ""DeviceName"": ""TRK Mode On/Off"",
                                ""GUID"": ""71618c5b-676f-41df-8ccf-d821d685de41"",
                                ""Active"": true,
                                ""Name"": ""Fenix A320 FCU: Mode TRK"",
                                ""Type"": ""OutputConfigItem"",
                                ""Controller"": {
                                    ""Name"": ""WINWING FCU-32 + EFIS-32L + EFIS-32R"",
                                    ""Serial"": ""JS-e23aa900-bee8-11ef-8001-444553540000"",
                                ""Devices"": []
                                },
                                ""Preconditions"": [
                                    {
                                        ""Type"": ""config"",
                                        ""Ref"": ""b22e0796-c2a4-4913-9bef-66a16d58f0b3"",
                                        ""Serial"": null,
                                        ""Pin"": null,
                                        ""Operand"": ""="",
                                        ""Value"": ""0"",
                                        ""Logic"": ""and"",
                                        ""Active"": true
                                    }
                                ]
                             }
                        ]
                    }
                ]
            }");

            // Act
            var result = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsTrue((configItem as JObject).ContainsKey("DeviceType"));
            Assert.IsTrue((configItem as JObject).ContainsKey("DeviceName"));
        }

        [TestMethod()]
        public void OnDeserialized_OldJsonFormat_CreatesDeviceFromDeviceTypeAndName()
        {
            // Arrange
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [
                    {
                        'ConfigItems': [{
                            ""Type"": ""InputConfigItem"", 
                            ""GUID"": ""test-guid"",
                            ""DeviceType"": ""Button"",
                            ""DeviceName"": ""Button 1""
                            }
                        ]
                    }
                ]
            }");

            // Act
            var migratedDocument = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);
            var item = migratedDocument["ConfigFiles"][0]["ConfigItems"][0].ToString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<IConfigItem>(item);

            Assert.IsNotNull(result.Device, "Device should be populated by OnDeserialized");
            Assert.IsInstanceOfType(result.Device, typeof(Button));
            Assert.AreEqual("Button 1", result.Device.Name);
        }

        [TestMethod()]
        public void OnDeserialized_OldJsonFormat_CreatesDeviceFromDeviceTypeAndName_InputMultiplexer()
        {
            // Simulate old JSON format: DeviceType + DeviceName set, but no Device object
            // Arrange
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [
                    {
                        'ConfigItems': [{
                        ""inputMultiplexer"": {
                            ""DataPin"": 5
                        },
                        ""Type"": ""InputConfigItem"", 
                        ""GUID"": ""test-guid"",
                        ""DeviceType"": ""InputMultiplexer"",
                        ""DeviceName"": ""Multiplexer 1""
                    }]
                    }
                ]
            }");

            // Act
            var migratedDocument = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);
            var item = migratedDocument["ConfigFiles"][0]["ConfigItems"][0].ToString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<IConfigItem>(item);

            Assert.IsNotNull(result.Device, "Device should be populated by OnDeserialized");
            Assert.IsInstanceOfType(result.Device, typeof(Button));
            Assert.AreEqual("Multiplexer 1:5", result.Device.Name);
        }

        [TestMethod()]
        public void OnDeserialized_OldJsonFormat_CreatesDeviceFromDeviceTypeAndName_InputShiftRegister()
        {
            // Simulate old JSON format: DeviceType + DeviceName set, but no Device object
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [{
                    'ConfigItems': [{
                    ""inputShiftRegister"": {
                        ""ExtPin"": 5
                    },
                    ""Type"": ""InputConfigItem"", 
                    ""GUID"": ""test-guid"",
                    ""DeviceType"": ""InputShiftRegister"",
                    ""DeviceName"": ""Shift Register 1""
                    }]
                }]
            }");

            // Act
            var migratedDocument = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);
            var item = migratedDocument["ConfigFiles"][0]["ConfigItems"][0].ToString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<IConfigItem>(item);

            Assert.IsNotNull(result.Device, "Device should be populated by OnDeserialized");
            Assert.IsInstanceOfType(result.Device, typeof(Button));
            Assert.AreEqual("Shift Register 1:5", result.Device.Name);
        }

        [TestMethod()]
        public void Apply_ConfigMigration_NoConfigFiles_DontBreak()
        {
            // Simulate old JSON format: DeviceType + DeviceName set, but no Device object
            var inputDocument = JObject.Parse(@"{
                'ConfigFiles': [{
                    'ConfigItems': [{
                        ""Type"": ""InputConfigItem"", 
                        ""GUID"": ""test-guid"", 
                        ""DeviceType"": ""-"" 
                    }]
                }]
            }");

            // Act
            var migratedDocument = V0_10_InputConfigItemDeviceMigration.Apply(inputDocument);
            var item = migratedDocument["ConfigFiles"][0]["ConfigItems"][0].ToString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<InputConfigItem>(item);

            Assert.IsNull(result.Device, "Device should remain null when DeviceType is TYPE_NOTSET");
        }
    }
}