namespace MobiFlight
{
    public class JoystickOutput
    {
        /// <summary>
        /// BitOutput | ByteOutput | LcdDisplay
        /// </summary>
        public string Type;
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
        /// <summary>
        /// Number of columns of the output display.
        /// </summary>
        public byte Cols;
        /// <summary>
        /// Number of lines of the output display.
        /// </summary>
        public byte Lines;
    }
}
