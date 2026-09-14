namespace MobiFlightMoza.Protocol
{
    /// <summary>
    /// Fixed wire-level constants from the FCD Display CDC Serial Protocol guide.
    /// </summary>
    internal static class MozaConstants
    {
        // SerialLink framing (guide §3.1/3.2).
        public const byte StartOfFrame = 0x7E;
        public const int ChecksumSeed = 0x0D + 0x7E;
        public const int MaxSerialPayloadLength = 64;

        // 0x43 tunnel (guide §3.3). Outer command's lower 7 bits identify the tunnel;
        // bit7 is the outer reply flag and must not be confused with the inner one.
        public const byte TunnelCommand = 0x43;
        public const byte DevicePairToDevice = 0x12;
        public const byte DevicePairFromDevice = 0x21;
        public const int MaxTunnelInnerPayloadLength = 62; // 63 - 1 byte for the inner command

        // Reliable Stream (guide §4).
        public const byte StreamInnerCommand = 0x7C;
        public const byte StreamAckInnerCommand = 0xFC;
        public const int MaxApplicationChunkLength = 54;

        // Logical service ports (guide §2.1).
        public const ushort ServicePortTelemetry = 9010;
        public const ushort ServicePortSettings = 9020;
        public const ushort ServicePortFileTransfer = 9030;
        public const ushort ServicePortMcduTcp = 9050;
        public const ushort ServicePortMcduUdp = 9051;

        // MCDU geometry (guide §7.1).
        public const int McduRows = 14;
        public const int McduColumns = 24;

        // Fixed startup frames (guide §2.2), sent as-is - never re-derived at runtime.
        public static readonly byte[] RootHandshakeRequest = [0x7E, 0x00, 0x00, 0x12, 0x9D];
        public static readonly byte[] RootHandshakeResponse = [0x7E, 0x00, 0x80, 0x21, 0x2C];
        public static readonly byte[] DeviceInitRequest = [0x7E, 0x01, 0x43, 0x12, 0x00, 0xE1];
        public static readonly byte[] DeviceInitResponse = [0x7E, 0x01, 0xC3, 0x21, 0x80, 0xF0];
    }
}
