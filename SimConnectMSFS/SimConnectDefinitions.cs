﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnectMSFS
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ClientDataValue
    {
        public float data;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ClientDataString
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] data;

        public ClientDataString(string strData)
        {
            byte[] txtBytes = Encoding.ASCII.GetBytes(strData);
            var ret = new byte[256];
            Array.Copy(txtBytes, ret, txtBytes.Length);
            data = ret;
        }
    }

    public struct ResponseString
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public String Data;
    }

    public class SimVar
    {
        public UInt32 ID { get; set; }
        public String Name { get; set; }
        public float Data { get; set; }

    }

    public enum SIMCONNECT_CLIENT_DATA_ID
    {
        MOBIFLIGHT_LVARS,
        MOBIFLIGHT_CMD,
        MOBIFLIGHT_RESPONSE
    }

    public enum SIMCONNECT_REQUEST_ID
    {
        Dummy = 0
    }

    public enum SIMCONNECT_DEFINE_ID
    {
        Dummy = 0
    }

    public enum SIMCONNECT_NOTIFICATION_GROUP_ID
    {
        SIMCONNECT_GROUP_PRIORITY_DEFAULT,
        SIMCONNECT_GROUP_PRIORITY_HIGHEST
    }
    public enum MOBIFLIGHT_EVENTS
    {
        DUMMY
    };
}
