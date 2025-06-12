using MobiFlight.Base;
using System;

namespace MobiFlight.InputConfig
{
    static public class InputDeviceConfigFactory
    {
        static public DeviceConfig CreateFromType(string type)
        {
            DeviceConfig deviceConfig = null;
            switch (type)
            {
                case MobiFlightInputMultiplexer.TYPE:
                    deviceConfig = new MobiFlightInputMultiplexer();
                    break;

                case MobiFlightAnalogInput.TYPE_OLD:
                case MobiFlightAnalogInput.TYPE:
                    deviceConfig = new MobiFlightAnalogInput();
                    break;

                case MobiFlightButton.TYPE:
                    deviceConfig = new MobiFlightButton();
                    break;

                case MobiFlightEncoder.TYPE:
                    deviceConfig = new MobiFlightEncoder();
                    break;

                case MobiFlightInputShiftRegister.TYPE:
                    deviceConfig = new MobiFlightInputShiftRegister();
                    break;

                default:
                    throw new ArgumentException($"Unknown input device type: {type}");
            }
            return deviceConfig;
        }
    }
}
