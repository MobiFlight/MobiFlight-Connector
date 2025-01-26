using MobiFlight.Base;
using MobiFlight.BrowserMessages.Incoming.Converter;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class ConfigEdit
    {
        [JsonProperty("item")] // Matches the lowercase "item" in JSON
        public ConfigItem Item { get; set; }
    }
}
