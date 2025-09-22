using HidSharp;
using HidSharp.Reports;
using HidSharp.Reports.Input;
using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.VKB
{
    internal class VKBDevice : Joystick
    {
        private HidStream Stream;
        private readonly HidDevice Device;
        private readonly new VKBLedContainer Lights = new VKBLedContainer();
        private HidDeviceInputReceiver InputReceiver;
        private readonly byte[] InputReportBuffer = new byte[64];
        private readonly SortedList<byte, VKBEncoder> Encoders = new SortedList<byte, VKBEncoder>();
        int lastSeqNo = -1;

        public VKBDevice(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
            if (Device == null)
            {
                Device = GetMatchingHidDevice(joystick);
            }
        }

        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            if (Device == null)
            {
                return;
            }
            if (Stream == null)
            {
                Stream = Device.Open();
                Stream.ReadTimeout = System.Threading.Timeout.Infinite;
            }

            if (InputReceiver == null)
            {
                // We use our own descriptor since the one we get from Windows/HIDSharp has been altered and encoder data is missing
                InputReceiver = new ReportDescriptor(VKBHidReport.Descriptor).CreateHidDeviceInputReceiver();
                InputReceiver.Received += OnHidReportReceived;
                InputReceiver.Start(Stream);
            }
        }

        protected override void SendData(byte[] data)
        {
            // VKBLedContainer has its own handling that replaces RequiresOutputUpdate, so we just send.
            // Don't try and send data if no outputs are defined.
            if (Definition?.Outputs == null || Definition?.Outputs.Count == 0)
            {
                return;
            }
            Stream?.SetFeature(data);

        }

        protected override void EnumerateDevices()
        {
            base.EnumerateDevices();
            var EncoderDecList = new SortedList<byte, JoystickDevice>();
            var EncoderIncList = new SortedList<byte, JoystickDevice>();
            if (Definition == null || Definition.Inputs == null)
            {
                return;
            }

            foreach (var input in Definition.Inputs)
            {
                // The 1xxx range is limited to encoders. They are also not fed from DirectInput.
                // Format for encoder virtual buttons is 1IID, where II is a two-digit ID and D is the direction.
                if (input.Id >= 1000 && input.Id < 2000 && input.Type == JoystickDeviceType.Button)
                {
                    byte encoderIndex = GetEncoderIndex(input);
                    VKBEncoder.EncoderAction encoderAction = GetEncoderAction(input);
                    // Store the encoders temporarily, for a valid encoder definition we need both directions.
                    // A correct definition should always have both, but a user may have edited it for a customized controller.
                    if (encoderAction == VKBEncoder.EncoderAction.DEC)
                    {
                        EncoderDecList.Add(encoderIndex, new JoystickDevice { Name = $"Button {1000 + 10 * encoderIndex + (int)encoderAction}", Label = input.Label, Type = DeviceType.Button, JoystickDeviceType = input.Type });
                    }
                    if (encoderAction == VKBEncoder.EncoderAction.INC)
                    {
                        EncoderIncList.Add(encoderIndex, new JoystickDevice { Name = $"Button {1000 + 10 * encoderIndex + (int)encoderAction}", Label = input.Label, Type = DeviceType.Button, JoystickDeviceType = input.Type });
                    }
                    // Rename the original buttons to keep indexing intact and ensure compatibility with devices not configured to use encoder channels.
                    Buttons.FindAll(but => but.Label == input.Label).ForEach(but => but.Label += " (Legacy DirectInput)"); 
                }
            }
            foreach (var encdec in EncoderDecList)
            {
                if (EncoderIncList.ContainsKey(encdec.Key))
                {
                    Encoders.Add(encdec.Key, new VKBEncoder(EncoderIncList[encdec.Key], encdec.Value)); // If both increment and decrement were in the definition, we create an encoder object from the definition file
                    Buttons.Add(Encoders[encdec.Key].DeviceDec); // And register their virtual buttons in the joystick's button list.
                    Buttons.Add(Encoders[encdec.Key].DeviceInc);
                }
            }
        }

        private static byte GetEncoderIndex(JoystickInput input)
        {
            return (byte)((input.Id - 1000) / 10);
        }

        private static VKBEncoder.EncoderAction GetEncoderAction(JoystickInput input)
        {
            return (VKBEncoder.EncoderAction)(input.Id % 10);
        }

        protected override void EnumerateOutputDevices()
        {
            Definition?.Outputs?.ForEach(output => Lights.AddChannel(output));
            base.EnumerateOutputDevices();
        }

        public override void SetOutputDeviceState(string name, byte state)
        {
            Lights.UpdateState(name, state);
        }

        public override void UpdateOutputDeviceStates()
        {
            var data = Lights.CreateMessage();
            // Only send message if there are non-zero LEDs to be updated
            if (data[7] == 0)
            {
                return; 
            }

            try
            {
                SendData(data);
            }
            catch (System.IO.IOException)
            {
                base.OnDeviceRemoved();
            }
        }

        private void OnHidReportReceived(object sender, System.EventArgs e)
        {
            var inputReceiver = sender as HidDeviceInputReceiver;
            while (inputReceiver?.TryRead(InputReportBuffer, 0, out _) ?? false)
            {
                byte ReportId = InputReportBuffer[0];
                if (ReportId != 0x08) // 0x08 = Monitoring channel / virtual bus
                {
                    continue;
                }

                byte MessageType = InputReportBuffer[1];
                if (MessageType != 0x13) // 0x13 = Encoder status
                {
                    continue;
                }

                ParseEncoderReport(InputReportBuffer);
            }
        }

        private void ParseEncoderReport(byte[] Report)
        {
            byte sequenceNo = Report[2];
            // Sequence number should increment once per report, but if the encoder is spun fast some reports can be missed.
            // Firmware is capable of transmitting up to 250 updates per second.
            if (((lastSeqNo + 1) & 0xFF) != sequenceNo)
            {
                Log.Instance.log("Some VKB encoder messages may have been missed", LogSeverity.Debug);
                // Not a problem since API reports absolute state, so the updates are just received on the next received message.
            }
            lastSeqNo = sequenceNo;
            byte encoderCount = Report[3];
            int maxEncoders = (Report.Length - 4) / 2;
            // It is possible to define more encoders than the report can handle. In this case, we limit ourselves to the encoders actually present in the report.
            if (encoderCount > maxEncoders)
            {
                Log.Instance.log($"Log message reports {encoderCount} encoders, but only has space for {maxEncoders}. Some encoders were ignored.", LogSeverity.Warn);
                encoderCount = (byte)maxEncoders;
                // Should not occur in most real-life scenarios
            }
            var events = new List<InputEventArgs>();
            for (byte i = 0; i < encoderCount; i++)
            {
                ushort newPos = (ushort)(Report[5 + 2 * i] << 8 | Report[4 + 2 * i]);
                // Add encoders that were not part of definition when we first receive a message with them.
                if (!Encoders.ContainsKey(i))
                {
                    Encoders.Add(i, new VKBEncoder(i, newPos));
                    Buttons.Add(Encoders[i].DeviceDec);
                    Buttons.Add(Encoders[i].DeviceInc);
                }
                else
                {
                    events.AddRange(Encoders[i].Update(newPos));
                }
            }
            foreach (InputEventArgs e in events)
            {
                // Process the encoder events created by the encoder object.
                e.Name = Name;
                e.Serial = SerialPrefix + DIJoystick.Information.InstanceGuid.ToString();
                TriggerButtonPressed(this, e);
            }
        }

        public static HidDevice GetMatchingHidDevice(SharpDX.DirectInput.Joystick joystick)
        {
            // Get the HID device using the device path. We are not relying on PID alone because multiple devices may have the same PID under certain circumstances, e.g. identical module combos.
            var DevList = DeviceList.Local.GetHidDevices(joystick.Properties.VendorId, joystick.Properties.ProductId);
            foreach (HidDevice dev in DevList)
            {
                if (dev.DevicePath == joystick.Properties.InterfacePath)
                {
                    return dev;
                }
            }

            return null;
        }
    }
}
