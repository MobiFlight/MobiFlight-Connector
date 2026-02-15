using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandUserAuthenticationAction
    {
        login,
        logout,
        successful,
        aborted
    }
    public class CommandUserAuthentication
    {
        [JsonProperty("action")]
        public CommandUserAuthenticationAction Action { get; set; }
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
