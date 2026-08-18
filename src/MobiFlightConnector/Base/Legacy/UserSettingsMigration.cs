using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MobiFlight.Base.Legacy
{
    static public class UserSettingsMigration
    {
        static readonly string Company = "MobiFlight";
        static readonly string LegacyFolderPrefix = "MFConnector.exe";

        public static void MigrateLegacySettingsIfNeeded()
        {
            if (Properties.Settings.Default.LegacySettingsMigrated)
                return;

            try
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string companyFolder = Path.Combine(localAppData, Company);

                if (!Directory.Exists(companyFolder))
                    return;

                // Legacy folders look like: MFConnector.exe_Url_<hash>  (hash varies)
                var legacyFolders = Directory.GetDirectories(companyFolder, $"{LegacyFolderPrefix}_*");

                if (legacyFolders.Length == 0)
                    return;

                var candidates = new List<(string ConfigPath, DateTime LastModified)>();

                foreach (var legacyFolder in legacyFolders)
                {
                    // Within this hash folder, find the highest version subfolder that actually has a user.config
                    var bestVersionConfig = Directory.GetDirectories(legacyFolder).ToList()
                        .Select(d => new { Path = d, Name = Path.GetFileName(d) })
                        .Where(x => Version.TryParse(x.Name, out _))
                        .OrderByDescending(x => Version.Parse(x.Name))
                        .Select(x => Path.Combine(x.Path, "user.config"))
                        .FirstOrDefault(File.Exists);

                    if (bestVersionConfig != null)
                    {
                        candidates.Add((bestVersionConfig, File.GetLastWriteTimeUtc(bestVersionConfig)));
                    }
                }

                if (candidates.Count == 0)
                {
                    Log.Instance.log("No legacy user.config files found for migration.", LogSeverity.Debug);
                    return;
                }

                // Across all lineages, take the most recently modified user.config
                string legacyConfigPath = candidates
                    .OrderByDescending(c => c.LastModified)
                    .First()
                    .ConfigPath;

                var doc = XDocument.Load(legacyConfigPath);
                foreach (var el in doc.Descendants("setting"))
                {
                    string name = el.Attribute("name")?.Value;
                    string value = el.Element("value")?.Value;
                    if (name == null || value == null) continue;

                    var prop = Properties.Settings.Default.Properties[name];
                    if (prop == null) continue;

                    try
                    {
                        var converter = TypeDescriptor.GetConverter(prop.PropertyType);
                        object converted = converter.CanConvertFrom(typeof(string))
                            ? converter.ConvertFromInvariantString(value)
                            : Convert.ChangeType(value, prop.PropertyType);

                        Properties.Settings.Default[name] = converted;
                    }
                    catch
                    {
                        // skip settings that fail to convert (e.g. complex serialized types)
                    }
                }

                Properties.Settings.Default.LegacySettingsMigrated = true;
                Properties.Settings.Default.Save();
            }
            catch
            {
                // log, don't block startup
            }
        }

    }
}