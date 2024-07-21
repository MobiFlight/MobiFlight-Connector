using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingEfis : IWinwingDevice
    {
        private WinwingMessageSender MessageSender = null;

        private string EfisType = "Left";
        private byte[] DestinationAddressLighting;

        // https://docs.flybywiresim.com/pilots-corner/airliner-flying-guide/altitude-refs/
        private string BaroHpa { get => $"hPa Value {EfisType}"; }
        private string BaroInHg { get => $"inHg Value {EfisType}"; }
        private string BaroInHgOnOff { get => $"inHg On/Off {EfisType}"; }
        private string BaroStd { get => $"STD On/Off {EfisType}"; }
        private string Qfe { get => $"QFE On/Off {EfisType}"; }

        private const string ANN_LIGHT = "LCD Test On/Off";
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";
        private const string LED_BRIGHTNESS = "LED Percentage";

        private bool IsInHg = false;

        private Dictionary<string, MsgEntry> DisplayTestCommands = new Dictionary<string, MsgEntry>()
        {
            { "AllOn",          new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x02 } } },
            { "AllOff",         new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x06 } } },
            { "Half1On",        new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x07 } } },
            { "Half2On",        new MsgEntry { StartPos = 21, Mask = new byte[1], Data = new byte[] { 0x09 } } },
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
        };

        //                          Time                         Baro    Qxx
        // f0000b1a0dbf000002010000 eb7d1c 0000 0900000000000000 607d605b 02 00000000000000000000000000000000000000000000000000000000000000000000
        // 0               xx       12     15   17    20      24 25       29 30   
   
        private Dictionary<string, MsgEntry> DisplaySetValuesData = new Dictionary<string, MsgEntry>()
        {
            { "BaroThousands",  new MsgEntry { StartPos = 25, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x60 } } },
            { "BaroHundreds",   new MsgEntry { StartPos = 26, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x7d } } },
            { "BaroTens",       new MsgEntry { StartPos = 27, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x60 } } },
            { "BaroOnes",       new MsgEntry { StartPos = 28, Mask = new byte[] { 0b10000000 }, Data = new byte[] { 0x7a } } },
            { "InHgDecPoint",   new MsgEntry { StartPos = 26, Mask = new byte[] { 0b01111111 }, Data = new byte[] { 0b10000000 } } },
            { "InHgNoDecPoint", new MsgEntry { StartPos = 26, Mask = new byte[] { 0b01111111 }, Data = new byte[] { 0b00000000 } } },
            { "QfeBaro",        new MsgEntry { StartPos = 29, Mask = new byte[] { 0b00000000 }, Data = new byte[] { 0x01 } } },
            { "QnhBaro",        new MsgEntry { StartPos = 29, Mask = new byte[] { 0b00000000 }, Data = new byte[] { 0x02 } } },
        };

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, byte> LedIdentifiers;

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();
        
        private byte[] LightMessageData = new byte[2];

        private byte[] DisplayTestMessage = new byte[64];
        private byte[] SetValuesMessage = new byte[64];
        private byte[] ConfirmMessage = new byte[64];

        public WinwingEfis(WinwingMessageSender sender, string efisType)
        {
            MessageSender = sender;
            EfisType = efisType;

            int DEST_ADDRESS_POS = 4;
            int PAYLOAD_LENGTH_POS = 17;
            
            // Init DisplayTestMessage. MessageLength: 0x12, MessageId: 0x04, 0x01
            byte[] initDisplayTest = new byte[] { 0xf0, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x04, 0x01 };
            initDisplayTest.CopyTo(DisplayTestMessage, 0);
            DisplayTestMessage[PAYLOAD_LENGTH_POS] = 0x01;

            // Init SetValuesMessage MessageLength: 0x1a, MessageId: 0x02, 0x01
            byte[] initSetValues = new byte[] { 0xf0, 0x00, 0x00, 0x1a, 0x00, 0x00, 0x00, 0x00, 0x02, 0x01 };
            initSetValues.CopyTo(SetValuesMessage, 0);
            SetValuesMessage[PAYLOAD_LENGTH_POS] = 0x09;

            // Init ConfirmMessage MessageLength: 0x11, MessageId: 0x03, 0x01
            byte[] initConfirmValues = new byte[] { 0xf0, 0x00, 0x00, 0x11, 0x00, 0x00, 0x00, 0x00, 0x03, 0x01 };
            initConfirmValues.CopyTo(ConfirmMessage, 0);

            foreach (var entry in DisplaySetValuesData.Values)
            {
                SetBytesDisplayMessage(entry, SetValuesMessage);
            }

            if (efisType == "Left")
            {
                DestinationAddressLighting = WinwingConstants.DEST_EFISL;
                WinwingConstants.DEST_EFISL.CopyTo(DisplayTestMessage, DEST_ADDRESS_POS);
                WinwingConstants.DEST_EFISL.CopyTo(SetValuesMessage, DEST_ADDRESS_POS);
                WinwingConstants.DEST_EFISL.CopyTo(ConfirmMessage, DEST_ADDRESS_POS);
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
        }

        public void Connect()
        {
            SendDisplayMessage(SetValuesMessage); // Init display
            SetBacklightBrightness("20");
            SetLcdBrightness("100");           
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
                LightMessageData[0] = LedIdentifiers[led];
                LightMessageData[1] = state == 0 ? (byte)0 : (byte)1;
                MessageSender.SendLightControlMessage(DestinationAddressLighting, LightMessageData);
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
        }

        private void SetBaroInternal(char[] baroChars, bool isInHg)
        {
            var baroThousands = DisplaySetValuesData["BaroThousands"];
            var baroHundreds = DisplaySetValuesData["BaroHundreds"];
            var baroTens = DisplaySetValuesData["BaroTens"];
            var baroOnes = DisplaySetValuesData["BaroOnes"];
            baroThousands.Data = BaroNumberCodes[baroChars[0]];
            baroHundreds.Data = BaroNumberCodes[baroChars[1]];
            baroTens.Data = BaroNumberCodes[baroChars[2]];
            baroOnes.Data = BaroNumberCodes[baroChars[3]];

            SetBytesDisplayMessage(baroThousands, SetValuesMessage);
            SetBytesDisplayMessage(baroHundreds, SetValuesMessage);
            SetBytesDisplayMessage(baroTens, SetValuesMessage);
            SetBytesDisplayMessage(baroOnes, SetValuesMessage);
            if (isInHg)
            {
                SetBytesDisplayMessage(DisplaySetValuesData["InHgDecPoint"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["InHgNoDecPoint"], SetValuesMessage);
            }

            SendDisplayMessage(SetValuesMessage);
        }

        private void SetBaroHpa(string baro)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                int myBaro = (int)Convert.ToDouble(baro, CultureInfo.InvariantCulture);
                char[] baroChars = myBaro.ToString("D4", CultureInfo.InvariantCulture).ToCharArray();
                SetBaroInternal(baroChars, false);
            }
        }

        private void SetBaroInHg(string baro)
        {
            if (LcdCurrentValuesCache[BaroStd] != "1")
            {
                int myBaro = (int)(Convert.ToDouble(baro, CultureInfo.InvariantCulture) * 100);
                char[] baroChars = myBaro.ToString("D4", CultureInfo.InvariantCulture).ToCharArray();
                SetBaroInternal(baroChars, true);
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
                //SetBytesDisplayMessage(DisplaySetValuesData["TODO STD"], SetValuesMessage);
            }
            ResetBaroCache();
            SendDisplayMessage(SetValuesMessage);
        }

        private void SetQfeOnOff(string qfe)
        {
            int isQfe = (int)Convert.ToDouble(qfe, CultureInfo.InvariantCulture);
            if (isQfe == 1)
            {
                SetBytesDisplayMessage(DisplaySetValuesData["QfeBaro"], SetValuesMessage);
            }
            else
            {
                SetBytesDisplayMessage(DisplaySetValuesData["QnhBaro"], SetValuesMessage);
            }           
            SendDisplayMessage(SetValuesMessage);
        }

        private void PrepareAndSendDisplayTestMessage(MsgEntry entry)
        {
            SetBytesDisplayMessage(entry, DisplayTestMessage);
            SendDisplayMessage(DisplayTestMessage);
        }

        private void EmptyDisplay()
        {
            var resetMsg = new MsgEntry { StartPos = 25, Mask = new byte[5], Data = new byte[5] };
            SetBytesDisplayMessage(resetMsg, SetValuesMessage);
            SendDisplayMessage(SetValuesMessage);
        }

        private void SendDisplayMessage(byte[] message)
        {
            MessageSender.SendDisplayMessage(message);
            MessageSender.SendDisplayMessage(ConfirmMessage); // Always at that second message
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

            LightMessageData[0] = type;
            LightMessageData[1] = byteValue;
            MessageSender.SendLightControlMessage(DestinationAddressLighting, LightMessageData);
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

        private void SetBytesDisplayMessage(MsgEntry msgEntry, byte[] message)
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
