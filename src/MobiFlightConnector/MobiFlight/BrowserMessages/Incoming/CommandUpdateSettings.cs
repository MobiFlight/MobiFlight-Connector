using Newtonsoft.Json;

namespace MobiFlight.BrowserMessages.Incoming
{
    public class CommandUpdateSettings
    {
        // General Settings
        [JsonProperty]
        public int RecentFilesMaxCount { get; set; }

        [JsonProperty]
        public bool LogEnabled { get; set; }

        [JsonProperty]
        public string LogLevel { get; set; }

        [JsonProperty]
        public bool LogJoystickAxis { get; set; }

        [JsonProperty]
        public bool BetaUpdates { get; set; }

        [JsonProperty]
        public bool CommunityFeedback { get; set; }

        [JsonProperty]
        public bool AutoRetrigger { get; set; }

        [JsonProperty]
        public bool MinimizeOnAutoRun { get; set; }

        [JsonProperty]
        public bool HubHopAutoCheck { get; set; }

        [JsonProperty]
        public string Language { get; set; }

        [JsonProperty]
        public int PollInterval { get; set; }

        [JsonProperty]
        public int TestTimerInterval { get; set; }

        // ProSim Settings
        [JsonProperty]
        public string ProSimHost { get; set; }

        [JsonProperty]
        public int ProSimPort { get; set; }

        [JsonProperty]
        public bool ProSimAutoConnectEnabled { get; set; }

        [JsonProperty]
        public int ProSimMaxRetryAttempts { get; set; }
    }
}

