using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.Base.Serialization.Json
{
    public class ConfigItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IConfigItem).IsAssignableFrom(objectType);
        }

        protected JObject HandleInputConfigItemMigration(JObject jsonObject)
        {
            var deviceName = jsonObject["DeviceName"]?.ToString();
            var deviceType = jsonObject["DeviceType"]?.ToString();

            if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(deviceType))
            {
                // Migrate DeviceName and DeviceType to Device
                jsonObject["Device"] = new JObject
                {
                    ["Name"] = deviceName,
                    ["Type"] = $"MobiFlight.MobiFlight{deviceType}"
                };
            }

            return jsonObject;
        }

        protected JObject HandleMigration(JObject jsonObject)
        {
            var type = jsonObject["Type"]?.ToString();
            if (type == "InputConfigItem")
            {
                // Handle InputConfigItem migration
                jsonObject = HandleInputConfigItemMigration(jsonObject);
            }

            if (type == "OutputConfigItem")
            {
                // Handle OutputConfigItem migration
                var outputType = jsonObject["Device"]?["Type"]?.ToString();
                if (jsonObject["Device"] != null)
                {
                    jsonObject["Device"]["Type"] = $"MobiFlight.OutputConfig.{outputType}"; // Ensure type is set correctly
                }
            }

            jsonObject.Remove("DeviceName");
            jsonObject.Remove("DeviceType");

            return jsonObject;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            jsonObject = HandleMigration(jsonObject);

            var typeName = $"MobiFlight.{(string)jsonObject["Type"]}";

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new NotSupportedException($"Unknown type: {typeName}");
            }

            var configItem = Activator.CreateInstance(type) as IConfigItem;
            serializer.Populate(jsonObject.CreateReader(), configItem);
            return configItem;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Use default serialization
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
