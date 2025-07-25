using MobiFlight.InputConfig;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Base
{
    public static class ConfigFileExtensions
    {
        public static List<ConfigRefValue> ResolveReferences(this ConfigFile configFile, ConfigRefList configRefs)
        {
            List<ConfigRefValue> result = new List<ConfigRefValue>();
            foreach (ConfigRef c in configRefs)
            {
                if (!c.Active) continue;
                string value = configFile.FindValueForRef(c.Ref);
                if (value == null) continue;
                result.Add(new ConfigRefValue(c, value));
            }
            return result;
        }

        public static Dictionary<string, MobiFlightVariable> GetAvailableVariables(this ConfigFile configFile)
        {
            Dictionary<string, MobiFlightVariable> variables = new Dictionary<string, MobiFlightVariable>();

            configFile.ConfigItems.Where(i =>
            {
                return i?.GetType() == typeof(OutputConfigItem) &&
                       (i as OutputConfigItem).Source is VariableSource;
            }).ToList().ForEach(i =>
            {
                var source = (i as OutputConfigItem).Source as VariableSource;
                if (variables.ContainsKey(source.MobiFlightVariable.Name)) return;
                variables[source.MobiFlightVariable.Name] = source.MobiFlightVariable;
            });

            configFile.ConfigItems.Where(i => i?.GetType() == typeof(InputConfigItem)).ToList().ForEach(i =>
            {
                var cfg = i as InputConfigItem;
                List<InputAction> actions = cfg.GetInputActionsByType(typeof(VariableInputAction));
                if (actions == null) return;

                actions.ForEach(action =>
                {
                    VariableInputAction a = (VariableInputAction)action;
                    if (variables.ContainsKey(a.Variable.Name)) return;
                    variables[a.Variable.Name] = a.Variable;
                });
            });

            return variables;
        }

        private static string FindValueForRef(this ConfigFile configFile, string refId)
        {
            foreach (var cfg in configFile.ConfigItems)
            {
                if (cfg.GUID != refId) continue;
                if (!cfg.Active) break;
                if (cfg.Value == null) break;

                string value = cfg.Value;
                if (string.IsNullOrEmpty(value)) break;
                return value;
            }
            return null;
        }
    }
}
