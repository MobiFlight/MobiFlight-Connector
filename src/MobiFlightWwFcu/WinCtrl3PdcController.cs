namespace MobiFlightWwFcu
{
    internal class WinCtrl3PdcController : SimpleOutputControllerBase
    {
        public override string Name => $"WinCtrl {PdcType}";

        private readonly string PdcType;

        public WinCtrl3PdcController(IWinCtrlMessageSender sender, string pdcType)
            : base(sender, ResolveDestination(pdcType))
        {
            PdcType = pdcType;

            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));

            InitializeCaches();
        }

        private static byte[] ResolveDestination(string pdcType)
        {
            if (pdcType == WinCtrlConstants.PDC3NL_NAME || pdcType == WinCtrlConstants.PDC3NR_NAME)
            {
                return WinCtrlConstants.DEST_3NPDC;
            }
            if (pdcType == WinCtrlConstants.PDC3ML_NAME || pdcType == WinCtrlConstants.PDC3MR_NAME)
            {
                return WinCtrlConstants.DEST_3MPDC;
            }
            return WinCtrlConstants.DEST_3NPDC;
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
            // 3PDC has no controllable LEDs — Stop is a no-op (preserves prior behaviour).
        }
    }
}
