namespace MobiFlight.Joysticks.WingFlex
{
    internal class OvhdCube : BaseCube
    {
        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Defintion file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName ?? "OVHD Cube"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public OvhdCube(JoystickDefinition definition) 
            : base(new OvhdCubeReport(), definition)
        {
        }
    }
}