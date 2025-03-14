using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    internal class ConfigValueFullUpdate
    {
        public List<IConfigItem> ConfigItems { get; set; }

        public ConfigValueFullUpdate() { }
        public ConfigValueFullUpdate(IConfigItem item)
        {
            ConfigItems = new List<IConfigItem>
            {
                item
            };
        }

        public ConfigValueFullUpdate(List<IConfigItem> configItems)
        {
            ConfigItems = configItems;
        }
    }
}
