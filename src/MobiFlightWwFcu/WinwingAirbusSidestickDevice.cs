using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingAirbusSidestickDevice : IWinwingDevice
    {
        public string Name { get => $"WinWing {StickType}"; }

        private WinwingMessageSender MessageSender = null;
        private string StickType = WinwingConstants.AIRBUS_STICK_R_NAME;
        private byte[] DestinationAddress = WinwingConstants.DEST_AIRBUS_STICK;
        private byte[] DestinationAddressVibration = WinwingConstants.DEST_AIRBUS_STICK_VIBRATION;

        private const string VIBRATION = "Vibration Percentage";
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LIGHT_PULSE = "Backlight Pulse On/Off";
       
        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
           

        public WinwingAirbusSidestickDevice(WinwingMessageSender sender, string stickType)
        {
            MessageSender = sender;
            StickType = stickType;

            DisplayNameToActionMapping.Add(VIBRATION, SetVibration);
            DisplayNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            DisplayNameToActionMapping.Add(LIGHT_PULSE, SetBacklightPulse);

            foreach (var displayName in GetDisplayNames())
            {
                LcdCurrentValuesCache.Add(displayName, string.Empty);
            }
        }


        public void Connect()
        {
            SetBacklightBrightness("20");
            SetVibration("0");
        }

        public void Shutdown()
        {
            SetBacklightBrightness("0");
            SetVibration("0");
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

        private void SetVibration(string level)
        {
            MessageSender.SetVibration(DestinationAddressVibration, 0x00, level);
        }

        private void SetBacklightPulse(string isOnString)
        {
            int value = (int)Convert.ToDouble(isOnString, CultureInfo.InvariantCulture);
            bool isOn = Convert.ToBoolean(value);
            MessageSender.SetPulseLight(DestinationAddress, isOn);
        }

        public List<string> GetLedNames()
        {
            // do nothing, there are no leds
            return new List<string>();
        }
    }
}
