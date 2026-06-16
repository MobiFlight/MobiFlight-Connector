using Device.Net;
using Hid.Net;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingRmp : WinwingBaseController
    {
        new IHidDevice Device { get; set; }

        private const int OUTER_KNOB_DEC = 15;
        private const int OUTER_KNOB_INC = 16;
        private const int INNER_KNOB_DEC = 17;
        private const int INNER_KNOB_INC = 18;
        private const uint BUTTONS_REPORT = 1;

        private Dictionary<int, JoystickDevice>  ButtonsToTrigger = new Dictionary<int, JoystickDevice>();
        private Dictionary<int, JoystickDevice> EncoderButtonsToTrigger = new Dictionary<int, JoystickDevice>();
        private List<int> EncoderIncDecButtons 
            = new List<int> { OUTER_KNOB_DEC, OUTER_KNOB_INC, INNER_KNOB_DEC, INNER_KNOB_INC }; 
  
        private volatile bool DoInitialize = true;
        private volatile bool DoRetrigger = false;
        private volatile bool DoReadHidReports = false;
        private WinwingRmpReport CurrentReport = new WinwingRmpReport();
        private WinwingRmpReport PreviousReport = new WinwingRmpReport();
        private HidBuffer HidDataBuffer = new HidBuffer();

        public WinwingRmp(SharpDX.DirectInput.Joystick joystick, JoystickDefinition def, int productId, WebSocketServer server) : base(joystick, def, productId, server)
        {
            // ctor logic is in base class
        }

        public async override void Connect(IntPtr handle)
        {
            base.Connect(handle);

            var hidFactory = new FilterDeviceDefinition(vendorId: (uint)VendorId, productId: (uint)ProductId).CreateWindowsHidDeviceFactory();
            var deviceDefinitions = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();
            Device = (IHidDevice)await hidFactory.GetDeviceAsync(deviceDefinitions.First()).ConfigureAwait(false);
            await Device.InitializeAsync().ConfigureAwait(false);
            DoReadHidReports = true;
            DisplayControl.SendRequestFirmware();

            await Task.Run(async () =>
            {
                while (DoReadHidReports)
                {
                    try
                    {
                        HidDataBuffer.HidReport = await Device.ReadReportAsync().ConfigureAwait(false);
                        InputReportReceived(HidDataBuffer);
                    }
                    catch 
                    {
                        // Exception when disconnecting fcu while mobiflight is running.
                        Shutdown();
                    }
                }
            });
        }

        // EnumerateInputDevices
        protected override void EnumerateDevices()
        {
            // Take the list from the config file. Do not show the internal encoder axis.
            foreach (var input in Definition.Inputs)
            {
                var device = new JoystickDevice() { Name = input.Name, Label = input.Label, Type = DeviceType.Button, JoystickDeviceType = JoystickDeviceType.Button };
                Buttons.Add(device);
           
                if (!EncoderIncDecButtons.Contains(input.Id))
                {
                    ButtonsToTrigger[input.Id] = device;
                }
                else
                {
                    EncoderButtonsToTrigger[input.Id] = device;
                }
                Log.Instance.log($"Added WINWING RMP Id: {input.Id} Axis: {input.Name} Label: {input.Label}.", LogSeverity.Debug);
            }
        }

        private void CheckForButtonTrigger(uint changes, MobiFlightButton.InputEvent inputEvent, int offset)
        {
            if (changes > 0)
            {
                for (int i = 0; i < 32; i++)
                {
                    if ((changes & (1 << i)) != 0) // IsBitSet
                    {
                        // Button IDs start with 1
                        bool hasValue = ButtonsToTrigger.TryGetValue((i + offset + 1), out var button);
                        if (hasValue)
                        {
                            TriggerButtonPress(button, inputEvent);
                        }
                    }
                }
            }
        }

        private void ExecuteEncoderTrigger(int increment, int id)
        {           
            for (int i = 0; i < Math.Abs(increment); i++)
            {
                // For encoder buttons only send press event 
                TriggerButtonPress(EncoderButtonsToTrigger[id], MobiFlightButton.InputEvent.PRESS);
            }
        }

        private void CheckForEncoderTrigger(int increment, int idDec, int idInc)
        {                       
            if (increment != 0)
            {
                // Adjust for overflow
                if (increment > 1000)
                {
                    increment = increment - (ushort.MaxValue + 1);
                }
                else if (increment < -1000)
                {
                    increment = increment + (ushort.MaxValue + 1);
                }
                if (increment > 0)
                {
                    ExecuteEncoderTrigger(increment, idInc);
                }
                else if (increment < 0)
                {
                    ExecuteEncoderTrigger(increment, idDec);
                }
            }
        }

        private void InputReportReceived(HidBuffer hidBuffer)
        {
            CurrentReport.ParseReport(hidBuffer);
            if (CurrentReport.ReportId == BUTTONS_REPORT)
            {
                if (DoInitialize)
                {
                    CurrentReport.CopyTo(PreviousReport);
                    DoInitialize = false;
                }

                if (DoRetrigger)
                {
                    PreviousReport.ButtonState = ~CurrentReport.ButtonState; // to retrigger
                    DoRetrigger = false;
                }

                // Detect and Trigger Button Events
                uint pressed = CurrentReport.ButtonState & ~PreviousReport.ButtonState; // rising edges
                uint released = PreviousReport.ButtonState & ~CurrentReport.ButtonState; // falling edges
                CheckForButtonTrigger(pressed, MobiFlightButton.InputEvent.PRESS, 0);
                CheckForButtonTrigger(released, MobiFlightButton.InputEvent.RELEASE, 0);

                // Detect and Trigger Encoder Turns
                int innerKnobIncrement = CurrentReport.InnerKnobValue - PreviousReport.InnerKnobValue;
                CheckForEncoderTrigger(innerKnobIncrement, INNER_KNOB_DEC, INNER_KNOB_INC);
                int outerKnobIncrement = CurrentReport.OuterKnobValue - PreviousReport.OuterKnobValue;
                CheckForEncoderTrigger(outerKnobIncrement, OUTER_KNOB_DEC, OUTER_KNOB_INC);

                CurrentReport.CopyTo(PreviousReport);
            }
        }

        protected void TriggerButtonPress(JoystickDevice device, MobiFlightButton.InputEvent inputEvent)
        {
            TriggerButtonPressed(this, new InputEventArgs()
            {
                Controller = new Base.Controller()
                {
                    Name = Name,
                    Serial = Serial,
                },
                Device = new Base.DeviceReference()
                {
                    Name = device.Name,
                    Label = device.Label,
                    Type = DeviceType.Button
                },
                InputType = DeviceType.Button,
                Value = (int)inputEvent
            });
        }

        public override void Retrigger()
        {
            DoRetrigger = true;
        }

        public override void Update()
        {
            // do nothing, update is event based not polled
        }

        public override void Shutdown()
        {
            DoReadHidReports = false;
            base.Shutdown();
            if (Device != null) 
            {
                Device.Close();
                Device = null;
            }
        }
    }
}