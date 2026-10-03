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
            Assert.IsTrue(settings.ArcazeSupportEnabled);
            Assert.IsTrue(settings.AutoRetrigger);

            Assert.IsTrue(settings.AutoRun);
            Assert.IsTrue(settings.AutoLoadLinkedConfig);
            Assert.IsTrue(settings.BetaUpdates);
            Assert.IsTrue(settings.CommunityFeedback);
            Assert.IsTrue(settings.EnableJoystickSupport);
            Assert.IsTrue(settings.EnableMidiSupport);
            Assert.AreEqual("Joy1", settings.ExcludedJoysticks);
            Assert.AreEqual("Midi1", settings.ExcludedMidiBoards);
            Assert.IsTrue(settings.FwAutoUpdateCheck);
            Assert.IsTrue(settings.HubHopAutoCheck);
            Assert.AreEqual("COM1,COM2", settings.IgnoredComPortsList);
            Assert.AreEqual("de-DE", settings.Language);
            Assert.IsTrue(settings.LogEnabled);
            Assert.IsTrue(settings.LogJoystickAxis);
            Assert.AreEqual(LogSeverity.Info, settings.LogLevel);
            Assert.IsTrue(settings.MinimizeOnAutoRun);
            Assert.AreEqual("custom", settings.ModuleSettings);
            Assert.AreEqual(75, settings.PollInterval);
            Assert.AreEqual(15, settings.RecentFilesMaxCount);
            Assert.AreEqual(250, settings.TestTimerInterval);
            Assert.AreEqual("192.168.1.100", settings.ProSimHost);
            Assert.AreEqual(9000, settings.ProSimPort);
            Assert.IsTrue(settings.ProSimAutoConnectEnabled);
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
            Assert.IsTrue(properties.ArcazeSupportEnabled);
            Assert.IsTrue(properties.AutoRetrigger);
            Assert.IsTrue(properties.AutoRun);
            Assert.IsTrue(properties.AutoLoadLinkedConfig);
            Assert.IsTrue(properties.BetaUpdates);
            Assert.IsTrue(properties.CommunityFeedback);
            Assert.IsTrue(properties.EnableJoystickSupport);
            Assert.IsTrue(properties.EnableMidiSupport);
            Assert.AreEqual("JoyA", properties.ExcludedJoysticks);
            Assert.AreEqual("MidiA", properties.ExcludedMidiBoards);
            Assert.IsTrue(properties.FwAutoUpdateCheck);
            Assert.IsTrue(properties.HubHopAutoCheck);
            Assert.AreEqual("COM3", properties.IgnoredComPortsList);
            Assert.AreEqual("fr-FR", properties.Language);
            Assert.IsTrue(properties.LogEnabled);
            Assert.IsTrue(properties.LogJoystickAxis);
            Assert.AreEqual("Warn", properties.LogLevel);
            Assert.IsTrue(properties.MinimizeOnAutoRun);
            Assert.AreEqual("mod1", properties.ModuleSettings);
            Assert.AreEqual(100, properties.PollInterval);
            Assert.AreEqual(8, properties.RecentFilesMaxCount);
            Assert.AreEqual(500, properties.TestTimerInterval);
            Assert.AreEqual("10.0.0.5", properties.ProSimHost);
            Assert.AreEqual(8085, properties.ProSimPort);
            Assert.IsFalse(properties.ProSimAutoConnectEnabled);
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