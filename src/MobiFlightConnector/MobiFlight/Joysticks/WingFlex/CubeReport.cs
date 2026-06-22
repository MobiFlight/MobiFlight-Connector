using SharpDX.DirectInput;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.WingFlex
{
    public interface ICubeReport
    {
        void CopyFromInputBuffer(byte[] inputBuffer);
        ICubeReport Parse(byte[] inputBuffer);

        byte[] FromOutputDeviceState(List<JoystickOutputDevice> state);
        JoystickState ToJoystickState();
    }
}