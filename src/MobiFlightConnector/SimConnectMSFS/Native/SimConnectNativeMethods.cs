using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MobiFlight.SimConnectMSFS.Native
{
    internal static partial class SimConnectNativeMethods
    {
        private const string DllName = "SimConnect.dll";

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_Open(out IntPtr phSimConnect, [MarshalAs(UnmanagedType.LPStr)] string szName, IntPtr hWnd, uint UserEventWin32, IntPtr hEventHandle, uint ConfigIndex);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_Close(IntPtr hSimConnect);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_GetNextDispatch(IntPtr hSimConnect, out IntPtr ppData, out uint pcbData);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_AddToDataDefinition(IntPtr hSimConnect, uint DefineID, [MarshalAs(UnmanagedType.LPStr)] string DatumName, [MarshalAs(UnmanagedType.LPStr)] string UnitsName, SIMCONNECT_DATATYPE DatumType, float fEpsilon, uint DatumID);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_RequestDataOnSimObject(IntPtr hSimConnect, uint RequestID, uint DefineID, uint ObjectID, SIMCONNECT_PERIOD Period, SIMCONNECT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_SubscribeToSystemEvent(IntPtr hSimConnect, uint EventID, [MarshalAs(UnmanagedType.LPStr)] string SystemEventName);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_RequestSystemState(IntPtr hSimConnect, uint RequestID, [MarshalAs(UnmanagedType.LPStr)] string szState);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_MapClientEventToSimEvent(IntPtr hSimConnect, uint EventID, [MarshalAs(UnmanagedType.LPStr)] string EventName);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_TransmitClientEvent(IntPtr hSimConnect, uint ObjectID, uint EventID, uint dwData, uint GroupID, SIMCONNECT_EVENT_FLAG Flags);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_MapClientDataNameToID(IntPtr hSimConnect, [MarshalAs(UnmanagedType.LPStr)] string szClientDataName, uint ClientDataID);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_CreateClientData(IntPtr hSimConnect, uint ClientDataID, uint dwSize, SIMCONNECT_CREATE_CLIENT_DATA_FLAG Flags);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_AddToClientDataDefinition(IntPtr hSimConnect, uint DefineID, uint dwOffset, uint dwSizeOrType, float fEpsilon, uint DatumID);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_RequestClientData(IntPtr hSimConnect, uint ClientDataID, uint RequestID, uint DefineID, SIMCONNECT_CLIENT_DATA_PERIOD Period, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG Flags, uint origin, uint interval, uint limit);

        [LibraryImport(DllName)]
        [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static partial int SimConnect_SetClientData(IntPtr hSimConnect, uint ClientDataID, uint DefineID, SIMCONNECT_CLIENT_DATA_SET_FLAG Flags, uint dwReserved, uint cbUnitSize, IntPtr pDataSet);
    }
}
