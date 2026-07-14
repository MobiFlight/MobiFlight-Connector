namespace MobiFlight.Base.Serialization.Json.Resolver
{
    using MobiFlight.Base.Serialization.Json.Annotations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;

    public class CustomJsonIgnoreContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            foreach (var property in properties)
            {
                if (property.AttributeProvider?.GetAttributes(typeof(JsonIgnoreWhenPersistedToFileAttribute), true).Count > 0)
                {
                    property.Ignored = true;
                }
            }

            return properties;
        }
    }
}