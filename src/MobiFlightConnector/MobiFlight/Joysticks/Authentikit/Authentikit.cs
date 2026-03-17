using MobiFlight.Joysticks.Bodnar;

namespace MobiFlight.Joysticks.AuthentiKit
{
    internal class AuthentiKit : BodnarBoard
    {
        /// <summary>
        /// Provide same instance name but trim it.
        /// Also works if Definition file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return base.Name.Trim() ?? "AuthentiKit"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="joystick">The DirectInput joystick instance.</param>
        /// <param name="definition">Joystick definition file.</param>
        public AuthentiKit(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(12, joystick, definition)
        {
        }
    }
}