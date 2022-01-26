using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight.InputConfig
{
    public class InputActionExecutionCache
    {
        Dictionary<int, InputEventArgs> LastInputActionEventArgs = new Dictionary<int, InputEventArgs>();

        public void Clear() {
            LastInputActionEventArgs.Clear();
        }

        public bool Execute(
            InputAction action, 
            FSUIPC.FSUIPCCacheInterface fsuipcCache,
            SimConnectMSFS.SimConnectCacheInterface simConnectCache,
            MobiFlightCacheInterface moduleCache,
            InputEventArgs args,
            List<ConfigRefValue> configRefsInputEventArgs)
        {
            if (action == null) return false;

            int HashKey = action.GetHashCode();
            if (LastInputActionEventArgs.Keys.Contains(HashKey))
            {
                if (LastInputActionEventArgs[HashKey].StrValue == args.StrValue)
                {
                    return false;
                }
            }

            LastInputActionEventArgs[HashKey] = args;

            action.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefsInputEventArgs);

            return true;
        }
    }
}
