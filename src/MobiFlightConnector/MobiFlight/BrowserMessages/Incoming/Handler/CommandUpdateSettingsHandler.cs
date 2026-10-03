using System;
using MobiFlight.Base;
using MobiFlight.BrowserMessages.Incoming;

namespace MobiFlight.BrowserMessages.Incoming.Handler
{
    public class CommandUpdateSettingsHandler
    {
        private readonly Func<ExecutionManager> _getExecManager;

        public CommandUpdateSettingsHandler(Func<ExecutionManager> getExecManager = null)
        {
            _getExecManager = getExecManager;
        }

        public CommandUpdateSettingsHandler(ExecutionManager execManager)
        {
            if (execManager != null)
            {
                _getExecManager = () => execManager;
            }
        }

        public void Handle(CommandUpdateSettings message)
        {
            if (message?.Settings == null) return;

            var oldProSimHost = Properties.Settings.Default.ProSimHost;
            var oldProSimPort = Properties.Settings.Default.ProSimPort;
            var oldProSimAutoConnect = Properties.Settings.Default.ProSimAutoConnectEnabled;

            // Apply all settings to Properties.Settings.Default
            message.Settings.ApplyTo(Properties.Settings.Default);

            // Apply live log settings immediately
            Log.Instance.LogJoystickAxis = Properties.Settings.Default.LogJoystickAxis;
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LogLevel))
            {
                try
                {
                    Log.Instance.Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), Properties.Settings.Default.LogLevel, true);
                }
                catch
                {
                    Log.Instance.Severity = LogSeverity.Info;
                }
            }

            // Save to user.config
            Properties.Settings.Default.Save();

            // Reset ProSim connection state if ProSim connection settings changed
            var execManager = _getExecManager?.Invoke();
            if (execManager != null &&
                (oldProSimHost != Properties.Settings.Default.ProSimHost ||
                 oldProSimPort != Properties.Settings.Default.ProSimPort ||
                 oldProSimAutoConnect != Properties.Settings.Default.ProSimAutoConnectEnabled))
            {
                try
                {
                    execManager.ResetProSimConnectionState();
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Failed to reset ProSim connection state: {ex.Message}", LogSeverity.Warn);
                }
            }

            Log.Instance.log("Settings updated and saved successfully.", LogSeverity.Info);
        }
    }
}
