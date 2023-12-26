using System;

namespace MobiFlight.Config
{
    public interface IConfigItem : IBaseDevice
    {
        bool FromInternal(String value);
        bool isMuxClient { get; }
        String ToInternal();
    }
}
