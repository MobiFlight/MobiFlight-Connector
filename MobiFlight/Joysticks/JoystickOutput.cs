namespace MobiFlight
{
    public class JoystickOutput
    {
        /// <summary>
        /// Unique Id for the output.
        /// </summary>
        public string Id;
        /// <summary>
        ///  Display name for the output.
        /// </summary>
        public string Label;
        /// <summary>
        /// Byte location of the output.
        /// </summary>
        public byte Byte;
        /// <summary>
        /// Bit location within the byte of the output.
        /// </summary>
        public byte Bit;
    }
}
