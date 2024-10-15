using MobiFlight.Frontend;

namespace MobiFlight.BrowserMessages
{
    public class ConfigItemUpdate
    {
        public IConfigItem ConfigItem { get; set; }

        public ConfigItemUpdate(IConfigItem configItem)
        {
            ConfigItem = configItem;
        }
    }
}