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
        public static void Ping(SimConnect simConnect, WasmModuleClientData clientData)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, "MF.Ping", clientData);
            DummyCommand(simConnect, clientData);
        }

        public static void Stop(SimConnect simConnect, WasmModuleClientData clientData)
        {
            if (simConnect == null) return;
               
            SendWasmCmd(simConnect, "MF.SimVars.Clear", clientData);
        }

        public static void GetLVarList(SimConnect simConnect, WasmModuleClientData clientData)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, "MF.LVars.List", clientData);
            DummyCommand(simConnect, clientData);
        }

        public static void DummyCommand(SimConnect simConnect, WasmModuleClientData clientData)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, "MF.DummyCmd", clientData);
        }

        public static void SendWasmCmd(SimConnect simConnect, String command, WasmModuleClientData clientData)
        {
            if (simConnect == null) return;

            simConnect.SetClientData(
               clientData.AREA_COMMAND_ID,
               (SIMCONNECT_DEFINE_ID)clientData.DATA_DEFINITION_ID,
               SIMCONNECT_CLIENT_DATA_SET_FLAG.DEFAULT,
               0,
               new ClientDataString(command)
            );
        }

        public static void SetConfig(SimConnect simConnect, String ConfigName, String ConfigValue, WasmModuleClientData clientData)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, $"MF.Config.{ConfigName}.Set.{ConfigValue}", clientData);
            DummyCommand(simConnect, clientData);
        }

        public static void AddAdditionalClient(SimConnect simConnect, String clientName, WasmModuleClientData clientData)
        {
            WasmModuleClient.DummyCommand(simConnect, clientData);
            WasmModuleClient.SendWasmCmd(simConnect, $"MF.Clients.Add.{clientName}", clientData);
        }

        public static void SetSimVar(SimConnect simConnect, String SimVarCode, WasmModuleClientData clientData)
        {
            WasmModuleClient.SendWasmCmd(simConnect, "MF.SimVars.Set." + SimVarCode, clientData);
            WasmModuleClient.DummyCommand(simConnect, clientData);
        }

        public static void AddSimVar(SimConnect simConnect, String SimVarName, WasmModuleClientData clientData)
        {
            WasmModuleClient.SendWasmCmd(simConnect, "MF.SimVars.Add." + SimVarName, clientData);
        }


    }
}
