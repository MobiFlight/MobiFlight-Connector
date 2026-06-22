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
                case "RMP Cube":
                case "EFIS Cube":
                case "FCU Cube":
                case "OVHD Cube":
                    return true;
            }

            return false;
        }
        internal static Joystick Create(JoystickDefinition definition)
        {
            Joystick result = null;
            switch (definition.InstanceName)
            {
                case "RMP Cube":
                    result = new WingFlex.RmpCube(definition);
                    break;
                case "EFIS Cube":
                    result = new WingFlex.EfisCube(definition);
                    break;
                case "FCU Cube":
                    result = new WingFlex.FcuCube(definition);
                    break;
                case "OVHD Cube":
                    result = new WingFlex.OvhdCube(definition);
                    break;
            }

            return result;
        }
    }
}