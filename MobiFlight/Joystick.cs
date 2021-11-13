using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace MobiFlight
{
    public enum JoystickDeviceType
    {
        Button,
        Axis,
        POV
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


    public class Joystick
    {
        public static string ButtonPrefix = "Button ";
        public static string AxisPrefix = "Axis ";
        public static string PovPrefix = "POV ";
        public static string SerialPrefix = "JS-";
        public event ButtonEventHandler OnButtonPressed;
        public event ButtonEventHandler OnAxisChanged;
        public event EventHandler OnDisconnected;
        private SharpDX.DirectInput.Joystick joystick;
        JoystickState state = null;
        List<JoystickDevice> Buttons = new List<JoystickDevice>();
        List<JoystickDevice> Axes = new List<JoystickDevice>();
        List<JoystickDevice> POV = new List<JoystickDevice>();
        public static string[] AxisNames = { "X", "Y", "Z", "RotationX", "RotationY", "RotationZ", "Slider1", "Slider2"};

        public static bool IsJoystickSerial(string serial)
        {
            return (serial != null && serial.Contains(SerialPrefix));
        }

        public string Name { 
            get { return joystick?.Information.InstanceName; }  
        }

        public string Serial
        {
            get { return SerialPrefix + joystick.Information.InstanceGuid;  }
        }

        public SharpDX.DirectInput.DeviceType Type
        {
            get { return joystick.Information.Type; }
        }

        public Capabilities Capabilities {
            get {
                return this.joystick.Capabilities;
            }
        }

        public Joystick(SharpDX.DirectInput.Joystick joystick)
        {
            this.joystick = joystick;
        }

        private void EnumerateDevices()
        {
            foreach (DeviceObjectInstance device in this.joystick.GetObjects())
            {

                this.joystick.GetObjectInfoById(device.ObjectId);


                int offset = device.Offset;
                int usage = device.Usage;
                ObjectAspect aspect = device.Aspect;
                String name = device.Name;

                bool IsAxis = (device.ObjectId.Flags & DeviceObjectTypeFlags.AbsoluteAxis) > 0;
                bool IsButton = (device.ObjectId.Flags & DeviceObjectTypeFlags.Button) > 0;
                bool IsPOV = (device.ObjectId.Flags & DeviceObjectTypeFlags.PointOfViewController) > 0;


                if (IsAxis && Axes.Count < joystick.Capabilities.AxeCount)
                {
                    String OffsetAxisName = "Unknown";
                    try
                    {
                        OffsetAxisName = GetAxisNameForUsage(usage);
                    } catch (ArgumentOutOfRangeException ex)
                    {
                        Log.Instance.log("EnumerateDevices: Axis can't be mapped:" + joystick.Information.InstanceName + ": Aspect : " + aspect.ToString() + ":Offset:" + offset + ":Usage:" + usage + ":" + "Axis: " + name, LogSeverity.Error);
                        continue;
                    }
                    Axes.Add(new JoystickDevice() { Name = AxisPrefix + OffsetAxisName, Label = name, Type = JoystickDeviceType.Axis });
                    Log.Instance.log("EnumerateDevices: " + joystick.Information.InstanceName + ": Aspect : " + aspect.ToString() + ":Offset:" + offset + ":Usage:" + usage + ":" + "Axis: " + name, LogSeverity.Debug);

                }
                else if (IsButton)
                {
                    String ButtonName = CorrectButtonIndexForButtonName(name, Buttons.Count + 1);
                    Buttons.Add(new JoystickDevice() { Name = ButtonPrefix + (Buttons.Count + 1), Label = ButtonName, Type = JoystickDeviceType.Button });
                    Log.Instance.log("EnumerateDevices: " + joystick.Information.InstanceName + ": Aspect : " + aspect.ToString() + ":Offset:" + offset + ":Usage:" + usage + ":" + "Button: " + name, LogSeverity.Debug);
                }
                else if (IsPOV)
                {
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (↑)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (↗)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (→)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (↘)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (↓)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (↙)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (←)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name, Label = name + " (↖)", Type = JoystickDeviceType.POV });
                }
                else
                {
                    break;
                }
            }
        }

        private string CorrectButtonIndexForButtonName(string name, int v)
        {
            return Regex.Replace(name, @"\d+", v.ToString()).ToString();
        }

        public void Connect(IntPtr handle)
        {
            EnumerateDevices();
            joystick.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            joystick.Properties.BufferSize = 16;
            joystick.Acquire();            
        }

        public List<ListItem> GetAvailableDevices()
        {
            List<ListItem> result = new List<ListItem>();
            Buttons.ForEach((item) =>
            {
                result.Add(item.ToListItem());
            });
            Axes.ForEach((item) =>
            {
                result.Add(item.ToListItem());
            });
            POV.ForEach((item) =>
            {
                result.Add(item.ToListItem());
            });
            return result;
        }

        public void Update()
        {           
            if (joystick == null) return;

            try
            {
                joystick.Poll();
                JoystickState newState = joystick.GetCurrentState();
                UpdateButtons(newState);
                UpdateAxis(newState);
                UpdatePOV(newState);

                // at the very end update our state
                state = newState;
            }
            catch(SharpDX.SharpDXException ex)
            {
                if (ex.Descriptor.ApiCode=="InputLost")
                {
                    joystick.Unacquire();
                    OnDisconnected?.Invoke(this, null);
                }
            }
        }

        private void UpdatePOV(JoystickState newState)
        {
            if (POV.Count == 0) return;
            int oldValue = -1;
            if (StateExists()) oldValue = state.PointOfViewControllers[0];
            int newValue = newState.PointOfViewControllers[0];

            int index = 0;

            if (oldValue != newValue)
            {
                if (oldValue > -1)
                {
                    index = (int)Math.Round(oldValue / 4500f);

                    OnButtonPressed?.Invoke(this, new InputEventArgs()
                    {
                        DeviceId = POV[index].Label,
                        Serial = SerialPrefix + joystick.Information.InstanceGuid.ToString(),
                        Type = DeviceType.Button,
                        Value = (int)MobiFlightButton.InputEvent.RELEASE
                    });
                };

                if (newValue > -1)
                {

                    index = (int)Math.Round(newValue / 4500f);

                    OnButtonPressed?.Invoke(this, new InputEventArgs()
                    {
                        DeviceId = POV[index].Label,
                        Serial = SerialPrefix + joystick.Information.InstanceGuid.ToString(),
                        Type = DeviceType.Button,
                        Value = (int)MobiFlightButton.InputEvent.PRESS
                    });
                }
            }
        }

        private void UpdateAxis(JoystickState newState)
        {            
            if (joystick.Capabilities.AxeCount > 0)
            {
                for (int CurrentAxis = 0; CurrentAxis != Axes.Count; CurrentAxis++)
                {
                    
                    int oldValue = 0;
                    if (StateExists())
                    {
                        oldValue = GetValueForAxisFromState(CurrentAxis, state);
                    }
                            
                    int newValue = GetValueForAxisFromState(CurrentAxis, newState);

                    if (!StateExists() || oldValue != newValue)
                        OnButtonPressed?.Invoke(this, new InputEventArgs()
                        {
                            DeviceId = Axes[CurrentAxis].Label,
                            Serial = SerialPrefix + joystick.Information.InstanceGuid.ToString(),
                            Type = DeviceType.AnalogInput,
                            Value = newValue
                        });

                }
            }
        }

        private void UpdateButtons(JoystickState newState)
        {
            for (int i = 0; i != newState.Buttons.Length; i++)
            {
                if (!StateExists() || state.Buttons.Length < i || state.Buttons[i] != newState.Buttons[i])
                {
                    if (newState.Buttons[i] || (state != null))
                        OnButtonPressed?.Invoke(this, new InputEventArgs()
                        {
                            DeviceId = Buttons[i].Label,
                            Serial = SerialPrefix + joystick.Information.InstanceGuid.ToString(),
                            Type = DeviceType.Button,
                            Value = newState.Buttons[i] ? 0 : 1
                        });
                }
            }
        }

        private int GetValueForAxisFromState(int currentAxis, JoystickState state)
        {
            String RawAxisName = Axes[currentAxis].Name.Replace(AxisPrefix, "");
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
            return state != null;
        }

        private String GetAxisNameForUsage(int usage)
        {
            Dictionary<int, string> UsageMap = new Dictionary<int, string>();
            UsageMap[48] = "X";
            UsageMap[49] = "Y";
            UsageMap[50] = "Z";
            UsageMap[51] = "RotationX";
            UsageMap[52] = "RotationY";
            UsageMap[53] = "RotationZ";
            UsageMap[54] = "Slider1";
            UsageMap[55] = "Slider2";

            if (!UsageMap.ContainsKey(usage))
                throw new ArgumentOutOfRangeException();
            return UsageMap[usage];
        }
    }
}
