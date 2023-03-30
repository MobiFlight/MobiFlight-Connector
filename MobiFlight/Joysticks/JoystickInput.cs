using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public class JoystickInput
    {
        /// <summary>
        /// The input's unique identifier on the joystick.
        /// </summary>
        public int Id;

        /// <summary>
        /// The input's type. Supported values: Button, Axis.
        /// </summary>
        public JoystickDeviceType Type;

        /// <summary>
        /// The legacy name for the input, for backwards compatibility with previous releases.
        /// </summary>
        public string Name
        {
            get
            {
                if (this.Type == JoystickDeviceType.Axis)
                {
                    var name = $"{Type} {Joystick.GetAxisNameForUsage(Id)}";
                    return name;
                }

                return $"{Type} {Id}";
            }
        }

        /// <summary>
        /// Friendly label for the input.
        /// </summary>
        public string Label;
    }
}
