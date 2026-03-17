using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.ProSim
{
    // The "data" level
    public class DataRefData
    {
        public DataRefResponse DataRef { get; set; }
    }

    // The "dataRef" level
    public class DataRefResponse
    {
        public List<DataRefDescription> DataRefDescriptions { get; set; }
        public string __typename { get; set; }
    }

    // The individual dataRefDescription items
    public class DataRefDescription
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public string DataType { get; set; }
        public string DataUnit { get; set; }  // Nullable string since it can be null
        public string __typename { get; set; }
    }

    public class DataRefSubscriptionResult
    {
        public DataRef DataRefs { get; set; }
    }
}

