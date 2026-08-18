using System.Collections;

namespace MobiFlight.Joysticks
{
    public class JoystickState
    {
        public int X { get; set; }

        public int Y { get; set; }
        public int Z { get; set; }

        public int RotationX { get; set; }

        public int RotationY { get; set; }
        public int RotationZ { get; set; }

        public int[] Sliders { get; internal set; }

        public int[] PointOfViewControllers { get; internal set; }
        public BitArray Buttons { get; protected set; }

        public JoystickState()
        {
            Buttons = new BitArray(256);
            Sliders = new int[2];
            PointOfViewControllers = new int[4];
        }

        static public JoystickState Create(SharpDX.DirectInput.JoystickState state)
        {
            JoystickState joystickState = new JoystickState
            {
                X = state.X,
                Y = state.Y,
                Z = state.Z,
                RotationX = state.RotationX,
                RotationY = state.RotationY,
                RotationZ = state.RotationZ,
                Sliders = state.Sliders,
                PointOfViewControllers = state.PointOfViewControllers,
                Buttons = new BitArray(state.Buttons)
            };

            return joystickState;
        }
    }
}