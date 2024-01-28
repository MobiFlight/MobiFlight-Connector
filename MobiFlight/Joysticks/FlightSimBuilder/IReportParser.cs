using SharpDX.DirectInput;

namespace MobiFlight.Joysticks.FlightSimBuilder
{
    public interface IReportParser
    {
        IReportParser Parse(byte[] inputBuffer);
        JoystickState ToJoystickState();
    }
}