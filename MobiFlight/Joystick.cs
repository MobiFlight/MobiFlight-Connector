using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace MobiFlight
{
    public class Joystick
    {
        public static string ButtonPrefix = "Button ";
        public static string AxisPrefix = "Axis";
        public static string SerialPrefix = "JS-";
        public event ButtonEventHandler OnButtonPressed;
        public event ButtonEventHandler OnAxisChanged;
        private SharpDX.DirectInput.Joystick joystick;
        JoystickState state = null;
        Dictionary<String, float> Buttons = new Dictionary<string, float>();
        Dictionary<String, float> Axes = new Dictionary<string, float>();
        public static string[] AxisNames = { "X", "Y", "Z", "RotationX", "RotationY", "RotationZ" };

        public static bool IsJoystickSerial(string serial)
        {
            return (serial != null && serial.StartsWith(SerialPrefix));
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

        public void Connect(IntPtr handle)
        {
            joystick.SetCooperativeLevel(handle, CooperativeLevel.Background | CooperativeLevel.NonExclusive);
            joystick.Properties.BufferSize = 16;
            joystick.Acquire();
        }

        public List<String> GetAvailableDevices()
        {
            List<String> result = new List<string>();

            for (int i = 0; i != joystick.Capabilities.ButtonCount; i++)
            {
                result.Add(ButtonPrefix + i);
            }

            for (int i = 0; i != joystick.Capabilities.AxeCount; i++)
            {
                result.Add(AxisPrefix + AxisNames[i]);
            }

            return result;
        }

        public void Update()
        {
            if (joystick == null) return;
            joystick.Poll();
            JoystickState newState = joystick.GetCurrentState();
            for(int i=0; i!= newState.Buttons.Length; i++)
            {
                if (state==null || state.Buttons.Length < i || state.Buttons[i] != newState.Buttons[i])
                {
                    if (newState.Buttons[i] || (state != null))
                        OnButtonPressed?.Invoke(this, new InputEventArgs()
                        {
                            DeviceId = ButtonPrefix + i,
                            Serial = SerialPrefix + joystick.Information.InstanceGuid.ToString(),
                            Type = DeviceType.Button,
                            Value = newState.Buttons[i] ? 0 : 1
                        });
                }
            }

            if (joystick.Capabilities.AxeCount > 0)
            {

                for (int i = 0; i != joystick.Capabilities.AxeCount; i++)
                {
                    if (state == null || state.GetType().GetProperty(AxisNames[i]) != null)
                    {
                        int oldValue = 0;
                        if (state!=null)
                            oldValue = (int)state.GetType().GetProperty(AxisNames[i]).GetValue(state, null);
                        int newValue = (int)newState.GetType().GetProperty(AxisNames[i]).GetValue(newState, null);

                        if (state == null || oldValue != newValue)
                            OnButtonPressed?.Invoke(this, new InputEventArgs()
                            {
                                DeviceId = AxisPrefix + AxisNames[i],
                                Serial = SerialPrefix + joystick.Information.InstanceGuid.ToString(),
                                Type = DeviceType.AnalogInput,
                                Value = newValue
                            });                        
                    }
                }               
            }

            // at the very end update our state
            state = newState;
        }
    }
}
