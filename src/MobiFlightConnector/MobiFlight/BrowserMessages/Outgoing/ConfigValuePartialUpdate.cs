using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    internal class ConfigValuePartialUpdate
    {   public List<IConfigItem> ConfigItems { get; set; }

        public ConfigValuePartialUpdate() { }
        public ConfigValuePartialUpdate(IConfigItem item)
        {
            ConfigItems = new List<IConfigItem>
            {
                item
            };
        }

        public ConfigValuePartialUpdate(List<IConfigItem> configItems)
        {
            ConfigItems = configItems;
        }
    }
}
