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
            if (!Leds.ContainsKey(output.Byte))
                Leds.Add(output.Byte, new VKBLed(output.Byte));
            Leds[output.Byte].AddChannel(output);
            Labels.Add(output.Label, (output.Byte, output.Bit));
        }
        public void UpdateState (string Label, byte State)
        {
            // Find the LED and its color channel and update it.
            String[] updateLabels = Label.Split('|'); // Multiple output pin support
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
            foreach(KeyValuePair<byte,VKBLed> entry in Leds)
            {
                if (entry.Value.IsChanged()) LedCount++;
            }
            byte[] buffer = new byte[129]; // Length defined by report descriptor.
            buffer[0] = 0x59; // Report ID
            buffer[1] = 0xA5; // Header preamble
            buffer[2] = 0x0A; // Opcode
                              // Bytes 4-7 are listed as "reserved", but have no details.
                              // They are basically ignored, might as well leave them empty.
            buffer[7] = (byte)LedCount;
            int bufferoffset = 8;
            foreach (KeyValuePair<byte, VKBLed> entry in Leds)
            {
                if (!entry.Value.IsChanged()) continue;
                Buffer.BlockCopy(entry.Value.Serialize(), 0, buffer, bufferoffset, 4); // Serialize single LED
                bufferoffset += 4;
                if (bufferoffset > 126) break; // We are not able to update more than that at once without
                                               // exceeding our buffer. But next call, only the remaning LEDs
                                               // ought to still return true on IsChanged().
                                               // In practice, we are more likely to deal with single LEDs here anyway.
            }
            return buffer;
        }
    }
}
