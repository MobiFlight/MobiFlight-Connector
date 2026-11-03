using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MobiFlight.Base.Migration
{
    /// <summary>
    /// v0.11 has the following changes:
    /// - LedModule output config: the separate DisplayLedConnector is removed
    ///   and merged into DisplayLedAddress in the format "Name:Module" (1-based)
    /// </summary>
    public static class V0_11_LedModuleAddressMigration
    {
        private const string LedModuleType = "LedModule";
        private static readonly Regex HasModuleIndex = new(@":\d+$");

        public static JObject Apply(JObject document)
        {
            var migrated = document.DeepClone() as JObject;

            var migrationCount = 0;
            foreach (var configItem in FindConfigItems(migrated))
            {
                var device = configItem["Device"] as JObject;
                if (device?["Type"]?.ToString() != LedModuleType)
                    continue;

                MigrateAddress(device);
                migrationCount++;
            }

            if (migrationCount > 0)
            {
                Log.Instance.log($"Migrated {migrationCount} LED module outputs to v0.11 format", LogSeverity.Debug);
            }

            return migrated;
        }

        private static void MigrateAddress(JObject device)
        {
            var address = device["DisplayLedAddress"]?.ToString();
            var connector = device["DisplayLedConnector"]?.ToString();

            device.Remove("DisplayLedConnector");

            if (address == null)
                return;

            if (connector != null)
            {
                device["DisplayLedAddress"] = $"{address}:{connector}";
                return;
            }

            // a missing connector used to default to 1
            if (!HasModuleIndex.IsMatch(address))
                device["DisplayLedAddress"] = $"{address}:1";
        }

        private static List<JToken> FindConfigItems(JObject document)
        {
            var result = new List<JToken>();

            if (document["ConfigFiles"] is not JArray configFiles)
                return result;

            foreach (var configFile in configFiles)
            {
                if (configFile["ConfigItems"] is not JArray configItems)
                    continue;

                foreach (var configItem in configItems)
                    result.Add(configItem);
            }

            return result;
        }
    }
}