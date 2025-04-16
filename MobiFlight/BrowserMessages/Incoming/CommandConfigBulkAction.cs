using MobiFlight.Base;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Incoming
{
    public class CommandConfigBulkAction
    {
        [JsonProperty("action")]
        public string Action { get; set; }
        [JsonProperty("items")] // Matches the lowercase "item" in JSON
        public List<ConfigItem> Items { get; set; }
    }
}
