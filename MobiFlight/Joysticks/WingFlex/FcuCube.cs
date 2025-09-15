using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using System;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class FcuCube : Joystick
    {
        HidDevice Device { get; set; }
        HidStream Stream { get; set; }

        protected HidDeviceInputReceiver inputReceiver;
        protected ReportDescriptor reportDescriptor;

        private readonly FcuCubeReport FcuCubeReport;

        public FcuCube(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
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
            var inputReportBuffer = new byte[5];

            try
            {
                while (inputRec.TryRead(inputReportBuffer, 0, out _))
                {
                    var newState = FcuCubeReport.Parse(inputReportBuffer).ToJoystickState();
                    UpdateButtons(newState);
                    UpdateAxis(newState);

                    // Finally store the new state as last state
                    State = newState;
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log(ex.Message, LogSeverity.Error);
            }
        }

        protected override void SendData(byte[] data)
        {
            if (!RequiresOutputUpdate) return;
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
            var data = FcuCubeReport.FromOutputDeviceState(Lights);

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
