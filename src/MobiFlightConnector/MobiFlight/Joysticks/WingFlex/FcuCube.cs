namespace MobiFlight.Joysticks.WingFlex
{
    internal class FcuCube : BaseCube
    {
        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Defintion file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName ?? "FcuCube"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public FcuCube(JoystickDefinition definition) : base(new FcuCubeReport(), definition)
        {
        }
    }
}