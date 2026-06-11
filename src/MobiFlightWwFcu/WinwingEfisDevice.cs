using System.Collections.Generic;
using System.Globalization;

namespace MobiFlightWwFcu
{
    internal class WinwingEfisDevice : SegmentDisplayDeviceBase
    {
        public override string Name => $"WinWing EFIS {EfisType}";

        protected override string SetValuesHeaderKey => "0201_E";

        private readonly string EfisType;

        // https://docs.flybywiresim.com/pilots-corner/airliner-flying-guide/altitude-refs/
        private string BaroHpa        => $"hPa Value {EfisType}";
        private string BaroInHg       => $"inHg Value {EfisType}";
        private string BaroInHgOnOff  => $"inHg Mode On/Off {EfisType}";
        private string BaroStd        => $"STD Mode On/Off {EfisType}";
        private string Qfe            => $"QFE Mode On/Off {EfisType}";

        public WinwingEfisDevice(IWinwingMessageSender sender, string efisType)
            : base(sender, ResolveDestination(efisType), 0x1a)
        {
            EfisType = efisType;

            DisplayTestCommands = new Dictionary<string, DisplaySegment>()
            {
                { "AllOn",   new DisplaySegment(new Bit[] { new Bit(0,0,true),  new Bit(0,1,true),  new Bit(0,2,false), new Bit(0,5,true) }, false) },
                { "AllOff",  new DisplaySegment(new Bit[] { new Bit(0,0,false), new Bit(0,1,false), new Bit(0,2,true),  new Bit(0,5,true) }, false) },
                { "Half1On", new DisplaySegment(new Bit[] { new Bit(0,0,true),  new Bit(0,1,false), new Bit(0,2,true),  new Bit(0,5,true) }, false) },
                { "Half2On", new DisplaySegment(new Bit[] { new Bit(0,0,false), new Bit(0,1,true),  new Bit(0,2,true),  new Bit(0,5,true) }, false) },
            };

            // Bit positions are byte-numbers in the data section (header offset 17 is added when writing).
            // Iteration order matters: overlapping mode-presets resolve last-writer-wins, so the entry that
            // should be active at init must come last within its bit-group.
            DisplaySetValueSegments = new Dictionary<string, DisplaySegment>
            {
                { "BaroThousands",  BaroDigit(4, '1') },
                { "BaroHundreds",   BaroDigit(5, '0') },
                { "BaroTens",       BaroDigit(6, '1') },
                { "BaroOnes",       BaroDigit(7, '3') },
                { "InHgDecPoint",   new DisplaySegment(new Bit(5, 7, false)) },
                { "NoBaroMode",     new DisplaySegment(new Bit[] { new Bit(8, 0, false), new Bit(8, 1, false) }, false) },
                { "QfeMode",        new DisplaySegment(new Bit[] { new Bit(8, 0, true),  new Bit(8, 1, false) }, false) },
                { "QnhMode",        new DisplaySegment(new Bit[] { new Bit(8, 0, false), new Bit(8, 1, true)  }, false) },
            };

            LedIdentifiers.Add($"FD {EfisType}",   0x03);
            LedIdentifiers.Add($"LS {EfisType}",   0x04);
            LedIdentifiers.Add($"CSTR {EfisType}", 0x05);
            LedIdentifiers.Add($"WPT {EfisType}",  0x06);
            LedIdentifiers.Add($"VORD {EfisType}", 0x07);
            LedIdentifiers.Add($"NDB {EfisType}",  0x08);
            LedIdentifiers.Add($"ARPT {EfisType}", 0x09);

            DisplayNameToActionMapping.Add(BaroHpa,         SetBaroHpa);
            DisplayNameToActionMapping.Add(BaroInHg,        SetBaroInHg);
            DisplayNameToActionMapping.Add(BaroInHgOnOff,   SetBaroInHgOnOff);
            DisplayNameToActionMapping.Add(BaroStd,         SetBaroStdOnOff);
            DisplayNameToActionMapping.Add(Qfe,             SetQfeOnOff);
            DisplayNameToActionMapping.Add(ANN_LIGHT,       SetAnnunciatorLightOnOffOverride);
            DisplayNameToActionMapping.Add(BACK_BRIGHTNESS, BrightnessFromString(0x00));
            DisplayNameToActionMapping.Add(LCD_BRIGHTNESS,  BrightnessFromString(0x01));
            DisplayNameToActionMapping.Add(LED_BRIGHTNESS,  BrightnessFromString(0x11));

            InitializeCaches();
            PrepareCommands();
        }

        private static byte[] ResolveDestination(string efisType)
        {
            if (efisType == WinwingConstants.EFISL_NAME) return WinwingConstants.DEST_EFISL;
            if (efisType == WinwingConstants.EFISR_NAME) return WinwingConstants.DEST_EFISR;
            return WinwingConstants.DEST_EFISL;
        }

        // EFIS Baro digit: 7 segments packed into one byte (bits 0..6); bit 7 is the inHg decimal point.
        // Order in the Bit[] must match the segment order [T, TR, BR, B, BL, TL, M].
        //   T = bit 4, TR = bit 5, BR = bit 6, B = bit 3, BL = bit 2, TL = bit 0, M = bit 1
        private static DisplaySegment BaroDigit(int dataByte, char init)
        {
            var seg = new DisplaySegment(new Bit[] {
                new Bit(dataByte, 4), // T
                new Bit(dataByte, 5), // TR
                new Bit(dataByte, 6), // BR
                new Bit(dataByte, 3), // B
                new Bit(dataByte, 2), // BL
                new Bit(dataByte, 0), // TL
                new Bit(dataByte, 1), // M
            }, true);
            seg.SetCharacter(init);
            return seg;
        }

        public override void Connect()
        {
            SendValues();
            InvokeDisplayBrightness(BACK_BRIGHTNESS, 20);
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 100);
        }

        public override void Shutdown()
        {
            EmptyDisplay();
            InvokeDisplayBrightness(BACK_BRIGHTNESS, 0);
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 0);
            TurnOffAllLEDs();
        }

        private void EmptyDisplay()
        {
            // Zero the data section bytes (absolute 21..25 = data 4..8): 4 digits + mode flag.
            for (int i = HeaderOffset + 4; i <= HeaderOffset + 8; i++)
            {
                SetValuesCommand[i] = 0;
            }
            SendValues();
        }

        private void ResetBaroCache()
        {
            ClearLcdCache(BaroHpa, BaroInHg, Qfe);
        }

        private void SetBaroInternal(char[] baroChars, bool isInHg, bool isStd)
        {
            SetDigits(baroChars, "BaroThousands", "BaroHundreds", "BaroTens", "BaroOnes");

            var inHgDot = DisplaySetValueSegments["InHgDecPoint"];
            if (!isStd)
            {
                inHgDot.SetValue(isInHg);
                ApplySegment(inHgDot, SetValuesCommand);
            }
            else
            {
                inHgDot.SetValue(false);
                ApplySegment(inHgDot, SetValuesCommand);
                ApplySegment(DisplaySetValueSegments["NoBaroMode"], SetValuesCommand);
            }

            SendValues();
        }

        private void SetBaroHpa(string baro)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                int myBaro = AsInt(baro);
                char[] baroChars = myBaro.ToString("D4", CultureInfo.InvariantCulture).ToCharArray();
                SetBaroInternal(baroChars, false, false);
            }
        }

        private void SetBaroInHg(string baro)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                int myBaro = (int)(AsDouble(baro) * 100);
                char[] baroChars = myBaro.ToString("D4", CultureInfo.InvariantCulture).ToCharArray();
                SetBaroInternal(baroChars, true, false);
            }
        }

        private void SetBaroInHgOnOff(string inHg)
        {
            ResetBaroCache();
        }

        private void SetBaroStdOnOff(string baroStd)
        {
            if (AsBool(baroStd))
            {
                SetBaroInternal(new char[] { 'S', 't', 'd', '*' }, false, true);
            }
            ResetBaroCache();
            SendValues();
        }

        private void SetQfeOnOff(string qfe)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                var preset = AsBool(qfe) ? "QfeMode" : "QnhMode";
                ApplySegment(DisplaySetValueSegments[preset], SetValuesCommand);
                SendValues();
            }
        }

        // EFIS variant: "1" → AllOn test; otherwise re-emit the current SetValues frame.
        // Same shape as the base default; kept explicit to match the original method name in the file.
        private void SetAnnunciatorLightOnOffOverride(string annLight)
        {
            if (AsBool(annLight))
            {
                LcdTest("AllOn");
            }
            else
            {
                SendValues();
            }
        }
    }
}
