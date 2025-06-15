using Newtonsoft.Json;
using System;

namespace MobiFlight
{
    public interface IConnectedDevice
    {
        String Name { get; }

        [JsonIgnore]
        DeviceType TypeDeprecated { get; }

        void Stop();
    }
}
