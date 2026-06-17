using System.Collections.Generic;
using System.Globalization;

namespace MobiFlightWwFcu
{
    internal class WinwingAgpDevice : SegmentDisplayDeviceBase
    {
        public override string Name => "WinWing AGP";

        protected override string SetValuesHeaderKey => "0201_AGP";

        private const string CHR_MIN = "CHR MIN Value";
        private const string CHR_SEC = "CHR SEC Value";
        private const string CHR_COLON_SHOWN = "CHR Colon Shown On/Off";
        private const string CHR_SHOWN = "CHR Shown On/Off";

        private const string ET_HR = "ET HR Value";
        private const string ET_MIN = "ET MIN Value";
        private const string ET_COLON_SHOWN = "ET Colon Shown On/Off";
        private const string ET_SHOWN = "ET Shown On/Off";

        private const string UTC_HR = "UTC HR/MO Value";
        private const string UTC_MIN = "UTC MIN/DY Value";
        private const string UTC_SEC = "UTC SEC/Y Value";
        private const string UTC_COLON_L_SHOWN = "UTC Colon Left Shown On/Off";
        private const string UTC_COLON_R_SHOWN = "UTC Colon Right Shown On/Off";
        private const string UTC_SHOWN = "UTC Shown On/Off";

        private const string UTC_HR_SHOWN = "UTC HR/MO Shown On/Off";
        private const string UTC_MIN_SHOWN = "UTC MIN/DY Shown On/Off";
        private const string UTC_SEC_SHOWN = "UTC SEC/Y Shown On/Off";

        private bool IsChrShown = true;
        private bool IsChrColonShown = true;
        private bool IsUtcShown = true;
        private bool IsUtcColonLShown = true;
        private bool IsUtcColonRShown = true;
        private bool IsEtShown = true;
        private bool IsEtColonShown = true;
        private bool IsUtcHrShown = true;
        private bool IsUtcMinShown = true;
        private bool IsUtcSecShown = true;

        public WinwingAgpDevice(IWinwingMessageSender sender)
            : base(sender, WinwingConstants.DEST_AGP, 0x35)  // 53 bytes: 4 dest addr + 13 header + 36 data
        {
            DisplayTestCommands = new Dictionary<string, DisplaySegment>()
            {
                { "AllOn",  new DisplaySegment(new Bit[] { new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false) },
                { "AllOff", new DisplaySegment(new Bit[] { new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false) },
            };

            DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
            {
                { "ChrMinTens",     new DisplaySegment(28, 0, isReverse: false) },
                { "ChrMinOnes",     new DisplaySegment(28, 1, isReverse: false) },
                { "ChrSecTens",     new DisplaySegment(28, 2, isReverse: false) },
                { "ChrSecOnes",     new DisplaySegment(28, 3, isReverse: false) },
                { "ChrColon",       new DisplaySegment(new Bit[] { new Bit(32,2), new Bit(32,3) }, isSevenSegment: false) },
                { "UtcHrTens",      new DisplaySegment(28, 4, isReverse: false) },
                { "UtcHrOnes",      new DisplaySegment(28, 5, isReverse: false) },
                { "UtcMinTens",     new DisplaySegment(28, 6, isReverse: false) },
                { "UtcMinOnes",     new DisplaySegment(28, 7, isReverse: false) },
                { "UtcSecTens",     new DisplaySegment(29, 0, isReverse: false) },
                { "UtcSecOnes",     new DisplaySegment(29, 1, isReverse: false) },
                { "UtcLeftColon",   new DisplaySegment(new Bit[] { new Bit(32,5), new Bit(32,6) }, isSevenSegment: false) },
                { "UtcRightColon",  new DisplaySegment(new Bit[] { new Bit(32,7), new Bit(33,0) }, isSevenSegment: false) },
                { "EtHrTens",       new DisplaySegment(29, 2, isReverse: false) },
                { "EtHrOnes",       new DisplaySegment(29, 3, isReverse: false) },
                { "EtMinTens",      new DisplaySegment(29, 4, isReverse: false) },
                { "EtMinOnes",      new DisplaySegment(29, 5, isReverse: false) },
                { "EtColon",        new DisplaySegment(new Bit[] { new Bit(33,4), new Bit(33,5) }, isSevenSegment: false) },
            };

            LedIdentifiers.Add("GEAR_1_UNLOCKED",     0x03);
            LedIdentifiers.Add("GEAR_2_UNLOCKED",     0x04);
            LedIdentifiers.Add("GEAR_3_UNLOCKED",     0x05);
            LedIdentifiers.Add("GEAR_1_LOCKED",       0x07);
            LedIdentifiers.Add("GEAR_2_LOCKED",       0x08);
            LedIdentifiers.Add("GEAR_3_LOCKED",       0x09);
            LedIdentifiers.Add("BRK_FAN_HOT",         0x06);
            LedIdentifiers.Add("BRK_FAN_ON",          0x0a);
            LedIdentifiers.Add("AUTO_BRK_LO_DECEL",   0x0b);
            LedIdentifiers.Add("AUTO_BRK_MED_DECEL",  0x0c);
            LedIdentifiers.Add("AUTO_BRK_MAX_DECEL",  0x0d);
            LedIdentifiers.Add("AUTO_BRK_LO_ON",      0x0e);
            LedIdentifiers.Add("AUTO_BRK_MED_ON",     0x0f);
            LedIdentifiers.Add("AUTO_BRK_MAX_ON",     0x10);
            LedIdentifiers.Add("TERR_ON_ND_ON",       0x11);
            LedIdentifiers.Add("GEAR_DOWN_RED_ARROW", 0x12);

            DisplayNameToActionMapping.Add(CHR_MIN,           SetChrMin);
            DisplayNameToActionMapping.Add(CHR_SEC,           SetChrSec);
            DisplayNameToActionMapping.Add(CHR_COLON_SHOWN,   SetChrColonShown);
            DisplayNameToActionMapping.Add(CHR_SHOWN,         SetChrShown);

            DisplayNameToActionMapping.Add(ET_HR,             SetEtHr);
            DisplayNameToActionMapping.Add(ET_MIN,            SetEtMin);
            DisplayNameToActionMapping.Add(ET_COLON_SHOWN,    SetEtColonShown);
            DisplayNameToActionMapping.Add(ET_SHOWN,          SetEtShown);

            DisplayNameToActionMapping.Add(UTC_HR,            SetUtcHr);
            DisplayNameToActionMapping.Add(UTC_MIN,           SetUtcMin);
            DisplayNameToActionMapping.Add(UTC_SEC,           SetUtcSec);
            DisplayNameToActionMapping.Add(UTC_COLON_L_SHOWN, SetUtcColonLShown);
            DisplayNameToActionMapping.Add(UTC_COLON_R_SHOWN, SetUtcColonRShown);
            DisplayNameToActionMapping.Add(UTC_SHOWN,         SetUtcShown);
            DisplayNameToActionMapping.Add(UTC_HR_SHOWN,      SetUtcHrShown);
            DisplayNameToActionMapping.Add(UTC_MIN_SHOWN,     SetUtcMinShown);
            DisplayNameToActionMapping.Add(UTC_SEC_SHOWN,     SetUtcSecShown);
            DisplayNameToActionMapping.Add(ANN_LIGHT,         SetAnnunciatorLightOnOff);

            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));
            OutputNameToActionMapping.Add(LED_BRIGHTNESS,  Brightness(0x02));
            OutputNameToActionMapping.Add(LCD_BRIGHTNESS,  Brightness(0x01));

            InitializeCaches();
            PrepareCommands();
        }

        public override void Connect()
        {
            SendValues();
            SetDisplay(UTC_HR, "0");
            SetDisplay(UTC_MIN, "0");
            SetDisplay(UTC_SEC, "0");
            InvokeOutputBrightness(BACK_BRIGHTNESS, 50);
            InvokeOutputBrightness(LCD_BRIGHTNESS, 100);
        }

        public override void Shutdown()
        {
            LcdTest("AllOff");
            InvokeOutputBrightness(BACK_BRIGHTNESS, 0);
            InvokeOutputBrightness(LCD_BRIGHTNESS, 0);
            TurnOffAllLEDs();
        }

        private void SetBoolInternal(bool value, bool isShown, string segmentName)
        {
            bool boolValue = isShown && value;
            SetSegmentBool(segmentName, boolValue);
            SendValues();
        }

        private void SetDoubleDigitInternal(string value, bool isShown, params string[] segmentNames)
        {
            char[] chars;
            if (isShown)
            {
                int valueInt = AsInt(value);
                chars = valueInt.ToString("D2", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                chars = new char[] { '*', '*' };
            }
            SetDigits(chars, segmentNames);
            SendValues();
        }

        private void SetChrMin(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsChrShown, "ChrMinTens", "ChrMinOnes");
            }
        }

        private void SetChrSec(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsChrShown, "ChrSecTens", "ChrSecOnes");
            }
        }

        private void SetChrShown(string isShown)
        {
            IsChrShown = AsBool(isShown);
            SetChrMin(LcdCurrentValuesCache[CHR_MIN]);
            SetChrSec(LcdCurrentValuesCache[CHR_SEC]);
            SetChrColonShown(LcdCurrentValuesCache[CHR_COLON_SHOWN]);
        }

        private void SetChrColonShown(string isShown)
        {
            if (!string.IsNullOrWhiteSpace(isShown))
            {
                IsChrColonShown = AsBool(isShown);
                SetBoolInternal(IsChrColonShown, IsChrShown, "ChrColon");
            }
        }

        private void SetUtcHr(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                bool isShown = IsUtcShown && IsUtcHrShown;
                SetDoubleDigitInternal(value, isShown, "UtcHrTens", "UtcHrOnes");
            }
        }

        private void SetUtcMin(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                bool isShown = IsUtcShown && IsUtcMinShown;
                SetDoubleDigitInternal(value, isShown, "UtcMinTens", "UtcMinOnes");
            }
        }

        private void SetUtcSec(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                bool isShown = IsUtcShown && IsUtcSecShown;
                SetDoubleDigitInternal(value, isShown, "UtcSecTens", "UtcSecOnes");
            }
        }

        private void SetUtcShown(string isShown)
        {
            IsUtcShown = AsBool(isShown);
            SetUtcHr(LcdCurrentValuesCache[UTC_HR]);
            SetUtcMin(LcdCurrentValuesCache[UTC_MIN]);
            SetUtcSec(LcdCurrentValuesCache[UTC_SEC]);
            SetUtcColonLShown(LcdCurrentValuesCache[UTC_COLON_L_SHOWN]);
            SetUtcColonRShown(LcdCurrentValuesCache[UTC_COLON_R_SHOWN]);
        }

        private void SetUtcColonLShown(string isShown)
        {
            if (!string.IsNullOrWhiteSpace(isShown))
            {
                IsUtcColonLShown = AsBool(isShown);
                SetBoolInternal(IsUtcColonLShown, IsUtcShown, "UtcLeftColon");
            }
        }

        private void SetUtcColonRShown(string isShown)
        {
            if (!string.IsNullOrWhiteSpace(isShown))
            {
                IsUtcColonRShown = AsBool(isShown);
                SetBoolInternal(IsUtcColonRShown, IsUtcShown, "UtcRightColon");
            }
        }

        private void SetUtcHrShown(string isShown)
        {
            IsUtcHrShown = AsBool(isShown);
            SetUtcHr(LcdCurrentValuesCache[UTC_HR]);
        }

        private void SetUtcMinShown(string isShown)
        {
            IsUtcMinShown = AsBool(isShown);
            SetUtcMin(LcdCurrentValuesCache[UTC_MIN]);
        }

        private void SetUtcSecShown(string isShown)
        {
            IsUtcSecShown = AsBool(isShown);
            SetUtcSec(LcdCurrentValuesCache[UTC_SEC]);
        }

        private void SetEtHr(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsEtShown, "EtHrTens", "EtHrOnes");
            }
        }

        private void SetEtMin(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsEtShown, "EtMinTens", "EtMinOnes");
            }
        }

        private void SetEtShown(string isShown)
        {
            IsEtShown = AsBool(isShown);
            SetEtHr(LcdCurrentValuesCache[ET_HR]);
            SetEtMin(LcdCurrentValuesCache[ET_MIN]);
            SetEtColonShown(LcdCurrentValuesCache[ET_COLON_SHOWN]);
        }

        private void SetEtColonShown(string isShown)
        {
            if (!string.IsNullOrWhiteSpace(isShown))
            {
                IsEtColonShown = AsBool(isShown);
                SetBoolInternal(IsEtColonShown, IsEtShown, "EtColon");
            }
        }

        // AGP override: "off" branch sends LcdTest("AllOff") instead of resending current SetValues.
        // Wait - actually base's default behaviour matches AGP's original (off → SendValues). Keep default.
    }
}
