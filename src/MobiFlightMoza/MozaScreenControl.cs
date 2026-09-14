using System;
using System.IO.Ports;
using System.Threading;
using MobiFlightMoza.Cdu;
using MobiFlightMoza.Protocol;
using MobiFlightMoza.Session;

namespace MobiFlightMoza
{
    /// <summary>
    /// The only class in this library that owns a <see cref="SerialPort"/>. Locates
    /// MOZA's CDC display port, opens it, and wires a <see cref="MozaScreenSession"/> plus
    /// the <see cref="MozaSessionPump"/> that drives it over the real stream and clock.
    /// </summary>
    public sealed class MozaScreenControl : IMozaScreenControl
    {
        private const int ShutdownWaitMilliseconds = 5000;
        private const int ShutdownPollMilliseconds = 50;

        private SerialPort Port;
        private MozaScreenSession Session;
        private readonly MozaSessionPump Pump = new();

        public event Action<byte> CabinPositionResolved;
        public event Action<string> ErrorMessageCreated;

        /// <summary>
        /// Locates and opens MOZA's CDC display port and starts driving it. Returns false
        /// - without raising <see cref="ErrorMessageCreated"/> - if hardware IDs aren't
        /// configured yet or no matching port is currently attached; that's an expected
        /// state, not a failure. Only an actual open failure (the port exists but couldn't
        /// be opened, e.g. it's held by MOZA's own software) raises the event.
        /// </summary>
        public bool Connect()
        {
            if (Port != null) return true; // already connected
            if (!MozaHardwareIds.AreConfigured) return false;

            var candidates = MozaPortLocator.FindPorts(MozaHardwareIds.VendorId, MozaHardwareIds.McduProductIds);
            if (candidates.Count == 0) return false;

            string portName = candidates[0].PortName;
            SerialPort port = new(portName, MozaConstants.BaudRate);
            try
            {
                port.Open();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or System.IO.IOException)
            {
                ErrorMessageCreated?.Invoke($"Could not open {portName}: {ex.Message}");
                return false;
            }

            Port = port;
            Session = new MozaScreenSession(new SerialStreamFrameSink(port));
            Session.CabinPositionResolved += value => CabinPositionResolved?.Invoke(value);
            Session.ErrorMessageCreated += message => ErrorMessageCreated?.Invoke(message);

            Pump.Start(port.BaseStream, Session, MozaConstants.ReadBufferSize, OnPumpError, "MozaSessionPump");
            return true;
        }

        public void SubmitScreenData(string json)
        {
            if (Session == null) return;
            if (!CduPage.TryParse(json, out var page, out var error))
            {
                ErrorMessageCreated?.Invoke($"Could not parse MCDU screen data: {error}");
                return;
            }
            Session.SubmitPage(page);
        }

        // Deliberately a no-op for v1: the panel simply keeps showing whatever it was last
        // told to show. Revisit if that turns out to be the wrong UX (R-STOP).
        public void Stop() { }

        /// <summary>
        /// Signals the session to restore the device's own display mode and close every
        /// open channel, waits (bounded) for that to finish, then closes the port. Safe to
        /// call even if <see cref="Connect"/> never succeeded.
        /// </summary>
        public void Shutdown()
        {
            if (Port == null) return;

            Session.BeginShutdown(DateTime.Now);

            int deadline = Environment.TickCount + ShutdownWaitMilliseconds;
            while (Pump.IsRunning && !Session.IsShutdownComplete && Environment.TickCount < deadline)
            {
                Thread.Sleep(ShutdownPollMilliseconds);
            }

            Pump.Stop();

            try { Port.Close(); } catch { }
            Port = null;
            Session = null;
        }

        private void OnPumpError(Exception ex) => ErrorMessageCreated?.Invoke(ex.Message);

        // Wraps the port's own stream - the write side IMozaFrameSink abstracts away for
        // everything below the session layer.
        private sealed class SerialStreamFrameSink : IMozaFrameSink
        {
            private readonly SerialPort Port;

            public SerialStreamFrameSink(SerialPort port) => Port = port;

            public void Send(byte[] wire) => Port.BaseStream.Write(wire, 0, wire.Length);
        }
    }
}
