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
            var typeName = $"MobiFlight.Modifier.{jsonObject["Type"]?.ToString()}";

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new JsonSerializationException($"Unknown type: {typeName}");
            }

            var modifier = Activator.CreateInstance(type) as ModifierBase;
            serializer.Populate(jsonObject.CreateReader(), modifier);
            return modifier;
        }
    }
}
