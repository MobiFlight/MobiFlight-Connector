namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Reliable Stream request type byte. The MOZA protocol documentation calls this
    /// field "Magic," but it discriminates message type (SYN1/SYN2/TRANS/FIN) rather
    /// than being a fixed validation constant, so it's named for what it does instead.
    /// </summary>
    internal enum StreamMessageType : byte
    {
        Fin = 0x00,
        Trans = 0x01,
        Syn1 = 0x80,
        Syn2 = 0x81,
    }
}
