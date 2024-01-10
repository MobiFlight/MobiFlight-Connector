using MobiFlight.Joysticks.Octavi;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;

namespace MobiFlight
{
    public class JoystickManager
    {
        // Set to true if any errors occurred when loading the definition files.
        // Used as part of the unit test automation to determine if the checked-in
        // JSON files are valid.
        public bool LoadingError = false;

        private static readonly SharpDX.DirectInput.DeviceType[] SupportedDeviceTypes =
        {
            SharpDX.DirectInput.DeviceType.Joystick,
            SharpDX.DirectInput.DeviceType.Gamepad,
            SharpDX.DirectInput.DeviceType.Driving,
            SharpDX.DirectInput.DeviceType.Flight,
            SharpDX.DirectInput.DeviceType.FirstPerson,
            SharpDX.DirectInput.DeviceType.Supplemental
        };

        private List<JoystickDefinition> Definitions = new List<JoystickDefinition>();
        public event EventHandler Connected;
        public event ButtonEventHandler OnButtonPressed;
        private readonly Timer PollTimer = new Timer();        
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, Joystick> Joysticks = new System.Collections.Concurrent.ConcurrentDictionary<string, Joystick>();
        private readonly List<Joystick> ExcludedJoysticks = new List<Joystick>();
        private IntPtr Handle;

        public JoystickManager()
        {
            PollTimer.Interval = 20;
            PollTimer.Elapsed += PollTimer_Tick;
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
        public void LoadDefinitions()
        {
            Definitions = JsonBackedObject.LoadDefinitions<JoystickDefinition>(Directory.GetFiles("Joysticks", "*.joystick.json"), "Joysticks/mfjoystick.schema.json",
                onSuccess: (joystick, definitionFile) => Log.Instance.log($"Loaded joystick definition for {joystick.InstanceName}", LogSeverity.Info),
                onError: () => LoadingError = true
            );
        }

        public bool JoysticksConnected()
        {
            return Joysticks.Count > 0;
        }

        private void PollTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                lock (Joysticks)
                {
                    foreach (MobiFlight.Joystick js in Joysticks.Values)
                    {
                        js?.Update();
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // this exception is thrown when a joystick is disconnected and removed from the list of joysticks
            }
            catch (Exception ex)
            {
                // something else has happened
                Log.Instance.log($"An exception occured during update {ex.Message}", LogSeverity.Error);
            }
        }

        public void Startup()
        {
            PollTimer.Start();
        }

        public void Shutdown()
        {
            PollTimer.Stop();
            foreach (var js in Joysticks.Values)
            {
                js.Shutdown();
            }
            Joysticks.Clear();
            ExcludedJoysticks.Clear();
        }

        public void Stop()
        {
            foreach (var j in Joysticks.Values)
            {
                j.Stop();
            }
        }

        /// <summary>
        /// Returns the list of Joysticks sorted by name
        /// </summary>
        /// <returns>List of currently connected joysticks</returns>
        public List<Joystick> GetJoysticks()
        {
            return Joysticks.Values.OrderBy(j=>j.Name).ToList();
        }

        public List<Joystick> GetExcludedJoysticks()
        {
            return ExcludedJoysticks;
        }

        public void SetHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public void Connect()
        {
            var di = new SharpDX.DirectInput.DirectInput();
            Joysticks?.Clear();
            ExcludedJoysticks?.Clear();
            List<string> settingsExcludedJoysticks = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.ExcludedJoysticks);

            var devices = di.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).ToList();

            foreach (var d in devices)
            {
                Log.Instance.log($"Found attached DirectInput device: {d.InstanceName} Type: {d.Type} SubType: {d.Subtype}.", LogSeverity.Debug);

                if (!SupportedDeviceTypes.Contains(d.Type))
                {
                    Log.Instance.log($"Skipping unsupported device: {d.InstanceName} Type: {d.Type} SubType: {d.Subtype}.", LogSeverity.Debug);
                    continue;
                }

                MobiFlight.Joystick js;
                if (d.InstanceName == "Octavi" || d.InstanceName == "IFR1")
                {
                    js = new Octavi(
                            new SharpDX.DirectInput.Joystick(di, d.InstanceGuid), 
                            // statically set this to Octavi
                            // until we might support (Octavi|IFR1) or similar
                            GetDefinitionByInstanceName("Octavi")
                         );
                }
                else
                {
                    js = new Joystick(new SharpDX.DirectInput.Joystick(di, d.InstanceGuid), GetDefinitionByInstanceName(d.InstanceName));
                }

                if (!HasAxisOrButtons(js))
                {
                    Log.Instance.log($"Skipping device with no buttons or axis: {d.InstanceName}.", LogSeverity.Debug);
                    continue;
                }

                // Check against exclusion list
                if (settingsExcludedJoysticks.Contains(js.Name))
                {
                    Log.Instance.log($"Ignore attached joystick device: {js.Name}.", LogSeverity.Info);
                    ExcludedJoysticks.Add(js);
                }
                else
                {
                    Log.Instance.log($"Adding attached joystick device: {d.InstanceName} Buttons: {js.Capabilities.ButtonCount} Axis: {js.Capabilities.AxeCount}.", LogSeverity.Info);
                    js.Connect(Handle);
                    Joysticks.TryAdd(js.Name, js);
                    js.OnButtonPressed += Js_OnButtonPressed;
                    js.OnDisconnected += Js_OnDisconnected;
                }       
            }

            if (JoysticksConnected())
            {
                Connected?.Invoke(this, null);
            }
        }

        private void Js_OnDisconnected(object sender, EventArgs e)
        {
            var js = sender as Joystick;
            Log.Instance.log($"Joystick disconnected: {js.Name}.", LogSeverity.Info);
            Joysticks.TryRemove(js.Name, out _);
        }

        private bool HasAxisOrButtons(Joystick js)
        {
            return
                js.Capabilities.AxeCount > 0 ||
                js.Capabilities.ButtonCount > 0;
        }

        private void Js_OnButtonPressed(object sender, InputEventArgs e)
        {
            OnButtonPressed?.Invoke(sender, e);
        }

        internal Joystick GetJoystickBySerial(string serial)
        {
            return Joysticks.Values.ToList().Find(js => js.Serial == serial);
        }

        public Dictionary<String, int> GetStatistics()
        {
            var result = new Dictionary<string, int>
            {
                ["Joysticks.Count"] = Joysticks.Count()
            };

            foreach (var joystick in Joysticks.Values)
            {
                var key = $"Joysticks.Model.{joystick.Name}";

                if (!result.ContainsKey(key)) result[key] = 0;
                result[key] += 1;
            }

            return result;
        }
    }
}
