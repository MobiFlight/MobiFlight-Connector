using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class HidDefinition
    {
        public string InstanceName;
        public List<HidInput> Inputs;
        public List<HidOutput> Outputs;

        public void Migrate() { }
    }
}
