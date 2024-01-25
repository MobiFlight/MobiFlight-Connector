using System;
using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;

namespace MobiFlight.Monitors
{
    public class DeviceMonitor
    {
        protected bool isScanning = false;
        public event EventHandler<PortDetails> PortAvailable;
        public event EventHandler<PortDetails> PortUnavailable;
        public List<PortDetails> DetectedPorts { get; set; } = new List<PortDetails>();

        private ManagementEventWatcher EventWatcher = new ManagementEventWatcher();

        public DeviceMonitor()
        {
            // EventType 2 is for connected, 3 for disconnected.
            // Most of the time several DeviceChangeEvent are fired in the process of connecting or disconnecting.
            // GROUP WITHIN 1 collects all events within one second.
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 or EventType = 3 GROUP WITHIN 1");
            EventWatcher.Query = query;
        }

        private void EventWatcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            Log.Instance.log($"DeviceMonitor EventWatcher_EventArrived: {e.NewEvent.ClassPath.Path}.", LogSeverity.Debug);            
            Scan();
        }

        public async void Start()
        {
            EventWatcher.EventArrived += EventWatcher_EventArrived;
            EventWatcher.Start();

            // Currently thread change necessary, otherwise exception in board reset process.
            await Task.Delay(50).ContinueWith(_ => Scan());                
        }

        public void Stop()
        {
            EventWatcher.Stop();
            EventWatcher.EventArrived -= EventWatcher_EventArrived;
        }

        virtual protected void Scan()
        {
            throw new NotImplementedException();
        }

        protected void UpdatePorts(List<PortDetails> ports)
        {
            // prevent concurrent modification of our DetectedPorts.
            // this could be theoretically possible because:
            // - scan events are triggered by a timer 
            // - a scan event could take longer than the timer interval
            lock (DetectedPorts)
            {
                ports.ForEach(p =>
                {
                    if (DetectedPorts.FindIndex(port => port.Name == p.Name) == -1)
                    {
                        // new port found!
                        DetectedPorts.Add(p);
                        PortAvailable?.Invoke(this, p);
                    }
                });

                var stalePorts = new List<PortDetails>();

                DetectedPorts.ForEach(p =>
                {
                    if (ports.FindIndex(port => port.Name == p.Name) == -1)
                    {
                        // stale port found!
                        stalePorts.Add(p);
                        PortUnavailable?.Invoke(this, p);
                    }
                });

                stalePorts.ForEach(p => DetectedPorts.Remove(p));
            }
        }
    }
}