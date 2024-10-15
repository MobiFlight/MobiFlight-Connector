using MobiFlight.Modifier;
using MobiFlight.Modifier.Converter;
using Newtonsoft.Json;

namespace MobiFlight.Frontend
{
    public class ConfigItem : IConfigItem
    {
        public string GUID { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public string Device { get; set; }
        public string Component { get; set; }
        public string Type { get; set; }
        public string[] Tags { get; set; }
        public string[] Status { get; set; }
        public string RawValue { get; set; }
        public string ModifiedValue { get; set; }
        public ConfigContext Context { get; set; }

        [JsonConverter(typeof(ModifierBaseConverter))]
        public ModifierBase[] Modifiers { get; set; }
        public ConfigEvent Event { get; set; }
        public ConfigAction Action { get; set; }
    }
}
