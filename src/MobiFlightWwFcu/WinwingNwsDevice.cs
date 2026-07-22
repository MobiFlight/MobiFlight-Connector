namespace MobiFlightWwFcu
{
    internal class WinwingNwsDevice : SimpleOutputDeviceBase
    {
        public override string Name => $"WinWing {VariantName}";

        private readonly string VariantName;

        public WinwingNwsDevice(IWinwingMessageSender sender, string variantName)
            : base(sender, WinwingConstants.DEST_NWS)
        {
            VariantName = variantName;

            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));

            InitializeCaches();
        }

        protected override void OnConnect()
        {
            InvokeOutputBrightness(BACK_BRIGHTNESS, 50);
        }

        protected override void OnShutdown()
        {
            InvokeOutputBrightness(BACK_BRIGHTNESS, 0);
        }

        public override void Stop()
        {
            // NWS has no controllable LEDs — Stop is a no-op.
        }
    }
}
