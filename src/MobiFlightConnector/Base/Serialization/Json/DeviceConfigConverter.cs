using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.Base.Serialization.Json
{
    public class DeviceConfigConverter : JsonConverter
    {
        private static readonly string[] TypeNamespaces = new[]
        {
            "MobiFlight.OutputConfig",
            "MobiFlight.InputConfig",
        };

        public override bool CanConvert(Type objectType)
        {
            return typeof(IConfigItem).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var shortTypeName = (string)jsonObject["Type"];

            Type type = null;
            foreach (var ns in TypeNamespaces)
            {
                type = Type.GetType($"{ns}.{shortTypeName}");
                if (type != null) break;
            }

            if (type == null)
            {
                throw new NotSupportedException($"Unknown device config type: {shortTypeName}");
            }

            var configItem = Activator.CreateInstance(type) as IDeviceConfig;
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
