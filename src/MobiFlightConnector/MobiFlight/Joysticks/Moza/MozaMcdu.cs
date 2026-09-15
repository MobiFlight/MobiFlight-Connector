using MobiFlight.Joysticks.Cdu;
using MobiFlightMoza;
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
        private readonly IMozaScreenControl ScreenControl;
        private readonly ICduWebsocketHub Hub;
        private string RegisteredPath;

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
        }

        public override void Connect(IntPtr handle)
        {
            base.Connect(handle);
            ScreenControl.Connect();
        }

        private void OnCabinPositionResolved(byte cabinPosition)
        {
            if (RegisteredPath != null) return;

            RegisteredPath = MozaSeatResolver.ResolvePath(cabinPosition);
            Hub.Register(RegisteredPath, this);
        }

        public void OnCduData(string json) => ScreenControl.SubmitScreenData(json);

        // MOZA v1 has no font upload (guide's 9030 channel, deferred).
        public void OnCduFont(string json) { }

        public override void SetLcdDisplay(string address, string value)
        {
            if (address != MozaConstants.ScreenAddress) return;
            ScreenControl.SubmitScreenData(value);
        }

        public override void ShowUserMessage(int messageCode, params string[] parameters)
        {
            try
            {
                ScreenControl.SubmitScreenData(CduUserMessageFormatter.Format(messageCode, parameters));
            }
            catch (Exception ex)
            {
                Log.Instance.log($"MozaMcdu - Error on show user message: {ex.Message}", LogSeverity.Error);
            }
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
