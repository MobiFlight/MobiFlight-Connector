using System;

namespace MobiFlight.Joysticks
{
    internal class HidControllerFactory
    {
        internal static Joystick Create(JoystickDefinition definition)
        {
            Joystick result = null;
            switch (definition.InstanceName)
            {
                case "FCU Cube":
                    result = new WingFlex.FcuCube(definition);
                    break;
            }

            return result;
        }
    }
}