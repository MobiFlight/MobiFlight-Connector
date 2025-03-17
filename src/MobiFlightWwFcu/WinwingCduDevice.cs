using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlightWwFcu
{
    public class WinwingCduDevice : IWinwingDevice
   {
        public string Name { get => $"WinWing {CduType}"; }

        private WinwingMessageSender MessageSender = null;

        private WinwingCduType CduType = WinwingCduType.MCDU;

        private byte[] DestinationAddress = WinwingConstants.DEST_MCDU;    

        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";
        private const string LED_BRIGHTNESS = "LED Percentage";


        private Dictionary<char, byte> FormatTable = new Dictionary<char, byte>()
        {
            { 'a', 0x03 },  // amber - 1           
            { 'w', 0x06 },  // white          
            { 'c', 0x09 },  // cyan        
            { 'g', 0x0c },  // green            
            { 'm', 0x0f },  // magenta          
            { 'r', 0x12 },  // red            
            { 'y', 0x15 },  // yellow            
            { 'o', 0x18 },  // brown
            { 'e', 0x1B },  // grey  - 9          
        };

        private Dictionary<char, byte[]> FormatTableLarge = new Dictionary<char, byte[]>()
        {
            { 'a', new byte[] {0x21, 0x00} },  // amber - 1           
            { 'w', new byte[] {0x42, 0x00} },  // white          
            { 'c', new byte[] {0x63, 0x00} },  // cyan        
            { 'g', new byte[] {0x84, 0x00} },  // green            
            { 'm', new byte[] {0xa5, 0x00} },  // magenta          
            { 'r', new byte[] {0xc6, 0x00} },  // red            
            { 'y', new byte[] {0xe7, 0x00} },  // yellow            
            { 'o', new byte[] {0x08, 0x01} },  // brown
            { 'e', new byte[] {0x29, 0x01} },  // grey
            { 'k', new byte[] {0x4a, 0x01} },  // khaki  - 10 , 0x6b, 0x8c
        };


        private Dictionary<char, byte[]> FormatTableSmall = new Dictionary<char, byte[]>()
        {
            { 'a', new byte[] {0x8c, 0x01} },  // amber - 1           
            { 'w', new byte[] {0xad, 0x01} },  // white          
            { 'c', new byte[] {0xce, 0x01} },  // cyan        
            { 'g', new byte[] {0xef, 0x01} },  // green            
            { 'm', new byte[] {0x10, 0x02} },  // magenta          
            { 'r', new byte[] {0x31, 0x02} },  // red            
            { 'y', new byte[] {0x52, 0x02} },  // yellow            
            { 'o', new byte[] {0x73, 0x02} },  // brown
            { 'e', new byte[] {0x94, 0x02} },  // grey
            { 'k', new byte[] {0xb5, 0x02} },  // khaki  - 10
        };

        private string InitialDisplayJson =
            @"{ ""Data"": [[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [""\u2192"",""w"",0], [""M"",""c"",0],[""o"",""w"",0],[""b"",""c"",0],[""i"",""w"",0],[""F"",""c"",0],[""l"",""w"",0],
                           [""i"",""c"",0],[""g"",""w"",0],[""h"",""c"",0],[""t"",""w"",0],[""\u2190"",""c"",0],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[]]}";


        private List<Tuple<string, byte[]>> InitCommandSequence = new List<Tuple<string, byte[]>>();


        private List<Tuple<string, byte[]>> InitCommandHeaderMcdu = new List<Tuple<string, byte[]>>()
        {           
            new Tuple<string, byte[]>("1e01", new byte[0]), // clear feature info
            // orig new Tuple<string, byte[]>("1801", new byte[] {0x34, 0x00, 0x25, 0x00, 0x0e, 0x00, 0x18, 0x00}),
            new Tuple<string, byte[]>("1801", new byte[] {0x34, 0x00, 0x18, 0x00, 0x0e, 0x00, 0x18, 0x00}),
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        };

        private List<Tuple<string, byte[]>> InitCommandHeaderPfp3n = new List<Tuple<string, byte[]>>()
        {
            new Tuple<string, byte[]>("1e01", new byte[0]),
            //new Tuple<string, byte[]>("1801", new byte[] {0x32, 0x00, 0x13, 0x00, 0x0e, 0x00, 0x18, 0x00}),
            //new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
            //new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
            new Tuple<string, byte[]>("1801", new byte[] {0x32, 0x00, 0x18, 0x00, 0x0e, 0x00, 0x18, 0x00}),
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        };

        private List<Tuple<string, byte[]>> InitCommandData = new List<Tuple<string, byte[]>>()
        {
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0xff, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x00, 0xa5, 0xff, 0xff, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0xff, 0xff, 0xff, 0xff, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0xff, 0xff, 0x00, 0xff, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x3d, 0xff, 0x00, 0xff, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0xff, 0x63, 0xff, 0xff, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x00, 0x00, 0xff, 0xff, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x00, 0xff, 0xff, 0xff, 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x42, 0x5c, 0x61, 0xff, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x77, 0x77, 0x77, 0xff, 0x0d, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0x5e, 0x73, 0x79, 0xff, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0xff, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x00, 0xa5, 0xff, 0xff, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0xff, 0xff, 0xff, 0xff, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0xff, 0xff, 0x00, 0xff, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x3d, 0xff, 0x00, 0xff, 0x13, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0xff, 0x63, 0xff, 0xff, 0x14, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x00, 0x00, 0xff, 0xff, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x00, 0xff, 0xff, 0xff, 0x16, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x42, 0x5c, 0x61, 0xff, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x77, 0x77, 0x77, 0xff, 0x18, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0x5e, 0x73, 0x79, 0xff, 0x19, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x1b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1901", new byte[] { 0x04, 0x00, 0x02, 0x00, 0x00, 0x00, 0x1c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
            new Tuple<string, byte[]>("1a01", new byte[] { 0x02 }),
            new Tuple<string, byte[]>("1c01", new byte[0]),
        };


        //private List<Tuple<string, byte[]>> InitCommandSequenceMcdu = new List<Tuple<string, byte[]>>()
        //{
        //    new Tuple<string, byte[]>("1e01", new byte[0]),
        //    new Tuple<string, byte[]>("1801", new byte[] {0x35, 0x00, 0x17, 0x00, 0x0e, 0x00, 0x18, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0x00, 0x00, 0xff, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0xa5, 0xff, 0xff, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0xff, 0xff, 0xff, 0xff, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0xff, 0xff, 0x00, 0xff, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x3d, 0xff, 0x00, 0xff, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0xff, 0x63, 0xff, 0xff, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0x00, 0xff, 0xff, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0xff, 0xff, 0xff, 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x42, 0x5c, 0x61, 0xff, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x77, 0x77, 0x77, 0xff, 0x0d, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x03, 0x00, 0x00, 0x00, 0x00, 0xff, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1901", new byte[] {0x04, 0x00, 0x02, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        //    new Tuple<string, byte[]>("1a01", new byte[] {0x01 }),
        //    new Tuple<string, byte[]>("1c01", new byte[0]),
        //};


        private List<Tuple<string, byte[]>> ClearCommandSequence = new List<Tuple<string, byte[]>>()
        {
            new Tuple<string, byte[]>("0401", new byte[] {0x0e }),
            new Tuple<string, byte[]>("0301", new byte[0]),
            new Tuple<string, byte[]>("1201", new byte[] {0xff, 0x06, 0x07, 0x0d}),
            new Tuple<string, byte[]>("1301", new byte[] {0xff, 0x06, 0x07, 0x0d}),
            new Tuple<string, byte[]>("1001", new byte[] {0x00, 0x00, 0x00, 0x00, 0x80, 0x02, 0xe0, 0x01}),
            new Tuple<string, byte[]>("0301", new byte[0]),
        };

        private List<byte[]> InitCommands;
        private List<byte[]> ClearCommands;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, byte> LedIdentifiers;

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();

        public WinwingCduDevice(WinwingMessageSender sender, WinwingCduType cduType)
        {
            MessageSender = sender;
            CduType = cduType;

            if (CduType == WinwingCduType.MCDU)
            {
                InitCommandSequence.AddRange(InitCommandHeaderMcdu);
                InitCommandSequence.AddRange(InitCommandData);
                //InitCommandSequence.AddRange(InitCommandSequenceMcdu);
                DestinationAddress = WinwingConstants.DEST_MCDU;
                LedIdentifiers = new Dictionary<string, byte>()
                {
                    { $"FAIL",   0x08 },
                    { $"FM",   0x09 },
                    { $"MCDU", 0x0a },
                    { $"MENU",  0x0b },
                    { $"FM1", 0x0c },
                    { $"IND",  0x0d },
                    { $"RDY", 0x0e },
                    { $"STATUS", 0x0f },
                    { $"FM2", 0x10 },
                };
            }
            else if (CduType == WinwingCduType.PFP3N)
            {
                InitCommandSequence.AddRange(InitCommandHeaderPfp3n);                
                InitCommandSequence.AddRange(InitCommandData);
                DestinationAddress = WinwingConstants.DEST_PFP3N;
                LedIdentifiers = new Dictionary<string, byte>()
                {
                    { $"CALL",   0x03 },
                    { $"FAIL",   0x04 },
                    { $"MSG", 0x05 },
                    { $"OFST",  0x06 },
                    { $"EXEC", 0x07 },
                };
            }

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
            InitCommands = new List<byte[]>();
            foreach (var cmd in InitCommandSequence)
            {
                var fullCommand = new List<byte>(DestinationAddress);
                fullCommand.AddRange(new byte[2]);
                fullCommand.AddRange(WinwingConstants.DisplayCmdHeaders[cmd.Item1]);
                fullCommand.AddRange(cmd.Item2);
                InitCommands.Add(fullCommand.ToArray());
            }
         
            ClearCommands = new List<byte[]>();
            foreach (var cmd in ClearCommandSequence)
            {
                var fullCommand = new List<byte>(DestinationAddress);
                fullCommand.AddRange(new byte[2]);
                fullCommand.AddRange(WinwingConstants.DisplayCmdHeaders[cmd.Item1]);
                fullCommand.AddRange(cmd.Item2);
                ClearCommands.Add(fullCommand.ToArray());
            }
        }

        public void Connect()
        {
            MessageSender.SendDisplayCommands(InitCommands);
            SetBacklightBrightness("80");
            SetLcdBrightness("100");
            ConvertAndSendCduData(InitialDisplayJson);
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
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (name == WinwingConstants.CDU_DATA)
                {
                    ConvertAndSendCduData(value);
                }
                else if (LcdCurrentValuesCache[name] != value) // check cache
                {
                    LcdCurrentValuesCache[name] = value;
                    DisplayNameToActionMapping[name](value); // Execute Action
                }
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

        private void EmptyDisplay()
        {
            MessageSender.SendDisplayCommands(ClearCommands);
        }

        private void ConvertAndSendCduData1(string json)
        {
            List<byte> byteList = new List<byte>();
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            JArray data = (JArray)jsonObject["Data"];

            byte formatByte = FormatTable['w'];
            char currentChar = ' ';
            char formatChar = 'w';
            bool isSmall = false;
            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                currentChar = ' ';
                formatChar = 'w';
                isSmall = false;

                if (item.HasValues)
                {
                    currentChar = item[0].Value<char>();
                    formatChar = item[1].Value<char>();
                    isSmall = item[2].Value<bool>();
                    formatByte = FormatTable[formatChar];
                    if (isSmall)
                    {
                        formatByte = (byte)(formatByte + 30);
                    }
                }
                // First char
                if (i == 0)
                {
                    byteList.Add((byte)(formatByte + 0x01));                   
                }
                // Last char
                else if ((i == data.Count - 1))
                {
                    byteList.Add((byte)(formatByte + 0x02));                    
                }
                else
                {
                    byteList.Add(formatByte);
                }
                byteList.AddRange(Encoding.UTF8.GetBytes(new char[] { currentChar }));
            }
        }

        private void ConvertAndSendCduData(string json)
        {
            List<byte> byteList = new List<byte>();
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            JArray data = (JArray)jsonObject["Data"];

            byte[] formatBytes = FormatTableLarge['w'];
            char currentChar = ' ';
            char formatChar = 'w';
            bool isSmall = false;

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                currentChar = ' ';
                //formatChar = 'w';
                //isSmall = false;

                if (item.HasValues)
                {
                    currentChar = item[0].Value<char>();
                    formatChar = item[1].Value<char>();
                    isSmall = item[2].Value<bool>();
                    var table = isSmall ? FormatTableSmall : FormatTableLarge;
                    if (table.ContainsKey(formatChar))
                    {
                        formatBytes = table[formatChar];
                    } 
                    else
                    {
                        // error format - do grey
                        formatBytes = table['e'];
                    }                       
                }

                // First char
                if (i == 0)
                {
                    byteList.Add((byte)(formatBytes[0] + 0x01));
                    byteList.Add(formatBytes[1]);
                }
                // Last char
                else if ((i == data.Count - 1))
                {
                    byteList.Add((byte)(formatBytes[0] + 0x02));
                    byteList.Add(formatBytes[1]);
                }
                else
                {
                    byteList.AddRange(formatBytes);
                }
                byteList.AddRange(Encoding.UTF8.GetBytes(new char[] { currentChar }));
            }

            MessageSender.SendCduDisplayBytes(byteList.ToArray());            
        }
    }
}
