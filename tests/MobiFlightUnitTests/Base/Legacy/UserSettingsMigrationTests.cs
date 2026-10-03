namespace MobiFlight.Base.Legacy.Tests
{
    [TestClass]
    public class UserSettingsMigrationTests
    {
        private static readonly Version DefaultCurrentVersion = new Version(11, 0, 0, 0);
        private string _testDirectory;
        [TestInitialize]
        public void Setup()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "UserSettingsMigrationTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);
        }
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_AlreadyMigrated_DoesNothing()
        {
            // Arrange: reach an already-migrated state via a real migration, then change state
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.5.2.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>75</value>
                </setting>");
            var settings = CreateFreshSettings();
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            settings.PollInterval = 123;
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.5.2.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>999</value>
                </setting>");
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.AreEqual(123, settings.PollInterval);
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_NoLegacyInstallFound_MarksMigratedWithoutChangingValues()
        {
            // Arrange
            var settings = CreateFreshSettings();
            var defaultPollInterval = settings.PollInterval;
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.IsTrue(settings.LegacySettingsMigrated);
            Assert.AreEqual(defaultPollInterval, settings.PollInterval);
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_RealisticLegacyConfig_MigratesKnownSettings()
        {
            // Arrange
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.5.2.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>75</value>
                </setting>
                <setting name=""BetaUpdates"" serializeAs=""String"">
                    <value>True</value>
                </setting>
                <setting name=""CacheId"" serializeAs=""String"">
                    <value>d290f1ee-6c54-4b01-90e6-d701748f0851</value>
                </setting>
                <setting name=""Language"" serializeAs=""String"">
                    <value>de-DE</value>
                </setting>
                <setting name=""LedIntensity"" serializeAs=""String"">
                    <value>128</value>
                </setting>");
            var settings = CreateFreshSettings();
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.IsTrue(settings.LegacySettingsMigrated);
            Assert.AreEqual(75, settings.PollInterval);
            Assert.IsTrue(settings.BetaUpdates);
            Assert.AreEqual("d290f1ee-6c54-4b01-90e6-d701748f0851", settings.CacheId);
            Assert.AreEqual("de-DE", settings.Language);
            Assert.AreEqual((byte)128, settings.LedIntensity);
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_UnknownSetting_IsSkippedWithoutFailingMigration()
        {
            // Arrange: "NoLongerExists" simulates a setting removed since the legacy version
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.5.2.0", @"
                <setting name=""NoLongerExists"" serializeAs=""String"">
                    <value>SomeOldValue</value>
                </setting>
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>90</value>
                </setting>");
            var settings = CreateFreshSettings();
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.IsTrue(settings.LegacySettingsMigrated);
            Assert.AreEqual(90, settings.PollInterval);
        }

        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_XmlSerializedRecentFiles_MigratesCorrectly()
        {
            // Arrange: RecentFiles is a StringCollection, serialized as Xml (not a plain string value)
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.5.2.0", @"
                <setting name=""RecentFiles"" serializeAs=""Xml"">
                    <value>
                        <ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                            <string>C:\configs\one.mcc</string>
                            <string>C:\configs\two.mcc</string>
                        </ArrayOfString>
                    </value>
                </setting>");
            var settings = CreateFreshSettings();
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.HasCount(2, settings.RecentFiles);
            Assert.AreEqual(@"C:\configs\one.mcc", settings.RecentFiles[0]);
            Assert.AreEqual(@"C:\configs\two.mcc", settings.RecentFiles[1]);
        }

        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_LinkedConfigSettings_AreRestored()
        {
            // Arrange: the auto-load-linked-config feature and its per-aircraft config mapping
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.5.2.0", @"
                <setting name=""AutoLoadLinkedConfig"" serializeAs=""String"">
                    <value>True</value>
                </setting>
                <setting name=""AutoLoadLinkedConfigList"" serializeAs=""String"">
                    <value>{&quot;PMDG 737&quot;:&quot;C:\\configs\\737.mcc&quot;}</value>
                </setting>");
            var settings = CreateFreshSettings();
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.IsTrue(settings.AutoLoadLinkedConfig);
            Assert.AreEqual(@"{""PMDG 737"":""C:\\configs\\737.mcc""}", settings.AutoLoadLinkedConfigList);
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_MultipleVersionFolders_UsesHighestVersion()
        {
            // Arrange
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.0.0.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>10</value>
                </setting>");
            CreateLegacyUserConfig("MFConnector.exe_Url_abc123", "10.5.2.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>75</value>
                </setting>");
            var settings = CreateFreshSettings();
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.AreEqual(75, settings.PollInterval);
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_HigherVersionThanCurrent_WillBeIgnored()
        {
            // Arrange
            var settings = CreateFreshSettings();
            var defaultPollInterval = settings.PollInterval;
            CreateLegacyUserConfig("MFConnector.exe_Url_newVersion", "11.1.0.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>10</value>
                </setting>");
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, new Version(11, 0));
            // Assert
            Assert.AreEqual(defaultPollInterval, settings.PollInterval);
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_MultipleHashFolders_UsesMostRecentlyModified()
        {
            // Arrange
            var olderConfig = CreateLegacyUserConfig("MFConnector.exe_Url_older", "10.5.2.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>10</value>
                </setting>");
            File.SetLastWriteTimeUtc(olderConfig, DateTime.UtcNow.AddDays(-2));
            var newerConfig = CreateLegacyUserConfig("MFConnector.exe_Url_newer", "10.5.2.0", @"
                <setting name=""PollInterval"" serializeAs=""String"">
                    <value>60</value>
                </setting>");
            File.SetLastWriteTimeUtc(newerConfig, DateTime.UtcNow.AddDays(-1));
            var settings = CreateFreshSettings();
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.AreEqual(60, settings.PollInterval);
        }
        [TestMethod]
        public void MigrateLegacySettingsIfNeeded_NoUserConfigAnywhere_MarksMigratedWithoutThrowing()
        {
            // Arrange: hash folder and version folder exist, but no user.config inside
            Directory.CreateDirectory(Path.Combine(_testDirectory, "MFConnector.exe_Url_abc123", "10.5.2.0"));
            var settings = CreateFreshSettings();
            // Act
            UserSettingsMigration.MigrateLegacySettingsIfNeeded(settings, _testDirectory, DefaultCurrentVersion);
            // Assert
            Assert.IsTrue(settings.LegacySettingsMigrated);
        }
        // Isolates the test from whatever a previous test in this process may have Save()d to disk.
        private Properties.Settings CreateFreshSettings()
        {
            var settings = new Properties.Settings();
            settings.Reset();
            return settings;
        }
        // Writes a legacy user.config at {companyFolder}\{hashFolderName}\{version}\user.config
        private string CreateLegacyUserConfig(string hashFolderName, string version, string settingsXml)
        {
            var versionFolder = Path.Combine(_testDirectory, hashFolderName, version);
            Directory.CreateDirectory(versionFolder);
            var configPath = Path.Combine(versionFolder, "user.config");
            File.WriteAllText(configPath, $@"<?xml version=""1.0"" encoding=""utf-8""?>
                <configuration>
                    <userSettings>
                        <MobiFlight.Properties.Settings>
                            {settingsXml}
                        </MobiFlight.Properties.Settings>
                    </userSettings>
                </configuration>");
            return configPath;
        }
    }
}