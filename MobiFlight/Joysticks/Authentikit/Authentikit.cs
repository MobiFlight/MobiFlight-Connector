using MobiFlight.Joysticks.Bodnar;

namespace MobiFlight.Joysticks.AuthentiKit
{
    internal class AuthentiKit : BU0836A
    {
        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Definition file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName.Trim() ?? "AuthentiKit"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="joystick">The DirectInput joystick instance.</param>
        /// <param name="definition">Joystick definition file.</param>
        public AuthentiKit(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
        }
    }
}