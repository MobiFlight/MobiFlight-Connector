using System;
using System.Collections.Generic;

namespace MobiFlight.Joysticks.VKB
{
    // This holds all LED channels of a single device along with a label list for speedy retrieval.
    internal class VKBLedContainer
    {
        private readonly SortedList<byte, VKBLed> Leds = new SortedList<byte, VKBLed>();
        private readonly Dictionary<String, (byte, byte)> Labels = new Dictionary<string, (byte, byte)>();

        public void AddChannel(JoystickOutput output)
        {
            // If this is the first channel of the LED, we need to create the LED first.
            if (!Leds.ContainsKey(output.Byte))
            {
                Leds.Add(output.Byte, new VKBLed(output.Byte));
            }

            Leds[output.Byte].AddChannel(output);
            Labels.Add(output.Label, (output.Byte, output.Bit));
        }

        public void UpdateState (string Label, byte State)
        {
            // Find the LED and its color channel and update it.
            var updateLabels = Label.Split('|'); // Multiple output pin support
            foreach (String updateLabel in updateLabels)
            {
                if (Labels.ContainsKey(updateLabel))
                {
                    (byte Byte, byte Bit) = Labels[updateLabel];
                    Leds[Byte]?.SetState(Bit, State);
                }
            }
        }

        public byte[] CreateMessage()
            // Serialize an entire feature report by iterating over all LEDs.
        {
            int LedCount = 0;
            var buffer = new byte[129]; // Length defined by report descriptor.
            buffer[0] = 0x59; // Report ID
            buffer[1] = 0xA5; // Header preamble
            buffer[2] = 0x0A; // Opcode
                              // Bytes 4-7 are listed as "reserved", but have no details.
                              // They are basically ignored, might as well leave them empty.
            buffer[7] = (byte)LedCount;
            int bufferoffset = 8;
            foreach (KeyValuePair<byte, VKBLed> entry in Leds)
            {
                // Only include changed LEDs
                if (!entry.Value.IsChanged())
                {
                    continue;
                }

                Buffer.BlockCopy(entry.Value.Serialize(), 0, buffer, bufferoffset, 4); // Serialize single LED
                bufferoffset += 4;
                LedCount++;
                // We are not able to update more LEDs than we can fit into the report. Once we have hit the limit, we need to
                // leave the remaining LEDs for the next report. Since they are not expected to change again that quickly, we
                // can still expect to be able to empty our buffer eventually. Practically, single LEDs should be more common
                // than more LEDs than we can fit.
                if (bufferoffset > 126)
                {
                    break;
                }
            }
            buffer[7] = (byte)LedCount;
            return buffer;
        }
    }
}
