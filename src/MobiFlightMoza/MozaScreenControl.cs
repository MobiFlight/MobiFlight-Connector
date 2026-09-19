using System;
using System.IO.Ports;
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
        private SerialPort Port;
        private MozaScreenSession Session;
        private readonly MozaSessionPump Pump = new();

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
            // A CDC-ACM device's line-state notification is how it knows a real host is
            // present, not just electrically attached - every other serial board in this
            // codebase (see MobiFlightModule.cs/board.json DtrEnable) already asserts this;
            // MOZA's port never did, so it always opened with DTR left low. RTS matters too:
            // a USB capture of MOZA's own Cockpit app showed SET_CONTROL_LINE_STATE ending
            // with BOTH DTR and RTS held (value 3) on every single connect, cold or warm - a
            // capture of our own app never held RTS at all on either of two failed
            // reconnects, and even the one working connect only pulsed it transiently before
            // dropping back to DTR-only. RtsEnable defaults to false and was never set here.
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

        // Deliberately a no-op for v1: the panel simply keeps showing whatever it was last
        // told to show. Revisit if that turns out to be the wrong UX (R-STOP).
        public void Stop() { }

        /// <summary>
        /// Closes the port without a Reliable Stream FIN handshake. Safe to call even if
        /// <see cref="Connect"/> never succeeded.
        /// </summary>
        /// <remarks>
        /// Deliberately does NOT send FIN on the open connections first, even though a clean
        /// FIN/ACK close is otherwise the documented way to end a session: a USB capture of
        /// MOZA's own Cockpit app closing and reopening (no replug) showed it never sends FIN
        /// either - it just stops using the port, and the device notices via its own
        /// multi-second connection timeout instead. That reconnects reliably; our own FIN-based
        /// close - byte-correct and confirmed to complete both ways - left the device unable to
        /// hand off MCDU rendering to the next session without a physical replug. Matching what
        /// the real client does, not what the written procedure says, is what's proven to work.
        /// <para>
        /// Does not attempt to restore displayMode here either, for the same reason: that
        /// depends on Shutdown() actually running to completion, which a crash or force-quit
        /// skips entirely. Session.TryEnterMcduMode() forces a fresh 0-then-1 transition on
        /// every connect instead, so the next session doesn't depend on this one having
        /// cleaned up after itself.
        /// </para>
        /// </remarks>
        public void Shutdown()
        {
            if (Port == null) return;

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
            public void Send(byte[] wire, bool isReply = false) => Port.BaseStream.Write(wire, 0, wire.Length);
        }
    }
}
