using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    internal class ConfigValueRawAndFinalUpdate
    {
        public List<IConfigValueOnlyItem> ConfigItems { get; set; }

        public ConfigValueRawAndFinalUpdate() { }
        public ConfigValueRawAndFinalUpdate(IConfigValueOnlyItem item)
        {
            ConfigItems = new List<IConfigValueOnlyItem>
            {
                item
            };
        }

        public ConfigValueRawAndFinalUpdate(List<IConfigValueOnlyItem> configItems)
        {
            ConfigItems = configItems;
        }
    }
}
