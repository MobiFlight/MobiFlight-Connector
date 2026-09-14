using System;
using MobiFlightMoza.Cdu;
using MobiFlightMoza.Protocol;

namespace MobiFlightMoza.Session
{
    internal enum MozaSessionState
    {
        Closed,
        RootHandshake,
        DeviceInit,
        Running,
        Faulted,
    }

    /// <summary>
    /// Orchestrates one CDC session end to end: root handshake, device init, then the
    /// settings and MCDU channels running side by side. Still no clock, no threads, no
    /// I/O beyond <see cref="IMozaFrameSink"/> - <see cref="MozaSessionPump"/> is the only
    /// thing that reads bytes off a real port and drives this with the real clock.
    /// </summary>
    internal sealed class MozaScreenSession
    {
        private static readonly ushort[] AcceptedServicePorts =
        [
            MozaConstants.ServicePortTelemetry,
            MozaConstants.ServicePortSettings,
            MozaConstants.ServicePortMcduTcp,
        ];

        private readonly IMozaFrameSink Sink;
        private readonly SerialLinkDecoder Decoder = new();
        private readonly ReliableStreamMultiplexer Multiplexer;
        private readonly MozaSettingsChannel SettingsChannel;
        private readonly MozaMcduChannel McduChannel;

        private bool McduStarted;
        private bool SettingsReady;
        private bool DisplayModeWritten;
        private bool CabinPositionResolvedFired;
        private byte? CachedDisplayMode;
        private bool ShuttingDown;
        private bool CloseBegun;

        // The `now` from the call currently in progress - set at the top of every public
        // entry point that receives one, so event handlers triggered synchronously during
        // that call (which carry no `now` of their own) can still act without reading the
        // clock themselves.
        private DateTime CurrentNow;

        public MozaSessionState State { get; private set; } = MozaSessionState.Closed;

        public event Action<byte> CabinPositionResolved;
        public event Action<string> ErrorMessageCreated;

        public MozaScreenSession(IMozaFrameSink sink)
        {
            Sink = sink;
            // The multiplexer/connections only ever produce Reliable Stream payloads, not
            // full wire frames - TunnelFrameSink wraps each one in the 0x43 tunnel (which
            // itself does the SerialLink framing) before it reaches the real sink. The two
            // fixed handshake frames below are already complete wire frames and bypass this.
            Multiplexer = new ReliableStreamMultiplexer(new TunnelFrameSink(sink), AcceptedServicePorts);
            Multiplexer.Error += message => ErrorMessageCreated?.Invoke(message);

            SettingsChannel = new MozaSettingsChannel(Multiplexer);
            SettingsChannel.Ready += OnSettingsReady;

            McduChannel = new MozaMcduChannel(Multiplexer);
            McduChannel.CapabilityReceived += _ => TryEnterMcduMode();
        }

        public void Start()
        {
            State = MozaSessionState.RootHandshake;
            Sink.Send(MozaConstants.RootHandshakeRequest);
        }

        public void OnBytesReceived(byte[] data, int count, DateTime now)
        {
            CurrentNow = now;
            byte[] slice = data.Length == count ? data : data[..count];
            foreach (var message in Decoder.Feed(slice, count))
            {
                Dispatch(message, now);
            }
        }

        public void Tick(DateTime now)
        {
            CurrentNow = now;
            if (State != MozaSessionState.Running) return;
            Multiplexer.Tick(now);
            SettingsChannel.Tick(now);

            // BeginClose is deliberately not called synchronously from BeginShutdown: it
            // sends a FIN immediately and claims the settings connection's one "pending"
            // slot, which would strand a just-queued restore write behind it forever (the
            // connection stops being Established once its FIN is acked). Waiting here for
            // the settings connection to have nothing pending means the restore write has
            // had its turn - either it went out, or there was never one to send.
            if (ShuttingDown && !CloseBegun
                && (!Multiplexer.TryGetConnection(MozaConstants.ServicePortSettings, out var settingsConnection) || !settingsConnection.HasPending))
            {
                CloseBegun = true;
                Multiplexer.BeginClose(now);
            }
        }

        public void SubmitPage(CduPage page)
        {
            if (State == MozaSessionState.Running && McduStarted)
            {
                McduChannel.SubmitPage(page);
            }
        }

        public void BeginShutdown(DateTime now)
        {
            ShuttingDown = true;
            if (CachedDisplayMode.HasValue)
            {
                SettingsChannel.RequestSetting(0x18, [CachedDisplayMode.Value], now);
            }
        }

        public bool IsShutdownComplete => CloseBegun && Multiplexer.IsCloseComplete;

        private void Dispatch(SerialLinkMessage message, DateTime now)
        {
            switch (State)
            {
                case MozaSessionState.RootHandshake:
                    if (message.Command == 0x80 && message.DevicePair == MozaConstants.DevicePairFromDevice)
                    {
                        State = MozaSessionState.DeviceInit;
                        Sink.Send(MozaConstants.DeviceInitRequest);
                    }
                    else
                    {
                        Fault("Unexpected reply to the root handshake.");
                    }
                    return;

                case MozaSessionState.DeviceInit:
                    // Any valid upstream frame confirms init - including the device's own
                    // SYN1 on a service port, which typically arrives instead of (or
                    // without) a separate reply to the init command.
                    if (message.DevicePair != MozaConstants.DevicePairFromDevice) return;
                    State = MozaSessionState.Running;
                    DispatchRunning(message, now);
                    return;

                case MozaSessionState.Running:
                    DispatchRunning(message, now);
                    return;
            }
        }

        private void DispatchRunning(SerialLinkMessage message, DateTime now)
        {
            if (!MozaTunnel.TryUnwrap(message, out var tunnel)) return;
            if (tunnel.InnerCommand != MozaConstants.StreamInnerCommand) return;

            Multiplexer.HandleStreamMessage(tunnel.InnerPayload, tunnel.IsReply, now);
            if (tunnel.IsReply) return; // an ACK carries no application bytes to drain

            if (!McduStarted && Multiplexer.TryGetConnection(MozaConstants.ServicePortMcduTcp, out var mcduConnection) && mcduConnection.Established)
            {
                McduStarted = true;
                McduChannel.Start();
            }

            if (Multiplexer.TryGetConnection(MozaConstants.ServicePortSettings, out var settingsConnection))
            {
                byte[] settingsBytes = settingsConnection.ReadApplicationBytes();
                if (settingsBytes.Length > 0)
                {
                    SettingsChannel.OnApplicationData(settingsBytes, now);
                    TryResolveCabinPosition();
                }
            }

            if (McduStarted && Multiplexer.TryGetConnection(MozaConstants.ServicePortMcduTcp, out var mcduData))
            {
                byte[] mcduBytes = mcduData.ReadApplicationBytes();
                if (mcduBytes.Length > 0) McduChannel.OnApplicationData(mcduBytes);
            }
        }

        private void TryResolveCabinPosition()
        {
            if (CabinPositionResolvedFired || !SettingsChannel.CabinPosition.HasValue) return;
            CabinPositionResolvedFired = true;
            CabinPositionResolved?.Invoke(SettingsChannel.CabinPosition.Value);
        }

        private void OnSettingsReady()
        {
            CachedDisplayMode = SettingsChannel.DisplayMode;
            SettingsReady = true;
            TryEnterMcduMode();
        }

        // Pins the display to the MCDU page once both the settings collection window has
        // closed and the MCDU channel is ready to accept a page - whichever finishes last
        // triggers it. Not gated on the first Keyframe's ACK specifically: a deliberate
        // simplification, at the cost of a possible one-time blank flash at connect.
        private void TryEnterMcduMode()
        {
            if (DisplayModeWritten || !SettingsReady || !McduChannel.Capability.HasValue) return;
            DisplayModeWritten = true;
            SettingsChannel.RequestSetting(0x18, [0x01], CurrentNow);
        }

        private void Fault(string message)
        {
            State = MozaSessionState.Faulted;
            ErrorMessageCreated?.Invoke(message);
        }

        // Wraps a Reliable Stream payload in the 0x43 tunnel (which does the SerialLink
        // framing itself) before forwarding it to the real sink.
        private sealed class TunnelFrameSink : IMozaFrameSink
        {
            private readonly IMozaFrameSink Inner;

            public TunnelFrameSink(IMozaFrameSink inner)
            {
                Inner = inner;
            }

            public void Send(byte[] wire) => Inner.Send(MozaTunnel.Wrap(MozaConstants.StreamInnerCommand, wire));
        }
    }
}
