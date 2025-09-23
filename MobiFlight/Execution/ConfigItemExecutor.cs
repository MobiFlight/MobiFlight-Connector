using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MobiFlight.Base;
using MobiFlight.BrowserMessages;
using MobiFlight.FSUIPC;
using MobiFlight.InputConfig;
using MobiFlight.OutputConfig;
using MobiFlight.ProSim;
using MobiFlight.SimConnectMSFS;
using MobiFlight.xplane;

namespace MobiFlight.Execution
{
    public class ConfigItemExecutor
    {
#if ARCAZE
        private readonly ArcazeCache arcazeCache;
#endif
        private readonly FSUIPCCacheInterface fsuipcCache;
        private readonly SimConnectCacheInterface simConnectCache;
        private readonly XplaneCacheInterface xplaneCache;
        private readonly MobiFlightCacheInterface mobiFlightCache;
        private readonly ProSimCacheInterface proSimCache;
        private readonly JoystickManager joystickManager;
        private readonly MidiBoardManager midiBoardManager;
        private readonly InputActionExecutionCache inputActionExecutionCache;
        private readonly List<IConfigItem> ConfigItems;
        private readonly OutputConfigItem ConfigItemInTestMode;

        public ConfigItemExecutor(
            List<IConfigItem> configItems,
#if ARCAZE
            ArcazeCache arcazeCache,
#endif
            FSUIPCCacheInterface fsuipcCache,
            SimConnectCacheInterface simConnectCache,
            XplaneCacheInterface xplaneCache,
            MobiFlightCacheInterface mobiFlightCache,
            ProSimCacheInterface proSimCache,
            JoystickManager joystickManager,
            MidiBoardManager midiBoardManager,
            InputActionExecutionCache inputActionExecutionCache,
            OutputConfigItem configItemInTestMode)
        {
#if ARCAZE
            this.arcazeCache = arcazeCache;
#endif
            this.ConfigItems = configItems;
            this.fsuipcCache = fsuipcCache;
            this.simConnectCache = simConnectCache;
            this.xplaneCache = xplaneCache;
            this.proSimCache = proSimCache;
            this.mobiFlightCache = mobiFlightCache;
            this.joystickManager = joystickManager;
            this.midiBoardManager = midiBoardManager;
            this.inputActionExecutionCache = inputActionExecutionCache;
            this.ConfigItemInTestMode = configItemInTestMode;
        }

        public void Execute(OutputConfigItem cfg, ConcurrentDictionary<string, IConfigItem> updatedValues)
        {
            if (!cfg.Active) return;

            var originalCfg = cfg.Clone() as ConfigItem;

            // Handle test mode
            if (ConfigItemInTestMode != null && ConfigItemInTestMode.GUID == cfg.GUID)
            {
                cfg.Status[ConfigItemStatusType.Test] = "TESTING";
                if (!cfg.Equals(originalCfg))
                {
                    updatedValues[cfg.GUID] = cfg;
                }
                return;
            }

            // If not connected to FSUIPC show an error message
            if (cfg.Source is FsuipcSource && !fsuipcCache.IsConnected())
            {
                cfg.Status[ConfigItemStatusType.Source] = "FSUIPC_NOT_AVAILABLE";
            }
            else if (cfg.Source is SimConnectSource && !simConnectCache.IsConnected())
            {
                cfg.Status[ConfigItemStatusType.Source] = "SIMCONNECT_NOT_AVAILABLE";
            }
            else if (cfg.Source is XplaneSource && !xplaneCache.IsConnected())
            {
                cfg.Status[ConfigItemStatusType.Source] = "XPLANE_NOT_AVAILABLE";
            }
            else if (cfg.Source is ProSimSource && !proSimCache.IsConnected())
            {
                cfg.Status[ConfigItemStatusType.Source] = "PROSIM_NOT_AVAILABLE";
            }
            else
            {
                cfg.Status.Remove(ConfigItemStatusType.Source);
            }

            ConnectorValue value = ExecuteRead(cfg);
            ConnectorValue processedValue = value;

            cfg.RawValue = value.ToString();
            cfg.Value = processedValue.ToString();

            List<ConfigRefValue> configRefs = GetRefs(cfg.ConfigRefs);
            cfg.Status.Remove(ConfigItemStatusType.Modifier);

            try
            {
                foreach (var modifier in cfg.Modifiers.Items.Where(m => m.Active))
                {
                    processedValue = modifier.Apply(processedValue, configRefs);
                }

                cfg.Value = processedValue.ToString();
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Transform error ({cfg.Name}): {ex.Message}", LogSeverity.Error);
                cfg.Status[ConfigItemStatusType.Modifier] = ex.Message;
                if (!cfg.Equals(originalCfg))
                {
                    updatedValues[cfg.GUID] = cfg;
                }

                // We used to `continue` here
                // but we want to evaluate the Precondition
                // so keep continuing
            }

            try
            {
                var precondition = true;
                if (!PreconditionChecker.CheckPrecondition(cfg, processedValue, ConfigItems, arcazeCache, mobiFlightCache))
                {
                    if (!cfg.Preconditions.ExecuteOnFalse)
                    {
                        cfg.Status[ConfigItemStatusType.Precondition] = "not satisfied";
                        if (!cfg.Equals(originalCfg))
                        {
                            updatedValues[cfg.GUID] = cfg;
                        }
                        precondition = false;
                    }
                    else
                    {
                        processedValue.type = FSUIPCOffsetType.String;
                        processedValue.String = cfg.Preconditions.FalseCaseValue;
                    }
                }

                if (precondition)
                {
                    cfg.Status.Remove(ConfigItemStatusType.Precondition);
                    ExecuteDisplay(processedValue.ToString(), cfg);
                }
            }
            catch (JoystickNotConnectedException jEx)
            {
                // TODO: REDESIGN: Review
                // row.ErrorText = jEx.Message;
                cfg.Status[ConfigItemStatusType.Device] = "NotConnected";
            }
            catch (MidiBoardNotConnectedException mEx)
            {
                // TODO: REDESIGN: Review
                // row.ErrorText = mEx.Message;
                cfg.Status[ConfigItemStatusType.Device] = "NotConnected";
            }
            catch (Exception exc)
            {
                Log.Instance.log($"Error during execution: {cfg.Name}. {exc.Message}", LogSeverity.Error);
                cfg.Status[ConfigItemStatusType.Device] = "NotConnected";
                throw new ConfigErrorException(cfg.Name + ". " + exc.Message, exc);
            }

            if (!originalCfg.Equals(cfg))
            {
                updatedValues[cfg.GUID] = cfg;
            }
        }

        public void ExecuteTestOn(OutputConfigItem cfg, ConnectorValue value = null)
        {
            if (cfg.DeviceType == null) return;

            switch (cfg.DeviceType)
            {
                case MobiFlightStepper.TYPE:
                    var stepper = cfg.Device as OutputConfig.Stepper;
                    ExecuteDisplay(value?.ToString() ?? stepper.TestValue.ToString(), cfg);
                    break;

                case MobiFlightServo.TYPE:
                    var servo = cfg.Device as OutputConfig.Servo;
                    ExecuteDisplay(value?.ToString() ?? servo.Max, cfg);
                    break;

                case ArcazeLedDigit.TYPE:
                case OutputConfig.LcdDisplay.DeprecatedType:
                    ExecuteDisplay(value?.ToString() ?? "1234567890", cfg);
                    break;

                case MobiFlightShiftRegister.TYPE:
                    ExecuteDisplay(value?.ToString() ?? "1", cfg);
                    break;

                case MobiFlightCustomDevice.TYPE:
                    ExecuteDisplay(value?.ToString() ?? "1", cfg);
                    break;

                case "InputAction":
                    // Do nothing for the InputAction
                    break;

                default:
                    ExecuteDisplay(value?.ToString() ?? "255", cfg);
                    break;
            }

            cfg.Status[ConfigItemStatusType.Test] = "TEST_EXECUTION";
            MessageExchange.Instance.Publish(new ConfigValuePartialUpdate(cfg));
        }

        public void ExecuteTestOff(OutputConfigItem cfg)
        {
            if (cfg == null || cfg.DeviceType == null) return;

            OutputConfigItem offCfg = (OutputConfigItem)cfg.Clone();

            switch (offCfg.DeviceType)
            {
                case MobiFlightServo.TYPE:
                    var servo = offCfg.Device as Servo;
                    ExecuteDisplay(servo.Min, offCfg);
                    break;

                case OutputConfig.LcdDisplay.DeprecatedType:
                    var lcdDisplay = offCfg.Device as LcdDisplay;
                    lcdDisplay.Lines.Clear();
                    lcdDisplay.Lines.Add(new string(' ', 20 * 4));
                    ExecuteDisplay(new string(' ', 20 * 4), offCfg);
                    break;

                case MobiFlightShiftRegister.TYPE:
                    ExecuteDisplay("0", offCfg);
                    break;

                case "InputAction":
                    // Do nothing for the InputAction
                    break;

                case MobiFlightOutput.TYPE:
                    ExecuteDisplay("0", offCfg);
                    break;

                case MobiFlightLedModule.TYPE:
                    var ledModule = offCfg.Device as LedModule;
                    ledModule.DisplayLedDecimalPoints = new List<string>();
                    ExecuteDisplay("        ", offCfg);
                    break;
            }

            cfg.Status.Remove(ConfigItemStatusType.Test);
            MessageExchange.Instance.Publish(new ConfigValuePartialUpdate(cfg));
        }

        private ConnectorValue ExecuteRead(OutputConfigItem cfg)
        {
            ConnectorValue result = new ConnectorValue();

            if (cfg.Source is FsuipcSource)
            {
                result = FsuipcHelper.executeRead((cfg.Source as FsuipcSource).FSUIPC, fsuipcCache);
            }
            else if (cfg.Source is VariableSource)
            {
                var source = cfg.Source as VariableSource;
                if (source.MobiFlightVariable.TYPE == MobiFlightVariable.TYPE_NUMBER)
                {
                    result.type = FSUIPCOffsetType.Float;
                    result.Float64 = mobiFlightCache.GetMobiFlightVariable(source.MobiFlightVariable.Name).Number;
                }
                else if (source.MobiFlightVariable.TYPE == MobiFlightVariable.TYPE_STRING)
                {
                    result.type = FSUIPCOffsetType.String;
                    result.String = mobiFlightCache.GetMobiFlightVariable(source.MobiFlightVariable.Name).Text;
                }
            }
            else if (cfg.Source is XplaneSource)
            {
                var source = cfg.Source as XplaneSource;
                result.type = FSUIPCOffsetType.Float;
                result.Float64 = xplaneCache.readDataRef(source.XplaneDataRef.Path);
            }
            else if (cfg.Source is SimConnectSource)
            {
                var source = cfg.Source as SimConnectSource;
                result.type = simConnectCache.GetSimVar(source.SimConnectValue.Value, out result.String, out result.Float64);
            }
            else if (cfg.Source is ProSimSource)
            {
                var source = cfg.Source as ProSimSource;
                result.Float64 = proSimCache.readDataref(source.ProSimDataRef.Path);
            }
            else
            {
                Log.Instance.log("Unknown source type: " + cfg.Source.SourceType, LogSeverity.Error);
            }

            return result;
        }

        private string ExecuteDisplay(string value, OutputConfigItem cfg)
        {
            string serial = SerialNumber.ExtractSerial(cfg.ModuleSerial);

            if (serial == "" && cfg.DeviceType != "InputAction")
                return value.ToString();

            if (SerialNumber.IsJoystickSerial(serial) && cfg.DeviceType != "InputAction")
            {
                Joystick joystick = joystickManager.GetJoystickBySerial(serial);
                if (joystick != null)
                {
                    switch (cfg.DeviceType)
                    {
                        case OutputConfig.LcdDisplay.DeprecatedType:
                            var lcdDisplay = cfg.Device as LcdDisplay;
                            joystick.SetLcdDisplay(lcdDisplay.Address, value);
                            joystick.UpdateOutputDeviceStates();
                            joystick.Update();
                            break;

                        case "-":
                            // do nothing
                            break;

                        default: // LED Output                          
                            byte state = 0;
                            if (value != "0") { 
                                if (!Byte.TryParse(value, out state))
                                {
                                    state = 1;
                                }
                            }
                            joystick.SetOutputDeviceState((cfg.Device as Output).DisplayPin, state);
                            joystick.UpdateOutputDeviceStates();
                            joystick.Update();
                            break;
                    }
                }
                else
                {
                    var joystickName = SerialNumber.ExtractDeviceName(cfg.ModuleSerial);
                    // throw new JoystickNotConnectedException(i18n._tr($"{joystickName} not connected"));
                    return i18n._tr($"{joystickName} not connected");
                }
            }
            else if (SerialNumber.IsMidiBoardSerial(serial) && cfg.DeviceType != "InputAction")
            {
                MidiBoard midiBoard = midiBoardManager.GetMidiBoardBySerial(serial);
                if (midiBoard != null)
                {
                    byte state = 0;
                    if (value != "0") state = 1;
                    midiBoard.SetOutputDeviceState((cfg.Device as Output).DisplayPin, state);
                }
                else
                {
                    var midiBoardName = SerialNumber.ExtractDeviceName(cfg.ModuleSerial);
                    return i18n._tr($"{midiBoardName} not connected");
                    // throw new MidiBoardNotConnectedException(i18n._tr($"{midiBoardName} not connected"));
                }
            }
            else if (serial.IndexOf("SN") != 0 && cfg.DeviceType != "InputAction")
            {
#if ARCAZE
                switch (cfg.DeviceType)
                {
                    case ArcazeLedDigit.TYPE:
                        var device = cfg.Device as LedModule;
                        var val = value.PadRight(device.DisplayLedDigits.Count, device.DisplayLedPaddingChar[0]);
                        if (device.DisplayLedPadding) val = value.PadLeft(device.DisplayLedPadding ? device.DisplayLedDigits.Count : 0, device.DisplayLedPaddingChar[0]);
                        arcazeCache.setDisplay(
                            serial,
                            device.DisplayLedAddress,
                            device.DisplayLedConnector,
                            device.DisplayLedDigits,
                            device.DisplayLedDecimalPoints,
                            val);
                        break;

                    default:
                        arcazeCache.setValue(serial,
                            (cfg.Device as Output).DisplayPin,
                            (value != "0" ? (cfg.Device as Output).DisplayPinBrightness.ToString() : "0"));
                        break;
                }
#endif
            }
            else
            {
                switch (cfg.DeviceType)
                {
                    case ArcazeLedDigit.TYPE:
                        var device = cfg.Device as LedModule;

                        var decimalCount = value.Count(c => c == '.');

                        var val = value.PadRight(device.DisplayLedDigits.Count + decimalCount, device.DisplayLedPaddingChar[0]);
                        var decimalPoints = new List<string>(device.DisplayLedDecimalPoints);

                        if (device.DisplayLedPadding)
                        {
                            val = value.PadLeft(device.DisplayLedPadding
                                    ? device.DisplayLedDigits.Count + decimalCount
                                    : 0, device.DisplayLedPaddingChar[0]);
                        }

                        if (!string.IsNullOrEmpty(device.DisplayLedBrightnessReference))
                        {
                            string refValue = FindValueForRef(device.DisplayLedBrightnessReference);

                            mobiFlightCache.SetDisplayBrightness(
                                serial,
                                device.DisplayLedAddress,
                                device.DisplayLedConnector,
                                refValue
                                );
                        }

                        var reverse = device.DisplayLedReverseDigits;

                        mobiFlightCache.SetDisplay(
                            serial,
                            device.DisplayLedAddress,
                            device.DisplayLedConnector,
                            device.DisplayLedDigits,
                            decimalPoints,
                            val,
                            reverse);

                        break;

                    case MobiFlightStepper.TYPE:
                        var stepper = cfg.Device as OutputConfig.Stepper;
                        mobiFlightCache.SetStepper(
                            serial,
                            stepper.Address,
                            value,
                            stepper.InputRev,
                            stepper.OutputRev,
                            stepper.CompassMode,
                            stepper.Speed,
                            stepper.Acceleration
                        );
                        break;

                    case MobiFlightServo.TYPE:
                        var servo = cfg.Device as OutputConfig.Servo;
                        mobiFlightCache.SetServo(
                            serial,
                            servo.Address,
                            value,
                            int.Parse(servo.Min),
                            int.Parse(servo.Max),
                            Byte.Parse(servo.MaxRotationPercent)
                        );
                        break;

                    case OutputConfig.LcdDisplay.DeprecatedType:
                        var lcdDisplay = cfg.Device as LcdDisplay;
                        mobiFlightCache.SetLcdDisplay(
                            serial,
                            lcdDisplay,
                            value,
                            GetRefs(cfg.ConfigRefs)
                            );
                        break;

                    case MobiFlightShiftRegister.TYPE:
                        if (serial != null)
                        {
                            string outputValueShiftRegister = value;
                            var shiftRegister = cfg.Device as ShiftRegister;

                            if (outputValueShiftRegister != "0" && shiftRegister.PWM)
                            {
                                outputValueShiftRegister = shiftRegister.Brightness.ToString();
                            }

                            mobiFlightCache.SetShiftRegisterOutput(
                                serial,
                                shiftRegister.Address,
                                shiftRegister.Pin,
                                outputValueShiftRegister);
                        }
                        break;

                    case OutputConfig.CustomDevice.DeprecatedType:
                        var customDevice = cfg.Device as OutputConfig.CustomDevice;
                        mobiFlightCache.Set(serial, customDevice, value);
                        break;

                    case "InputAction":
                        int iValue = 0;
                        int.TryParse(value, out iValue);

                        List<ConfigRefValue> cfgRefs = GetRefs(cfg.ConfigRefs);
                        CacheCollection cacheCollection = new CacheCollection()
                        {
                            fsuipcCache = fsuipcCache,
                            simConnectCache = simConnectCache,
                            moduleCache = mobiFlightCache,
                            xplaneCache = xplaneCache,
                            proSimCache = proSimCache,
                            joystickManager = joystickManager,
                        };

                        if (cfg.ButtonInputConfig != null)
                            inputActionExecutionCache.Execute(
                                cfg.ButtonInputConfig,
                                cacheCollection,
                                new InputEventArgs() { Value = iValue, StrValue = value },
                                cfgRefs
                            );
                        else
                        {
                            inputActionExecutionCache.Execute(
                                cfg.AnalogInputConfig,
                                cacheCollection,
                                new InputEventArgs() { Value = iValue, StrValue = value },
                                cfgRefs
                            );
                        }
                        break;

                    case MobiFlightOutput.TYPE:
                        string outputValue = value;

                        if (outputValue != "0" && !(cfg.Device as Output).DisplayPinPWM)
                            outputValue = (cfg.Device as Output).DisplayPinBrightness.ToString();

                        mobiFlightCache.SetValue(serial,
                            (cfg.Device as Output).DisplayPin,
                            outputValue);
                        break;
                }
            }

            return value.ToString();
        }

        private List<ConfigRefValue> GetRefs(ConfigRefList configRefs)
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
            foreach (var cfg in ConfigItems)
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
    }
}