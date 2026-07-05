using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}