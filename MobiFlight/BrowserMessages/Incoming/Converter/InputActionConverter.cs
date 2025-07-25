using MobiFlight.InputConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.BrowserMessages.Incoming.Converter
{
    public class InputActionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(InputAction).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(type.Name); // Write the type discriminator
            foreach (var property in type.GetProperties())
            {
                if (property.CanRead)
                {
                    var propertyValue = property.GetValue(value);
                    if (propertyValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                    {
                        continue; // Skip null values if NullValueHandling is set to Ignore
                    }
                    writer.WritePropertyName(property.Name);
                    serializer.Serialize(writer, property.GetValue(value));
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var typeName = $"MobiFlight.InputConfig.{jsonObject["Type"]?.ToString()}";

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new JsonSerializationException($"Unknown type: {typeName}");
            }

            var action = Activator.CreateInstance(type) as InputAction;
            serializer.Populate(jsonObject.CreateReader(), action);
            return action;
        }
    }
}
