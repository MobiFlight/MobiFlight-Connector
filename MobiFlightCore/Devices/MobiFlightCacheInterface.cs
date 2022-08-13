using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public interface MobiFlightCacheInterface
    {
        Task<IEnumerable<MobiFlightModule>> GetModulesAsync();
        IEnumerable<MobiFlightModule> GetModules();

        void SetMobiFlightVariable(MobiFlightVariable value);

        MobiFlightVariable GetMobiFlightVariable(String name);
    }
}
