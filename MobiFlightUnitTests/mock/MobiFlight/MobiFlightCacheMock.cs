using MobiFlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlightUnitTests.mock.MobiFlight
{
    class MobiFlightCacheMock : MobiFlightCacheInterface
    {
        public async Task<IEnumerable<MobiFlightModule>> GetModulesAsync()
        {
            List<MobiFlightModule> list = new List<MobiFlightModule>()
            {
                new MobiFlightModule(new MobiFlightModuleConfig())
            };

            return list;
        }

        public IEnumerable<MobiFlightModule> GetModules()
        {
            return new List<MobiFlightModule>() {
                new MobiFlightModule(new MobiFlightModuleConfig())
            };
        }
    }
}
