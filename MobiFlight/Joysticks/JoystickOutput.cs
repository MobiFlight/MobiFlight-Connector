using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class JoystickOutput
    {
        /// <summary>
        ///  Display name for the output.
        /// </summary>
        public string Label;
        /// <summary>
        /// Device's name for the input.
        /// </summary>
        public string Name;
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
