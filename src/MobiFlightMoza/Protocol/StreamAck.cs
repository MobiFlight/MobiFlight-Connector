namespace MobiFlightMoza.Protocol
{
    /// <summary>A Reliable Stream cumulative ACK.</summary>
    internal readonly struct StreamAck
    {
        public ushort DestinationPort { get; }
        public ushort AcknowledgedIsn { get; }

        public StreamAck(ushort destinationPort, ushort acknowledgedIsn)
        {
            DestinationPort = destinationPort;
            AcknowledgedIsn = acknowledgedIsn;
        }
    }
}
