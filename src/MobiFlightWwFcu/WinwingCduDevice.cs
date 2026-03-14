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

        private IWinwingMessageSender MessageSender = null;

        private WinwingCduType CduType = WinwingCduType.MCDU;

        private byte[] DestinationAddress = WinwingConstants.DEST_MCDU;    

        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";
        private const string LED_BRIGHTNESS = "LED Percentage";

        private WinwingFontConverter FontConverter = new WinwingFontConverter();
        private AesCtrHelper CryptoHelper = new AesCtrHelper();

        private const string KEY_STRING = @"MY6JFI/baxXX0dyOV1c8Bw==";
        private const string NONCE_STRING = @"CcwLDBJBtVe2JUHDnMhWtw==";
        
        private const int ColorStep = 0x21;
        private const int ColorInvertOffset = 0x1B;
        private const int ColorSmallOffset = 0x16B;
        private enum Color : int
        {
            Black = 0x00,
            Amber = Black + ColorStep,
            White = Amber + ColorStep,
            Cyan = White + ColorStep,
            Green = Cyan + ColorStep,
            Magenta = Green + ColorStep,
            Red = Magenta + ColorStep,
            Yellow = Red + ColorStep,
            Blue = Yellow + ColorStep,
            Grey = Blue + ColorStep,
            Khaki = Grey + ColorStep,
        }
        
        private static Dictionary<char, Color> FormatTable = new Dictionary<char, Color>()
        {
            { 'a', Color.Amber },
            { 'w', Color.White },
            { 'c', Color.Cyan },
            { 'g', Color.Green },
            { 'm', Color.Magenta },
            { 'r', Color.Red },
            { 'y', Color.Yellow},
            { 'o', Color.Blue },
            { 'e', Color.Grey },
            { 'k', Color.Khaki },
        };

        private string InitialDisplayJson =
            @"{ 
                ""Target"": ""Display"",
                ""Data"": [[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [""\u2192"",""w"",0], [""M"",""c"",0],[""O"",""w"",0],[""B"",""c"",0],[""I"",""w"",0],[""F"",""c"",0],[""L"",""w"",0],
                           [""I"",""c"",0],[""G"",""w"",0],[""H"",""c"",0],[""T"",""w"",0],[""\u2190"",""c"",0],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],
                           [],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[],[]]}";


        private List<Tuple<string, byte[]>> InitCommandSequence = new List<Tuple<string, byte[]>>();

        private List<Tuple<string, byte[]>> InitCommandHeaderMcdu = new List<Tuple<string, byte[]>>()
        {
            new Tuple<string, byte[]>("1e01", new byte[0]), // clear feature info
            new Tuple<string, byte[]>("1801", new byte[] {0x34, 0x00, 0x18, 0x00, 0x0e, 0x00, 0x18, 0x00}), // SetScreenInfo x-Axis: 16 = 0x34[16! + 36] y-Axis: 4 =  0x18[4! + 20] LineCount: 0x0e[14] ColumnCount: 0x18[24]
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
        };

        private List<Tuple<string, byte[]>> InitCommandHeaderPfp3n = new List<Tuple<string, byte[]>>()
        {
            new Tuple<string, byte[]>("1e01", new byte[0]),            
            new Tuple<string, byte[]>("1801", new byte[] {0x32, 0x00, 0x13, 0x00, 0x0e, 0x00, 0x18, 0x00}),            
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
            new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}),
            // use same as for MCDU with font 5 and 6. Otherwise last line is missing.            
            //new Tuple<string, byte[]>("1801", new byte[] {0x32, 0x00, 0x18, 0x00, 0x0e, 0x00, 0x18, 0x00}), // without fonts 
            //new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x05, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // without fonts 
            //new Tuple<string, byte[]>("1901", new byte[] {0x01, 0x00, 0x06, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}), // without fonts 
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
            new Tuple<string, byte[]>("1901", new byte[] { 0x02, 0x00, 0xff, 0x99, 0x00, 0xff, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
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
            new Tuple<string, byte[]>("1901", new byte[] { 0x03, 0x00, 0xff, 0x99, 0x00, 0xff, 0x17, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }),
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

        public WinwingCduDevice(IWinwingMessageSender sender, WinwingCduType cduType)
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
            else if (CduType == WinwingCduType.PFP7)
            {
                InitCommandSequence.AddRange(InitCommandHeaderPfp3n);
                InitCommandSequence.AddRange(InitCommandData);
                DestinationAddress = WinwingConstants.DEST_PFP7;
                LedIdentifiers = new Dictionary<string, byte>()
                {
                    { $"DSPY",   0x03 },
                    { $"FAIL",   0x04 },
                    { $"MSG", 0x05 },
                    { $"OFST",  0x06 },
                    { $"EXEC", 0x07 },
                };
            }
            else if (CduType == WinwingCduType.PFP4)
            {
                InitCommandSequence.AddRange(InitCommandHeaderPfp3n);
                InitCommandSequence.AddRange(InitCommandData);
                DestinationAddress = WinwingConstants.DEST_PFP4;
                LedIdentifiers = new Dictionary<string, byte>()
                {
                    { $"DSPY",   0x03 },
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

        private void SetFontAndInitDisplay(string fontData)
        {            
            string plainFontJson = fontData;
            // Check and decrypt
            if (!fontData.Contains('{'))
            {
                byte[] key = Convert.FromBase64String(KEY_STRING);
                byte[] nonce = Convert.FromBase64String(NONCE_STRING);
                byte[] decrypted = CryptoHelper.Decrypt(Convert.FromBase64String(fontData), key, nonce);
                plainFontJson = Encoding.UTF8.GetString(decrypted);
            }
            var fontCommands = FontConverter.FontJsonToDisplayCommands(plainFontJson, DestinationAddress);
            MessageSender.SendDisplayCommands(fontCommands.LargeFontHead);
            MessageSender.SendDisplayCommands(fontCommands.LargeFont);
            MessageSender.SendDisplayCommands(fontCommands.SmallFontHead);
            MessageSender.SendDisplayCommands(fontCommands.SmallFont);
            MessageSender.SendDisplayCommands(InitCommands);
            EmptyDisplay();
        }

        public void Connect()
        {
            // MessageSender.SendDisplayCommands(InitCommands); // Remove if font is set and used
            SetBacklightBrightness("80");
            SetLcdBrightness("100");

            // Load font as default            
            FontLoader fontLoader = new FontLoader();
            fontLoader.LoadFont(this, @"{ ""Target"": ""Font"", ""Data"": ""Boeing"" }");
            ConvertAndSendCduData(InitialDisplayJson);
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
            return new List<string>(new string[] { WinwingConstants.FONT_DATA, WinwingConstants.CDU_DATA });
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
                if (LcdCurrentValuesCache.TryGetValue(name, out string currentValue)) // check cache
                {
                    if (currentValue != value)
                    {
                        LcdCurrentValuesCache[name] = value;
                        DisplayNameToActionMapping[name](value); // Execute Action
                    }
                }
                else if (name == WinwingConstants.CDU_DATA)
                {
                    ConvertAndSendCduData(value);
                }
                else if (name == WinwingConstants.FONT_DATA)
                {
                    SetFontAndInitDisplay(value);
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

        private void ConvertAndSendCduData(string json)
        {
            List<byte> byteList = new List<byte>();
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            JArray data = (JArray)jsonObject["Data"];

            for (int i = 0; i < data.Count; i++)
            {
                var item = data[i];
                var (lowByte, highByte) = GetFormatBytes(item, out var currentChar); 
                
                if (i == 0) // First char
                    lowByte += 0x01;
                else if (i == data.Count - 1) // Last char
                    lowByte += 0x02;

                byteList.Add(lowByte);
                byteList.Add(highByte);
                byteList.AddRange(Encoding.UTF8.GetBytes(new char[] { currentChar }));
            }

            MessageSender.SendCduDisplayBytes(byteList.ToArray());            
        }

        internal static (byte lowByte, byte highByte) GetFormatBytes(JToken item, out char currentChar)
        {
            currentChar = ' ';
            var color = Color.White;

            if (item.HasValues)
            {
                currentChar = item[0].Value<char>();
                var formatChar = item[1].Value<char>();
                var isSmall = item[2].Value<bool>();

                // Backwards compatibility: default to false
                var isInverted = item.Count() > 3
                    ? item[3].Value<bool>()
                    : false;

                if (!FormatTable.TryGetValue(formatChar, out color))
                    color = Color.Grey;

                if (isInverted)
                    color += ColorInvertOffset;

                if (isSmall)
                    color += ColorSmallOffset;
            }

            return ((byte)color, (byte)((int)color >> 8));
        }

        public void Stop()
        {
            TurnOffAllLEDs();
        }
    }
}
