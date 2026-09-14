namespace MobiFlightMoza.Session
{
    /// <summary>
    /// The write side of the CDC connection. Mirrors how <c>HidReportReceiver</c> reads
    /// from a plain <see cref="System.IO.Stream"/> - this is the seam tests fake instead
    /// of touching a real port.
    /// </summary>
    internal interface IMozaFrameSink
    {
        void Send(byte[] wire);
    }
}
