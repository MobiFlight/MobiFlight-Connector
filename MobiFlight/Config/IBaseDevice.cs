using System;
using System.Drawing;

namespace MobiFlight.Config
{
    public interface IBaseDevice
    {
        DeviceType Type { get; }
        String Name { get; }
        String Label { get; }
    }
}
