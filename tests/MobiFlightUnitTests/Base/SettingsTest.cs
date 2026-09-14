using Microsoft.VisualStudio.TestTools.UnitTesting;
using MobiFlight.BrowserMessages.Incoming;
using MobiFlight.BrowserMessages.Incoming.Handler;

namespace MobiFlight.Base.Tests
{
    [TestClass]
    public class SettingsTest
    {
        [TestMethod]
        public void Settings_LogLevel_InvalidValue_ShouldDefaultToDebug()
        {
            // Arrange
            var settings = new Properties.Settings
            {
                LogLevel = "InvalidLogLevel"
            };
            // Act
            var mobiflightSettings = new Settings(settings);
            // Assert
            Assert.AreEqual(LogSeverity.Debug, mobiflightSettings.LogLevel);
        }

        [TestMethod]
        public void Settings_Constructor_ShouldPopulateAllFieldsFromPropertiesSettings()
        {
            // Arrange
            var properties = new Properties.Settings
            {
                ArcazeSupportEnabled = true,
                AutoRetrigger = true,
                AutoRun = true,
                AutoLoadLinkedConfig = true,
                BetaUpdates = true,
                CommunityFeedback = true,
                EnableJoystickSupport = true,
                EnableMidiSupport = true,
                ExcludedJoysticks = "Joy1",
                ExcludedMidiBoards = "Midi1",
                FwAutoUpdateCheck = true,
                HubHopAutoCheck = true,
                IgnoredComPortsList = "COM1,COM2",
                Language = "de-DE",
                LogEnabled = true,
                LogJoystickAxis = true,
                LogLevel = "Info",
                MinimizeOnAutoRun = true,
                ModuleSettings = "custom",
                PollInterval = 75,
                RecentFilesMaxCount = 15,
                TestTimerInterval = 250,
                ProSimHost = "192.168.1.100",
                ProSimPort = 9000,
                ProSimAutoConnectEnabled = true,
                ProSimMaxRetryAttempts = 10
            };

            // Act
            var settings = new Settings(properties);

            // Assert
            Assert.AreEqual(true, settings.ArcazeSupportEnabled);
            Assert.AreEqual(true, settings.AutoRetrigger);
            Assert.AreEqual(true, settings.AutoRun);
            Assert.AreEqual(true, settings.AutoLoadLinkedConfig);
            Assert.AreEqual(true, settings.BetaUpdates);
            Assert.AreEqual(true, settings.CommunityFeedback);
            Assert.AreEqual(true, settings.EnableJoystickSupport);
            Assert.AreEqual(true, settings.EnableMidiSupport);
            Assert.AreEqual("Joy1", settings.ExcludedJoysticks);
            Assert.AreEqual("Midi1", settings.ExcludedMidiBoards);
            Assert.AreEqual(true, settings.FwAutoUpdateCheck);
            Assert.AreEqual(true, settings.HubHopAutoCheck);
            Assert.AreEqual("COM1,COM2", settings.IgnoredComPortsList);
            Assert.AreEqual("de-DE", settings.Language);
            Assert.AreEqual(true, settings.LogEnabled);
            Assert.AreEqual(true, settings.LogJoystickAxis);
            Assert.AreEqual(LogSeverity.Info, settings.LogLevel);
            Assert.AreEqual(true, settings.MinimizeOnAutoRun);
            Assert.AreEqual("custom", settings.ModuleSettings);
            Assert.AreEqual(75, settings.PollInterval);
            Assert.AreEqual(15, settings.RecentFilesMaxCount);
            Assert.AreEqual(250, settings.TestTimerInterval);
            Assert.AreEqual("192.168.1.100", settings.ProSimHost);
            Assert.AreEqual(9000, settings.ProSimPort);
            Assert.AreEqual(true, settings.ProSimAutoConnectEnabled);
            Assert.AreEqual(10, settings.ProSimMaxRetryAttempts);
        }

        [TestMethod]
        public void Settings_ApplyTo_ShouldUpdatePropertiesSettingsCorrectly()
        {
            // Arrange
            var properties = new Properties.Settings();
            var settings = new Settings
            {
                ArcazeSupportEnabled = true,
                AutoRetrigger = true,
                AutoRun = true,
                AutoLoadLinkedConfig = true,
                BetaUpdates = true,
                CommunityFeedback = true,
                EnableJoystickSupport = true,
                EnableMidiSupport = true,
                ExcludedJoysticks = "JoyA",
                ExcludedMidiBoards = "MidiA",
                FwAutoUpdateCheck = true,
                HubHopAutoCheck = true,
                IgnoredComPortsList = "COM3",
                Language = "fr-FR",
                LogEnabled = true,
                LogJoystickAxis = true,
                LogLevel = LogSeverity.Warn,
                MinimizeOnAutoRun = true,
                ModuleSettings = "mod1",
                PollInterval = 100,
                RecentFilesMaxCount = 8,
                TestTimerInterval = 500,
                ProSimHost = "10.0.0.5",
                ProSimPort = 8085,
                ProSimAutoConnectEnabled = false,
                ProSimMaxRetryAttempts = 7
            };

            // Act
            settings.ApplyTo(properties);

            // Assert
            Assert.AreEqual(true, properties.ArcazeSupportEnabled);
            Assert.AreEqual(true, properties.AutoRetrigger);
            Assert.AreEqual(true, properties.AutoRun);
            Assert.AreEqual(true, properties.AutoLoadLinkedConfig);
            Assert.AreEqual(true, properties.BetaUpdates);
            Assert.AreEqual(true, properties.CommunityFeedback);
            Assert.AreEqual(true, properties.EnableJoystickSupport);
            Assert.AreEqual(true, properties.EnableMidiSupport);
            Assert.AreEqual("JoyA", properties.ExcludedJoysticks);
            Assert.AreEqual("MidiA", properties.ExcludedMidiBoards);
            Assert.AreEqual(true, properties.FwAutoUpdateCheck);
            Assert.AreEqual(true, properties.HubHopAutoCheck);
            Assert.AreEqual("COM3", properties.IgnoredComPortsList);
            Assert.AreEqual("fr-FR", properties.Language);
            Assert.AreEqual(true, properties.LogEnabled);
            Assert.AreEqual(true, properties.LogJoystickAxis);
            Assert.AreEqual("Warn", properties.LogLevel);
            Assert.AreEqual(true, properties.MinimizeOnAutoRun);
            Assert.AreEqual("mod1", properties.ModuleSettings);
            Assert.AreEqual(100, properties.PollInterval);
            Assert.AreEqual(8, properties.RecentFilesMaxCount);
            Assert.AreEqual(500, properties.TestTimerInterval);
            Assert.AreEqual("10.0.0.5", properties.ProSimHost);
            Assert.AreEqual(8085, properties.ProSimPort);
            Assert.AreEqual(false, properties.ProSimAutoConnectEnabled);
            Assert.AreEqual(7, properties.ProSimMaxRetryAttempts);
        }

        [TestMethod]
        public void CommandUpdateSettingsHandler_Handle_NullMessageOrSettings_ShouldNotThrow()
        {
            // Arrange
            var handler = new CommandUpdateSettingsHandler();

            // Act & Assert
            handler.Handle(null);
            handler.Handle(new CommandUpdateSettings { Settings = null });
        }
    }
}