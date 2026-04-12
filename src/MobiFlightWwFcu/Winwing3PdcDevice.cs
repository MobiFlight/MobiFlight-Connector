using System;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class Winwing3PdcDevice : IWinwingDevice
    {
        public string Name { get => $"WinWing {PdcType}"; }

        private IWinwingMessageSender MessageSender = null;
        private string PdcType = WinwingConstants.PDC3NL_NAME;

        private byte[] DestinationAddress;
     
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<byte>> OutputNameToActionMapping = new Dictionary<string, Action<byte>>();
        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();


        public Winwing3PdcDevice(IWinwingMessageSender sender, string pdcType)
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

            // Add output options         
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
 
            foreach (var ledName in GetLedNames())
            {
                LedCurrentValuesCache.Add(ledName, 255);
            }
        }


        public void Connect()
        {
            SetBacklightBrightness(50);
        }

        public void Shutdown()
        {
            SetBacklightBrightness(0);
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
                if (OutputNameToActionMapping.TryGetValue(led, out Action<byte> action))
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

        private void SetBacklightBrightness(byte brightness)
        {
            MessageSender.SetBrightness(DestinationAddress, 0x00, brightness);       
        }

        public List<string> GetLedNames()
        {
            List<string> ledNames = new List<string>();
            ledNames.AddRange(OutputNameToActionMapping.Keys.ToList());
            return ledNames;
        }

        public void Stop()
        {
            // Do nothing
        }
    }
}
