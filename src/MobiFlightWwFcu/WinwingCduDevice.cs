using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MobiFlightWwFcu
{
    internal class WinwingCduDevice : IWinwingDevice
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
            { 'o', 0x18 },  // olive
            { 'x', 0x1B },  // grey  - 9          
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
       

        // Defines Colors, Fonts and Screen
        private List<Tuple<string, byte[]>> InitCommandSequence = new List<Tuple<string, byte[]>>()
        {
            new Tuple<string, byte[]>("1e01", new byte[0]),
            new Tuple<string, byte[]>("1801", new byte[] {0x35, 0x00, 0x17, 0x00, 0x0e, 0x00, 0x18, 0x00}),  // geht erst Mal ohne
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),  // Nr. 1 definiert schriften - große Schriftart
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),  // - Kleine Schriftart
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0x00, 0x00, 0xff, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // BackColor 1 BLACK  00 00 00 {ff] -> 00 00 00 {ff]
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0xa5, 0xff, 0xff, 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 1 AMBER [00 a5] (ff) {ff}-> (ff) [a5 00] {ff}
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0xff, 0xff, 0xff, 0xff, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 2 WHITE
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0xff, 0xff, 0x00, 0xff, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 3 CYAN
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x3d, 0xff, 0x00, 0xff, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 4 GREEN [3d ff] (00) ff ->  (00) [ff 3d] ff
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0xff, 0x63, 0xff, 0xff, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 5 LIGHT YELLOW / Magenta?
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0x00, 0xff, 0xff, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 6 RED
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x00, 0xff, 0xff, 0xff, 0x0b, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 7 YELLOW
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x42, 0x5c, 0x61, 0xff, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 8 Brownish
            new Tuple<string, byte[]>("1901", new byte[] {0x02, 0x00, 0x77, 0x77, 0x77, 0xff, 0x0d, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // FontColor 9 Grey
            new Tuple<string, byte[]>("1901", new byte[] {0x03, 0x00, 0x00, 0x00, 0x00, 0xff, 0x0e, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // geht ohne. Was macht Nr. 3???
            new Tuple<string, byte[]>("1901", new byte[] {0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0f, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // wenn fehlt, wird nur noch 1 Zeichen links oben aktualisiert
            new Tuple<string, byte[]>("1901", new byte[] {0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // Top Left Corner X Coordinate (0x10 = 16)
            new Tuple<string, byte[]>("1901", new byte[] {0x04, 0x00, 0x02, 0x00, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // Top Left Corner Y Coordinate (0x11 = 17)
            new Tuple<string, byte[]>("1a01", new byte[] {0x01 }),
            new Tuple<string, byte[]>("1c01", new byte[0]), 
        };


        private List<Tuple<string, byte[]>> ClearCommandSequence = new List<Tuple<string, byte[]>>()
        {
            //new Tuple<string, byte[]>("0401", new byte[] {0x14 }),   // 0d is white, 0e is black, 0f is red, 10 green, 11 blue, 12 yellow, 13 magenta, 14 black with winwing logo
            new Tuple<string, byte[]>("1201", new byte[] {0xff, 0x06, 0x07, 0x0d}), // that and 1001 leads to black screen. Everything together black screen
            new Tuple<string, byte[]>("1301", new byte[] {0xff, 0x06, 0x07, 0x0d}), // that and 1001 leads to white screen
            new Tuple<string, byte[]>("1001", new byte[] {0x00, 0x00, 0x00, 0x00, 0x80, 0x02, 0xe0, 0x01}), // only that, white screen
            new Tuple<string, byte[]>("0301", new byte[0]), // 1201, 1301, 1001 only have effect when 0301 afterwards
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
                DestinationAddress = WinwingConstants.DEST_PFP3N;
                LedIdentifiers = new Dictionary<string, byte>() // TODO
                {            
                    { $"CALL",   0x08 },
                    { $"FAIL",   0x09 },
                    { $"MSG", 0x0a },
                    { $"OFST",  0x0b },
                    { $"EXEC", 0x0c },              
                };
            }
 
            // Do not add the cdu display
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

        // set brightness by mobiflight process
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
            MessageSender.SetBrightness(DestinationAddress, 0x11, brightness);          
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

        private void ConvertAndSendCduData(string json)
        {
            List<byte> byteList = new List<byte>();
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            JArray data = (JArray)jsonObject["Data"];

            byte formatByte = 0x06;
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
                    byteList.Add(0x07);
                }
                // Last char
                else if ((i == data.Count - 1))
                {
                    byteList.Add(0x08);
                }
                else
                {
                    byteList.Add(formatByte);
                }
                byteList.AddRange(Encoding.UTF8.GetBytes(new char[] { currentChar }));
            }

            MessageSender.SendCduDisplayBytes(byteList.ToArray());            
        }
    }
}
