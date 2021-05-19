using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.SimConnectMSFS
{

    static class WasmModuleClient
    {
        public static void Stop(SimConnect simConnect)
        {
            SendWasmCmd(simConnect, "MF.SimVars.Clear");
        }
        public static void SendWasmCmd(SimConnect simConnect, String command)
        {
            if (simConnect == null) return;

            simConnect.SetClientData(
                SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD,
               (SIMCONNECT_CLIENT_DATA_ID)0,
               SIMCONNECT_CLIENT_DATA_SET_FLAG.DEFAULT, 0,
               new ClientDataString(command)
            );
        }
    }
}
