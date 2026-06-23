namespace MobiFlight.Joysticks.WingFlex
{
    internal class EfisCube : BaseCube
    {
        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Defintion file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName ?? "EFIS Cube"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public EfisCube(JoystickDefinition definition) 
            : base(new EfisCubeReport(), definition)
        {
        }
    }
}