using MobiFlight.Joysticks.Cdu;
using MobiFlightMoza;
using MobiFlightWwFcu;
using System;

namespace MobiFlight.Joysticks.Moza
{
    /// <summary>
    /// The MOZA FCD Display's display module, on top of the base unit's buttons/LEDs.
    /// Registers with the shared <see cref="ICduWebsocketHub"/> once the device's own
    /// <c>cabinPosition</c> setting resolves which seat path it drives - not eagerly at
    /// <see cref="Connect"/> time, since the seat isn't known yet.
    /// </summary>
    internal class MozaMcdu : MozaBaseController, ICduDataConsumer
    {
        private static readonly TimeSpan ScreenConnectRetryInterval = TimeSpan.FromSeconds(2);
        // Experiment: MOZA's own app kept pushing keyframes continuously while the screen
        // was actually rendering content, rather than sending one page and stopping - a
        // single static page never got past the firmware's own HOMEPAGE. Resending
        // whatever page is current on an interval tests whether that sustained traffic is
        // what the firmware is waiting for.
        private static readonly TimeSpan ScreenRefreshInterval = TimeSpan.FromSeconds(1);

        private readonly IMozaScreenControl ScreenControl;
        private readonly ICduWebsocketHub Hub;
        private string RegisteredPath;
        private bool ScreenConnected;
        private DateTime NextScreenConnectAttempt = DateTime.MinValue;
        private string LastSubmittedJson;
        private DateTime NextScreenRefresh = DateTime.MaxValue;

        public MozaMcdu(JoystickDefinition definition, ICduWebsocketHub hub)
            : this(definition, hub, new MozaScreenControl())
        {
        }

        // Testing seam: lets tests substitute a fake IMozaScreenControl.
        internal MozaMcdu(JoystickDefinition definition, ICduWebsocketHub hub, IMozaScreenControl screenControl)
            : base(definition)
        {
            Hub = hub;
            ScreenControl = screenControl;
            ScreenControl.CabinPositionResolved += OnCabinPositionResolved;
            ScreenControl.ErrorMessageCreated += message => Log.Instance.log(message, LogSeverity.Error);
            ScreenControl.TraceCreated += message => Log.Instance.log(message, LogSeverity.Debug);
        }

        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            TryConnectScreen();
        }

        public override void Update()
        {
            base.Update();
            if (!ScreenConnected)
            {
                TryConnectScreen();
                return;
            }
            if (LastSubmittedJson != null && DateTime.Now >= NextScreenRefresh)
            {
                // ForceResend, not SubmitScreenData - resubmitting identical JSON would just
                // get silently deduped by McduFrameBuilder's own unchanged-page check and
                // never actually touch the wire.
                ScreenControl.ForceResend();
                NextScreenRefresh = DateTime.Now + ScreenRefreshInterval;
            }
        }

        // The CDC display interface can still be mid-enumeration by Windows when the base
        // unit's HID interface is already usable (e.g. right after a power cycle), so a
        // single attempt at Connect() time isn't reliable - retried here, throttled so a
        // missing device doesn't repeat the WMI port lookup on every 20ms poll tick.
        private void TryConnectScreen()
        {
            if (ScreenConnected || DateTime.Now < NextScreenConnectAttempt) return;

            if (ScreenControl.Connect())
            {
                ScreenConnected = true;
                SubmitScreenData(WinCtrlConstants.InitialDisplayJson);
            }
            else
            {
                Log.Instance.log($"{Name} - CDC display port not found yet, will retry.", LogSeverity.Debug);
                NextScreenConnectAttempt = DateTime.Now + ScreenConnectRetryInterval;
            }
        }

        private void OnCabinPositionResolved(byte cabinPosition)
        {
            if (RegisteredPath != null) return;

            RegisteredPath = MozaSeatResolver.ResolvePath(cabinPosition);
            Hub.Register(RegisteredPath, this);
        }

        public void OnCduData(string json) => SubmitScreenData(json);

        // MOZA v1 has no font upload (guide's 9030 channel, deferred).
        public void OnCduFont(string json) { }

        public override void SetLcdDisplay(string address, string value)
        {
            if (address != MozaConstants.ScreenAddress) return;
            SubmitScreenData(value);
        }

        public override void ShowUserMessage(int messageCode, params string[] parameters)
        {
            try
            {
                SubmitScreenData(CduUserMessageFormatter.Format(messageCode, parameters));
            }
            catch (Exception ex)
            {
                Log.Instance.log($"MozaMcdu - Error on show user message: {ex.Message}", LogSeverity.Error);
            }
        }

        private void SubmitScreenData(string json)
        {
            ScreenControl.SubmitScreenData(json);
            LastSubmittedJson = json;
            NextScreenRefresh = DateTime.Now + ScreenRefreshInterval;
        }

        public override void Stop()
        {
            base.Stop();
            ScreenControl.Stop();
        }

        public override void Shutdown()
        {
            if (RegisteredPath != null)
            {
                Hub.Unregister(RegisteredPath, this);
                RegisteredPath = null;
            }

            ScreenControl.Shutdown();
            base.Shutdown();
        }
    }
}
