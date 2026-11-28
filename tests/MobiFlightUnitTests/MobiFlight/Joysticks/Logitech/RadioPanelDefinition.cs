using Newtonsoft.Json;

namespace MobiFlight.Joysticks.Logitech.Tests
{
    /// <summary>Loads the shipped Radio Panel definition, which the build copies next to the test binaries.</summary>
    internal static class RadioPanelDefinition
    {
        public static JoystickDefinition Load()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Joysticks", "logitech", "logitech_radio_panel.joystick.json");
            return JsonConvert.DeserializeObject<JoystickDefinition>(File.ReadAllText(path));
        }
    }
}
