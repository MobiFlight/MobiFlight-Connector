using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandAddConfigItem
    {
        [JsonProperty("type")] // Matches the lowercase "item" in JSON
        public string Type { get; set; }
        public string Name { get; set; }
    }
}
