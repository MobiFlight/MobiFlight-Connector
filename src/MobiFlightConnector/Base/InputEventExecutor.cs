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

            if (!inputCache.ContainsKey(inputKey))
            {
                inputCache[inputKey] = GetMatchingInputConfigs(e);
            }

            if (inputCache[inputKey].Count == 0)
            {
                return updatedValues;
            }

            var cacheCollection = CreateCacheCollection();

            foreach (var cfg in inputCache[inputKey])
            {
                // A HOLD/REPEAT/LONG_RELEASE only applies to a config whose own delay produced it.
                if (cfg.button != null && !cfg.button.MatchesSyntheticDelay(e))
                {
                    continue;
                }

                string dispatchedLabel;
                string logLabel;
                if (e.InputType == DeviceType.Button && cfg.button != null)
                {
                    var dispatchedValue = cfg.button.ResolveDispatchedEvent(e);
                    dispatchedLabel = e.GetEventActionLabel((int)dispatchedValue);
                    logLabel = AppendSyntheticDelay(dispatchedLabel, dispatchedValue, e, cfg.button);
                }
                else
                {
                    dispatchedLabel = e.GetEventActionLabel();
                    logLabel = dispatchedLabel;
                }

                var hasMatchingAction = cfg.GetInputAction(e) != null;

                var isSyntheticEvent = e.SyntheticDelayMs.HasValue;
                if (!hasMatchingAction && isSyntheticEvent)
                {
                    continue;
                }

                var cfgEventLabel = $"{e.Controller.Name} => {e.Device.Label} => {logLabel}";

                if (!isStarted)
                {
                    if (hasMatchingAction)
                    {
                        Log.Instance.log($"{cfgEventLabel} => Skipping \"{cfg.Name}\", MobiFlight not running.", LogSeverity.Warn);
                    }
                    continue;
                }

                if (!cfg.Active)
                {
                    if (hasMatchingAction)
                    {
                        Log.Instance.log($"{cfgEventLabel} => Skipping inactive config \"{cfg.Name}\".", LogSeverity.Warn);
                    }
                    continue;
                }

                try
                {
                    if (!CheckPreconditions(cfg))
                    {
                        if (hasMatchingAction)
                        {
                            Log.Instance.log($"{cfgEventLabel} => Preconditions not satisfied for \"{cfg.Name}\".", LogSeverity.Debug);
                        }
                        continue;
                    }

                    cfg.RawValue = dispatchedLabel;
                    cfg.Value = " ";
                    updatedValues[cfg.GUID] = cfg;

                    if (!hasMatchingAction)
                    {
                        continue;
                    }

                    Log.Instance.log($"{e.Controller.Name} => Executing \"{cfg.Name}\". ({logLabel})", LogSeverity.Info);

                    var references = ResolveReferences(cfg.ConfigRefs);
                    var modifiableValue = new ConnectorValue()
                    {
                        type = FSUIPCOffsetType.Float,
                        Float64 = e.Value,
                    };

                    try
                    {
                        foreach (var modifier in cfg.Modifiers.Items.Where(m => m.Active))
                        {
                            modifiableValue = modifier.Apply(modifiableValue, references);
                        }

                        cfg.Value = modifiableValue.ToString();
                        e.Value = modifiableValue.Float64;
                        e.StrValue = modifiableValue.type == FSUIPCOffsetType.String ? modifiableValue.String : null;
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.log($"Transform error ({cfg.Name}): {ex.Message}", LogSeverity.Error);
                        cfg.Status[ConfigItemStatusType.Modifier] = ex.Message;
                    }

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
            var result = e.Controller.Serial + e.Device.Type + e.Device.Name;
            return result;
        }

        /// <summary>":Nms" suffix for the log only - the configured delay/setting that produced this synthetic event.</summary>
        private static string AppendSyntheticDelay(string label, MobiFlightButton.InputEvent value, InputEventArgs e, ButtonInputConfig button)
        {
            switch (value)
            {
                case MobiFlightButton.InputEvent.HOLD:
                case MobiFlightButton.InputEvent.REPEAT:
                    return e.SyntheticDelayMs.HasValue ? $"{label}:{e.SyntheticDelayMs}ms" : label;
                case MobiFlightButton.InputEvent.LONG_RELEASE:
                    return $"{label}:{button.LongReleaseDelay}ms";
                default:
                    return label;
            }
        }

        /// <summary>
        /// The distinct Hold/Repeat delay settings among configs bound to this button - but also,
        /// implicitly, "should this button be tracked at all." Empty means the generator won't track
        /// this press (see SyntheticButtonEventGenerator.Observe) - no HOLD/REPEAT ever, AND no
        /// HeldDurationMs on RELEASE (needed for LONG_RELEASE - see
        /// ButtonInputConfig.ResolveDispatchedEvent). So a config bound with only onLongRelease (no
        /// onHold) still needs an entry here to keep the button tracked, even though it contributes
        /// nothing real to scheduling - it gets the ButtonTimings.NoHold sentinel instead, same as any
        /// other config with no onHold. Active/precondition gating happens in Execute(), not here -
        /// this only decides which delays govern this press. No config identity is carried (see
        /// SyntheticButtonEventGenerator.ResolveTimings) - two configs sharing a delay collapse to one
        /// entry.
        /// </summary>
        public List<ButtonTimings> ResolveButtonTimingsPerConfig(InputEventArgs e)
        {
            string inputKey = CreateInputKey(e);
            if (!inputCache.ContainsKey(inputKey))
            {
                inputCache[inputKey] = GetMatchingInputConfigs(e);
            }

            return inputCache[inputKey]
                .Where(cfg => cfg.button != null && (cfg.button.onHold != null || cfg.button.onLongRelease != null))
                .Select(cfg => cfg.button.onHold != null
                    ? new ButtonTimings(cfg.button.HoldDelay, cfg.button.RepeatDelay)
                    : new ButtonTimings(ButtonTimings.NoHold, 0))
                .Distinct()
                .ToList();
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
            if (cfg.Controller == null)
                return false;

            bool serialMatches = cfg.Controller.Serial == e.Controller.Serial;
            if (!serialMatches)
                return false;

            bool deviceNameMatches = cfg.Device.Name == e.Device.Name;

            // For backward compatibility we have to make this check
            // because we used to have the label in the config
            // but now we want to store the internal button identifier
            // so that the label can change any time without breaking the config
            bool isJoystickWithLabelMatch = Joystick.IsJoystickSerial(cfg.Controller.Serial) && cfg.Device.Name == e.Device.Label;

            return deviceNameMatches || isJoystickWithLabelMatch;
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