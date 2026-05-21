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
        private readonly OctaviHandler octaviHandler;

        public Octavi(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
            octaviHandler = new OctaviHandler(this.Definition);
        }

        /// <summary>
        /// Method is not called by regular Joystick Manager
        /// This method is called implicitly when Update() is called
        /// </summary>
        public void Connect()
        {
            // Prevent reentry and parallel execution by multiple threads
            lock (this)
            {
                if (Device == null)
                {
                    Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
                    if (Device == null) return;
                }

                if (Stream == null)
                {
                    Stream = Device.Open();
                    Stream.ReadTimeout = System.Threading.Timeout.Infinite;
                    reportDescriptor = Device.GetReportDescriptor();
                }

                if (inputReceiver == null)
                {
                    inputReceiver = reportDescriptor.CreateHidDeviceInputReceiver();
                    inputReceiver.Received += InputReceiver_Received;
                    inputReceiver.Start(Stream);
                }
            }
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

                this.Update();
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
                Controller = new Base.Controller()
                {
                    Name = this.Name,
                    Serial = this.Serial
                },
                Device = new Base.DeviceReference()
                {
                    Name = Buttons[i].Name,
                    Label = Buttons[i].Label,
                    Type = Buttons[i].Type
                },
                InputType = DeviceType.Button,
                Value = (int)inputEvent
            });
        }

        private void UpdateShiftModeLed()
        {
            /* this will set nothing if the Ouput device label is not found - disable auto blinking by renaming the output in joystick.json */
            this.SetOutputDeviceState("Auto Blink Context", (byte)(octaviHandler.IsInShiftMode ? 1 : 0));
        }

        public override void Update()
        {
            // Octavi is not a DirectInput device
            // so we have to connect it here.
            if (Stream == null || inputReceiver == null)
            {
                Connect();
            };

            // We don't do anything else
            // because we have a callback for
            // handling the incoming reports
            // only check for updated shift state and update the outputs if needed.
            this.UpdateShiftModeLed();
            this.UpdateOutputDeviceStates();
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