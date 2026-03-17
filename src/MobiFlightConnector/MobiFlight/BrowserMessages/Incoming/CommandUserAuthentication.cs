using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    public enum CommandUserAuthenticationFlow
    {
        login,
        logout
    }

    public enum CommandUserAuthenticationState
    {
        started,
        success,
        cancelled,
        error
    }

    public class CommandUserAuthentication
    {
        [JsonProperty("flow")]
        public CommandUserAuthenticationFlow Flow { get; set; }

        public CommandUserAuthenticationState State { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
