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

            IConfigItem configItem;
            switch (type)
            {
                case "MobiFlight.InputConfigItem":
                    configItem = new InputConfigItem();
                    break;
                case "MobiFlight.OutputConfigItem":
                    configItem = new OutputConfigItem();
                    break;
                default:
                    throw new NotSupportedException($"Type '{type}' is not supported");
            }

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
