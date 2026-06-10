using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight;
using MobiFlight.Base;
using MobiFlight.Base.Migration;
using MobiFlight.InputConfig;
using Newtonsoft.Json.Linq;

namespace MobiFlightUnitTests.Base.Migration
{
    [TestClass]
    public class InputConfig_V_0_10_MigrationTests
    {
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
            var result = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsNull(configItem["inputMultiplexer"]);

            var button = configItem["button"];
            Assert.IsNotNull(button);
            Assert.AreEqual("MSFS2020CustomInputAction", button["onPress"]["Type"]);
            Assert.AreEqual("(>K:A32NX.FCU_AP_1_PUSH)", button["onPress"]["Command"]);
            Assert.AreEqual("7f471277-ed45-481b-aa59-6a305bc74465", button["onPress"]["PresetId"]);

            var device = configItem["Device"];
            Assert.IsNull(device["SubIndex"], $"SubIndex is {device["SubIndex"]}");
            Assert.IsNotNull(device["Name"]);
            Assert.AreEqual("Multiplexer:1", device["Name"]);
            Assert.IsNotNull(device["Type"]);
            Assert.AreEqual("Button", device["Type"]);
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
            var result = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsNull(configItem["inputMultiplexer"]);

            var button = configItem["button"];
            Assert.IsNotNull(button);
            Assert.AreEqual("MSFS2020CustomInputAction", button["onPress"]["Type"]);
            Assert.AreEqual("(>K:A32NX.FCU_AP_1_PUSH)", button["onPress"]["Command"]);
            Assert.AreEqual("7f471277-ed45-481b-aa59-6a305bc74465", button["onPress"]["PresetId"]);

            var device = configItem["Device"];
            Assert.IsNull(device["SubIndex"], $"SubIndex is {device["SubIndex"]}");
            Assert.IsNotNull(device["Name"]);
            Assert.AreEqual("Multiplexer:1", device["Name"]);
            Assert.IsNotNull(device["Type"]);
            Assert.AreEqual("Button", device["Type"]);
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
            var result = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsNull(configItem["inputMultiplexer"]);
            Assert.IsNotNull(configItem["button"]);

            var device = configItem["Device"];
            Assert.IsNull(device["SubIndex"], $"SubIndex is {device["SubIndex"]}");
            Assert.IsNotNull(device["Name"]);
            Assert.AreEqual("Multiplexer:1", device["Name"]);
            Assert.IsNotNull(device["Type"]);
            Assert.AreEqual(InputConfigItem.TYPE_BUTTON.ToString(), device["Type"]);
            Assert.IsNull(configItem["DeviceType"]);
            Assert.IsNull(configItem["DeviceName"]);
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
            var result = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);

            // Assert
            var configItem = result["ConfigFiles"][0]["ConfigItems"][0];
            Assert.IsNull(configItem["DeviceType"]);
            Assert.IsNull(configItem["DeviceName"]);
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
            var migratedDocument = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);
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
            var migratedDocument = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);
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
            var migratedDocument = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);
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
            var migratedDocument = V0_10_ConfigItemDeviceMigration.Apply(inputDocument);
            var item = migratedDocument["ConfigFiles"][0]["ConfigItems"][0].ToString();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<InputConfigItem>(item);

            Assert.IsNull(result.Device, "Device should remain null when DeviceType is TYPE_NOTSET");
        }
    }
}