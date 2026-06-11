using System;
using System.Collections.Generic;
using System.Globalization;

namespace MobiFlightWwFcu
{
    internal class WinwingAirbusThrottleDevice : SegmentDisplayDeviceBase
    {
        public override string Name => $"WinWing {ThrottleType}";

        protected override string SetValuesHeaderKey => "0201_PAC";
        protected override byte[] HeaderDestinationAddress => DestinationAddressPac;

        private readonly string ThrottleType;
        private readonly byte[] DestinationAddressPac = WinwingConstants.DEST_AIRBUS_PAC;

        private const string VIBRATION_1 = "Vibration 1 Percentage";
        private const string VIBRATION_2 = "Vibration 2 Percentage";
        private const string TRIM_DASHES = "Trim Dashes On/Off";
        private const string TRIM        = "Trim Value";  // Negative and 0 → L, Positive → R

        private bool IsTrimDashed = false;

        public WinwingAirbusThrottleDevice(IWinwingMessageSender sender, string throttleType)
            : base(sender, WinwingConstants.DEST_AIRBUS_THROTTLE, 0x35)  // 53 bytes: 4 dest addr + 13 header + 36 data
        {
            ThrottleType = throttleType;

            DisplayTestCommands = new Dictionary<string, DisplaySegment>()
            {
                { "AllOn",  new DisplaySegment(new Bit[] { new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false) },
                { "AllOff", new DisplaySegment(new Bit[] { new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false) },
            };

            // Examples of Trim "L 0.0", "L 0.2", "R 0.0", "L 5.1" "L11.3"
            // In Error case, both FAC lost: " ---"
            // Center console rudder trim display.  Negative = Left, Positive = Right.
            // On the A320, the rudder trim indication becomes dashed when the FACs are not supplying valid data.
            DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
            {
                { "TrimDecimal", new DisplaySegment(32, 3, 'b') },
                { "TrimOnes",    new DisplaySegment(32, 2, 'o') },  // 3 = ones, 2 = tenth, 1 = hundreds, 0 = L/R
                { "TrimTens",    new DisplaySegment(32, 1, '}') },
                { "TrimLR",      new DisplaySegment(32, 0, '{') },
                { "TrimDot",     new DisplaySegment(new Bit[] { new Bit(4,2, false) }, false) },
            };

            LedIdentifiers.Add("FAULT_1", 0x03);
            LedIdentifiers.Add("FIRE_1",  0x04);
            LedIdentifiers.Add("FAULT_2", 0x05);
            LedIdentifiers.Add("FIRE_2",  0x06);

            DisplayNameToActionMapping.Add(TRIM,         SetTrim);
            DisplayNameToActionMapping.Add(TRIM_DASHES,  SetTrimDashed);
            DisplayNameToActionMapping.Add(ANN_LIGHT,    SetAnnunciatorLightOnOff);

            OutputNameToActionMapping.Add(VIBRATION_1,     SetVibration1);
            OutputNameToActionMapping.Add(VIBRATION_2,     SetVibration2);
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            OutputNameToActionMapping.Add(LED_BRIGHTNESS,  Brightness(0x02));                        // throttle, type 0x02
            OutputNameToActionMapping.Add(LCD_BRIGHTNESS,  state => MessageSender.SetBrightness(DestinationAddressPac, 0x02, state)); // PAC, type 0x02

            InitializeCaches();
            PrepareCommands();
        }

        public override void Connect()
        {
            SendValues();
            InvokeOutputBrightness(BACK_BRIGHTNESS, 20);
            InvokeOutputBrightness(LCD_BRIGHTNESS, 100);
            SetVibration1(0);
            SetVibration2(0);
        }

        public override void Shutdown()
        {
            LcdTest("AllOff");
            InvokeOutputBrightness(BACK_BRIGHTNESS, 0);
            InvokeOutputBrightness(LCD_BRIGHTNESS, 0);
            SetVibration1(0);
            SetVibration2(0);
            TurnOffAllLEDs();
        }

        public override void Stop()
        {
            TurnOffAllLEDs();
            SetVibration1(0);
            SetVibration2(0);
        }

        private void SetVibration1(byte level)
        {
            MessageSender.SetVibration(DestinationAddress, 0x0e, level);
        }

        private void SetVibration2(byte level)
        {
            MessageSender.SetVibration(DestinationAddress, 0x10, level);
        }

        private void SetBacklightBrightness(byte brightness)
        {
            // Backlight is mirrored on both the throttle and the PAC.
            MessageSender.SetBrightness(DestinationAddress,    0x00, brightness);
            MessageSender.SetBrightness(DestinationAddressPac, 0x00, brightness);
        }

        private void SetTrim(string trim)
        {
            int value = (int)(AsDouble(trim) * 10);
            char[] chars = new char[] { '*', '*', '*', '*' };

            if (IsTrimDashed)
            {
                chars = new char[] { '*', '-', '-', '-' };
                SetSegmentBool("TrimDot", false);
            }
            else
            {
                chars[0] = value <= 0 ? 'L' : 'A';

                // D2 specifies the minimum number of digits to display. If there are fewer than 2 digits,
                // it will be left-padded with zeros: 0 → 00, 3 → 03, 10 → 10.
                string valueString = Math.Abs(value).ToString("D2", CultureInfo.InvariantCulture).PadLeft(3, '*');
                for (int i = 0; i < 3; i++)
                {
                    chars[i + 1] = valueString[i];
                }
                SetSegmentBool("TrimDot", true);
            }
            SetDigits(chars, "TrimLR", "TrimTens", "TrimOnes", "TrimDecimal");
            SendValues();
        }

        private void SetTrimDashed(string isDashed)
        {
            IsTrimDashed = AsBool(isDashed);

            if (!string.IsNullOrEmpty(LcdCurrentValuesCache[TRIM]))
            {
                SetTrim(LcdCurrentValuesCache[TRIM]);
                ClearLcdCache(TRIM);
            }
        }
    }
}
