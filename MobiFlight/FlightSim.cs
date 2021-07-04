﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static public bool OfflineMode = false;

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
                FlightSimConnectionMethod = FlightSimConnectionMethod.FSUIPC;
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
                FlightSimConnectionMethod = FlightSimConnectionMethod.XPUIPC;
                FlightSimType = FlightSimType.XPLANE;
                return true;
            }

            proc = "x-plane-32bit";
            if (Process.GetProcessesByName(proc).Length > 0)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.XPUIPC;
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

            if (OfflineMode)
            {
                FlightSimConnectionMethod = FlightSimConnectionMethod.OFFLINE;
                FlightSimType = FlightSimType.UNKNOWN;
                return true;
            }

            return false;
        }
    }
}
