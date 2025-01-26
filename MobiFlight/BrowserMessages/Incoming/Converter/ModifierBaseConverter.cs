using MobiFlight.Modifier;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.BrowserMessages.Incoming.Converter
{
    public class ModifierBaseConverter : JsonConverter<ModifierBase>
    {
        public override void WriteJson(JsonWriter writer, ModifierBase value, JsonSerializer serializer)
        {
            var type = value.GetType();
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(type.Name); // Write the type discriminator
            foreach (var property in type.GetProperties())
            {
                if (property.CanRead)
                {
                    writer.WritePropertyName(property.Name);
                    serializer.Serialize(writer, property.GetValue(value));
                }
            }
            writer.WriteEndObject();
        }

        public override ModifierBase ReadJson(JsonReader reader, Type objectType, ModifierBase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var typeName = jsonObject["Type"]?.ToString();

            switch (typeName)
            {
                case "Transformation":
                    return jsonObject.ToObject<Transformation>(serializer);
                case "Comparison":
                    return jsonObject.ToObject<Comparison>(serializer);
                case "Interpolation":
                    return jsonObject.ToObject<Interpolation>(serializer);
                case "Blink":
                    return jsonObject.ToObject<Blink>(serializer);
                case "Padding":
                    return jsonObject.ToObject<Padding>(serializer);
                case "Substring":
                    return jsonObject.ToObject<Substring>(serializer);
                default:
                    throw new NotSupportedException($"Modifier type '{typeName}' is not supported.");
            }
        }
    }
}
