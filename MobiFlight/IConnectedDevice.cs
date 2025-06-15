using System;

namespace MobiFlight
{
    public interface IConnectedDevice
    {
        String Name { get; }
        DeviceType TypeDeprecated { get; }

        void Stop();
    }
}
