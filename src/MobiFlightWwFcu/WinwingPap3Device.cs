using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingPap3Device : IWinwingDevice
    {
        public string Name { get; } = "WinWing PAP3";

        private IWinwingMessageSender MessageSender = null;

        private byte[] DestinationAddress = WinwingConstants.DEST_PAP3;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();

        private const string COURSE_LEFT = "Course Left Value";
        private const string COURSE_RIGHT = "Course Right Value";
        private const string COL_SHOWN = "Course Left Shown On/Off";
        private const string COR_SHOWN = "Course Right Shown On/Off";

        private const string SPEED = "Speed Value";
        private const string MACH = "Mach Value";
        private const string SPEED_SHOWN = "Speed Shown On/Off";
        private const string MACH_LABEL = "MACH Label On/Off";
        private const string IAS_LABEL = "IAS Label On/Off";        
        private const string SPEED_A = "Speed A On/Off";           
        private const string SPEED_B = "Speed B On/Off"; 


        private const string HEADING = "Heading Value";
        private const string TRK = "TRK Value";       
        private const string HDG_LABEL = "HDG Label On/Off";
        private const string TRK_LABEL = "TRK Label On/Off";

        private const string ALTITUDE = "Altitude Value";
   
        private const string VS = "VS Value";
        private const string FPA = "FPA Value";    
        private const string VS_SHOWN = "VS Shown On/Off";     
        private const string VS_LABEL = "VS Label On/Off";
        private const string FPA_LABEL = "FPA Label On/Off";

        private const string ANN_LIGHT = "LCD Test On/Off";
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";
        private const string LED_BRIGHTNESS = "LED Percentage";

        private bool IsCourseLeftShown = true;
        private bool IsCourseRightShown = true;
        private bool IsSpeedShown = true;
        private bool IsVsShown = true;
        private bool IsSpeedA = false;
        private bool IsSpeedB = false;


        private Dictionary<string, DisplaySegment> DisplayTestCommands = new Dictionary<string, DisplaySegment>()
        {
            { "AllOn",       new DisplaySegment(new Bit[] {new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false)},
            { "AllOff",      new DisplaySegment(new Bit[] {new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false)},
        };


        // Element top byte is byte number in data section. So 0 is start of data section. Header with 17 bytes is not included.
        private Dictionary<string, DisplaySegment> DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
        {                                   
            { "CoLHundreds",  new DisplaySegment(32, 7)}, // PAP3 topByte, BitNumber
            { "CoLTens",      new DisplaySegment(32, 6)},
            { "CoLOnes",      new DisplaySegment(new Bit[] { new Bit(32,5), new Bit(28,5), new Bit(24,5), new Bit(20,5), new Bit(16,5), new Bit(12,5), new Bit(8,5) }, true)},                   
            { "SpdThousands", new DisplaySegment(32, 3)},
            { "SpdHundreds",  new DisplaySegment(32, 2)},
            { "SpdTens",      new DisplaySegment(32, 1)},
            { "SpdOnes",      new DisplaySegment(32, 0)},
            { "HdgHundreds",  new DisplaySegment(33, 6, '-')},
            { "HdgTens",      new DisplaySegment(33, 5, '-')},
            { "HdgOnes",      new DisplaySegment(33, 4, '-')},
            { "AltTenthsds",  new DisplaySegment(33, 2, '{')},
            { "AltThousands", new DisplaySegment(33, 1, '}')},
            { "AltHundreds",  new DisplaySegment(33, 0, 'o')},
            { "AltTens",      new DisplaySegment(34, 7, 'b')},
            { "AltOnes",      new DisplaySegment(34, 6, 'l')},
            { "VsThousands",  new DisplaySegment(34, 3, '-')},
            { "VsHundreds",   new DisplaySegment(34, 2, '-')},
            { "VsTens",       new DisplaySegment(34, 1)},
            { "VsOnes",       new DisplaySegment(34, 0)},
            { "CoRHundreds",  new DisplaySegment(35, 6)},
            { "CoRTens",      new DisplaySegment(35, 5)},
            { "CoROnes",      new DisplaySegment(35, 4)},
            { "CoLDot",       new DisplaySegment(new Bit(4,5))},
            { "IasLabel",     new DisplaySegment(new Bit(33,7))},
            { "MachLabel",    new DisplaySegment(new Bit(29,7))},
            { "SpdPlusVert",  new DisplaySegment(new Bit[] {new Bit(13,7), new Bit(9,7) }, false)},
            { "SpdPlusHoriz", new DisplaySegment(new Bit(8,3))},
            { "MachDot",      new DisplaySegment(new Bit(4,2))},
            { "HdgLabel",     new DisplaySegment(new Bit(33,3))},
            { "TrkLabel",     new DisplaySegment(new Bit(25,3))},
            { "HdgDot",       new DisplaySegment(new Bit(17,3))},
            { "AltDot",       new DisplaySegment(new Bit(5,0))},
            { "VsLabel",      new DisplaySegment(new Bit(35,7))},
            { "FpaLabel",     new DisplaySegment(new Bit(31,7))},
            { "VsPlusVert",   new DisplaySegment(new Bit[] {new Bit(23,7), new Bit(19,7) }, false)},
            { "VsPlusHoriz",  new DisplaySegment(new Bit(10,4, true))},
            { "VsDot",        new DisplaySegment(new Bit(6,2))},
            { "CoRDot",       new DisplaySegment(new Bit(7,4))},
        };   

        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {
            { "N1",     0x03 },
            { "SPEED",  0x04 },
            { "VNAV",   0x05 },
            { "LVL_CHG",0x06 },
            { "HDG_SEL",0x07 },            
            { "LNAV",   0x08 },
            { "VOR_LOC",0x09 },
            { "APP",    0x0a },
            { "ALT_HLD",0x0b },
            { "VS",     0x0c },
            { "A_CMD",  0x0d },
            { "A_CWS",  0x0e },
            { "B_CMD",  0x0f },
            { "B_CWS",  0x10 },
            { "AT_ARM", 0x11 },
            { "L_MA",   0x12 },
            { "R_MA",   0x13 },
            { "Solenoid", 0x1e },            
        };

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();        

        private byte[] DisplayTestCommand = new byte[0x12];
        private byte[] RefreshCommand = new byte[0x11];       
        private byte[] SetValuesCommand = new byte[0x3C];  // 3C equals 60, max of a content message 4 + 13 + 43 data

        public WinwingPap3Device(IWinwingMessageSender sender)
        {
            MessageSender = sender;

            // Add display options
            DisplayNameToActionMapping.Add(COURSE_LEFT, SetCourseLeft);
            DisplayNameToActionMapping.Add(COURSE_RIGHT, SetCourseRight);
            DisplayNameToActionMapping.Add(COL_SHOWN, SetCourseLeftShown);
            DisplayNameToActionMapping.Add(COR_SHOWN, SetCourseRightShown);

            DisplayNameToActionMapping.Add(SPEED, SetSpeed);       
            DisplayNameToActionMapping.Add(MACH, SetMachSpeed);
            DisplayNameToActionMapping.Add(SPEED_SHOWN, SetSpeedShown);
            DisplayNameToActionMapping.Add(IAS_LABEL, SetIasLabel);
            DisplayNameToActionMapping.Add(MACH_LABEL, SetMachLabel);
            DisplayNameToActionMapping.Add(SPEED_A, SetSpeedA);
            DisplayNameToActionMapping.Add(SPEED_B, SetSpeedB);

            DisplayNameToActionMapping.Add(HEADING, SetHeading);
            DisplayNameToActionMapping.Add(TRK, SetTrack);
            DisplayNameToActionMapping.Add(HDG_LABEL, SetHdgLabel);
            DisplayNameToActionMapping.Add(TRK_LABEL, SetTrkLabel);

            DisplayNameToActionMapping.Add(ALTITUDE, SetAltitude);

            DisplayNameToActionMapping.Add(VS, SetVs);
            DisplayNameToActionMapping.Add(FPA, SetFpa);
            DisplayNameToActionMapping.Add(VS_SHOWN, SetVsShown);
            DisplayNameToActionMapping.Add(VS_LABEL, SetVsLabel);
            DisplayNameToActionMapping.Add(FPA_LABEL, SetFpaLabel);

            DisplayNameToActionMapping.Add(ANN_LIGHT, SetAnnunciatorLightOnOff);
            DisplayNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            DisplayNameToActionMapping.Add(LCD_BRIGHTNESS, SetLcdBrightness);
            DisplayNameToActionMapping.Add(LED_BRIGHTNESS, SetLedBrightness);

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
            initSetValues.AddRange(WinwingConstants.DisplayCmdHeaders["0201_PAP"]);
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
            SendDisplayCommand(SetValuesCommand); // Init display
            SetBacklightBrightness("50");
            SetLcdBrightness("100");
            //SetLedBrightness("100");

            //LcdTest("AllOn"); // used for testing

            //-------------------------------
            ////SetSpeed("360");
            //SetMachSpeed("0.4989");
            ////SetSpeedB("1");
            //SetSpeedA("0");
            //SetIasLabel("1");
            //SetMachLabel("0.2");
            //SetCourseLeft("234");
            //SetCourseRight("11");
            //SetHeading("233");
            //SetHdgLabel("1");
            //SetAltitude("1200");
            //SetVsShown("1");
            //SetVs("2500");
            ////SetFpa("-1.88");     
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
            SetBacklightBrightness("0");
            SetLcdBrightness("0");
            TurnOffAllLEDs();        
        }

        public List<string> GetLedNames()
        {
            return LedIdentifiers.Keys.ToList();
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
                LedCurrentValuesCache[led] = state;           
                byte stateAdjusted = state == 0 ? (byte)0 : (byte)1;
                MessageSender.SendLightControlMessage(DestinationAddress, LedIdentifiers[led], stateAdjusted);
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
        private void SetLedBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x02, brightness);
        }

        private void SetBacklightBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);
        }

        private void SetLcdBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x01, brightness);
        }

        private void PrepareAndSendDisplayTestCommand(DisplaySegment segment)
        {
            SetSegmentDisplayCommand(segment, DisplayTestCommand);
            SendDisplayCommand(DisplayTestCommand);
        }


        private void EmptyDisplay()
        {
            LcdTest("AllOff");

            //var resetMsg = new MsgEntry { StartPos = 21, Mask = new byte[18], Data = new byte[18] };
            //SetBytesDisplayCommand(resetMsg, SetValuesCommand);
            //SendDisplayCommand(SetValuesCommand);
        }

        private void SetBoolInternal(string isSetString, string segmentName)
        {
            int isSet = (int)Convert.ToDouble(isSetString, CultureInfo.InvariantCulture);
            var segment = DisplaySetValueSegments[segmentName];
            segment.SetValue(Convert.ToBoolean(isSet));
            SetSegmentDisplayCommand(segment, SetValuesCommand);
            SendDisplayCommand(SetValuesCommand);
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

        private void SetCourseLeft(string course)
        {
            char[] chars;
            if (IsCourseLeftShown)
            {
                int courseInt = (int)Convert.ToDouble(course, CultureInfo.InvariantCulture);
                chars = courseInt.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                chars = new char[] { '*', '*', '*' };
            }

            SetDigitsInternal(chars, new string[] { "CoLHundreds", "CoLTens", "CoLOnes" });            
        }

        private void SetCourseRight(string course)
        {
            char[] chars;
            if (IsCourseRightShown)
            {
                int courseInt = (int)Convert.ToDouble(course, CultureInfo.InvariantCulture);
                chars = courseInt.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                chars = new char[] { '*', '*', '*' };
            }
            SetDigitsInternal(chars, new string[] { "CoRHundreds", "CoRTens", "CoROnes" });
        }

        private void SetCourseLeftShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsCourseLeftShown = Convert.ToBoolean(value);

            // Reset cache
            LcdCurrentValuesCache[COURSE_LEFT] = string.Empty;     
        }

        private void SetCourseRightShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsCourseRightShown = Convert.ToBoolean(value);

            // Reset cache
            LcdCurrentValuesCache[COURSE_RIGHT] = string.Empty;
        }

        private void SetMachDot(bool isDotSet)
        {
            var machDot = DisplaySetValueSegments["MachDot"];
            machDot.SetValue(isDotSet);
            SetSegmentDisplayCommand(machDot, SetValuesCommand);
        }

        private void RefreshOnMachModeChange()
        {
            var spdThousands = DisplaySetValueSegments["SpdThousands"];
            spdThousands.SetCharacter('*');
            SetSegmentDisplayCommand(spdThousands, SetValuesCommand);
            var spdHundreds = DisplaySetValueSegments["SpdHundreds"];
            spdHundreds.SetCharacter('*');
            SetSegmentDisplayCommand(spdHundreds, SetValuesCommand);
            LcdCurrentValuesCache[SPEED_A] = string.Empty;
            LcdCurrentValuesCache[SPEED_B] = string.Empty;
        }

        private void SetSpeed(string speed)
        {            
            var machDot = DisplaySetValueSegments["MachDot"];
            bool isMachModeChange = machDot.Bits[0].Value == true;
            SetMachDot(false); // update beforehand!
            if (isMachModeChange)
            {
                RefreshOnMachModeChange();
            }

            int value = (int)Convert.ToDouble(speed, CultureInfo.InvariantCulture);
            char[] chars;    
            
            if (IsSpeedShown)
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
            LcdCurrentValuesCache[MACH] = string.Empty; // Reset for Speed/Mach change
        }
        
        private void SetMachSpeed(string speed)
        {                        
            var machDot = DisplaySetValueSegments["MachDot"];
            bool isMachModeChange = machDot.Bits[0].Value == false;
            SetMachDot(true); // update beforehand!
            if (isMachModeChange )
            {
                RefreshOnMachModeChange();
            }

            int value = (int)(Convert.ToDouble(speed, CultureInfo.InvariantCulture) * 100);
            char[] chars;

            if (IsSpeedShown)
            {
                if (value == 999)
                {
                    chars = new char[] { '-', '-', '-' };
                    SetDigitsInternal(chars, new string[] { "SpdHundreds", "SpdTens", "SpdOnes" });
                }
                else if (IsSpeedA || IsSpeedB)
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
                                       
            LcdCurrentValuesCache[SPEED] = string.Empty; // Reset for Speed/Mach change
        }

        private void SetSpeedShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsSpeedShown = Convert.ToBoolean(value);

            // Reset cache
            LcdCurrentValuesCache[MACH] = string.Empty;
            LcdCurrentValuesCache[SPEED] = string.Empty;
            LcdCurrentValuesCache[SPEED_A] = string.Empty;
            LcdCurrentValuesCache[SPEED_B] = string.Empty;
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
            IsSpeedA = isA;

            if (IsSpeedShown)
            {
                var machDot = DisplaySetValueSegments["MachDot"];
                string segmentName = machDot.Bits[0].Value ? "SpdHundreds" : "SpdThousands";

                if (isA)
                {
                    SetDigitsInternal(new char[] { 'A' }, new string[] { segmentName });
                }
                else
                {
                    SetDigitsInternal(new char[] { '*' }, new string[] { segmentName });
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
            IsSpeedB = isB;

            if (IsSpeedShown)
            {
                var machDot = DisplaySetValueSegments["MachDot"];
                string segmentName = machDot.Bits[0].Value ? "SpdHundreds" : "SpdThousands";

                if (isB)
                {
                    SetDigitsInternal(new char[] { 'B' }, new string[] { segmentName });
                }
                else
                {
                    SetDigitsInternal(new char[] { '*' }, new string[] { segmentName });
                }
            }
            else
            {
                SetDigitsInternal(new char[] { '*', '*' }, new string[] { "SpdThousands", "SpdHundreds" });
            }
        }

        private void SetTrack(string track)
        {
            int value = (int)Convert.ToDouble(track, CultureInfo.InvariantCulture);
            char[] chars = value.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            if (value == 999) chars = new char[] { '-', '-', '-' };
            SetDigitsInternal(chars, new string[] { "HdgHundreds", "HdgTens", "HdgOnes" });
            LcdCurrentValuesCache[HEADING] = string.Empty; // Reset for Heading/Track change
        }

        private void SetHeading(string heading)
        {
            int value = (int)Convert.ToDouble(heading, CultureInfo.InvariantCulture);
            char[] chars = value.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            if (value == 999) chars = new char[] { '-', '-', '-' };
            SetDigitsInternal(chars, new string[] { "HdgHundreds", "HdgTens", "HdgOnes" });
            LcdCurrentValuesCache[TRK] = string.Empty; // Reset for Heading/Track change
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
            var vsDot = DisplaySetValueSegments["VsDot"];
            vsDot.SetValue(isDotSet);
            SetSegmentDisplayCommand(vsDot, SetValuesCommand);
        }

        private void SetVsSign(bool isPlus, bool isMinus)
        {
            var vsPlusHoriz = DisplaySetValueSegments["VsPlusHoriz"];
            var vsPlusVert = DisplaySetValueSegments["VsPlusVert"];
            
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

            SetSegmentDisplayCommand(vsPlusHoriz, SetValuesCommand);
            SetSegmentDisplayCommand(vsPlusVert, SetValuesCommand);
        }

        private void SetVs(string vs)
        {
            int value = (int)Convert.ToDouble(vs, CultureInfo.InvariantCulture);
            char[] chars;

            if (IsVsShown)
            {                
                if (value == 0)
                {
                    chars = new char[] { '*', '*', '*', '*' };
                    SetVsSign(false, false);
                }
                else if (value == 9999)
                {
                    chars = new char[] { '-', '-', '-', '-' };
                    SetVsSign(false, true);
                }
                else if (value < 0)
                {
                    chars = Math.Abs(value).ToString().PadLeft(4, '*').ToCharArray();
                    SetVsSign(false, true);
                }
                else
                {
                    chars = Math.Abs(value).ToString().PadLeft(4, '*').ToCharArray();
                    SetVsSign(true, false);
                }
                SetVsDot(false);
            }
            else
            {
                chars = new char[] { '*', '*', '*', '*' };
                SetVsSign(false, false);
                SetVsDot(false);
            }

            SetDigitsInternal(chars, new string[] { "VsThousands", "VsHundreds", "VsTens", "VsOnes" });
            LcdCurrentValuesCache[FPA] = string.Empty; // Reset for Vs/Fpa change
        }

        private void SetFpa(string vs)
        {
            int value = (int)(Convert.ToDouble(vs, CultureInfo.InvariantCulture) * 10);
            char[] chars;

            if (IsVsShown)
            {
                if (value == 0)
                {
                    chars = new char[] { '*', '0', '0', '*' };
                    SetVsSign(false, false);
                }
                else if (value == 999)
                {
                    chars = new char[] { '-', '-', '-', '-' };
                    SetVsSign(false, true);
                }
                else if (value < 0)
                {
                    string valueString = (Math.Abs(value).ToString("D2", CultureInfo.InvariantCulture).PadLeft(3, '*')).PadRight(4, '*');
                    chars = valueString.ToCharArray();
                    SetVsSign(false, true);
                }
                else
                {
                    string valueString = (Math.Abs(value).ToString("D2", CultureInfo.InvariantCulture).PadLeft(3, '*')).PadRight(4, '*');
                    chars = valueString.ToCharArray();
                    SetVsSign(true, false);
                }
                SetVsDot(true);
            }
            else
            {
                chars = new char[] { '*', '*', '*', '*' };
                SetVsSign(false, false);
                SetVsDot(false);
            }
            SetDigitsInternal(chars, new string[] { "VsThousands", "VsHundreds", "VsTens", "VsOnes" });
            LcdCurrentValuesCache[VS] = string.Empty; // Reset for Vs/Fpa change,
        }

        private void SetVsLabel(string isLabel)
        {
            SetBoolInternal(isLabel, "VsLabel");
        }

        private void SetFpaLabel(string isLabel)
        {
            SetBoolInternal(isLabel, "FpaLabel");
        }

        private void SetVsShown(string isShown)
        {
            int value = (int)Convert.ToDouble(isShown, CultureInfo.InvariantCulture);
            IsVsShown = Convert.ToBoolean(value);

            // Reset cache
            LcdCurrentValuesCache[VS] = string.Empty;
            LcdCurrentValuesCache[FPA] = string.Empty;
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
        }
    }
}
