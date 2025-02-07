using MobiFlight.Base;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandUpdateConfigItem
    {
        [JsonProperty("item")] // Matches the lowercase "item" in JSON
        public ConfigItem Item { get; set; }
    }
}
