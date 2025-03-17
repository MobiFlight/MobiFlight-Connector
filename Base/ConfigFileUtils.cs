using System.Linq;

namespace MobiFlight.Base
{
    public static class ConfigFileUtils
    {
        public static void MergeConfigItems(IConfigFile target, IConfigFile source)
        {
            if (target.ReferenceOnly || source.ReferenceOnly)
            {
                // Cannot merge read-only files
                return;
            }

            target.ConfigItems = target.ConfigItems.Union(source.ConfigItems).ToList();
        }
    }
}