using MobiFlight.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.BrowserMessages.Incoming.Converter
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
            var type = (string)jsonObject["Type"];

            switch (type)
            {
                case "MobiFlight.InputConfigItem":
                    return jsonObject.ToObject<InputConfigItem>(serializer);
                case "MobiFlight.OutputConfigItem":
                    return jsonObject.ToObject<OutputConfigItem>(serializer);
                default:
                    throw new NotSupportedException($"Type '{type}' is not supported");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
