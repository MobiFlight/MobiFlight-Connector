namespace MobiFlight.Joysticks.Bodnar
{
    internal class BU0836A : BodnarBoard
    {
        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Definition file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName.Trim() ?? "BU0836A"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="joystick">The DirectInput joystick instance.</param>
        /// <param name="definition">Joystick definition file.</param>
        public BU0836A(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(12, joystick, definition)
        {
        }
    }
}