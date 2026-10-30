using MobiFlight.Base;
using MobiFlight.Firmware;
using System.Collections.Generic;

namespace MobiFlight
{
    public static class DeviceReferenceFactory
    {
        public static List<DeviceReference> Create(BaseDevice dev)
        {
            var result = new List<DeviceReference>();
            switch (dev.Type)
            {
                case DeviceType.Button:
                case DeviceType.Encoder:
                case DeviceType.AnalogInput:
                case DeviceType.Output:
                case DeviceType.LedModule:
                case DeviceType.Servo:
                case DeviceType.Stepper:
                case DeviceType.LcdDisplay:
                case DeviceType.CustomDevice:
                    result.Add(new DeviceReference { Name = dev.Name, Label = dev.Name, Type = dev.Type });
                    break;

                case DeviceType.InputMultiplexer:
                    var mux = dev as InputMultiplexer;
                    int muxPinCount = int.TryParse(mux?.NumBytes, out int nb) ? nb * 8 : 16;
                    for (int i = 0; i < muxPinCount; i++)
                        result.Add(new DeviceReference { Name = $"{dev.Name}:{i}", Label = $"{dev.Name}:{i}", Type = DeviceType.Button });
                    break;

                case DeviceType.InputShiftRegister:
                    var isr = dev as InputShiftRegister;
                    int isrPinCount = int.TryParse(isr?.NumModules, out int nm) ? nm * 8 : 8;
                    for (int i = 0; i < isrPinCount; i++)
                        result.Add(new DeviceReference { Name = $"{dev.Name}:{i}", Label = $"{dev.Name}:{i}", Type = DeviceType.Button });
                    break;

                case DeviceType.ShiftRegister:
                    var sr = dev as ShiftRegister;
                    int srPinCount = int.TryParse(sr?.NumModules, out int numModules) ? numModules * 8 : 8;
                    for (int i = 0; i < srPinCount; i++)
                        result.Add(new DeviceReference { Name = $"{dev.Name}:{i}", Label = $"{dev.Name}:{i}", Type = DeviceType.Output });
                    break;
            }
            return result;
        }
    }
}