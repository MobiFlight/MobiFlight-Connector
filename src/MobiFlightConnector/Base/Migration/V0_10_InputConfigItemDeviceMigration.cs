using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MobiFlight.Base.Migration
{
    /// <summary>
    /// v0.10 has the following changes:
    /// - DeviceName and DeviceType properties are removed from config and replaced by DeviceReference
    /// - InputMultiplexer Input Actions are of type button
    /// - InputShiftRegister Input Actions are of type button
    /// - DeviceReference for InputMultiplexer is like button, with the name in the format "MultiplexerName:SubIndex"
    /// - DeviceReference for InputShiftRegister is like button, with the name in the format "ShiftRegisterName:SubIndex"
    /// </summary>
    public static class V0_10_InputConfigItemDeviceMigration
    {
        public static JObject Apply(JObject document)
        {
            var migrated = document.DeepClone() as JObject;

            var configFiles = migrated["ConfigFiles"] as JArray;
            if (configFiles == null)
                return migrated;

            var migrationCount = 0;
            foreach (var configFile in configFiles)
            {
                var configItems = configFile["ConfigItems"] as JArray;
                if (configItems == null)
                    continue;

                foreach (var configItem in configItems)
                {
                    if (configItem["Type"] == null)
                    {
                        continue;
                    }
                    if (configItem["Type"].ToString() != "InputConfigItem")
                    {
                        continue;
                    }
                    InitializeDeviceIfNotPresent(configItem as JObject);
                    // Special handling for config that have been partially migrated during beta
                    MigrateDeviceNameAndTypeProperty(configItem as JObject);
                    MigrateInputDeviceActionsToButton(configItem as JObject);
                    RemoveDeviceNameAndDeviceType(configItem as JObject);
                }
            }
            return migrated;
        }

        private static void InitializeDeviceIfNotPresent(JObject configItem)
        {
            // Migration: If DeviceName and DeviceType exist but Device doesn't, convert them to Device
            var deviceName = (string)configItem["DeviceName"];
            var deviceType = (string)configItem["DeviceType"];
            if (deviceName != null && deviceType != null && configItem["Device"] == null)
            {
                var subIndex = 0;
                if (deviceType == InputConfigItem.TYPE_INPUT_MULTIPLEXER)
                {
                    var multiplexerPin = configItem["inputMultiplexer"]?["DataPin"];
                    if (multiplexerPin != null)
                    {
                        int.TryParse(multiplexerPin.ToString(), out subIndex);
                    }
                }

                if (deviceType == InputConfigItem.TYPE_INPUT_SHIFT_REGISTER)
                {
                    var pin = configItem["inputShiftRegister"]?["ExtPin"];
                    if (pin != null)
                    {
                        int.TryParse(pin.ToString(), out subIndex);
                    }
                }

                var device = InputConfigItem.CreateInputDevice(deviceType, deviceName, subIndex);
                if (device != null)
                    configItem["Device"] = JObject.FromObject(device);
            }

            // Migration: If Device still has SubIndex property, remove it
            else if (configItem["Device"]?["SubIndex"] != null)
            {
                var subIndex = 0;
                var dataPin = configItem["Device"]?["SubIndex"];
                deviceType = configItem["Device"]?["Type"]?.ToString();
                deviceName = (string)configItem["Device"]?["Name"]?.ToString();
                if (dataPin != null)
                {
                    int.TryParse(dataPin.ToString(), out subIndex);
                }
                var device = InputConfigItem.CreateInputDevice(deviceType, deviceName, subIndex);
                if (device != null)
                    configItem["Device"] = JObject.FromObject(device);
            }
        }

        private static void MigrateInputDeviceActionsToButton(JObject configItem)
        {
            var isInputMultiplexer = configItem["inputMultiplexer"] != null;
            var isInputShiftRegister = configItem["inputShiftRegister"] != null;

            // no migration necessary
            if (!isInputMultiplexer && !isInputShiftRegister) return;

            if (isInputMultiplexer)
            {
                var multiplexerPin = configItem["inputMultiplexer"]?["DataPin"];
                if (multiplexerPin == null) return;

                configItem["inputMultiplexer"]["DataPin"].Parent.Remove();

                if (configItem["button"] != null)
                    configItem["button"].Parent.Remove();

                (configItem as JObject).Add(
                    new JProperty(
                        "button",
                        configItem["inputMultiplexer"].DeepClone()
                    )
                );

                configItem["inputMultiplexer"].Parent.Remove();
            }
            else if (isInputShiftRegister)
            {
                var pin = configItem["inputShiftRegister"]?["ExtPin"];

                if (pin == null) return;

                configItem["inputShiftRegister"]["ExtPin"].Parent.Remove();

                if (configItem["button"] != null)
                    configItem["button"].Parent.Remove();

                (configItem as JObject).Add(
                    new JProperty(
                        "button",
                        configItem["inputShiftRegister"].DeepClone()
                    )
                );

                configItem["inputShiftRegister"].Parent.Remove();
            }
        }

        private static void RemoveDeviceNameAndDeviceType(JObject configItem)
        {
            // Clean up old properties
            if (configItem["DeviceName"] != null)
                configItem["DeviceName"].Parent.Remove();

            if (configItem["DeviceType"] != null)
                configItem["DeviceType"].Parent.Remove();
        }

        private static List<JObject> FindDevicesInDocument(JObject document)
        {
            return FindPropertiesInDocument(document, "Device");
        }

        private static List<JObject> FindPropertiesInDocument(JObject document, string propertyName)
        {
            var result = new List<JObject>();

            // Look in ConfigFiles -> ConfigItems -> Devices
            var configFiles = document["ConfigFiles"] as JArray;
            if (configFiles != null)
            {
                foreach (var configFile in configFiles)
                {
                    var configItems = configFile["ConfigItems"] as JArray;
                    if (configItems != null)
                    {
                        foreach (var configItem in configItems)
                        {
                            var property = configItem[propertyName] as JObject;
                            if (property != null)
                            {
                                result.Add(property);
                            }
                        }
                    }
                }
            }

            return result;
        }

        private static void MigrateDeviceNameAndTypeProperty(JObject configItem)
        {
            // Find all Device objects in the document
            var device = configItem["Device"] as JObject;
            if (device == null)
                return;

            var deviceType = device["Type"]?.ToString();

            // We only deal with InputMultiplexer and InputShiftRegister here
            if (deviceType != InputConfigItem.TYPE_INPUT_MULTIPLEXER && deviceType != InputConfigItem.TYPE_INPUT_SHIFT_REGISTER)
                return;

            var deviceName = device["Name"]?.ToString();
            // this update might have happened during beta
            if (!deviceName.Contains(":"))
            {
                var subIndex = device["SubIndex"]?.ToString() ?? "0";
                device["Name"] = $"{deviceName}:{subIndex}";
            }

            device["SubIndex"]?.Parent?.Remove();
            device["Type"] = InputConfigItem.TYPE_BUTTON;
        }
    }
}