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
            ButtonInputConfig config, 
            FSUIPC.FSUIPCCacheInterface fsuipcCache,
            SimConnectMSFS.SimConnectCacheInterface simConnectCache,
            MobiFlightCacheInterface moduleCache,
            InputEventArgs args,
            List<ConfigRefValue> configRefsInputEventArgs)
        {

            if (config == null) return false;

            int HashKey = config.GetHashCode();

            if (LastInputActionEventArgs.Keys.Contains(HashKey))
            {
                if (LastInputActionEventArgs[HashKey].StrValue == args.StrValue)
                {
                    return false;
                }
            }

            LastInputActionEventArgs[HashKey] = args;

            if (args.Value==1)
            {
                config.onPress.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefsInputEventArgs);
            } else
            {
                config.onRelease.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefsInputEventArgs);
            }

            return true;
        }

        public bool Execute(
            AnalogInputConfig config,
            FSUIPC.FSUIPCCacheInterface fsuipcCache,
            SimConnectMSFS.SimConnectCacheInterface simConnectCache,
            MobiFlightCacheInterface moduleCache,
            InputEventArgs args,
            List<ConfigRefValue> configRefsInputEventArgs)
        {
            if (config?.onChange == null) return false;

            int HashKey = config.onChange.GetHashCode();
            if (LastInputActionEventArgs.Keys.Contains(HashKey))
            {
                if (LastInputActionEventArgs[HashKey].StrValue == args.StrValue)
                {
                    return false;
                }
            }

            LastInputActionEventArgs[HashKey] = args;

            config.onChange.execute(fsuipcCache, simConnectCache, moduleCache, args, configRefsInputEventArgs);

            return true;
        }
    }
}
