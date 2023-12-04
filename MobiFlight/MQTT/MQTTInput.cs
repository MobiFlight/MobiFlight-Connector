using MobiFlight.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class MQTTInput : IBaseDevice
    {
        public DeviceType Type { get; set; }

        [JsonIgnore()]
        public String Name { get { return Label; } }

        public String Label { get; set; }
    }
}
