using System;
using System.Collections.Generic;
using System.Globalization;

namespace MobiFlightWwFcu
{
    internal class WinwingFcuDevice : SegmentDisplayDeviceBase
    {
        public override string Name => "WinWing FCU";

        protected override string SetValuesHeaderKey => "0201";

        private const string SPEED          = "Speed Value";
        private const string MACH           = "Mach Value";
        private const string MACH_MODE      = "Mach Mode On/Off";
        private const string SPEED_DASHES   = "Speed Dashes On/Off";
        private const string SPEED_DOT      = "Speed Dot";
        private const string HEADING        = "Heading Value";
        private const string TRK            = "TRK Value";
        private const string HEADING_DASHES = "Heading Dashes On/Off";
        private const string HEADING_DOT    = "Heading Dot";
        private const string ALTITUDE       = "Altitude Value";
        private const string ALTITUDE_DOT   = "Altitude Dot";
        private const string VS             = "VS Value";
        private const string FPA            = "FPA Value";
        private const string VS_DASHES      = "VS Dashes On/Off";
        private const string TRK_MODE       = "TRK Mode On/Off";

        public WinwingFcuDevice(IWinwingMessageSender sender)
            : base(sender, WinwingConstants.DEST_FCU, 0x31)
        {
            DisplayTestCommands = new Dictionary<string, DisplaySegment>()
            {
                { "AllOn",   new DisplaySegment(new Bit[] { new Bit(0,0,false), new Bit(0,1,true),  new Bit(0,2,false), new Bit(0,3,false) }, false) },
                { "AllOff",  new DisplaySegment(new Bit[] { new Bit(0,0,false), new Bit(0,1,true),  new Bit(0,2,true),  new Bit(0,3,false) }, false) },
                { "Half1On", new DisplaySegment(new Bit[] { new Bit(0,0,true),  new Bit(0,1,true),  new Bit(0,2,true),  new Bit(0,3,false) }, false) },
                { "Half2On", new DisplaySegment(new Bit[] { new Bit(0,0,true),  new Bit(0,1,false), new Bit(0,2,false), new Bit(0,3,true)  }, false) },
            };

            BuildDisplaySetValueSegments();

            LedIdentifiers.Add("LOC",   0x03);
            LedIdentifiers.Add("AP1",   0x05);
            LedIdentifiers.Add("AP2",   0x07);
            LedIdentifiers.Add("ATHR",  0x09);
            LedIdentifiers.Add("APPR",  0x0D);
            LedIdentifiers.Add("EXPED", 0x0b);

            DisplayNameToActionMapping.Add(SPEED,           SetSpeed);
            DisplayNameToActionMapping.Add(MACH,            SetMachSpeed);
            DisplayNameToActionMapping.Add(MACH_MODE,       SetMachModeOnOff);
            DisplayNameToActionMapping.Add(SPEED_DASHES,    SetSpeedDashes);
            DisplayNameToActionMapping.Add(SPEED_DOT,       SetSpeedDotOnOff);
            DisplayNameToActionMapping.Add(HEADING,         SetHeading);
            DisplayNameToActionMapping.Add(TRK,             SetTrack);
            DisplayNameToActionMapping.Add(HEADING_DASHES,  SetHeadingDashes);
            DisplayNameToActionMapping.Add(HEADING_DOT,     SetHeadingDotOnOff);
            DisplayNameToActionMapping.Add(ALTITUDE,        SetAltitude);
            DisplayNameToActionMapping.Add(ALTITUDE_DOT,    SetAltitudeDotOnOff);
            DisplayNameToActionMapping.Add(VS,              SetVs);
            DisplayNameToActionMapping.Add(FPA,             SetFpa);
            DisplayNameToActionMapping.Add(VS_DASHES,       SetVSDashes);
            DisplayNameToActionMapping.Add(TRK_MODE,        SetTrackFpaModeOnOff);
            DisplayNameToActionMapping.Add(ANN_LIGHT,       SetFcuAnnunciatorLightOnOff);
            DisplayNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            DisplayNameToActionMapping.Add(LCD_BRIGHTNESS,  BrightnessFromString(0x01));
            DisplayNameToActionMapping.Add(LED_BRIGHTNESS,  BrightnessFromString(0x11));

            InitializeCaches();
            PrepareCommands();
        }

        // 7-segment digit packed within a single FCU "speed" byte (Bits 7..1).
        // Bit-order in the constructor must match the segment order [T, TR, BR, B, BL, TL, M].
        private static DisplaySegment SpeedDigit(int dataByte, char init)
        {
            var seg = new DisplaySegment(new Bit[] {
                new Bit(dataByte, 7), // T
                new Bit(dataByte, 6), // TR
                new Bit(dataByte, 5), // BR
                new Bit(dataByte, 4), // B
                new Bit(dataByte, 1), // BL
                new Bit(dataByte, 3), // TL
                new Bit(dataByte, 2), // M
            }, true);
            seg.SetCharacter(init);
            return seg;
        }

        // 7-segment digit straddling two FCU "general" bytes:
        // upper nibble of byte N (Bits 7,6,5) holds TL, M, BL;
        // lower nibble of byte N+1 (Bits 3,2,1,0) holds T, TR, BR, B.
        private static DisplaySegment GeneralDigit(int dataByteN, char init)
        {
            var seg = new DisplaySegment(new Bit[] {
                new Bit(dataByteN + 1, 3), // T
                new Bit(dataByteN + 1, 2), // TR
                new Bit(dataByteN + 1, 1), // BR
                new Bit(dataByteN + 1, 0), // B
                new Bit(dataByteN,     5), // BL
                new Bit(dataByteN,     7), // TL
                new Bit(dataByteN,     6), // M
            }, true);
            seg.SetCharacter(init);
            return seg;
        }

        private void BuildDisplaySetValueSegments()
        {
            // Element bit positions are byte-numbers in the data section (header offset 17 is added when writing).
            // Iteration order matters: overlapping mode-presets resolve last-writer-wins, so the entry that
            // should be active at init must come last within its bit-group.
            DisplaySetValueSegments = new Dictionary<string, DisplaySegment>
            {
                // Speed
                { "SpeedHundreds",  SpeedDigit(4, '1') },
                { "SpeedTens",      SpeedDigit(5, '0') },
                { "SpeedOnes",      SpeedDigit(6, '0') },
                { "MachDecPoint",   new DisplaySegment(new Bit(5, 0, false)) },
                { "NoLabel",        new DisplaySegment(new Bit[] { new Bit(7,3,false), new Bit(7,2,false) }, false) },
                { "MachLabel",      new DisplaySegment(new Bit[] { new Bit(7,3,false), new Bit(7,2,true)  }, false) },
                { "SpeedLabel",     new DisplaySegment(new Bit[] { new Bit(7,3,true),  new Bit(7,2,false) }, false) },
                { "SpeedDot",       new DisplaySegment(new Bit[] { new Bit(7,1,true),  new Bit(7,0,false) }, false) },
                { "SpeedNoDot",     new DisplaySegment(new Bit[] { new Bit(7,1,false), new Bit(7,0,true)  }, false) },

                // Heading
                { "HdgHundreds",    GeneralDigit(7,  '0') },
                { "HdgTens",        GeneralDigit(8,  '0') },
                { "HdgOnes",        GeneralDigit(9,  '0') },
                { "HdgDot",         new DisplaySegment(new Bit(10, 4, false)) },
                { "NoLateralMode",  new DisplaySegment(new Bit[] { new Bit(10,7,false), new Bit(10,6,false), new Bit(10,5,false), new Bit(11,3,false), new Bit(11,1,false) }, false) },
                { "TrackMode",      new DisplaySegment(new Bit[] { new Bit(10,7,false), new Bit(10,6,true),  new Bit(10,5,true),  new Bit(11,3,false), new Bit(11,1,true)  }, false) },
                { "HeadingMode",    new DisplaySegment(new Bit[] { new Bit(10,7,true),  new Bit(10,6,false), new Bit(10,5,true),  new Bit(11,3,true),  new Bit(11,1,false) }, false) },
                { "NoVertMode1",    new DisplaySegment(new Bit[] { new Bit(11,2,false), new Bit(11,0,false) }, false) },
                { "FpaMode1",       new DisplaySegment(new Bit[] { new Bit(11,2,false), new Bit(11,0,true)  }, false) },
                { "VsMode1",        new DisplaySegment(new Bit[] { new Bit(11,2,true),  new Bit(11,0,false) }, false) },

                // Altitude
                { "Alt",            new DisplaySegment(new Bit(12, 4, true)) },
                { "LvlCh",          new DisplaySegment(new Bit[] { new Bit(13,4,true), new Bit(14,4,true), new Bit(15,4,true) }, false) },
                { "AltTenthsds",    GeneralDigit(11, '0') },
                { "AltThousands",   GeneralDigit(12, '0') },
                { "AltHundreds",    GeneralDigit(13, '1') },
                { "AltTens",        GeneralDigit(14, '0') },
                { "AltOnes",        GeneralDigit(15, '0') },

                // VS / FPA
                { "VsSignHoriz",    new DisplaySegment(new Bit(16, 4, true)) },
                { "VsSignVert",     new DisplaySegment(new Bit(18, 4, true)) },
                { "FpaDecPoint",    new DisplaySegment(new Bit(17, 4, false)) },
                { "VsThousands",    GeneralDigit(16, '0') },
                { "VsHundreds",     GeneralDigit(17, '0') },
                { "VsTens",         GeneralDigit(18, 'o') },
                { "VsOnes",         GeneralDigit(19, 'o') },
                { "AltDot",         new DisplaySegment(new Bit(19, 4, false)) },
                { "NoVertMode2",    new DisplaySegment(new Bit[] { new Bit(20,7,false), new Bit(20,6,false), new Bit(20,5,false) }, false) },
                { "FpaMode2",       new DisplaySegment(new Bit[] { new Bit(20,7,true),  new Bit(20,6,false), new Bit(20,5,false) }, false) },
                { "VsMode2",        new DisplaySegment(new Bit[] { new Bit(20,7,false), new Bit(20,6,true),  new Bit(20,5,false) }, false) },
            };
        }

        public override void Connect()
        {
            SendValues();
            SetBacklightBrightness("20");
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 100);
        }

        public override void Shutdown()
        {
            EmptyDisplay();
            SetBacklightBrightness("0");
            InvokeDisplayBrightness(LCD_BRIGHTNESS, 0);
            TurnOffAllLEDs();
        }

        private void SetBacklightBrightness(string brightness)
        {
            // Backlight controls two channels: backlight (0x00) and EXPED (0x1e).
            byte b = (byte)AsDouble(brightness);
            MessageSender.SetBrightness(DestinationAddress, 0x00, b);
            MessageSender.SetBrightness(DestinationAddress, 0x1e, b);
        }

        private void EmptyDisplay()
        {
            // Zero the data section bytes (absolute 21..38 = data 4..21).
            for (int i = HeaderOffset + 4; i <= HeaderOffset + 21; i++)
            {
                SetValuesCommand[i] = 0;
            }
            SendValues();
        }

        private void SetFcuAnnunciatorLightOnOff(string annLight)
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

        private void ResetSpeedCache()    => ClearLcdCache(SPEED, MACH);
        private void ResetHeadingCache()  => ClearLcdCache(HEADING, TRK);
        private void ResetVSCache()       => ClearLcdCache(VS, FPA);

        private void SetSpeedInternal(char[] speedChars)
        {
            SetDigits(speedChars, "SpeedHundreds", "SpeedTens", "SpeedOnes");
            SendValues();
        }

        private void SetSpeed(string speed)
        {
            int mySpeed = AsInt(speed);
            char[] speedChars = mySpeed.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetSpeedInternal(speedChars);
        }

        private void SetMachSpeed(string speed)
        {
            int mySpeed = (int)(AsDouble(speed) * 100);
            char[] speedChars = mySpeed.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetSpeedInternal(speedChars);
        }

        private void SetSpeedDotOnOff(string speedDot)
        {
            int myDot = AsInt(speedDot);
            var preset = (myDot == 0) ? DisplaySetValueSegments["SpeedNoDot"] : DisplaySetValueSegments["SpeedDot"];
            ApplySegment(preset, SetValuesCommand);
            SendValues();
        }

        private void SetSpeedDashes(string speedDashes)
        {
            int myDashes = AsInt(speedDashes);
            if (myDashes == 1)
            {
                SetSpeedInternal(new char[] { '-', '-', '-' });
            }
            else if (myDashes == 0)
            {
                ResetSpeedCache();
            }
        }

        private void SetMachModeOnOff(string machMode)
        {
            int myMachMode = AsInt(machMode);
            var machDecPoint = DisplaySetValueSegments["MachDecPoint"];
            if (myMachMode == 1)
            {
                ApplySegment(DisplaySetValueSegments["MachLabel"], SetValuesCommand);
                machDecPoint.SetValue(true);
                ApplySegment(machDecPoint, SetValuesCommand);
            }
            else if (myMachMode == 0)
            {
                ApplySegment(DisplaySetValueSegments["SpeedLabel"], SetValuesCommand);
                machDecPoint.SetValue(false);
                ApplySegment(machDecPoint, SetValuesCommand);
            }
            else if (myMachMode == 2)
            {
                ApplySegment(DisplaySetValueSegments["NoLabel"], SetValuesCommand);
                // MachDecPoint left untouched (matches previous behaviour).
            }
            ResetSpeedCache();
            SendValues();
        }

        private void SetHeadingInternal(char[] hdgChars)
        {
            SetDigits(hdgChars, "HdgHundreds", "HdgTens", "HdgOnes");
            SendValues();
        }

        private void SetTrack(string track)
        {
            int myHeading = AsInt(track);
            char[] hdgChars = myHeading.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetHeadingInternal(hdgChars);
        }

        private void SetHeading(string heading)
        {
            int myHeading = AsInt(heading);
            char[] hdgChars = myHeading.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetHeadingInternal(hdgChars);
        }

        private void SetHeadingDashes(string headingDashes)
        {
            int myDashes = AsInt(headingDashes);
            if (myDashes == 1)
            {
                SetHeadingInternal(new char[] { '-', '-', '-' });
            }
            else if (myDashes == 0)
            {
                ResetHeadingCache();
            }
        }

        private void SetHeadingDotOnOff(string headingDot)
        {
            SetSegmentBool("HdgDot", AsBool(headingDot));
            SendValues();
        }

        private void SetTrackFpaModeOnOff(string trackFpaMode)
        {
            int myTrackFpaMode = AsInt(trackFpaMode);
            if (myTrackFpaMode == 1)
            {
                ApplySegment(DisplaySetValueSegments["TrackMode"], SetValuesCommand);
                ApplySegment(DisplaySetValueSegments["FpaMode1"],  SetValuesCommand);
                ApplySegment(DisplaySetValueSegments["FpaMode2"],  SetValuesCommand);
            }
            else if (myTrackFpaMode == 0)
            {
                ApplySegment(DisplaySetValueSegments["HeadingMode"], SetValuesCommand);
                ApplySegment(DisplaySetValueSegments["VsMode1"],     SetValuesCommand);
                ApplySegment(DisplaySetValueSegments["VsMode2"],     SetValuesCommand);
            }
            else if (myTrackFpaMode == 2)
            {
                ApplySegment(DisplaySetValueSegments["NoLateralMode"], SetValuesCommand);
                ApplySegment(DisplaySetValueSegments["NoVertMode1"],   SetValuesCommand);
                ApplySegment(DisplaySetValueSegments["NoVertMode2"],   SetValuesCommand);
            }
            else if (trackFpaMode.Length == 3)
            {
                char tens = trackFpaMode[1];
                char ones = trackFpaMode[2];
                if (tens == '0')
                {
                    ApplySegment(DisplaySetValueSegments["HeadingMode"], SetValuesCommand);
                }
                else if (tens == '1')
                {
                    ApplySegment(DisplaySetValueSegments["TrackMode"], SetValuesCommand);
                }
                else if (tens == '2')
                {
                    ApplySegment(DisplaySetValueSegments["NoLateralMode"], SetValuesCommand);
                }

                if (ones == '0')
                {
                    ApplySegment(DisplaySetValueSegments["VsMode1"], SetValuesCommand);
                    ApplySegment(DisplaySetValueSegments["VsMode2"], SetValuesCommand);
                }
                else if (ones == '1')
                {
                    ApplySegment(DisplaySetValueSegments["FpaMode1"], SetValuesCommand);
                    ApplySegment(DisplaySetValueSegments["FpaMode2"], SetValuesCommand);
                }
                else if (ones == '2')
                {
                    ApplySegment(DisplaySetValueSegments["NoVertMode1"], SetValuesCommand);
                    ApplySegment(DisplaySetValueSegments["NoVertMode2"], SetValuesCommand);
                }
            }

            ResetHeadingCache();
            ResetVSCache();
            SendValues();
        }

        private void SetAltitude(string altitude)
        {
            int myAlt = AsInt(altitude);
            char[] altChars = myAlt.ToString("D5", CultureInfo.InvariantCulture).ToCharArray();
            SetDigits(altChars, "AltTenthsds", "AltThousands", "AltHundreds", "AltTens", "AltOnes");
            SendValues();
        }

        private void SetAltitudeDotOnOff(string altitudeDot)
        {
            int myDot = AsInt(altitudeDot);
            bool altOn = (myDot == 0 || myDot == 1);
            bool dotOn = (myDot == 1 || myDot == 21);

            SetSegmentBool("Alt",    altOn);
            SetSegmentBool("AltDot", dotOn);

            SendValues();
        }

        private void SetFpa(string vs)
        {
            int myFpa = (int)(AsDouble(vs) * 10);
            string stringFpa = Math.Abs(myFpa).ToString("D2", CultureInfo.InvariantCulture) + "**";
            char[] fpaChars = stringFpa.ToCharArray();
            SetVSInternal(fpaChars, (myFpa < 0), true);
        }

        private void SetVs(string vs)
        {
            int myVs = AsInt(vs);
            char[] vsChars = Math.Abs(myVs).ToString("D4", CultureInfo.InvariantCulture).ToCharArray();
            if (vsChars[2] == '0' && vsChars[3] == '0')
            {
                // Do airbus style and set the last two digits to 'o'
                vsChars[2] = 'o';
                vsChars[3] = 'o';
            }
            SetVSInternal(vsChars, (myVs < 0), false);
        }

        private void SetVSInternal(char[] vsChars, bool isMinus, bool isFpa)
        {
            var horiz = DisplaySetValueSegments["VsSignHoriz"];
            var vert  = DisplaySetValueSegments["VsSignVert"];
            horiz.SetValue(true);    // visible sign → horizontal stroke always lit
            vert.SetValue(!isMinus); // vertical stroke only for '+'
            ApplySegment(horiz, SetValuesCommand);
            ApplySegment(vert,  SetValuesCommand);

            SetSegmentBool("FpaDecPoint", isFpa);

            SetDigits(vsChars, "VsThousands", "VsHundreds", "VsTens", "VsOnes");
            SendValues();
        }

        private void SetVSDashes(string vsDashes)
        {
            int myDashes = AsInt(vsDashes);
            var lvlCh = DisplaySetValueSegments["LvlCh"];

            if (myDashes == 1)
            {
                lvlCh.SetValue(true);
                ApplySegment(lvlCh, SetValuesCommand);
                SetVSInternal(new char[] { '-', '-', '-', '-' }, true, false);
            }
            else if (myDashes == 0)
            {
                lvlCh.SetValue(true);
                ApplySegment(lvlCh, SetValuesCommand);
                ResetVSCache();
                SendValues();
            }
            else if (myDashes == 21)
            {
                lvlCh.SetValue(false);
                ApplySegment(lvlCh, SetValuesCommand);
                SetVSInternal(new char[] { '-', '-', '-', '-' }, true, false);
            }
            else if (myDashes == 20)
            {
                lvlCh.SetValue(false);
                ApplySegment(lvlCh, SetValuesCommand);
                ResetVSCache();
                SendValues();
            }
        }
    }
}
