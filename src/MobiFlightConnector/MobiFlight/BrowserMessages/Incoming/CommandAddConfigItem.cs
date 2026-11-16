using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    internal class CommandAddConfigItem
    {
        [JsonProperty("type")] // Matches the lowercase "item" in JSON
        public string Type { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// GUID of the config item the new config item is inserted after.
        /// The new config item is added at the end of the list if not set.
        /// </summary>
        public string InsertAfterGuid { get; set; }
    }
}
