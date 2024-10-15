using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MobiFlight.Modifier.Converter
{
    public class ModifierBaseConverter : JsonConverter
    {
        private static readonly Dictionary<string, Type> ModifierTypes = new Dictionary<string, Type>
        {
            { "Padding", typeof(Padding) },
            { "Substring", typeof(Substring) },
            { "Blink", typeof(Blink) },
            { "Transformation", typeof(Transformation) },
            { "Interpolation", typeof(Interpolation) },        
            // Add other specific modifiers here
        };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ModifierBase[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var modifiers = new List<ModifierBase>();
            var array = JArray.Load(reader);

            foreach (var item in array)
            {
                var type = item["Type"]?.ToString();
                if (type != null && ModifierTypes.TryGetValue(type, out var modifierType))
                {
                    var modifier = (ModifierBase)item.ToObject(modifierType, serializer);
                    modifiers.Add(modifier);
                }
            }

            return modifiers.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var modifiers = (ModifierBase[])value;
            writer.WriteStartArray();

            foreach (var modifier in modifiers)
            {
                serializer.Serialize(writer, modifier, modifier.GetType());
            }

            writer.WriteEndArray();
        }
    }
}

