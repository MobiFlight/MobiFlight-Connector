using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class Winwing3PdcDevice : IWinwingDevice
    {
        public string Name { get => $"WinWing {PdcType}"; }

        private WinwingMessageSender MessageSender = null;
        private string PdcType = WinwingConstants.PDC3NL_NAME;

        private byte[] DestinationAddress;
     
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
           

        public Winwing3PdcDevice(WinwingMessageSender sender, string pdcType)
        {
            MessageSender = sender;
            PdcType = pdcType;

            if (PdcType == WinwingConstants.PDC3NL_NAME || PdcType == WinwingConstants.PDC3NR_NAME)
            {
                DestinationAddress = WinwingConstants.DEST_3NPDC;
            }
            else if (PdcType == WinwingConstants.PDC3ML_NAME || PdcType == WinwingConstants.PDC3MR_NAME)
            {
                DestinationAddress = WinwingConstants.DEST_3MPDC;
            }           
           
            DisplayNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);

            foreach (var displayName in GetDisplayNames())
            {
                LcdCurrentValuesCache.Add(displayName, string.Empty);
            }
        }


        public void Connect()
        {
            SetBacklightBrightness("20");
        }

        public void Shutdown()
        {
            SetBacklightBrightness("0");
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
            // do nothing, there are no leds
        }

        public void SetDisplay(string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && LcdCurrentValuesCache[name] != value) // check cache
            {
                LcdCurrentValuesCache[name] = value;
                DisplayNameToActionMapping[name](value); // Execute Action
            }
        }

        private void SetBacklightBrightness(string brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);       
        }

        public List<string> GetLedNames()
        {
            // do nothing, there are no leds
            return new List<string>();
        }
    }
}
