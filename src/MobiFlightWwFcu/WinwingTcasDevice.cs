using System;
using System.Collections.Generic;
using System.Globalization;

namespace MobiFlightWwFcu
{
    internal class WinwingTcasDevice : SegmentDisplayDeviceBase
    {
        public override string Name => "WinWing TCAS";

        private const string IDENT_NUMBER = "Ident Value";
        private const string NUMBER_DIGITS = "Number Of Digits";

        protected override string SetValuesHeaderKey => "0201_TCAS";

        // 7-segment digit packed within a single TCAS "ident" byte (Bits 7..1).
        // Bit-order in the constructor must match the segment order [T, TR, BR, B, BL, TL, M].
        private static DisplaySegment IdentDigit(int bit, char initChar)
        {
            var seg = new DisplaySegment(new Bit[] {
                new Bit(16, bit), // T
                new Bit(12, bit), // TR
                new Bit(8,  bit), // BR
                new Bit(4,  bit), // B
                new Bit(28, bit), // BL
                new Bit(24, bit), // TL
                new Bit(20, bit), // M
            }, true);
            seg.SetCharacter(initChar);
            return seg;
        }

        private int NumberOfCharsShown = 4;

        public WinwingTcasDevice(IWinwingMessageSender sender)
            : base(sender, WinwingConstants.DEST_TCAS, 0x35)  // 53 bytes: 4 dest addr + 13 header + 36 data
        {
            DisplayTestCommands = new Dictionary<string, DisplaySegment>()
            {
                { "AllOn",  new DisplaySegment(new Bit[] { new Bit(0, 0, true), new Bit(0, 1), new Bit(0, 2), new Bit(0, 3) }, false) },
                { "AllOff", new DisplaySegment(new Bit[] { new Bit(0, 0), new Bit(0, 1, true), new Bit(0, 2), new Bit(0, 3) }, false) },
            };

            DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
            {
                { "IdentThousands", IdentDigit(0, '{') },
                { "IdentHundreds",  IdentDigit(1, '}') },
                { "IdentTens",      IdentDigit(2, 'o') },
                { "IdentOnes",      IdentDigit(3, 'b') },
            };

            LedIdentifiers.Add("ATC_FAIL", 0x03);

            DisplayNameToActionMapping.Add(NUMBER_DIGITS, SetNumberOfCharsShown);
            DisplayNameToActionMapping.Add(IDENT_NUMBER,  SetIdentNumber);
            DisplayNameToActionMapping.Add(ANN_LIGHT,     SetAnnunciatorLightOnOff);

            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, Brightness(0x00));
            OutputNameToActionMapping.Add(LED_BRIGHTNESS,  Brightness(0x02));
            OutputNameToActionMapping.Add(LCD_BRIGHTNESS,  Brightness(0x01));

            InitializeCaches();
            PrepareCommands();
        }

        public override void Connect()
        {
            SendValues();
            InvokeOutputBrightness(BACK_BRIGHTNESS, 50);
            InvokeOutputBrightness(LCD_BRIGHTNESS, 100);

            // Testing
            // SetDisplay(IDENT_NUMBER, "11");
            // SetDisplay(IDENT_NUMBER, "7620");
            // SetDisplay(IDENT_NUMBER, "123456789");
            // SetDisplay(CHR_SEC, "22");
            // SetDisplay(ANN_LIGHT, "1");
            // SetLed("ATC_FAIL", 0);
        }

        public override void Shutdown()
        {
            LcdTest("AllOff");
            InvokeOutputBrightness(BACK_BRIGHTNESS, 0);
            InvokeOutputBrightness(LCD_BRIGHTNESS, 0);
            TurnOffAllLEDs();
        }

        private void SetIdentNumber(string number)
        {
            int value = AsInt(number);
            string myNumber = value.ToString(CultureInfo.InvariantCulture);

            string shortened = (myNumber.Length > NumberOfCharsShown ? myNumber.Substring(0, NumberOfCharsShown) : myNumber);
            char[] chars = shortened.PadLeft(NumberOfCharsShown, '0').PadRight(4, '*').ToCharArray();

            SetDigits(chars, "IdentThousands", "IdentHundreds", "IdentTens", "IdentOnes");
            SendValues();
        }

        private void SetNumberOfCharsShown(string number)
        {
            int value = AsInt(number);
            int clampedValue = Math.Min(4, Math.Max(0, value));
            NumberOfCharsShown = clampedValue;
            ClearLcdCache(IDENT_NUMBER);
        }
    }
}
