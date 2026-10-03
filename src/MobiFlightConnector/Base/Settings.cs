using System;
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
        public LogSeverity LogLevel { get; set; }
        public bool MinimizeOnAutoRun { get; set; }
        public string ModuleSettings { get; set; }
        public string[] RecentFiles { get; set; }
        public int RecentFilesMaxCount { get; set; }
        public int TestTimerInterval { get; set; }
        public int PollInterval { get; set; }
        public string ProSimHost { get; set; }
        public int ProSimPort { get; set; }
        public bool ProSimAutoConnectEnabled { get; set; }
        public int ProSimMaxRetryAttempts { get; set; }

        public Settings()
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

            try
            {
                LogLevel = (LogSeverity)Enum.Parse(typeof(LogSeverity), settings.LogLevel, true);
            }
            catch (Exception)
            {
                LogLevel = LogSeverity.Debug;
            }

            MinimizeOnAutoRun = settings.MinimizeOnAutoRun;
            ModuleSettings = settings.ModuleSettings;
            PollInterval = settings.PollInterval;
            RecentFiles = settings.RecentFiles?.Cast<string>().ToArray() ?? new string[0];
            RecentFilesMaxCount = settings.RecentFilesMaxCount;
            TestTimerInterval = settings.TestTimerInterval;

            ProSimHost = settings.ProSimHost;
            ProSimPort = settings.ProSimPort;
            ProSimAutoConnectEnabled = settings.ProSimAutoConnectEnabled;
            ProSimMaxRetryAttempts = settings.ProSimMaxRetryAttempts;
        }

        internal void ApplyTo(Properties.Settings settings)
        {
            if (settings == null) return;

            settings.ArcazeSupportEnabled = ArcazeSupportEnabled;
            settings.AutoRetrigger = AutoRetrigger;
            settings.AutoRun = AutoRun;
            settings.AutoLoadLinkedConfig = AutoLoadLinkedConfig;
            settings.BetaUpdates = BetaUpdates;
            settings.CommunityFeedback = CommunityFeedback;
            settings.EnableJoystickSupport = EnableJoystickSupport;
            settings.EnableMidiSupport = EnableMidiSupport;
            settings.ExcludedJoysticks = ExcludedJoysticks;
            settings.ExcludedMidiBoards = ExcludedMidiBoards;
            settings.FwAutoUpdateCheck = FwAutoUpdateCheck;
            settings.HubHopAutoCheck = HubHopAutoCheck;
            settings.IgnoredComPortsList = IgnoredComPortsList;
            settings.Language = Language ?? "";
            settings.LogEnabled = LogEnabled;
            settings.LogJoystickAxis = LogJoystickAxis;
            settings.LogLevel = LogLevel.ToString();
            settings.MinimizeOnAutoRun = MinimizeOnAutoRun;
            settings.ModuleSettings = ModuleSettings;

            if (PollInterval > 0)
            {
                settings.PollInterval = Math.Max(25, PollInterval);
            }

            if (RecentFilesMaxCount > 0)
            {
                settings.RecentFilesMaxCount = RecentFilesMaxCount;
            }

            if (TestTimerInterval > 0)
            {
                settings.TestTimerInterval = Math.Max(50, TestTimerInterval);
            }

            if (ProSimHost != null)
            {
                settings.ProSimHost = ProSimHost;
            }

            if (ProSimPort > 0)
            {
                settings.ProSimPort = ProSimPort;
            }

            settings.ProSimAutoConnectEnabled = ProSimAutoConnectEnabled;

            if (ProSimMaxRetryAttempts > 0)
            {
                settings.ProSimMaxRetryAttempts = ProSimMaxRetryAttempts;
            }
        }
    }
}