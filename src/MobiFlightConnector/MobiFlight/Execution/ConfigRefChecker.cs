using MobiFlight.Base;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Execution
{
    public static class ConfigRefChecker
    {
        /// <summary>
        /// Flags the config item if one of its active config references points to a config item that no longer exists.
        /// </summary>
        public static void UpdateStatus(IConfigItem cfg, List<IConfigItem> configItems)
        {
            var hasMissingRef = cfg.ConfigRefs.Any(
                configRef => configRef.Active && !configItems.Any(item => item.GUID == configRef.Ref)
            );

            if (hasMissingRef)
            {
                cfg.Status[ConfigItemStatusType.ConfigRef] = "Missing";
            }
            else
            {
                cfg.Status.Remove(ConfigItemStatusType.ConfigRef);
            }
        }
    }
}
