using System;
using System.Collections.Generic;

namespace MobiFlight.ProSim
{
    public interface ProSimCacheInterface : Base.CacheInterface
    {
        // Core reading/writing methods with proper type handling
        double readDataref(string datarefPath);
        void writeDataref(string datarefPath, object value);
        Dictionary<string, DataRefDescription> GetDataRefDescriptions();
    }
} 