using System;

namespace MobiFlight.Joysticks
{
    public class HidControllerFactory
    {
        public static bool CanCreate(string InstanceName)
        {
            if (String.IsNullOrWhiteSpace(InstanceName))
            {
                return false;
            }

            switch (InstanceName.Trim())
            {
                case "WINGFLEX FCU CUBE":
                case "FCU Cube":
                    return true;
            }

            return false;
        }
        internal static Joystick Create(JoystickDefinition definition)
        {
            Joystick result = null;
            switch (definition.InstanceName)
            {
                case "WINGFLEX FCU CUBE":
                case "FCU Cube":
                    result = new WingFlex.FcuCube(definition);
                    break;
            }

            return result;
        }
    }
}