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

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var typeName = $"MobiFlight.{(string)jsonObject["Type"]}";

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new NotSupportedException($"Unknown type: {typeName}");
            }

            // Migration: If ModuleSerial exists but Controller doesn't, convert ModuleSerial to Controller
            if (jsonObject["ModuleSerial"] != null && jsonObject["Controller"] == null)
            {
                var moduleSerial = (string)jsonObject["ModuleSerial"];
                var controller = SerialNumber.CreateController(moduleSerial);

                // Add Controller property to JSON
                if (controller != null)
                    jsonObject["Controller"] = JObject.FromObject(controller);

                // Remove ModuleSerial from JSON (it will be ignored during deserialization anyway)
                jsonObject.Remove("ModuleSerial");
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
