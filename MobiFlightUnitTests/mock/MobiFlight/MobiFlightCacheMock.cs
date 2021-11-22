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
        Dictionary<string, MobiFlightVariable> variables = new Dictionary<string, MobiFlightVariable>();

        public async Task<IEnumerable<MobiFlightModule>> GetModulesAsync()
        {
            return await Task.Run(() => {
                List<MobiFlightModule> list = new List<MobiFlightModule>()
                {
                    new MobiFlightModule("COM1", new Board())
                };

                    return list;
                }
            );
            
        }

        public IEnumerable<MobiFlightModule> GetModules()
        {
            return new List<MobiFlightModule>() {
                new MobiFlightModule("COM1", new Board())
            };
        }

        public void SetMobiFlightVariable(MobiFlightVariable value)
        {
            variables[value.Name] = value;
        }

        public MobiFlightVariable GetMobiFlightVariable(String name)
        {
            if (!variables.Keys.Contains(name))
            {
                variables[name] = new MobiFlightVariable();
            }

            return variables[name];
        }
    }
}
