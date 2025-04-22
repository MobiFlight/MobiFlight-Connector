using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandActiveConfigFile
    {
        [JsonProperty("type")] // Matches the lowercase "item" in JSON
        public string Type { get; set; }
        public int index { get; set; }
    }
}
