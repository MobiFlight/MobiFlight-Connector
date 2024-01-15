using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;

namespace MobiFlight.Joysticks.FlightSimBuilder
{
    internal class GNS530 : Joystick
    {
        int VendorId = 0x04D8;
        int ProductId = 0xE89D;
        HidStream Stream { get; set; }
        HidSharp.HidDevice Device { get; set; }

        protected HidDeviceInputReceiver inputReceiver;

        public GNS530(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition) {
        }

        public void Connect()
        {
            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
                if (Device == null) return;
            }

            Stream = Device.Open();
            var reportDescriptor = Device.GetReportDescriptor();
            inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
            inputReceiver.Received += InputReceiver_Received;
            inputReceiver.Start(Stream);
        }

        private void InputReceiver_Received(object sender, System.EventArgs e)
        {
            var inputRec = sender as HidDeviceInputReceiver;
            var inputReportBuffer = new byte[5];
            
            while (inputRec.TryRead(inputReportBuffer, 0, out Report report))
            {
                var newState = Gns530Report.ParseReport(inputReportBuffer).ToJoystickState();
                UpdateButtons(newState);
                // at the very end update our state
                State = newState;
            }
        }

        protected override void SendData(byte[] data)
        {
            /* do nothing */
        }

        public override void Update()
        {
            if (Stream == null || inputReceiver == null)
            {
                Connect();
            };
            // We don't do anything else
            // because we have a callback for
            // handling the incoming reports
            // InputReceiver_Received(inputReceiver, new System.EventArgs());
        }

        public override void Shutdown()
        {
            Stream.Close();
            inputReceiver.Received -= InputReceiver_Received;
            Stream = null;
            inputReceiver = null;
        }
    }
}
