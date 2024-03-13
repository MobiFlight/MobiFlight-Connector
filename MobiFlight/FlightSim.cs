using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MobiFlight
{
    public enum FlightSimConnectionMethod
    {
        NONE,
        UNKNOWN,
        FSUIPC,
        WIDECLIENT,
        XPUIPC,
        SIMCONNECT,
        SIMCONNECT_REMOTE,
        XPLANE,
        XPLANE_REMOTE,
        OFFLINE
    }

    public enum FlightSimType
    {
        NONE,
        UNKNOWN,
        FS9,
        FSX,
        P3D,
        XPLANE,
        MSFS2020
    }

    static public class FlightSim
    {
        static public FlightSimConnectionMethod FlightSimConnectionMethod = FlightSimConnectionMethod.NONE;
        static public FlightSimType FlightSimType = FlightSimType.NONE;

        static public Dictionary<FlightSimConnectionMethod, String> SimConnectionNames = new Dictionary<FlightSimConnectionMethod, string>()
        {
            { FlightSimConnectionMethod.NONE, "None" },
            { FlightSimConnectionMethod.UNKNOWN, "Unknown" },
            { FlightSimConnectionMethod.FSUIPC, "FSUIPC" },
            { FlightSimConnectionMethod.WIDECLIENT, "WideClient" },
            { FlightSimConnectionMethod.XPUIPC, "XPUIPC" },
            { FlightSimConnectionMethod.SIMCONNECT, "SimConnect" },
            { FlightSimConnectionMethod.SIMCONNECT_REMOTE, "SimConnect (Remote)" },
            { FlightSimConnectionMethod.XPLANE, "X-Plane (Direct)" },
            { FlightSimConnectionMethod.XPLANE_REMOTE, "X-Plane (Remote)" },
            { FlightSimConnectionMethod.OFFLINE, "Offline" },
        };

        static public Dictionary<FlightSimType, String> SimNames = new Dictionary<FlightSimType, string>()
        {
            { FlightSimType.NONE, "None" },
            { FlightSimType.UNKNOWN, "Unknown" },
            { FlightSimType.FS9, "FS2004" },
            { FlightSimType.FSX, "FSX" },
            { FlightSimType.P3D, "Prepar3D" },
            { FlightSimType.XPLANE, "X-Plane" },
            { FlightSimType.MSFS2020, "MSFS2020" },
        };

        static public bool IsAvailable()
        {
            string proc = "fs9";
            // check for fs2004 / fs9
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.FSUIPC;
                FlightSimType = FlightSimType.FS9;
                return true;
            }
            proc = "fsx";
            // check for fsx
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.FSUIPC;
                FlightSimType = FlightSimType.FSX;
                return true;
            }

            proc = "flightsimulator";
            // check for msfs2020
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.SIMCONNECT;
                FlightSimType = FlightSimType.MSFS2020;
                return true;
            }

            if (Properties.Settings.Default.RemoteConnection == FlightSimType.MSFS2020.ToString())
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.SIMCONNECT_REMOTE;
                FlightSimType = FlightSimType.MSFS2020;
                return true;
            }

            proc = "wideclient";
            // check for FSUIPC wide client
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                //fsuipcToolStripStatusLabel.Text = i18n._tr("fsuipcStatus") + ":";
                FlightSimConnectionMethod = FlightSimConnectionMethod.WIDECLIENT;
                FlightSimType = FlightSimType.UNKNOWN;
                return true;
            }
            // check for prepar3d
            proc = "prepar3d";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.FSUIPC;
                FlightSimType = FlightSimType.P3D;
                return true;
            }
            // check for x-plane and xpuipc
            proc = "x-plane";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.XPLANE;
                FlightSimType = FlightSimType.XPLANE;
                return true;
            }

            proc = "x-plane-32bit";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.XPLANE;
                FlightSimType = FlightSimType.XPLANE;
                return true;
            }

            proc = "xpwideclient";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.WIDECLIENT;
                FlightSimType = FlightSimType.XPLANE;
                return true;
            }

            if (Properties.Settings.Default.RemoteConnection == FlightSimType.XPLANE.ToString())
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.XPLANE_REMOTE;
                FlightSimType = FlightSimType.XPLANE;
                return true;
            }

            // if we made it here, then
            // we didn't detect anything
            FlightSimType = FlightSimType.NONE;
            return false;
        }
    }
}
