using HidSharp;
using HidSharp.Reports;
using SharpDX.DirectInput;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.Octavi
{
    internal class Octavi : Joystick
    {
        int VendorId = 0x04D8;
        int ProductId = 0xE6D6;
        HidStream Stream { get; set; }
        HidDevice Device { get; set; }

        protected HidSharp.Reports.Input.HidDeviceInputReceiver inputReceiver;
        protected ReportDescriptor reportDescriptor;

        protected Dictionary<string, string> OctaviButtons = new Dictionary<string, string>();

        OctaviHandler octaviHandler = new OctaviHandler();

        public Octavi(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition) {

            OctaviButtons  = new Dictionary<string, string>()
            {
                { "Button_COM1_OI", "COM1 Outer +" },
                { "Button_COM1_OD", "COM1 Outer -" },
                { "Button_COM1_II", "COM1 Inner +" },
                { "Button_COM1_ID", "COM1 Inner -" },
                { "Button_COM1_TOG", "COM1 Toggle" },
                { "Button_COM2_OI", "COM2 Outer +" },
                { "Button_COM2_OD", "COM2 Outer -" },
                { "Button_COM2_II", "COM2 Inner +" },
                { "Button_COM2_ID", "COM2 Inner -" },
                { "Button_COM2_TOG", "COM2 Toggle" },
                { "Button_NAV1_OI", "NAV1 Outer +" },
                { "Button_NAV1_OD", "NAV1 Outer -" },
                { "Button_NAV1_II", "NAV1 Inner +" },
                { "Button_NAV1_ID", "NAV1 Inner -" },
                { "Button_NAV1_TOG", "NAV1 Toggle" },
                { "Button_NAV2_OI", "NAV2 Outer +" },
                { "Button_NAV2_OD", "NAV2 Outer -" },
                { "Button_NAV2_II", "NAV2 Inner +" },
                { "Button_NAV2_ID", "NAV2 Inner -" },
                { "Button_NAV2_TOG", "NAV2 Toggle" },
                { "Button_FMS1_OI", "FMS1 Outer +" },
                { "Button_FMS1_OD", "FMS1 Outer -" },
                { "Button_FMS1_II", "FMS1 Inner +" },
                { "Button_FMS1_ID", "FMS1 Inner -" },
                { "Button_FMS1_TOG", "FMS1 Toggle" },
                { "Button_FMS2_OI", "FMS2 Outer +" },
                { "Button_FMS2_OD", "FMS2 Outer -" },
                { "Button_FMS2_II", "FMS2 Inner +" },
                { "Button_FMS2_ID", "FMS2 Inner -" },
                { "Button_FMS2_TOG", "FMS2 Toggle" },
                { "Button_AP_OI", "AP Outer +" },
                { "Button_AP_OD", "AP Outer -" },
                { "Button_AP_II", "AP Inner +" },
                { "Button_AP_ID", "AP Inner -" },
                { "Button_AP_TOG", "AP Toggle" },
                { "Button_XPDR_OI", "XPDR Outer +" },
                { "Button_XPDR_OD", "XPDR Outer -" },
                { "Button_XPDR_II", "XPDR Inner +" },
                { "Button_XPDR_ID", "XPDR Inner -" },
                { "Button_XPDR_TOG", "XPDR Toggle" },
                { "Button_FMS1_CRSR", "FMS1 CRSR" },
                { "Button_FMS1_DCT", "FMS1 DCT" },
                { "Button_FMS1_MENU", "FMS1 MENU" },
                { "Button_FMS1_CLR", "FMS1 CLR" },
                { "Button_FMS1_ENT", "FMS1 ENT" },
                { "Button_FMS1_CDI", "FMS1 CDI" },
                { "Button_FMS1_OBS", "FMS1 OBS" },
                { "Button_FMS1_MSG", "FMS1 MSG" },
                { "Button_FMS1_FPL", "FMS1 FPL" },
                { "Button_FMS1_VNAV", "FMS1 VNAV" },
                { "Button_FMS1_PROC", "FMS1 PROC" },
                { "Button_FMS2_CRSR", "FMS2 CRSR" },
                { "Button_FMS2_DCT", "FMS2 DCT" },
                { "Button_FMS2_MENU", "FMS2 MENU" },
                { "Button_FMS2_CLR", "FMS2 CLR" },
                { "Button_FMS2_ENT", "FMS2 ENT" },
                { "Button_FMS2_CDI", "FMS2 CDI" },
                { "Button_FMS2_OBS", "FMS2 OBS" },
                { "Button_FMS2_MSG", "FMS2 MSG" },
                { "Button_FMS2_FPL", "FMS2 FPL" },
                { "Button_FMS2_VNAV", "FMS2 VNAV" },
                { "Button_FMS2_PROC", "FMS2 PROC" },
                { "Button_AP_AP", "Autopilot Toggle" },
                { "Button_AP_HDG", "Autopilot HDG" },
                { "Button_AP_NAV", "Autopilot NAV" },
                { "Button_AP_APR", "Autopilot APR" },
                { "Button_AP_VS", "Autopilot VS" },
                { "Button_AP_BC", "Autopilot BC" },
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
            inputReceiver.Start(Stream);
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
            byte[] streambuffer = null;
            byte[] inputReportBuffer = new byte[8];
            Report report;

            if (Stream == null)
            {
                Connect();
            };

            int n = 0;
            while(inputReceiver.TryRead(inputReportBuffer, 0, out report))
            {
                n++;
                OctaviReport OReport = new OctaviReport();
                OReport.parseReport(inputReportBuffer);
                List<int> btnz = octaviHandler.toButton(OReport);
                foreach(int i in btnz) TriggerButtonPress(i);
            }
            if(n>1)
            {

            }
            if (streambuffer != null)
            {
                sbyte outerknob = (sbyte) streambuffer[5];
                sbyte innerknob = (sbyte) streambuffer[6];

                if (innerknob < 0)
                {
                    TriggerButtonPress(0);
                }
                else if (innerknob > 0)
                {
                    TriggerButtonPress(1);
                }
            }

            
        }

        protected override void EnumerateDevices()
        {
            foreach (string entry in octaviHandler.OctaviButtonList)
            {
                    Buttons.Add(new JoystickDevice() { Name = entry, Label = entry, Type = JoystickDeviceType.Button });
            }
        }
    }
}
