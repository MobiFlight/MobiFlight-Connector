using MobiFlight.Base;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    public class CommandUpdateSettings
    {
        [JsonProperty("Settings")]
        public Settings Settings { get; set; }
    }
}
