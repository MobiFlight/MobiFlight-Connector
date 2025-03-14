using System.Linq;

namespace MobiFlight.Base
{
    public class Settings
    {
        public bool ArcazeSupportEnabled { get; set; }
        public bool AutoRetrigger { get; set; }
        public bool AutoRun { get; set; }
        public bool AutoLoadLinkedConfig { get; set; }
        public bool BetaUpdates { get; set; }
        public bool CommunityFeedback { get; set; }
        public bool EnableJoystickSupport { get; set; }
        public bool EnableMidiSupport { get; set; }
        public string ExcludedJoysticks { get; set; }
        public string ExcludedMidiBoards { get; set; }
        public bool FwAutoUpdateCheck { get; set; }
        public bool HubHopAutoCheck { get; set; }
        public string IgnoredComPortsList { get; set; }
        public string Language { get; set; }
        public bool LogEnabled { get; set; }
        public bool LogJoystickAxis { get; set; }
        public string LogLevel { get; set; }
        public bool MinimizeOnAutoRun { get; set; }
        public string ModuleSettings { get; set; }
        public string[] RecentFiles { get; set; }
        public int RecentFilesMaxCount { get; set; }
        public int TestTimerInterval { get; set; }

        internal Settings()
        {
        }

        internal Settings(Properties.Settings settings)
        {
            ArcazeSupportEnabled = settings.ArcazeSupportEnabled;
            AutoRetrigger = settings.AutoRetrigger;
            AutoRun = settings.AutoRun;
            AutoLoadLinkedConfig = settings.AutoLoadLinkedConfig;
            BetaUpdates = settings.BetaUpdates;
            CommunityFeedback = settings.CommunityFeedback;
            EnableJoystickSupport = settings.EnableJoystickSupport;
            EnableMidiSupport = settings.EnableMidiSupport;
            ExcludedJoysticks = settings.ExcludedJoysticks;
            ExcludedMidiBoards = settings.ExcludedMidiBoards;
            FwAutoUpdateCheck = settings.FwAutoUpdateCheck;
            HubHopAutoCheck = settings.HubHopAutoCheck;
            IgnoredComPortsList = settings.IgnoredComPortsList;
            Language = settings.Language;
            LogEnabled = settings.LogEnabled;
            LogJoystickAxis = settings.LogJoystickAxis;
            LogLevel = settings.LogLevel;
            MinimizeOnAutoRun = settings.MinimizeOnAutoRun;
            ModuleSettings = settings.ModuleSettings;
            // Skip: OfflineMode = settings.OfflineMode;
            // Skip: PollInterval = settings.PollInterval;
            RecentFiles = settings.RecentFiles.Cast<string>().ToArray();
            RecentFilesMaxCount = settings.RecentFilesMaxCount;
            TestTimerInterval = settings.TestTimerInterval;
            // Properties.Settings.Default.AutoRetrigger = true;
            // Properties.Settings.Default.AutoRun = true;
            // Properties.Settings.Default.AutoLoadLinkedConfig = true;
            // Properties.Settings.Default.BetaUpdates = true;
            // Properties.Settings.Default.CommunityFeedback = true;
            // Properties.Settings.Default.EnableJoystickSupport = true;
            // Properties.Settings.Default.EnableMidiSupport = true;
            // Properties.Settings.Default.ExcludedJoysticks
            // Properties.Settings.Default.ExcludedMidiBoards
            // Properties.Settings.Default.FwAutoUpdateCheck = true;
            // Properties.Settings.Default.HubHopAutoCheck = true;
            // Properties.Settings.Default.IgnoredComPortsList
            // Properties.Settings.Default.Language = "en";
            // Properties.Settings.Default.LogEnabled = true;
            // Properties.Settings.Default.LogJoystickAxis = false;
            // Properties.Settings.Default.LogLevel = "Debug";
            // Properties.Settings.Default.MinimizeOnAutoRun = true;
            // Properties.Settings.Default.ModuleSettings = true;
            // Skip: Properties.Settings.Default.OfflineMode = false;
            // Skip: Properties.Settings.Default.PollInterval = 100;
            // Properties.Settings.Default.RecentFiles = new System.Collections.Specialized.StringCollection();
            // Properties.Settings.Default.RecentFilesMaxCount = 10;
            // Skip: Properties.Settings.Default.TestTimerInterval = 1000;
        }


    }
}
