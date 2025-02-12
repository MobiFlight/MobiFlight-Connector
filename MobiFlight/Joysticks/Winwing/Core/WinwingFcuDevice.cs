using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MobiFlightWwFcu;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingFcuDevice : IWinwingDevice
    {
        public string Name { get; } = "WinWing FCU";

        private WinwingMessageSender MessageSender = null;

        private byte[] DestinationAddress = WinwingConstants.DEST_FCU;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();

        private const string SPEED = "Speed Value";
        private const string MACH = "Mach Value";
        private const string MACH_MODE = "Mach Mode On/Off";
        private const string SPEED_DASHES = "Speed Dashes On/Off";
        private const string SPEED_DOT = "Speed Dot";
        private const string HEADING = "Heading Value";
        private const string TRK = "TRK Value";
        private const string HEADING_DASHES = "Heading Dashes On/Off";
        private const string HEADING_DOT = "Heading Dot";
        private const string ALTITUDE = "Altitude Value";
        private const string ALTITUDE_DOT = "Altitude Dot";
        private const string VS = "VS Value";
        private const string FPA = "FPA Value";
        private const string VS_DASHES = "VS Dashes On/Off";
        private const string TRK_MODE = "TRK Mode On/Off";
        private const string ANN_LIGHT = "LCD Test On/Off";
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";
        private const string LED_BRIGHTNESS = "LED Percentage";


        private Dictionary<string, MsgEntry> DisplayTestCommands = new Dictionary<string, MsgEntry>()
        {
            { "AllOn",          new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x02 } } },
            { "AllOff",         new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x06 } } },
            { "Half1On",        new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x07 } } },
            { "Half2On",        new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x09 } } },
        };

        private Dictionary<char, byte[]> SpeedNumberCodes = new Dictionary<char, byte[]>()
        {
            { '*', new byte[] { 0x00 } },
            { '-', new byte[] { 0x04 } },
            { 'o', new byte[] { 0x36 } },
            { '0', new byte[] { 0xfa } },
            { '1', new byte[] { 0x60 } },
            { '2', new byte[] { 0xd6 } },
            { '3', new byte[] { 0xf4 } },
            { '4', new byte[] { 0x6c } },
            { '5', new byte[] { 0xbc } },
            { '6', new byte[] { 0xbe } },
            { '7', new byte[] { 0xe0 } },
            { '8', new byte[] { 0xfe } },
            { '9', new byte[] { 0xfc } },
        };

        private Dictionary<char, byte[]> GeneralNumberCodes = new Dictionary<char, byte[]>()
        {
            { '*', new byte[] { 0x00, 0x00 } },
            { '-', new byte[] { 0x40, 0x00 } },
            { 'o', new byte[] { 0x60, 0x03 } },
            { '0', new byte[] { 0xa0, 0x0f } },
            { '1', new byte[] { 0x00, 0x06 } },
            { '2', new byte[] { 0x60, 0x0d } },
            { '3', new byte[] { 0x40, 0x0f } },
            { '4', new byte[] { 0xc0, 0x06 } },
            { '5', new byte[] { 0xc0, 0x0b } },
            { '6', new byte[] { 0xe0, 0x0b } },
            { '7', new byte[] { 0x00, 0x0e } },
            { '8', new byte[] { 0xe0, 0x0f } },
            { '9', new byte[] { 0xc0, 0x0f } },
        };


        private Dictionary<string, MsgEntry> DisplaySetValuesData = new Dictionary<string, MsgEntry>()
        {
            { "SpeedHundreds",  new MsgEntry { StartPos = 21, Mask = new byte[] { 0b00000001 }, Data = new byte[] { 0x60 } } },
            { "SpeedTens",      new MsgEntry { StartPos = 22, Mask = new byte[] { 0b00000001 }, Data = new byte[] { 0xfa } } },
            { "SpeedOnes",      new MsgEntry { StartPos = 23, Mask = new byte[] { 0b00000001 }, Data = new byte[] { 0xfa } } },
            { "MachDecPoint",   new MsgEntry { StartPos = 22, Mask = new byte[] { 0b11111110 }, Data = new byte[] { 0b00000001 } } },
            { "MachNoDecPoint", new MsgEntry { StartPos = 22, Mask = new byte[] { 0b11111110 }, Data = new byte[] { 0b00000000 } } },
            { "NoLabel",        new MsgEntry { StartPos = 24, Mask = new byte[] { 0b11110011 }, Data = new byte[] { 0b00000000 } } },
            { "MachLabel",      new MsgEntry { StartPos = 24, Mask = new byte[] { 0b11110011 }, Data = new byte[] { 0b00000100 } } },
            { "SpeedLabel",     new MsgEntry { StartPos = 24, Mask = new byte[] { 0b11110011 }, Data = new byte[] { 0b00001000 } } },
            { "SpeedDot",       new MsgEntry { StartPos = 24, Mask = new byte[] { 0b11111100 }, Data = new byte[] { 0b00000010 } } },
            { "SpeedNoDot",     new MsgEntry { StartPos = 24, Mask = new byte[] { 0b11111100 }, Data = new byte[] { 0b00000001 } } },
            { "HdgHundreds",    new MsgEntry { StartPos = 24, Mask = new byte[] { 0b00001111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "HdgTens",        new MsgEntry { StartPos = 25, Mask = new byte[] { 0b00001111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "HdgOnes",        new MsgEntry { StartPos = 26, Mask = new byte[] { 0b00001111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "HdgDot",         new MsgEntry { StartPos = 27, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00010000 } } },
            { "HdgNoDot",       new MsgEntry { StartPos = 27, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00000000 } } },
            { "NoLateralMode",  new MsgEntry { StartPos = 27, Mask = new byte[] { 0b00011111, 0b11110101  }, Data = new byte[] { 0b00000000, 0b00000000 } } },
            { "TrackMode",      new MsgEntry { StartPos = 27, Mask = new byte[] { 0b00011111, 0b11110101  }, Data = new byte[] { 0b01100000, 0b00000010 } } },
            { "HeadingMode",    new MsgEntry { StartPos = 27, Mask = new byte[] { 0b00011111, 0b11110101  }, Data = new byte[] { 0b10100000, 0b00001000 } } },
            { "NoVertMode1",    new MsgEntry { StartPos = 28, Mask = new byte[] { 0b11111010  }, Data = new byte[] { 0b00000000 } } },
            { "FpaMode1",       new MsgEntry { StartPos = 28, Mask = new byte[] { 0b11111010  }, Data = new byte[] { 0b00000001 } } },
            { "VsMode1",        new MsgEntry { StartPos = 28, Mask = new byte[] { 0b11111010  }, Data = new byte[] { 0b00000100 } } },
            { "NoVertMode2",    new MsgEntry { StartPos = 37, Mask = new byte[] { 0b00011111 }, Data = new byte[] { 0b00000000 } } },
            { "FpaMode2",       new MsgEntry { StartPos = 37, Mask = new byte[] { 0b00011111 }, Data = new byte[] { 0b10000000 } } },
            { "VsMode2",        new MsgEntry { StartPos = 37, Mask = new byte[] { 0b00011111 }, Data = new byte[] { 0b01000000 } } },
            { "NoAlt",          new MsgEntry { StartPos = 29, Mask = new byte[] { 0xef }, Data = new byte[] { 0x00 } } },
            { "Alt",            new MsgEntry { StartPos = 29, Mask = new byte[] { 0xef }, Data = new byte[] { 0x10 } } },
            { "NoLvlCh",        new MsgEntry { StartPos = 30, Mask = new byte[] { 0xef, 0xef, 0xef }, Data = new byte[] { 0x00, 0x00, 0x00 } } },
            { "LvlCh",          new MsgEntry { StartPos = 30, Mask = new byte[] { 0xef, 0xef, 0xef }, Data = new byte[] { 0x10, 0x10, 0x10 } } },
            { "AltTenthsds",    new MsgEntry { StartPos = 28, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "AltThousands",   new MsgEntry { StartPos = 29, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "AltHundreds",    new MsgEntry { StartPos = 30, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0x00, 0x06 } } },
            { "AltTens",        new MsgEntry { StartPos = 31, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "AltOnes",        new MsgEntry { StartPos = 32, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "VsMinus",        new MsgEntry { StartPos = 33, Mask = new byte[] { 0b11101111, 0xff, 0b11101111 }, Data = new byte[] { 0b00010000, 0x00, 0b00000000 } } },
            { "VsPlus",         new MsgEntry { StartPos = 33, Mask = new byte[] { 0b11101111, 0xff, 0b11101111 }, Data = new byte[] { 0b00010000, 0x00, 0b00010000 } } },
            { "FpaDecPoint",    new MsgEntry { StartPos = 34, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00010000 } } },
            { "FpaNoDecPoint",  new MsgEntry { StartPos = 34, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00000000 } } },
            { "VsThousands",    new MsgEntry { StartPos = 33, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "VsHundreds",     new MsgEntry { StartPos = 34, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            // Intentionally set initial VS to '00oo'
            { "VsTens",         new MsgEntry { StartPos = 35, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0x60, 0x03 } } },
            { "VsOnes",         new MsgEntry { StartPos = 36, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0x60, 0x03 } } },
            { "AltDot",         new MsgEntry { StartPos = 36, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00010000 } } },
            { "AltNoDot",       new MsgEntry { StartPos = 36, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00000000 } } },
            { "ZeroEndBlock",   new MsgEntry { StartPos = 38, Mask = new byte[11], Data = new byte[11] } },
        };

        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {
            { "LOC", 0x03 },
            { "AP1", 0x05 },
            { "AP2", 0x07 },
            { "ATHR", 0x09 },
            { "APPR", 0x0D },
            { "EXPED", 0x0b }
        };

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();        

        private byte[] DisplayTestCommand = new byte[0x12];
        private byte[] RefreshCommand = new byte[0x11];
        private byte[] SetValuesCommand = new byte[0x31];   

        public WinwingFcuDevice(WinwingMessageSender sender)
        {
            MessageSender = sender;
            DisplayNameToActionMapping.Add(SPEED, SetSpeed);
            DisplayNameToActionMapping.Add(MACH, SetMachSpeed);
            DisplayNameToActionMapping.Add(MACH_MODE, SetMachModeOnOff);
            DisplayNameToActionMapping.Add(SPEED_DASHES, SetSpeedDashes);
            DisplayNameToActionMapping.Add(SPEED_DOT, SetSpeedDotOnOff);
            DisplayNameToActionMapping.Add(HEADING, SetHeading);
            DisplayNameToActionMapping.Add(TRK, SetTrack);
            DisplayNameToActionMapping.Add(HEADING_DASHES, SetHeadingDashes);
            DisplayNameToActionMapping.Add(HEADING_DOT, SetHeadingDotOnOff);
            DisplayNameToActionMapping.Add(ALTITUDE, SetAltitude);
            DisplayNameToActionMapping.Add(ALTITUDE_DOT, SetAltitudeDotOnOff);
            DisplayNameToActionMapping.Add(VS, SetVs);
            DisplayNameToActionMapping.Add(FPA, SetFpa);
            DisplayNameToActionMapping.Add(VS_DASHES, SetVSDashes);
            DisplayNameToActionMapping.Add(TRK_MODE, SetTrackFpaModeOnOff);
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
        
            var initSetValues = new List<byte>(DestinationAddress);
            initSetValues.AddRange(new byte[2]);
            initSetValues.AddRange(WinwingConstants.DisplayCmdHeaders["0201"]);
            initSetValues.CopyTo(SetValuesCommand, 0);

            var initRefresh = new List<byte>(DestinationAddress);
            initRefresh.AddRange(new byte[2]);
            initRefresh.AddRange(WinwingConstants.DisplayCmdHeaders["0301"]);
            initRefresh.CopyTo(RefreshCommand, 0);

            foreach (var entry in DisplaySetValuesData.Values)
            {
                SetBytesDisplayCommand(entry, SetValuesCommand);
            }
        }

        public void Connect()
        {            
            SendDisplayCommand(SetValuesCommand); // Init display
            SetBacklightBrightness("20");
            SetLcdBrightness("100");

            // LcdTest("AllOn"); // used for testing
        }

        public void Shutdown()
        {                
            EmptyDisplay();
            SetBacklightBrightness("0");
            SetLcdBrightness("0");
            foreach (var ledName in LedIdentifiers.Keys)
            {
                SetLed(ledName, 0);
            }         
        }

        public List<string> GetLedNames()
        {
            return LedIdentifiers.Keys.ToList();
        }

        public List<string> GetDisplayNames()
        {
            return DisplayNameToActionMapping.Keys.ToList();
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
                PrepareAndSendDisplayTestCommand(DisplayTestCommands["AllOn"]);
            }
            else
            {
                SendDisplayCommand(SetValuesCommand);
            }
        }
        private void SetLedBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x11, brightness);
        }

        private void SetBacklightBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);
            MessageSender.SetBrightness(DestinationAddress, 0x1e, brightness); // EXPED
        }

        private void SetLcdBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x01, brightness);
        }

        private void PrepareAndSendDisplayTestCommand(MsgEntry entry)
        {
            SetBytesDisplayCommand(entry, DisplayTestCommand);
            SendDisplayCommand(DisplayTestCommand);
        }

        private void EmptyDisplay()
        {
            var resetMsg = new MsgEntry { StartPos = 21, Mask = new byte[18], Data = new byte[18] };
            SetBytesDisplayCommand(resetMsg, SetValuesCommand);
            SendDisplayCommand(SetValuesCommand);
        }

        private void ResetSpeedCache()
        {
            LcdCurrentValuesCache[SPEED] = string.Empty;
            LcdCurrentValuesCache[MACH] = string.Empty;
        }

        private void ResetHeadingCache()
        {
            LcdCurrentValuesCache[HEADING] = string.Empty;
            LcdCurrentValuesCache[TRK] = string.Empty;
        }

        private void ResetVSCache()
        {
            LcdCurrentValuesCache[VS] = string.Empty;
            LcdCurrentValuesCache[FPA] = string.Empty;
        }

        private void SetSpeedInternal(char[] speedChars)
        {
            var speedHundreds = DisplaySetValuesData["SpeedHundreds"];
            var speedTens = DisplaySetValuesData["SpeedTens"];
            var speedOnes = DisplaySetValuesData["SpeedOnes"];
            speedHundreds.Data = SpeedNumberCodes[speedChars[0]];
            speedTens.Data = SpeedNumberCodes[speedChars[1]];
            speedOnes.Data = SpeedNumberCodes[speedChars[2]];

            SetBytesDisplayCommand(speedHundreds, SetValuesCommand);
            SetBytesDisplayCommand(speedTens, SetValuesCommand);
            SetBytesDisplayCommand(speedOnes, SetValuesCommand);

            SendDisplayCommand(SetValuesCommand);
        }

        private void SetSpeed(string speed)
        {
            int mySpeed = (int)Convert.ToDouble(speed, CultureInfo.InvariantCulture);
            char[] speedChars = mySpeed.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetSpeedInternal(speedChars);
        }

        private void SetMachSpeed(string speed)
        {
            int mySpeed = (int)(Convert.ToDouble(speed, CultureInfo.InvariantCulture) * 100);
            char[] speedChars = mySpeed.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetSpeedInternal(speedChars);
        }

        private void SetSpeedDotOnOff(string speedDot)
        {
            int myDot = (int)Convert.ToDouble(speedDot, CultureInfo.InvariantCulture);
            if (myDot == 0)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["SpeedNoDot"], SetValuesCommand);
            }
            else
            {
                SetBytesDisplayCommand(DisplaySetValuesData["SpeedDot"], SetValuesCommand);
            }
            SendDisplayCommand(SetValuesCommand);
        }

        private void SetSpeedDashes(string speedDashes)
        {
            int myDashes = (int)Convert.ToDouble(speedDashes, CultureInfo.InvariantCulture);            
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
            int myMachMode = (int)Convert.ToDouble(machMode, CultureInfo.InvariantCulture);
            if (myMachMode == 1)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["MachLabel"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["MachDecPoint"], SetValuesCommand);
            }
            else if (myMachMode == 0)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["SpeedLabel"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["MachNoDecPoint"], SetValuesCommand);
            }
            else if (myMachMode == 2)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["NoLabel"], SetValuesCommand);
            }
            ResetSpeedCache();
            SendDisplayCommand(SetValuesCommand);
        }


        private void SetHeadingInternal(char[] hdgChars)
        {
            var hdgHundreds = DisplaySetValuesData["HdgHundreds"];
            var hdgTens = DisplaySetValuesData["HdgTens"];
            var hdgOnes = DisplaySetValuesData["HdgOnes"];
            hdgHundreds.Data = GeneralNumberCodes[hdgChars[0]];
            hdgTens.Data = GeneralNumberCodes[hdgChars[1]];
            hdgOnes.Data = GeneralNumberCodes[hdgChars[2]];

            SetBytesDisplayCommand(hdgHundreds, SetValuesCommand);
            SetBytesDisplayCommand(hdgTens, SetValuesCommand);
            SetBytesDisplayCommand(hdgOnes, SetValuesCommand);

            SendDisplayCommand(SetValuesCommand);
        }

        private void SetTrack(string track)
        {
            int myHeading = (int)Convert.ToDouble(track, CultureInfo.InvariantCulture);
            char[] hdgChars = myHeading.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetHeadingInternal(hdgChars);
        }

        private void SetHeading(string heading)
        {
            int myHeading = (int)Convert.ToDouble(heading, CultureInfo.InvariantCulture);
            char[] hdgChars = myHeading.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetHeadingInternal(hdgChars);
        }


        private void SetHeadingDashes(string headingDashes)
        {
            int myDashes = (int)Convert.ToDouble(headingDashes, CultureInfo.InvariantCulture);
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
            int myDot = (int)Convert.ToDouble(headingDot, CultureInfo.InvariantCulture);
            if (myDot == 0)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["HdgNoDot"], SetValuesCommand);
            }
            else
            {
                SetBytesDisplayCommand(DisplaySetValuesData["HdgDot"], SetValuesCommand);
            }
            SendDisplayCommand(SetValuesCommand);
        }

        private void SetTrackFpaModeOnOff(string trackFpaMode)
        {
            int myTrackFpaMode = (int)Convert.ToDouble(trackFpaMode, CultureInfo.InvariantCulture);                        
            if (myTrackFpaMode == 1)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["TrackMode"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["FpaMode1"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["FpaMode2"], SetValuesCommand);
            }
            else if (myTrackFpaMode == 0)// myFpaMode == 0
            {
                SetBytesDisplayCommand(DisplaySetValuesData["HeadingMode"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["VsMode1"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["VsMode2"], SetValuesCommand);
            }
            else if (myTrackFpaMode == 2)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["NoLateralMode"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["NoVertMode1"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["NoVertMode2"], SetValuesCommand);
            }
            else if (trackFpaMode.Length == 3)
            {                              
                char tens = trackFpaMode[1];
                char ones = trackFpaMode[2];
                if (tens == '0') 
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["HeadingMode"], SetValuesCommand);
                }
                else if (tens == '1') 
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["TrackMode"], SetValuesCommand);
                }
                else if (tens == '2')
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["NoLateralMode"], SetValuesCommand);
                }

                if (ones == '0')
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["VsMode1"], SetValuesCommand);
                    SetBytesDisplayCommand(DisplaySetValuesData["VsMode2"], SetValuesCommand);
                }
                else if (ones == '1') 
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["FpaMode1"], SetValuesCommand);
                    SetBytesDisplayCommand(DisplaySetValuesData["FpaMode2"], SetValuesCommand);
                }
                else if (ones == '2')
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["NoVertMode1"], SetValuesCommand);
                    SetBytesDisplayCommand(DisplaySetValuesData["NoVertMode2"], SetValuesCommand);
                }
            }

            ResetHeadingCache();
            ResetVSCache();
            SendDisplayCommand(SetValuesCommand);
        }

        private void SetAltitude(string altitude)
        {
            int myAlt = (int)Convert.ToDouble(altitude, CultureInfo.InvariantCulture);
            char[] altChars = myAlt.ToString("D5", CultureInfo.InvariantCulture).ToCharArray();

            var altTenthsds = DisplaySetValuesData["AltTenthsds"];
            var altThousands = DisplaySetValuesData["AltThousands"];
            var altHundreds = DisplaySetValuesData["AltHundreds"];
            var altTens = DisplaySetValuesData["AltTens"];
            var altOnes = DisplaySetValuesData["AltOnes"];
            altTenthsds.Data = GeneralNumberCodes[altChars[0]];
            altThousands.Data = GeneralNumberCodes[altChars[1]];
            altHundreds.Data = GeneralNumberCodes[altChars[2]];
            altTens.Data = GeneralNumberCodes[altChars[3]];
            altOnes.Data = GeneralNumberCodes[altChars[4]];

            SetBytesDisplayCommand(altTenthsds, SetValuesCommand);
            SetBytesDisplayCommand(altThousands, SetValuesCommand);
            SetBytesDisplayCommand(altHundreds, SetValuesCommand);
            SetBytesDisplayCommand(altTens, SetValuesCommand);
            SetBytesDisplayCommand(altOnes, SetValuesCommand);

            SendDisplayCommand(SetValuesCommand);
        }

        private void SetAltitudeDotOnOff(string altitudeDot)
        {
            int myDot = (int)Convert.ToDouble(altitudeDot, CultureInfo.InvariantCulture);
            if (myDot == 0)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["Alt"], SetValuesCommand);                
                SetBytesDisplayCommand(DisplaySetValuesData["AltNoDot"], SetValuesCommand);
            }
            else if (myDot == 1)            
            {
                SetBytesDisplayCommand(DisplaySetValuesData["Alt"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["AltDot"], SetValuesCommand);
            }
            else if (myDot == 20)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["NoAlt"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["AltNoDot"], SetValuesCommand);
            }
            else if (myDot == 21)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["NoAlt"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["AltDot"], SetValuesCommand);
            }

            SendDisplayCommand(SetValuesCommand);
        }


        private void SetFpa(string vs)
        {
            int myFpa = (int)(Convert.ToDouble(vs, CultureInfo.InvariantCulture) * 10);
            string stringFpa = Math.Abs(myFpa).ToString("D2", CultureInfo.InvariantCulture) + "**";
            char[] fpaChars = stringFpa.ToCharArray();
            SetVSInternal(fpaChars, (myFpa < 0), true);
        }


        private void SetVs(string vs)
        {
            int myVs = (int)Convert.ToDouble(vs, CultureInfo.InvariantCulture);
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
            var vsThousands = DisplaySetValuesData["VsThousands"];
            var vsHundreds = DisplaySetValuesData["VsHundreds"];
            var vsTens = DisplaySetValuesData["VsTens"];
            var vsOnes = DisplaySetValuesData["VsOnes"];
            vsThousands.Data = GeneralNumberCodes[vsChars[0]];
            vsHundreds.Data = GeneralNumberCodes[vsChars[1]];
            vsTens.Data = GeneralNumberCodes[vsChars[2]];
            vsOnes.Data = GeneralNumberCodes[vsChars[3]];

            if (isMinus)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["VsMinus"], SetValuesCommand);
            }
            else
            {
                SetBytesDisplayCommand(DisplaySetValuesData["VsPlus"], SetValuesCommand);
            }

            if (isFpa)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["FpaDecPoint"], SetValuesCommand);
            }
            else
            {
                SetBytesDisplayCommand(DisplaySetValuesData["FpaNoDecPoint"], SetValuesCommand);
            }
            SetBytesDisplayCommand(vsThousands, SetValuesCommand);
            SetBytesDisplayCommand(vsHundreds, SetValuesCommand);
            SetBytesDisplayCommand(vsTens, SetValuesCommand);
            SetBytesDisplayCommand(vsOnes, SetValuesCommand);

            SendDisplayCommand(SetValuesCommand);

        }

        private void SetVSDashes(string vsDashes)
        {
            int myDashes = (int)Convert.ToDouble(vsDashes, CultureInfo.InvariantCulture);
            if (myDashes == 1)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["LvlCh"], SetValuesCommand);
                SetVSInternal(new char[] { '-', '-', '-', '-' }, true, false);                
            }
            else if (myDashes == 0)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["LvlCh"], SetValuesCommand);
                ResetVSCache();
                SendDisplayCommand(SetValuesCommand);
            }
            else if (myDashes == 21)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["NoLvlCh"], SetValuesCommand);
                SetVSInternal(new char[] { '-', '-', '-', '-' }, true, false);                            
            }
            else if (myDashes == 20)
            {
                SetBytesDisplayCommand(DisplaySetValuesData["NoLvlCh"], SetValuesCommand);
                ResetVSCache();
                SendDisplayCommand(SetValuesCommand);
            }
        }

        // "AllOn", "AllOff", "Half1On", "Half2On"        
        private void LcdTest(string command)
        {
            PrepareAndSendDisplayTestCommand(DisplayTestCommands[command]);
        }


        private void SendDisplayCommand(byte[] message)
        {        
            MessageSender.SendDisplayCommands(new byte[][] { message, RefreshCommand });
        }

        private void SetBytesDisplayCommand(MsgEntry msgEntry, byte[] message)
        {
            byte setPos = msgEntry.StartPos;
            for (int i = 0; i < msgEntry.Data.Length; i++)
            {
                message[setPos] &= msgEntry.Mask[i];
                message[setPos] |= msgEntry.Data[i];
                setPos++;
            }
        }
    }
}
