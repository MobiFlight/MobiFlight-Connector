using System;
using System.Collections.Generic;
using XPlaneConnector;

namespace MobiFlight.xplane
{
    public class XplaneCache : XplaneCacheInterface
    {
        public event EventHandler Closed;
        public event EventHandler Connected;
        public event EventHandler ConnectionLost;
        public event EventHandler OnUpdateFrequencyPerSecondChanged;
        public event EventHandler<string> AircraftChanged;

        private bool _connected = false;
        private int _updateFrequencyPerSecond = 10;
        private string _detectedAircraft = string.Empty;
        public int UpdateFrequencyPerSecond
        {
            get { return _updateFrequencyPerSecond; }
            set
            {
                if (_updateFrequencyPerSecond == value) return;
                _updateFrequencyPerSecond = value;
                OnUpdateFrequencyPerSecondChanged?.Invoke(value, new EventArgs());
            }
        }

        XPlaneConnector.XPlaneConnector Connector = null;

        Dictionary<String, DataRefElement> SubscribedDataRefs = new Dictionary<String, DataRefElement>();

        public XplaneCache()
        {
            Connected += (s, e) =>
            {
                // As soon as we get connected
                // we want to check for the aircraft name,
                // so we can trigger the AircraftChanged event correctly
                CheckForAircraftName();
            };
        }

        public bool Connect()
        {
            if (Connector == null)
            {
                Connector = new XPlaneConnector.XPlaneConnector();

                Connector.OnLog += (m) =>
                {
                    // Log.Instance.log(m, LogSeverity.Debug);
                };

                OnUpdateFrequencyPerSecondChanged += (v, e) =>
                {
                    Log.Instance.log($"update frequency changed: {v} per second.", LogSeverity.Debug);
                    UnsubscribeAll();
                };
            }

            WaitForConnection();
            return _connected;
        }

        /// <summary>
        /// This method will raise a Connected event as soon as we detect that we are receiving values from the sim.
        /// The method subscribes to a DatRef which is present for all aircraft and changes constantly.
        /// </summary>
        private void WaitForConnection()
        {
            var dataRefTime = new DataRefElement() { DataRef = "sim/time/total_running_time_sec", Frequency = 1, Value = 0 };
            
            Connector.Start();
            Connector.Unsubscribe(dataRefTime.DataRef);
            Connector.Subscribe(dataRefTime, 1, (e, v) =>
            {
#if DEBUG
                Log.Instance.log($"sim/time/total_running_time_sec = {v}", LogSeverity.Debug);
#endif
                if (_connected) return;

                _connected = true;
                Connected?.Invoke(this, new EventArgs());
            });
        }

        /// <summary>
        /// Updates the subscription to the aircraft name data reference and notifies listeners when the aircraft
        /// changes.
        /// </summary>
        /// <remarks>This method itself does not reliably detect the name change because of a flaw in the libary used.
        /// We basically resubscribe to the datarefs which will provide us with the correct aircraft name reliably.
        /// This method is called when CheckForAircraftName detects a change in the first or third character of the aircraft name.
        /// </remarks>
        private void UpdateAircraftSubscription()
        {
            StringDataRefElement datarefAircraftName = new StringDataRefElement
            {
                DataRef = "sim/aircraft/view/acf_ui_name",
                Frequency = 1,
                Value = string.Empty,
                StringLenght = 64
            };

            Connector.Unsubscribe(datarefAircraftName.DataRef);
            Connector.Subscribe(datarefAircraftName, 1, (e1, v1) =>
            {
                if (_detectedAircraft == v1) return;
                _detectedAircraft = v1;
                AircraftChanged?.Invoke(this, _detectedAircraft);
            });
        }

        /// <summary>
        /// This method probes two characters of the aircraft name for change and is used in conjunction with the UpdateAircraftSubscription method.
        /// </summary>
        /// <remarks>It is a workkaround because the StringDataRefElement does not trigger
        /// the change reliably. Subscribing only two characters does work reliably.
        /// If the old aircraft and the new aircraft have the same characters at the position 0 and 2, \
        /// then the change will not be detected. But in most cases this should work fine.
        /// </remarks>
        private void CheckForAircraftName()
        {
            if (!_connected) return;
            // just start the connector
            _detectedAircraft = string.Empty;
            Connector?.Start();
            var datarefAircraftName0 = new DataRefElement() { DataRef = "sim/aircraft/view/acf_ui_name[0]", Frequency = 1, Value = 0 };
            var datarefAircraftName2 = new DataRefElement() { DataRef = "sim/aircraft/view/acf_ui_name[2]", Frequency = 1, Value = 0 };

            Connector.Unsubscribe(datarefAircraftName0.DataRef);
            Connector.Unsubscribe(datarefAircraftName2.DataRef);
            Connector.Subscribe(datarefAircraftName0, 1, (e, v) =>
            {
                Log.Instance.log($"sim/aircraft/view/acf_ui_name[0] = {v}", LogSeverity.Debug);
                UpdateAircraftSubscription();
            });
            Connector.Subscribe(datarefAircraftName2, 1, (e, v) =>
            {
                Log.Instance.log($"sim/aircraft/view/acf_ui_name[2] = {v}", LogSeverity.Debug);
                UpdateAircraftSubscription();
            });
        }

        public bool Disconnect()
        {
            if (_connected)
            {
                _detectedAircraft = string.Empty;
                AircraftChanged?.Invoke(this, _detectedAircraft);
                _connected = false;
                Connector.Stop();
                Closed?.Invoke(this, new EventArgs());
            }

            return _connected;
        }

        public bool IsConnected()
        {
            return _connected;
        }

        public void Start()
        {
            UnsubscribeAll();
        }

        public void Stop()
        {
            UnsubscribeAll();
        }

        private void UnsubscribeAll()
        {
            foreach (var dataRef in SubscribedDataRefs)
            {
                Connector.Unsubscribe(dataRef.Value.DataRef);
            }
            SubscribedDataRefs.Clear();
        }

        public void Clear()
        {
            UnsubscribeAll();
        }

        public float readDataRef(string dataRefPath)
        {
            if (Connector == null) return 0;

            if (!SubscribedDataRefs.ContainsKey(dataRefPath))
            {
                var dataRefElement = new DataRefElement() { DataRef = dataRefPath, Frequency = UpdateFrequencyPerSecond, Value = 0 };
                SubscribedDataRefs.Add(dataRefPath, dataRefElement);
                Connector.Subscribe(dataRefElement, UpdateFrequencyPerSecond, (e, v) =>
                {
                    SubscribedDataRefs[e.DataRef].Value = v;
                });
            }

            // make it extra safe when reading the value
            if (!SubscribedDataRefs.TryGetValue(dataRefPath, out var data)) return 0;

            return data.Value;
        }

        public void writeDataRef(string dataRefPath, float value)
        {
            Connector?.SetDataRefValue(dataRefPath, value);
        }

        public void sendCommand(string command)
        {
            XPlaneCommand xPlaneCommand = new XPlaneCommand(command, command);
            Connector?.SendCommand(xPlaneCommand);
        }
    }
}