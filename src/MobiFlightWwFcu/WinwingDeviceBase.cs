using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MobiFlightWwFcu
{
    internal abstract class WinwingDeviceBase : IWinwingDevice
    {
        protected const string BACK_BRIGHTNESS = "Backlight Percentage";
        protected const string LCD_BRIGHTNESS  = "LCD Percentage";
        protected const string LED_BRIGHTNESS  = "LED Percentage";

        protected readonly IWinwingMessageSender MessageSender;
        protected byte[] DestinationAddress;

        protected readonly Dictionary<string, Action<string>> DisplayNameToActionMapping = new Dictionary<string, Action<string>>();
        protected readonly Dictionary<string, Action<byte>>   OutputNameToActionMapping  = new Dictionary<string, Action<byte>>();
        protected readonly Dictionary<string, byte>           LedIdentifiers             = new Dictionary<string, byte>();
        protected readonly Dictionary<string, string>         LcdCurrentValuesCache      = new Dictionary<string, string>();
        protected readonly Dictionary<string, byte>           LedCurrentValuesCache      = new Dictionary<string, byte>();

        protected WinwingDeviceBase(IWinwingMessageSender sender, byte[] destinationAddress)
        {
            MessageSender = sender;
            DestinationAddress = destinationAddress;
        }

        public abstract string Name { get; }

        public virtual List<string> GetDisplayNames()
            => DisplayNameToActionMapping.Keys.ToList();

        public virtual List<string> GetInternalDisplayNames()
            => new List<string>();

        public virtual List<string> GetLedNames()
        {
            var names = new List<string>(LedIdentifiers.Keys);
            names.AddRange(OutputNameToActionMapping.Keys);
            return names;
        }

        public virtual void SetDisplay(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            if (LcdCurrentValuesCache[name] == value) return;
            LcdCurrentValuesCache[name] = value;
            DisplayNameToActionMapping[name](value);
        }

        public virtual void SetLed(string led, byte state)
        {
            if (string.IsNullOrEmpty(led)) return;
            if (LedCurrentValuesCache[led] == state) return;

            if (LedIdentifiers.TryGetValue(led, out byte ledId))
            {
                // Classic on/off LED (FCU, EFIS, PAP3, ...): the name maps to a hardware
                // LED id, any non-zero state is normalized to 1 and sent as a light
                // control message.
                LedCurrentValuesCache[led] = state;
                byte adjustedState = state == 0 ? (byte)0 : (byte)1;
                MessageSender.SendLightControlMessage(DestinationAddress, ledId, adjustedState);
            }
            else if (OutputNameToActionMapping.TryGetValue(led, out Action<byte> action))
            {
                // Only used by the newer devices (ECAM, PTO2, AGP, TCAS, throttle,
                // sidestick, 3PDC): they expose value outputs such as brightness
                // channels or vibration, so the raw state byte is passed unchanged
                // to a device-specific action instead of a plain on/off message.
                LedCurrentValuesCache[led] = state;
                action(state);
            }
        }

        public virtual void Stop()
        {
            TurnOffAllLEDs();
        }

        public abstract void Connect();
        public abstract void Shutdown();

        protected void TurnOffAllLEDs()
        {
            foreach (var name in LedIdentifiers.Keys)
            {
                SetLed(name, 0);
            }
        }

        // Subclasses call this after they have populated DisplayNameToActionMapping,
        // OutputNameToActionMapping, and LedIdentifiers.
        protected void InitializeCaches()
        {
            foreach (var name in GetDisplayNames())
            {
                if (!LcdCurrentValuesCache.ContainsKey(name))
                {
                    LcdCurrentValuesCache.Add(name, string.Empty);
                }
            }
            foreach (var name in GetLedNames())
            {
                if (!LedCurrentValuesCache.ContainsKey(name))
                {
                    LedCurrentValuesCache.Add(name, 255);
                }
            }
        }

        protected Action<byte> Brightness(byte channel)
            => state => MessageSender.SetBrightness(DestinationAddress, channel, state);

        protected Action<string> BrightnessFromString(byte channel)
            => s => MessageSender.SetBrightness(DestinationAddress, channel, (byte)AsDouble(s));

        // Invokes a brightness action registered in DisplayNameToActionMapping (FCU/PAP3/EFIS-style).
        protected void InvokeDisplayBrightness(string name, byte value)
            => DisplayNameToActionMapping[name](value.ToString(CultureInfo.InvariantCulture));

        // Invokes a brightness action registered in OutputNameToActionMapping (Ecam/Pto2/Agp/...-style).
        protected void InvokeOutputBrightness(string name, byte value)
            => OutputNameToActionMapping[name](value);

        protected static int    AsInt(string s)    => (int)Convert.ToDouble(s, CultureInfo.InvariantCulture);
        protected static bool   AsBool(string s)   => AsInt(s) != 0;
        protected static double AsDouble(string s) => Convert.ToDouble(s, CultureInfo.InvariantCulture);
    }
}
