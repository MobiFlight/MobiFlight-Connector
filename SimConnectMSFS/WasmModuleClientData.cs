using System;

namespace MobiFlight.SimConnectMSFS
{
    public class WasmModuleClientData
    {
        public string NAME;
        public Enum AREA_SIMVAR_ID;
        public Enum AREA_COMMAND_ID;
        public Enum AREA_RESPONSE_ID;
        public SIMCONNECT_DEFINE_ID DATA_DEFINITION_ID;
        public uint RESPONSE_OFFSET;
    }
}
