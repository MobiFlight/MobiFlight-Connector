using System;
using MobiFlight.BrowserMessages.Incoming;

namespace MobiFlight.BrowserMessages.Incoming.Handler
{
    public class CommandUpdateSettingsHandler
    {
        public void Handle(CommandUpdateSettings message)
        {
            if (message == null) return;

            // General Settings
            Properties.Settings.Default.RecentFilesMaxCount = message.RecentFilesMaxCount;
            Properties.Settings.Default.LogEnabled = message.LogEnabled;
            Properties.Settings.Default.LogLevel = message.LogLevel;
            Properties.Settings.Default.LogJoystickAxis = message.LogJoystickAxis;
            Properties.Settings.Default.BetaUpdates = message.BetaUpdates;
            Properties.Settings.Default.CommunityFeedback = message.CommunityFeedback;
            Properties.Settings.Default.AutoRetrigger = message.AutoRetrigger;
            Properties.Settings.Default.MinimizeOnAutoRun = message.MinimizeOnAutoRun;
            Properties.Settings.Default.HubHopAutoCheck = message.HubHopAutoCheck;
            Properties.Settings.Default.Language = message.Language;
            if (message.PollInterval > 0)
            {
                Properties.Settings.Default.PollInterval = Math.Max(25, message.PollInterval);
            }
            if (message.TestTimerInterval > 0)
            {
                Properties.Settings.Default.TestTimerInterval = Math.Max(50, message.TestTimerInterval);
            }

            // ProSim Settings
            Properties.Settings.Default.ProSimHost = message.ProSimHost;
            Properties.Settings.Default.ProSimPort = message.ProSimPort;
            Properties.Settings.Default.ProSimAutoConnectEnabled = message.ProSimAutoConnectEnabled;
            Properties.Settings.Default.ProSimMaxRetryAttempts = message.ProSimMaxRetryAttempts;

            // Apply live log settings immediately
            Log.Instance.LogJoystickAxis = message.LogJoystickAxis;
            if (!string.IsNullOrEmpty(message.LogLevel))
            {
                try
                {
                    Log.Instance.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), message.LogLevel, true);
                }
                catch
                {
                    Log.Instance.Severity = LogSeverity.Info;
                }
            }

            // Save to user.config
            Properties.Settings.Default.Save();

            Log.Instance.log("Settings updated and saved successfully.", LogSeverity.Info);
        }
    }
}

