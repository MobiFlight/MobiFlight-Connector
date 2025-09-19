using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using SharpDX.DirectInput;
using System;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class FcuCube : Joystick
    {
        HidDevice Device { get; set; }
        HidStream Stream { get; set; }

        protected HidDeviceInputReceiver inputReceiver;
        protected ReportDescriptor reportDescriptor;

        private readonly FcuCubeReport FcuCubeReport = new FcuCubeReport();
        public override string Name
        {
            get { return Definition?.InstanceName ?? "FcuCube"; }
        }

        public override string Serial
        {
            get { return SerialPrefix + Device?.GetSerialNumber() ?? SerialPrefix + "FCU-Cube-Serial-Nummer-Dummy"; }
        }

        public FcuCube(JoystickDefinition definition) : base(null, definition)
        {
        }

        protected bool Connect()
        {
            var VendorId = Definition.VendorId;
            var ProductId = Definition.ProductId;

            var device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);

            if (device != Device)
            {
                Log.Instance.log($"FcuCube found with VID:{VendorId.ToString("X4")} and PID:{ProductId.ToString("X4")}", LogSeverity.Info);
                if (Stream != null)
                {
                    Stream.Close();
                    Stream = null;
                    Device = null;
                }
            }

            if (device == null)
            {
                Log.Instance.log($"no FcuCube found with VID:{VendorId.ToString("X4")} and PID:{ProductId.ToString("X4")}", LogSeverity.Info);
                return false;
            }

            Device = device;

            Stream = device.Open();
            var reportDescriptor = device.GetReportDescriptor();
            inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
            inputReceiver.Received += InputReceiver_Received;
            inputReceiver.Start(Stream);

            return true;
        }

        private void InputReceiver_Received(object sender, EventArgs e)
        {
            var inputRec = sender as HidDeviceInputReceiver;
            var inputReportBuffer = new byte[65];

            try
            {
                var newState = new JoystickState();
                while (inputRec.TryRead(inputReportBuffer, 0, out _)) {
                    newState = FcuCubeReport.Parse(inputReportBuffer).ToJoystickState();
                }

                UpdateButtons(newState);
                UpdateAxis(newState);

                // Finally store the new state as last state
                State = newState;
            }
            catch (Exception ex)
            {
                Log.Instance.log(ex.Message, LogSeverity.Error);
            }
        }

        protected override void SendData(byte[] data)
        {
            if (Stream == null)
            {
                Connect();
            }

            Stream.Write(data);
            RequiresOutputUpdate = false;
        }

        public override void Update()
        {
            if (Stream != null && inputReceiver != null) return;
            Connect();
            UpdateOutputDeviceStates();
        }

        public override void UpdateOutputDeviceStates()
        {
            // if (!RequiresOutputUpdate) return;
            var data = FcuCubeReport?.FromOutputDeviceState(Lights);

            if (data == null) return;

            try
            {
                SendData(data);
            }
            catch (System.IO.IOException)
            {
                // this happens when the device is removed.
                OnDeviceRemoved();
            }
        }

        protected override void EnumerateDevices()
        {
            Definition.Inputs.ForEach(d => { 
                var device = new JoystickDevice() { Name = d.Name, Label = d.Label, JoystickDeviceType = d.Type };
                switch (d.Type)
                {
                    case JoystickDeviceType.Axis:
                        device.Type = DeviceType.AnalogInput;
                        Axes.Add(device);
                        break;
                    case JoystickDeviceType.Button:
                        device.Type = DeviceType.Button;
                        Buttons.Add(device);
                        break;
                    case JoystickDeviceType.POV:
                        device.Type = DeviceType.Button;
                        POV.Add(device);
                        break;
                }
            });
        }

        protected override void EnumerateOutputDevices()
        {
            base.EnumerateOutputDevices();

            // LcdDisplays
            Definition?.Outputs?.FindAll(d => d.Type==DeviceType.LcdDisplay.ToString()).ForEach(device =>
            {
                Lights.Add(new JoystickOutputDevice() { Name = device.Id, Label = device.Label, Type = DeviceType.LcdDisplay });
            });
        }

        public override void Shutdown()
        {
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }

            if (inputReceiver != null)
            {
                inputReceiver.Received -= InputReceiver_Received;
                inputReceiver = null;
            }
        }
    }
}
