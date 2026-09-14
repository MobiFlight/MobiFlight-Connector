namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// One decoded 0x43 tunnel message: inner command, inner reply flag, inner payload.
    /// </summary>
    internal sealed class MozaTunnelMessage
    {
        public byte InnerCommand { get; }
        public bool IsReply { get; }
        public byte[] InnerPayload { get; }

        public MozaTunnelMessage(byte innerCommand, bool isReply, byte[] innerPayload)
        {
            InnerCommand = innerCommand;
            IsReply = isReply;
            InnerPayload = innerPayload;
        }
    }
}
