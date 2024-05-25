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
        private readonly JoystickDefinition Definition;
        private readonly VKBLedContainer Lights = new VKBLedContainer();
        private HidDeviceInputReceiver InputReceiver;
        private readonly byte[] InputReportBuffer = new byte[64];
        private readonly SortedList<byte, VKBEncoder> Encoders = new SortedList<byte, VKBEncoder>();
        int lastSeqNo = -1;

        public VKBDevice(SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
            Definition = definition;
            if (Device == null)
            {
                Device = GetMatchingHidDevice(joystick);
                if (Device == null) return;
            }
        }
        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            if (Stream == null)
            {
                Stream = Device.Open();
                Stream.ReadTimeout = System.Threading.Timeout.Infinite;
            }

            if (InputReceiver == null)
            {
                InputReceiver = new ReportDescriptor(VKBHidReport.Descriptor).CreateHidDeviceInputReceiver();
                // We use our own descriptor since the one we get from Windows/HIDSharp has been altered and encoder data is missing
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
            SortedList<byte, JoystickDevice> EncoderDecList = new SortedList<byte, JoystickDevice>();
            SortedList<byte, JoystickDevice> EncoderIncList = new SortedList<byte, JoystickDevice>();
            if (Definition == null || Definition.Inputs == null) return;
            foreach (var input in Definition.Inputs)
            {
                // The 1xxx range is limited to encoders. They are also not fed from DirectInput.
                if (input.Id >= 1000 && input.Id < 2000 && input.Type == JoystickDeviceType.Button)
                {
                    byte encoderIndex = GetEncoderIndex(input);
                    VKBEncoder.EncoderAction encoderAction = GetEncoderAction(input);
                    if (encoderAction == VKBEncoder.EncoderAction.DEC)
                    {
                        EncoderDecList.Add(encoderIndex, new JoystickDevice { Name = $"Button {1000 + 10 * encoderIndex + (int)encoderAction}", Label = input.Label, Type = DeviceType.Button, JoystickDeviceType = input.Type });
                    }
                    if (encoderAction == VKBEncoder.EncoderAction.INC)
                    {
                        EncoderIncList.Add(encoderIndex, new JoystickDevice { Name = $"Button {1000 + 10 * encoderIndex + (int)encoderAction}", Label = input.Label, Type = DeviceType.Button, JoystickDeviceType = input.Type });
                    }
                    Buttons.FindAll(but => but.Label == input.Label).ForEach(but => but.Label += " (Legacy DirectInput)"); 
                    // Rename the original buttons to keep indexing intact and ensure compatibility with devices not configured to use encoder channels.
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
            if (data[7] == 0) return; // Only send message if there are non-zero LEDs to be updated
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
            while (inputReceiver.TryRead(InputReportBuffer, 0, out _))
            {
                byte ReportId = InputReportBuffer[0];
                if (ReportId != 0x08) // 0x08 = Monitoring channel / virtual bus
                    continue;
                byte MessageType = InputReportBuffer[1];
                if (MessageType != 0x13) // 0x13 = Encoder status
                    continue;
                ParseEncoderReport(InputReportBuffer);
            }
        }
        private void ParseEncoderReport(byte[] Report)
        {
            byte sequenceNo = Report[2];
            if (((lastSeqNo + 1) & 0xFF) != sequenceNo)
            {
                Log.Instance.log("Some VKB encoder messages may have been missed", LogSeverity.Debug);
                // Can easily happen if many updates are sent in quick succession (like when an encoder is spun fast)
                // Not a problem since API reports absolute state, so the updates are just received on the next received message.
            }
            lastSeqNo = sequenceNo;
            byte encoderCount = Report[3];
            int maxEncoders = (Report.Length - 4) / 2;
            if (encoderCount > maxEncoders)
            {
                Log.Instance.log($"Log message reports {encoderCount} encoders, but only has space for {maxEncoders}. Some encoders were ignored.", LogSeverity.Warn);
                encoderCount = (byte)maxEncoders;
                // Should not occur in most real-life scenarios, but it is theoretically possible to construct a device with more encoders than the report can handle.
            }
            List<InputEventArgs> events = new List<InputEventArgs>();
            for (byte i = 0; i < encoderCount; i++)
            {
                ushort newPos = (ushort)(Report[5 + 2 * i] << 8 | Report[4 + 2 * i]);
                if (!Encoders.ContainsKey(i))
                {
                    // Add encoders that were not part of definition when we first receive a message with them.
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
                if (dev.DevicePath == joystick.Properties.InterfacePath)
                    return dev;
            return null;
        }
    }
}