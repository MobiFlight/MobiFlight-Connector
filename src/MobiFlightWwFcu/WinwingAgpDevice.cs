using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

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
        private const string CHR_SHOWN = "CHR Shown On/Off";

        private const string ET_HR = "ET HR Value";
        private const string ET_MIN = "ET MIN Value";        
        private const string ET_SHOWN = "ET Shown On/Off";

        private const string UTC_HR = "UTC HR/MO Value";
        private const string UTC_MIN = "UTC MIN/DY Value";
        private const string UTC_SEC = "UTC SEC/Y Value";
        private const string UTC_SHOWN = "UTC Shown On/Off";

        private const string UTC_HR_SHOWN = "UTC HR/MO Shown On/Off";
        private const string UTC_MIN_SHOWN = "UTC MIN/DY Shown On/Off";
        private const string UTC_SEC_SHOWN = "UTC SEC/Y Shown On/Off";

        private const string BACK_BRIGHTNESS = "Backlight Percentage";     
        private const string LED_BRIGHTNESS = "LED Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";

        private bool IsChrShown = true;
        private bool IsUtcShown = true;
        private bool IsEtShown = true;
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
            { "AllOn",       new DisplaySegment(new Bit[] {new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false)},
            { "AllOff",      new DisplaySegment(new Bit[] {new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false)},
        };


        // Element top byte is byte number in data section. So 0 is start of data section. Header with 17 bytes is not included.
        private Dictionary<string, DisplaySegment> DisplaySetValueElements = new Dictionary<string, DisplaySegment>()
        {          
            { "ChrSecOnes",   new DisplaySegment(28, 3, 'b', isReverse: false)},
            { "ChrSecTens",   new DisplaySegment(28, 2, 'o', isReverse: false)},
            { "ChrMinOnes",   new DisplaySegment(28, 1, '}', isReverse: false)},
            { "ChrMinTens",   new DisplaySegment(28, 0, '{', isReverse: false)},
            { "ChrCol",       new DisplaySegment(new Bit[] { new Bit(32,2, true), new Bit(32,3, true) }, isSevenSegment: false)},

            //{ "CoLHundreds",  new DisplaySegment(32, 7)}, // PAP3 topByte, BitNumber
            //{ "CoLTens",      new DisplaySegment(32, 6)},
            //{ "CoLOnes",      new DisplaySegment(new DisplayBit[] { new DisplayBit(32,5), new DisplayBit(28,5), new DisplayBit(24,5), new DisplayBit(20,5), new DisplayBit(16,5), new DisplayBit(12,5), new DisplayBit(8,5) }, true)},
            //{ "SpdThousands", new DisplaySegment(32, 3)},
            //{ "SpdHundreds",  new DisplaySegment(32, 2)},
            //{ "SpdTens",      new DisplaySegment(32, 1)},
            //{ "SpdOnes",      new DisplaySegment(32, 0)},
            //{ "HdgHundreds",  new DisplaySegment(33, 6, '-')},
            //{ "HdgTens",      new DisplaySegment(33, 5, '-')},
            //{ "HdgOnes",      new DisplaySegment(33, 4, '-')},
            //{ "AltTenthsds",  new DisplaySegment(33, 2, '{')},
            //{ "HdgDot",       new DisplaySegment(new DisplayBit(17,3))},
            //{ "AltDot",       new DisplaySegment(new DisplayBit(5,0))},
            //{ "VsLabel",      new DisplaySegment(new DisplayBit(35,7))},
            //{ "FpaLabel",     new DisplaySegment(new DisplayBit(31,7))},
            //{ "VsPlusVert",   new DisplaySegment(new DisplayBit[] {new DisplayBit(23,7), new DisplayBit(19,7) }, false)},
            //{ "VsPlusHoriz",  new DisplaySegment(new DisplayBit(10,4, true))},
            //{ "VsDot",        new DisplaySegment(new DisplayBit(6,2))},
            //{ "CoRDot",       new DisplaySegment(new DisplayBit(7,4))},
        };

        public WinwingAgpDevice(IWinwingMessageSender sender)
        {
            MessageSender = sender;

            // Add display options
            DisplayNameToActionMapping.Add(CHR_MIN, SetChrMin);
            DisplayNameToActionMapping.Add(CHR_SEC, SetChrSec);
            DisplayNameToActionMapping.Add(CHR_SHOWN, SetChrShown);

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

            foreach (var element in DisplaySetValueElements.Values)
            {
                SetSegmentDisplayCommand(element, SetValuesCommand);
            }
        }

        public void Connect()
        {
            SendDisplayCommand(SetValuesCommand);
            SetBacklightBrightness(50);
            SetLcdBrightness(100);
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

        private void SetBoolInternal(string isSetString, string elementName)
        {
            int isSet = (int)Convert.ToDouble(isSetString, CultureInfo.InvariantCulture);
            var element = DisplaySetValueElements[elementName];
            element.SetValue(Convert.ToBoolean(isSet));
            SetSegmentDisplayCommand(element, SetValuesCommand);
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


        private void SetDigitsInternal(char[] chars, string[] elementNames)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                var element = DisplaySetValueElements[elementNames[i]];
                element.SetCharacter(chars[i]);
                SetSegmentDisplayCommand(element, SetValuesCommand);
            }

            SendDisplayCommand(SetValuesCommand);
        }


        private void SetChrMin(string course)
        {
            char[] chars;
            if (IsChrShown)
            {
                int courseInt = (int)Convert.ToDouble(course, CultureInfo.InvariantCulture);
                chars = courseInt.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                // TODO COLON
                chars = new char[] { '*', '*', '*' };
            }
            SetDigitsInternal(chars, new string[] { "CoRHundreds", "CoRTens", "CoROnes" });
        }

        private void SetChrSec(string course)
        {
            char[] chars;
            if (IsChrShown)
            {
                int courseInt = (int)Convert.ToDouble(course, CultureInfo.InvariantCulture);
                chars = courseInt.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                // TODO COLON
                chars = new char[] { '*', '*', '*' };
            }
            SetDigitsInternal(chars, new string[] { "CoRHundreds", "CoRTens", "CoROnes" });
        }

        private void SetChrShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsChrShown = Convert.ToBoolean(value);

            // Reset cache
            LcdCurrentValuesCache[CHR_MIN] = string.Empty;
            LcdCurrentValuesCache[CHR_SEC] = string.Empty;
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
