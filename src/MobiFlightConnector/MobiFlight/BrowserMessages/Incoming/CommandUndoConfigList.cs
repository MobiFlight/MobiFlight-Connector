using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandUndoConfigList
    {
        [JsonProperty("steps")]
        public int Steps { get; set; } = 1;
    }
}
