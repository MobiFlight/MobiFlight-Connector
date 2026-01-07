using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingEcamDevice : IWinwingDevice
    {
        public string Name { get; } = "WinWing ECAM";

        private IWinwingMessageSender MessageSender = null;
        private byte[] DestinationAddress = WinwingConstants.DEST_ECAM;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<byte>> OutputNameToActionMapping = new Dictionary<string, Action<byte>>();

        private const string BACK_BRIGHTNESS = "Backlight Percentage";     
        private const string LED_BRIGHTNESS = "LED Percentage"; 

        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {
            { "EMER_CANC",  0x03 },
            { "ENG",        0x04 },
            { "BLEED",      0x05 },
            { "PRESS",      0x06 },
            { "ELEC",       0x07 },
            { "HYD",        0x08 },
            { "FUEL",       0x09 },
            { "APU",        0x0a },
            { "COND",       0x0b },
            { "DOOR",       0x0c },
            { "WHEEL",      0x0d },
            { "FCTL",       0x0e },
            { "CLR_L",      0x0f },
            { "STS",        0x10 },
            { "CLR_R",      0x11 },
        };

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();        

        public WinwingEcamDevice(IWinwingMessageSender sender)
        {
            MessageSender = sender;

            // Add output options
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            OutputNameToActionMapping.Add(LED_BRIGHTNESS, SetLedBrightness);

            foreach (var ledName in GetLedNames())
            {
                LedCurrentValuesCache.Add(ledName, 255);
            }

        }

        public void Connect()
        {            
            SetBacklightBrightness(50);
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
            SetBacklightBrightness(0);
            TurnOffAllLEDs();        
        }

        public List<string> GetLedNames()
        {
            List<string> ledNames = new List<string>();
            ledNames.AddRange(LedIdentifiers.Keys.ToList());
            ledNames.AddRange(OutputNameToActionMapping.Keys.ToList()); 
            return ledNames;
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
                if (LedIdentifiers.TryGetValue(led, out byte ledType))
                {
                    LedCurrentValuesCache[led] = state;
                    byte stateAdjusted = state == 0 ? (byte)0 : (byte)1;
                    MessageSender.SendLightControlMessage(DestinationAddress, ledType, stateAdjusted);
                }
                else if (OutputNameToActionMapping.TryGetValue(led, out Action<byte> action)) 
                {
                    action(state);
                }
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


        private void SetLedBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x01, brightness);
        }

        private void SetBacklightBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);
        }


        public void Stop()
        {
            TurnOffAllLEDs();
        }
    }
}
