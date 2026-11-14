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
        public event Action ScreenReady;
        public event Action<byte> CabinPositionResolved;
        public event Action<string> ErrorMessageCreated;
        public event Action<string> TraceCreated;

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
            // DTR must be asserted - a CDC-ACM device uses it to know a real host is present,
            // not just electrically attached (see implementation-state doc for the USB
            // capture that found this; MOZA's own app also holds RTS, but DTR alone connects
            // reliably in testing).
            SerialPort port = new(portName, MozaConstants.BaudRate) { DtrEnable = true };
            try
            {
                port.Open();
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or System.IO.IOException)
            {
                ErrorMessageCreated?.Invoke($"Could not open {portName}: {ex.Message}");
                return false;
            }

            // Bytes left over in the OS's receive buffer from a previous session (the
            // device keeps transmitting even with nobody listening) would otherwise be
            // mistaken for the reply to our own root handshake request below.
            port.DiscardInBuffer();

            Port = port;
            Session = new MozaScreenSession(new SerialStreamFrameSink(port));
            Session.ScreenReady += () => ScreenReady?.Invoke();
            Session.CabinPositionResolved += value => CabinPositionResolved?.Invoke(value);
            Session.ErrorMessageCreated += message => ErrorMessageCreated?.Invoke(message);
            Session.TraceCreated += message => TraceCreated?.Invoke(message);

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

        public void ForceResend() => Session?.ForceResend();

        /// <summary>Switches one annunciator LED (device index, e.g. MCDU 0 = FM1, Boeing 2 = EXEC).</summary>
        public void SetLed(byte index, bool on)
        {
            Session?.SendPanelCommand(MozaPanelCommand.Led(index, on));
        }

        /// <summary>
        /// Brightness 0..15 of a lighting group (the Cockpit slider). Only effective while the
        /// group is in <see cref="MozaLightMode.Manual"/>.
        /// </summary>
        public void SetLightGroupBrightness(MozaLightGroup group, int level)
        {
            Session?.SendPanelCommand(MozaPanelCommand.Brightness(group, level));
        }

        public void SetLightGroupMode(MozaLightGroup group, MozaLightMode mode)
        {
            Session?.SendPanelCommand(MozaPanelCommand.LightMode(group, mode));
        }

        /// <summary>Display backlight in percent (0 switches the display off), as Cockpit's Display slider does.</summary>
        public void SetDisplayBrightness(int percent)
        {
            Session?.SetDisplayBrightness((byte)Math.Clamp(percent, 0, 100));
        }

        // Deliberately a no-op for v1: the panel simply keeps showing whatever it was last
        // told to show. Revisit if that turns out to be the wrong UX (R-STOP).
        public void Stop() { }

        /// <summary>
        /// Signals the session to close every open Reliable Stream connection with FIN,
        /// waits (bounded) for that to finish, then closes the port. Safe to call even if
        /// <see cref="Connect"/> never succeeded.
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

            // wire is already a complete, correctly-flagged SerialLink frame by the time it
            // reaches the real port - isReply only matters one layer up, at the tunnel wrap.
            // The session serializes its own sends; the lock keeps whole frames together even
            // if another writer is added later.
            public void Send(byte[] wire, bool isReply = false)
            {
                lock (this)
                {
                    Port.BaseStream.Write(wire, 0, wire.Length);
                }
            }
        }
    }
}