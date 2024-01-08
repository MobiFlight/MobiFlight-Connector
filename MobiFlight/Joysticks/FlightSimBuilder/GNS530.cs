using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.FlightSimBuilder
{
    internal class GNS530 : Joystick
    {
        int VendorId = 0x04D8;
        int ProductId = 0xE89D;
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }

        protected HidSharp.Reports.Input.HidDeviceInputReceiver inputReceiver;
        protected ReportDescriptor reportDescriptor;

        protected Dictionary<string, string> OctaviButtons = new Dictionary<string, string>();

        Gns530Handler octaviHandler = new Gns530Handler();

        public GNS530(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition) {

            OctaviButtons = new Dictionary<string, string>()
            {
                { "Button_COM1_OI", "COM1 Outer +" }
            };
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
            var inputRec = sender as HidDeviceInputReceiver;
            byte[] inputReportBuffer = new byte[1+2+9];
            
            while (inputRec.TryRead(inputReportBuffer, 0, out _))
            {
                Gns530Report OReport = new Gns530Report();
                OReport.parseReport(inputReportBuffer);
                List<int> btnz = octaviHandler.toButton(OReport);
                foreach (int i in btnz) {
                    // TriggerButtonPress(i);
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

        protected void TriggerButtonPress(int i)
        {
            TriggerButtonPressed(this, new InputEventArgs()
            {
                Name = Name,
                DeviceId = Buttons[i].Name,
                DeviceLabel = Buttons[i].Label,
                Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                Type = DeviceType.Button,
                Value = 0
            });
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
        }

        protected override void EnumerateDevices()
        {
            foreach (string entry in octaviHandler.OctaviButtonList)
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
