using MobiFlight.Base;
using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    /// <summary>
    /// Sent by the config wizard with the current draft of an LCD config item.
    /// The backend answers with an <see cref="Outgoing.LcdDisplayPreview"/>
    /// that contains the text exactly as the display would show it.
    /// </summary>
    internal class CommandLcdDisplayPreview
    {
        [JsonProperty("item")]
        public ConfigItem Item { get; set; }
    }
}
