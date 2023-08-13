using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight.Config
{
    public interface IConfigItem : IBaseDevice
    {
        bool FromInternal(String value);
        bool isMuxClient { get; }
        String ToInternal();
    }
}
