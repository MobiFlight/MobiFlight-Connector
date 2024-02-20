using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;

namespace MobiFlight.Joysticks.Octavi
{
    internal class Octavi : Joystick
    {
        readonly int VendorId = 0x04D8;
        readonly int ProductId = 0xE6D6;
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }

        protected HidDeviceInputReceiver inputReceiver;
        protected ReportDescriptor reportDescriptor;
        private readonly OctaviHandler octaviHandler = new OctaviHandler();

        public Octavi(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {

        }

        public void Connect()
        {
            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
                if (Device == null) return;
            }

            Stream = Device.Open();
            Stream.ReadTimeout = System.Threading.Timeout.Infinite;
            reportDescriptor = Device.GetReportDescriptor();
            inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
            inputReceiver.Received += InputReceiver_Received;
            inputReceiver.Start(Stream);
        }

        private void InputReceiver_Received(object sender, System.EventArgs e)
        {
            var inputReceiver = sender as HidDeviceInputReceiver;
            byte[] inputReportBuffer = new byte[8];

            while (inputReceiver.TryRead(inputReportBuffer, 0, out _))
            {
                OctaviReport report = new OctaviReport();
                report.parseReport(inputReportBuffer);
                var buttonEvents = octaviHandler.DetectButtonEvents(report);
                foreach (var (buttonIndex, inputEvent) in buttonEvents)
                {
                    TriggerButtonPress(buttonIndex, inputEvent);
                }
            }
        }

        protected override void SendData(byte[] data)
        {
            if (!RequiresOutputUpdate) return;
            if (Stream == null)
            {
                Connect();
            };
            data[0] = 11;
            Stream.Write(data, 0, 2);
            RequiresOutputUpdate = false;
        }

        protected void TriggerButtonPress(int i, MobiFlightButton.InputEvent inputEvent)
        {
            TriggerButtonPressed(this, new InputEventArgs()
            {
                Name = Name,
                DeviceId = Buttons[i].Name,
                DeviceLabel = Buttons[i].Label,
                Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                Type = DeviceType.Button,
                Value = (int)inputEvent
            });
        }

        public override void Update()
        {
            //if (Stream == null || inputReceiver == null)
            //{
            //    Connect();
            //};
            // We don't do anything else
            // because we have a callback for
            // handling the incoming reports
        }

        protected override void EnumerateDevices()
        {
            foreach (string entry in octaviHandler.JoystickButtonNames)
            {
                Buttons.Add(new JoystickDevice() { Name = entry, Label = entry, Type = DeviceType.Button, JoystickDeviceType = JoystickDeviceType.Button });
            }
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
