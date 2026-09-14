namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// One decoded Reliable Stream request. <see cref="ApplicationData"/> is
    /// only meaningful for <see cref="StreamMessageType.Trans"/>; <see cref="AnnouncedPort"/> and
    /// <see cref="Version"/> only for <see cref="StreamMessageType.Syn1"/>/<see cref="StreamMessageType.Syn2"/>.
    /// </summary>
    internal sealed class StreamRequest
    {
        public ushort DestinationPort { get; }
        public StreamMessageType MessageType { get; }
        public ushort Isn { get; }
        public byte[] ApplicationData { get; }
        public ushort? AnnouncedPort { get; }
        public byte? Version { get; }

        public StreamRequest(
            ushort destinationPort,
            StreamMessageType messageType,
            ushort isn,
            byte[] applicationData = null,
            ushort? announcedPort = null,
            byte? version = null)
        {
            DestinationPort = destinationPort;
            MessageType = messageType;
            Isn = isn;
            ApplicationData = applicationData ?? [];
            AnnouncedPort = announcedPort;
            Version = version;
        }
    }
}
