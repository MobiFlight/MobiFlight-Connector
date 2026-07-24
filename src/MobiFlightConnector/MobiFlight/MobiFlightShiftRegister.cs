using CommandMessenger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace MobiFlight
{
    public class MobiFlightShiftRegister : IConnectedDevice
    {
        private readonly object _lock = new object();
        public const int DEFAULT_AGGREGATION_WINDOW_IN_MS = 50;
        public const string TYPE = "ShiftRegister";
        public const string LABEL_PREFIX = "Output";

        public CmdMessenger CmdMessenger { get; set; }

        public int NumberOfShifters { get; set; }

        public int ModuleNumber { get; set; }

        private String _name = "ShiftRegister";

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected bool _initialized = false;

        public double AggregationWindowInMs
        {
            get { return AggregationTimer.Interval; }
            set
            {
                AggregationTimer.Interval = value;
            }
        }

        public Timer AggregationTimer { get; set; } = new Timer();

        Dictionary<string, string> AggregationSet = new Dictionary<string, string>();

        public MobiFlightShiftRegister()
        {
            AggregationTimer.AutoReset = false;
            AggregationTimer.Interval = DEFAULT_AGGREGATION_WINDOW_IN_MS;
            AggregationTimer.Elapsed += ProcessAggregatedPins;
        }

        protected void ProcessAggregatedPins(object sender, ElapsedEventArgs e)
        {
            List<string> onKeys;
            List<string> offKeys;

            lock (_lock)
            {
                if (AggregationSet.Count == 0) return;

                onKeys = AggregationSet.Keys.Where(k => AggregationSet[k] == "1").ToList();
                offKeys = AggregationSet.Keys.Where(k => AggregationSet[k] == "0").ToList();
                AggregationSet.Clear();
            }

            if (onKeys.Count > 0)
            {
                var onPins = string.Join("|", onKeys);
                InternalDisplay(onPins, "1");
            }

            if (offKeys.Count > 0)
            {
                var offPins = string.Join("|", offKeys);
                InternalDisplay(offPins, "0");
            }
        }

        private DeviceType _type = DeviceType.ShiftRegister;

        public DeviceType TypeDeprecated
        {
            get { return _type; }
            set { _type = value; }
        }

        protected void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
        }

        public void Display(String outputPins, String value)
        {
            var v = value == "0" ? "0" : "1";
            var pins = outputPins.Replace(LABEL_PREFIX + " ", "")
                                 .Split('|')
                                 .Select(p => p.Trim())
                                 .ToList();
            lock (_lock)
            {
                pins.ForEach(p =>
                {
                    AggregationSet[p] = v;
                });

                if (AggregationTimer.Enabled) return;
                AggregationTimer.Start();
            }
        }

        public virtual void InternalDisplay(String outputPins, String value)
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetShiftRegisterPins);

            command.AddArgument(this.ModuleNumber);
            command.AddArgument(outputPins);
            command.AddArgument(value);

            Log.Instance.log($"Command: SetShiftRegisterPin <{(int)MobiFlightModule.Command.SetShiftRegisterPins},{this.ModuleNumber},{outputPins},{value};>.", LogSeverity.Debug);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public void Stop()
        {
            if (NumberOfShifters == 0) return;

            List<String> pins = new List<string>();
            for (int i = 0; i != NumberOfShifters * 8; i++)
                pins.Add(i.ToString());

            String pinString = string.Join("|", pins);
            Display(pinString, "0");
        }
    }
}