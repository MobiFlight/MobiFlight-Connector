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
    internal sealed class MozaScreenSession : IMozaSessionDriver
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
        private readonly MozaTelemetryChannel TelemetryChannel;

        private const double ShutdownGracePeriodSeconds = 3.0;

        private bool McduStarted;
        private bool SettingsReady;
        private bool DisplayModeWritten;
        private bool CabinPositionResolvedFired;
        private byte? CachedDisplayMode;
        private bool CloseBegun;
        private DateTime? ShutdownDeadline;

        // volatile: BeginShutdown may run on a different thread than Tick/OnBytesReceived
        // (the pump thread), and this write publishes ShutdownDeadline (set just before it).
        private volatile bool ShuttingDown;

        // The `now` of the call in progress, for event handlers triggered synchronously
        // during that call that need a clock but receive no `now` of their own.
        private DateTime CurrentNow;

        public MozaSessionState State { get; private set; } = MozaSessionState.Closed;

        public event Action<byte> CabinPositionResolved;
        public event Action<string> ErrorMessageCreated;
        // Milestone tracing through the settings/MCDU bring-up - genuinely useful for
        // partner hardware bring-up, not just a one-off debugging aid.
        public event Action<string> TraceCreated;

        public MozaScreenSession(IMozaFrameSink sink)
        {
            Sink = sink;
            // Multiplexer/connections only produce Reliable Stream payloads; TunnelFrameSink
            // wraps each in the 0x43 tunnel before it reaches the real sink.
            Multiplexer = new ReliableStreamMultiplexer(new TunnelFrameSink(sink), AcceptedServicePorts);
            Multiplexer.Error += message => ErrorMessageCreated?.Invoke(message);

            SettingsChannel = new MozaSettingsChannel(Multiplexer);
            SettingsChannel.Ready += OnSettingsReady;
            SettingsChannel.SettingEchoed += (settingId, data) =>
                TraceCreated?.Invoke($"Setting 0x{settingId:X2} echoed by device: [{string.Join(",", data)}].");

            TelemetryChannel = new MozaTelemetryChannel(Multiplexer);

            McduChannel = new MozaMcduChannel(Multiplexer);
            McduChannel.CapabilityReceived += _ =>
            {
                TraceCreated?.Invoke("MCDU ClientCapability received.");
                TryEnterMcduMode();
            };
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

            if (!ShuttingDown) return;

            if (!CloseBegun)
            {
                CloseBegun = true;
                Multiplexer.BeginClose(now);
            }
        }

        // Forwards unconditionally - MozaMcduChannel already holds a page submitted before
        // its capability negotiation completes and sends it as soon as it does, so a page
        // submitted before the MCDU connection even exists isn't lost either.
        public void SubmitPage(CduPage page)
        {
            McduChannel.SubmitPage(page);
        }

        public void ForceResend() => McduChannel.ForceResend();

        // Callable from any thread: only sets fields. Tick (pump thread) does the actual
        // work once it observes ShuttingDown, keeping every real mutation single-threaded.
        public void BeginShutdown(DateTime now)
        {
            ShutdownDeadline = now.AddSeconds(ShutdownGracePeriodSeconds);
            ShuttingDown = true; // written last: the volatile write publishes ShutdownDeadline too
        }

        public bool IsShutdownComplete
        {
            get
            {
                if (!CloseBegun) return false;
                if (Multiplexer.IsCloseComplete) return true;
                return ShutdownDeadline.HasValue && CurrentNow >= ShutdownDeadline.Value;
            }
        }

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
                        Fault($"Unexpected reply to the root handshake (got Command=0x{message.Command:X2} DevicePair=0x{message.DevicePair:X2}, expected Command=0x80 DevicePair=0x{MozaConstants.DevicePairFromDevice:X2}).");
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
                TraceCreated?.Invoke("MCDU (9050) connection established, InitConfig sent.");
            }

            DrainChannel(MozaConstants.ServicePortSettings, bytes =>
            {
                SettingsChannel.OnApplicationData(bytes, now);
                TryResolveCabinPosition();
            });
            DrainChannel(MozaConstants.ServicePortTelemetry, TelemetryChannel.OnApplicationData);
            if (McduStarted) DrainChannel(MozaConstants.ServicePortMcduTcp, McduChannel.OnApplicationData);
        }

        // Forwards a connection's newly-received application bytes to onData, skipping the
        // call entirely if the connection doesn't exist yet or has nothing new.
        private void DrainChannel(ushort servicePort, Action<byte[]> onData)
        {
            if (!Multiplexer.TryGetConnection(servicePort, out var connection)) return;
            byte[] bytes = connection.ReadApplicationBytes();
            if (bytes.Length > 0) onData(bytes);
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
            TraceCreated?.Invoke($"Settings channel ready. Cached displayMode={CachedDisplayMode?.ToString() ?? "(none reported)"}, cabinPosition={SettingsChannel.CabinPosition?.ToString() ?? "(none reported)"}.");
            TryEnterMcduMode();
        }

        // Entered once both settings collection and MCDU capability negotiation are done -
        // by then the first Keyframe has already gone out (SubmitPage sends it as soon as
        // Capability arrives), so displayMode is only flipped after the page is ready.
        private void TryEnterMcduMode()
        {
            if (DisplayModeWritten) return;
            if (!SettingsReady || !McduChannel.Capability.HasValue)
            {
                TraceCreated?.Invoke($"Not entering MCDU mode yet: SettingsReady={SettingsReady} McduCapabilityReceived={McduChannel.Capability.HasValue}.");
                return;
            }
            DisplayModeWritten = true;
            // Deliberate: forces displayMode through 0 then 1 on every connect, since a
            // same-value write gets no device echo/repaint and a prior session that exited
            // uncleanly could leave it already at 1. Intentional, load-bearing - see the
            // implementation-state doc before changing this.
            TraceCreated?.Invoke("Forcing displayMode 0 then 1 (MCDU) to guarantee a genuine transition.");
            SettingsChannel.RequestSetting(0x18, [0x00]);
            SettingsChannel.RequestSetting(0x18, [0x01]);
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

            public void Send(byte[] wire, bool isReply = false) => Inner.Send(MozaTunnel.Wrap(MozaConstants.StreamInnerCommand, wire, isReply));
        }
    }
}
