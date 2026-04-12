using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingAirbusThrottleDevice : IWinwingDevice
    {
        public string Name { get => $"WinWing {ThrottleType}"; }

        private IWinwingMessageSender MessageSender = null;
        private string ThrottleType = WinwingConstants.AIRBUS_THROTTLE_L_NAME;
        private byte[] DestinationAddress = WinwingConstants.DEST_AIRBUS_THROTTLE;
        private byte[] DestinationAddressPac = WinwingConstants.DEST_AIRBUS_PAC;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();

        private Dictionary<string, Action<byte>> OutputNameToActionMapping = new Dictionary<string, Action<byte>>();

        private const string VIBRATION_1 = "Vibration 1 Percentage";
        private const string VIBRATION_2 = "Vibration 2 Percentage";
        private const string BACK_BRIGHTNESS = "Backlight Percentage"; // PAC + THROTTLE        
        private const string LED_BRIGHTNESS = "LED Percentage"; // THROTTLE
        private const string LCD_BRIGHTNESS = "LCD Percentage"; // PAC

        private const string ANN_LIGHT = "LCD Test On/Off"; // PAC
        private const string TRIM_DASHES = "Trim Dashes On/Off"; // PAC        
        private const string TRIM = "Trim Value"; // PAC  Negative and 0 is L, Positive R

        private bool IsTrimDashed = false;

        private Dictionary<string, DisplaySegment> DisplayTestCommands = new Dictionary<string, DisplaySegment>()
        {
            { "AllOn",       new DisplaySegment(new Bit[] {new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false)},
            { "AllOff",      new DisplaySegment(new Bit[] {new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false)},
        };

        // Examples of Trim "L 0.0", "L 0.2", "R 0.0", "L 5.1" "L11.3"
        // In Error case, both FAC lost: " ---"

        // Center console rudder trim display.  Negative = Left, Positive = Right. Am FlyByWire A320
        // Beim Fenix: 
        // On the A320, the rudder trim indication becomes dashed when the Flight Augmentation Computers(FACs) are not
        // supplying valid rudder trim data—typically due to a failure or when both FACs are lost.

        // Element top byte is byte number in data section. So 0 is start of data section. Header with 17 bytes is not included.
        private Dictionary<string, DisplaySegment> DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
        {
            { "TrimDecimal",  new DisplaySegment(32, 3, 'b')},
            { "TrimOnes",     new DisplaySegment(32, 2, 'o')},  // 3 ist ones, 2 ist tenth, 1 ist hundreds, 0 ist L/R
            { "TrimTens",     new DisplaySegment(32, 1, '}')},
            { "TrimLR",       new DisplaySegment(32, 0, '{')},
            { "TrimDot",      new DisplaySegment(new Bit[] { new Bit(4,2, false) }, false)},
        };

 
        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {
            { "FAULT_1", 0x03 },
            { "FIRE_1",  0x04 },
            { "FAULT_2", 0x05 },
            { "FIRE_2",  0x06 }, 
        };

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();        

        private byte[] DisplayTestCommand = new byte[0x12];
        private byte[] RefreshCommand = new byte[0x11];    
        private byte[] SetValuesCommand = new byte[0x35];  // 35 equals 53, max of a content message 4 + 13 + 36 data

        public WinwingAirbusThrottleDevice(IWinwingMessageSender sender, string throttleType)
        {
            MessageSender = sender;
            ThrottleType = throttleType;

            // Add display options                                    
            DisplayNameToActionMapping.Add(TRIM, SetTrim);
            DisplayNameToActionMapping.Add(TRIM_DASHES, SetTrimDashed);
            DisplayNameToActionMapping.Add(ANN_LIGHT, SetAnnunciatorLightOnOff);

            // Add output options
            OutputNameToActionMapping.Add(VIBRATION_1, SetVibration1);
            OutputNameToActionMapping.Add(VIBRATION_2, SetVibration2);
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
            var initDisplayTest = new List<byte>(DestinationAddressPac);
            initDisplayTest.AddRange(new byte[2]);
            initDisplayTest.AddRange(WinwingConstants.DisplayCmdHeaders["0401"]);
            initDisplayTest.CopyTo(DisplayTestCommand, 0);

            // 4 + 13
            var initSetValues = new List<byte>(DestinationAddressPac);
            initSetValues.AddRange(new byte[2]);
            initSetValues.AddRange(WinwingConstants.DisplayCmdHeaders["0201_PAC"]);
            initSetValues.CopyTo(SetValuesCommand, 0);

            var initRefresh = new List<byte>(DestinationAddressPac);
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
            SetBacklightBrightness(20);
            SetLcdBrightness(100);
            SetVibration1(0);
            SetVibration2(0);

            //--------testing-----------------------
            //LcdTest("AllOn");
            //LcdTest("AllOff");
            //SetLedBrightness("100");
            //SetTrim("5");
            //SetTrim("0");
            //SetTrim("-2");
            //SetTrim("-0.3");
            //SetTrimDashed("1");
            //SetTrim("-24.3");
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
            SetVibration1(0);
            SetVibration2(0);
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

        private void SetVibration1(byte level)
        {
            MessageSender.SetVibration(DestinationAddress, 0x0e, level);
        }

        private void SetVibration2(byte level)
        {
            MessageSender.SetVibration(DestinationAddress, 0x10, level);
        }

        private void SetLedBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x02, brightness);
        }

        private void SetBacklightBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);
            MessageSender.SetBrightness(DestinationAddressPac, 0x00, brightness);
        }

        private void SetLcdBrightness(byte brightness)
        {
            // Yes strangely here 0x02 is used for LCD
            MessageSender.SetBrightness(DestinationAddressPac, 0x02, brightness);
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


        private void SetTrimDot(bool isDotSet)
        {
            var trimDot = DisplaySetValueSegments["TrimDot"];
            trimDot.SetValue(isDotSet);
            SetSegmentDisplayCommand(trimDot, SetValuesCommand);
        }

        private void SetTrim(string trim)
        {
            int value = (int)(Convert.ToDouble(trim, CultureInfo.InvariantCulture) * 10);
            char[] chars = new char[] { '*', '*', '*', '*' };

            if (IsTrimDashed)
            {
                chars = new char[] { '*', '-', '-', '-' };
                SetTrimDot(false);        
            }
            else
            {
                chars[0] = value <= 0 ? 'L' : 'A';

                // D2 specifies the minimum number of digits to display. If there are fewer than 2 digits, it will be left-padded with zeros
                // Value 0 => 00, Value 3 => 03, Value 10 => 10
                string valueString = Math.Abs(value).ToString("D2", CultureInfo.InvariantCulture).PadLeft(3, '*');
                for (int i = 0; i < 3; i++)
                {
                    chars[i+1] = valueString[i];
                }
                SetTrimDot(true);                
            }
            SetDigitsInternal(chars, new string[] { "TrimLR", "TrimTens", "TrimOnes", "TrimDecimal" });
        }

        
        private void SetTrimDashed(string isDashed)
        {
            int value = (int)Convert.ToDouble(isDashed, CultureInfo.InvariantCulture);
            IsTrimDashed = Convert.ToBoolean(value);
            
            if (!string.IsNullOrEmpty(LcdCurrentValuesCache[TRIM]))
            {
                // Update display
                SetTrim(LcdCurrentValuesCache[TRIM]);

                // Reset cache
                LcdCurrentValuesCache[TRIM] = string.Empty;
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

        public void Stop()
        {
            TurnOffAllLEDs();
            SetVibration1(0);
            SetVibration2(0);
        }
    }
}
