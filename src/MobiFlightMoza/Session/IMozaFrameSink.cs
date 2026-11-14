namespace MobiFlightMoza.Session
{
    /// <summary>
    /// The write side of the CDC connection. Mirrors how <c>HidReportReceiver</c> reads
    /// from a plain <see cref="System.IO.Stream"/> - this is the seam tests fake instead
    /// of touching a real port.
    /// </summary>
    internal interface IMozaFrameSink
    {
        // isReply distinguishes an ACK from every other Reliable Stream message (SYN1/SYN2/
        // TRANS/FIN all share the request inner command and carry their own Magic byte) -
        // a tunnel-wrapping sink needs it to set the tunnel's inner reply bit correctly.
        void Send(byte[] wire, bool isReply = false);
    }
}
