using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace MobiFlight.Base.Serialization.Json
{
    public class PreconditionConverter : JsonConverter
    {
        /// <summary>
        /// A custom JsonConverter for Precondition objects that skips serialization of empty preconditions
        /// (where Type="none" and key fields are null). This prevents saving null or empty properties to
        /// project files, improving file cleanliness and maintainability.
        /// </summary>
        public override bool CanRead => false;
        public override bool CanWrite => true;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var p = value as Precondition;
            if (IsNullOrEmpty(p))
            {
                return;
            }

            // Create a resolver that ignores the JsonConverter attribute on Precondition
            var resolver = new IgnoreAttributeContractResolver();
            var tmp = new JsonSerializer
            {
                ContractResolver = resolver,
                Culture = serializer.Culture,
                Formatting = serializer.Formatting,
                NullValueHandling = serializer.NullValueHandling,
                DefaultValueHandling = serializer.DefaultValueHandling,
                TypeNameHandling = serializer.TypeNameHandling
            };

            foreach (var c in serializer.Converters)
            {
                if (c.GetType() != this.GetType())
                    tmp.Converters.Add(c);
            }

            tmp.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException("ReadJson is disabled via CanRead = false.");
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Precondition).IsAssignableFrom(objectType);
        }

        public static bool IsNullOrEmpty(Precondition p)
        {
            return p == null || p.IsEmpty();
        }

        private class IgnoreAttributeContractResolver : DefaultContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                var contract = base.CreateContract(objectType);
                
                // Remove converter from Precondition to avoid recursion
                if (objectType == typeof(Precondition))
                {
                    contract.Converter = null;
                }
                
                return contract;
            }
        }
    }
}