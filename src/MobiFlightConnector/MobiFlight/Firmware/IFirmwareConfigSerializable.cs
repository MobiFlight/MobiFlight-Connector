using System;

namespace MobiFlight.Firmware
{
    public interface IFirmwareConfigSerializable
    {
        bool FromInternal(String value);
        bool isMuxClient { get; }
        String ToInternal();
    }
}