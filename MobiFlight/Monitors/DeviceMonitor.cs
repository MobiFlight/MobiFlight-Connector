using System;
using System.Collections.Generic;
using System.Timers;

namespace MobiFlight.Monitors
{
    public class DeviceMonitor : IDisposable
    {
        protected bool isScanning = false;
        public event EventHandler<PortDetails> PortAvailable;
        public event EventHandler<PortDetails> PortUnavailable;
        public List<PortDetails> DetectedPorts { get; set; } = new List<PortDetails>();

        // initial timer interval will be 10 
        // so it will execute Timer_Elapsed immediately
        private readonly Timer timer = new Timer(10);

        const double TimerIntervalInMs = 1000;

        public DeviceMonitor() {
            timer.Elapsed += Timer_Elapsed;
        }
        public void Start()
        {
            timer.Start();
        }

        public void Stop() 
        { 
            timer.Stop(); 
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // we initialize the timer with default 10ms
            // and on first timer_elapsed we set it to the
            // regular interval `TimerIntervalInMs`
            timer.Interval = TimerIntervalInMs;
            Scan();
        }

        virtual protected void Scan() { 
            throw new NotImplementedException();
        }

        protected void UpdatePorts(List<PortDetails> ports)
        {
            // prevent concurrent modification of our DetectedPorts.
            // this could be theoretically possible because:
            // - scan events are triggered by a timer 
            // - a scan event could take longer than the timer interval
            lock(DetectedPorts)
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

        public void Dispose()
        {
            ((IDisposable)timer).Dispose();
        }
    }
}
