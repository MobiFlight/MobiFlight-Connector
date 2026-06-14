using System;
using System.Globalization;
using System.Collections.Generic;

namespace MobiFlightWwFcu
{
    internal class WinwingRmpDevice : SegmentDisplayDeviceBase
    {
        public override string Name => $"WinWing {RmpType}";

        protected override string SetValuesHeaderKey => "0201_RMP";

        private readonly string RmpType;

        private const string ACTIVE_MODE   = "Active Mode";
        private const string ACTIVE_VALUE  = "Active Value";
        private const string STANDBY_MODE  = "Standby Mode";
        private const string STANDBY_VALUE = "Standby Value";

        // Display modes (both displays support all of them).
        private const int MODE_EMPTY   = 0; // display stays blank
        private const int MODE_FREQ3   = 1; // XXX.XXX  (VHF/HF COM, VOR, ILS)
        private const int MODE_ADF     = 2; // XXXX.X  (ADF/NDB)
        private const int MODE_COURSE  = 3; // " C-DDD"
        private const int MODE_DATA    = 4; // "dAtA"

        private int    ActiveMode    = MODE_FREQ3;
        private int    StandbyMode   = MODE_FREQ3;
        private string ActiveRawValue;
        private string StandbyRawValue;

        public WinwingRmpDevice(IWinwingMessageSender sender, string rmpType)
            : base(sender, WinwingConstants.DEST_RMP, 0x91)
        {
            RmpType = rmpType;

            DisplayTestCommands = new Dictionary<string, DisplaySegment>()
            {
                { "AllOn",  new DisplaySegment(new Bit[] { new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false) },
                { "AllOff", new DisplaySegment(new Bit[] { new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false) },
            };

            // One digit cell = one data byte in standard 7-segment coding (confirmed via capture):
            // bit0=T, bit1=TR, bit2=BR, bit3=B, bit4=BL, bit5=TL, bit6=M; bit7 = decimal point
            // right of the digit. The 6th digit has no decimal point (5 dots per display).
            // ACTIVE (left) display: data bytes 10..15 left to right; STBY/CRS (right): bytes 4..9.
            // Init chars show "Mobi" right-aligned on ACTIVE until the first values arrive:
            // 'M' spans two cells as "{" + "}" (same convention as other devices), 'i' is drawn
            // as 'l'. STBY stays blank.
            DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
            {
                { "ActD0", RmpDigit(10, '*') },
                { "ActD1", RmpDigit(11, '{') },
                { "ActD2", RmpDigit(12, '}') },
                { "ActD3", RmpDigit(13, 'o') },
                { "ActD4", RmpDigit(14, 'b') },
                { "ActD5", RmpDigit(15, 'l') },
                { "StbD0", RmpDigit(4, '*') },
                { "StbD1", RmpDigit(5, '*') },
                { "StbD2", RmpDigit(6, '*') },
                { "StbD3", RmpDigit(7, '*') },
                { "StbD4", RmpDigit(8, '*') },
                { "StbD5", RmpDigit(9, '*') },
                { "ActDot0", new DisplaySegment(new Bit(10, 7)) },
                { "ActDot1", new DisplaySegment(new Bit(11, 7)) },
                { "ActDot2", new DisplaySegment(new Bit(12, 7)) },
                { "ActDot3", new DisplaySegment(new Bit(13, 7)) },
                { "ActDot4", new DisplaySegment(new Bit(14, 7)) },
                { "StbDot0", new DisplaySegment(new Bit(4, 7)) },
                { "StbDot1", new DisplaySegment(new Bit(5, 7)) },
                { "StbDot2", new DisplaySegment(new Bit(6, 7)) },
                { "StbDot3", new DisplaySegment(new Bit(7, 7)) },
                { "StbDot4", new DisplaySegment(new Bit(8, 7)) },
            };

            // 14 indicator LEDs, type bytes confirmed via capture.
            LedIdentifiers.Add("SEL",  0x03);
            LedIdentifiers.Add("VHF1", 0x04);
            LedIdentifiers.Add("VHF2", 0x05);
            LedIdentifiers.Add("VHF3", 0x06);
            LedIdentifiers.Add("LOAD", 0x07);
            LedIdentifiers.Add("HF1",  0x08);
            LedIdentifiers.Add("HF2",  0x09);
            LedIdentifiers.Add("AM",   0x0A);
            LedIdentifiers.Add("NAV",  0x0B);
            LedIdentifiers.Add("VOR",  0x0C);
            LedIdentifiers.Add("ILS",  0x0D);
            LedIdentifiers.Add("GLS",  0x0E);
            LedIdentifiers.Add("MLS",  0x0F);
            LedIdentifiers.Add("ADF",  0x10);

            DisplayNameToActionMapping.Add(ACTIVE_MODE,   SetActiveMode);
            DisplayNameToActionMapping.Add(ACTIVE_VALUE,  SetActiveValue);
            DisplayNameToActionMapping.Add(STANDBY_MODE,  SetStandbyMode);
            DisplayNameToActionMapping.Add(STANDBY_VALUE, SetStandbyValue);

            DisplayNameToActionMapping.Add(ANN_LIGHT,       SetAnnunciatorLightOnOff);
            DisplayNameToActionMapping.Add(BACK_BRIGHTNESS, BrightnessFromString(0x00));
            DisplayNameToActionMapping.Add(LCD_BRIGHTNESS,  BrightnessFromString(0x01));
            DisplayNameToActionMapping.Add(LED_BRIGHTNESS,  BrightnessFromString(0x02));

            InitializeCaches();
            PrepareCommands();
        }

        // 7-segment digit packed into a single byte. Bit-order in the constructor must match
        // the segment order [T, TR, BR, B, BL, TL, M] of WinwingConstants.CharacterDict.
        private static DisplaySegment RmpDigit(int dataByte, char init)
        {
            var seg = new DisplaySegment(new Bit[] {
                new Bit(dataByte, 0), // T
                new Bit(dataByte, 1), // TR
                new Bit(dataByte, 2), // BR
                new Bit(dataByte, 3), // B
                new Bit(dataByte, 4), // BL
                new Bit(dataByte, 5), // TL
                new Bit(dataByte, 6), // M
            }, true);
            seg.SetCharacter(init);
            return seg;
        }

        public override void Connect()
        {
            SendValues();
            InvokeDisplayBrightness(BACK_BRIGHTNESS, 50);
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 100);

            // Testing
            //SetDisplay("Active Value", "11");
            //SetDisplay("Standby Value", "11");
            //SetDisplay("Active Value", "111.22");
            //SetDisplay("Standby Value", "113.2345678");
            //SetDisplay("Standby Mode", "3");
            //SetDisplay("Active Mode", "2");
            //SetDisplay("Active Value", "111.8");
            //SetDisplay("Standby Value", "118.2");
        }

        public override void Shutdown()
        {
            LcdTest("AllOff");
            InvokeDisplayBrightness(BACK_BRIGHTNESS, 0);
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 0);
            TurnOffAllLEDs();
        }

        private void SetActiveMode(string mode)
        {
            ActiveMode = AsInt(mode);
            ActiveRawValue = null;
            ClearLcdCache(ACTIVE_VALUE);

            // No refresh with the stale value on a mode change: only DATA ("dAtA") and EMPTY
            // (blank) render immediately; numeric modes wait for the next value.
            if (ActiveMode == MODE_DATA || ActiveMode == MODE_EMPTY)
            {
                RenderActive();
            }
        }

        private void SetActiveValue(string value)
        {
            ActiveRawValue = value;
            RenderActive();
        }

        private void SetStandbyMode(string mode)
        {
            StandbyMode = AsInt(mode);
            StandbyRawValue = null;
            ClearLcdCache(STANDBY_VALUE);

            // Same as SetActiveMode: only DATA and EMPTY render immediately.
            if (StandbyMode == MODE_DATA || StandbyMode == MODE_EMPTY)
            {
                RenderStandby();
            }
        }

        private void SetStandbyValue(string value)
        {
            StandbyRawValue = value;
            RenderStandby();
        }

        private void RenderActive()
            => Render(ActiveMode, ActiveRawValue, "ActD", "ActDot");

        private void RenderStandby()
            => Render(StandbyMode, StandbyRawValue, "StbD", "StbDot");

        private void Render(int mode, string rawValue, string digitPrefix, string dotPrefix)
        {
            char[] cells = FormatForMode(mode, rawValue, out int dotIndex);

            SetDigits(cells, digitPrefix + "0", digitPrefix + "1", digitPrefix + "2",
                             digitPrefix + "3", digitPrefix + "4", digitPrefix + "5");

            // Only the first 5 cells have a decimal point.
            for (int i = 0; i < 5; i++)
            {
                SetSegmentBool(dotPrefix + i, i == dotIndex);
            }

            SendValues();
        }

        // Produces the 6 character cells ('*' = blank) for the given mode and sets dotIndex to the
        // cell after which the decimal point lights (-1 = none).
        private char[] FormatForMode(int mode, string rawValue, out int dotIndex)
        {
            char[] cells = new char[] { '*', '*', '*', '*', '*', '*' };
            dotIndex = -1;

            switch (mode)
            {
                case MODE_DATA:
                    // "dAtA" centered (existing glyphs, uppercase A).
                    cells[1] = 'd'; cells[2] = 'A'; cells[3] = 't'; cells[4] = 'A';
                    return cells;

                case MODE_EMPTY:
                    return cells;
            }

            // Remaining modes need a numeric value; blank display until one arrives.
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return cells;
            }

            switch (mode)
            {
                case MODE_FREQ3:
                {
                    // XXX.XXX : integer right-aligned in c0-c2, 3 decimals in c3-c5, dot after c2.
                    long scaled = (long)Math.Round(AsDouble(rawValue) * 1000.0, MidpointRounding.AwayFromZero);
                    int whole = (int)(scaled / 1000);
                    int frac  = (int)(scaled % 1000); // guaranteed 0..999

                    string w = RightDigits(whole, 3);
                    string f = frac.ToString("D3", CultureInfo.InvariantCulture);

                    cells[0] = w[0]; cells[1] = w[1]; cells[2] = w[2];
                    cells[3] = f[0]; cells[4] = f[1]; cells[5] = f[2];
                    dotIndex = 2;
                    break;
                }

                case MODE_ADF:
                {
                    // XXXX.X : integer right-aligned in c0-c3, 1 decimal in c4 (c5 blank), dot after c3.
                    long scaled = (long)Math.Round(AsDouble(rawValue) * 10.0, MidpointRounding.AwayFromZero);
                    int whole = (int)(scaled / 10);
                    int frac  = (int)(scaled % 10); // guaranteed 0..9

                    string w = RightDigits(whole, 4);

                    cells[0] = w[0]; cells[1] = w[1]; cells[2] = w[2]; cells[3] = w[3];
                    cells[4] = (char)('0' + frac); // single digit (0..9) to its char, no string alloc
                    dotIndex = 3;
                    break;
                }

                case MODE_COURSE:
                {
                    // " C-DDD" : blank, 'C', '-', then 3-digit course (space sits before the C).
                    int deg = ((AsInt(rawValue) % 360) + 360) % 360;
                    string d = deg.ToString("D3", CultureInfo.InvariantCulture);
                    cells[1] = 'C'; cells[2] = '-';
                    cells[3] = d[0]; cells[4] = d[1]; cells[5] = d[2];
                    break;
                }
            }

            return cells;
        }

        // Returns the absolute integer as a string of exactly 'width' chars, blank-padded ('*') on
        // the left; if it has more digits than 'width', the rightmost 'width' digits are kept.
        private static string RightDigits(int value, int width)
        {
            string s = Math.Abs(value).ToString(CultureInfo.InvariantCulture);
            if (s.Length > width) s = s.Substring(s.Length - width);
            return s.PadLeft(width, '*');
        }
    }
}
