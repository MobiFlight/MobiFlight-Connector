namespace MobiFlightWwFcu
{
    internal class WinwingPto2Device : SimpleOutputDeviceBase
    {
        public override string Name => "WinWing PTO2";

        private const string LG_BRIGHTNESS   = "Landing Gear Percentage";  // 0x01
        private const string SL_BRIGHTNESS   = "SL Percentage";            // 0x02
        private const string FLAG_BRIGHTNESS = "FLAG Percentage";          // 0x03

        public WinwingPto2Device(IWinwingMessageSender sender)
            : base(sender, WinwingConstants.DEST_PTO2)
        {
            LedIdentifiers.Add("MASTER_CAUTION", 0x04);
            LedIdentifiers.Add("JETT",  0x05);
            LedIdentifiers.Add("CTR",   0x06);
            LedIdentifiers.Add("LI",    0x07);
            LedIdentifiers.Add("LO",    0x08);
            LedIdentifiers.Add("RO",    0x09);
            LedIdentifiers.Add("RI",    0x0a);
            LedIdentifiers.Add("FLAPS", 0x0b);
            LedIdentifiers.Add("NOSE",  0x0c);
            LedIdentifiers.Add("FULL",  0x0d);
            LedIdentifiers.Add("RIGHT", 0x0e);
            LedIdentifiers.Add("LEFT",  0x0f);
            LedIdentifiers.Add("HALF",  0x10);
            LedIdentifiers.Add("HOOK",  0x11);

            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));
            OutputNameToActionMapping.Add(LG_BRIGHTNESS,   Brightness(0x01));
            OutputNameToActionMapping.Add(SL_BRIGHTNESS,   Brightness(0x02));
            OutputNameToActionMapping.Add(FLAG_BRIGHTNESS, Brightness(0x03));

            InitializeCaches();
        }

        protected override void OnConnect()
        {
            InvokeOutputBrightness(BACK_BRIGHTNESS, 50);
        }

        protected override void OnShutdown()
        {
            InvokeOutputBrightness(BACK_BRIGHTNESS, 0);
            InvokeOutputBrightness(LG_BRIGHTNESS, 0);
            InvokeOutputBrightness(SL_BRIGHTNESS, 0);
            InvokeOutputBrightness(FLAG_BRIGHTNESS, 0);
        }
    }
}
