using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Execution
{
    public static class PreconditionChecker
    {
        public static bool CheckPrecondition(
            IConfigItem cfg, 
            ConnectorValue currentValue, 
            List<IConfigItem> configItems, 
            ArcazeCache arcazeCache, 
            MobiFlightCacheInterface mobiFlightCache
        )
        {
            bool finalResult = true;
            bool result = true;
            bool logicOr = false; // false:and true:or

            foreach (Precondition p in cfg.Preconditions)
            {
                if (!p.PreconditionActive)
                {
                    continue;
                }

                switch (p.PreconditionType)
                {
#if ARCAZE
                    case "pin":
                        string serial = SerialNumber.ExtractSerial(p.PreconditionSerial);
                        string val = arcazeCache.getValue(serial, p.PreconditionPin, "repeat");

                        result = p.Evaluate(val, currentValue);
                        break;
#endif
                    case "variable":
                        var variableValue = mobiFlightCache.GetMobiFlightVariable(p.PreconditionRef);
                        if (variableValue == null) break;

                        result = p.Evaluate(variableValue);
                        break;
                    case "config":
                        // iterate over the config row by row
                        foreach (var outputConfig in configItems)
                        {
                            // here we just don't have a match
                            if (outputConfig.GUID != p.PreconditionRef) continue;

                            // if inactive ignore?
                            if (!outputConfig.Active) break;

                            // was there an error on reading the value?
                            if (outputConfig.Value == null) break;

                            // read the value
                            string value = outputConfig.Value;

                            // if there hasn't been determined any value yet
                            // we cannot compare
                            if (value == "") break;

                            result = p.Evaluate(value, currentValue);
                            break;
                        }
                        break;
                } // switch

                if (logicOr)
                {
                    finalResult |= result;
                }
                else
                {
                    finalResult &= result;
                }

                logicOr = (p.PreconditionLogic == "or");
            } // foreach

            return finalResult;
        }
    }
}
