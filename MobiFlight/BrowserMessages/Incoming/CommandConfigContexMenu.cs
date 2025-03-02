using MobiFlight.Base;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandConfigContextMenu
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("item")] // Matches the lowercase "item" in JSON
        public ConfigItem Item { get; set; }
    }
}
