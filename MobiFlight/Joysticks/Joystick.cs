using HidSharp;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml.Linq;

namespace MobiFlight
{
    public class JoystickNotConnectedException : Exception
    {
        public JoystickNotConnectedException(string Message) : base(Message) { }
    }

    public enum JoystickDeviceType
    {
        Button,
        Axis,
        POV,
        Light
    }
    
    public class JoystickDevice
    {
        public String Name;
        public String Label;
        public JoystickDeviceType Type;
        public ListItem ToListItem()
        {
            return new ListItem() { Label = Label, Value = Name };
        }
    }

    public class JoystickOutputDevice : JoystickDevice
    {
        public byte Byte = 0;
        public byte Bit = 0;
        public byte State = 0;
        public JoystickOutputDevice()
        {
            Type = JoystickDeviceType.Light;
        }
    }


    public class Joystick
    {
        public static readonly string ButtonPrefix = "Button";
        public static readonly string AxisPrefix = "Axis";
        public static readonly string PovPrefix = "POV";
        public static readonly string SerialPrefix = "JS-";
        public static readonly string[] AxisNames = { "X", "Y", "Z", "RotationX", "RotationY", "RotationZ", "Slider1", "Slider2" };

        public event ButtonEventHandler OnButtonPressed;
        public event ButtonEventHandler OnAxisChanged;
        public event EventHandler OnDisconnected;

        protected List<JoystickDevice> Buttons = new List<JoystickDevice>();
        private readonly List<JoystickDevice> Axes = new List<JoystickDevice>();
        private readonly List<JoystickDevice> POV = new List<JoystickDevice>();
        private readonly List<JoystickOutputDevice> Lights = new List<JoystickOutputDevice>();

        protected readonly SharpDX.DirectInput.Joystick DIJoystick;
        private readonly JoystickDefinition Definition;

        private HidDevice Device;
        protected bool RequiresOutputUpdate = false;
        private JoystickState State = null;
        private HidStream Stream;

        private static readonly Dictionary<int, string> UsageMap = new Dictionary<int, string>
        {
            [48] = "X",
            [49] = "Y",
            [50] = "Z",
            [51] = "RotationX",
            [52] = "RotationY",
            [53] = "RotationZ",
            [54] = "Slider1",
            [55] = "Slider2"
        };

        /// <summary>
        /// This allows to raise OnButtonPressed from derived classes
        /// https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-raise-base-class-events-in-derived-classes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void TriggerButtonPressed(object sender, InputEventArgs e)
        {
            OnButtonPressed?.Invoke(sender, e);
        }

        public static bool IsJoystickSerial(string serial)
        {
            return (serial != null && serial.Contains(SerialPrefix));
        }

        public string Name { 
            get { return DIJoystick?.Information.InstanceName; }  
        }

        public string Serial
        {
            get { return SerialPrefix + DIJoystick.Information.InstanceGuid;  }
        }

        public SharpDX.DirectInput.DeviceType Type
        {
            get { return DIJoystick.Information.Type; }
        }

        public Capabilities Capabilities {
            get {
                return this.DIJoystick.Capabilities;
            }
        }

        public Joystick(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition)
        {
            this.DIJoystick = joystick;
            this.Definition = definition;
        }

        protected virtual void EnumerateDevices()
        {
            foreach (DeviceObjectInstance device in this.DIJoystick.GetObjects())
            {

                this.DIJoystick.GetObjectInfoById(device.ObjectId);

                int offset = device.Offset;
                int usage = device.Usage;
                ObjectAspect aspect = device.Aspect;
                String name = device.Name;

                bool IsAxis = (device.ObjectId.Flags & DeviceObjectTypeFlags.AbsoluteAxis) > 0;
                bool IsButton = (device.ObjectId.Flags & DeviceObjectTypeFlags.Button) > 0;
                bool IsPOV = (device.ObjectId.Flags & DeviceObjectTypeFlags.PointOfViewController) > 0;


                if (IsAxis && Axes.Count < DIJoystick.Capabilities.AxeCount)
                {
                    String axisName;
                    string axisLabel = "Unknown";
                    try
                    {
                        var OffsetAxisName = GetAxisNameForUsage(usage);
                        axisName = $"{AxisPrefix} {OffsetAxisName}";
                        axisLabel = MapDeviceNameToLabel($"{AxisPrefix} {OffsetAxisName}");

                    } catch (ArgumentOutOfRangeException)
                    {
                        Log.Instance.log($"Axis can't be mapped: {DIJoystick.Information.InstanceName} Aspect: {aspect} Offset: {offset} Usage: {usage} Axis: {name} Label: {axisLabel}.", LogSeverity.Error);
                        continue;
                    }
                    Axes.Add(new JoystickDevice() { Name = axisName, Label = axisLabel, Type = JoystickDeviceType.Axis });
                    Log.Instance.log($"Added {DIJoystick.Information.InstanceName} Aspect {aspect} + Offset: {offset} Usage: {usage} Axis: {name} Label: {axisLabel}.", LogSeverity.Debug);

                }
                else if (IsButton)
                {
                    // Use the device.Usage value so this is consistent with how Axes are referenced and avoid ID collisions
                    // when looking up names in the the .joystick.json file.
                    var buttonName = $"{ButtonPrefix} {device.Usage}";
                    var buttonLabel = MapDeviceNameToLabel(buttonName);
                    Buttons.Add(new JoystickDevice() { Name = buttonName, Label = buttonLabel, Type = JoystickDeviceType.Button });
                    Log.Instance.log($"Added {DIJoystick.Information.InstanceName} Aspect: {aspect} Offset: {offset} Usage: {usage} Button: {name} Label: {buttonLabel}.", LogSeverity.Debug);
                }
                else if (IsPOV)
                {
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}U", Label = $"{name} (↑)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}UR", Label = $"{name} (↗)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}R", Label = $"{name} (→)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}DR", Label = $"{name} (↘)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}D", Label = $"{name} (↓)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}DL", Label = $"{name} (↙)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}L", Label = $"{name} (←)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = $"{PovPrefix} {name}UL", Label = $"{name} (↖)", Type = JoystickDeviceType.POV });
                }
                else
                {
                    break;
                }
            }
        }

        public string MapDeviceNameToLabel(string deviceName)
        {
            // First try and look for a custom label.
            var input = Definition?.FindInputByName(deviceName);
            if (input != null)
            {
                return input.Label;
            }

            string result = string.Empty;
             
            if (deviceName.StartsWith(ButtonPrefix))
            {
                result = Buttons.Find(b => b.Name == deviceName)?.Label ?? string.Empty;
            } else if (deviceName.StartsWith(AxisPrefix))
            {
                result = Axes.Find(a => a.Name == deviceName)?.Label ?? string.Empty;
            } else if (deviceName.StartsWith(PovPrefix))
            {
                result = POV.Find(p => p.Name == deviceName)?.Label ?? string.Empty;
            }

            if (result == string.Empty)
                result = deviceName;

            return result;
        }

        public void Connect(IntPtr handle)
        {
            EnumerateDevices();
            EnumerateOutputDevices();
            DIJoystick.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            DIJoystick.Properties.BufferSize = 16;
            DIJoystick.Acquire();            
        }

        virtual protected void EnumerateOutputDevices()
        {
            Lights.Clear();
            
            Definition?.Outputs?.ForEach(output => Lights.Add(new JoystickOutputDevice() { Label = output.Label, Name = output.Name, Byte = output.Byte, Bit = output.Bit }));
            return;
        }

        public List<ListItem> GetAvailableDevices()
        {
            List<ListItem> result = new List<ListItem>();

            GetButtonsSorted().ForEach((item) =>
            {
                result.Add(item.ToListItem());
            });
            GetAxisSorted().ForEach((item) =>
            {
                result.Add(item.ToListItem());
            });
            POV.ForEach((item) =>
            {
                result.Add(item.ToListItem());
            });
            return result;
        }

        private List<JoystickDevice> GetButtonsSorted()
        {
            var buttons = Buttons.ToArray().ToList();
            buttons.Sort(SortByPositionInDefintion);
            return Buttons;
        }

        private List<JoystickDevice> GetAxisSorted()
        {
            var axes = Axes.ToArray().ToList();
            Axes.Sort(SortByPositionInDefintion);

            return Axes;
        }

        public int GetIndexForKey(string key)
        {
            return Definition?.Inputs?.FindIndex(input => input.Name == key) ?? 0;
        }

        int SortByPositionInDefintion(JoystickDevice b1, JoystickDevice b2)
        {
            if (GetIndexForKey(b1.Name) == GetIndexForKey(b2.Name)) return 0;
            if (GetIndexForKey(b1.Name) > GetIndexForKey(b2.Name)) return 1;
            return -1;
        }


        private void Connect()
        {
            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: Definition.VendorId, productID: Definition.ProductId);
                if (Device == null) return;
            }

            Stream = Device.Open();
        }

        public List<ListItem> GetAvailableOutputDevices()
        {
            List<ListItem> result = new List<ListItem>();
            Lights.ForEach((item) =>
            {
                result.Add(item.ToListItem());
            });
            return result;
        }

        public virtual void Update()
        {           
            if (DIJoystick == null) return;

            try
            {
                DIJoystick.Poll();

                JoystickState newState = DIJoystick.GetCurrentState();
                UpdateButtons(newState);
                UpdateAxis(newState);
                UpdatePOV(newState);
                UpdateOutputDeviceStates();

                // at the very end update our state
                State = newState;
            }
            catch(SharpDX.SharpDXException ex)
            {
                if (ex.Descriptor.ApiCode=="InputLost")
                {
                    DIJoystick.Unacquire();
                    OnDisconnected?.Invoke(this, null);
                }
            }
        }

        private void UpdatePOV(JoystickState newState)
        {
            if (POV.Count == 0) return;
            int oldValue = -1;
            if (StateExists()) oldValue = State.PointOfViewControllers[0];
            int newValue = newState.PointOfViewControllers[0];

            int index;

            if (oldValue != newValue)
            {
                if (oldValue > -1)
                {
                    index = (int)Math.Round(oldValue / 4500f);

                    OnButtonPressed?.Invoke(this, new InputEventArgs()
                    {
                        Name = Name,
                        DeviceId = POV[index].Name,
                        DeviceLabel = POV[index].Label,
                        Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                        Type = DeviceType.Button,
                        Value = (int)MobiFlightButton.InputEvent.RELEASE
                    });
                };

                if (newValue > -1)
                {

                    index = (int)Math.Round(newValue / 4500f);

                    OnButtonPressed?.Invoke(this, new InputEventArgs()
                    {
                        Name = Name,
                        DeviceId = POV[index].Name,
                        DeviceLabel = POV[index].Label,
                        Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                        Type = DeviceType.Button,
                        Value = (int)MobiFlightButton.InputEvent.PRESS
                    });
                }
            }
        }

        private void UpdateAxis(JoystickState newState)
        {            
            if (DIJoystick.Capabilities.AxeCount > 0)
            {
                for (int CurrentAxis = 0; CurrentAxis != Axes.Count; CurrentAxis++)
                {
                    
                    int oldValue = 0;
                    if (StateExists())
                    {
                        oldValue = GetValueForAxisFromState(CurrentAxis, State);
                    }
                            
                    int newValue = GetValueForAxisFromState(CurrentAxis, newState);

                    if (!StateExists() || oldValue != newValue)
                        OnButtonPressed?.Invoke(this, new InputEventArgs()
                        {
                            Name = Name,
                            DeviceId = Axes[CurrentAxis].Name,
                            DeviceLabel = Axes[CurrentAxis].Label,
                            Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                            Type = DeviceType.AnalogInput,
                            Value = newValue
                        });

                }
            }
        }

        private void UpdateButtons(JoystickState newState)
        {
            if (Buttons.Count==0) return;

            for (int i = 0; i < newState.Buttons.Length; i++)
            {
                if (!StateExists() || State.Buttons.Length < i || State.Buttons[i] != newState.Buttons[i])
                {
                    if (newState.Buttons[i] || (State != null))
                        OnButtonPressed?.Invoke(this, new InputEventArgs()
                        {
                            Name = Name,
                            DeviceId = Buttons[i].Name,
                            DeviceLabel = Buttons[i].Label,
                            Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                            Type = DeviceType.Button,
                            Value = newState.Buttons[i] ? 0 : 1
                        });
                }
            }
        }

        private int GetValueForAxisFromState(int currentAxis, JoystickState state)
        {
            String RawAxisName = Axes[currentAxis].Name.Replace(AxisPrefix, "").TrimStart();
            if (RawAxisName.Contains("Slider"))
            {
                byte index = 0;
                if (RawAxisName == "Slider2") index = 1;

                return state.Sliders[index];
            }
            return (int)state.GetType().GetProperty(RawAxisName).GetValue(state, null);
        }

        private bool StateExists()
        {
            return State != null;
        }

        public static String GetAxisNameForUsage(int usage)
        {
            if (!UsageMap.ContainsKey(usage))
                throw new ArgumentOutOfRangeException();
            return UsageMap[usage];
        }

        public void SetOutputDeviceState(string name, byte state)
        {
            foreach(var light in Lights)
            {
                if (light.Label != name) continue;
                if (light.State == state) continue;

                light.State = state;
                RequiresOutputUpdate = true;
                return;
            }
        }

        protected virtual void SendData(byte[] data)
        {
            // Don't try and send data if no outputs are defined.
            if (Definition?.Outputs == null || Definition?.Outputs.Count == 0)
            {
                return;
            }

            if (!RequiresOutputUpdate) return;
            if (Stream == null)
            {
                Connect();
            };
            Stream.SetFeature(data);

            RequiresOutputUpdate = false;
        }

        public void UpdateOutputDeviceStates()
        {
            var data = new byte[] { 0, 0, 0, 0, 0 };

            foreach (var light in Lights)
            {
                data[light.Byte] |= (byte)(light.State << light.Bit);
            }

            SendData(data);
        }

        public void Stop()
        {
            foreach(var light in Lights)
            {
                light.State = 0;
            }
            RequiresOutputUpdate = true;
            UpdateOutputDeviceStates();
        }
    }
}
