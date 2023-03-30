using Newtonsoft.Json;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight
{
    public class JoystickManager
    {
        private readonly List<JoystickDefinition> Definitions = new List<JoystickDefinition>();
        public event EventHandler Connected;
        public event ButtonEventHandler OnButtonPressed;
        readonly Timer PollTimer = new Timer();
        readonly List<Joystick> joysticks = new List<Joystick>();

        public JoystickManager ()
        {
            PollTimer.Interval = 50;
            PollTimer.Tick += PollTimer_Tick;
            LoadDefinitions();
        }

        /// <summary>
        /// Finds a JoystickDefinition by the device's instance name.
        /// </summary>
        /// <param name="instanceName">The instance name of the device.</param>
        /// <returns>The first definition matching the instanceMae, or null if none found.</returns>
        private JoystickDefinition GetDefinitionByInstanceName(String instanceName)
        {
            return Definitions.Find(definition => definition.InstanceName == instanceName);
        }

        /// <summary>
        /// Loads all joystick definitions from disk.
        /// </summary>
        private void LoadDefinitions()
        {
            foreach (var definitionFile in Directory.GetFiles("Joysticks", "*.joystick.json"))
            {
                try
                {
                    var joystick = JsonConvert.DeserializeObject<JoystickDefinition>(File.ReadAllText(definitionFile));
                    joystick.Migrate();
                    Definitions.Add(joystick);
                    Log.Instance.log($"Loaded joystick definition for {joystick.InstanceName}", LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                }
            }

        }

        public bool JoysticksConnected()
        {
            return joysticks.Count > 0;
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                lock (joysticks) { 
                    foreach (MobiFlight.Joystick js in joysticks)
                    {
                        js?.Update();
                    }
                }
            } catch (InvalidOperationException)
            {
                // this exception is thrown when a joystick is disconnected and removed from the list of joysticks
            }
        }

        public void Start()
        {
            PollTimer.Start();
        }

        public void Shutdown()
        {
            PollTimer.Stop();
        }

        public void Stop()
        {
            foreach (var j in joysticks)
            {
                j.Stop();
            }
        }

        public List<MobiFlight.Joystick> GetJoysticks()
        {
            return joysticks;
        }

        public void Connect(IntPtr Handle)
        {
            var di = new SharpDX.DirectInput.DirectInput();
            joysticks?.Clear();

            var devices = di.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).ToList();

            foreach (var d in devices)
            {
                Log.Instance.log($"Found attached DirectInput device: {d.InstanceName} Type: {d.Type} SubType: {d.Subtype}.", LogSeverity.Debug);

                if (!IsSupportedDeviceType(d)) continue;

                var js = new Joystick(new SharpDX.DirectInput.Joystick(di, d.InstanceGuid), GetDefinitionByInstanceName(d.InstanceName));                        

                if (!HasAxisOrButtons(js)) continue;

                Log.Instance.log($"Adding attached joystick device: {d.InstanceName} Buttons: {js.Capabilities.ButtonCount} Axis: {js.Capabilities.AxeCount}.", LogSeverity.Info);
                js.Connect(Handle); 
                joysticks.Add(js);
                js.OnButtonPressed += Js_OnButtonPressed;
                js.OnAxisChanged += Js_OnAxisChanged;
                js.OnDisconnected += Js_OnDisconnected;
            }

            joysticks.Sort((j1, j2) => j1.Name.CompareTo(j2.Name));
            Connected?.Invoke(this, null);
        }

        private void Js_OnDisconnected(object sender, EventArgs e)
        {
            var js = sender as Joystick;
            Log.Instance.log($"Joystick disconnected: {js.Name}.", LogSeverity.Info);
            lock (joysticks)
                joysticks.Remove(js);            
        }

        private bool HasAxisOrButtons(Joystick js)
        {
            return
                js.Capabilities.AxeCount > 0 ||
                js.Capabilities.ButtonCount > 0;
        }

        private bool IsSupportedDeviceType(DeviceInstance d)
        {
            return
                d.Type == SharpDX.DirectInput.DeviceType.Joystick ||
                d.Type == SharpDX.DirectInput.DeviceType.Gamepad ||
                d.Type == SharpDX.DirectInput.DeviceType.Flight ||
                d.Type == SharpDX.DirectInput.DeviceType.Supplemental ||
                d.Type == SharpDX.DirectInput.DeviceType.FirstPerson;
        }

        private void Js_OnAxisChanged(object sender, InputEventArgs e)
        {
            OnButtonPressed?.Invoke(sender, e);
        }

        private void Js_OnButtonPressed(object sender, InputEventArgs e)
        {
            OnButtonPressed?.Invoke(sender, e);
        }

        internal Joystick GetJoystickBySerial(string serial)
        {
            return joysticks.Find(js => js.Serial == serial);
        }

        public Dictionary<String, int> GetStatistics()
        {
            var result = new Dictionary<string, int>
            {
                ["Joysticks.Count"] = joysticks.Count()
            };

            foreach (var joystick in joysticks)
            {
                var key = $"Joysticks.Model.{joystick.Name}";

                if (!result.ContainsKey(key)) result[key] = 0;
                result[key] += 1;
            }

            return result;
        }
    }
}
