using MobiFlight.Frontend;

namespace MobiFlight.BrowserMessages
{
    public class ConfigItemEdit
    {
        public IConfigItem ConfigItem { get; set; }

        public ConfigItemEdit(IConfigItem configItem)
        {
            ConfigItem = configItem;
        }
    }
}