using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using MobiFlight.Config;
using MobiFlight.Joysticks.Logitech;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks
{
    internal class HidDevice : IHidDevice
    {
        public static readonly string ButtonPrefix = "Button";
        public static readonly string AxisPrefix = "Axis";
        public static readonly string PovPrefix = "POV";
        public static readonly string SerialPrefix = "JS-";

        public string Name { get { return Definition.InstanceName; }}
        public string Serial { get { return $"{SerialPrefix}{Definition.InstanceName}"; } }

        protected JoystickState State = null;
        protected List<JoystickDevice> Buttons = new List<JoystickDevice>();
        private readonly List<JoystickDevice> Axes = new List<JoystickDevice>();
        private readonly List<JoystickDevice> POV = new List<JoystickDevice>();
        private readonly List<JoystickOutputDevice> Outputs = new List<JoystickOutputDevice>();

        public event ButtonEventHandler OnButtonPressed;

        protected readonly JoystickDefinition Definition;
        protected bool RequiresOutputUpdate = false;
        private HidStream Stream;
        private HidSharp.HidDevice Device;

        protected HidDeviceInputReceiver inputReceiver;

        public HidDevice(JoystickDefinition definition)
        {
            Definition = definition;
        }

        public void Connect()
        {
            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: Definition.VendorId, productID: Definition.ProductId);
                if (Device == null) return;
            }

            Stream = Device.Open();
            var reportDescriptor = Device.GetReportDescriptor();
            InitializeDevicesWithDescriptor(reportDescriptor);
            inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
            inputReceiver.Received += InputReceiver_Received;
            inputReceiver.Start(Stream);
        }

        private void InitializeDevicesWithDescriptor(ReportDescriptor reportDescriptor)
        {
            reportDescriptor.Reports.ToList().ForEach(report =>
            {
                if (report.ReportType == ReportType.Input)
                {
                    report.DataItems.ToList().ForEach(data =>
                    {
                        if (data.ExpectedUsageType == ExpectedUsageType.PushButton)
                        {
                            for (var i = 0; i < data.ElementCount; i++)
                            {
                                var buttonName = $"{ButtonPrefix} {i}";
                                var buttonLabel = MapDeviceNameToLabel(buttonName);
                                Buttons.Add(new JoystickDevice() { Name = buttonName, Label = buttonLabel, Type = DeviceType.Button, JoystickDeviceType = JoystickDeviceType.Button });
                            }
                        }
                    });
                }

                if (report.ReportType == ReportType.Feature)
                {
                    // Do something with the feature information from the json file
                }

                if (report.ReportType == ReportType.Output)
                {
                    // Do something with the output information from the json file
                }
            });
            reportDescriptor.DeviceItems.ToList().ForEach(item => {
                var parser = item.CreateDeviceItemInputParser();
            });

            EnumerateOutputDevices();
        }

        virtual protected void EnumerateOutputDevices()
        {
            Outputs.Clear();

            Definition?.Outputs?.ForEach(output => {
                JoystickOutputDevice device = new JoystickOutputDevice() { Label = output.Label, Name = output.Id, Byte = output.Byte, Bit = output.Bit };
                if (output.Type=="Custom")
                {
                    device = new JoystickStringOutputDevice(device);
                }
                Outputs.Add(device);
            });
            return;
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
            }
            else if (deviceName.StartsWith(AxisPrefix))
            {
                result = Axes.Find(a => a.Name == deviceName)?.Label ?? string.Empty;
            }
            else if (deviceName.StartsWith(PovPrefix))
            {
                result = POV.Find(p => p.Name == deviceName)?.Label ?? string.Empty;
            }

            if (result == string.Empty)
                result = deviceName;

            return result;
        }

        protected void UpdateButtons(JoystickState newState)
        {
            if (Buttons.Count == 0) return;

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
                            Serial = Serial,
                            Type = DeviceType.Button,
                            Value = newState.Buttons[i] ? 0 : 1
                        });
                }
            }
        }

        private void InputReceiver_Received(object sender, System.EventArgs e)
        {
            var inputRec = sender as HidDeviceInputReceiver;
            var inputReportBuffer = new byte[32];

            while (inputRec.TryRead(inputReportBuffer, 0, out Report report))
            {
                var newState = MultipanelReport.ParseReport(inputReportBuffer).ToJoystickState();
                UpdateButtons(newState);
                // at the very end update our state
                State = newState;
            }
        }

        private bool StateExists()
        {
            return State != null;
        }

        public void SetOutputDeviceState(string name, byte state)
        {
            foreach (var light in Outputs.FindAll(l => l.JoystickDeviceType == JoystickDeviceType.Light))
            {
                if (light.Label != name) continue;
                if (light.State == state) continue;

                light.State = state;
                RequiresOutputUpdate = true;
                break;
            }
        }
        public void SetCustomDeviceState(string name, string value)
        {
            foreach (var light in Outputs.FindAll(l => l.JoystickDeviceType == JoystickDeviceType.String))
            {
                //
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
            var data = new byte[] { 0, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x01, 0, 0xff };
            var mappingTable = new Dictionary<string, byte>()
            {
                { " ", 0b00001111 },
                { "0", 0x00 },
                { "1", 0x01 },
                { "2", 0x02 },
                { "3", 0x03 },
                { "4", 0x04 },
                { "5", 0x05 },
                { "6", 0x06 },
                { "7", 0x07 },
                { "8", 0x08 },
                { "9", 0x09 },
            };

            foreach (var light in Outputs.FindAll(l=>l.JoystickDeviceType==JoystickDeviceType.Light))
            {
                data[light.Byte] |= (byte)(light.State << light.Bit);
            }

            foreach (var stringOutput in Outputs.FindAll(l=>l.JoystickDeviceType==JoystickDeviceType.String))
            {
                var strOutputDevice = stringOutput as JoystickStringOutputDevice;
                if (strOutputDevice == null) continue;
                if (strOutputDevice.StringState == null) continue;
                for (var i = 0; i!= strOutputDevice.StringState.Length; i++)
                {
                    var value = mappingTable.ContainsKey(strOutputDevice.StringState[i].ToString()) ? mappingTable[strOutputDevice.StringState[i].ToString()] : (byte)0b00001111;
                    data[i + strOutputDevice.Byte] = value;
                }
            }
            RequiresOutputUpdate = true;

            SendData(data);
        }

        public virtual void Update()
        {
            if (Stream == null || inputReceiver == null)
            {
                Connect();
            };
        }

        public virtual void Shutdown()
        {
            Stream.Close();
            inputReceiver.Received -= InputReceiver_Received;
            Stream = null;
            inputReceiver = null;
        }

        public List<ListItem<IBaseDevice>> GetAvailableDevicesAsListItems()
        {
            List<ListItem<IBaseDevice>> result = new List<ListItem<IBaseDevice>>();

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

        public List<ListItem<IBaseDevice>> GetAvailableOutputDevicesAsListItems()
        {
            List<ListItem<IBaseDevice>> result = new List<ListItem<IBaseDevice>>();
            Outputs.ForEach((item) =>
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

        public void Stop()
        {
            foreach (var light in Outputs)
            {
                light.State = 0;
            }
            RequiresOutputUpdate = true;
            UpdateOutputDeviceStates();
        }

        
    }
}
