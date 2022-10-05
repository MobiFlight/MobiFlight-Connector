using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight
{
    public class JoystickManager
    {
        public event EventHandler Connected;
        public event ButtonEventHandler OnButtonPressed;
        Timer PollTimer = new Timer();
        List<Joystick> joysticks = new List<Joystick>();

        public JoystickManager ()
        {
            PollTimer.Interval = 50;
            PollTimer.Tick += PollTimer_Tick;
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
            } catch (InvalidOperationException ex)
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
            SharpDX.DirectInput.DirectInput di = new SharpDX.DirectInput.DirectInput();
            joysticks?.Clear();

            List<SharpDX.DirectInput.DeviceInstance> devices = di.GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly).ToList();

            foreach (DeviceInstance d in devices)
            {
                Log.Instance.log("Found attached DirectInput Device: " + d.InstanceName + ", Type: " + d.Type.ToString() + ", SubType: " + d.Subtype, LogSeverity.Debug);

                if (!IsSupportedDeviceType(d)) continue;

                MobiFlight.Joystick js;
                if (d.InstanceName == "Bravo Throttle Quadrant")
                {
                    js = new Joysticks.HoneycombBravo(new SharpDX.DirectInput.Joystick(di, d.InstanceGuid));
                } else
                    js = new Joystick(new SharpDX.DirectInput.Joystick(di, d.InstanceGuid));

                if (!HasAxisOrButtons(js)) continue;

                Log.Instance.log("Adding attached Joystick Device: " + d.InstanceName + " Buttons " + js.Capabilities.ButtonCount + ", Axis: " + js.Capabilities.AxeCount, LogSeverity.Debug);
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
            Joystick js = sender as Joystick;
            Log.Instance.log("Joystick Disconnected: " + js.Name, LogSeverity.Debug);
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
            Joystick result = null;

            result = joysticks.Find((Joystick js) => { return (js.Serial == serial); });

            return result;
        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();

            result["Joysticks.Count"] = joysticks.Count();
            
            foreach (Joystick joystick in joysticks)
            {
                string key = "Joysticks.Model." + joystick.Name;

                if (!result.ContainsKey(key)) result[key] = 0;
                result[key] += 1;
            }

            return result;
        }
    }
}
