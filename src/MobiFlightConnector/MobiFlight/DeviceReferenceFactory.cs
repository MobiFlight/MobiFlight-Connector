using MobiFlight.Base;
using MobiFlight.Firmware;
using System.Collections.Generic;

namespace MobiFlight
{
    public static class DeviceReferenceFactory
    {
        public static IEnumerable<DeviceReference> Create(BaseDevice dev)
        {
            switch (dev.Type)
            {
                case DeviceType.Button:
                    yield return new DeviceReference { Name = dev.Name, Type = DeviceType.Button };
                    break;

                case DeviceType.Encoder:
                    yield return new DeviceReference { Name = dev.Name, Type = DeviceType.Encoder };
                    break;

                case DeviceType.AnalogInput:
                    yield return new DeviceReference { Name = dev.Name, Type = DeviceType.AnalogInput };
                    break;

                case DeviceType.InputMultiplexer:
                    var mux = dev as InputMultiplexer;
                    int muxPinCount = int.TryParse(mux?.NumBytes, out int nb) ? nb * 8 : 16;
                    for (int i = 0; i < muxPinCount; i++)
                        yield return new DeviceReference { Name = $"{dev.Name}:{i}", Type = DeviceType.Button };
                    break;

                case DeviceType.InputShiftRegister:
                    var isr = dev as InputShiftRegister;
                    int isrPinCount = int.TryParse(isr?.NumModules, out int nm) ? nm * 8 : 8;
                    for (int i = 0; i < isrPinCount; i++)
                        yield return new DeviceReference { Name = $"{dev.Name}:{i}", Type = DeviceType.Button };
                    break;
            }
        }
    }
}