using System;
using System.Collections.Generic;
using System.Globalization;

namespace MobiFlightWwFcu
{
    internal class WinwingPap3Device : SegmentDisplayDeviceBase
    {
        public override string Name => "WinWing PAP3";

        protected override string SetValuesHeaderKey => "0201_PAP";

        private const string COURSE_LEFT  = "Course Left Value";
        private const string COURSE_RIGHT = "Course Right Value";
        private const string COL_SHOWN    = "Course Left Shown On/Off";
        private const string COR_SHOWN    = "Course Right Shown On/Off";

        private const string SPEED       = "Speed Value";
        private const string MACH        = "Mach Value";
        private const string SPEED_SHOWN = "Speed Shown On/Off";
        private const string MACH_LABEL  = "MACH Label On/Off";
        private const string IAS_LABEL   = "IAS Label On/Off";
        private const string SPEED_A     = "Speed A On/Off";
        private const string SPEED_B     = "Speed B On/Off";

        private const string HEADING   = "Heading Value";
        private const string TRK       = "TRK Value";
        private const string HDG_LABEL = "HDG Label On/Off";
        private const string TRK_LABEL = "TRK Label On/Off";

        private const string ALTITUDE = "Altitude Value";

        private const string VS        = "VS Value";
        private const string FPA       = "FPA Value";
        private const string VS_SHOWN  = "VS Shown On/Off";
        private const string VS_LABEL  = "VS Label On/Off";
        private const string FPA_LABEL = "FPA Label On/Off";

        private bool IsCourseLeftShown  = true;
        private bool IsCourseRightShown = true;
        private bool IsSpeedShown       = true;
        private bool IsVsShown          = true;
        private bool IsSpeedA           = false;
        private bool IsSpeedB           = false;

        public WinwingPap3Device(IWinwingMessageSender sender)
            : base(sender, WinwingConstants.DEST_PAP3, 0x3C)  // 60 = max content message: 4 + 13 + 43 data
        {
            DisplayTestCommands = new Dictionary<string, DisplaySegment>()
            {
                { "AllOn",  new DisplaySegment(new Bit[] { new Bit(0,0, true), new Bit(0,1), new Bit(0,2), new Bit(0,3) }, false) },
                { "AllOff", new DisplaySegment(new Bit[] { new Bit(0,0), new Bit(0,1, true), new Bit(0,2), new Bit(0,3) }, false) },
            };

            // Element top byte is byte number in data section. So 0 is start of data section. Header with 17 bytes is not included.
            DisplaySetValueSegments = new Dictionary<string, DisplaySegment>()
            {
                { "CoLHundreds",  new DisplaySegment(32, 7) }, // PAP3 topByte, BitNumber
                { "CoLTens",      new DisplaySegment(32, 6) },
                { "CoLOnes",      new DisplaySegment(new Bit[] { new Bit(32,5), new Bit(28,5), new Bit(24,5), new Bit(20,5), new Bit(16,5), new Bit(12,5), new Bit(8,5) }, true) },
                { "SpdThousands", new DisplaySegment(32, 3) },
                { "SpdHundreds",  new DisplaySegment(32, 2) },
                { "SpdTens",      new DisplaySegment(32, 1) },
                { "SpdOnes",      new DisplaySegment(32, 0) },
                { "HdgHundreds",  new DisplaySegment(33, 6, '-') },
                { "HdgTens",      new DisplaySegment(33, 5, '-') },
                { "HdgOnes",      new DisplaySegment(33, 4, '-') },
                { "AltTenthsds",  new DisplaySegment(33, 2, '{') },
                { "AltThousands", new DisplaySegment(33, 1, '}') },
                { "AltHundreds",  new DisplaySegment(33, 0, 'o') },
                { "AltTens",      new DisplaySegment(34, 7, 'b') },
                { "AltOnes",      new DisplaySegment(34, 6, 'l') },
                { "VsThousands",  new DisplaySegment(34, 3, '-') },
                { "VsHundreds",   new DisplaySegment(34, 2, '-') },
                { "VsTens",       new DisplaySegment(34, 1) },
                { "VsOnes",       new DisplaySegment(34, 0) },
                { "CoRHundreds",  new DisplaySegment(35, 6) },
                { "CoRTens",      new DisplaySegment(35, 5) },
                { "CoROnes",      new DisplaySegment(35, 4) },
                { "CoLDot",       new DisplaySegment(new Bit(4,5)) },
                { "IasLabel",     new DisplaySegment(new Bit(33,7)) },
                { "MachLabel",    new DisplaySegment(new Bit(29,7)) },
                { "SpdPlusVert",  new DisplaySegment(new Bit[] { new Bit(13,7), new Bit(9,7) }, false) },
                { "SpdPlusHoriz", new DisplaySegment(new Bit(8,3)) },
                { "MachDot",      new DisplaySegment(new Bit(4,2)) },
                { "HdgLabel",     new DisplaySegment(new Bit(33,3)) },
                { "TrkLabel",     new DisplaySegment(new Bit(25,3)) },
                { "HdgDot",       new DisplaySegment(new Bit(17,3)) },
                { "AltDot",       new DisplaySegment(new Bit(5,0)) },
                { "VsLabel",      new DisplaySegment(new Bit(35,7)) },
                { "FpaLabel",     new DisplaySegment(new Bit(31,7)) },
                { "VsPlusVert",   new DisplaySegment(new Bit[] { new Bit(23,7), new Bit(19,7) }, false) },
                { "VsPlusHoriz",  new DisplaySegment(new Bit(10,4, true)) },
                { "VsDot",        new DisplaySegment(new Bit(6,2)) },
                { "CoRDot",       new DisplaySegment(new Bit(7,4)) },
            };

            LedIdentifiers.Add("N1",       0x03);
            LedIdentifiers.Add("SPEED",    0x04);
            LedIdentifiers.Add("VNAV",     0x05);
            LedIdentifiers.Add("LVL_CHG",  0x06);
            LedIdentifiers.Add("HDG_SEL",  0x07);
            LedIdentifiers.Add("LNAV",     0x08);
            LedIdentifiers.Add("VOR_LOC",  0x09);
            LedIdentifiers.Add("APP",      0x0a);
            LedIdentifiers.Add("ALT_HLD",  0x0b);
            LedIdentifiers.Add("VS",       0x0c);
            LedIdentifiers.Add("A_CMD",    0x0d);
            LedIdentifiers.Add("A_CWS",    0x0e);
            LedIdentifiers.Add("B_CMD",    0x0f);
            LedIdentifiers.Add("B_CWS",    0x10);
            LedIdentifiers.Add("AT_ARM",   0x11);
            LedIdentifiers.Add("L_MA",     0x12);
            LedIdentifiers.Add("R_MA",     0x13);
            LedIdentifiers.Add("Solenoid", 0x1e);

            DisplayNameToActionMapping.Add(COURSE_LEFT,  SetCourseLeft);
            DisplayNameToActionMapping.Add(COURSE_RIGHT, SetCourseRight);
            DisplayNameToActionMapping.Add(COL_SHOWN,    SetCourseLeftShown);
            DisplayNameToActionMapping.Add(COR_SHOWN,    SetCourseRightShown);

            DisplayNameToActionMapping.Add(SPEED,        SetSpeed);
            DisplayNameToActionMapping.Add(MACH,         SetMachSpeed);
            DisplayNameToActionMapping.Add(SPEED_SHOWN,  SetSpeedShown);
            DisplayNameToActionMapping.Add(IAS_LABEL,    SetIasLabel);
            DisplayNameToActionMapping.Add(MACH_LABEL,   SetMachLabel);
            DisplayNameToActionMapping.Add(SPEED_A,      SetSpeedA);
            DisplayNameToActionMapping.Add(SPEED_B,      SetSpeedB);

            DisplayNameToActionMapping.Add(HEADING,   SetHeading);
            DisplayNameToActionMapping.Add(TRK,       SetTrack);
            DisplayNameToActionMapping.Add(HDG_LABEL, SetHdgLabel);
            DisplayNameToActionMapping.Add(TRK_LABEL, SetTrkLabel);

            DisplayNameToActionMapping.Add(ALTITUDE,  SetAltitude);

            DisplayNameToActionMapping.Add(VS,        SetVs);
            DisplayNameToActionMapping.Add(FPA,       SetFpa);
            DisplayNameToActionMapping.Add(VS_SHOWN,  SetVsShown);
            DisplayNameToActionMapping.Add(VS_LABEL,  SetVsLabel);
            DisplayNameToActionMapping.Add(FPA_LABEL, SetFpaLabel);

            DisplayNameToActionMapping.Add(ANN_LIGHT,        SetPap3AnnunciatorLightOnOff);
            DisplayNameToActionMapping.Add(BACK_BRIGHTNESS,  BrightnessFromString(0x00));
            DisplayNameToActionMapping.Add(LCD_BRIGHTNESS,   BrightnessFromString(0x01));
            DisplayNameToActionMapping.Add(LED_BRIGHTNESS,   BrightnessFromString(0x02));

            InitializeCaches();
            PrepareCommands();
        }

        public override void Connect()
        {
            SendValues();
            InvokeDisplayBrightness(BACK_BRIGHTNESS, 50);
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 100);
        }

        public override void Shutdown()
        {
            // PAP3 EmptyDisplay calls LcdTest("AllOff").
            LcdTest("AllOff");
            InvokeDisplayBrightness(BACK_BRIGHTNESS, 0);
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 0);
            TurnOffAllLEDs();
        }

        private void SetPap3AnnunciatorLightOnOff(string annLight)
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

        private void SetBoolInternal(string isSetString, string segmentName)
        {
            SetSegmentBool(segmentName, AsBool(isSetString));
            SendValues();
        }

        private void SetDigitsInternal(char[] chars, params string[] segmentNames)
        {
            SetDigits(chars, segmentNames);
            SendValues();
        }

        private void SetCourseLeft(string course)
        {
            char[] chars;
            if (IsCourseLeftShown)
            {
                int courseInt = AsInt(course);
                chars = courseInt.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                chars = new char[] { '*', '*', '*' };
            }
            SetDigitsInternal(chars, "CoLHundreds", "CoLTens", "CoLOnes");
        }

        private void SetCourseRight(string course)
        {
            char[] chars;
            if (IsCourseRightShown)
            {
                int courseInt = AsInt(course);
                chars = courseInt.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            }
            else
            {
                chars = new char[] { '*', '*', '*' };
            }
            SetDigitsInternal(chars, "CoRHundreds", "CoRTens", "CoROnes");
        }

        private void SetCourseLeftShown(string isShown)
        {
            IsCourseLeftShown = AsBool(isShown);
            ClearLcdCache(COURSE_LEFT);
        }

        private void SetCourseRightShown(string isShown)
        {
            IsCourseRightShown = AsBool(isShown);
            ClearLcdCache(COURSE_RIGHT);
        }

        private void SetMachDot(bool isDotSet)
        {
            SetSegmentBool("MachDot", isDotSet);
        }

        private void RefreshOnMachModeChange()
        {
            var spdThousands = DisplaySetValueSegments["SpdThousands"];
            spdThousands.SetCharacter('*');
            ApplySegment(spdThousands, SetValuesCommand);
            var spdHundreds = DisplaySetValueSegments["SpdHundreds"];
            spdHundreds.SetCharacter('*');
            ApplySegment(spdHundreds, SetValuesCommand);
            ClearLcdCache(SPEED_A, SPEED_B);
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

            int value = AsInt(speed);
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

            SetDigitsInternal(chars, "SpdHundreds", "SpdTens", "SpdOnes");
            ClearLcdCache(MACH);
        }

        private void SetMachSpeed(string speed)
        {
            var machDot = DisplaySetValueSegments["MachDot"];
            bool isMachModeChange = machDot.Bits[0].Value == false;
            SetMachDot(true); // update beforehand!
            if (isMachModeChange)
            {
                RefreshOnMachModeChange();
            }

            int value = (int)(AsDouble(speed) * 100);
            char[] chars;

            if (IsSpeedShown)
            {
                if (value == 999)
                {
                    chars = new char[] { '-', '-', '-' };
                    SetDigitsInternal(chars, "SpdHundreds", "SpdTens", "SpdOnes");
                }
                else if (IsSpeedA || IsSpeedB)
                {
                    // A or B is shown at the hundreds position
                    chars = value.ToString("D2", CultureInfo.InvariantCulture).ToCharArray();
                    SetDigitsInternal(chars, "SpdTens", "SpdOnes");
                }
                else
                {
                    chars = value.ToString("D2", CultureInfo.InvariantCulture).PadLeft(3, '*').ToCharArray();
                    SetDigitsInternal(chars, "SpdHundreds", "SpdTens", "SpdOnes");
                }
            }
            else
            {
                SetMachDot(false);
                chars = new char[] { '*', '*', '*' };
                SetDigitsInternal(chars, "SpdHundreds", "SpdTens", "SpdOnes");
            }

            ClearLcdCache(SPEED);
        }

        private void SetSpeedShown(string isShown)
        {
            IsSpeedShown = AsBool(isShown);
            ClearLcdCache(MACH, SPEED, SPEED_A, SPEED_B);
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
            bool isA = AsBool(isSpeedA);
            IsSpeedA = isA;

            if (IsSpeedShown)
            {
                var machDot = DisplaySetValueSegments["MachDot"];
                string segmentName = machDot.Bits[0].Value ? "SpdHundreds" : "SpdThousands";

                if (isA)
                {
                    SetDigitsInternal(new char[] { 'A' }, segmentName);
                }
                else
                {
                    SetDigitsInternal(new char[] { '*' }, segmentName);
                }
            }
            else
            {
                SetDigitsInternal(new char[] { '*', '*' }, "SpdThousands", "SpdHundreds");
            }
        }

        private void SetSpeedB(string isSpeedB)
        {
            bool isB = AsBool(isSpeedB);
            IsSpeedB = isB;

            if (IsSpeedShown)
            {
                var machDot = DisplaySetValueSegments["MachDot"];
                string segmentName = machDot.Bits[0].Value ? "SpdHundreds" : "SpdThousands";

                if (isB)
                {
                    SetDigitsInternal(new char[] { 'B' }, segmentName);
                }
                else
                {
                    SetDigitsInternal(new char[] { '*' }, segmentName);
                }
            }
            else
            {
                SetDigitsInternal(new char[] { '*', '*' }, "SpdThousands", "SpdHundreds");
            }
        }

        private void SetTrack(string track)
        {
            int value = AsInt(track);
            char[] chars = value.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            if (value == 999) chars = new char[] { '-', '-', '-' };
            SetDigitsInternal(chars, "HdgHundreds", "HdgTens", "HdgOnes");
            ClearLcdCache(HEADING);
        }

        private void SetHeading(string heading)
        {
            int value = AsInt(heading);
            char[] chars = value.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            if (value == 999) chars = new char[] { '-', '-', '-' };
            SetDigitsInternal(chars, "HdgHundreds", "HdgTens", "HdgOnes");
            ClearLcdCache(TRK);
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
            int value = AsInt(altitude);
            char[] chars;
            if (value == 0)
            {
                chars = new char[] { '*', '0', '0', '0', '0' };
            }
            else
            {
                chars = value.ToString().PadLeft(5, '*').ToCharArray();
            }

            SetDigitsInternal(chars, "AltTenthsds", "AltThousands", "AltHundreds", "AltTens", "AltOnes");
        }

        private void SetVsDot(bool isDotSet)
        {
            SetSegmentBool("VsDot", isDotSet);
        }

        private void SetVsSign(bool isPlus, bool isMinus)
        {
            var vsPlusHoriz = DisplaySetValueSegments["VsPlusHoriz"];
            var vsPlusVert  = DisplaySetValueSegments["VsPlusVert"];

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

            ApplySegment(vsPlusHoriz, SetValuesCommand);
            ApplySegment(vsPlusVert,  SetValuesCommand);
        }

        private void SetVs(string vs)
        {
            int value = AsInt(vs);
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

            SetDigitsInternal(chars, "VsThousands", "VsHundreds", "VsTens", "VsOnes");
            ClearLcdCache(FPA);
        }

        private void SetFpa(string vs)
        {
            int value = (int)(AsDouble(vs) * 10);
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
            SetDigitsInternal(chars, "VsThousands", "VsHundreds", "VsTens", "VsOnes");
            ClearLcdCache(VS);
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
            IsVsShown = AsBool(isShown);
            ClearLcdCache(VS, FPA);
        }
    }
}
