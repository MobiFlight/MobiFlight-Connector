using System;

namespace MobiFlight
{
    public interface IConnectedDevice
    {
        String Name { get; }
        DeviceType Type { get; }
        void Stop();
    }
}
