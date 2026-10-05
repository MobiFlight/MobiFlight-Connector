using System.Runtime.InteropServices;

namespace MobiFlight.SimConnectMSFS.Native
{
    // Values taken from the FSX SP2 SimConnect.h / the managed wrapper's metadata.
    // These are stable across FSX/P3D/MSFS since SimConnect.dll preserves wire compatibility.

    public enum SIMCONNECT_RECV_ID
    {
        NULL = 0,
        EXCEPTION = 1,
        OPEN = 2,
        QUIT = 3,
        EVENT = 4,
        EVENT_OBJECT_ADDREMOVE = 5,
        EVENT_FILENAME = 6,
        EVENT_FRAME = 7,
        SIMOBJECT_DATA = 8,
        SIMOBJECT_DATA_BYTYPE = 9,
        WEATHER_OBSERVATION = 10,
        CLOUD_STATE = 11,
        ASSIGNED_OBJECT_ID = 12,
        RESERVED_KEY = 13,
        CUSTOM_ACTION = 14,
        SYSTEM_STATE = 15,
        CLIENT_DATA = 16,
        EVENT_WEATHER_MODE = 17,
        AIRPORT_LIST = 18,
        VOR_LIST = 19,
        NDB_LIST = 20,
        WAYPOINT_LIST = 21,
        EVENT_MULTIPLAYER_SERVER_STARTED = 22,
        EVENT_MULTIPLAYER_CLIENT_STARTED = 23,
        EVENT_MULTIPLAYER_SESSION_ENDED = 24,
        EVENT_RACE_END = 25,
        EVENT_RACE_LAP = 26
    }

    public enum SIMCONNECT_DATATYPE
    {
        INVALID = 0,
        INT32 = 1,
        INT64 = 2,
        FLOAT32 = 3,
        FLOAT64 = 4,
        STRING8 = 5,
        STRING32 = 6,
        STRING64 = 7,
        STRING128 = 8,
        STRING256 = 9,
        STRING260 = 10,
        STRINGV = 11,
        INITPOSITION = 12,
        MARKERSTATE = 13,
        WAYPOINT = 14,
        LATLONALT = 15,
        XYZ = 16,
        MAX = 17
    }

    public enum SIMCONNECT_EXCEPTION
    {
        NONE = 0,
        ERROR = 1,
        SIZE_MISMATCH = 2,
        UNRECOGNIZED_ID = 3,
        UNOPENED = 4,
        VERSION_MISMATCH = 5,
        TOO_MANY_GROUPS = 6,
        NAME_UNRECOGNIZED = 7,
        TOO_MANY_EVENT_NAMES = 8,
        EVENT_ID_DUPLICATE = 9,
        TOO_MANY_MAPS = 10,
        TOO_MANY_OBJECTS = 11,
        TOO_MANY_REQUESTS = 12,
        WEATHER_INVALID_PORT = 13,
        WEATHER_INVALID_METAR = 14,
        WEATHER_UNABLE_TO_GET_OBSERVATION = 15,
        WEATHER_UNABLE_TO_CREATE_STATION = 16,
        WEATHER_UNABLE_TO_REMOVE_STATION = 17,
        INVALID_DATA_TYPE = 18,
        INVALID_DATA_SIZE = 19,
        DATA_ERROR = 20,
        INVALID_ARRAY = 21,
        CREATE_OBJECT_FAILED = 22,
        LOAD_FLIGHTPLAN_FAILED = 23,
        OPERATION_INVALID_FOR_OBJECT_TYPE = 24,
        ILLEGAL_OPERATION = 25,
        ALREADY_SUBSCRIBED = 26,
        INVALID_ENUM = 27,
        DEFINITION_ERROR = 28,
        DUPLICATE_ID = 29,
        DATUM_ID = 30,
        OUT_OF_BOUNDS = 31,
        ALREADY_CREATED = 32,
        OBJECT_OUTSIDE_REALITY_BUBBLE = 33,
        OBJECT_CONTAINER = 34,
        OBJECT_AI = 35,
        OBJECT_ATC = 36,
        OBJECT_SCHEDULE = 37
    }

    public enum SIMCONNECT_PERIOD
    {
        NEVER = 0,
        ONCE = 1,
        VISUAL_FRAME = 2,
        SIM_FRAME = 3,
        SECOND = 4
    }

    public enum SIMCONNECT_CLIENT_DATA_PERIOD
    {
        NEVER = 0,
        ONCE = 1,
        VISUAL_FRAME = 2,
        ON_SET = 3,
        SECOND = 4
    }

    [System.Flags]
    public enum SIMCONNECT_EVENT_FLAG : uint
    {
        DEFAULT = 0x00000000,
        FAST_REPEAT_TIMER = 0x00000001,
        SLOW_REPEAT_TIMER = 0x00000002,
        GROUPID_IS_PRIORITY = 0x00000010
    }

    [System.Flags]
    public enum SIMCONNECT_DATA_REQUEST_FLAG : uint
    {
        DEFAULT = 0x00000000,
        CHANGED = 0x00000001,
        TAGGED = 0x00000002
    }

    [System.Flags]
    public enum SIMCONNECT_CREATE_CLIENT_DATA_FLAG : uint
    {
        DEFAULT = 0x00000000,
        READ_ONLY = 0x00000001
    }

    [System.Flags]
    public enum SIMCONNECT_CLIENT_DATA_REQUEST_FLAG : uint
    {
        DEFAULT = 0x00000000,
        CHANGED = 0x00000001,
        TAGGED = 0x00000002
    }

    [System.Flags]
    public enum SIMCONNECT_CLIENT_DATA_SET_FLAG : uint
    {
        DEFAULT = 0x00000000,
        TAGGED = 0x00000001
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SIMCONNECT_RECV
    {
        public uint dwSize;
        public uint dwVersion;
        public uint dwID;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class SIMCONNECT_RECV_EXCEPTION
    {
        public uint dwSize;
        public uint dwVersion;
        public uint dwID;
        public uint dwException;
        public uint dwSendID;
        public uint dwIndex;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class SIMCONNECT_RECV_OPEN
    {
        public uint dwSize;
        public uint dwVersion;
        public uint dwID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szApplicationName;
        public uint dwApplicationVersionMajor;
        public uint dwApplicationVersionMinor;
        public uint dwApplicationBuildMajor;
        public uint dwApplicationBuildMinor;
        public uint dwSimConnectVersionMajor;
        public uint dwSimConnectVersionMinor;
        public uint dwSimConnectBuildMajor;
        public uint dwSimConnectBuildMinor;
        public uint dwReserved1;
        public uint dwReserved2;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class SIMCONNECT_RECV_EVENT_FILENAME
    {
        public uint dwSize;
        public uint dwVersion;
        public uint dwID;
        public uint uGroupID;
        public uint uEventID;
        public uint dwData;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szFileName;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    public class SIMCONNECT_RECV_SYSTEM_STATE
    {
        public uint dwSize;
        public uint dwVersion;
        public uint dwID;
        public uint dwRequestID;
        public uint dwInteger;
        public float fFloat;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szString;
    }

    // Fixed header shared by SIMCONNECT_RECV_SIMOBJECT_DATA and SIMCONNECT_RECV_CLIENT_DATA.
    // The variable-size payload that follows (dwData) is read manually via the registered
    // struct type for (recv kind, dwDefineID) - see SimConnect.ParseSimObjectData.
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SIMCONNECT_RECV_SIMOBJECT_DATA_HEADER
    {
        public uint dwSize;
        public uint dwVersion;
        public uint dwID;
        public uint dwRequestID;
        public uint dwObjectID;
        public uint dwDefineID;
        public uint dwFlags;
        public uint dwentrynumber;
        public uint dwoutof;
        public uint dwDefineCount;
    }

    public class SIMCONNECT_RECV_SIMOBJECT_DATA
    {
        public uint dwRequestID;
        public uint dwObjectID;
        public uint dwDefineID;
        public uint dwFlags;
        public uint dwentrynumber;
        public uint dwoutof;
        public uint dwDefineCount;
        public object[] dwData;
    }

    public class SIMCONNECT_RECV_CLIENT_DATA : SIMCONNECT_RECV_SIMOBJECT_DATA
    {
    }
}
