using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MobiFlight.SimConnectMSFS.Native
{
    /// <summary>
    /// Thin P/Invoke replacement for the FSX SP2 "Microsoft.FlightSimulator.SimConnect" C++/CLI wrapper.
    /// Talks directly to the native SimConnect.dll and mirrors the subset of the old wrapper's API that
    /// MobiFlight uses, so call sites only need their `using` changed.
    ///
    /// Unlike the old wrapper, message dispatch is done by polling SimConnect_GetNextDispatch from managed
    /// code instead of registering a native callback via SimConnect_CallDispatch. That avoids ever handing
    /// the CRT a delegate to keep alive across CLR shutdown, which was the source of the
    /// CallbackOnCollectedDelegate crash on exit.
    /// </summary>
    public sealed class SimConnect : IDisposable
    {
        public delegate void RecvOpenEventHandler(SimConnect sender, SIMCONNECT_RECV_OPEN data);
        public delegate void RecvQuitEventHandler(SimConnect sender, SIMCONNECT_RECV data);
        public delegate void RecvExceptionEventHandler(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data);
        public delegate void RecvSimobjectDataEventHandler(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data);
        public delegate void RecvSystemStateEventHandler(SimConnect sender, SIMCONNECT_RECV_SYSTEM_STATE data);
        public delegate void RecvEventFilenameEventHandler(SimConnect sender, SIMCONNECT_RECV_EVENT_FILENAME data);
        public delegate void RecvClientDataEventHandler(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data);

        public event RecvOpenEventHandler OnRecvOpen;
        public event RecvQuitEventHandler OnRecvQuit;
        public event RecvExceptionEventHandler OnRecvException;
        public event RecvSimobjectDataEventHandler OnRecvSimobjectData;
        public event RecvSystemStateEventHandler OnRecvSystemState;
        public event RecvEventFilenameEventHandler OnRecvEventFilename;
        public event RecvClientDataEventHandler OnRecvClientData;

        public const uint SIMCONNECT_UNUSED = 0xFFFFFFFF;
        public const uint SIMCONNECT_OBJECT_ID_USER = 0;

        private IntPtr hSimConnect;

        // Keyed by the receive-message kind (SIMCONNECT_RECV_SIMOBJECT_DATA or SIMCONNECT_RECV_CLIENT_DATA),
        // then by dwDefineID, recording which struct type to unmarshal the variable-size payload into.
        private readonly Dictionary<Type, Dictionary<uint, Type>> m_RegisteredStructs = new Dictionary<Type, Dictionary<uint, Type>>();

        public SimConnect(string name, IntPtr hWnd, uint userEventWin32, IntPtr hEvent, uint configIndex)
        {
            int hr = SimConnectNativeMethods.SimConnect_Open(out hSimConnect, name, hWnd, userEventWin32, hEvent, configIndex);
            if (hr < 0)
            {
                hSimConnect = IntPtr.Zero;
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        public void AddToDataDefinition(Enum defineId, string datumName, string unitsName, SIMCONNECT_DATATYPE datumType, float epsilon, uint datumId)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_AddToDataDefinition(hSimConnect, Convert.ToUInt32(defineId), datumName, unitsName, datumType, epsilon, datumId));
        }

        public void RequestDataOnSimObject(Enum requestId, Enum defineId, uint objectId, SIMCONNECT_PERIOD period, SIMCONNECT_DATA_REQUEST_FLAG flags, uint origin, uint interval, uint limit)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_RequestDataOnSimObject(hSimConnect, Convert.ToUInt32(requestId), Convert.ToUInt32(defineId), objectId, period, flags, origin, interval, limit));
        }

        public void RegisterDataDefineStruct<T>(Enum defineId)
        {
            RegisterStruct<SIMCONNECT_RECV_SIMOBJECT_DATA, T>(defineId);
        }

        public void RegisterStruct<TRecv, T>(Enum defineId)
        {
            if (!m_RegisteredStructs.TryGetValue(typeof(TRecv), out var map))
            {
                map = new Dictionary<uint, Type>();
                m_RegisteredStructs[typeof(TRecv)] = map;
            }
            map[Convert.ToUInt32(defineId)] = typeof(T);
        }

        public void SubscribeToSystemEvent(Enum eventId, string systemEventName)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_SubscribeToSystemEvent(hSimConnect, Convert.ToUInt32(eventId), systemEventName));
        }

        public void RequestSystemState(Enum requestId, string state)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_RequestSystemState(hSimConnect, Convert.ToUInt32(requestId), state));
        }

        public void MapClientEventToSimEvent(Enum eventId, string eventName = "")
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_MapClientEventToSimEvent(hSimConnect, Convert.ToUInt32(eventId), eventName));
        }

        public void TransmitClientEvent(uint objectId, Enum eventId, uint data, Enum groupId, SIMCONNECT_EVENT_FLAG flags)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_TransmitClientEvent(hSimConnect, objectId, Convert.ToUInt32(eventId), data, Convert.ToUInt32(groupId), flags));
        }

        public void MapClientDataNameToID(string clientDataName, Enum clientDataId)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_MapClientDataNameToID(hSimConnect, clientDataName, Convert.ToUInt32(clientDataId)));
        }

        public void CreateClientData(Enum clientDataId, uint size, SIMCONNECT_CREATE_CLIENT_DATA_FLAG flags)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_CreateClientData(hSimConnect, Convert.ToUInt32(clientDataId), size, flags));
        }

        public void AddToClientDataDefinition(Enum defineId, uint offset, uint sizeOrType, float epsilon, uint datumId)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_AddToClientDataDefinition(hSimConnect, Convert.ToUInt32(defineId), offset, sizeOrType, epsilon, datumId));
        }

        public void RequestClientData(Enum clientDataId, Enum requestId, Enum defineId, SIMCONNECT_CLIENT_DATA_PERIOD period, SIMCONNECT_CLIENT_DATA_REQUEST_FLAG flags, uint origin, uint interval, uint limit)
        {
            ThrowIfFailed(SimConnectNativeMethods.SimConnect_RequestClientData(hSimConnect, Convert.ToUInt32(clientDataId), Convert.ToUInt32(requestId), Convert.ToUInt32(defineId), period, flags, origin, interval, limit));
        }

        public void SetClientData(Enum clientDataId, Enum defineId, SIMCONNECT_CLIENT_DATA_SET_FLAG flags, uint reserved, object dataSet)
        {
            int size = Marshal.SizeOf(dataSet);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(dataSet, buffer, false);
                ThrowIfFailed(SimConnectNativeMethods.SimConnect_SetClientData(hSimConnect, Convert.ToUInt32(clientDataId), Convert.ToUInt32(defineId), flags, reserved, (uint)size, buffer));
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Drains all currently queued SimConnect messages, raising the corresponding Recv* event for each.
        /// Exceptions thrown by an event handler propagate to the caller (matching the old wrapper's
        /// behavior of letting a handler exception surface through ReceiveMessage), so SimConnectCache's
        /// own try/catch around ReceiveSimConnectMessage still triggers a reconnect on handler failures.
        /// Only malformed/unexpected native payloads are caught here and skipped.
        /// </summary>
        public void ReceiveMessage()
        {
            while (SimConnectNativeMethods.SimConnect_GetNextDispatch(hSimConnect, out IntPtr pData, out uint cbData) == 0)
            {
                Dispatch(pData, cbData);
            }
        }

        private void Dispatch(IntPtr pData, uint cbData)
        {
            SIMCONNECT_RECV_ID recvId;
            try
            {
                recvId = (SIMCONNECT_RECV_ID)Marshal.PtrToStructure<SIMCONNECT_RECV>(pData).dwID;
            }
            catch (Exception e)
            {
                Log.Instance.log($"Failed to read SimConnect message header: {e.Message}", LogSeverity.Error);
                return;
            }

            switch (recvId)
            {
                case SIMCONNECT_RECV_ID.EXCEPTION:
                    OnRecvException?.Invoke(this, Marshal.PtrToStructure<SIMCONNECT_RECV_EXCEPTION>(pData));
                    break;

                case SIMCONNECT_RECV_ID.OPEN:
                    OnRecvOpen?.Invoke(this, Marshal.PtrToStructure<SIMCONNECT_RECV_OPEN>(pData));
                    break;

                case SIMCONNECT_RECV_ID.QUIT:
                    OnRecvQuit?.Invoke(this, Marshal.PtrToStructure<SIMCONNECT_RECV>(pData));
                    break;

                case SIMCONNECT_RECV_ID.EVENT_FILENAME:
                    OnRecvEventFilename?.Invoke(this, Marshal.PtrToStructure<SIMCONNECT_RECV_EVENT_FILENAME>(pData));
                    break;

                case SIMCONNECT_RECV_ID.SYSTEM_STATE:
                    OnRecvSystemState?.Invoke(this, Marshal.PtrToStructure<SIMCONNECT_RECV_SYSTEM_STATE>(pData));
                    break;

                case SIMCONNECT_RECV_ID.SIMOBJECT_DATA:
                    OnRecvSimobjectData?.Invoke(this, ParseSimObjectData<SIMCONNECT_RECV_SIMOBJECT_DATA>(pData, cbData, typeof(SIMCONNECT_RECV_SIMOBJECT_DATA)));
                    break;

                case SIMCONNECT_RECV_ID.CLIENT_DATA:
                    OnRecvClientData?.Invoke(this, ParseSimObjectData<SIMCONNECT_RECV_CLIENT_DATA>(pData, cbData, typeof(SIMCONNECT_RECV_CLIENT_DATA)));
                    break;

                default:
                    // Message kinds MobiFlight doesn't subscribe to are ignored.
                    break;
            }
        }

        private T ParseSimObjectData<T>(IntPtr pData, uint cbData, Type recvKind) where T : SIMCONNECT_RECV_SIMOBJECT_DATA, new()
        {
            m_RegisteredStructs.TryGetValue(recvKind, out var typesByDefineId);
            return ParseSimObjectData<T>(pData, cbData, typesByDefineId);
        }

        internal static T ParseSimObjectData<T>(IntPtr pData, uint cbData, Dictionary<uint, Type> typesByDefineId) where T : SIMCONNECT_RECV_SIMOBJECT_DATA, new()
        {
            var header = Marshal.PtrToStructure<SIMCONNECT_RECV_SIMOBJECT_DATA_HEADER>(pData);

            object value = null;
            if (typesByDefineId != null && typesByDefineId.TryGetValue(header.dwDefineID, out Type payloadType))
            {
                try
                {
                    IntPtr payloadPtr = IntPtr.Add(pData, Marshal.SizeOf<SIMCONNECT_RECV_SIMOBJECT_DATA_HEADER>());
                    value = Marshal.PtrToStructure(payloadPtr, payloadType);
                }
                catch (Exception e)
                {
                    Log.Instance.log($"Failed to unmarshal SimConnect client data payload for defineId {header.dwDefineID}: {e.Message}", LogSeverity.Error);
                }
            }

            return new T
            {
                dwRequestID = header.dwRequestID,
                dwObjectID = header.dwObjectID,
                dwDefineID = header.dwDefineID,
                dwFlags = header.dwFlags,
                dwentrynumber = header.dwentrynumber,
                dwoutof = header.dwoutof,
                dwDefineCount = header.dwDefineCount,
                dwData = new object[] { value }
            };
        }

        private static void ThrowIfFailed(int hr)
        {
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
        }

        public void Dispose()
        {
            if (hSimConnect != IntPtr.Zero)
            {
                SimConnectNativeMethods.SimConnect_Close(hSimConnect);
                hSimConnect = IntPtr.Zero;
            }
        }
    }
}
