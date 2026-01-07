using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingAgpDevice : IWinwingDevice
    {
        public string Name { get; } = "WinWing AGP";

        private IWinwingMessageSender MessageSender = null;
        private byte[] DestinationAddress = WinwingConstants.DEST_AGP;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<byte>> OutputNameToActionMapping = new Dictionary<string, Action<byte>>();

        private const string BACK_BRIGHTNESS = "Backlight Percentage";     
        private const string LED_BRIGHTNESS = "LED Percentage";
        private const string LCD_BRIGHTNESS = "LCD Percentage";

        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {
            { "GEAR_1_UNLOCKED",    0x03 },
            { "GEAR_2_UNLOCKED",    0x04 },
            { "GEAR_3_UNLOCKED",    0x05 },            
            { "GEAR_1_LOCKED",      0x07 },
            { "GEAR_2_LOCKED",      0x08 },
            { "GEAR_3_LOCKED",      0x09 },
            { "BRK_FAN_HOT",        0x06 },
            { "BRK_FAN_ON",         0x0a },
            { "AUTO_BRK_LO_DECEL",  0x0b },
            { "AUTO_BRK_MED_DECEL", 0x0c },
            { "AUTO_BRK_MAX_DECEL", 0x0d },
            { "AUTO_BRK_LO_ON",     0x0e },
            { "AUTO_BRK_MED_ON",    0x0f },
            { "AUTO_BRK_MAX_ON",    0x10 },
            { "TERR_ON_ND_ON",      0x11 },
            { "GEAR_DOWN_RED_ARROW",0x12 },
        };

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();        

        public WinwingAgpDevice(IWinwingMessageSender sender)
        {
            MessageSender = sender;

            // Add output options
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            OutputNameToActionMapping.Add(LED_BRIGHTNESS, SetLedBrightness);
            OutputNameToActionMapping.Add(LCD_BRIGHTNESS, SetLcdBrightness);

            foreach (var ledName in GetLedNames())
            {
                LedCurrentValuesCache.Add(ledName, 255);
            }
        }

        public void Connect()
        {            
            SetBacklightBrightness(50);
            SetLcdBrightness(100);
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
            SetLcdBrightness(0);
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
            MessageSender.SetBrightness(DestinationAddress, 0x02, brightness);
        }

        private void SetBacklightBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);
        }

        private void SetLcdBrightness(byte brightness)
        {            
            MessageSender.SetBrightness(DestinationAddress, 0x01, brightness);
        }
        
        public void Stop()
        {
            TurnOffAllLEDs();
        }
    }
}
