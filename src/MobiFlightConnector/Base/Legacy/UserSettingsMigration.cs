using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace MobiFlight.Base.Legacy
{
    static public class UserSettingsMigration
    {
        static readonly string Company = "MobiFlight";
        static readonly string LegacyFolderPrefix = "MFConnector.exe";

        public static void MigrateLegacySettingsIfNeeded()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var companyFolder = Path.Combine(localAppData, Company);
            var currentVersion = typeof(UserSettingsMigration).Assembly.GetName().Version;

            MigrateLegacySettingsIfNeeded(Properties.Settings.Default, companyFolder, currentVersion);
        }

        /// <summary>
        /// Does the actual migration work. Takes the settings to migrate into and the folder to look
        /// for legacy "MFConnector.exe_*" installs under as parameters so this can be exercised in
        /// tests without touching the real %LOCALAPPDATA% or the process-wide Properties.Settings.Default.
        /// </summary>
        internal static void MigrateLegacySettingsIfNeeded(Properties.Settings settings, string companyFolder, Version currentVersion)
        {
            if (settings.LegacySettingsMigrated)
                return;

            try
            {
                string legacyConfigPath = FindMostRecentLegacyConfigPath(companyFolder, LegacyFolderPrefix, currentVersion);
                if (legacyConfigPath == null)
                {
                    Log.Instance.log("No legacy user.config files found for migration.", LogSeverity.Debug);
                }

                foreach (var setting in ParseLegacySettings(legacyConfigPath))
                {
                    var prop = settings.Properties[setting.Key];
                    if (prop == null) continue;

                    if (TryConvertSettingValue(setting.Value, prop.PropertyType, out object converted))
                    {
                        settings[setting.Key] = converted;
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Instance.log($"Exception during user.config migration: {ex}", LogSeverity.Debug);
            }

            // in all cases mark as migrated so we don't keep scanning for legacy configs on every startup
            settings.LegacySettingsMigrated = true;
            settings.Save();
        }

        /// <summary>
        /// Finds the most recently modified legacy user.config across all "MFConnector.exe_*" hash
        /// folders under <paramref name="companyFolder"/>, taking the highest version subfolder
        /// within each hash folder that actually contains a user.config. Returns null if none exist.
        /// </summary>
        private static string FindMostRecentLegacyConfigPath(string companyFolder, string legacyFolderPrefix, Version currentVersion)
        {
            if (!Directory.Exists(companyFolder))
                return null;

            // Legacy folders look like: MFConnector.exe_Url_<hash>  (hash varies)
            var legacyFolders = Directory.GetDirectories(companyFolder, $"{legacyFolderPrefix}_*");

            if (legacyFolders.Length == 0)
                return null;

            var candidates = new List<(string ConfigPath, DateTime LastModified)>();

            foreach (var legacyFolder in legacyFolders)
            {
                // Within this hash folder, find the highest version subfolder that actually has a user.config
                var bestVersionConfig = Directory.GetDirectories(legacyFolder).ToList()
                    .Select(d => new { Path = d, Name = Path.GetFileName(d) })
                    // only consider versions <= the current version, to avoid migrating from a newer install
                    .Where(x => Version.TryParse(x.Name, out var version) && version.CompareTo(currentVersion) <= 0)
                    .OrderByDescending(x => Version.Parse(x.Name))
                    .Select(x => Path.Combine(x.Path, "user.config"))
                    .FirstOrDefault(File.Exists);

                if (bestVersionConfig != null)
                {
                    candidates.Add((bestVersionConfig, File.GetLastWriteTimeUtc(bestVersionConfig)));
                }
            }

            if (candidates.Count == 0)
                return null;

            // Across all lineages, take the most recently modified user.config
            return candidates
                .OrderByDescending(c => c.LastModified)
                .First()
                .ConfigPath;
        }

        // Reads each <setting> element out of a legacy .NET user.config file, keyed by name.
        // Purely parses XML, no dependency on Properties.Settings.Default.
        private static Dictionary<string, XElement> ParseLegacySettings(string legacyConfigPath)
        {
            var result = new Dictionary<string, XElement>();

            if (legacyConfigPath == null || !File.Exists(legacyConfigPath))
                return result;

            var doc = XDocument.Load(legacyConfigPath);
            foreach (var el in doc.Descendants("setting"))
            {
                string name = el.Attribute("name")?.Value;
                if (name == null || el.Element("value") == null) continue;

                result[name] = el;
            }

            return result;
        }

        // Converts a legacy <setting> element to the given target type, the same way
        // System.Configuration would when loading a .config file: serializeAs="String" goes
        // through a TypeConverter, serializeAs="Xml" (e.g. StringCollection) through XmlSerializer.
        private static bool TryConvertSettingValue(XElement settingElement, Type targetType, out object converted)
        {
            converted = null;
            var valueElement = settingElement.Element("value");

            try
            {
                if (settingElement.Attribute("serializeAs")?.Value == "Xml")
                {
                    var xml = valueElement.Elements().FirstOrDefault();
                    if (xml == null) return false;

                    using (var reader = xml.CreateReader())
                    {
                        converted = new XmlSerializer(targetType).Deserialize(reader);
                    }
                    return true;
                }

                var converter = TypeDescriptor.GetConverter(targetType);
                converted = converter.CanConvertFrom(typeof(string))
                    ? converter.ConvertFromInvariantString(valueElement.Value)
                    : Convert.ChangeType(valueElement.Value, targetType);
                return true;
            }
            catch
            {
                // skip settings that fail to convert (e.g. an unrecognized complex type)
                converted = null;
                return false;
            }
        }
    }
}