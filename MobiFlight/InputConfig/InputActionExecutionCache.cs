using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;

namespace MobiFlight.InputConfig
{
    public class InputActionExecutionCache
    {
        Dictionary<int, InputEventArgs> LastInputActionEventArgs = new Dictionary<int, InputEventArgs>();

        private static bool ShouldSkipExecution(InputEventArgs previousArgs, InputEventArgs currentArgs)
        {
            if (previousArgs == null || currentArgs == null)
            {
                return false;
            }

            // We are not always guaranteed to have StrValue, so we must check if it is / was a string value (non-null)
            // if so, we should use that for comparison
            var hasStringValue = !string.IsNullOrEmpty(previousArgs.StrValue) || !string.IsNullOrEmpty(currentArgs.StrValue);
            if (hasStringValue)
            {
                return previousArgs.StrValue == currentArgs.StrValue;
            }

            // Otherwise check the underlying raw value to see if a change has occurred
            return previousArgs.Value == currentArgs.Value;
        }

        public void Clear() {
            LastInputActionEventArgs.Clear();
        }

        public bool Execute(
            ButtonInputConfig config, 
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefsInputEventArgs)
        {

            if (config == null) return false;

            int HashKey = config.GetHashCode();

            if (LastInputActionEventArgs.Keys.Contains(HashKey))
            {
                if (ShouldSkipExecution(LastInputActionEventArgs[HashKey], args))
                {
                    return false;
                }
            }

            // We need to clone before mutation; otherwise the cached "last input" is mutated too, which can break dedupe comparisons
            LastInputActionEventArgs[HashKey] = (InputEventArgs)args.Clone();

            if (args.Value == 1)
            {
                args.Value = (int)MobiFlightButton.InputEvent.PRESS;
            }
            else
            {
                args.Value = (int)MobiFlightButton.InputEvent.RELEASE;
            }
            config.execute(cacheCollection, args, configRefsInputEventArgs);
           
            return true;
        }

        public bool Execute(
            AnalogInputConfig config,
            CacheCollection cacheCollection,
            InputEventArgs args,
            List<ConfigRefValue> configRefsInputEventArgs)
        {
            if (config?.onChange == null) return false;

            int HashKey = config.onChange.GetHashCode();
            if (LastInputActionEventArgs.Keys.Contains(HashKey))
            {
                if (ShouldSkipExecution(LastInputActionEventArgs[HashKey], args))
                {
                    return false;
                }
            }

            LastInputActionEventArgs[HashKey] = (InputEventArgs)args.Clone();

            config.onChange.execute(cacheCollection, args, configRefsInputEventArgs);

            return true;
        }
    }
}
