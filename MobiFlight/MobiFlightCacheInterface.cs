using System;
using System.Collections.Generic;

namespace MobiFlight
{
    public interface MobiFlightCacheInterface
    {
        // Task<IEnumerable<MobiFlightModule>> GetModulesAsync();
        IEnumerable<MobiFlightModule> GetModules();

        void SetMobiFlightVariable(MobiFlightVariable value);

        MobiFlightVariable GetMobiFlightVariable(String name);
    }
}
