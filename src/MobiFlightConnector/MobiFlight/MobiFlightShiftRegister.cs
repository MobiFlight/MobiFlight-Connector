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

        private double _aggregationWindowInMs = 20;
        public double AggregationWindowInMs
        {
            get { return _aggregationWindowInMs; }
            set
            {
                _aggregationWindowInMs = value;
                AggregationTimer.Interval = value;
            }
        }
        public Timer AggregationTimer { get; set; } = new Timer();

        ConcurrentStack<string> AggregationOn = new ConcurrentStack<string>();
        ConcurrentStack<string> AggregationOff = new ConcurrentStack<string>();

        public MobiFlightShiftRegister()
        {
            AggregationTimer.AutoReset = false;
            AggregationTimer.Elapsed += ProcessAggregatedPins;
        }

        private void ProcessAggregatedPins(object sender, ElapsedEventArgs e)
        {
            if (AggregationOn.Count > 0)
            {
                var onPins = AggregateFinalDisplayProps(AggregationOn);
                InternalDisplay(onPins, "1");
            }

            if (AggregationOff.Count > 0)
            {
                var offPins = AggregateFinalDisplayProps(AggregationOff);
                InternalDisplay(offPins, "0");
            }
        }

        static public string AggregateFinalDisplayProps(ConcurrentStack<string> aggregation)
        {
            lock (aggregation)
            {
                var outputPins = string.Join("|", aggregation.Distinct());
                aggregation.Clear();
                return outputPins;
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
            var aggregation = v == "1" ? AggregationOn : AggregationOff;

            var pins = outputPins.Replace(LABEL_PREFIX + " ", "")
                                 .Split('|')
                                 .Select(p => p.Trim())
                                 .ToList();

            pins.ForEach(p =>
            {
                aggregation.Push(p);
            });

            if (AggregationTimer.Enabled) return;

            AggregationTimer.Start();
        }

        public virtual void InternalDisplay(String outputPins, String value)
        {
            if (!_initialized) Initialize();

            var command = new SendCommand((int)MobiFlightModule.Command.SetShiftRegisterPins);

            // Let's strip the static label
            String pinsOnly = outputPins.Replace(LABEL_PREFIX + " ", "");

            // clamp and reverse the string
            if (value.Length > 8) value = value.Substring(0, 8);

            command.AddArgument(this.ModuleNumber);
            command.AddArgument(pinsOnly);
            command.AddArgument(value);

            Log.Instance.log($"Command: SetShiftRegisterPin <{(int)MobiFlightModule.Command.SetShiftRegisterPins},{this.ModuleNumber},{pinsOnly},{value};>.", LogSeverity.Debug);
            // Send command
            CmdMessenger.SendCommand(command);
        }

        public void Stop()
        {
            List<String> pins = new List<string>();
            for (int i = 0; i != NumberOfShifters * 8; i++)
                pins.Add(i.ToString());

            String pinString = string.Join("|", pins);
            Display(pinString, "0");
        }
    }
}