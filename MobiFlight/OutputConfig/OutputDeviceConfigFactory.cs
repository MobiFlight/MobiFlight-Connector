using MobiFlight.Base;

namespace MobiFlight.OutputConfig
{
    public class OutputDeviceConfigFactory
    {
        static public DeviceConfig CreateFromType(string DeviceType)
        {
            DeviceConfig Device = null;

            // preserve backward compatibility
            if (DeviceType == "Pin") DeviceType = MobiFlightOutput.TYPE;
            if (DeviceType == ArcazeLedDigit.OLDTYPE) DeviceType = ArcazeLedDigit.TYPE;

            switch (DeviceType)
            {
                case MobiFlightOutput.TYPE:
                    Device = new Output();
                    break;
                case MobiFlightLedModule.TYPE:
                    Device = new LedModule();
                    break;
                case MobiFlightServo.TYPE:
                    Device = new Servo();
                    break;
                case MobiFlightStepper.TYPE:
                    Device = new Stepper();
                    break;
                case LcdDisplay.DeprecatedType:
                    Device = new LcdDisplay();
                    break;
                case MobiFlightShiftRegister.TYPE:
                    Device = new ShiftRegister();
                    break;
                case MobiFlightCustomDevice.TYPE:
                    Device = new CustomDevice();
                    break;
            }

            return Device;
        }
    }
}
