using MobiFlight.Base;
using MobiFlight.InputConfig;
using MobiFlight.Modifier;
using System.Collections.Generic;

namespace MobiFlight.Frontend
{
    public class ConfigEvent
    {
        public string Type { get; set; }
        public object Settings { get; set; }

        static public ConfigEvent Create(OutputConfigItem item)
        {
            var result = new ConfigEvent();

            result.Type = item.SourceType.ToString();
            switch (item.SourceType)
            {
                case SourceType.SIMCONNECT:
                    result.Settings = item.SimConnectValue;
                    break;

                case SourceType.FSUIPC:
                    result.Settings = item.FSUIPC;
                    break;

                case SourceType.VARIABLE:
                    result.Settings = item.MobiFlightVariable;
                    break;

                case SourceType.XPLANE:
                    result.Settings = item.XplaneDataRef;
                    break;
            }

            return result;
        }

        static public ConfigEvent Create(InputConfigItem item)
        {
            var result = new ConfigEvent();
            result.Type = item.Type;
            switch (item.Type)
            {
                case InputConfigItem.TYPE_BUTTON:
                    result.Settings = item.button;
                    break;

                case InputConfigItem.TYPE_ENCODER:
                    result.Settings = item.encoder;
                    break;

                case InputConfigItem.TYPE_ANALOG_OLD:
                case InputConfigItem.TYPE_ANALOG:
                    result.Settings = item.analog;
                    break;

                case InputConfigItem.TYPE_INPUT_MULTIPLEXER:
                    result.Settings = item.inputMultiplexer;
                    break;

                case InputConfigItem.TYPE_INPUT_SHIFT_REGISTER:
                    result.Settings = item.inputShiftRegister;
                    break;
            }

            return result;
        }
    }

    public class ConfigAction
    {
        public string Type { get; set; }
        public object Settings { get; set; }

        public static ConfigAction Create(OutputConfigItem item)
        {
            var result = new ConfigAction();
            result.Type = item.DisplayType;

            switch (item.DisplayType)
            {
                case MobiFlightLedModule.TYPE:
                    result.Settings = item.LedModule;
                    break;

                case MobiFlightLcdDisplay.TYPE:
                    result.Settings = item.LcdDisplay;
                    break;

                case MobiFlightOutput.TYPE:
                    result.Settings = item.Pin;
                    break;

                case MobiFlightServo.TYPE:
                    result.Settings = item.Servo;
                    break;

                case MobiFlightStepper.TYPE:
                    result.Settings = item.Stepper;
                    break;

                case MobiFlightShiftRegister.TYPE:
                    result.Settings = item.ShiftRegister;
                    break;

                case MobiFlightCustomDevice.TYPE:
                    result.Settings = item.CustomDevice;
                    break;
            }

            return result;
        }

        public static ConfigAction Create(InputConfigItem item)
        {
            var result = new ConfigAction();
            result.Type = item.Type;

            switch (item.Type)
            {
                case InputConfigItem.TYPE_BUTTON:
                    result.Settings = new Dictionary<string, InputAction>() {
                        { "OnPress" , item.button.onPress },
                        { "OnRelease" , item.button.onRelease },
                        { "OnLongRelease", item.button.onLongRelease },
                        { "OnHold" , item.button.onHold }
                    };
                    break;

                case InputConfigItem.TYPE_ENCODER:
                    result.Settings = new Dictionary<string, InputAction>() {
                        { "OnLeft" , item.encoder.onLeft },
                        { "OnLeftFast" , item.encoder.onLeftFast },
                        { "OnRight", item.encoder.onRight },
                        { "OnRightFast" , item.encoder.onRightFast }
                    }; ;
                    break;

                case InputConfigItem.TYPE_ANALOG_OLD:
                case InputConfigItem.TYPE_ANALOG:
                    result.Settings = item.analog;
                    break;

                case InputConfigItem.TYPE_INPUT_MULTIPLEXER:
                    result.Settings = item.inputMultiplexer;
                    break;

                case InputConfigItem.TYPE_INPUT_SHIFT_REGISTER:
                    result.Settings = item.inputShiftRegister;
                    break;
            }

            return result;
        }
    }

    public class ConfigContext
    {
        public List<Precondition> Preconditions { get; set; } = new List<Precondition>();
        public List<ConfigRef> ConfigRefs { get; set; } = new List<ConfigRef>();

        public static ConfigContext Create(InputConfigItem item)
        {
            var result = new ConfigContext
            {
                Preconditions = item.Preconditions.Items,
                ConfigRefs = item.ConfigRefs.Items
            };
            return result;
        }

        public static ConfigContext Create(OutputConfigItem item)
        {
            var result = new ConfigContext
            {
                Preconditions = item.Preconditions.Items,
                ConfigRefs = item.ConfigRefs.Items
            };
            return result;
        }
    }


    public interface IConfigItem
    {
        string GUID { get; set; }
        bool Active { get; set; }
        string Description { get; set; }
        string Device { get; set; }
        string Component { get; set; }
        string Type { get; set; }
        string[] Tags { get; set; }
        string[] Status { get; set; }
        string RawValue { get; set; }
        string ModifiedValue { get; set; }

        ModifierBase[] Modifiers { get; set; }
        ConfigEvent Event { get; set; }
        ConfigAction Action { get; set; }
        ConfigContext Context { get; set; }
    }
}
