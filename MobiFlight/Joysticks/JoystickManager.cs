using HidSharp;
using MobiFlight.BrowserMessages;
using MobiFlight.Joysticks;
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

        public readonly List<JoystickDefinition> Definitions = new List<JoystickDefinition>();
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
            MobiFlight.Joysticks.ControllerDefinitionMigrator.MigrateJoysticks();
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
            var jsonFiles = Directory.GetFiles("Joysticks", "*.joystick.json", SearchOption.AllDirectories);
            var schemaFilePath = "Joysticks/mfjoystick.schema.json";

            var rawDefinitions = JsonBackedObject.LoadDefinitions<JoystickDefinition>(
                jsonFiles, 
                schemaFilePath,
                onSuccess: (joystick, definitionFile) => Log.Instance.log($"Loaded joystick definition for {joystick.InstanceName}", LogSeverity.Debug),
                onError: () => LoadingError = true
            );

            // now we have a symmetry with the MidiBoardManager
            Definitions.Clear();
            Definitions.AddRange(rawDefinitions);

            MessageExchange.Instance.Publish(Definitions);
        }

        public string MapDeviceNameToLabel(string boardName, string deviceName)
        {
            var definition = Definitions.Find(def => def.InstanceName == boardName);
            return definition?.MapDeviceNameToLabel(deviceName) ?? deviceName;
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
                        try
                        {
                            js?.Update();
                        }
                        catch (Exception ex)
                        {
                            Log.Instance.log($"An exception occurred during joystick update for {js.Name}: {ex.Message}", LogSeverity.Error);
                        }
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
                Log.Instance.log($"An exception occurred during update {ex.Message}", LogSeverity.Error);
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
        public virtual List<Joystick> GetJoysticks()
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

                var diJoystick = new SharpDX.DirectInput.Joystick(di, d.InstanceGuid);
                var productId = diJoystick.Properties.ProductId;
                var vendorId = diJoystick.Properties.VendorId;
                
                // Check if this device should be handled later as an HID controller
                if (HidControllerFactory.CanCreate(d.InstanceName))
                {
                    continue;
                }

                // Get the product name (handles special cases like VKB)
                string productName = ControllerFactory.GetProductName(d, diJoystick, vendorId);

                // Get the appropriate definition for this device
                JoystickDefinition definition = GetJoystickDefinition(d.InstanceName, productName, vendorId, productId);

                // Use factory to create appropriate controller instance
                var js = ControllerFactory.Create(d, diJoystick, vendorId, productId, definition, WSServer);
                
                // If factory returns null, create a standard Joystick
                if (js == null)
                {
                    js = new Joystick(diJoystick, definition);
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

            ConnectHidController();

            if (JoysticksConnected())
            {
                Connected?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Gets the appropriate JoystickDefinition for a device.
        /// </summary>
        private JoystickDefinition GetJoystickDefinition(string instanceName, string productName, int vendorId, int productId)
        {
            // Octavi/IFR1 devices: statically set to Octavi
            if (instanceName == "Octavi" || instanceName == "IFR1")
            {
                return GetDefinitionByInstanceName("Octavi");
            }

            // Try to get definition by product name first, then by product ID
            return GetDefinitionByInstanceName(productName) ?? GetDefinitionByProductId(vendorId, productId);
        }

        private void ConnectHidController()
        {
            try
            {
                var allHidDevices = DeviceList.Local.GetHidDevices().ToList();
                Log.Instance.log($"Found {allHidDevices.Count} HID devices, checking for supported devices", LogSeverity.Debug);

                allHidDevices.ForEach(hidDevice =>
                {
                    try
                    {
                        var definition = GetDefinitionByProductId(hidDevice.VendorID, hidDevice.ProductID);
                        if (definition == null) return;

                        if (Joysticks.Values.Where(j => j.Name == definition.InstanceName).Count() > 0)
                        {
                            // already loaded as regular DirectInput Joystick
                            return;
                        }

                        var joystick = HidControllerFactory.Create(definition);

                        if (joystick == null) return;

                        joystick.Connect(new IntPtr());
                        joystick.OnButtonPressed += Js_OnButtonPressed;
                        joystick.OnDisconnected += Js_OnDisconnected;
                        Joysticks.TryAdd(joystick.Serial, joystick);
                        Log.Instance.log($"Connected HID device: {definition.InstanceName}", LogSeverity.Info);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.log($"Error connecting HID device {hidDevice.GetFriendlyName()}: {ex.Message}", LogSeverity.Error);
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error enumerating HID devices: {ex.Message}", LogSeverity.Error);
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
