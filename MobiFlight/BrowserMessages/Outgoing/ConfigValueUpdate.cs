using MobiFlight.Base;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages
{
    internal class ConfigValueUpdate
    {
        public string UpdateType { get; set; }
        public List<IConfigItem> ConfigItems { get; set; }        
    }
}
