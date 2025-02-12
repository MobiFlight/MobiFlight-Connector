using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MobiFlightWwFcu;

namespace MobiFlight.Joysticks.Winwing
{
    internal class WinwingEfisDevice : IWinwingDevice
    {
        public string Name { get => $"WinWing EFIS {EfisType}"; }

        private WinwingMessageSender MessageSender = null;
        private string EfisType = WinwingConstants.EFISL_NAME;

        private byte[] DestinationAddress;

        // https://docs.flybywiresim.com/pilots-corner/airliner-flying-guide/altitude-refs/
        private string BaroHpa { get => $"hPa Value {EfisType}"; }
        private string BaroInHg { get => $"inHg Value {EfisType}"; }
        private string BaroInHgOnOff { get => $"inHg Mode On/Off {EfisType}"; }
        private string BaroStd { get => $"STD Mode On/Off {EfisType}"; }
        private string Qfe { get => $"QFE Mode On/Off {EfisType}"; }

        private const string ANN_LIGHT = "LCD Test On/Off";
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";
        private const string LED_BRIGHTNESS = "LED Percentage";

        private Dictionary<string, MsgEntry> DisplayTestCommands = new Dictionary<string, MsgEntry>()
        {
            { "AllOn",    new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x23 } } },
            { "AllOff",   new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x24 } } },
            { "Half1On",  new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x25 } } },
            { "Half2On",  new MsgEntry { StartPos = 17, Mask = new byte[1], Data = new byte[] { 0x26 } } },
        };

        private Dictionary<char, byte[]> BaroNumberCodes = new Dictionary<char, byte[]>()
        {
            { '*', new byte[] { 0x00 } },
            { '0', new byte[] { 0x7d } },
            { '1', new byte[] { 0x60 } },
            { '2', new byte[] { 0x3e } },
            { '3', new byte[] { 0x7a } },
            { '4', new byte[] { 0x63 } },
            { '5', new byte[] { 0x5b } },
            { '6', new byte[] { 0x5f } },
            { '7', new byte[] { 0x70 } },
            { '8', new byte[] { 0x7f } },
            { '9', new byte[] { 0x7b } },
            { 'S', new byte[] { 0b01011011 } },
            { 't', new byte[] { 0b00001111 } },
            { 'd', new byte[] { 0b01101110 } },
        };



        private Dictionary<string, MsgEntry> DisplaySetValuesData = new Dictionary<string, MsgEntry>()
        {
            { "BaroThousands",  new MsgEntry { StartPos = 21, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x60 } } },
            { "BaroHundreds",   new MsgEntry { StartPos = 22, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x7d } } },
            { "BaroTens",       new MsgEntry { StartPos = 23, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x60 } } },
            { "BaroOnes",       new MsgEntry { StartPos = 24, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x7a } } },
            { "InHgDecPoint",   new MsgEntry { StartPos = 22, Mask = new byte[] { 0b01111111 }, Data = new byte[] { 0b10000000 } } },
            { "InHgNoDecPoint", new MsgEntry { StartPos = 22, Mask = new byte[] { 0b01111111 }, Data = new byte[] { 0b00000000 } } },
            { "NoBaro",         new MsgEntry { StartPos = 25, Mask = new byte[] { 0b00000000 }, Data = new byte[] { 0x00 } } },
            { "QfeBaro",        new MsgEntry { StartPos = 25, Mask = new byte[] { 0b00000000 }, Data = new byte[] { 0x01 } } },
            { "QnhBaro",        new MsgEntry { StartPos = 25, Mask = new byte[] { 0b00000000 }, Data = new byte[] { 0x02 } } },           
        };

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, byte> LedIdentifiers;

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();       

        private byte[] DisplayTestCommand = new byte[0x12];
        private byte[] RefreshCommand = new byte[0x11];
        private byte[] SetValuesCommand = new byte[0x1a];        

        public WinwingEfisDevice(WinwingMessageSender sender, string efisType)
        {
            MessageSender = sender;
            EfisType = efisType;

            if (EfisType == WinwingConstants.EFISL_NAME)
            {
                DestinationAddress = WinwingConstants.DEST_EFISL;
            }
            else if (EfisType == WinwingConstants.EFISR_NAME)
            {
                DestinationAddress = WinwingConstants.DEST_EFISR;
            }           
           
            LedIdentifiers = new Dictionary<string, byte>()
            {
                { $"FD {EfisType}",   0x03 },
                { $"LS {EfisType}",   0x04 },
                { $"CSTR {EfisType}", 0x05 },
                { $"WPT {EfisType}",  0x06 },
                { $"VORD {EfisType}", 0x07 },
                { $"NDB {EfisType}",  0x08 },
                { $"ARPT {EfisType}", 0x09 }
            };

            DisplayNameToActionMapping.Add(BaroHpa, SetBaroHpa);
            DisplayNameToActionMapping.Add(BaroInHg, SetBaroInHg);
            DisplayNameToActionMapping.Add(BaroInHgOnOff, SetBaroInHgOnOff);
            DisplayNameToActionMapping.Add(BaroStd, SetBaroStdOnOff);      
            DisplayNameToActionMapping.Add(Qfe, SetQfeOnOff);

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
            // Init DisplayTestCommand
            var initDisplayTest = new List<byte>(DestinationAddress);
            initDisplayTest.AddRange(new byte[2]);
            initDisplayTest.AddRange(WinwingConstants.DisplayCmdHeaders["0401"]);
            initDisplayTest.CopyTo(DisplayTestCommand, 0);

            // Init SetValuesCommand 
            var initSetValues = new List<byte>(DestinationAddress);
            initSetValues.AddRange(new byte[2]);
            initSetValues.AddRange(WinwingConstants.DisplayCmdHeaders["0201_E"]);
            initSetValues.CopyTo(SetValuesCommand, 0);

            // Init RefreshCommand
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

            //LcdTest("AllOff"); // used for testing
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

        private void ResetBaroCache()
        {
            LcdCurrentValuesCache[BaroHpa] = string.Empty;
            LcdCurrentValuesCache[BaroInHg] = string.Empty;
            LcdCurrentValuesCache[Qfe] = string.Empty;
        }

        private void SetBaroInternal(char[] baroChars, bool isInHg, bool isStd)
        {
            var baroThousands = DisplaySetValuesData["BaroThousands"];
            var baroHundreds = DisplaySetValuesData["BaroHundreds"];
            var baroTens = DisplaySetValuesData["BaroTens"];
            var baroOnes = DisplaySetValuesData["BaroOnes"];
            baroThousands.Data = BaroNumberCodes[baroChars[0]];
            baroHundreds.Data = BaroNumberCodes[baroChars[1]];
            baroTens.Data = BaroNumberCodes[baroChars[2]];
            baroOnes.Data = BaroNumberCodes[baroChars[3]];

            SetBytesDisplayCommand(baroThousands, SetValuesCommand);
            SetBytesDisplayCommand(baroHundreds, SetValuesCommand);
            SetBytesDisplayCommand(baroTens, SetValuesCommand);
            SetBytesDisplayCommand(baroOnes, SetValuesCommand);
            if (!isStd)
            {
                if (isInHg)
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["InHgDecPoint"], SetValuesCommand);
                }
                else
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["InHgNoDecPoint"], SetValuesCommand);
                }
            }
            else
            {                
                SetBytesDisplayCommand(DisplaySetValuesData["InHgNoDecPoint"], SetValuesCommand);
                SetBytesDisplayCommand(DisplaySetValuesData["NoBaro"], SetValuesCommand);
            }

            SendDisplayCommand(SetValuesCommand);
        }

        private void SetBaroHpa(string baro)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                int myBaro = (int)Convert.ToDouble(baro, CultureInfo.InvariantCulture);
                char[] baroChars = myBaro.ToString("D4", CultureInfo.InvariantCulture).ToCharArray();
                SetBaroInternal(baroChars, false, false);
            }
        }

        private void SetBaroInHg(string baro)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                int myBaro = (int)(Convert.ToDouble(baro, CultureInfo.InvariantCulture) * 100);
                char[] baroChars = myBaro.ToString("D4", CultureInfo.InvariantCulture).ToCharArray();
                SetBaroInternal(baroChars, true, false);
            }
        }

        private void SetBaroInHgOnOff(string inHg)
        {
            ResetBaroCache();
        }

        private void SetBaroStdOnOff(string baroStd)
        {
            int isBaroStd = (int)Convert.ToDouble(baroStd, CultureInfo.InvariantCulture);
            if (isBaroStd == 1)
            {                
                SetBaroInternal(new char[] { 'S', 't', 'd', '*' }, false, true);
            }
            ResetBaroCache();
            SendDisplayCommand(SetValuesCommand);
        }

        private void SetQfeOnOff(string qfe)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                int isQfe = (int)Convert.ToDouble(qfe, CultureInfo.InvariantCulture);
                if (isQfe == 1)
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["QfeBaro"], SetValuesCommand);
                }
                else
                {
                    SetBytesDisplayCommand(DisplaySetValuesData["QnhBaro"], SetValuesCommand);
                }
                SendDisplayCommand(SetValuesCommand);
            }
        }

        private void PrepareAndSendDisplayTestCommand(MsgEntry entry)
        {
            SetBytesDisplayCommand(entry, DisplayTestCommand);
            SendDisplayCommand(DisplayTestCommand);
        }

        private void EmptyDisplay()
        {
            var resetMsg = new MsgEntry { StartPos = 21, Mask = new byte[5], Data = new byte[5] };
            SetBytesDisplayCommand(resetMsg, SetValuesCommand);
            SendDisplayCommand(SetValuesCommand);
        }

        private void SendDisplayCommand(byte[] message)
        {
            MessageSender.SendDisplayCommands(new byte[][] { message, RefreshCommand });
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
        }

        private void SetLcdBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x01, brightness);
        }

        // "AllOn", "AllOff", "Half1On", "Half2On"        
        private void LcdTest(string command)
        {
            PrepareAndSendDisplayTestCommand(DisplayTestCommands[command]);
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
