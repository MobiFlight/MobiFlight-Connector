using HidSharp;
using MobiFlight.Modifier;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight.Joysticks.Bodnar
{
    internal class BodnarBoard : Joystick
    {
        /// <summary>
        /// The threshold for axis changes to trigger events. 
        /// This helps to avoid noise and small fluctuations from triggering events.
        /// </summary>
        readonly int AxisChangeThreshold = 32;
        readonly int WindowSize = 8;

        /// <summary>
        /// Reads HID reports on a dedicated background thread.
        /// </summary>
        private readonly HidReportReceiver Receiver = new HidReportReceiver();

        /// <summary>
        /// The report implementation.
        /// </summary>
        protected readonly BodnarReport report = new BodnarReport(buttonCount: 32);

        /// <summary>
        /// Provide same instance name but trim it.
        /// Also works if Definition file is not set yet.
        /// </summary>
        public override string Name
        {
            get { return base.Name ?? "BU0836"; }
        }

        /// <summary>
        /// Provides Serial including prefix.
        /// Serial information is provided through DirectInput GUID.
        /// </summary>
        public override string Serial
        {
            get { return $"{Joystick.SerialPrefix}{DIJoystick.Information.InstanceGuid}"; }
        }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="joystick">The DirectInput joystick instance.</param>
        /// <param name="definition">Joystick definition file.</param>
        public BodnarBoard(int buttonCount, SharpDX.DirectInput.Joystick joystick, JoystickDefinition definition) : base(joystick, definition)
        {
            report = new BodnarReport(buttonCount);
        }

        List<List<ModifierBase>> axisFilter = new List<List<ModifierBase>>();

        /// <summary>
        /// Cached delegates for reading axis values — avoids reflection on every HID report.
        /// Built once in EnumerateDevices after all axes are registered.
        /// </summary>
        private Func<JoystickState, int>[] _axisGetters;

        /// <summary>
        /// Cached delegates for writing axis values — avoids reflection on every HID report.
        /// Built once in EnumerateDevices after all axes are registered.
        /// </summary>
        private Action<JoystickState, int>[] _axisSetters;

        /// <summary>
        /// This creates a connection to the HID device using the HidSharp library.
        /// </summary>
        /// <returns>True if connection was successful, false otherwise.</returns>
        protected bool Connect()
        {
            var vendorId = DIJoystick.Properties.VendorId;
            var productId = DIJoystick.Properties.ProductId;

            if (Device == null)
            {
                Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: vendorId, productID: productId);
                if (Device == null)
                {
                    Log.Instance.log($"no {Name} found with VID:{vendorId.ToString("X4")} and PID:{productId.ToString("X4")}", LogSeverity.Info);
                    return false;
                }
            }

            if (Stream == null)
            {
                try
                {
                    Stream = Device.Open();
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Failed to open {Name} device: {ex.Message}", LogSeverity.Error);
                    return false;
                }
            }

            if (!Receiver.IsReceiving)
            {
                Receiver.Start(Stream, Device.GetMaxInputReportLength(), OnReportReceived, OnReadError, $"{Name}-HID-Reader");
            }

            return true;
        }

        private void OnReportReceived(byte[] rawReport)
        {
            // The report parser expects the payload without the leading report ID byte.
            ProcessInputReportBuffer(rawReport[0], HidReportReceiver.GetPayload(rawReport));
        }

        private void OnReadError(Exception exception)
        {
            // Exception when disconnecting while mobiflight is running.
            Log.Instance.log($"{Name} disconnected because of exception: {exception}", LogSeverity.Error);
            Shutdown();
        }

        /// <summary>
        /// Update is called by the base class.
        /// It ensures that the HID device is correctly initialized.
        /// </summary>
        public override void Update()
        {
            if (Stream == null || !Receiver.IsReceiving)
            {
                var connected = Connect();
                if (!connected) return;
            }
        }

        /// <summary>
        /// This processes the input report buffer, triggers button events and stores the state.
        /// </summary>
        /// <param name="reportId">The HID report ID</param>
        /// <param name="inputReportBuffer">The report data buffer</param>
        protected void ProcessInputReportBuffer(byte reportId, byte[] inputReportBuffer)
        {
            var newState = report.Parse(inputReportBuffer).ToJoystickState(Axes);
            UpdateButtons(newState);
            UpdateAxis(newState);
            // Finally store the new state as last state
            State = newState;
        }

        /// <summary>
        /// Cleans up HID device resources and stops the background reading thread.
        /// </summary>
        public override void Shutdown()
        {
            Receiver.Stop();
            Stream?.Close();
            Stream = null;
            Device = null;

            base.Shutdown();
        }

        /// <summary>
        /// We are applying some hysteresis to avoid noise triggering events.
        /// </summary>
        /// <param name="newState">The new joystick state to compare against.</param>
        protected override void UpdateAxis(JoystickState newState)
        {
            for (int CurrentAxis = 0; CurrentAxis != Axes.Count; CurrentAxis++)
            {

                int oldValue = 0;
                if (StateExists())
                {
                    oldValue = _axisGetters[CurrentAxis](State);
                }

                int newValue = _axisGetters[CurrentAxis](newState);

                int filteredValue = newValue;
                foreach (var item in axisFilter[CurrentAxis])
                {
                    filteredValue = (int)Math.Round(item.Apply(new ConnectorValue() { Float64 = filteredValue }, null).Float64);
                }

                _axisSetters[CurrentAxis](newState, filteredValue);

                if (oldValue == filteredValue)
                {
                    continue;
                }

                TriggerButtonPressed(this, new InputEventArgs()
                {
                    Controller = new Base.Controller() { Name = this.Name, Serial = this.Serial },
                    Device = new Base.DeviceReference() { Type = Axes[CurrentAxis].Type, Name = Axes[CurrentAxis].Name, Label = Axes[CurrentAxis].Label },
                    InputType = DeviceType.AnalogInput,
                    Value = filteredValue
                });
            }
        }

        protected override void EnumerateDevices()
        {
            // In unit test context
            // This can be possibly null -> so we skip this part.
            if (DIJoystick != null)
            {
                foreach (DeviceObjectInstance device in this.DIJoystick.GetObjects().ToList().OrderBy((a) => a.Usage))
                {
                    this.DIJoystick.GetObjectInfoById(device.ObjectId);

                    bool IsAxis = (device.ObjectId.Flags & DeviceObjectTypeFlags.AbsoluteAxis) > 0;
                    bool IsButton = (device.ObjectId.Flags & DeviceObjectTypeFlags.Button) > 0;
                    bool IsPOV = (device.ObjectId.Flags & DeviceObjectTypeFlags.PointOfViewController) > 0;

                    if (IsAxis && Axes.Count < DIJoystick.Capabilities.AxeCount)
                    {
                        RegisterAxis(device);
                    }
                    else if (IsButton)
                    {
                        RegisterButton(device);
                    }
                    else if (IsPOV)
                    {
                        RegisterPOV(device);
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            // Build axis cache after all axes are registered
            BuildAxisAccessMethods();

            axisFilter.Clear();
            axisFilter.AddRange(
                Axes.Select(a => new List<ModifierBase>() {
                    new Quantize() { StepSize = AxisChangeThreshold, Active = true },
                    new SimpleMovingAverage() { WindowSize = WindowSize, Active = true }
                })
            );
        }

        /// <summary>
        /// Builds cached getter and setter delegates for each axis to avoid
        /// costly reflection lookups on every HID report.
        /// </summary>
        protected void BuildAxisAccessMethods()
        {
            _axisGetters = new Func<JoystickState, int>[Axes.Count];
            _axisSetters = new Action<JoystickState, int>[Axes.Count];

            for (int i = 0; i < Axes.Count; i++)
            {
                var rawName = Axes[i].Name.Replace(AxisPrefix, "").TrimStart();
                if (rawName.Contains("Slider"))
                {
                    int sliderIndex = rawName == "Slider2" ? 1 : 0;
                    _axisGetters[i] = s => s.Sliders[sliderIndex];
                    _axisSetters[i] = (s, v) => s.Sliders[sliderIndex] = v;
                }
                else
                {
                    var prop = typeof(JoystickState).GetProperty(rawName);
                    _axisGetters[i] = s => (int)prop.GetValue(s, null);
                    _axisSetters[i] = (s, v) => prop.SetValue(s, v, null);
                }
            }
        }
    }
}