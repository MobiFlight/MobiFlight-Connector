namespace MobiFlight.Joysticks.Bodnar
{
    internal class BU0836X : BodnarBoard
    {
        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Definition file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName.Trim() ?? "BU0836X"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="joystick">The DirectInput joystick instance.</param>
        /// <param name="definition">Joystick definition file.</param>
        public BU0836X(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(32, joystick, definition)
        {
        }
    }
}