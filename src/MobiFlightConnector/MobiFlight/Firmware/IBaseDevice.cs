using System;

namespace MobiFlight.Firmware
{
    public interface IBaseDevice
    {
        DeviceType Type { get; }
        String Name { get; }
        String Label { get; }
    }
}