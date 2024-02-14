using MobiFlight.Frontend;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    internal class ConfigValueUpdate
    {
        public List<IConfigItem> ConfigItems { get; set; }        
    }
}
