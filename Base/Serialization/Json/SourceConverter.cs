using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace MobiFlight.Base.Serialization.Json
{
    public class SourceConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Source).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var typeName = $"MobiFlight.Base.{(string)jsonObject["Type"]}";

            var type = Type.GetType(typeName);
            if (type == null)
            {
                throw new NotSupportedException($"Unknown type: {typeName}");
            }

            var source = Activator.CreateInstance(type) as Source;
            serializer.Populate(jsonObject.CreateReader(), source);
            return source;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Use default serialization
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
