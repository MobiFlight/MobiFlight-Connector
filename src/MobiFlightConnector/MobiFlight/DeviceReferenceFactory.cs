using MobiFlight.Base;
using MobiFlight.CustomDevices;
using MobiFlight.Firmware;
using MobiFlight.UI.Panels.Settings.Device;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight
{
    public static class DeviceReferenceFactory
    {
        public static List<ControllerDevice> Create(BaseDevice dev, Board board = null)
        {
            var result = new List<ControllerDevice>();
            switch (dev.Type)
            {
                case DeviceType.Button:
                case DeviceType.Encoder:
                case DeviceType.AnalogInput:
                case DeviceType.Servo:
                    result.Add(CreateDevice(dev.Name, dev.Type));
                    break;

                case DeviceType.Output:
                    result.Add(CreateDevice(dev.Name, dev.Type, OutputCapability(dev as Output, board)));
                    break;

                case DeviceType.LcdDisplay:
                    result.Add(CreateDevice(dev.Name, dev.Type, LcdCapability(dev as LcdDisplay)));
                    break;

                case DeviceType.Stepper:
                    result.Add(CreateDevice(dev.Name, dev.Type, StepperCapability(dev as Stepper)));
                    break;

                case DeviceType.CustomDevice:
                    result.Add(CreateDevice(dev.Name, dev.Type, CustomDeviceCapability(dev as Firmware.CustomDevice)));
                    break;

                case DeviceType.LedModule:
                    var led = dev as LedModule;
                    int ledCount = int.TryParse(led?.NumModules, out int nl) ? nl : 1;
                    var ledCapability = new DeviceCapability { Digits = LedDigits(led) };
                    for (int i = 1; i <= ledCount; i++)
                        result.Add(CreateDevice($"{dev.Name}:{i}", DeviceType.LedModule, ledCapability));
                    break;

                case DeviceType.InputMultiplexer:
                    var mux = dev as InputMultiplexer;
                    int muxPinCount = int.TryParse(mux?.NumBytes, out int nb) ? nb * 8 : 16;
                    for (int i = 0; i < muxPinCount; i++)
                        result.Add(CreateDevice($"{dev.Name}:{i}", DeviceType.Button));
                    break;

                case DeviceType.InputShiftRegister:
                    var isr = dev as InputShiftRegister;
                    int isrPinCount = int.TryParse(isr?.NumModules, out int nm) ? nm * 8 : 8;
                    for (int i = 0; i < isrPinCount; i++)
                        result.Add(CreateDevice($"{dev.Name}:{i}", DeviceType.Button));
                    break;

                case DeviceType.ShiftRegister:
                    var sr = dev as ShiftRegister;
                    int srPinCount = int.TryParse(sr?.NumModules, out int numModules) ? numModules * 8 : 8;
                    result.Add(CreateDevice(dev.Name, DeviceType.ShiftRegister, new DeviceCapability { Pins = srPinCount }));
                    break;
            }
            return result;
        }

        private static ControllerDevice CreateDevice(string name, DeviceType type, DeviceCapability capability = null)
        {
            return new ControllerDevice { Name = name, Label = name, Type = type, Capabilities = capability };
        }

        private static DeviceCapability OutputCapability(Output output, Board board)
        {
            var pin = board?.Pins.Find(p => p.Pin.ToString() == output?.Pin);
            return new DeviceCapability { Pwm = pin?.isPWM ?? false };
        }

        private static DeviceCapability LcdCapability(LcdDisplay lcd)
        {
            return new DeviceCapability { Rows = lcd?.Lines, Columns = lcd?.Cols };
        }

        private static int LedDigits(LedModule led) => led?.ModelType switch
        {
            LedModule.MODEL_TYPE_TM1637_4DIGIT => 4,
            LedModule.MODEL_TYPE_TM1637_6DIGIT => 6,
            _ => 8
        };

        private static DeviceCapability StepperCapability(Stepper stepper)
        {
            var profile = MFStepperPanel.Profiles.Find(p => p.Value.id == stepper?.Profile)?.Value;
            return new DeviceCapability { Profile = profile, AutoZero = stepper?.BtnPin != Stepper.AUTOHOME_NONE };
        }

        private static DeviceCapability CustomDeviceCapability(Firmware.CustomDevice device)
        {
            var definition = CustomDeviceDefinitions.GetDeviceByType(device?.CustomType);
            var messageTypes = definition?.MessageTypes
                .Select(m => new CustomDeviceMessageType(m.Id, m.Label, m.Description))
                .ToList();
            return new DeviceCapability { MessageTypes = messageTypes };
        }
    }
}