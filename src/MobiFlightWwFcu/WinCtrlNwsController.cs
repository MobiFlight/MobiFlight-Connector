namespace MobiFlightWwFcu
{
    internal class WinCtrlNwsController : SimpleOutputControllerBase
    {
        public override string Name => $"WinCtrl {VariantName}";

        private readonly string VariantName;

        public WinCtrlNwsController(IWinCtrlMessageSender sender, string variantName)
            : base(sender, WinCtrlConstants.DEST_NWS)
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
