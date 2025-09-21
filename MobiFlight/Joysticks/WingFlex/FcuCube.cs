using Device.Net;
using Hid.Net;
using Hid.Net.Windows;
using MobiFlight.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MobiFlight.Joysticks.WingFlex
{
    internal class FcuCube : Joystick
    {
        bool DoReadHidReports = false;
        private Thread readThread;
        IHidDevice Device { get; set; }
        
        private readonly FcuCubeReport FcuCubeReport = new FcuCubeReport();
        public override string Name
        {
            get { return Definition?.InstanceName ?? "FcuCube"; }
        }

        public override string Serial
        {
            get { return $"{Joystick.SerialPrefix}{Device?.CreateSerialFromPath()}" ?? "FCU-CUBE-1234-ABCD-12345678"; }
        }

        public FcuCube(JoystickDefinition definition) : base(null, definition)
        {
        }

        protected async Task<bool> Connect()
        {
            var VendorId = Definition.VendorId;
            var ProductId = Definition.ProductId;

            var hidFactory = new FilterDeviceDefinition(vendorId: (uint)VendorId, productId: (uint)ProductId).CreateWindowsHidDeviceFactory();
            var deviceDefinitions = (await hidFactory.GetConnectedDeviceDefinitionsAsync().ConfigureAwait(false)).ToList();

            if (deviceDefinitions.Count == 0)
            {
                Log.Instance.log($"no FcuCube found with VID:{VendorId.ToString("X4")} and PID:{ProductId.ToString("X4")}", LogSeverity.Info);
                return false;
            }

            Device = (IHidDevice)await hidFactory.GetDeviceAsync(deviceDefinitions.First()).ConfigureAwait(false);
            await Device.InitializeAsync().ConfigureAwait(false);

            DoReadHidReports = true;

            readThread = new Thread(ReadHidReportsLoop)
            {
                IsBackground = true,
                Name = "FcuCube-HID-Reader"
            };
            readThread.Start();

            return true;
        }

        private void ReadHidReportsLoop()
        {
            while (DoReadHidReports)
            {
                try
                {
                    var HidReport = Device.ReadReportAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    var data = HidReport.TransferResult.Data;
                    ProcessInputReportBuffer(HidReport.ReportId, data);
                }
                catch
                {
                    // Exception when disconnecting fcu while mobiflight is running.
                    Shutdown();
                    break;
                }
            }
        }

        public override void Update()
        {
            if (Device == null || !Device.IsInitialized)
            {
                var connected = Connect().GetAwaiter().GetResult();
                if (!connected) return;
            }
        }

        protected void ProcessInputReportBuffer(byte reportId, byte[] inputReportBuffer) {
            var newState = FcuCubeReport.Parse(inputReportBuffer).ToJoystickState();
            UpdateButtons(newState);
            UpdateAxis(newState);
            // Finally store the new state as last state
            State = newState;
        }

        protected async override void SendData(byte[] data)
        {
            await Device.WriteReportAsync(data, 0).ConfigureAwait(false);
            RequiresOutputUpdate = false;
        }

        public override void UpdateOutputDeviceStates()
        {
            // if (!RequiresOutputUpdate) return;
            var data = FcuCubeReport?.FromOutputDeviceState(Lights);

            if (data == null) return;

            try
            {
                SendData(data);
            }
            catch (System.IO.IOException)
            {
                // this happens when the device is removed.
                OnDeviceRemoved();
            }
        }

        protected override void EnumerateDevices()
        {
            Definition.Inputs.ForEach(d => { 
                var device = new JoystickDevice() { Name = d.Name, Label = d.Label, JoystickDeviceType = d.Type };
                switch (d.Type)
                {
                    case JoystickDeviceType.Axis:
                        device.Type = DeviceType.AnalogInput;
                        Axes.Add(device);
                        break;
                    case JoystickDeviceType.Button:
                        device.Type = DeviceType.Button;
                        Buttons.Add(device);
                        break;
                    case JoystickDeviceType.POV:
                        device.Type = DeviceType.Button;
                        POV.Add(device);
                        break;
                }
            });
        }

        protected override void EnumerateOutputDevices()
        {
            base.EnumerateOutputDevices();

            // LcdDisplays
            Definition?.Outputs?.FindAll(d => d.Type==DeviceType.LcdDisplay.ToString()).ForEach(device =>
            {
                Lights.Add(new JoystickOutputDisplay() { Name = device.Id, Label = device.Label, Type = DeviceType.LcdDisplay, Cols = device.Cols, Lines = device.Lines, Byte = device.Byte });
            });
        }

        public override IEnumerable<DeviceType> GetConnectedOutputDeviceTypes()
        {
            List<DeviceType> result = new List<DeviceType>();

            Definition?.Outputs?.ForEach(d => {
                if (d.Type == null && !result.Contains(DeviceType.Output))
                {
                    result.Add(DeviceType.Output);
                    return;
                }

                if (Enum.TryParse<DeviceType>(d.Type, out var deviceType) && !result.Contains(deviceType))
                {
                    result.Add(deviceType);
                }
            });

            return result;
        }

        public override void Shutdown()
        {
            Device?.Dispose();
        }
    }
}
