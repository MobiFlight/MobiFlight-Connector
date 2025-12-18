using System;

namespace MobiFlight.Joysticks
{
    internal class HidControllerFactory
    {
        public static bool CanCreate(string InstanceName)
        {
            switch (InstanceName.Trim())
            {
                case "FCU Cube":
                    return true;
                default:
                    return false;
            }
        }
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