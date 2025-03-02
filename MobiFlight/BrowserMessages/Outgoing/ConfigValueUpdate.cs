using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    internal class ConfigValueUpdate
    {
        public string UpdateType { get; set; }
        public List<IConfigItem> ConfigItems { get; set; }

        public ConfigValueUpdate() { }
        public ConfigValueUpdate(IConfigItem item)
        {
            ConfigItems = new List<IConfigItem>
            {
                item
            };
        }

        public ConfigValueUpdate(List<IConfigItem> configItems)
        {
            ConfigItems = configItems;
        }
    }
}
