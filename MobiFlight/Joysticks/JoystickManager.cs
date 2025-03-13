using MobiFlight.Joysticks.Octavi;
using MobiFlight.Joysticks.Winwing;
using MobiFlight.Joysticks.VKB;
using Newtonsoft.Json;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WebSocketSharp.Server;

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

        // Websocket Server on port 8320, not yet started
        WebSocketServer WSServer = new WebSocketServer(System.Net.IPAddress.Loopback, 8320);

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
        /// Finds a JoystickDefinition by the device's vendor and product id.
        /// </summary>
        /// <param name="vendorId">The vendor id of the device.</param>
        /// <param name="productId">The product id of the device.</param>
        /// <returns>The first definition matching the product id, or null if none found.</returns>
        private JoystickDefinition GetDefinitionByProductId(int vendorId, int productId)
        {
            return Definitions.Find(def => (def.ProductId == productId && def.VendorId == vendorId));
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
                    foreach (Joystick js in Joysticks.Values)
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
            if (WSServer.IsListening)
            {
                WSServer.Stop();
            }
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
            return Joysticks.Values.OrderBy(j => j.Name).ToList();
        }

        public List<Joystick> GetExcludedJoysticks()
        {
            return ExcludedJoysticks;
        }

        public void SetHandle(IntPtr handle)
        {
            Handle = handle;
        }

        public async void Connect()
        {
            var di = new SharpDX.DirectInput.DirectInput();
            Joysticks?.Clear();
            ExcludedJoysticks?.Clear();
            List<string> settingsExcludedJoysticks = JsonConvert.DeserializeObject<List<string>>(Properties.Settings.Default.ExcludedJoysticks);

            // make this next call async so that it doesn't block the UI
            var devices = await Task.Run(() => di.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).ToList());

            foreach (var d in devices)
            {
                Log.Instance.log($"Found attached DirectInput device: {d.InstanceName} Type: {d.Type} SubType: {d.Subtype}.", LogSeverity.Debug);

                if (!SupportedDeviceTypes.Contains(d.Type))
                {
                    Log.Instance.log($"Skipping unsupported device: {d.InstanceName} Type: {d.Type} SubType: {d.Subtype}.", LogSeverity.Debug);
                    continue;
                }

                Joystick js;
                var diJoystick = new SharpDX.DirectInput.Joystick(di, d.InstanceGuid);
                var productId = diJoystick.Properties.ProductId;
                var vendorId = diJoystick.Properties.VendorId;
                if (d.InstanceName == "Octavi" || d.InstanceName == "IFR1")
                {
                    // statically set this to Octavi until we might support (Octavi|IFR1) or similar
                    js = new Octavi(diJoystick, GetDefinitionByInstanceName("Octavi"));
                }     
                else if (vendorId == 0x4098 && WinwingConstants.FCU_PRODUCTIDS.Contains(productId))
                {
                    var joystickDef = GetDefinitionByProductId(vendorId, productId);
                    js = new WinwingFcu(diJoystick, joystickDef, productId, WSServer);
                }
                else if (vendorId == 0x4098 && WinwingConstants.CDU_PRODUCTIDS.Contains(productId))
                {
                    var joystickDef = GetDefinitionByProductId(vendorId, productId);
                    js = new WinwingCdu(diJoystick, joystickDef, productId, WSServer);
                }
                else if (vendorId == 0x231D)
                {
                    // VKB devices are highly configurable. DirectInput names can have old values cached in the registry, but HID names seem to be immune to that.
                    // Also trim the extraneous whitespaces on VKB device names.
                    var hidDevice = VKBDevice.GetMatchingHidDevice(diJoystick);
                    string productName = d.InstanceName;
                    if (hidDevice != null)
                    {
                        productName = hidDevice.GetProductName();
                    }
                    js = new VKBDevice(diJoystick, GetDefinitionByInstanceName(productName.Trim()));
                }
                else
                {
                    js = new Joystick(diJoystick, GetDefinitionByInstanceName(d.InstanceName));
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
                    Joysticks.TryAdd(js.Serial, js);
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
            Joysticks.TryRemove(js.Serial, out _);
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

                if (!result.ContainsKey(key))
                {
                    result[key] = 0;
                }

                result[key] += 1;
            }

            return result;
        }
    }
}
