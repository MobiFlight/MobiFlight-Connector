using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingPto2Device : IWinwingDevice
    {
        public string Name { get; } = "WinWing PTO2";

        private IWinwingMessageSender MessageSender = null;
        private byte[] DestinationAddress = WinwingConstants.DEST_PTO2;

        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<byte>> OutputNameToActionMapping = new Dictionary<string, Action<byte>>();

        private const string BACK_BRIGHTNESS = "Backlight Percentage";     // 0x00     
        private const string LG_BRIGHTNESS   = "Landing Gear Percentage";  // 0x01
        private const string SL_BRIGHTNESS   = "SL Percentage";            // 0x02 
        private const string FLAG_BRIGHTNESS = "FLAG Percentage";          // 0x03


        private Dictionary<string, byte> LedIdentifiers = new Dictionary<string, byte>()
        {     
            { "MASTER_CAUTION",   0x04 },
            { "JETT",      0x05 },
            { "CTR",       0x06 },
            { "LI",        0x07 },
            { "LO",        0x08 },
            { "RO",        0x09 },
            { "RI",        0x0a },
            { "FLAPS",     0x0b },
            { "NOSE",      0x0c },
            { "FULL",      0x0d },
            { "RIGHT",     0x0e },
            { "LEFT",      0x0f },
            { "HALF",      0x10 },
            { "HOOK",      0x11 },
        };

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();        

        public WinwingPto2Device(IWinwingMessageSender sender)
        {
            MessageSender = sender;

            // Add output options
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            OutputNameToActionMapping.Add(LG_BRIGHTNESS,   SetLandingGearBrightness);
            OutputNameToActionMapping.Add(SL_BRIGHTNESS,   SetSlBrightness);
            OutputNameToActionMapping.Add(FLAG_BRIGHTNESS, SetFlagBrightness);

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
            SetLandingGearBrightness(0);
            SetSlBrightness(0);
            SetFlagBrightness(0);
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

        private void SetFlagBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x03, brightness);
        }

        private void SetSlBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x02, brightness);
        }

        private void SetLandingGearBrightness(byte brightness)
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
