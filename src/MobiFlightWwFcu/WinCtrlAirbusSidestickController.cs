namespace MobiFlightWwFcu
{
    internal class WinCtrlAirbusSidestickController : SimpleOutputControllerBase
    {
        public override string Name => $"WinCtrl {StickType}";

        private readonly string StickType;
        private readonly byte[] DestinationAddressVibration;

        private const string VIBRATION = "Vibration Percentage";
        private const string LIGHT_PULSE = "Backlight Pulse On/Off";

        public WinCtrlAirbusSidestickController(IWinCtrlMessageSender sender, string stickType)
            : base(sender, WinCtrlConstants.DEST_AIRBUS_STICK)
        {
            StickType = stickType;
            DestinationAddressVibration = (StickType == WinCtrlConstants.AIRBUS_STICK_L_NAME)
                ? WinCtrlConstants.DEST_AIRBUS_STICK_VIBRATION_L
                : WinCtrlConstants.DEST_AIRBUS_STICK_VIBRATION_R;

            OutputNameToActionMapping.Add(VIBRATION,       SetVibration);
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));
            OutputNameToActionMapping.Add(LIGHT_PULSE,     SetBacklightPulse);

            InitializeCaches();
        }

        protected override void OnConnect()
        {
            InvokeOutputBrightness(BACK_BRIGHTNESS, 20);
            SetVibration(0);
        }

        protected override void OnShutdown()
        {
            InvokeOutputBrightness(BACK_BRIGHTNESS, 0);
            SetVibration(0);
        }

        public override void Stop()
        {
            SetVibration(0);
        }

        private void SetVibration(byte level)
        {
            MessageSender.SetVibration(DestinationAddressVibration, 0x00, level);
        }

        private void SetBacklightPulse(byte isOnValue)
        {
            MessageSender.SetPulseLight(DestinationAddress, isOnValue != 0);
        }
    }
}
