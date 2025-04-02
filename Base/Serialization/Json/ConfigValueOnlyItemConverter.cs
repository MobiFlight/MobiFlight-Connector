using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


namespace MobiFlight.Base.Serialization.Json
{
    public class ConfigValueOnlyItemConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IConfigValueOnlyItem).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var configValueOnlyItem = value as IConfigValueOnlyItem;
            if (configValueOnlyItem == null)
            {
                writer.WriteNull();
                return;
            }

            JObject obj = new JObject();

            if (configValueOnlyItem.GUID != null)
            {
                obj.Add(nameof(IConfigValueOnlyItem.GUID), JToken.FromObject(configValueOnlyItem.GUID));
            }

            if (configValueOnlyItem.RawValue != null)
            {
                obj.Add(nameof(IConfigValueOnlyItem.RawValue), JToken.FromObject(configValueOnlyItem.RawValue));
            }

            if (configValueOnlyItem.Value != null)
            {
                obj.Add(nameof(IConfigValueOnlyItem.Value), JToken.FromObject(configValueOnlyItem.Value));
            }

            obj.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Deserialization is not implemented for this converter.");
        }
    }
}
