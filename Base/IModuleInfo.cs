using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArcazeUSB;

namespace MobiFlight
{
    public interface IModuleInfo
    {
        ushort Version { get; set; }
        String Type { get; set; }
        String Serial { get; set; }
        String Port { get; set; }
        String Name { get; set; }
    }
}
