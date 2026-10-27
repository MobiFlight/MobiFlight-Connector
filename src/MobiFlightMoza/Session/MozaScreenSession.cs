using System;
using System.Collections.Generic;
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
    /// Every public entry point takes <see cref="Gate"/>: the pump thread (bytes, Tick) and
    /// the caller's thread (pages, LEDs, brightness) share the multiplexer queues and the sink.
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
        // Fallback if the device does not echo the displayMode write: declare the screen ready anyway.
        private const double DisplayModeEchoTimeoutSeconds = 2.0;
        private const int DisplayModeSettingId = 0x18;
        private const int DisplayBrightnessSettingId = 0x01;
        private const byte DisplayModeMcdu = 0x01;

        private readonly object Gate = new();
        // Panel commands issued before the device init completed, latest value per target, in issue order.
        private readonly Dictionary<int, byte[]> PendingPanelCommands = [];
        private readonly List<int> PendingPanelOrder = [];
        private byte? DisplayBrightnessPercent;
        private bool DisplayBrightnessSent;

        private DateTime? DisplayModeWrittenAt;
        private bool DisplayModeConfirmed;
        private bool ScreenReadyReached;
        private bool ScreenReadyToRaise;

        private bool McduStarted;
        private bool SettingsReady;
        public bool DisplayModeWritten;
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

        /// <summary>
        /// True once the display accepts screen content: the device has echoed displayMode 1
        /// (MCDU) and the 9050 MCDU channel has negotiated its capability.
        /// </summary>
        public bool IsScreenReady => ScreenReadyReached;

        /// <summary>Raised once per session when <see cref="IsScreenReady"/> becomes true (outside the session lock).</summary>
        public event Action ScreenReady;
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
            SettingsChannel.SettingEchoed += OnSettingEchoed;

            TelemetryChannel = new MozaTelemetryChannel(Multiplexer);

            McduChannel = new MozaMcduChannel(Multiplexer);
            McduChannel.CapabilityReceived += _ =>
            {
                TraceCreated?.Invoke("MCDU ClientCapability received.");
                TryEnterMcduMode();
                TryReachScreenReady("MCDU capability received after displayMode confirmation");
            };
        }

        public void Start()
        {
            lock (Gate)
            {
                State = MozaSessionState.RootHandshake;
                Sink.Send(MozaConstants.RootHandshakeRequest);
            }
        }

        public void OnBytesReceived(byte[] data, int count, DateTime now)
        {
            lock (Gate)
            {
                CurrentNow = now;
                byte[] slice = data.Length == count ? data : data[..count];
                foreach (var message in Decoder.Feed(slice, count))
                {
                    Dispatch(message, now);
                }
            }
            RaiseScreenReadyIfDue();
        }

        public void Tick(DateTime now)
        {
            lock (Gate)
            {
                CurrentNow = now;
                if (State != MozaSessionState.Running) return;

                Multiplexer.Tick(now);
                SettingsChannel.Tick(now);
                CheckDisplayModeEchoTimeout(now);

                if (ShuttingDown && !CloseBegun)
                {
                    CloseBegun = true;
                    Multiplexer.BeginClose(now);
                }
            }
            RaiseScreenReadyIfDue();
        }

        // Forwards unconditionally - MozaMcduChannel already holds a page submitted before
        // its capability negotiation completes and sends it as soon as it does, so a page
        // submitted before the MCDU connection even exists isn't lost either.
        public void SubmitPage(CduPage page)
        {
            lock (Gate)
            {
                McduChannel.SubmitPage(page);
            }
        }

        public void ForceResend()
        {
            lock (Gate)
            {
                McduChannel.ForceResend();
            }
        }

        /// <summary>
        /// Sends a plain panel command (LED, lighting group brightness / mode) to the control
        /// MCU. Before the device init has completed only the latest command per target is kept
        /// and sent as soon as the session is running.
        /// </summary>
        public void SendPanelCommand(byte[] payload)
        {
            lock (Gate)
            {
                if (State == MozaSessionState.Running)
                {
                    Sink.Send(MozaPanelCommand.Encode(payload));
                    return;
                }
                int key = MozaPanelCommand.TargetKey(payload);
                if (!PendingPanelCommands.ContainsKey(key)) PendingPanelOrder.Add(key);
                PendingPanelCommands[key] = payload;
            }
        }

        /// <summary>
        /// Sets the display backlight via 9020 setting 0x01 (percent), the value MOZA Cockpit
        /// writes for its Display brightness slider. Applied once the settings channel is ready
        /// if the session is not there yet.
        /// </summary>
        public void SetDisplayBrightness(byte percent)
        {
            lock (Gate)
            {
                DisplayBrightnessPercent = Math.Min(percent, (byte)100);
                DisplayBrightnessSent = false;
                TrySendDisplayBrightness();
            }
        }

        // Callable from any thread: only sets fields. Tick (pump thread) does the actual
        // work once it observes ShuttingDown.
        public void BeginShutdown(DateTime now)
        {
            lock (Gate)
            {
                ShutdownDeadline = now.AddSeconds(ShutdownGracePeriodSeconds);
                ShuttingDown = true;
            }
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
                    FlushPendingPanelCommands();
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
            TrySendDisplayBrightness();
            TryEnterMcduMode();
        }

        private void OnSettingEchoed(int settingId, byte[] data)
        {
            TraceCreated?.Invoke($"Setting 0x{settingId:X2} echoed by device: [{string.Join(",", data)}].");
            // The forced 0 -> 1 transition is echoed twice; only the final MCDU value counts.
            if (DisplayModeWrittenAt.HasValue && settingId == DisplayModeSettingId && data.Length > 0 && data[0] == DisplayModeMcdu)
            {
                DisplayModeConfirmed = true;
                TryReachScreenReady("displayMode 1 echoed by device");
            }
        }

        private void CheckDisplayModeEchoTimeout(DateTime now)
        {
            if (ScreenReadyReached || DisplayModeConfirmed || !DisplayModeWrittenAt.HasValue) return;
            if ((now - DisplayModeWrittenAt.Value).TotalSeconds < DisplayModeEchoTimeoutSeconds) return;
            DisplayModeConfirmed = true;
            TryReachScreenReady($"no displayMode echo within {DisplayModeEchoTimeoutSeconds} s, assuming MCDU mode");
        }

        private void TryReachScreenReady(string reason)
        {
            if (ScreenReadyReached || !DisplayModeConfirmed || !McduChannel.Capability.HasValue) return;
            ScreenReadyReached = true;
            ScreenReadyToRaise = true;
            TraceCreated?.Invoke($"Screen ready ({reason}).");
            // Pages sent before the mode switch may have been rendered in the old mode - repaint.
            McduChannel.ForceResend();
        }

        // Called without holding Gate, so a handler may call back into the session (e.g. submit a page).
        private void RaiseScreenReadyIfDue()
        {
            bool raise;
            lock (Gate)
            {
                raise = ScreenReadyToRaise;
                ScreenReadyToRaise = false;
            }
            if (raise) ScreenReady?.Invoke();
        }

        private void FlushPendingPanelCommands()
        {
            foreach (int key in PendingPanelOrder)
            {
                Sink.Send(MozaPanelCommand.Encode(PendingPanelCommands[key]));
            }
            PendingPanelOrder.Clear();
            PendingPanelCommands.Clear();
        }

        private void TrySendDisplayBrightness()
        {
            if (DisplayBrightnessSent || !DisplayBrightnessPercent.HasValue || !SettingsReady) return;
            DisplayBrightnessSent = true;
            SettingsChannel.RequestSetting(DisplayBrightnessSettingId, [DisplayBrightnessPercent.Value, 0, 0, 0]);
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
            DisplayModeWrittenAt = CurrentNow;
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
