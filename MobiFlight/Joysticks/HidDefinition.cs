using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class HidDefinition
    {
        public List<HidInput> Inputs;
        public string InstanceName;
        public List<HidOutput> Outputs;
        public int ProductId;
        public int VendorId;

        public void Migrate() { }
    }
}
