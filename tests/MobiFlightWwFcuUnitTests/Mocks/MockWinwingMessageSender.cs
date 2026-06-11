using MobiFlightWwFcu;

namespace MobiFlightWwFcuUnitTests.Mocks
{
    internal class MockWinwingMessageSender : IWinwingMessageSender
    {
        public List<DisplayCommandMessage> DisplayCommandsSent { get; } = new List<DisplayCommandMessage>();
        public List<LightControlMessage> LightControlCommands { get; } = new List<LightControlMessage>();
        public List<BrightnessMessage> BrightnessCommands { get; } = new List<BrightnessMessage>();
        public List<VibrationMessage> VibrationCommands { get; } = new List<VibrationMessage>();
        public List<PulseLightMessage> PulseLightCommands { get; } = new List<PulseLightMessage>();
        public List<byte[]> CduDisplayBytes { get; } = new List<byte[]>();

        public void Reset()
        {
            DisplayCommandsSent.Clear();
            LightControlCommands.Clear();
            BrightnessCommands.Clear();
            VibrationCommands.Clear();
            PulseLightCommands.Clear();
            CduDisplayBytes.Clear();
        }

        public bool IsConnected() => true;
        public void Connect() { }
        public void Shutdown() { }

        public void SendDisplayCommands(IList<byte[]> commands)
        {
            DisplayCommandsSent.Add(new DisplayCommandMessage
            {
                Commands = commands.Select(c => (byte[])c.Clone()).ToList()
            });

            // Console output captured by the TRX logger; supports the
            // "test-then-extract-expected-bytes" workflow described in CLAUDE.md.
            Console.WriteLine("SendDisplayCommands called with {0} command(s):", commands.Count);
            for (int i = 0; i < commands.Count; i++)
            {
                var bytes = commands[i];
                var hexValues = string.Join(", ", bytes.Select(b => string.Format("0x{0:X2}", b)));
                Console.WriteLine("  Command {0}: new byte[] {{ {1} }}", i, hexValues);
            }
            Console.WriteLine();
        }

        public void SendCduDisplayBytes(byte[] byteList)
        {
            CduDisplayBytes.Add((byte[])byteList.Clone());

            // Console output captured by the TRX logger; supports the
            // "test-then-extract-expected-bytes" workflow described in CLAUDE.md.
            var hexValues = string.Join(", ", byteList.Select(b => string.Format("0x{0:X2}", b)));
            Console.WriteLine("SendCduDisplayBytes called with {0} byte(s):", byteList.Length);
            Console.WriteLine("  CduBytes: new byte[] {{ {0} }}", hexValues);
            Console.WriteLine();
        }

        public void SendLightControlMessage(byte[] destination, byte type, byte value)
        {
            LightControlCommands.Add(new LightControlMessage
            {
                Destination = (byte[])destination.Clone(),
                Type = type,
                Value = value
            });
        }

        public void SetBrightness(byte[] destinationAddress, byte type, byte brightness)
        {
            BrightnessCommands.Add(new BrightnessMessage
            {
                DestinationAddress = (byte[])destinationAddress.Clone(),
                Type = type,
                Brightness = brightness.ToString()
            });
        }

        public void SetVibration(byte[] destinationAddress, byte type, byte level)
        {
            VibrationCommands.Add(new VibrationMessage
            {
                DestinationAddress = (byte[])destinationAddress.Clone(),
                Type = type,
                Level = level
            });
        }

        public void SetPulseLight(byte[] destinationAddress, bool isOn)
        {
            PulseLightCommands.Add(new PulseLightMessage
            {
                DestinationAddress = (byte[])destinationAddress.Clone(),
                IsOn = isOn
            });
        }

        public void SendHeartBeatMessage() { }
        public void SendRequestFirmwareMessage() { }
    }

    internal class DisplayCommandMessage
    {
        public List<byte[]> Commands { get; set; } = null!;
    }

    internal class LightControlMessage
    {
        public byte[] Destination { get; set; } = null!;
        public byte Type { get; set; }
        public byte Value { get; set; }
    }

    internal class BrightnessMessage
    {
        public byte[] DestinationAddress { get; set; } = null!;
        public byte Type { get; set; }
        public string Brightness { get; set; } = null!;
    }

    internal class VibrationMessage
    {
        public byte[] DestinationAddress { get; set; } = null!;
        public byte Type { get; set; }
        public byte Level { get; set; }
    }

    internal class PulseLightMessage
    {
        public byte[] DestinationAddress { get; set; } = null!;
        public bool IsOn { get; set; }
    }
}
