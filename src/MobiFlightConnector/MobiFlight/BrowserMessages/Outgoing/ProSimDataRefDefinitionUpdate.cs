using MobiFlight.ProSim;
using System.Collections.Generic;

namespace MobiFlight.BrowserMessages.Outgoing
{
    public class ProSimDataRefDefinitionUpdate
    {
        public Dictionary<string, DataRefDescription> DataRefs { get; set; } = new Dictionary<string, DataRefDescription>();
    }
}