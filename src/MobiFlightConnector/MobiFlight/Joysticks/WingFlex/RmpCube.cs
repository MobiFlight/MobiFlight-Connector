namespace MobiFlight.Joysticks.WingFlex
{
    internal class RmpCube : BaseCube
    {
        /// <summary>
        /// Provide same instance name as defined in the definition file.
        /// Also works if Defintion file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return Definition?.InstanceName ?? "RMP Cube"; }
        }

        /// <summary>
        /// Provides Serial including prefix.
        /// Serial information is provided through Device.Net
        /// </summary>
        public override string Serial
        {
            get { return $"{Joystick.SerialPrefix}{Device?.ConnectedDeviceDefinition?.SerialNumber}" ?? "RMP-CUBE-1234-ABCD-12345678"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="definition">joystick definition file.</param>
        public RmpCube(JoystickDefinition definition) 
            : base(new RmpCubeReport(), definition)
        {
        }
    }
}