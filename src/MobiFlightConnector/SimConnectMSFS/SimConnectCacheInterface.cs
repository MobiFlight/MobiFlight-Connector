using System;

namespace MobiFlight.SimConnectMSFS
{
    public interface SimConnectCacheInterface : Base.CacheInterface, Base.WriteCacheInterface
    {
        event EventHandler LVarListUpdated;
        event EventHandler<string> AircraftPathChanged;
        void Start();
        void Stop();
        void RefreshLVarsList();
        void SetHandle(IntPtr handle);
        void SetSimVar(String SimVarCode);
        void ReceiveSimConnectMessage();
        FSUIPCOffsetType GetSimVar(string simVarName, out string stringVal, out double floatVal);        
    }
}