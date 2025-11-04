using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MobiFlight.Base.Migration
{
    /// <summary>
    /// Migrates Output properties from initial version (long names) to v0.9 (short names)
    /// </summary>
    public static class Output_V_0_9_Migration
    {
        public static JObject Apply(JObject document)
        {
            var migrated = document.DeepClone() as JObject;

            MigrateOutputProperties(migrated);
            
            return migrated;
        }

        private static List<JObject> FindOutputsInDocument(JObject document)
        {
            var outputs = new List<JObject>();

            // Look in ConfigFiles -> ConfigItems -> Outputs
            var configFiles = document["ConfigFiles"] as JArray;
            if (configFiles != null)
            {
                foreach (var configFile in configFiles)
                {
                    var configItems = configFile["ConfigItems"] as JArray;
                    if (configItems != null)
                    {
                        foreach (var configItem in configItems)
                        {
                            var itemOutput = configItem["Device"] as JObject;
                            if (itemOutput != null && itemOutput["Type"].ToString() == "Output")
                            {
                                outputs.Add(itemOutput);
                            }
                        }
                    }
                }
            }

            return outputs;
        }

        private static void MigrateOutputProperties(JObject document)
        {
            // Find all outputs objects in the document
            var outputs = FindOutputsInDocument(document);

            foreach (var output in outputs)
            {
                var propertyMappings = new Dictionary<string, string>
                {
                    { "DisplayPin", "pin" },
                    { "DisplayPinBrightness", "brightness" },
                    { "DisplayPinPWM", "pwmMode" },
                };

                foreach (var mapping in propertyMappings)
                {
                    if (output[mapping.Key] != null)
                    {
                        output[mapping.Value] = output[mapping.Key];
                        output.Remove(mapping.Key);
                    }
                }
            }

            if (outputs.Count > 0)
            {
                Log.Instance.log($"Migrated {outputs.Count} outputs to v0.9 format", LogSeverity.Debug);
            }
        }
    }
}