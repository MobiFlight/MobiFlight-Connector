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
            var msgEventLabel = e.GetMsgEventLabel();

            if (!inputCache.ContainsKey(inputKey))
            {
                inputCache[inputKey] = GetMatchingInputConfigs(e);
            }

            if (inputCache[inputKey].Count == 0)
            {                
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

                    Log.Instance.log($"{e.Name} => Executing \"{cfg.Name}\". ({e.GetEventActionLabel()})", LogSeverity.Info);

                    cfg.RawValue = e.GetEventActionLabel();
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

        private List<InputConfigItem> GetMatchingInputConfigs(InputEventArgs e)
        {
            var result = new List<InputConfigItem>();

            foreach (var cfg in _configItems.Where(c => c is InputConfigItem).Cast<InputConfigItem>())
            {
                try
                {
                    // item currently created and not saved yet.
                    if (cfg == null) continue;

                    if (!MatchesControllerAndDeviceName(cfg, e))
                        continue;

                    if (ShouldSkipDueToInputShiftRegisterPinMismatch(cfg, e))
                        continue;

                    if (ShouldSkipDueToInputMultiplexerPinMismatch(cfg, e))
                        continue;

                    result.Add(cfg);
                }
                catch (Exception ex)
                {
                    // probably the last row with no settings object 
                    continue;
                }
            }

            return result;
        }

        internal static bool MatchesControllerAndDeviceName(InputConfigItem cfg, InputEventArgs e)
        {
            if (cfg.ModuleSerial == null)
                return false;

            bool serialMatches = cfg.ModuleSerial.Contains("/ " + e.Serial);
            if (!serialMatches)
                return false;

            bool deviceNameMatches = cfg.DeviceName == e.DeviceId;
            
            // For backward compatibility we have to make this check
            // because we used to have the label in the config
            // but now we want to store the internal button identifier
            // so that the label can change any time without breaking the config
            bool isJoystickWithLabelMatch = Joystick.IsJoystickSerial(cfg.ModuleSerial) && cfg.DeviceName == e.DeviceLabel;
            
            return deviceNameMatches || isJoystickWithLabelMatch;
        }

        internal static bool ShouldSkipDueToInputShiftRegisterPinMismatch(InputConfigItem cfg, InputEventArgs e)
        {
            // Input shift registers have an additional check to see if the pin that changed matches the pin
            // assigned to the row. If not just skip this row. Without this every row that uses the input shift register
            // would get added to the input cache and fired even though the pins don't match.
            // Only perform this check if the config's DeviceType is actually InputShiftRegister
            bool isButtonEvent = e.Type == DeviceType.Button;
            bool isInputShiftRegisterConfig = cfg.DeviceType == InputConfigItem.TYPE_INPUT_SHIFT_REGISTER;
            bool hasInputShiftRegisterConfig = cfg.inputShiftRegister != null;
            bool pinMismatch = cfg.inputShiftRegister?.ExtPin != e.ExtPin;

            return isButtonEvent && isInputShiftRegisterConfig && hasInputShiftRegisterConfig && pinMismatch;
        }

        internal static bool ShouldSkipDueToInputMultiplexerPinMismatch(InputConfigItem cfg, InputEventArgs e)
        {
            // Similarly for digital input Multiplexer
            // Only perform this check if the config's DeviceType is actually InputMultiplexer
            bool isButtonEvent = e.Type == DeviceType.Button;
            bool isInputMultiplexerConfig = cfg.DeviceType == InputConfigItem.TYPE_INPUT_MULTIPLEXER;
            bool hasInputMultiplexerConfig = cfg.inputMultiplexer != null;
            bool pinMismatch = cfg.inputMultiplexer?.DataPin != e.ExtPin;

            return isButtonEvent && isInputMultiplexerConfig && hasInputMultiplexerConfig && pinMismatch;
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
    }
}
