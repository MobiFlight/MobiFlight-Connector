using System;

namespace MobiFlight.Base.Serialization.Json.Annotations
{
    // Attribute you can put on any DTO property
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class JsonIgnoreWhenPersistedToFileAttribute : Attribute { }
}