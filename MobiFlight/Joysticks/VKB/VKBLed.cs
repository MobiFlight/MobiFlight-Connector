namespace MobiFlight.Joysticks.VKB
{
    internal class VKBLed
    {
        /*
         * VKB LEDs come in three types: Mono, bi-color and RGB.
         * Mobiflight uses a bit/byte system to handle LEDs in generic controllers (e.g. Honeycomb), where each LED is a bit flag.
         * For VKB devices, this is co-opted to encode LED IDs (assigned in VKBDevCfg for each sub-device) and color channels.
         * Monochrome LEDs just have "bit" 0 to turn the LED on and off
         * Bicolor LEDs have bit 0 for color1 (usually green/blue) and bit 1 for color2 (usually red)
         * RGB LEDs have bit 0 for the red channel, 1 for green and 2 for blue.
         */
        public enum ColorMode : byte // 1, 2 and 1+2 are relevant for Mobiflight. 1/2 and 2/1 are used with hardware flash patterns.
        {
            Color1 = 0,
            Color2 = 1,
            Color1_2 = 2,
            Color2_1 = 3,
            Color1plus2 = 4
        };
        public enum FlashPattern : byte // MobiFlight LEDs are not complex enough to take advantage of hardware flash patterns yet.
        {
            Off = 0,
            Constantly = 1,
            Slow = 2,
            Fast = 3,
            UltraFast = 4
        };

        private readonly JoystickOutputDevice[] LedChannels = new JoystickOutputDevice[3];
        private bool dirty = true; // There have been changes since the last time the values were sent to the device
                                   // Initialized as true to ensure that no output gets ignored.
        private bool used = false; // Changes have been made to the LED since initialization.
                                   // This prevents packets from being sent when no output is configured.
        private bool greenred = false; // Special handling of green/red LEDs to get a decent amber when both are on
        private readonly byte LedId = 0;
        private const byte defaultBrightness = 5; // VKB LEDs have 7 brightness levels, but MobiFlight does not support PWM on HIDs

        public VKBLed(byte id)
        {
            LedId = id;
        }

        public void AddChannel(JoystickOutput output)
        {
            // Check if channel already exists
            if (LedChannels[output.Bit] != null)
            {
                return;
            }

            LedChannels[output.Bit] = new JoystickOutputDevice
            {
                Label = output.Label,
                Name = output.Id,
                Byte = output.Byte,
                Bit = output.Bit,
                State = 0
            };

            // Green/Red LEDs need special treatment, so let us track that:
            if (LedChannels[0] != null
             && LedChannels[0].Label.Contains("Green") // feels like a dirty hack, but requires no additional config data
             && LedChannels[1] != null
             && LedChannels[1].Label.Contains("Red"))
            {
                greenred = true;
            }
        }

        public void SetState(byte channel, byte state)
        {
            used = true; // Ensure that the LED state is only sent if an output is associated with the LED.
            if (LedChannels[channel].State != state)
            {
                dirty = true;
            }
            LedChannels[channel].State = state;
        }

        public byte[] Serialize()
        {
            // Collect the states of each channel and turn it into an LED command block
            var LedBlock = new byte[] { 0, 0, 0, 0 };
            var pattern = FlashPattern.Off;
            var colmode = ColorMode.Color1;
            var ColorIntensity = new byte[2, 3] { { 0, 0, 0 }, { 0, 0, 0 } };
            int activeLeds = 0;
            // Count used channels (to compensate for multiplex dimming on bi-color LEDs) and ensure the LED turns on
            foreach (JoystickOutputDevice channel in LedChannels)
            {
                if ((channel?.State ?? 0) != 0)
                {
                    pattern = FlashPattern.Constantly;
                    activeLeds++;
                }
            }

            // Handle different LED Types:

            // Check for RGB LED
            if (LedChannels[2] != null)
            {
                // With RGB LEDs, use Color1 with RGB values;
                foreach (JoystickOutputDevice channel in LedChannels)
                {
                    ColorIntensity[0, channel.Bit] = (byte)(channel.State * defaultBrightness);
                }
            }

            // Check for green/red bicolor LED
            else if (greenred)
            {
                // Green/Red LEDs need special treatment, due to the always-green feature and the overpowering green at high intensities
                var color = ((LedChannels[0].State != 0) ? 1 : 0) + ((LedChannels[1].State != 0) ? 2 : 0); // 0: off, 1: green, 2: red, 3: amber
                byte brightnessG = defaultBrightness;
                byte brightnessR = defaultBrightness;
                switch (color)
                {
                    case 0:
                        brightnessG = 0;
                        brightnessR = 0;
                        pattern = FlashPattern.Constantly;  // Off would turn the LEDs on in a dim green unless a jumper
                        colmode = ColorMode.Color1;         // is changed in hardware, so Color1 at brightness zero it is
                        break;
                    case 1:
                        brightnessG = 3;
                        brightnessR = 0;
                        pattern = FlashPattern.Constantly;
                        colmode = ColorMode.Color1;
                        break;
                    case 2:
                        brightnessG = 0;
                        brightnessR = 5;
                        pattern = FlashPattern.Constantly;
                        colmode = ColorMode.Color2;
                        break;
                    case 3:
                        brightnessG = 2;
                        brightnessR = 7;
                        pattern = FlashPattern.Constantly;
                        colmode = ColorMode.Color1plus2;
                        break;
                    default: // no default, all cases handled
                        break;
                }
                ColorIntensity[0, 1] = brightnessG;
                ColorIntensity[1, 1] = brightnessR;
            }

            // Handle other LED types
            else
            {
                // Single or bicolor LED, other than red/green LEDs
                byte brightness = defaultBrightness;
                if (activeLeds > 1)
                {
                    brightness = 7; // Since Color1+2 means the controller is alternating between both colors (like PWM),
                                    // we need to increase the brightness over the base brightness.
                }
                for (int col = 0; col < ColorIntensity.GetLength(0); col++)
                {
                    var channelstate = new byte[2] { (LedChannels[0]?.State ?? 0), (LedChannels[1]?.State ?? 0) };
                    if (channelstate[0] != 0)
                    {
                        ColorIntensity[0, col] = (byte)(channelstate[0] * brightness);
                    }
                    if (channelstate[1] != 0)
                    {
                        ColorIntensity[1, col] = (byte)(channelstate[1] * brightness);
                        if (channelstate[0] != 0)
                        {
                            colmode = ColorMode.Color1plus2;
                        }
                        else
                        {
                            colmode = ColorMode.Color2;
                        }

                    }
                    ColorIntensity[0, col] = (byte)((LedChannels[0]?.State ?? 0) * brightness);
                    ColorIntensity[1, col] = (byte)((LedChannels[1]?.State ?? 0) * brightness);
                }
            }

            // Serialize the block into a tightly-packed structure with several 3-bit variables - USB is LSB-first.
            LedBlock[0] = LedId;
            LedBlock[1] = (byte)(ColorIntensity[0, 0] | ColorIntensity[0, 1] << 3 | (ColorIntensity[0, 2] & 0x03) << 6);
            LedBlock[2] = (byte)(ColorIntensity[0, 2] >> 2 | ColorIntensity[1, 0] << 1 | ColorIntensity[1, 1] << 4 | (ColorIntensity[1, 2] & 0x01) << 7);
            LedBlock[3] = (byte)(ColorIntensity[1, 2] >> 1 | (byte)pattern << 2 | (byte)colmode << 5);
            dirty = false; // The message will get sent out, so we can consider the current state the new base state.

            return LedBlock;
        }

        public bool IsChanged()
        {
            return dirty && used; // The values have changed since the last message was sent.
        }
    }
}
