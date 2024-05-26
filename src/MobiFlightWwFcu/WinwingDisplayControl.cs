using HidSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Xml.Linq;

namespace MobiFlight
{
    internal class MsgEntry
    {
        public byte StartPos;
        public byte[] Mask;
        public byte[] Data;
    }

    public class WinwingDisplayControl
    {
        // f0 00 0e 12 10bb0000 04 010000 44 8f 32 00000100000002 000000000000000000000000000000000000000000000000000000000000000000000000000000000000
        // 0  1  2  3  4 5 6 7  8  9      12 13 14 15

        private readonly int VendorId = 0x4098;
        private readonly int ProductId = 0xBB10;
        private HidStream Stream { get; set; }
        private HidDevice Device { get; set; }

        private Thread HeartbeatThread = null;
        private volatile bool DoExecuteHeartbeat = false;
        private object StreamLock = new object();

        public event EventHandler<string> ErrorMessageCreated;

        // ----------
        private Dictionary<string, MsgEntry> DisplayHeader = new Dictionary<string, MsgEntry>()
        {
            { "ReportId",       new MsgEntry { StartPos = 0, Mask = new byte[1], Data = new byte[] { 0xF0 } } },
            { "Reserve",        new MsgEntry { StartPos = 1, Mask = new byte[1], Data = new byte[] { 0x00 } } },
            { "Counter",        new MsgEntry { StartPos = 2, Mask = new byte[1], Data = new byte[] { 0x00 } } },
            //{ "MessageLength",new MsgEntry { StartPos = 3, Mask = new byte[1], Data = new byte[] { 0x12 } } },
            { "HeaderBlock1",   new MsgEntry { StartPos = 4, Mask = new byte[4], Data = new byte[] { 0x10, 0xbb, 0x00, 0x00 } } },
            //{ "MessageId",    new MsgEntry { StartPos = 8, Mask = new byte[1], Data = new byte[] { 0x02 } } },
            { "HeaderBlock2",   new MsgEntry { StartPos = 9, Mask = new byte[3], Data = new byte[] { 0x01, 0x00, 0x00 } } },
            { "TimeBlock",      new MsgEntry { StartPos = 12, Mask = new byte[3], Data = new byte[] { 0x01, 0x00, 0x00 } } },
        };
        private Dictionary<string, MsgEntry> DisplayHeaderMessageLength = new Dictionary<string, MsgEntry>()
        {
            { "SetValues",      new MsgEntry { StartPos = 3, Mask = new byte[1], Data = new byte[] { 0x31 } } },
            { "Confirm",        new MsgEntry { StartPos = 3, Mask = new byte[1], Data = new byte[] { 0x11 } } },
            { "Test",          new MsgEntry { StartPos = 3, Mask = new byte[1], Data = new byte[] { 0x12 } } },
        };
        private Dictionary<string, MsgEntry> DisplayHeaderMessageIds = new Dictionary<string, MsgEntry>()
        {
            { "SetValues",      new MsgEntry { StartPos = 8, Mask = new byte[1], Data = new byte[] { 0x02 } } },
            { "Confirm",        new MsgEntry { StartPos = 8, Mask = new byte[1], Data = new byte[] { 0x03 } } },
            { "Test",          new MsgEntry { StartPos = 8, Mask = new byte[1], Data = new byte[] { 0x04 } } },
        };
        // -----------------------


        // -----------------------
        private Dictionary<string, MsgEntry> DisplayTestModeData = new Dictionary<string, MsgEntry>()
        {
            { "DataBlock",      new MsgEntry { StartPos = 15, Mask = new byte[6], Data = new byte[] { 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 } } },
        };

        private Dictionary<string, MsgEntry> DisplayTestCommands = new Dictionary<string, MsgEntry>()
        {
            { "AllOn",          new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x02 } } },
            { "AllOff",         new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x06 } } },
            { "Half1On",        new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x07 } } },
            { "Half2On",        new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x09 } } },
        };
        // ---------------------------


        private MsgEntry TimeBlock = new MsgEntry { StartPos = 12, Mask = new byte[3], Data = new byte[] { 0x01, 0x00, 0x00 } };
        private MsgEntry Counter = new MsgEntry { StartPos = 2, Mask = new byte[1], Data = new byte[] { 0x00 } };

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
        //                          Time                         SPD     HDG    -- ALT-----   VS
        // f000d93810bb000002010000 86e9ab 00002000000000000000 d6fcbc 48404070 03 d6b6bfbf 5f50000080000000000000000000000010bb000003010000000000
        // 0               xx       12     15        20      24 25     28  30   32   34  36 37  39
        // TODO Die 02 ist CommandID
        private Dictionary<string, MsgEntry> DisplaySetValuesData = new Dictionary<string, MsgEntry>()
        {
            { "DataBlock",      new MsgEntry { StartPos = 15, Mask = new byte[10], Data = new byte[] { 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 } } },
            { "SpeedHundreds",  new MsgEntry { StartPos = 25, Mask = new byte[] { 0b00000001 }, Data = new byte[] { 0x60 } } },
            { "SpeedTens",      new MsgEntry { StartPos = 26, Mask = new byte[] { 0b00000001 }, Data = new byte[] { 0xfa } } },
            { "SpeedOnes",      new MsgEntry { StartPos = 27, Mask = new byte[] { 0b00000001 }, Data = new byte[] { 0xfa } } },
            { "MachDecPoint",   new MsgEntry { StartPos = 26, Mask = new byte[] { 0b11111110 }, Data = new byte[] { 0b00000001 } } },
            { "MachNoDecPoint", new MsgEntry { StartPos = 26, Mask = new byte[] { 0b11111110 }, Data = new byte[] { 0b00000000 } } },
            { "MachLabel",      new MsgEntry { StartPos = 28, Mask = new byte[] { 0b11110011 }, Data = new byte[] { 0b00000100 } } },
            { "SpeedLabel",     new MsgEntry { StartPos = 28, Mask = new byte[] { 0b11110011 }, Data = new byte[] { 0b00001000 } } },           
            { "SpeedDot",       new MsgEntry { StartPos = 28, Mask = new byte[] { 0b11111100 }, Data = new byte[] { 0b00000010 } } },
            { "SpeedNoDot",     new MsgEntry { StartPos = 28, Mask = new byte[] { 0b11111100 }, Data = new byte[] { 0b00000001 } } },
            { "HdgHundreds",    new MsgEntry { StartPos = 28, Mask = new byte[] { 0b00001111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "HdgTens",        new MsgEntry { StartPos = 29, Mask = new byte[] { 0b00001111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "HdgOnes",        new MsgEntry { StartPos = 30, Mask = new byte[] { 0b00001111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },
            { "HdgDot",         new MsgEntry { StartPos = 31, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00010000 } } },
            { "HdgNoDot",       new MsgEntry { StartPos = 31, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00000000 } } },
            { "TrackFpaMode1",  new MsgEntry { StartPos = 31, Mask = new byte[] { 0b00011111, 0b11110000  }, Data = new byte[] { 0b01100000, 0b00000011 } } },
            { "HdgVsMode1",     new MsgEntry { StartPos = 31, Mask = new byte[] { 0b00011111, 0b11110000  }, Data = new byte[] { 0b10100000, 0b00001100 } } },
            { "TrackFpaMode2",  new MsgEntry { StartPos = 41, Mask = new byte[] { 0b00011111 }, Data = new byte[] { 0b10000000 } } }, // 0x40-VS  und  0x80-FPA - Erste Nibbel vom letzten Byte
            { "HdgVsMode2",     new MsgEntry { StartPos = 41, Mask = new byte[] { 0b00011111 }, Data = new byte[] { 0b01000000 } } }, // 0x40-VS  und  0x80-FPA - Erste Nibbel vom letzten Byte
            { "NoAltLvlCh",     new MsgEntry { StartPos = 32, Mask = new byte[] { 0xef, 0xef, 0xef, 0xef, 0xef }, Data = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 } } },
            { "AltLvlCh",       new MsgEntry { StartPos = 32, Mask = new byte[] { 0xef, 0xef, 0xef, 0xef, 0xef }, Data = new byte[] { 0x10, 0x10, 0x10, 0x10, 0x10 } } },
            { "AltTenthsds",    new MsgEntry { StartPos = 32, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } }, // 2 should be 0x70 -> letzte Bit macht nix??
            { "AltThousands",   new MsgEntry { StartPos = 33, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } }, // 6 shoud be 0xfb -> letzte bit setzt ALT LOGO
            { "AltHundreds",    new MsgEntry { StartPos = 34, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0x00, 0x06 } } }, // 0 should be 0xbf -> letzte bit setzt oben |- gezeichnet
            { "AltTens",        new MsgEntry { StartPos = 35, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } }, // sollte sein 0xbf - Letzte Bit setzt LVL/CH 
            { "AltOnes",        new MsgEntry { StartPos = 36, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } }, // sollte sein 0xbf - Letzte Bit setzt -|
            { "VsMinus",        new MsgEntry { StartPos = 37, Mask = new byte[] { 0b11101111, 0xff, 0b11101111 }, Data = new byte[] { 0b00010000, 0x00, 0b00000000 } } },
            { "VsPlus",         new MsgEntry { StartPos = 37, Mask = new byte[] { 0b11101111, 0xff, 0b11101111 }, Data = new byte[] { 0b00010000, 0x00, 0b00010000 } } },
            { "FpaDecPoint",    new MsgEntry { StartPos = 38, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00010000 } } },
            { "FpaNoDecPoint",  new MsgEntry { StartPos = 38, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00000000 } } },
            { "VsThousands",    new MsgEntry { StartPos = 37, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },  // letzte Bit -> Sorgt für das Minus vor der Zahl
            { "VsHundreds",     new MsgEntry { StartPos = 38, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0xa0, 0x0f } } },  // letzte Bit -> Setzt den Dezimalpunkt vor die Zahl
            // Intentionally set initial VS to '00oo'
            { "VsTens",         new MsgEntry { StartPos = 39, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0x60, 0x03 } } },  // letzte Bit -> Setzt den | vom Plus vor VS-Wert                                                      
            { "VsOnes",         new MsgEntry { StartPos = 40, Mask = new byte[] { 0b00011111, 0b11110000 }, Data = new byte[] { 0x60, 0x03 } } },  // letzte Bit -> Setzt den ALT DOT
            { "AltDot",         new MsgEntry { StartPos = 40, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00010000 } } },
            { "AltNoDot",       new MsgEntry { StartPos = 40, Mask = new byte[] { 0b11101111 }, Data = new byte[] { 0b00000000 } } },
        };

        private List<byte> DisplayTestMessage = new List<byte>();
        private List<byte> SetValuesMessage = new List<byte>();
        private List<byte> ConfirmMessage = new List<byte>();
        private byte[] LightMessage = new byte[14] { 0x02, 0x10, 0xbb, 0, 0, 3, 0x49, 3, 0, 0, 0, 0, 0, 0 };

        // 02 01 00 00 00 01 00 00 00 00 00 00 00 00
        private byte[] HeartBeatMessage = new byte[14] { 0x02, 0x01, 0, 0, 0, 0x01, 0, 0, 0, 0, 0, 0, 0, 0 };

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

        
        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {
            { "LOC", 0x03 },
            { "AP1", 0x05 },
            { "AP2", 0x07 },
            { "ATHR", 0x09 },
            { "APPR", 0x0D },
            { "EXPED", 0x0b }
        };

        private Dictionary<string, Action<string>> DisplayList = new Dictionary<string, Action<string>>();
        private Dictionary<string, string> LcdCurrentValues = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValues = new Dictionary<string, byte>();

        public WinwingDisplayControl()
        {
            DisplayList.Add(SPEED, SetSpeed);
            DisplayList.Add(MACH, SetMachSpeed);
            DisplayList.Add(MACH_MODE, SetMachModeOnOff);
            DisplayList.Add(SPEED_DASHES, SetSpeedDashes);
            DisplayList.Add(SPEED_DOT, SetSpeedDotOnOff);
            DisplayList.Add(HEADING, SetHeading);
            DisplayList.Add(TRK, SetTrack);
            DisplayList.Add(HEADING_DASHES, SetHeadingDashes);
            DisplayList.Add(HEADING_DOT, SetHeadingDotOnOff);
            DisplayList.Add(ALTITUDE, SetAltitude);
            DisplayList.Add(ALTITUDE_DOT, SetAltitudeDotOnOff);
            DisplayList.Add(VS, SetVs);
            DisplayList.Add(FPA, SetFpa);
            DisplayList.Add(VS_DASHES, SetVSDashes);
            DisplayList.Add(TRK_MODE, SetTrackFpaModeOnOff);
            DisplayList.Add(ANN_LIGHT, SetAnnunciatorLightOnOff);
            DisplayList.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            DisplayList.Add(LCD_BRIGHTNESS, SetLcdBrightness); 
            DisplayList.Add(LED_BRIGHTNESS, SetLedBrightness);
           
            foreach (var displayName in GetDisplayNames())
            {
                LcdCurrentValues.Add(displayName, string.Empty);
            }

            foreach (var ledName in GetLedNames()) 
            {
                LedCurrentValues.Add(ledName, 255);
            }
            
            DisplayTestMessage.AddRange(new byte[64]);
            SetValuesMessage.AddRange(new byte[64]);
            ConfirmMessage.AddRange(new byte[64]);
            Init();
        }

        private void InitDisplayMessage(string id, List<byte> message, Dictionary<string, MsgEntry> data)
        {            
            foreach (var entry in DisplayHeader.Values)
            {
                SetBytesDisplayMessage(entry, message);
            }
            SetBytesDisplayMessage(DisplayHeaderMessageIds[id], message);
            SetBytesDisplayMessage(DisplayHeaderMessageLength[id], message);

            foreach (var entry in data.Values)
            {
                SetBytesDisplayMessage(entry, message);
            }
        }

        private void Init()
        {
            // Init TestModeMessage
            InitDisplayMessage("Test", DisplayTestMessage, DisplayTestModeData);

            // Init SetValuesMessage
            InitDisplayMessage("SetValues", SetValuesMessage, DisplaySetValuesData);

            // Init ConfirmMessage
            InitDisplayMessage("Confirm", ConfirmMessage, new Dictionary<string, MsgEntry>());
        }

        public void Connect()
        {
            Device = DeviceList.Local.GetHidDeviceOrNull(vendorID: VendorId, productID: ProductId);
            if (Device == null) return;
            Stream = Device.Open();
            Stream.ReadTimeout = System.Threading.Timeout.Infinite;
         
            SendDisplayMessage(SetValuesMessage); // Init display
            SetBacklightBrightness("20");
            SetLcdBrightness("100");
            StartHeartbeat();            
        }

        public void Shutdown()
        {
            try
            {
                if (Stream != null)
                {                    
                    StopHeartbeat();
                    ResetDisplay();
                    SetBacklightBrightness("0");
                    SetLcdBrightness("0");
                    foreach (var ledName in LedIdentifiers.Keys)
                    {
                        SetLed(ledName, 0);
                    }
                    Stream.Close();
                    Stream = null;
                }
            }
            catch
            {
                // do nothing if issue on shutdown
            }
        }

        private void StartHeartbeat()
        {
            if (HeartbeatThread == null)
            {
                Thread thread = new Thread(ExecuteHeartbeat);
                thread.IsBackground = true;
                thread.Start();
            }
            DoExecuteHeartbeat = true;                  
        }

        private void StopHeartbeat()
        {
            DoExecuteHeartbeat = false;
        }


        private void ExecuteHeartbeat()
        {
            while (true)
            {
                if (DoExecuteHeartbeat)
                {
                    // Do the pattern like in recording
                    WriteStream(HeartBeatMessage, 0, 14);
                    Thread.Sleep(450);
                    WriteStream(HeartBeatMessage, 0, 14);
                }
                Thread.Sleep(2550);
            }
        }


        private void PrepareAndSendDisplayTestMessage(MsgEntry entry)
        {
            SetBytesDisplayMessage(entry, DisplayTestMessage);
            SendDisplayMessage(DisplayTestMessage);
        }

        private void ResetDisplay()
        {
            var resetMsg = new MsgEntry { StartPos = 25, Mask = new byte[18], Data = new byte[18] };
            SetBytesDisplayMessage(resetMsg, SetValuesMessage);
            SendDisplayMessage(SetValuesMessage);
        }

        private void ResetSpeedCache()
        {
            LcdCurrentValues[SPEED] = string.Empty;
            LcdCurrentValues[MACH] = string.Empty;  
        }

        private void ResetHeadingCache()
        {
            LcdCurrentValues[HEADING] = string.Empty;
            LcdCurrentValues[TRK] = string.Empty;
        }

        private void ResetVSCache()
        {
            LcdCurrentValues[VS] = string.Empty;
            LcdCurrentValues[FPA] = string.Empty;
        }

        private void SetSpeedInternal(char[] speedChars, bool isMach)
        {
            var speedHundreds = DisplaySetValuesData["SpeedHundreds"];
            var speedTens = DisplaySetValuesData["SpeedTens"];
            var speedOnes = DisplaySetValuesData["SpeedOnes"];
            speedHundreds.Data = SpeedNumberCodes[speedChars[0]];
            speedTens.Data = SpeedNumberCodes[speedChars[1]];
            speedOnes.Data = SpeedNumberCodes[speedChars[2]];

            SetBytesDisplayMessage(speedHundreds, SetValuesMessage);
            SetBytesDisplayMessage(speedTens, SetValuesMessage);
            SetBytesDisplayMessage(speedOnes, SetValuesMessage);
            if (isMach)
            {
                SetBytesDisplayMessage(DisplaySetValuesData["MachDecPoint"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["MachNoDecPoint"], SetValuesMessage);
            }

            SendDisplayMessage(SetValuesMessage);
        }

        private void SetSpeed(string speed)
        {                       
            int mySpeed = (int)Convert.ToDouble(speed, CultureInfo.InvariantCulture);
            char[] speedChars = mySpeed.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetSpeedInternal(speedChars, false);
        }

        private void SetMachSpeed(string speed)
        {           
            int mySpeed = (int)(Convert.ToDouble(speed, CultureInfo.InvariantCulture) * 100);
            char[] speedChars = mySpeed.ToString("D3", CultureInfo.InvariantCulture).ToCharArray();
            SetSpeedInternal(speedChars, true);
        }

        private void SetSpeedDotOnOff(string speedDot)
        {
            int myDot = (int)Convert.ToDouble(speedDot, CultureInfo.InvariantCulture);
            if (myDot == 0)
            {
                SetBytesDisplayMessage(DisplaySetValuesData["SpeedNoDot"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["SpeedDot"], SetValuesMessage);
            }
            SendDisplayMessage(SetValuesMessage);
        }

        private void SetSpeedDashes(string speedDashes)
        {
            int myDashes = (int)Convert.ToDouble(speedDashes, CultureInfo.InvariantCulture);
            if (myDashes == 1)
            {                
                SetSpeedInternal(new char[] { '-', '-', '-' }, false);
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
                SetBytesDisplayMessage(DisplaySetValuesData["MachLabel"], SetValuesMessage);               
            }
            else
            {            
                SetBytesDisplayMessage(DisplaySetValuesData["SpeedLabel"], SetValuesMessage);             
            }
            ResetSpeedCache();
            SendDisplayMessage(SetValuesMessage);
        }


        private void SetHeadingInternal(char[] hdgChars)
        {
            var hdgHundreds = DisplaySetValuesData["HdgHundreds"];
            var hdgTens = DisplaySetValuesData["HdgTens"];
            var hdgOnes = DisplaySetValuesData["HdgOnes"];
            hdgHundreds.Data = GeneralNumberCodes[hdgChars[0]];
            hdgTens.Data = GeneralNumberCodes[hdgChars[1]];
            hdgOnes.Data = GeneralNumberCodes[hdgChars[2]];

            SetBytesDisplayMessage(hdgHundreds, SetValuesMessage);
            SetBytesDisplayMessage(hdgTens, SetValuesMessage);
            SetBytesDisplayMessage(hdgOnes, SetValuesMessage);

            SendDisplayMessage(SetValuesMessage);
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
                SetBytesDisplayMessage(DisplaySetValuesData["HdgNoDot"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["HdgDot"], SetValuesMessage);
            }
            SendDisplayMessage(SetValuesMessage);
        }

        private void SetTrackFpaModeOnOff(string fpaMode)
        {
            int myFpaMode = (int)Convert.ToDouble(fpaMode, CultureInfo.InvariantCulture);
            if (myFpaMode == 1)
            {            
                SetBytesDisplayMessage(DisplaySetValuesData["TrackFpaMode1"], SetValuesMessage);
                SetBytesDisplayMessage(DisplaySetValuesData["TrackFpaMode2"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["HdgVsMode1"], SetValuesMessage);
                SetBytesDisplayMessage(DisplaySetValuesData["HdgVsMode2"], SetValuesMessage);
            }
            ResetHeadingCache();
            ResetVSCache();
            SendDisplayMessage(SetValuesMessage);
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

            SetBytesDisplayMessage(altTenthsds, SetValuesMessage);
            SetBytesDisplayMessage(altThousands, SetValuesMessage);
            SetBytesDisplayMessage(altHundreds, SetValuesMessage);
            SetBytesDisplayMessage(altTens, SetValuesMessage);
            SetBytesDisplayMessage(altOnes, SetValuesMessage);

            SendDisplayMessage(SetValuesMessage);
        }

        private void SetAltitudeDotOnOff(string altitudeDot)
        {
            int myDot = (int)Convert.ToDouble(altitudeDot, CultureInfo.InvariantCulture);
            if (myDot == 0)
            {
                SetBytesDisplayMessage(DisplaySetValuesData["AltNoDot"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["AltDot"], SetValuesMessage);
            }
            SendDisplayMessage(SetValuesMessage);
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
                SetBytesDisplayMessage(DisplaySetValuesData["VsMinus"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["VsPlus"], SetValuesMessage);
            }

            if (isFpa)
            {
                SetBytesDisplayMessage(DisplaySetValuesData["FpaDecPoint"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["FpaNoDecPoint"], SetValuesMessage);
            }
            SetBytesDisplayMessage(vsThousands, SetValuesMessage);
            SetBytesDisplayMessage(vsHundreds, SetValuesMessage);
            SetBytesDisplayMessage(vsTens, SetValuesMessage);
            SetBytesDisplayMessage(vsOnes, SetValuesMessage);

            SendDisplayMessage(SetValuesMessage);

        }

        private void SetVSDashes(string vsDashes)
        {
            int myDashes = (int)Convert.ToDouble(vsDashes, CultureInfo.InvariantCulture);
            if (myDashes == 1)
            {
                SetVSInternal(new char[] { '-', '-', '-', '-' }, true, false);
            }
            else if (myDashes == 0)
            {
                ResetVSCache();
            }
        }

        public void SetLed(string led, byte state)
        {
            try
            {
                if (!string.IsNullOrEmpty(led) && LedCurrentValues[led] != state)
                {
                    LedCurrentValues[led] = state;
                    LightMessage[5] = 0x03; // length
                    LightMessage[6] = 0x49; // light control command
                    LightMessage[7] = LedIdentifiers[led];
                    LightMessage[8] = state == 0 ? (byte)0 : (byte)1;
                    LightMessage[9] = 0x00;
                    LightMessage[10] = 0x00;
                    LightMessage[11] = 0x00;
                    LightMessage[12] = 0x00;
                    LightMessage[13] = 0x00;
                    WriteStream(LightMessage, 0, 14);
                }
            }
            catch
            {
                ErrorMessageCreated?.Invoke(this, $"Error setting Winwing FCU LED name='{led}' to value='{state}'. Please check input.");
            }
        }
        
        private void SetAnnunciatorLightOnOff(string annLight)
        {
            int myAnnLight = (int)Convert.ToDouble(annLight, CultureInfo.InvariantCulture);
            if (myAnnLight == 1)
            {
                PrepareAndSendDisplayTestMessage(DisplayTestCommands["AllOn"]);                                 
            }
            else
            {
                SendDisplayMessage(SetValuesMessage);
            }            
        }

        private void SetBrightnessInternal(byte type, string brightness)
        {
            // Input should be 0 to 100 percent - scale to 0..255
            int value = (int)Math.Round((Convert.ToDouble(brightness, CultureInfo.InvariantCulture) * 2.55));           
            byte byteValue = value >= 255 ? (byte)255 : (byte)value;
            LightMessage[5] = 0x03; // length
            LightMessage[6] = 0x49; // light control command
            LightMessage[7] = type; 
            LightMessage[8] = byteValue;
            LightMessage[9] = 0x00;
            LightMessage[10] = 0x00;
            LightMessage[11] = 0x00;
            LightMessage[12] = 0x00;
            LightMessage[13] = 0x00;
            WriteStream(LightMessage, 0, 14);
        }

        private void SetLedBrightness(string brightness)
        {
            SetBrightnessInternal(0x11, brightness);
        }

        private void SetBacklightBrightness(string brightness) 
        {
            SetBrightnessInternal(0x00, brightness);
        }

        private void SetLcdBrightness(string brightness)
        {
            SetBrightnessInternal(0x01, brightness);
        }

        public List<string> GetLedNames()
        {
            return LedIdentifiers.Keys.ToList();
        }

        public List<string> GetDisplayNames()
        {           
            return DisplayList.Keys.ToList();
        }

        public void SetDisplay(string name, string value)
        {            
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (LcdCurrentValues[name] != value)
                {
                    try
                    {
                        LcdCurrentValues[name] = value;
                        DisplayList[name](value);                        
                    }
                    catch
                    {
                        ErrorMessageCreated?.Invoke(this, $"Error setting Winwing FCU display name='{name}' to value='{value}'. Probably value not in a valid number format.");
                    }
                }                
            }
        }

        // "AllOn", "AllOff", "Half1On", "Half2On"
        // TODO Set to private for MobiFlight DLL Build
        private void LcdTest(string command)
        {
            PrepareAndSendDisplayTestMessage(DisplayTestCommands[command]);
        }


        private void SendDisplayMessage(List<byte> message)
        {
            SendDisplayMessageToFcu(message);
            SendDisplayMessageToFcu(ConfirmMessage); // Always at that second message
        }

        private void SendDisplayMessageToFcu(List<byte> message)
        {
            Counter.Data[0]++;
            TimeBlock.Data = GetTimeAsBytes();
            SetBytesDisplayMessage(TimeBlock, message);
            SetBytesDisplayMessage(Counter, message);
    
            WriteStream(message.ToArray(), 0, 64);
        }

        private void WriteStream(byte[] buffer, int offset, int count)
        {
            if (Stream == null)
            {
                throw new ApplicationException("WinwingDisplayControl cannot send data. Not connected to device. Stream is null.");
            }
            lock (StreamLock)
            {
                Stream.Write(buffer, offset, count);
            }
        }


        private void SetBytesDisplayMessage(MsgEntry msgEntry, List<byte> message)
        {
            byte setPos = msgEntry.StartPos;
            for (int i = 0; i < msgEntry.Data.Length; i++)
            {
                message[setPos] &= msgEntry.Mask[i];
                message[setPos] |= msgEntry.Data[i];
                setPos++;
            }
        }

        private byte[] GetTimeAsBytes()
        {
            DateTime time = DateTime.Now;
            byte[] timeBytes = new byte[3];
            timeBytes[0] = (byte)(time.Millisecond / 4);
            timeBytes[1] = (byte)(time.Second * 3);
            timeBytes[2] = (byte)time.Minute;
            return timeBytes;
        }
    }
}
