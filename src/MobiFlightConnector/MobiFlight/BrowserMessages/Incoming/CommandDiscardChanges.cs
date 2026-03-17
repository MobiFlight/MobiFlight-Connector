using MobiFlight.Base;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandDiscardChanges
    {
        [JsonProperty("project")] // Matches the lowercase "item" in JSON
        public Project Project { get; set; }
    }
}
