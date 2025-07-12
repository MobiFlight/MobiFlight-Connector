using MobiFlight.Base;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.ProSim;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Execution
{
    public class InputEventExecutor
    {
        private readonly List<IConfigItem> _configItems;
        private readonly InputActionExecutionCache _inputActionExecutionCache;
        private readonly FSUIPCCacheInterface _fsuipcCache;
        private readonly SimConnectCacheInterface _simConnectCache;
        private readonly XplaneCacheInterface _xplaneCache;
        private readonly MobiFlightCache _mobiFlightCache;
        private readonly ProSim.ProSimCacheInterface _proSimCache;
        private readonly JoystickManager _joystickManager;
        private readonly ArcazeCache _arcazeCache;
        private readonly Dictionary<string, List<InputConfigItem>> inputCache = new Dictionary<string, List<InputConfigItem>>();

        public InputEventExecutor(
            List<IConfigItem> configItems,
            InputActionExecutionCache inputActionExecutionCache,
            FSUIPCCacheInterface fsuipcCache,
            SimConnectCacheInterface simConnectCache,
            XplaneCacheInterface xplaneCache,
            MobiFlightCache mobiFlightCache,
            ProSim.ProSimCacheInterface proSimCache,
            JoystickManager joystickManager,
            ArcazeCache arcazeCache)
        {
            _configItems = configItems;
            _inputActionExecutionCache = inputActionExecutionCache;
            _fsuipcCache = fsuipcCache;
            _simConnectCache = simConnectCache;
            _xplaneCache = xplaneCache;
            _mobiFlightCache = mobiFlightCache;
            _joystickManager = joystickManager;
            _arcazeCache = arcazeCache;
            _proSimCache = proSimCache;
        }

        public void ClearCache()
        {
            inputCache.Clear();
        }

        public Dictionary<string, IConfigItem> Execute(InputEventArgs e, bool isStarted)
        {
            var updatedValues = new Dictionary<string, IConfigItem>();
            string inputKey = CreateInputKey(e);
            string eventAction = GetEventAction(e);
            var msgEventLabel = $"{e.Name} => {e.DeviceLabel} {(e.ExtPin.HasValue ? $":{e.ExtPin}" : "")} => {eventAction}";

            if (!inputCache.ContainsKey(inputKey))
            {
                inputCache[inputKey] = GetMatchingInputConfigs(e);
            }

            if (inputCache[inputKey].Count == 0)
            {
                if (LogIfNotJoystickOrJoystickAxisEnabled(e.Serial, e.Type))
                {
                    Log.Instance.log($"{msgEventLabel} =>  No config found.", LogSeverity.Warn);
                }
                
                return updatedValues;
            }

            var cacheCollection = CreateCacheCollection();

            if (!isStarted)
            {
                Log.Instance.log($"{msgEventLabel} skipping, MobiFlight not running.", LogSeverity.Warn);
                return updatedValues;
            }

            foreach (var cfg in inputCache[inputKey])
            {
                if (!cfg.Active)
                {
                    Log.Instance.log($"{msgEventLabel} => Skipping inactive config \"{cfg.Name}\".", LogSeverity.Warn);
                    continue;
                }

                try
                {
                    if (!CheckPreconditions(cfg))
                    {
                        Log.Instance.log($"{msgEventLabel} => Preconditions not satisfied for \"{cfg.Name}\".", LogSeverity.Debug);
                        continue;
                    }

                    Log.Instance.log($"{e.Name} => Executing \"{cfg.Name}\". ({eventAction})", LogSeverity.Info);

                    cfg.RawValue = eventAction;
                    cfg.Value = " ";
                    updatedValues[cfg.GUID] = cfg;
                    var references = ResolveReferences(cfg.ConfigRefs);
                    cfg.execute(cacheCollection, e, references);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Error executing \"{cfg.Name}\": {ex.Message}", LogSeverity.Error);
                    cfg.Status[ConfigItemStatusType.Device] = "DEVICE_ERROR";
                }
            }

            return updatedValues;
        }

        private string CreateInputKey(InputEventArgs e)
        {
            var result = e.Serial + e.Type + e.DeviceId;
            if (e.ExtPin.HasValue)
            {
                result += e.ExtPin.Value.ToString();
            }
            return result;
        }

        private string GetEventAction(InputEventArgs e)
        {
            switch (e.Type)
            {
                case DeviceType.Button:
                    return MobiFlightButton.InputEventIdToString(e.Value);
                case DeviceType.Encoder:
                    return MobiFlightEncoder.InputEventIdToString(e.Value);
                case DeviceType.InputShiftRegister:
                    return MobiFlightInputShiftRegister.InputEventIdToString(e.Value);
                case DeviceType.InputMultiplexer:
                    return MobiFlightInputMultiplexer.InputEventIdToString(e.Value);
                case DeviceType.AnalogInput:
                    return $"{MobiFlightAnalogInput.InputEventIdToString(0)} => {e.Value}";
                default:
                    return "n/a";
            }
        }

        private List<InputConfigItem> GetMatchingInputConfigs(InputEventArgs e)
        {
            var result = new List<InputConfigItem>();

            foreach (var cfg in _configItems.Where(c => c is InputConfigItem).Cast<InputConfigItem>())
            {
                try
                {
                    // item currently created and not saved yet.
                    if (cfg == null) continue;

                    if (cfg.ModuleSerial != null &&
                        cfg.ModuleSerial.Contains("/ " + e.Serial) &&
                       (cfg.DeviceName == e.DeviceId ||
                       // for backward compatibility we have to make this check
                       // because we used to have the label in the config
                       // but now we want to store the internal button identifier
                       // so that the label can change any time without breaking the config
                       (Joystick.IsJoystickSerial(cfg.ModuleSerial) && cfg.DeviceName == e.DeviceLabel)))
                    {
                        // Input shift registers have an additional check to see if the pin that changed matches the pin
                        // assigned to the row. If not just skip this row. Without this every row that uses the input shift register
                        // would get added to the input cache and fired even though the pins don't match.
                        //GCC CHECK
                        if (e.Type == DeviceType.InputShiftRegister && cfg.inputShiftRegister != null && cfg.inputShiftRegister.ExtPin != e.ExtPin)
                        {
                            continue;
                        }
                        // similarly also for digital input Multiplexer
                        if (e.Type == DeviceType.InputMultiplexer && cfg.inputMultiplexer != null && cfg.inputMultiplexer.DataPin != e.ExtPin)
                        {
                            continue;
                        }
                        result.Add(cfg);
                    }
                }
                catch (Exception ex)
                {
                    // probably the last row with no settings object 
                    continue;
                }
            }

            return result;
        }

        private bool CheckPreconditions(InputConfigItem cfg)
        {
            var currentValue = new ConnectorValue();
            return PreconditionChecker.CheckPrecondition(cfg, currentValue, _configItems, _arcazeCache, _mobiFlightCache);
        }

        private List<ConfigRefValue> ResolveReferences(ConfigRefList configRefs)
        {
            List<ConfigRefValue> result = new List<ConfigRefValue>();
            foreach (ConfigRef c in configRefs)
            {
                if (!c.Active) continue;
                String s = FindValueForRef(c.Ref);
                if (s == null) continue;
                result.Add(new ConfigRefValue(c, s));
            }
            return result;
        }

        private String FindValueForRef(String refId)
        {
            String result = null;
            foreach (var cfg in _configItems)
            {
                if (cfg.GUID != refId) continue;

                if (!cfg.Active) break;

                if (cfg.Value == null) break;

                string value = cfg.Value;

                if (value == "") break;
                result = value;
            }
            return result;
        }

        private CacheCollection CreateCacheCollection()
        {
            return new CacheCollection
            {
                fsuipcCache = _fsuipcCache,
                simConnectCache = _simConnectCache,
                xplaneCache = _xplaneCache,
                moduleCache = _mobiFlightCache,
                proSimCache = _proSimCache,
                joystickManager = _joystickManager
            };
        }

        private bool LogIfNotJoystickOrJoystickAxisEnabled(String Serial, DeviceType type)
        {
            return !Joystick.IsJoystickSerial(Serial) ||
                    (Joystick.IsJoystickSerial(Serial) && (type != DeviceType.AnalogInput || Log.Instance.LogJoystickAxis));
        }
    }
}
