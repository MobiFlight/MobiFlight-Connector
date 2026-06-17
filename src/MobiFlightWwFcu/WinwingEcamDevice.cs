namespace MobiFlightWwFcu
{
    internal class WinwingEcamDevice : SimpleOutputDeviceBase
    {
        public override string Name => "WinWing ECAM";

        public WinwingEcamDevice(IWinwingMessageSender sender)
            : base(sender, WinwingConstants.DEST_ECAM)
        {
            LedIdentifiers.Add("EMER_CANC", 0x03);
            LedIdentifiers.Add("ENG",       0x04);
            LedIdentifiers.Add("BLEED",     0x05);
            LedIdentifiers.Add("PRESS",     0x06);
            LedIdentifiers.Add("ELEC",      0x07);
            LedIdentifiers.Add("HYD",       0x08);
            LedIdentifiers.Add("FUEL",      0x09);
            LedIdentifiers.Add("APU",       0x0a);
            LedIdentifiers.Add("COND",      0x0b);
            LedIdentifiers.Add("DOOR",      0x0c);
            LedIdentifiers.Add("WHEEL",     0x0d);
            LedIdentifiers.Add("FCTL",      0x0e);
            LedIdentifiers.Add("CLR_L",     0x0f);
            LedIdentifiers.Add("STS",       0x10);
            LedIdentifiers.Add("CLR_R",     0x11);

            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));
            OutputNameToActionMapping.Add(LED_BRIGHTNESS,  Brightness(0x01));

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
    }
}
