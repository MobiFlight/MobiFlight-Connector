using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;

namespace MobiFlight.Joysticks.FlightSimBuilder
{
    internal class FlightSimBuilderJoystick : Joystick
    {
        int VendorId = 0x04D8;
        int ProductId = 0xE89D;
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }

        IReportParser ReportParser { get; set; }

        protected HidDeviceInputReceiver inputReceiver;

        public FlightSimBuilderJoystick(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition) {
            VendorId = definition.VendorId;
            ProductId = definition.ProductId;

            ReportParser = new Gns530Report();
            if (definition.InstanceName == "FlightSimBuilder G1000 PFD")
            {
                ReportParser = new Gns1000Report();
            }
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
            var inputReportBuffer = new byte[12];
            
            while (inputRec.TryRead(inputReportBuffer, 0, out Report report))
            {
                var newState = ReportParser.Parse(inputReportBuffer).ToJoystickState();
                UpdateButtons(newState);
                UpdateAxis(newState);
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
