using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlight
{
    public enum FlightSimConnectionMethod
    {
        NONE,
        UNKNOWN,
        FSUIPC,
        WIDECLIENT,
        XPUIPC,
        OFFLINE
    }

    public enum FlightSim
    {
        NONE,
        UNKNOWN,
        FS9,
        FSX,
        P3D,
        XPLANE,
        MSFS2020
    }
}
