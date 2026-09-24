namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// One decoded SerialLink frame: outer command, device pair, and unescaped payload.
    /// </summary>
    internal sealed class SerialLinkMessage
    {
        public byte Command { get; }
        public byte DevicePair { get; }
        public byte[] Payload { get; }

        public SerialLinkMessage(byte command, byte devicePair, byte[] payload)
        {
            Command = command;
            DevicePair = devicePair;
            Payload = payload;
        }
    }
}
