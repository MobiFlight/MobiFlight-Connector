using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace MobiFlightWwFcu
{
    internal class WinwingAgpDevice : IWinwingDevice
    {
        public string Name { get; } = "WinWing AGP";

        private IWinwingMessageSender MessageSender = null;
        private byte[] DestinationAddress = WinwingConstants.DEST_AGP;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<byte>> OutputNameToActionMapping = new Dictionary<string, Action<byte>>();

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

        private const string ANN_LIGHT = "LCD Test On/Off";
        private const string BACK_BRIGHTNESS = "Backlight Percentage";     
        private const string LED_BRIGHTNESS = "LED Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";

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


        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {
            { "GEAR_1_UNLOCKED",    0x03 },
            { "GEAR_2_UNLOCKED",    0x04 },
            { "GEAR_3_UNLOCKED",    0x05 },            
            { "GEAR_1_LOCKED",      0x07 },
            { "GEAR_2_LOCKED",      0x08 },
            { "GEAR_3_LOCKED",      0x09 },
            { "BRK_FAN_HOT",        0x06 },
            { "BRK_FAN_ON",         0x0a },
            { "AUTO_BRK_LO_DECEL",  0x0b },
            { "AUTO_BRK_MED_DECEL", 0x0c },
            { "AUTO_BRK_MAX_DECEL", 0x0d },
            { "AUTO_BRK_LO_ON",     0x0e },
            { "AUTO_BRK_MED_ON",    0x0f },
            { "AUTO_BRK_MAX_ON",    0x10 },
            { "TERR_ON_ND_ON",      0x11 },
            { "GEAR_DOWN_RED_ARROW",0x12 },
        };

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();

        private byte[] DisplayTestCommand = new byte[0x12];
        private byte[] RefreshCommand = new byte[0x11];
        private byte[] SetValuesCommand = new byte[0x35];  // 35 equals 53, max of a content message 4 + 13 + 36 data

        private Dictionary<string, DisplaySegment> DisplayTestCommands = new Dictionary<string, DisplaySegment>()
        {
            { "AllOn",       new DisplaySegment(new Bit[] {new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false)}, // 01 for On
            { "AllOff",      new DisplaySegment(new Bit[] {new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false)}, // 02 for Off
        };

        // Element top byte is byte number in data section. So 0 is start of data section. Header with 17 bytes is not included.
        private Dictionary<string, DisplaySegment> DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
        {                                              
            { "ChrMinTens",   new DisplaySegment(28, 0, isReverse: false)},
            { "ChrMinOnes",   new DisplaySegment(28, 1, isReverse: false)},
            { "ChrSecTens",   new DisplaySegment(28, 2, isReverse: false)},
            { "ChrSecOnes",   new DisplaySegment(28, 3, isReverse: false)},
            { "ChrColon",  new DisplaySegment(new Bit[] { new Bit(32,2), new Bit(32,3) }, isSevenSegment: false)},
            { "UtcHrTens",    new DisplaySegment(28, 4, isReverse: false)},
            { "UtcHrOnes",    new DisplaySegment(28, 5, isReverse: false)},
            { "UtcMinTens",   new DisplaySegment(28, 6, isReverse: false)},
            { "UtcMinOnes",   new DisplaySegment(28, 7, isReverse: false)},
            { "UtcSecTens",   new DisplaySegment(29, 0, isReverse: false)},
            { "UtcSecOnes",   new DisplaySegment(29, 1, isReverse: false)},
            { "UtcLeftColon",   new DisplaySegment(new Bit[] { new Bit(32,5), new Bit(32,6) }, isSevenSegment: false)},
            { "UtcRightColon",  new DisplaySegment(new Bit[] { new Bit(32,7), new Bit(33,0) }, isSevenSegment: false)},
            { "EtHrTens",     new DisplaySegment(29, 2, isReverse: false)},
            { "EtHrOnes",     new DisplaySegment(29, 3, isReverse: false)},
            { "EtMinTens",    new DisplaySegment(29, 4, isReverse: false)},
            { "EtMinOnes",    new DisplaySegment(29, 5, isReverse: false)},
            { "EtColon",    new DisplaySegment(new Bit[] { new Bit(33,4), new Bit(33,5) }, isSevenSegment: false)},
        };

        public WinwingAgpDevice(IWinwingMessageSender sender)
        {
            MessageSender = sender;

            // Add display options
            DisplayNameToActionMapping.Add(CHR_MIN, SetChrMin);
            DisplayNameToActionMapping.Add(CHR_SEC, SetChrSec);
            DisplayNameToActionMapping.Add(CHR_COLON_SHOWN, SetChrColonShown);
            DisplayNameToActionMapping.Add(CHR_SHOWN, SetChrShown);

            DisplayNameToActionMapping.Add(ET_HR, SetEtHr);
            DisplayNameToActionMapping.Add(ET_MIN, SetEtMin);
            DisplayNameToActionMapping.Add(ET_COLON_SHOWN, SetEtColonShown);
            DisplayNameToActionMapping.Add(ET_SHOWN, SetEtShown);

            DisplayNameToActionMapping.Add(UTC_HR, SetUtcHr);
            DisplayNameToActionMapping.Add(UTC_MIN, SetUtcMin);
            DisplayNameToActionMapping.Add(UTC_SEC, SetUtcSec);
            DisplayNameToActionMapping.Add(UTC_COLON_L_SHOWN, SetUtcColonLShown);
            DisplayNameToActionMapping.Add(UTC_COLON_R_SHOWN, SetUtcColonRShown);
            DisplayNameToActionMapping.Add(UTC_SHOWN, SetUtcShown);
            DisplayNameToActionMapping.Add(UTC_HR_SHOWN, SetUtcHrShown);
            DisplayNameToActionMapping.Add(UTC_MIN_SHOWN, SetUtcMinShown);
            DisplayNameToActionMapping.Add(UTC_SEC_SHOWN, SetUtcSecShown);
            DisplayNameToActionMapping.Add(ANN_LIGHT, SetAnnunciatorLightOnOff);

            // Add output options
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            OutputNameToActionMapping.Add(LED_BRIGHTNESS, SetLedBrightness);
            OutputNameToActionMapping.Add(LCD_BRIGHTNESS, SetLcdBrightness);

            foreach (var displayName in GetDisplayNames())
            {
                LcdCurrentValuesCache.Add(displayName, string.Empty);
            }

            foreach (var ledName in GetLedNames())
            {
                LedCurrentValuesCache.Add(ledName, 255);
            }

            PrepareCommands();
        }

        private void PrepareCommands()
        {
            var initDisplayTest = new List<byte>(DestinationAddress);
            initDisplayTest.AddRange(new byte[2]);
            initDisplayTest.AddRange(WinwingConstants.DisplayCmdHeaders["0401"]);
            initDisplayTest.CopyTo(DisplayTestCommand, 0);

            // 4 + 13
            var initSetValues = new List<byte>(DestinationAddress);
            initSetValues.AddRange(new byte[2]);
            initSetValues.AddRange(WinwingConstants.DisplayCmdHeaders["0201_AGP"]);
            initSetValues.CopyTo(SetValuesCommand, 0);

            var initRefresh = new List<byte>(DestinationAddress);
            initRefresh.AddRange(new byte[2]);
            initRefresh.AddRange(WinwingConstants.DisplayCmdHeaders["0301"]);
            initRefresh.CopyTo(RefreshCommand, 0);

            foreach (var segment in DisplaySetValueSegments.Values)
            {
                SetSegmentDisplayCommand(segment, SetValuesCommand);
            }
        }

        public void Connect()
        {
            SendDisplayCommand(SetValuesCommand);
            SetDisplay(UTC_HR, "0");
            SetDisplay(UTC_MIN, "0");
            SetDisplay(UTC_SEC, "0");
            SetBacklightBrightness(50);
            SetLcdBrightness(100);

            // Testing
            //SetDisplay(CHR_MIN, "11");
            //SetDisplay(CHR_SEC, "22");
            //SetDisplay(UTC_HR,  "33");
            //SetDisplay(UTC_MIN, "44");
            //SetDisplay(UTC_SEC, "55");
            //SetDisplay(ET_HR,   "66");
            //SetDisplay(ET_MIN,  "77");
            //SetDisplay(CHR_COLON_SHOWN, "0");
            //SetDisplay(ET_COLON_SHOWN, "0");
            //SetDisplay(UTC_COLON_L_SHOWN, "0");
            //Thread.Sleep(5000);
            //SetDisplay(CHR_COLON_SHOWN, "1");
            //SetDisplay(ET_COLON_SHOWN, "1");
            //SetDisplay(UTC_COLON_L_SHOWN, "1");
            //SetDisplay(UTC_HR_SHOWN, "0");
            //SetDisplay(UTC_SEC_SHOWN, "0");
            //Thread.Sleep(5000);
            //SetDisplay(CHR_SHOWN, "0");
            //SetDisplay(ET_SHOWN, "0");
            //SetDisplay(UTC_SHOWN, "0");
        }

        private void TurnOffAllLEDs()
        {
            foreach (var ledName in LedIdentifiers.Keys)
            {
                SetLed(ledName, 0);
            }
        }

        public void Shutdown()
        {
            EmptyDisplay();
            SetBacklightBrightness(0);
            SetLcdBrightness(0);
            TurnOffAllLEDs();        
        }

        public List<string> GetLedNames()
        {
            List<string> ledNames = new List<string>();
            ledNames.AddRange(LedIdentifiers.Keys.ToList());
            ledNames.AddRange(OutputNameToActionMapping.Keys.ToList()); 
            return ledNames;
        }

        public List<string> GetDisplayNames()
        {
            return DisplayNameToActionMapping.Keys.ToList();
        }

        public List<string> GetInternalDisplayNames()
        {
            return new List<string>();
        }

        public void SetLed(string led, byte state)
        {
            if (!string.IsNullOrEmpty(led) && LedCurrentValuesCache[led] != state)
            {             
                if (LedIdentifiers.TryGetValue(led, out byte ledType))
                {
                    LedCurrentValuesCache[led] = state;
                    byte stateAdjusted = state == 0 ? (byte)0 : (byte)1;
                    MessageSender.SendLightControlMessage(DestinationAddress, ledType, stateAdjusted);
                }
                else if (OutputNameToActionMapping.TryGetValue(led, out Action<byte> action)) 
                {
                    action(state);
                }
            }
        }

        public void SetDisplay(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && LcdCurrentValuesCache[name] != value) // check cache
            {
                LcdCurrentValuesCache[name] = value;
                DisplayNameToActionMapping[name](value); // Execute Action
            }
        }

        private void SetAnnunciatorLightOnOff(string annLight)
        {
            int myAnnLight = (int)Convert.ToDouble(annLight, CultureInfo.InvariantCulture);
            if (myAnnLight == 1)
            {
                LcdTest("AllOn");
            }
            else
            {
                SendDisplayCommand(SetValuesCommand);
            }
        }


        private void SetLedBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x02, brightness);
        }

        private void SetBacklightBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);
        }

        private void SetLcdBrightness(byte brightness)
        {            
            MessageSender.SetBrightness(DestinationAddress, 0x01, brightness);
        }
        
        public void Stop()
        {
            TurnOffAllLEDs();
        }


        private void SetBoolInternal(bool value, bool isShown, string segmentName)
        {
            bool boolValue;
            if (isShown)
            {
                boolValue = value;
            }
            else
            {
                boolValue = false;
            }
            var segment = DisplaySetValueSegments[segmentName];
            segment.SetValue(boolValue);
            SetSegmentDisplayCommand(segment, SetValuesCommand);
            SendDisplayCommand(SetValuesCommand);
        }

        private void PrepareAndSendDisplayTestCommand(DisplaySegment segment)
        {
            SetSegmentDisplayCommand(segment, DisplayTestCommand);
            SendDisplayCommand(DisplayTestCommand);
        }

        private void EmptyDisplay()
        {
            LcdTest("AllOff");
        }


        private void SetDigitsInternal(char[] chars, string[] segmentNames)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                var segment = DisplaySetValueSegments[segmentNames[i]];
                segment.SetCharacter(chars[i]);
                SetSegmentDisplayCommand(segment, SetValuesCommand);
            }

            SendDisplayCommand(SetValuesCommand);
        }

        private void SetDoubleDigitInternal(string value, bool isShown, string[] segmentNames)
        {
            char[] chars;

            if (isShown)
            {
                int valueInt = (int)Convert.ToDouble(value, CultureInfo.InvariantCulture);
                chars = valueInt.ToString("D2", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                chars = new char[] { '*', '*' };
            }
            SetDigitsInternal(chars, segmentNames);
        }


        private void SetChrMin(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsChrShown, new string[] { "ChrMinTens", "ChrMinOnes" });
            }
        }

        private void SetChrSec(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsChrShown, new string[] { "ChrSecTens", "ChrSecOnes" });
            }
        }

        private void SetChrShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsChrShown = Convert.ToBoolean(value);

            // Instead of resetting the cache, just call them again to force update
            SetChrMin(LcdCurrentValuesCache[CHR_MIN]);  
            SetChrSec(LcdCurrentValuesCache[CHR_SEC]);
            SetChrColonShown(LcdCurrentValuesCache[CHR_COLON_SHOWN]);             
        }

        private void SetChrColonShown(string isShown)
        {
            if (!string.IsNullOrWhiteSpace(isShown))
            {
                int intValue = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
                IsChrColonShown = Convert.ToBoolean(intValue);
                SetBoolInternal(IsChrColonShown, IsChrShown, "ChrColon");
            }
        }

        private void SetUtcHr(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                bool isShown = IsUtcShown && IsUtcHrShown;
                SetDoubleDigitInternal(value, isShown, new string[] { "UtcHrTens", "UtcHrOnes" });
            }
        }

        private void SetUtcMin(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                bool isShown = IsUtcShown && IsUtcMinShown;
                SetDoubleDigitInternal(value, isShown, new string[] { "UtcMinTens", "UtcMinOnes" });
            }
        }

        private void SetUtcSec(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                bool isShown = IsUtcShown && IsUtcSecShown;
                SetDoubleDigitInternal(value, isShown, new string[] { "UtcSecTens", "UtcSecOnes" });
            }
        }

        private void SetUtcShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsUtcShown = Convert.ToBoolean(value);

            // Instead of resetting the cache, just call them again to force update
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
                int intValue = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
                IsUtcColonLShown = Convert.ToBoolean(intValue);
                SetBoolInternal(IsUtcColonLShown, IsUtcShown, "UtcLeftColon");
            }
        }

        private void SetUtcColonRShown(string isShown)
        {
            if (!string.IsNullOrWhiteSpace(isShown))
            {
                int intValue = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
                IsUtcColonRShown = Convert.ToBoolean(intValue);
                SetBoolInternal(IsUtcColonRShown, IsUtcShown, "UtcRightColon");
            }
        }

        private void SetUtcHrShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsUtcHrShown = Convert.ToBoolean(value);
            SetUtcHr(LcdCurrentValuesCache[UTC_HR]);
        }

        private void SetUtcMinShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsUtcMinShown = Convert.ToBoolean(value);
            SetUtcMin(LcdCurrentValuesCache[UTC_MIN]);
        }

        private void SetUtcSecShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsUtcSecShown = Convert.ToBoolean(value);
            SetUtcSec(LcdCurrentValuesCache[UTC_SEC]);
        }


        private void SetEtHr(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsEtShown, new string[] { "EtHrTens", "EtHrOnes" });
            }
        }

        private void SetEtMin(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                SetDoubleDigitInternal(value, IsEtShown, new string[] { "EtMinTens", "EtMinOnes" });
            }
        }


        private void SetEtShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsEtShown = Convert.ToBoolean(value);

            // Instead of resetting the cache, just call them again to force update
            SetEtHr(LcdCurrentValuesCache[ET_HR]);
            SetEtMin(LcdCurrentValuesCache[ET_MIN]);
            SetEtColonShown(LcdCurrentValuesCache[ET_COLON_SHOWN]);
        }

        private void SetEtColonShown(string isShown)
        {
            if (!string.IsNullOrWhiteSpace(isShown))
            {
                int intValue = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
                IsEtColonShown = Convert.ToBoolean(intValue);
                SetBoolInternal(IsEtColonShown, IsEtShown, "EtColon");
            }
        }

        // "AllOn", "AllOff"      
        private void LcdTest(string command)
        {
            PrepareAndSendDisplayTestCommand(DisplayTestCommands[command]);
        }

        private void SendDisplayCommand(byte[] message)
        {
            MessageSender.SendDisplayCommands(new byte[][] { message, RefreshCommand });
        }

        private void SetSegmentDisplayCommand(DisplaySegment e, byte[] mes)
        {
            foreach (Bit b in e.Bits)
            {
                int index = b.ByteNumber + 17; // with header
                mes[index] = b.Value ? (byte)(mes[index] | (1 << b.BitPosition))
                                     : (byte)(mes[index] & ~(1 << b.BitPosition));
            }
        }
    }
}
