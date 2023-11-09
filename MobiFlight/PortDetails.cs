using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    /// <summary>
    /// Provides raw information about detected ports and the device attached to the port
    /// </summary>
    public class PortDetails
    {
        public Board Board { get; set; }
        public string HardwareId { get; set; }
        public string Name { get; set; }
    }

    public class UsbPortDetails : PortDetails 
    { 
        public string Path { get; set; }
    }
}
