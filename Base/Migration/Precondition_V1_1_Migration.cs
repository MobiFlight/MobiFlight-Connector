using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Base.Migration
{
    /// <summary>
    /// Migrates Precondition properties from V1 (long names) to V2 (short names)
    /// </summary>
    public static class Precondition_V1_1_Migration
    {
        public static JObject Apply(JObject document)
        {
            var migrated = document.DeepClone() as JObject;
            
            // Find all Precondition objects in the document
            var preconditions = FindPreconditionsInDocument(migrated);
            
            foreach (var precondition in preconditions)
            {
                MigratePreconditionProperties(precondition);
            }
            
            if (preconditions.Count > 0)
            {
                Log.Instance.log($"Migrated {preconditions.Count} preconditions from V1 to V2 format", LogSeverity.Debug);
            }
            
            return migrated;
        }
        
        private static List<JObject> FindPreconditionsInDocument(JObject document)
        {
            var preconditions = new List<JObject>();
            
            // Look in ConfigFiles -> ConfigItems -> Preconditions
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
                            var itemPreconditions = configItem["Preconditions"] as JArray;
                            if (itemPreconditions != null)
                            {
                                preconditions.AddRange(itemPreconditions.OfType<JObject>());
                            }
                        }
                    }
                }
            }
            
            return preconditions;
        }
        
        private static void MigratePreconditionProperties(JObject precondition)
        {
            var propertyMappings = new Dictionary<string, string>
            {
                { "PreconditionType", "type" },
                { "PreconditionRef", "ref" },
                { "PreconditionSerial", "serial" },
                { "PreconditionPin", "pin" },
                { "PreconditionOperand", "operand" },
                { "PreconditionValue", "value" },
                { "PreconditionLogic", "logic" },
                { "PreconditionActive", "active" }
            };
            
            foreach (var mapping in propertyMappings)
            {
                if (precondition[mapping.Key] != null)
                {
                    precondition[mapping.Value] = precondition[mapping.Key];
                    precondition.Remove(mapping.Key);
                }
            }
            
            // Apply value transformations
            ApplyV1ValueTransformations(precondition);
        }
        
        private static void ApplyV1ValueTransformations(JObject precondition)
        {
            // Fix V1 operand values
            var operand = precondition["operand"]?.ToString();
            switch (operand)
            {
                case "equals": precondition["operand"] = "="; break;
                case "notequals": precondition["operand"] = "!="; break;
                case "greater": precondition["operand"] = ">"; break;
                case "less": precondition["operand"] = "<"; break;
            }
            
            // Handle V1 boolean strings
            if (precondition["active"]?.Type == JTokenType.String)
            {
                var activeStr = precondition["active"]?.ToString().ToLower();
                precondition["active"] = activeStr == "true" || activeStr == "1" || activeStr == "yes";
            }
        }
    }
}