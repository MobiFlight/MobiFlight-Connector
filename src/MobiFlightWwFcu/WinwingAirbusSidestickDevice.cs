using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal class WinwingAirbusSidestickDevice : IWinwingDevice
    {
        public string Name { get => $"WinWing {StickType}"; }

        private IWinwingMessageSender MessageSender = null;
        private string StickType = WinwingConstants.AIRBUS_STICK_R_NAME;
        private byte[] DestinationAddress = WinwingConstants.DEST_AIRBUS_STICK;
        private byte[] DestinationAddressVibration = WinwingConstants.DEST_AIRBUS_STICK_VIBRATION_R;

        private const string VIBRATION = "Vibration Percentage";
        private const string BACK_BRIGHTNESS = "Backlight Percentage";
        private const string LIGHT_PULSE = "Backlight Pulse On/Off";
       
        private Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        private Dictionary<string, Action<byte>> OutputNameToActionMapping = new Dictionary<string, Action<byte>>();

        private Dictionary<string, string> LcdCurrentValuesCache = new Dictionary<string, string>();
        private Dictionary<string, byte> LedCurrentValuesCache = new Dictionary<string, byte>();


        public WinwingAirbusSidestickDevice(IWinwingMessageSender sender, string stickType)
        {
            MessageSender = sender;
            StickType = stickType;

            if (StickType == WinwingConstants.AIRBUS_STICK_L_NAME)
            {
                DestinationAddressVibration = WinwingConstants.DEST_AIRBUS_STICK_VIBRATION_L;
            }
            else
            {
                DestinationAddressVibration = WinwingConstants.DEST_AIRBUS_STICK_VIBRATION_R;
            }

            // Add output options
            OutputNameToActionMapping.Add(VIBRATION, SetVibration);
            OutputNameToActionMapping.Add(BACK_BRIGHTNESS, SetBacklightBrightness);
            OutputNameToActionMapping.Add(LIGHT_PULSE, SetBacklightPulse);


            foreach (var ledName in GetLedNames())
            {
                LedCurrentValuesCache.Add(ledName, 255);
            }
        }


        public void Connect()
        {
            SetBacklightBrightness(20);
            SetVibration(0);
        }

        public void Shutdown()
        {
            SetBacklightBrightness(0);
            SetVibration(0);
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

        private void SetVibration(byte level)
        {
            MessageSender.SetVibration(DestinationAddressVibration, 0x00, level);
        }

        private void SetBacklightPulse(byte isOnValue)
        {     
            bool isOn = Convert.ToBoolean(isOnValue);
            MessageSender.SetPulseLight(DestinationAddress, isOn);
        }

        public List<string> GetLedNames()
        {
            List<string> ledNames = new List<string>();     
            ledNames.AddRange(OutputNameToActionMapping.Keys.ToList());
            return ledNames;
        }

        public void Stop()
        {
            SetVibration(0);
        }
    }
}
