using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.Config
{
    public interface IConfigItem
    {
        bool FromInternal(String value);
        DeviceType Type { get; }
        String Name { get; }
        bool isMuxClient { get; }
        String ToInternal();
    }
}
