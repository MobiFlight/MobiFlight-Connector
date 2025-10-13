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

        private WinwingMessageSender MessageSender = null;
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
        private const string TRIM = "Trim Value"; // PAC
        private const string TRIM_DIR_SHOWN = "Trim Direction On/Off"; // PAC
        private const string TRIM_DIR = "Trim Direction Switch"; // PAC

        private bool IsTrimDirShown = true;

        private Dictionary<string, Element> DisplayTestCommands = new Dictionary<string, Element>()
        {
            { "AllOn",       new Element(new Bit[] {new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false)},
            { "AllOff",      new Element(new Bit[] {new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false)},
        };

        private Dictionary<string, Element> DisplaySetValueElements = new Dictionary<string, Element>()
        {                                   
            { "CoLHundreds",  new Element(32, 7)}, // PAP3 topByte, BitNumber
            { "CoLTens",      new Element(32, 6)},
            { "CoLOnes",      new Element(new Bit[] { new Bit(32,5), new Bit(28,5), new Bit(24,5), new Bit(20,5), new Bit(16,5), new Bit(12,5), new Bit(8,5) }, true)},                   
            { "SpdThousands", new Element(32, 3)},
            { "SpdHundreds",  new Element(32, 2)},
            { "SpdTens",      new Element(32, 1)},
            { "SpdOnes",      new Element(32, 0)},
            { "VsThousands",  new Element(34, 3, '-')},
            { "VsHundreds",   new Element(34, 2, '-')},
            { "VsTens",       new Element(34, 1)},
            { "VsOnes",       new Element(34, 0)},
            { "VsLabel",      new Element(new Bit(35,7))},
            { "FpaLabel",     new Element(new Bit(31,7))},
            { "VsPlusVert",   new Element(new Bit[] {new Bit(23,7), new Bit(19,7) }, false)},
            { "VsPlusHoriz",  new Element(new Bit(10,4, true))},
            { "VsDot",        new Element(new Bit(6,2))},

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
        private byte[] SetValuesCommand = new byte[0x3C];  // 3C equals 60, max of a content message 4 + 13 + 43 data

        public WinwingAirbusThrottleDevice(WinwingMessageSender sender, string throttleType)
        {
            MessageSender = sender;
            ThrottleType = throttleType;

            // Add display options                        
            //DisplayNameToActionMapping.Add(ANN_LIGHT, SetAnnunciatorLightOnOff);
            //DisplayNameToActionMapping.Add(TRIM, SetVs);
            //DisplayNameToActionMapping.Add(TRIM_DIR_SHOWN, SetFpa);
            //DisplayNameToActionMapping.Add(TRIM_DIR, SetVsShown);

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
            initSetValues.AddRange(WinwingConstants.DisplayCmdHeaders["0201_P"]);
            initSetValues.CopyTo(SetValuesCommand, 0);

            var initRefresh = new List<byte>(DestinationAddressPac);
            initRefresh.AddRange(new byte[2]);
            initRefresh.AddRange(WinwingConstants.DisplayCmdHeaders["0301"]);
            initRefresh.CopyTo(RefreshCommand, 0);

            foreach (var element in DisplaySetValueElements.Values)
            {
                SetElementDisplayCommand(element, SetValuesCommand);
            }
        }

        public void Connect()
        {            
            // SendDisplayCommand(SetValuesCommand); // Init display TODO
            SetBacklightBrightness(20);
            SetLcdBrightness(100);
            SetVibration1(0);
            SetVibration2(0);

            //SetLedBrightness("100");
            //LcdTest("AllOn"); // used for testing

            //-------------------------------
            ////SetSpeed("360");
            //SetMachSpeed("0.4989");
            ////SetSpeedB("1");
            //SetSpeedA("0");
            //SetIasLabel("1");    
        }

        public void Shutdown()
        {                
            EmptyDisplay();
            SetBacklightBrightness(0);
            SetLcdBrightness(0);
            SetVibration1(0);
            SetVibration2(0);
            foreach (var ledName in LedIdentifiers.Keys)
            {
                SetLed(ledName, 0);
            }         
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

        private void PrepareAndSendDisplayTestCommand(Element element)
        {
            SetElementDisplayCommand(element, DisplayTestCommand);
            SendDisplayCommand(DisplayTestCommand);
        }


        private void EmptyDisplay()
        {
            LcdTest("AllOff");

            //var resetMsg = new MsgEntry { StartPos = 21, Mask = new byte[18], Data = new byte[18] };
            //SetBytesDisplayCommand(resetMsg, SetValuesCommand);
            //SendDisplayCommand(SetValuesCommand);
        }

        private void SetBoolInternal(string isSetString, string elementName)
        {
            int isSet = (int)Convert.ToDouble(isSetString, CultureInfo.InvariantCulture);
            var element = DisplaySetValueElements[elementName];
            element.SetValue(Convert.ToBoolean(isSet));
            SetElementDisplayCommand(element, SetValuesCommand);
            SendDisplayCommand(SetValuesCommand);
        }


        private void SetDigitsInternal(char[] chars, string[] elementNames)
        {
            for (int i = 0; i < chars.Length; i++)
            {
                var element = DisplaySetValueElements[elementNames[i]];
                element.SetCharacter(chars[i]);
                SetElementDisplayCommand(element, SetValuesCommand);
            }

            SendDisplayCommand(SetValuesCommand);
        }


        private void SetMachDot(bool isDotSet)
        {
            var machDot = DisplaySetValueElements["MachDot"];
            machDot.SetValue(isDotSet);
            SetElementDisplayCommand(machDot, SetValuesCommand);
        }

        private void RefreshOnMachModeChange()
        {
            var spdThousands = DisplaySetValueElements["SpdThousands"];
            spdThousands.SetCharacter('*');
            SetElementDisplayCommand(spdThousands, SetValuesCommand);
            var spdHundreds = DisplaySetValueElements["SpdHundreds"];
            spdHundreds.SetCharacter('*');
            SetElementDisplayCommand(spdHundreds, SetValuesCommand);
            //LcdCurrentValuesCache[SPEED_A] = string.Empty;
            //LcdCurrentValuesCache[SPEED_B] = string.Empty;
        }

        private void SetSpeed(string speed)
        {            
            var machDot = DisplaySetValueElements["MachDot"];
            bool isMachModeChange = machDot.Bits[0].Value == true;
            SetMachDot(false); // update beforehand!
            if (isMachModeChange)
            {
                RefreshOnMachModeChange();
            }

            int value = (int)Convert.ToDouble(speed, CultureInfo.InvariantCulture);
            char[] chars;    
            
            if (IsTrimDirShown)
            {                
                if (value == 999)
                {
                    chars = new char[] { '-', '-', '-' };
                }
                else
                {
                    chars = value.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();                   
                }
            }
            else
            {
                chars = new char[] { '*', '*', '*' };                
            }

            SetDigitsInternal(chars, new string[] { "SpdHundreds", "SpdTens", "SpdOnes" });
            //LcdCurrentValuesCache[MACH] = string.Empty; // Reset for Speed/Mach change
        }
        
        private void SetMachSpeed(string speed)
        {                        
            var machDot = DisplaySetValueElements["MachDot"];
            bool isMachModeChange = machDot.Bits[0].Value == false;
            SetMachDot(true); // update beforehand!
            if (isMachModeChange )
            {
                RefreshOnMachModeChange();
            }

            int value = (int)(Convert.ToDouble(speed, CultureInfo.InvariantCulture) * 100);
            char[] chars;

            if (IsTrimDirShown)
            {
                if (value == 999)
                {
                    chars = new char[] { '-', '-', '-' };
                    SetDigitsInternal(chars, new string[] { "SpdHundreds", "SpdTens", "SpdOnes" });
                }
                else if (IsTrimDirShown || IsTrimDirShown)
                {
                    // A or B is shown at the hundreds position
                    chars = value.ToString("D2", CultureInfo.InvariantCulture).ToCharArray();
                    SetDigitsInternal(chars, new string[] { "SpdTens", "SpdOnes" });
                }
                else
                {
                    chars = value.ToString("D2", CultureInfo.InvariantCulture).PadLeft(3, '*').ToCharArray();
                    SetDigitsInternal(chars, new string[] { "SpdHundreds", "SpdTens", "SpdOnes" });
                }
            }
            else
            {
                SetMachDot(false);
                chars = new char[] { '*', '*', '*' };
                SetDigitsInternal(chars, new string[] { "SpdHundreds", "SpdTens", "SpdOnes" });
            }
                                       
            //LcdCurrentValuesCache[SPEED] = string.Empty; // Reset for Speed/Mach change
        }

        private void SetSpeedShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsTrimDirShown = Convert.ToBoolean(value);
        }

        private void SetIasLabel(string isLabel)        
        {
            SetBoolInternal(isLabel, "IasLabel");
        }

        private void SetMachLabel(string isLabel)
        {
            SetBoolInternal(isLabel, "MachLabel");
        }

        private void SetSpeedA(string isSpeedA)
        {
            int value = (int)Convert.ToDouble(isSpeedA, CultureInfo.InvariantCulture);
            bool isA = Convert.ToBoolean(value);
            IsTrimDirShown = isA;

            if (IsTrimDirShown)
            {
                var machDot = DisplaySetValueElements["MachDot"];
                string elementName = machDot.Bits[0].Value ? "SpdHundreds" : "SpdThousands";

                if (isA)
                {
                    SetDigitsInternal(new char[] { 'A' }, new string[] { elementName });
                }
                else
                {
                    SetDigitsInternal(new char[] { '*' }, new string[] { elementName });
                }
            }
            else
            {
                SetDigitsInternal(new char[] { '*', '*' }, new string[] { "SpdThousands", "SpdHundreds" });
            }
        }

        private void SetSpeedB(string isSpeedB)
        {
            int value = (int)Convert.ToDouble(isSpeedB, CultureInfo.InvariantCulture);
            bool isB = Convert.ToBoolean(value);
            IsTrimDirShown = isB;

            if (IsTrimDirShown)
            {
                var machDot = DisplaySetValueElements["MachDot"];
                string elementName = machDot.Bits[0].Value ? "SpdHundreds" : "SpdThousands";

                if (isB)
                {
                    SetDigitsInternal(new char[] { 'B' }, new string[] { elementName });
                }
                else
                {
                    SetDigitsInternal(new char[] { '*' }, new string[] { elementName });
                }
            }
            else
            {
                SetDigitsInternal(new char[] { '*', '*' }, new string[] { "SpdThousands", "SpdHundreds" });
            }
        }

        private void SetHdgLabel(string isLabel)
        {
            SetBoolInternal(isLabel, "HdgLabel");
        }

        private void SetTrkLabel(string isLabel)
        {
            SetBoolInternal(isLabel, "TrkLabel");
        }


        private void SetAltitude(string altitude)
        {            
            int value = (int)Convert.ToDouble(altitude, CultureInfo.InvariantCulture);
            char[] chars;
            if (value == 0)
            {
                chars = new char[] {'*', '0', '0', '0', '0' };
            }
            else
            {
                chars = value.ToString().PadLeft(5, '*').ToCharArray();
            }
                     
            SetDigitsInternal(chars, new string[] { "AltTenthsds", "AltThousands", "AltHundreds", "AltTens", "AltOnes" });           
        }

        private void SetVsDot(bool isDotSet)
        {
            var vsDot = DisplaySetValueElements["VsDot"];
            vsDot.SetValue(isDotSet);
            SetElementDisplayCommand(vsDot, SetValuesCommand);
        }

        private void SetVsSign(bool isPlus, bool isMinus)
        {
            var vsPlusHoriz = DisplaySetValueElements["VsPlusHoriz"];
            var vsPlusVert = DisplaySetValueElements["VsPlusVert"];
            
            if (isPlus)
            {
                vsPlusHoriz.SetValue(true);
                vsPlusVert.SetValue(true);
            }
            else if (isMinus)
            {
                vsPlusHoriz.SetValue(true);
                vsPlusVert.SetValue(false);
            }
            else
            {
                vsPlusHoriz.SetValue(false);
                vsPlusVert.SetValue(false);
            }

            SetElementDisplayCommand(vsPlusHoriz, SetValuesCommand);
            SetElementDisplayCommand(vsPlusVert, SetValuesCommand);
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

        private void SetElementDisplayCommand(Element e, byte[] mes)
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
