using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    internal class ConfigValueFullUpdate
    {
        public int ConfigIndex { get; set; }
        public List<IConfigItem> ConfigItems { get; set; }

        public ConfigValueFullUpdate() { }
        public ConfigValueFullUpdate(int configIndex, IConfigItem item) : this(configIndex, new List<IConfigItem>() { item })
        {
        }

        public ConfigValueFullUpdate(int configIndex, List<IConfigItem> configItems)
        {
            ConfigIndex = configIndex;
            ConfigItems = configItems;
        }
    }
}
