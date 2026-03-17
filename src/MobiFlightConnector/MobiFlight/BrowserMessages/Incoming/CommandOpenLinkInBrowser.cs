using MobiFlight.Base;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandOpenLinkInBrowser
    {
        [JsonProperty("url")] // Matches the lowercase "url" in JSON
        public string Url { get; set; }
    }
}
