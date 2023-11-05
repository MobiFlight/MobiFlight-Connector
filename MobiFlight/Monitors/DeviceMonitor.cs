using System;
using System.Collections.Generic;
using System.Timers;

namespace MobiFlight.Monitors
{
    public class DeviceMonitor
    {
        public event EventHandler<PortDetails> PortAvailable;
        public event EventHandler<PortDetails> PortUnavailable;
        public List<PortDetails> DetectedPorts { get; set; } = new List<PortDetails>();
        private Timer timer = new Timer();

        public DeviceMonitor() {
            timer.Interval = 1000;
            timer.Elapsed += Timer_Elapsed;
        }
        public void Start()
        {
            timer.Start();
        }

        public void Stop() { timer.Stop(); }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Scan();
        }

        virtual protected void Scan() { 
            throw new NotImplementedException();
        }

        protected void UpdatePorts(List<PortDetails> ports)
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
