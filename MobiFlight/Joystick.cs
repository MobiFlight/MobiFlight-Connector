﻿using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
        public static string ButtonPrefix = "Button ";
        public static string AxisPrefix = "Axis ";
        public static string PovPrefix = "POV ";
        public static string SerialPrefix = "JS-";
        public event ButtonEventHandler OnButtonPressed;
        public event ButtonEventHandler OnAxisChanged;
        public event EventHandler OnDisconnected;
        private SharpDX.DirectInput.Joystick joystick;
        JoystickState state = null;
        protected List<JoystickDevice> Buttons = new List<JoystickDevice>();
        protected List<JoystickDevice> Axes = new List<JoystickDevice>();
        protected List<JoystickDevice> POV = new List<JoystickDevice>();
        protected List<JoystickOutputDevice> Lights = new List<JoystickOutputDevice>();
        protected bool RequiresOutputUpdate = false;
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

        protected virtual void EnumerateDevices()
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
                    var FriendlyAxisName = name;
                    try
                    {
                        OffsetAxisName = GetAxisNameForUsage(usage);
                        FriendlyAxisName = GetFriendlyAxisName(name);

                    } catch (ArgumentOutOfRangeException ex)
                    {
                        Log.Instance.log($"Axis can't be mapped: {joystick.Information.InstanceName} Aspect: {aspect} Offset: {offset} Usage: {usage} Axis: {name} Label: {FriendlyAxisName}.", LogSeverity.Error);
                        continue;
                    }
                    Axes.Add(new JoystickDevice() { Name = AxisPrefix + OffsetAxisName, Label = FriendlyAxisName, Type = JoystickDeviceType.Axis });
                    Log.Instance.log($"Added {joystick.Information.InstanceName} Aspect {aspect} + Offset: {offset} Usage: {usage} Axis: {name} Label: {FriendlyAxisName}.", LogSeverity.Debug);

                }
                else if (IsButton)
                {
                    String ButtonName = GetFriendlyButtonName(name, Buttons.Count + 1);
                    Buttons.Add(new JoystickDevice() { Name = ButtonPrefix + (Buttons.Count + 1), Label = ButtonName, Type = JoystickDeviceType.Button });
                    Log.Instance.log($"Added {joystick.Information.InstanceName} Aspect: {aspect} Offset: {offset} Usage: {usage} Button: {name} Label: {ButtonName}.", LogSeverity.Debug);
                }
                else if (IsPOV)
                {
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "U", Label = name + " (↑)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "UR", Label = name + " (↗)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "R", Label = name + " (→)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "DR", Label = name + " (↘)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "D", Label = name + " (↓)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "DL", Label = name + " (↙)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "L", Label = name + " (←)", Type = JoystickDeviceType.POV });
                    POV.Add(new JoystickDevice() { Name = PovPrefix + name + "UL", Label = name + " (↖)", Type = JoystickDeviceType.POV });
                }
                else
                {
                    break;
                }
            }
        }

        protected string GetFriendlyAxisName(string name)
        {
            return MapDeviceNameToLabel(name);
        }

        protected string GetFriendlyButtonName(string name, int v)
        {
            return MapDeviceNameToLabel(Regex.Replace(name, @"\d+", v.ToString()).ToString());
        }

        public virtual string MapDeviceNameToLabel(string deviceName)
        {
            var result = deviceName;

            if(deviceName.StartsWith(ButtonPrefix))
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
            joystick.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            joystick.Properties.BufferSize = 16;
            joystick.Acquire();            
        }

        virtual protected void EnumerateOutputDevices()
        {
            Lights.Clear();
            return;
        }

        public List<ListItem> GetAvailableDevices()
        {
            List<ListItem> result = new List<ListItem>();

            GetButtonsSorted().ForEach((item) =>
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

        protected virtual List<JoystickDevice> GetButtonsSorted()
        {
            return Buttons;
        }

        protected virtual List<JoystickDevice> GetAxisSorted()
        {
            return Axes;
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
                UpdateOutputDeviceStates();

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
                        Name = Name,
                        DeviceId = POV[index].Name,
                        DeviceLabel = POV[index].Label,
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
                        Name = Name,
                        DeviceId = POV[index].Name,
                        DeviceLabel = POV[index].Label,
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
                            Name = Name,
                            DeviceId = Axes[CurrentAxis].Name,
                            DeviceLabel = Axes[CurrentAxis].Label,
                            Serial = SerialPrefix + joystick.Information.InstanceGuid.ToString(),
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
                if (!StateExists() || state.Buttons.Length < i || state.Buttons[i] != newState.Buttons[i])
                {
                    if (newState.Buttons[i] || (state != null))
                        OnButtonPressed?.Invoke(this, new InputEventArgs()
                        {
                            Name = Name,
                            DeviceId = Buttons[i].Name,
                            DeviceLabel = Buttons[i].Label,
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

        virtual public void Stop()
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
