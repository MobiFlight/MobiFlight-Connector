using MobiFlight.InputConfig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Windows.Forms;

namespace MobiFlight.Base.Serialization.Json
{
    internal class KeyInputActionConverter : JsonConverter
    {
        public override bool CanWrite => false;
        public override bool CanConvert(Type objectType)
        {
            return typeof(InputAction).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Never called because CanWrite = false
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);
            var deprecatedKey = jsonObject["Key"]?.ToString();

            if (deprecatedKey != null)
            {
                try
                {
                    var restoredKey = new KeysConverter().ConvertFromString(deprecatedKey);
                    var code = restoredKey.ToString();
                    jsonObject["Code"] = code;
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Failed to convert deprecated keyboard key '{deprecatedKey}' to Keys: {ex.Message}", LogSeverity.Error);
                }
            }

            var action = Activator.CreateInstance(typeof(KeyInputAction)) as InputAction;
            serializer.Populate(jsonObject.CreateReader(), action);
            return action;
        }
    }
}