namespace MobiFlightWwFcu
{
    internal class Winwing3PdcDevice : SimpleOutputDeviceBase
    {
        public override string Name => $"WinWing {PdcType}";

        private readonly string PdcType;

        public Winwing3PdcDevice(IWinwingMessageSender sender, string pdcType)
            : base(sender, ResolveDestination(pdcType))
        {
            PdcType = pdcType;

            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));

            InitializeCaches();
        }

        private static byte[] ResolveDestination(string pdcType)
        {
            if (pdcType == WinwingConstants.PDC3NL_NAME || pdcType == WinwingConstants.PDC3NR_NAME)
            {
                return WinwingConstants.DEST_3NPDC;
            }
            if (pdcType == WinwingConstants.PDC3ML_NAME || pdcType == WinwingConstants.PDC3MR_NAME)
            {
                return WinwingConstants.DEST_3MPDC;
            }
            return WinwingConstants.DEST_3NPDC;
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
