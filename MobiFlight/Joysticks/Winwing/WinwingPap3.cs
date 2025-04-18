﻿using Device.Net;
using Hid.Net;
using Hid.Net.Windows;
using MobiFlight.Config;
using MobiFlightWwFcu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingPap3 : Joystick
    {
        private readonly int VendorId = 0x4098;
        private int ProductId = 0xBF0F;
        IHidDevice Device { get; set; }

        private const int SPD_DEC = 20;
        private const int SPD_INC = 21;
        private const int HDG_DEC = 22;
        private const int HDG_INC = 23;
        private const int ALT_DEC = 24;
        private const int ALT_INC = 25;
        private const int VS_DEC = 39;
        private const int VS_INC = 40;
        private const int CRSL_DEC = 18;
        private const int CRSL_INC = 19;
        private const int CRSR_DEC = 26;
        private const int CRSR_INC = 27;
        private const uint BUTTONS_REPORT = 1;

        private Dictionary<int, JoystickDevice>  ButtonsToTrigger = new Dictionary<int, JoystickDevice>();
        private Dictionary<int, JoystickDevice> EncoderButtonsToTrigger = new Dictionary<int, JoystickDevice>();
        private List<int> EncoderIncDecButtons 
            = new List<int> { SPD_DEC, SPD_INC, HDG_DEC, HDG_INC, ALT_DEC, ALT_INC, VS_DEC, VS_INC, CRSL_DEC, CRSL_INC, CRSR_DEC, CRSR_INC }; 
  
        private JoystickDefinition Definition;
        private volatile bool DoInitialize = true;
        private volatile bool DoReadHidReports = false;
        private WinwingPap3Report CurrentReport = new WinwingPap3Report();
        private WinwingPap3Report PreviousReport = new WinwingPap3Report();
        private HidBuffer HidDataBuffer = new HidBuffer();
        private WinwingDisplayControl DisplayControl;

        private List<IBaseDevice> LcdDevices = new List<IBaseDevice>();        
        private List<ListItem<IBaseDevice>> LedDevices = new List<ListItem<IBaseDevice>>();

        public WinwingPap3(SharpDX.DirectInput.Joystick joystick, JoystickDefinition def, int productId, WebSocketServer server) : base(joystick, def)
        {
            Definition = def;
            ProductId = productId;
            DisplayControl = new WinwingDisplayControl(productId, server);
            var displayNames = DisplayControl.GetDisplayNames();
            var ledNames = DisplayControl.GetLedNames();

            DisplayControl.ErrorMessageCreated += DisplayControl_ErrorMessageCreated;

            // Initialize LCD and LED device lists and current value cache
            foreach (string displayName in displayNames) 
            {
                LcdDevices.Add(new LcdDisplay() { Name = displayName }); // Col and Lines values don't matter   
            }
            foreach (string ledName in ledNames)
            {
                LedDevices.Add(new JoystickOutputDevice() { Label = ledName, Name = ledName }.ToListItem()); // Byte and Bit values don't matter           
            }
        }

        private void DisplayControl_ErrorMessageCreated(object sender, string e)
        {
            if (!string.IsNullOrEmpty(e))
            {
                Log.Instance.log(e, LogSeverity.Error);
            }
        }


        public async override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            DisplayControl.Connect();

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
                Log.Instance.log($"Added WINWING PAP3 Id: {input.Id} Axis: {input.Name} Label: {input.Label}.", LogSeverity.Debug);
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
                    PreviousReport.ButtonState = ~PreviousReport.ButtonState; // to retrigger
                    PreviousReport.ButtonState2 = ~PreviousReport.ButtonState2; // to retrigger
                    PreviousReport.ButtonState3 = ~PreviousReport.ButtonState3; // to retrigger
                    DoInitialize = false;
                }

                // Detect and Trigger Button Events
                uint pressed = CurrentReport.ButtonState & ~PreviousReport.ButtonState; // rising edges
                uint released = PreviousReport.ButtonState & ~CurrentReport.ButtonState; // falling edges
                uint pressed2 = CurrentReport.ButtonState2 & ~PreviousReport.ButtonState2; // rising edges
                uint released2 = PreviousReport.ButtonState2 & ~CurrentReport.ButtonState2; // falling edges
                uint pressed3 = CurrentReport.ButtonState3 & ~PreviousReport.ButtonState3; // rising edges
                uint released3 = PreviousReport.ButtonState3 & ~CurrentReport.ButtonState3; // falling edges
                CheckForButtonTrigger(pressed, MobiFlightButton.InputEvent.PRESS, 0);
                CheckForButtonTrigger(released, MobiFlightButton.InputEvent.RELEASE, 0);
                CheckForButtonTrigger(pressed2, MobiFlightButton.InputEvent.PRESS, 32);
                CheckForButtonTrigger(released2, MobiFlightButton.InputEvent.RELEASE, 32);
                CheckForButtonTrigger(pressed3, MobiFlightButton.InputEvent.PRESS, 64);
                CheckForButtonTrigger(released3, MobiFlightButton.InputEvent.RELEASE, 64);

                // Detect and Trigger Encoder Turns
                int spdIncrement = CurrentReport.SpdEncoderValue - PreviousReport.SpdEncoderValue;
                CheckForEncoderTrigger(spdIncrement, SPD_DEC, SPD_INC);
                int hdgIncrement = CurrentReport.HdgEncoderValue - PreviousReport.HdgEncoderValue;
                CheckForEncoderTrigger(hdgIncrement, HDG_DEC, HDG_INC);
                int altIncrement = CurrentReport.AltEncoderValue - PreviousReport.AltEncoderValue;
                CheckForEncoderTrigger(altIncrement, ALT_DEC, ALT_INC);
                int vsIncrement = CurrentReport.VsEncoderValue - PreviousReport.VsEncoderValue;
                CheckForEncoderTrigger(vsIncrement, VS_DEC, VS_INC);
                int courseLeftIncrement = CurrentReport.CourseLeftEncoderValue - PreviousReport.CourseLeftEncoderValue;
                CheckForEncoderTrigger(courseLeftIncrement, CRSL_DEC, CRSL_INC);
                int courseRightIncrement = CurrentReport.CourseRightEncoderValue - PreviousReport.CourseRightEncoderValue;
                CheckForEncoderTrigger(courseRightIncrement, CRSR_DEC, CRSR_INC);
                CurrentReport.CopyTo(PreviousReport);
            }
        }

        protected void TriggerButtonPress(JoystickDevice device, MobiFlightButton.InputEvent inputEvent)
        {
            TriggerButtonPressed(this, new InputEventArgs()
            {
                Name = Name,
                DeviceId = device.Name,
                DeviceLabel = device.Label,
                Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString(),
                Type = DeviceType.Button,
                Value = (int)inputEvent
            });
        }

        public override void Retrigger()
        {
            DoInitialize = true;
        }

        public override IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {     
            return new List<DeviceType>() { DeviceType.Output, DeviceType.LcdDisplay };
        }

        public override void SetLcdDisplay(string address, string value)
        {
            // Check for value change is done inside the library
            DisplayControl.SetDisplay(address, value);
        }

        public override void SetOutputDeviceState(string name, byte state)
        {
            // Check for value change is done inside the library
            DisplayControl.SetLed(name, state);      
        }

        public override List<IBaseDevice> GetAvailableLcdDevices()
        {
            return LcdDevices;
        }

        public override List<ListItem<IBaseDevice>> GetAvailableOutputDevicesAsListItems()
        {
            return LedDevices;
        }

        public override void Update()
        {
            // do nothing, update is event based not polled
        }

        public override void UpdateOutputDeviceStates()
        {
            // do nothing, update is event based not polled
        }

        protected override void SendData(byte[] data)
        {
            // do nothing, data is directly send in SetOutputDeviceState
        }

        public override void Shutdown()
        {
            DoReadHidReports = false;
            DisplayControl.Shutdown();              
            if (Device != null) 
            {
                Device.Close();
                Device = null;
            }
        }
    }
}
